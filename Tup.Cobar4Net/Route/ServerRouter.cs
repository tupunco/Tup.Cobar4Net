/*
* Copyright 1999-2012 Alibaba Group.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tup.Cobar4Net.Config.Model;
using Tup.Cobar4Net.Config.Model.Rule;
using Tup.Cobar4Net.Parser.Ast;
using Tup.Cobar4Net.Parser.Ast.Expression;
using Tup.Cobar4Net.Parser.Ast.Expression.Comparison;
using Tup.Cobar4Net.Parser.Ast.Expression.Primary;
using Tup.Cobar4Net.Parser.Ast.Stmt;
using Tup.Cobar4Net.Parser.Ast.Stmt.Dal;
using Tup.Cobar4Net.Parser.Ast.Stmt.Dml;
using Tup.Cobar4Net.Parser.Recognizer;
using Tup.Cobar4Net.Parser.Recognizer.Mysql.Syntax;
using Tup.Cobar4Net.Parser.Util;
using Tup.Cobar4Net.Parser.Visitor;
using Tup.Cobar4Net.Route.Hint;
using Tup.Cobar4Net.Route.Visitor;
using Tup.Cobar4Net.Util;

namespace Tup.Cobar4Net.Route
{
    using ColumnValueType = IDictionary<object, ICollection<Pair<IExpression, IAstNode>>>;

    /// <author>xianmao.hexm</author>
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public sealed class ServerRouter
    {
        //private static readonly Logger Logger = Logger.GetLogger(typeof(ServerRouter));

        public static RouteResultset Route(SchemaConfig schema,
                                           string stmt,
                                           string charset,
                                           object info)
        {
            var rrs = new RouteResultset(stmt);
            // 检查是否含有cobar hint
            var prefixIndex = HintRouter.IndexOfPrefix(stmt);
            if (prefixIndex >= 0)
            {
                HintRouter.RouteFromHint(info, schema, rrs, prefixIndex, stmt);
                return rrs;
            }

            // 检查schema是否含有拆分库
            if (schema.IsNoSharding)
            {
                if (schema.IsKeepSqlSchema)
                {
                    var ast = SqlParserDelegate.Parse(stmt, charset ?? MySqlParser.DefaultCharset);
                    var visitor = new PartitionKeyVisitor(schema.Tables);
                    visitor.SetTrimSchema(schema.Name);
                    ast.Accept(visitor);
                    if (visitor.IsSchemaTrimmed())
                    {
                        stmt = GenSql(ast, stmt);
                    }
                }
                var nodes = new RouteResultsetNode[1];
                nodes[0] = new RouteResultsetNode(schema.DataNode, stmt);
                rrs.Nodes = nodes;
                return rrs;
            }

            // 生成和展开AST
            var ast1 = SqlParserDelegate.Parse(stmt, charset ?? MySqlParser.DefaultCharset);
            var visitor1 = new PartitionKeyVisitor(schema.Tables);
            visitor1.SetTrimSchema(schema.IsKeepSqlSchema ? schema.Name : null);
            ast1.Accept(visitor1);
            // 如果sql包含用户自定义的schema，则路由到default节点
            if (schema.IsKeepSqlSchema && visitor1.IsCustomedSchema())
            {
                if (visitor1.IsSchemaTrimmed())
                {
                    stmt = GenSql(ast1, stmt);
                }
                var nodes = new RouteResultsetNode[1];
                nodes[0] = new RouteResultsetNode(schema.DataNode, stmt);
                rrs.Nodes = nodes;
                return rrs;
            }

            // 元数据语句路由
            if (visitor1.IsTableMetaRead())
            {
                MetaRouter.RouteForTableMeta(rrs, schema, ast1, visitor1, stmt);
                if (visitor1.IsNeedRewriteField())
                {
                    rrs.Flag = RouteResultset.RewriteField;
                }
                return rrs;
            }

            // 匹配规则
            TableConfig matchedTable = null;
            RuleConfig rule = null;
            IDictionary<string, IList<object>> columnValues = null;
            var astExt = visitor1.GetColumnValue();
            var tables = schema.Tables;
            foreach (var e in astExt)
            {
                var col2Val = e.Value;
                var tc = tables.GetValue(e.Key);
                if (tc == null)
                {
                    continue;
                }
                if (matchedTable == null)
                {
                    matchedTable = tc;
                }
                if (col2Val == null || col2Val.IsEmpty())
                {
                    continue;
                }
                var tr = tc.Rule;
                if (tr != null)
                {
                    foreach (var rc in tr.Rules)
                    {
                        var match = true;
                        foreach (var ruleColumn in rc.Columns)
                        {
                            match &= col2Val.ContainsKey(ruleColumn);
                        }

                        if (match)
                        {
                            columnValues = col2Val;
                            rule = rc;
                            matchedTable = tc;
                            goto ft_break;
                        }
                    }
                }
            }
            ft_break:
            ;

            // 规则匹配处理，表级别和列级别。
            if (matchedTable == null)
            {
                var sql = visitor1.IsSchemaTrimmed() ? GenSql(ast1, stmt) : stmt;
                var rn = new RouteResultsetNode[1];
                if (string.Empty.Equals(schema.DataNode) && IsSystemReadSql(ast1))
                {
                    rn[0] = new RouteResultsetNode(schema.RandomDataNode, sql);
                }
                else
                {
                    rn[0] = new RouteResultsetNode(schema.DataNode, sql);
                }
                rrs.Nodes = rn;
                return rrs;
            }
            if (rule == null)
            {
                if (matchedTable.IsRuleRequired)
                {
                    throw new ArgumentException(string.Format("route rule for table {0} is required: {1}",
                        matchedTable.Name, stmt));
                }
                var dataNodes = matchedTable.DataNodes;
                var sql = visitor1.IsSchemaTrimmed() ? GenSql(ast1, stmt) : stmt;
                var rn = new RouteResultsetNode[dataNodes.Length];
                for (var i = 0; i < dataNodes.Length; ++i)
                {
                    rn[i] = new RouteResultsetNode(dataNodes[i], sql);
                }
                rrs.Nodes = rn;
                SetGroupFlagAndLimit(rrs, visitor1);
                return rrs;
            }

            // 规则计算
            ValidateAst(ast1, matchedTable, rule, visitor1);
            var dnMap = RuleCalculate(matchedTable, rule, columnValues);
            if (dnMap == null || dnMap.IsEmpty())
            {
                throw new ArgumentException("No target dataNode for rule " + rule);
            }

            // 判断路由结果是单库还是多库
            if (dnMap.Count == 1)
            {
                var dataNode = matchedTable.DataNodes[dnMap.Keys.FirstOrDefault()];
                //string dataNode = matchedTable.GetDataNodes()[dnMap.Keys.GetEnumerator().Current];
                var sql = visitor1.IsSchemaTrimmed() ? GenSql(ast1, stmt) : stmt;
                var rn = new RouteResultsetNode[1];
                rn[0] = new RouteResultsetNode(dataNode, sql);
                rrs.Nodes = rn;
            }
            else
            {
                var rn = new RouteResultsetNode[dnMap.Count];
                if (ast1 is DmlInsertReplaceStatement)
                {
                    var ir = (DmlInsertReplaceStatement)ast1;
                    DispatchInsertReplace(rn, ir, rule.Columns, dnMap, matchedTable, stmt, visitor1);
                }
                else
                {
                    DispatchWhereBasedStmt(rn, ast1, rule.Columns, dnMap, matchedTable, stmt, visitor1);
                }
                rrs.Nodes = rn;
                SetGroupFlagAndLimit(rrs, visitor1);
            }
            return rrs;
        }

        private static int[] CalcDataNodeIndexesByFunction(IRuleAlgorithm algorithm,
                                                           IDictionary<string, object> parameter)
        {
            return Number.ToInt32(algorithm.Calculate(parameter.ToDictionary(x => (object)x.Key, y => y.Value)));
        }

        private static bool Equals(string str1, string str2)
        {
            if (str1 == null)
            {
                return str2 == null;
            }
            return str1.Equals(str2);
        }

        /// <summary>
        /// </summary>
        /// <param name="ast"></param>
        /// <param name="tc"></param>
        /// <param name="rule"></param>
        /// <param name="visitor"></param>
        private static void ValidateAst(ISqlStatement ast,
                                        TableConfig tc,
                                        RuleConfig rule,
                                        PartitionKeyVisitor visitor)
        {
            if (!(ast is DmlUpdateStatement))
                return;

            IList<Identifier> columns = null;
            var ruleCols = rule.Columns;
            var update = (DmlUpdateStatement)ast;
            foreach (var pair in update.Values)
            {
                foreach (var ruleCol in ruleCols)
                {
                    if (pair.Key.IdTextUpUnescape == ruleCol)
                    {
                        if (columns == null)
                        {
                            columns = new List<Identifier>(ruleCols.Count);
                        }
                        columns.Add(pair.Key);
                    }
                }
            }

            if (columns == null)
                return;

            var alias = visitor.GetTableAlias();
            foreach (var column in columns)
            {
                var table = column.GetLevelUnescapeUpName(2);
                table = alias.GetValue(table);
                if (table != null && table.Equals(tc.Name))
                {
                    throw new NotSupportedException("partition key cannot be changed");
                }
            }
        }

        private static bool IsSystemReadSql(ISqlStatement ast)
        {
            if (ast is DalShowStatement)
                return true;

            DmlSelectStatement select = null;
            if (ast is DmlSelectStatement)
            {
                select = (DmlSelectStatement)ast;
            }
            else if (ast is DmlSelectUnionStatement)
            {
                var union = (DmlSelectUnionStatement)ast;
                if (union.SelectStmtList.Count == 1)
                {
                    select = union.SelectStmtList[0];
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            return @select.Tables == null;
        }

        private static void SetGroupFlagAndLimit(RouteResultset rrs, PartitionKeyVisitor visitor)
        {
            rrs.LimitSize = visitor.GetLimitSize();
            switch (visitor.GetGroupFuncType())
            {
                case PartitionKeyVisitor.GroupSum:
                {
                    rrs.Flag = RouteResultset.SumFlag;
                    break;
                }

                case PartitionKeyVisitor.GroupMax:
                {
                    rrs.Flag = RouteResultset.MaxFlag;
                    break;
                }

                case PartitionKeyVisitor.GroupMin:
                {
                    rrs.Flag = RouteResultset.MinFlag;
                    break;
                }
            }
        }

        /// <returns>dataNodeIndex -&gt; [partitionKeysValueTuple+]</returns>
        private static IDictionary<int, IList<object[]>> RuleCalculate(TableConfig matchedTable,
                                                                       RuleConfig rule,
                                                                       IDictionary<string, IList<object>> columnValues)
        {
            var map = new Dictionary<int, IList<object[]>>();
            var algorithm = rule.RuleAlgorithm;
            var cols = rule.Columns;
            var parameter = new Dictionary<string, object>(cols.Count);
            var colsValIter = new List<IList<object>>(columnValues.Count);
            var columnCount = 0;
            foreach (var rc in cols)
            {
                var list = columnValues.GetValue(rc);
                if (list == null)
                {
                    var msg = "route err: rule column " + rc + " dosn't exist in extract: " + columnValues;
                    throw new ArgumentException(msg);
                }
                if (columnCount <= 0)
                    columnCount = list.Count;

                colsValIter.Add(list);
            }

            try
            {
                var countIndex = 0;
                while (countIndex < columnCount)
                {
                    var tuple = new object[cols.Count];
                    for (int i = 0, len = cols.Count; i < len; ++i)
                    {
                        var value = colsValIter[i][countIndex];
                        tuple[i] = value;
                        parameter[cols[i]] = value;
                    }

                    var dataNodeIndexes = CalcDataNodeIndexesByFunction(algorithm, parameter);
                    for (var i1 = 0; i1 < dataNodeIndexes.Length; ++i1)
                    {
                        var dataNodeIndex = dataNodeIndexes[i1];
                        var list = map.GetValue(dataNodeIndex);
                        if (list == null)
                        {
                            list = new List<object[]>();
                            map[dataNodeIndex] = list;
                        }
                        list.Add(tuple);
                    }

                    countIndex++;
                }
            }
            catch (Exception e)
            {
                var msg = "route err: different rule columns should have same value number:  " + columnValues;
                throw new ArgumentException(msg, e);
            }
            return map;
        }

        private static void DispatchWhereBasedStmt(RouteResultsetNode[] rn,
                                                   ISqlStatement stmtAST,
                                                   IList<string> ruleColumns,
                                                   IDictionary<int, IList<object[]>> dataNodeMap,
                                                   TableConfig matchedTable,
                                                   string originalSQL,
                                                   PartitionKeyVisitor visitor)
        {
            // [perf tag] 11.617 us: sharding multivalue
            if (ruleColumns.Count > 1)
            {
                string sql;
                if (visitor.IsSchemaTrimmed())
                    sql = GenSql(stmtAST, originalSQL);
                else
                    sql = originalSQL;

                var i = -1;
                foreach (var dataNodeId in dataNodeMap.Keys)
                {
                    var dataNode = matchedTable.DataNodes[dataNodeId];
                    rn[++i] = new RouteResultsetNode(dataNode, sql);
                }
                return;
            }

            var table = matchedTable.Name;
            var columnIndex = visitor.GetColumnIndex(table);
            var valueMap = columnIndex.GetValue(ruleColumns[0]);
            ReplacePartitionKeyOperand(columnIndex, ruleColumns);
            var unreplacedInExpr = new Dictionary<InExpression, ICollection<IExpression>>(1);
            var unreplacedSingleExprs = new HashSet<IReplacableExpression>();
            // [perf tag] 12.2755 us: sharding multivalue
            var nodeId = -1;
            foreach (var en in dataNodeMap)
            {
                var tuples = en.Value;
                unreplacedSingleExprs.Clear();
                unreplacedInExpr.Clear();

                foreach (var tuple in tuples)
                {
                    var value = tuple[0];
                    var indexedExpressionPair = GetExpressionSet(valueMap, value);
                    foreach (var pair in indexedExpressionPair)
                    {
                        var expr = pair.Key;
                        var parent = (InExpression)pair.Value;
                        if (PartitionKeyVisitor.IsPartitionKeyOperandSingle(expr, parent))
                        {
                            unreplacedSingleExprs.Add((IReplacableExpression)expr);
                        }
                        else if (PartitionKeyVisitor.IsPartitionKeyOperandIn(expr, parent))
                        {
                            var newInSet = unreplacedInExpr.GetValue(parent);
                            if (newInSet == null)
                            {
                                newInSet = new HashSet<IExpression>();
                                unreplacedInExpr[parent] = newInSet;
                            }
                            newInSet.Add(expr);
                        }
                    }
                }
                // [perf tag] 15.3745 us: sharding multivalue
                foreach (var expr1 in unreplacedSingleExprs)
                {
                    expr1.ClearReplaceExpr();
                }

                foreach (var entemp in unreplacedInExpr)
                {
                    var @in = entemp.Key;
                    var set = entemp.Value;
                    if (set == null || set.IsEmpty())
                    {
                        @in.ReplaceExpr = ReplacableExpressionConstants.BoolFalse;
                    }
                    else
                    {
                        @in.ClearReplaceExpr();
                        var inlist = @in.GetInExpressionList();
                        if (inlist != null)
                        {
                            inlist.ReplaceExpr = new List<IExpression>(set);
                        }
                    }
                }

                // [perf tag] 16.506 us: sharding multivalue
                var sql = GenSql(stmtAST, originalSQL);
                // [perf tag] 21.3425 us: sharding multivalue
                var dataNodeName = matchedTable.DataNodes[en.Key];
                rn[++nodeId] = new RouteResultsetNode(dataNodeName, sql);
                foreach (var expr2 in unreplacedSingleExprs)
                {
                    expr2.ReplaceExpr = ReplacableExpressionConstants.BoolFalse;
                }

                foreach (var in1 in unreplacedInExpr.Keys)
                {
                    in1.ReplaceExpr = ReplacableExpressionConstants.BoolFalse;
                    var list = in1.GetInExpressionList();
                    if (list != null)
                    {
                        list.ClearReplaceExpr();
                    }
                }
            }
        }

        // [perf tag] 22.0965 us: sharding multivalue
        private static void ReplacePartitionKeyOperand(IDictionary<string, ColumnValueType> index, IList<string> cols)
        {
            if (cols == null)
                return;

            foreach (var col in cols)
            {
                var map = index.GetValue(col);
                if (map == null)
                    continue;

                foreach (var set in map.Values)
                {
                    if (set == null)
                        continue;

                    foreach (var p in set)
                    {
                        var expr = p.Key;
                        var parent = p.Value;
                        if (PartitionKeyVisitor.IsPartitionKeyOperandSingle(expr, parent))
                        {
                            ((IReplacableExpression)expr).ReplaceExpr = ReplacableExpressionConstants.BoolFalse;
                        }
                        else if (PartitionKeyVisitor.IsPartitionKeyOperandIn(expr, parent))
                        {
                            ((IReplacableExpression)parent).ReplaceExpr = ReplacableExpressionConstants.BoolFalse;
                        }
                    }
                }
            }
        }

        private static void DispatchInsertReplace(RouteResultsetNode[] rn,
                                                  DmlInsertReplaceStatement stmt,
                                                  IList<string> ruleColumns,
                                                  IDictionary<int, IList<object[]>> dataNodeMap,
                                                  TableConfig matchedTable,
                                                  string originalSql,
                                                  PartitionKeyVisitor visitor)
        {
            if (stmt.Select != null)
            {
                DispatchWhereBasedStmt(rn, stmt, ruleColumns, dataNodeMap, matchedTable, originalSql, visitor);
                return;
            }

            var colsIndex = visitor.GetColumnIndex(stmt.Table.IdTextUpUnescape);
            if (colsIndex == null || colsIndex.IsEmpty())
                throw new ArgumentException("columns index is empty: " + originalSql);

            var colsIndexList = new List<ColumnValueType>(ruleColumns.Count);
            for (int i = 0, len = ruleColumns.Count; i < len; ++i)
            {
                colsIndexList.Add(colsIndex[ruleColumns[i]]);
            }

            var dataNodeId = -1;
            foreach (var en in dataNodeMap)
            {
                var tuples = en.Value;
                var replaceRowList = new HashSet<RowExpression>();
                foreach (var tuple in tuples)
                {
                    ICollection<Pair<IExpression, IAstNode>> tupleExprs = null;
                    for (var i1 = 0; i1 < tuple.Length; ++i1)
                    {
                        var valueMap = colsIndexList[i1];
                        var value = tuple[i1];
                        var set = GetExpressionSet(valueMap, value);
                        tupleExprs = CollectionUtil.IntersectSet(tupleExprs, set);
                    }

                    if (tupleExprs == null || tupleExprs.IsEmpty())
                        throw new ArgumentException(
                            string.Format("route: empty expression list for insertReplace stmt: {0}", originalSql));

                    foreach (var p in tupleExprs)
                    {
                        if (p.Value == stmt && p.Key is RowExpression)
                        {
                            replaceRowList.Add((RowExpression)p.Key);
                        }
                    }
                }

                stmt.ReplaceRowList = new List<RowExpression>(replaceRowList);
                var sql = GenSql(stmt, originalSql);
                stmt.ClearReplaceRowList();

                var dataNodeName = matchedTable.DataNodes[en.Key];
                rn[++dataNodeId] = new RouteResultsetNode(dataNodeName, sql);
            }
        }

        private static ICollection<Pair<IExpression, IAstNode>> GetExpressionSet(ColumnValueType map, object value)
        {
            if (map == null || map.IsEmpty())
                return new HashSet<Pair<IExpression, IAstNode>>();

            var set = map.GetValue(value);
            return set ?? new HashSet<Pair<IExpression, IAstNode>>();
        }

        private static string GenSql(ISqlStatement ast, string orginalSql)
        {
            var s = new StringBuilder();
            ast.Accept(new MySqlOutputAstVisitor(s));
            return s.ToString();
        }

        private class HintRouter
        {
            public static int IndexOfPrefix(string sql)
            {
                var i = 0;
                for (; i < sql.Length; ++i)
                {
                    switch (sql[i])
                    {
                        case ' ':
                        case '\t':
                        case '\r':
                        case '\n':
                        {
                            continue;
                        }
                    }
                    break;
                }
                if (sql.Substring(i).StartsWith(CobarHint.CobarHintPrefix, StringComparison.Ordinal))
                {
                    return i;
                }
                return -1;
            }

            /// <exception cref="System.SqlSyntaxErrorException" />
            public static void RouteFromHint(object frontConn,
                                             SchemaConfig schema,
                                             RouteResultset rrs,
                                             int prefixIndex,
                                             string sql)
            {
                var hint = CobarHint.ParserCobarHint(sql, prefixIndex);
                var outputSql = hint.OutputSql;
                var replica = hint.Replica;
                var table = hint.Table;
                var dataNodes = hint.DataNodes;
                var partitionOperand = hint.PartitionOperand;
                TableConfig tableConfig = null;
                if (table == null
                    || schema.Tables == null
                    || (tableConfig = schema.Tables.GetValue(table)) == null)
                {
                    // table not indicated
                    var nodes = new RouteResultsetNode[1];
                    rrs.Nodes = nodes;
                    if (dataNodes != null && !dataNodes.IsEmpty())
                    {
                        var replicaIndex = dataNodes[0].Value;
                        if (replicaIndex >= 0 && RouteResultsetNode.DefaultReplicaIndex != replicaIndex)
                        {
                            // replica index indicated in dataNodes references
                            nodes[0] = new RouteResultsetNode(schema.DataNode, replicaIndex, outputSql);
                            LogExplicitReplicaSet(frontConn, sql, rrs);
                            return;
                        }
                    }
                    nodes[0] = new RouteResultsetNode(schema.DataNode, replica, outputSql);
                    if (replica != RouteResultsetNode.DefaultReplicaIndex)
                    {
                        LogExplicitReplicaSet(frontConn, sql, rrs);
                    }
                    return;
                }

                if (dataNodes != null && !dataNodes.IsEmpty())
                {
                    var nodes = new RouteResultsetNode[dataNodes.Count];
                    rrs.Nodes = nodes;
                    var i = 0;
                    var replicaSet = false;
                    foreach (var pair in dataNodes)
                    {
                        var dataNodeName = tableConfig.DataNodes[pair.Key];
                        var replicaIndex = dataNodes[i].Value;
                        if (replicaIndex >= 0 && RouteResultsetNode.DefaultReplicaIndex != replicaIndex)
                        {
                            replicaSet = true;
                            nodes[i] = new RouteResultsetNode(dataNodeName, replicaIndex, outputSql);
                        }
                        else
                        {
                            replicaSet = replicaSet || (replica != RouteResultsetNode.DefaultReplicaIndex);
                            nodes[i] = new RouteResultsetNode(dataNodeName, replica, outputSql);
                        }
                        ++i;
                    }
                    if (replicaSet)
                    {
                        LogExplicitReplicaSet(frontConn, sql, rrs);
                    }
                    return;
                }

                if (partitionOperand == null)
                {
                    var tableDataNodes = tableConfig.DataNodes;
                    var nodes = new RouteResultsetNode[tableDataNodes.Length];
                    rrs.Nodes = nodes;
                    for (var i = 0; i < nodes.Length; ++i)
                    {
                        nodes[i] = new RouteResultsetNode(tableDataNodes[i], replica, outputSql);
                    }
                    return;
                }

                var cols = partitionOperand.Key;
                var vals = partitionOperand.Value;
                if (cols == null || vals == null)
                {
                    throw new SqlSyntaxErrorException("${partitionOperand} is invalid: " + sql);
                }
                RuleConfig rule = null;
                var tr = tableConfig.Rule;
                var rules = tr == null ? null : tr.Rules;
                if (rules != null)
                {
                    foreach (var r in rules)
                    {
                        var ruleCols = r.Columns;
                        var match = true;
                        foreach (var ruleCol in ruleCols)
                        {
                            match &= cols.Contains(ruleCol);
                        }
                        if (match)
                        {
                            rule = r;
                            break;
                        }
                    }
                }

                var tableDataNodes1 = tableConfig.DataNodes;
                if (rule == null)
                {
                    var nodes = new RouteResultsetNode[tableDataNodes1.Length];
                    rrs.Nodes = nodes;
                    var replicaSet = false;
                    for (var i = 0; i < tableDataNodes1.Length; ++i)
                    {
                        replicaSet = replicaSet || (replica != RouteResultsetNode.DefaultReplicaIndex);
                        nodes[i] = new RouteResultsetNode(tableDataNodes1[i], replica, outputSql);
                    }
                    if (replicaSet)
                    {
                        LogExplicitReplicaSet(frontConn, sql, rrs);
                    }
                    return;
                }

                var destDataNodes = CalcHintDataNodes(rule, cols, vals, tableDataNodes1);
                var nodes1 = new RouteResultsetNode[destDataNodes.Count];
                rrs.Nodes = nodes1;
                var i1 = 0;
                var replicaSet1 = false;
                foreach (var dataNode in destDataNodes)
                {
                    replicaSet1 = replicaSet1 || (replica != RouteResultsetNode.DefaultReplicaIndex);
                    nodes1[i1++] = new RouteResultsetNode(dataNode, replica, outputSql);
                }
                if (replicaSet1)
                {
                    LogExplicitReplicaSet(frontConn, sql, rrs);
                }
            }

            private static ICollection<string> CalcHintDataNodes(RuleConfig rule,
                                                                 string[] cols,
                                                                 object[][] vals,
                                                                 string[] dataNodes)
            {
                ICollection<string> destDataNodes = new HashSet<string>();
                var parameter = new Dictionary<string, object>(cols.Length);
                foreach (var val in vals)
                {
                    for (var i = 0; i < cols.Length; ++i)
                    {
                        parameter[cols[i]] = val[i];
                    }
                    var dataNodeIndexes = CalcDataNodeIndexesByFunction(rule.RuleAlgorithm, parameter);
                    foreach (var index in dataNodeIndexes)
                    {
                        destDataNodes.Add(dataNodes[index]);
                    }
                }
                return destDataNodes;
            }

            private static void LogExplicitReplicaSet(object frontConn, string sql, RouteResultset rrs)
            {
                //LogExplicitReplicaSet
                //if (frontConn != null && Logger.IsInfoEnabled())
                //{
                //    StringBuilder s = new StringBuilder();
                //    s.Append(frontConn).Append("Explicit data node replica set from, sql=[");
                //    s.Append(sql).Append(']');
                //    Logger.Info(s.ToString());
                //}
            }
        }

        private class MetaRouter
        {
            public static void RouteForTableMeta(RouteResultset rrs,
                                                 SchemaConfig schema,
                                                 ISqlStatement ast,
                                                 PartitionKeyVisitor visitor,
                                                 string stmt)
            {
                var sql = stmt;
                if (visitor.IsSchemaTrimmed())
                    sql = GenSql(ast, stmt);

                var tables = visitor.GetMetaReadTable();
                if (tables == null)
                    throw new ArgumentException(string.Format("route err: tables[] is null for meta read table: {0}",
                        stmt));

                string[] dataNodes;
                if (tables.Length <= 0)
                {
                    dataNodes = schema.MetaDataNodes;
                }
                else
                {
                    if (tables.Length == 1)
                    {
                        dataNodes = new string[1];
                        dataNodes[0] = GetMetaReadDataNode(schema, tables[0]);
                    }
                    else
                    {
                        ICollection<string> dataNodeSet = new HashSet<string>();
                        foreach (var table in tables)
                        {
                            var dataNode = GetMetaReadDataNode(schema, table);
                            dataNodeSet.Add(dataNode);
                        }
                        dataNodes = dataNodeSet.ToArray();
                        //dataNodes = new string[dataNodeSet.Count];
                        //IEnumerator<string> iter = dataNodeSet.GetEnumerator();
                        //for (int i = 0; i < dataNodes.Length; ++i)
                        //{
                        //    dataNodes[i] = iter.Current;
                        //}
                    }
                }

                var nodes = new RouteResultsetNode[dataNodes.Length];
                rrs.Nodes = nodes;
                for (var i1 = 0; i1 < dataNodes.Length; ++i1)
                {
                    nodes[i1] = new RouteResultsetNode(dataNodes[i1], sql);
                }
            }

            private static string GetMetaReadDataNode(SchemaConfig schema, string table)
            {
                var dataNode = schema.DataNode;
                var tables = schema.Tables;
                TableConfig tc;
                if (tables != null
                    && table != null //TODO GetMetaReadDataNode table != null
                    && (tc = tables.GetValue(table)) != null)
                {
                    var dn = tc.DataNodes;
                    if (dn != null && dn.Length > 0)
                    {
                        dataNode = dn[0];
                    }
                }
                return dataNode;
            }
        }
    }
}