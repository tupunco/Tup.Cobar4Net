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

//using Org.Apache.Log4j;
using Tup.Cobar4Net.Config.Model;
using Tup.Cobar4Net.Config.Model.Rule;
using Tup.Cobar4Net.Parser.Ast;
using Tup.Cobar4Net.Parser.Ast.Expression;
using Tup.Cobar4Net.Parser.Ast.Expression.Comparison;
using Tup.Cobar4Net.Parser.Ast.Expression.Misc;
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
using Expr = Tup.Cobar4Net.Parser.Ast.Expression.Expression;

namespace Tup.Cobar4Net.Route
{
    using ColumnValueType = IDictionary<object, ICollection<Pair<Expr, ASTNode>>>;

    /// <author>xianmao.hexm</author>
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public sealed class ServerRouter
    {
        //private static readonly Logger Logger = Logger.GetLogger(typeof(ServerRouter));

        /// <exception cref="System.Data.Sql.SQLNonTransientException"/>
        public static RouteResultset Route(SchemaConfig schema,
            string stmt,
            string charset,
            object info)
        {
            var rrs = new RouteResultset(stmt);
            // 检查是否含有cobar hint
            int prefixIndex = HintRouter.IndexOfPrefix(stmt);
            if (prefixIndex >= 0)
            {
                HintRouter.RouteFromHint(info, schema, rrs, prefixIndex, stmt);
                return rrs;
            }

            // 检查schema是否含有拆分库
            if (schema.IsNoSharding())
            {
                if (schema.IsKeepSqlSchema())
                {
                    var ast = SQLParserDelegate.Parse(stmt, charset == null
                                                            ? MySQLParser.DefaultCharset
                                                            : charset);
                    var visitor = new PartitionKeyVisitor(schema.GetTables());
                    visitor.SetTrimSchema(schema.GetName());
                    ast.Accept(visitor);
                    if (visitor.IsSchemaTrimmed())
                    {
                        stmt = GenSQL(ast, stmt);
                    }
                }
                RouteResultsetNode[] nodes = new RouteResultsetNode[1];
                nodes[0] = new RouteResultsetNode(schema.GetDataNode(), stmt);
                rrs.SetNodes(nodes);
                return rrs;
            }

            // 生成和展开AST
            var ast_1 = SQLParserDelegate.Parse(stmt, charset == null
                                                                ? MySQLParser.DefaultCharset
                                                                : charset);
            var visitor_1 = new PartitionKeyVisitor(schema.GetTables());
            visitor_1.SetTrimSchema(schema.IsKeepSqlSchema() ? schema.GetName() : null);
            ast_1.Accept(visitor_1);
            // 如果sql包含用户自定义的schema，则路由到default节点
            if (schema.IsKeepSqlSchema() && visitor_1.IsCustomedSchema())
            {
                if (visitor_1.IsSchemaTrimmed())
                {
                    stmt = GenSQL(ast_1, stmt);
                }
                RouteResultsetNode[] nodes = new RouteResultsetNode[1];
                nodes[0] = new RouteResultsetNode(schema.GetDataNode(), stmt);
                rrs.SetNodes(nodes);
                return rrs;
            }

            // 元数据语句路由
            if (visitor_1.IsTableMetaRead())
            {
                MetaRouter.RouteForTableMeta(rrs, schema, ast_1, visitor_1, stmt);
                if (visitor_1.IsNeedRewriteField())
                {
                    rrs.SetFlag(RouteResultset.RewriteField);
                }
                return rrs;
            }

            // 匹配规则
            TableConfig matchedTable = null;
            RuleConfig rule = null;
            IDictionary<string, IList<object>> columnValues = null;
            var astExt = visitor_1.GetColumnValue();
            var tables = schema.GetTables();
            foreach (var e in astExt)
            {
                var col2Val = e.Value;
                TableConfig tc = tables.GetValue(e.Key);
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
                TableRuleConfig tr = tc.GetRule();
                if (tr != null)
                {
                    foreach (RuleConfig rc in tr.GetRules())
                    {
                        bool match = true;
                        foreach (string ruleColumn in rc.GetColumns())
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
        ft_break:;

            // 规则匹配处理，表级别和列级别。
            if (matchedTable == null)
            {
                string sql = visitor_1.IsSchemaTrimmed() ? GenSQL(ast_1, stmt) : stmt;
                RouteResultsetNode[] rn = new RouteResultsetNode[1];
                if (string.Empty.Equals(schema.GetDataNode()) && IsSystemReadSQL(ast_1))
                {
                    rn[0] = new RouteResultsetNode(schema.GetRandomDataNode(), sql);
                }
                else
                {
                    rn[0] = new RouteResultsetNode(schema.GetDataNode(), sql);
                }
                rrs.SetNodes(rn);
                return rrs;
            }
            if (rule == null)
            {
                if (matchedTable.IsRuleRequired())
                {
                    throw new ArgumentException("route rule for table " + matchedTable.GetName() + " is required: "
                         + stmt);
                }
                string[] dataNodes = matchedTable.GetDataNodes();
                string sql = visitor_1.IsSchemaTrimmed() ? GenSQL(ast_1, stmt) : stmt;
                RouteResultsetNode[] rn = new RouteResultsetNode[dataNodes.Length];
                for (int i = 0; i < dataNodes.Length; ++i)
                {
                    rn[i] = new RouteResultsetNode(dataNodes[i], sql);
                }
                rrs.SetNodes(rn);
                SetGroupFlagAndLimit(rrs, visitor_1);
                return rrs;
            }

            // 规则计算
            ValidateAST(ast_1, matchedTable, rule, visitor_1);
            var dnMap = RuleCalculate(matchedTable, rule, columnValues);
            if (dnMap == null || dnMap.IsEmpty())
            {
                throw new ArgumentException("No target dataNode for rule " + rule);
            }
            // 判断路由结果是单库还是多库
            if (dnMap.Count == 1)
            {
                string dataNode = matchedTable.GetDataNodes()[dnMap.Keys.FirstOrDefault()];
                //string dataNode = matchedTable.GetDataNodes()[dnMap.Keys.GetEnumerator().Current];
                string sql = visitor_1.IsSchemaTrimmed() ? GenSQL(ast_1, stmt) : stmt;
                RouteResultsetNode[] rn = new RouteResultsetNode[1];
                rn[0] = new RouteResultsetNode(dataNode, sql);
                rrs.SetNodes(rn);
            }
            else
            {
                RouteResultsetNode[] rn = new RouteResultsetNode[dnMap.Count];
                if (ast_1 is DMLInsertReplaceStatement)
                {
                    DMLInsertReplaceStatement ir = (DMLInsertReplaceStatement)ast_1;
                    DispatchInsertReplace(rn, ir, rule.GetColumns(), dnMap, matchedTable, stmt, visitor_1);
                }
                else
                {
                    DispatchWhereBasedStmt(rn, ast_1, rule.GetColumns(), dnMap, matchedTable, stmt, visitor_1);
                }
                rrs.SetNodes(rn);
                SetGroupFlagAndLimit(rrs, visitor_1);
            }
            return rrs;
        }

        private class HintRouter
        {
            public static int IndexOfPrefix(string sql)
            {
                int i = 0;
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
                else
                {
                    return -1;
                }
            }

            /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
            public static void RouteFromHint(object frontConn,
                SchemaConfig schema,
                RouteResultset rrs,
                int prefixIndex,
                string sql)
            {
                CobarHint hint = CobarHint.ParserCobarHint(sql, prefixIndex);
                string outputSql = hint.GetOutputSql();
                int replica = hint.GetReplica();
                string table = hint.GetTable();
                IList<Pair<int, int>> dataNodes = hint.GetDataNodes();
                Pair<string[], object[][]> partitionOperand = hint.GetPartitionOperand();
                TableConfig tableConfig = null;
                if (table == null
                    || schema.GetTables() == null
                    || (tableConfig = schema.GetTables().GetValue(table)) == null)
                {
                    // table not indicated
                    RouteResultsetNode[] nodes = new RouteResultsetNode[1];
                    rrs.SetNodes(nodes);
                    if (dataNodes != null && !dataNodes.IsEmpty())
                    {
                        int replicaIndex = dataNodes[0].GetValue();
                        if (replicaIndex >= 0 && RouteResultsetNode.DefaultReplicaIndex != replicaIndex)
                        {
                            // replica index indicated in dataNodes references
                            nodes[0] = new RouteResultsetNode(schema.GetDataNode(), replicaIndex, outputSql);
                            LogExplicitReplicaSet(frontConn, sql, rrs);
                            return;
                        }
                    }
                    nodes[0] = new RouteResultsetNode(schema.GetDataNode(), replica, outputSql);
                    if (replica != RouteResultsetNode.DefaultReplicaIndex)
                    {
                        LogExplicitReplicaSet(frontConn, sql, rrs);
                    }
                    return;
                }
                if (dataNodes != null && !dataNodes.IsEmpty())
                {
                    RouteResultsetNode[] nodes = new RouteResultsetNode[dataNodes.Count];
                    rrs.SetNodes(nodes);
                    int i = 0;
                    bool replicaSet = false;
                    foreach (Pair<int, int> pair in dataNodes)
                    {
                        string dataNodeName = tableConfig.GetDataNodes()[pair.GetKey()];
                        int replicaIndex = dataNodes[i].GetValue();
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
                    string[] tableDataNodes = tableConfig.GetDataNodes();
                    RouteResultsetNode[] nodes = new RouteResultsetNode[tableDataNodes.Length];
                    rrs.SetNodes(nodes);
                    for (int i = 0; i < nodes.Length; ++i)
                    {
                        nodes[i] = new RouteResultsetNode(tableDataNodes[i], replica, outputSql);
                    }
                    return;
                }
                string[] cols = partitionOperand.GetKey();
                object[][] vals = partitionOperand.GetValue();
                if (cols == null || vals == null)
                {
                    throw new SQLSyntaxErrorException("${partitionOperand} is invalid: " + sql);
                }
                RuleConfig rule = null;
                TableRuleConfig tr = tableConfig.GetRule();
                IList<RuleConfig> rules = tr == null ? null : tr.GetRules();
                if (rules != null)
                {
                    foreach (RuleConfig r in rules)
                    {
                        IList<string> ruleCols = r.GetColumns();
                        bool match = true;
                        foreach (string ruleCol in ruleCols)
                        {
                            match &= ArrayUtil.Contains(cols, ruleCol);
                        }
                        if (match)
                        {
                            rule = r;
                            break;
                        }
                    }
                }
                string[] tableDataNodes_1 = tableConfig.GetDataNodes();
                if (rule == null)
                {
                    RouteResultsetNode[] nodes = new RouteResultsetNode[tableDataNodes_1.Length];
                    rrs.SetNodes(nodes);
                    bool replicaSet = false;
                    for (int i = 0; i < tableDataNodes_1.Length; ++i)
                    {
                        replicaSet = replicaSet || (replica != RouteResultsetNode.DefaultReplicaIndex);
                        nodes[i] = new RouteResultsetNode(tableDataNodes_1[i], replica, outputSql);
                    }
                    if (replicaSet)
                    {
                        LogExplicitReplicaSet(frontConn, sql, rrs);
                    }
                    return;
                }
                var destDataNodes = CalcHintDataNodes(rule, cols, vals, tableDataNodes_1);
                RouteResultsetNode[] nodes_1 = new RouteResultsetNode[destDataNodes.Count];
                rrs.SetNodes(nodes_1);
                int i_1 = 0;
                bool replicaSet_1 = false;
                foreach (string dataNode in destDataNodes)
                {
                    replicaSet_1 = replicaSet_1 || (replica != RouteResultsetNode.DefaultReplicaIndex);
                    nodes_1[i_1++] = new RouteResultsetNode(dataNode, replica, outputSql);
                }
                if (replicaSet_1)
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
                foreach (object[] val in vals)
                {
                    for (int i = 0; i < cols.Length; ++i)
                    {
                        parameter[cols[i]] = val[i];
                    }
                    int[] dataNodeIndexes = CalcDataNodeIndexesByFunction(rule.GetRuleAlgorithm(), parameter);
                    foreach (int index in dataNodeIndexes)
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
                SQLStatement ast,
                PartitionKeyVisitor visitor,
                string stmt)
            {
                string sql = stmt;
                if (visitor.IsSchemaTrimmed())
                {
                    sql = GenSQL(ast, stmt);
                }
                string[] tables = visitor.GetMetaReadTable();
                if (tables == null)
                {
                    throw new ArgumentException("route err: tables[] is null for meta read table: " +
                         stmt);
                }
                string[] dataNodes;
                if (tables.Length <= 0)
                {
                    dataNodes = schema.GetMetaDataNodes();
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
                        foreach (string table in tables)
                        {
                            string dataNode = GetMetaReadDataNode(schema, table);
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
                RouteResultsetNode[] nodes = new RouteResultsetNode[dataNodes.Length];
                rrs.SetNodes(nodes);
                for (int i_1 = 0; i_1 < dataNodes.Length; ++i_1)
                {
                    nodes[i_1] = new RouteResultsetNode(dataNodes[i_1], sql);
                }
            }

            private static string GetMetaReadDataNode(SchemaConfig schema, string table)
            {
                string dataNode = schema.GetDataNode();
                var tables = schema.GetTables();
                TableConfig tc;
                if (tables != null
                    && table != null //TODO GetMetaReadDataNode table != null
                    && (tc = tables.GetValue(table)) != null)
                {
                    string[] dn = tc.GetDataNodes();
                    if (dn != null && dn.Length > 0)
                    {
                        dataNode = dn[0];
                    }
                }
                return dataNode;
            }
        }

        private static int[] CalcDataNodeIndexesByFunction(RuleAlgorithm algorithm, IDictionary<string, object> parameter)
        {
            //int[] dataNodeIndexes;
            return Number.ToInt32(algorithm.Calculate(parameter.ToDictionary(x => (object)x.Key, y => y.Value)));
            //object calRst = algorithm.Calculate(parameter);
            //if (calRst is Number)
            //{
            //    dataNodeIndexes = new int[1];
            //    dataNodeIndexes[0] = ((Number)calRst);
            //}
            //else if (calRst is int[])
            //{
            //    dataNodeIndexes = (int[])calRst;
            //}
            //else if (calRst is int[])
            //{
            //    int[] intArray = (int[])calRst;
            //    dataNodeIndexes = new int[intArray.Length];
            //    for (int i = 0; i < intArray.Length; ++i)
            //    {
            //        dataNodeIndexes[i] = intArray[i];
            //    }
            //}
            //else
            //{
            //    throw new ArgumentException("route err: result of route function is wrong type or null: "
            //         + calRst);
            //}
            //return dataNodeIndexes;
        }

        private static bool Equals(string str1, string str2)
        {
            if (str1 == null)
            {
                return str2 == null;
            }
            return str1.Equals(str2);
        }

        /// <exception cref="System.Data.Sql.SQLNonTransientException"/>
        private static void ValidateAST(SQLStatement ast,
            TableConfig tc,
            RuleConfig rule,
            PartitionKeyVisitor visitor)
        {
            if (ast is DMLUpdateStatement)
            {
                IList<Identifier> columns = null;
                IList<string> ruleCols = rule.GetColumns();
                DMLUpdateStatement update = (DMLUpdateStatement)ast;
                foreach (Pair<Identifier, Expr> pair in
                    update.GetValues())
                {
                    foreach (string ruleCol in ruleCols)
                    {
                        if (Equals(pair.GetKey().GetIdTextUpUnescape(), ruleCol))
                        {
                            if (columns == null)
                            {
                                columns = new List<Identifier>(ruleCols.Count);
                            }
                            columns.Add(pair.GetKey());
                        }
                    }
                }
                if (columns == null)
                {
                    return;
                }
                var alias = visitor.GetTableAlias();
                foreach (Identifier column in columns)
                {
                    string table = column.GetLevelUnescapeUpName(2);
                    table = alias.GetValue(table);
                    if (table != null && table.Equals(tc.GetName()))
                    {
                        throw new NotSupportedException("partition key cannot be changed");
                        //throw new SQLFeatureNotSupportedException("partition key cannot be changed");
                    }
                }
            }
        }

        private static bool IsSystemReadSQL(SQLStatement ast)
        {
            if (ast is DALShowStatement)
            {
                return true;
            }
            DMLSelectStatement select = null;
            if (ast is DMLSelectStatement)
            {
                select = (DMLSelectStatement)ast;
            }
            else
            {
                if (ast is DMLSelectUnionStatement)
                {
                    DMLSelectUnionStatement union = (DMLSelectUnionStatement)ast;
                    if (union.GetSelectStmtList().Count == 1)
                    {
                        select = union.GetSelectStmtList()[0];
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
            }
            return select.GetTables() == null;
        }

        private static void SetGroupFlagAndLimit(RouteResultset rrs,
            PartitionKeyVisitor visitor)
        {
            rrs.SetLimitSize(visitor.GetLimitSize());
            switch (visitor.GetGroupFuncType())
            {
                case PartitionKeyVisitor.GroupSum:
                    {
                        rrs.SetFlag(RouteResultset.SumFlag);
                        break;
                    }

                case PartitionKeyVisitor.GroupMax:
                    {
                        rrs.SetFlag(RouteResultset.MaxFlag);
                        break;
                    }

                case PartitionKeyVisitor.GroupMin:
                    {
                        rrs.SetFlag(RouteResultset.MinFlag);
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
            var algorithm = rule.GetRuleAlgorithm();
            var cols = rule.GetColumns();
            var parameter = new Dictionary<string, object>(cols.Count);
            var colsValIter = new List<IList<object>>(columnValues.Count);
            var columnCount = 0;
            foreach (var rc in cols)
            {
                var list = columnValues.GetValue(rc);
                if (list == null)
                {
                    string msg = "route err: rule column " + rc + " dosn't exist in extract: " + columnValues;
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
                        object value = colsValIter[i][countIndex];
                        tuple[i] = value;
                        parameter[cols[i]] = value;
                    }

                    int[] dataNodeIndexes = CalcDataNodeIndexesByFunction(algorithm, parameter);
                    for (int i_1 = 0; i_1 < dataNodeIndexes.Length; ++i_1)
                    {
                        int dataNodeIndex = dataNodeIndexes[i_1];
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
                string msg = "route err: different rule columns should have same value number:  " + columnValues;
                throw new ArgumentException(msg, e);
            }
            return map;
        }

        private static void DispatchWhereBasedStmt(RouteResultsetNode[] rn,
            SQLStatement stmtAST,
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
                {
                    sql = GenSQL(stmtAST, originalSQL);
                }
                else
                {
                    sql = originalSQL;
                }
                int i = -1;
                foreach (int dataNodeId in dataNodeMap.Keys)
                {
                    string dataNode = matchedTable.GetDataNodes()[dataNodeId];
                    rn[++i] = new RouteResultsetNode(dataNode, sql);
                }
                return;
            }

            string table = matchedTable.GetName();
            var columnIndex = visitor.GetColumnIndex(table);
            var valueMap = columnIndex.GetValue(ruleColumns[0]);
            ReplacePartitionKeyOperand(columnIndex, ruleColumns);
            var unreplacedInExpr = new Dictionary<InExpression, ICollection<Expr>>(1);
            var unreplacedSingleExprs = new HashSet<ReplacableExpression>();
            // [perf tag] 12.2755 us: sharding multivalue
            int nodeId = -1;
            foreach (var en in dataNodeMap)
            {
                IList<object[]> tuples = en.Value;
                unreplacedSingleExprs.Clear();
                unreplacedInExpr.Clear();
                foreach (object[] tuple in tuples)
                {
                    object value = tuple[0];
                    var indexedExpressionPair = GetExpressionSet(valueMap, value);
                    foreach (var pair in indexedExpressionPair)
                    {
                        Expr expr = pair.GetKey();
                        var parent = (InExpression)pair.GetValue();
                        if (PartitionKeyVisitor.IsPartitionKeyOperandSingle(expr, parent))
                        {
                            unreplacedSingleExprs.Add((ReplacableExpression)expr);
                        }
                        else
                        {
                            if (PartitionKeyVisitor.IsPartitionKeyOperandIn(expr, parent))
                            {
                                var newInSet = unreplacedInExpr.GetValue(parent);
                                if (newInSet == null)
                                {
                                    newInSet = new HashSet<Expr>();
                                    unreplacedInExpr[(InExpression)parent] = newInSet;
                                }
                                newInSet.Add(expr);
                            }
                        }
                    }
                }
                // [perf tag] 15.3745 us: sharding multivalue
                foreach (ReplacableExpression expr_1 in unreplacedSingleExprs)
                {
                    expr_1.ClearReplaceExpr();
                }
                foreach (var entemp in unreplacedInExpr)
                {
                    var @in = entemp.Key;
                    var set = entemp.Value;
                    if (set == null || set.IsEmpty())
                    {
                        @in.SetReplaceExpr(ReplacableExpressionConstants.BoolFalse);
                    }
                    else
                    {
                        @in.ClearReplaceExpr();
                        var inlist = @in.GetInExpressionList();
                        if (inlist != null)
                        {
                            inlist.SetReplaceExpr(new List<Expr>(set));
                        }
                    }
                }
                // [perf tag] 16.506 us: sharding multivalue
                string sql = GenSQL(stmtAST, originalSQL);
                // [perf tag] 21.3425 us: sharding multivalue
                string dataNodeName = matchedTable.GetDataNodes()[en.Key];
                rn[++nodeId] = new RouteResultsetNode(dataNodeName, sql);
                foreach (var expr_2 in unreplacedSingleExprs)
                {
                    expr_2.SetReplaceExpr(ReplacableExpressionConstants.BoolFalse);
                }
                foreach (InExpression in_1 in unreplacedInExpr.Keys)
                {
                    in_1.SetReplaceExpr(ReplacableExpressionConstants.BoolFalse);
                    InExpressionList list = in_1.GetInExpressionList();
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
            {
                return;
            }
            foreach (string col in cols)
            {
                var map = index.GetValue(col);
                if (map == null)
                {
                    continue;
                }
                foreach (var set in map.Values)
                {
                    if (set == null)
                    {
                        continue;
                    }
                    foreach (var p in set)
                    {
                        Expr expr = p.GetKey();
                        ASTNode parent = p.GetValue();
                        if (PartitionKeyVisitor.IsPartitionKeyOperandSingle(expr, parent))
                        {
                            ((ReplacableExpression)expr).SetReplaceExpr(ReplacableExpressionConstants.BoolFalse);
                        }
                        else
                        {
                            if (PartitionKeyVisitor.IsPartitionKeyOperandIn(expr, parent))
                            {
                                ((ReplacableExpression)parent).SetReplaceExpr(ReplacableExpressionConstants.BoolFalse);
                            }
                        }
                    }
                }
            }
        }

        private static void DispatchInsertReplace(RouteResultsetNode[] rn,
            DMLInsertReplaceStatement stmt,
            IList<string> ruleColumns,
            IDictionary<int, IList<object[]>> dataNodeMap,
            TableConfig matchedTable,
            string originalSQL,
            PartitionKeyVisitor visitor)
        {
            if (stmt.GetSelect() != null)
            {
                DispatchWhereBasedStmt(rn, stmt, ruleColumns, dataNodeMap, matchedTable, originalSQL, visitor);
                return;
            }
            var colsIndex = visitor.GetColumnIndex(stmt.GetTable().GetIdTextUpUnescape());
            if (colsIndex == null || colsIndex.IsEmpty())
            {
                throw new ArgumentException("columns index is empty: " + originalSQL);
            }
            var colsIndexList = new List<ColumnValueType>(ruleColumns.Count);
            for (int i = 0, len = ruleColumns.Count; i < len; ++i)
            {
                colsIndexList.Add(colsIndex[ruleColumns[i]]);
            }
            int dataNodeId = -1;
            foreach (var en in dataNodeMap)
            {
                var tuples = en.Value;
                var replaceRowList = new HashSet<RowExpression>();
                foreach (object[] tuple in tuples)
                {
                    ICollection<Pair<Expr, ASTNode>> tupleExprs = null;
                    for (int i_1 = 0; i_1 < tuple.Length; ++i_1)
                    {
                        var valueMap = colsIndexList[i_1];
                        object value = tuple[i_1];
                        var set = GetExpressionSet(valueMap, value);
                        tupleExprs = CollectionUtil.IntersectSet(tupleExprs, set);
                    }
                    if (tupleExprs == null || tupleExprs.IsEmpty())
                    {
                        throw new ArgumentException("route: empty expression list for insertReplace stmt: "
                             + originalSQL);
                    }
                    foreach (var p in tupleExprs)
                    {
                        if (p.GetValue() == stmt && p.GetKey() is RowExpression)
                        {
                            replaceRowList.Add((RowExpression)p.GetKey());
                        }
                    }
                }
                stmt.SetReplaceRowList(new List<RowExpression>(replaceRowList));
                string sql = GenSQL(stmt, originalSQL);
                stmt.ClearReplaceRowList();
                string dataNodeName = matchedTable.GetDataNodes()[en.Key];
                rn[++dataNodeId] = new RouteResultsetNode(dataNodeName, sql);
            }
        }

        private static ICollection<Pair<Expr, ASTNode>> GetExpressionSet(ColumnValueType map, object value)
        {
            if (map == null || map.IsEmpty())
            {
                return new HashSet<Pair<Expr, ASTNode>>();
            }
            var set = map.GetValue(value);
            if (set == null)
            {
                return new HashSet<Pair<Expr, ASTNode>>();
            }
            return set;
        }

        private static string GenSQL(SQLStatement ast, string orginalSql)
        {
            StringBuilder s = new StringBuilder();
            ast.Accept(new MySQLOutputASTVisitor(s));
            return s.ToString();
        }
    }
}