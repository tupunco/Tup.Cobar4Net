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
using Tup.Cobar4Net.Config.Model;
using Tup.Cobar4Net.Parser.Ast;
using Tup.Cobar4Net.Parser.Ast.Expression;
using Tup.Cobar4Net.Parser.Ast.Expression.Comparison;
using Tup.Cobar4Net.Parser.Ast.Expression.Logical;
using Tup.Cobar4Net.Parser.Ast.Expression.Misc;
using Tup.Cobar4Net.Parser.Ast.Expression.Primary;
using Tup.Cobar4Net.Parser.Ast.Expression.Primary.Function;
using Tup.Cobar4Net.Parser.Ast.Expression.Primary.Function.Cast;
using Tup.Cobar4Net.Parser.Ast.Expression.Primary.Function.Datetime;
using Tup.Cobar4Net.Parser.Ast.Expression.Primary.Function.Groupby;
using Tup.Cobar4Net.Parser.Ast.Expression.Primary.Function.String;
using Tup.Cobar4Net.Parser.Ast.Expression.Primary.Literal;
using Tup.Cobar4Net.Parser.Ast.Expression.String;
using Tup.Cobar4Net.Parser.Ast.Expression.Type;
using Tup.Cobar4Net.Parser.Ast.Fragment;
using Tup.Cobar4Net.Parser.Ast.Fragment.Ddl;
using Tup.Cobar4Net.Parser.Ast.Fragment.Tableref;
using Tup.Cobar4Net.Parser.Ast.Stmt.Dal;
using Tup.Cobar4Net.Parser.Ast.Stmt.Ddl;
using Tup.Cobar4Net.Parser.Ast.Stmt.Dml;
using Tup.Cobar4Net.Parser.Ast.Stmt.Extension;
using Tup.Cobar4Net.Parser.Ast.Stmt.Mts;
using Tup.Cobar4Net.Parser.Util;
using Tup.Cobar4Net.Parser.Visitor;
using Char = Tup.Cobar4Net.Parser.Ast.Expression.Primary.Function.String.Char;
using Convert = Tup.Cobar4Net.Parser.Ast.Expression.Primary.Function.Cast.Convert;

namespace Tup.Cobar4Net.Route.Visitor
{
    using ColumnValueType = IDictionary<object, ICollection<Pair<IExpression, IAstNode>>>;

    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public sealed class PartitionKeyVisitor : ISqlAstVisitor
    {
        public const int GroupCancel = -1;

        public const int GroupNon = 0;

        public const int GroupSum = 1;

        public const int GroupMin = 2;

        public const int GroupMax = 3;
        private static readonly ICollection<Type> VerdictPassThroughWhere = new HashSet<Type>();

        private static readonly ICollection<Type> GroupFuncPassThroughSelect = new HashSet<Type>();

        private static readonly ICollection<Type> PartitionOperandSingle = new HashSet<Type>();

        private static readonly string Null_Alias_Key = "_NULL_ALIAS_";

        private static readonly string[] EmptyStringArray = new string[0];

        private readonly IDictionary<string, string> _tableAlias = new Dictionary<string, string>();

        /// <summary>{tableNameUp -&gt; {columnNameUp -&gt; columnValues}}, obj[] never null</summary>
        private readonly IDictionary<string, IDictionary<string, IList<object>>> columnValue
            = new Dictionary<string, IDictionary<string, IList<object>>>();

        private readonly IDictionary<object, object> evaluationParameter = new Dictionary<object, object>();

        private readonly IDictionary<string, TableConfig> tablesRuleConfig;

        private bool _customedSchema;

        private bool _dual;

        private int _groupFuncType = GroupNon;

        private int _idLevel = 2;

        private long _limitOffset = -1L;

        private long _limitSize = -1L;

        private bool _rewriteField;

        private bool _schemaTrimmed;

        private bool _tableMetaRead;

        private string _trimSchema;

        private bool _verdictColumn = true;

        private bool _verdictGroupFunc = true;

        /// <summary>{table -&gt; {column -&gt; {value -&gt; [(expr,parentExpr)]}}}</summary>
        private IDictionary<string, IDictionary<string, ColumnValueType>> columnValueIndex;

        static PartitionKeyVisitor()
        {
            VerdictPassThroughWhere.Add(typeof(LogicalAndExpression));
            VerdictPassThroughWhere.Add(typeof(LogicalOrExpression));
            VerdictPassThroughWhere.Add(typeof(BetweenAndExpression));
            VerdictPassThroughWhere.Add(typeof(InExpression));
            VerdictPassThroughWhere.Add(typeof(ComparisionNullSafeEqualsExpression));
            VerdictPassThroughWhere.Add(typeof(ComparisionEqualsExpression));
            GroupFuncPassThroughSelect.Add(typeof(Count));
            GroupFuncPassThroughSelect.Add(typeof(Sum));
            GroupFuncPassThroughSelect.Add(typeof(Min));
            GroupFuncPassThroughSelect.Add(typeof(Max));
            GroupFuncPassThroughSelect.Add(typeof(Wildcard));
            PartitionOperandSingle.Add(typeof(BetweenAndExpression));
            PartitionOperandSingle.Add(typeof(ComparisionNullSafeEqualsExpression));
            PartitionOperandSingle.Add(typeof(ComparisionEqualsExpression));
        }

        public PartitionKeyVisitor(IDictionary<string, TableConfig> tables)
        {
            // ---temp
            // state------------------------------------------------------------------
            if (tables == null || tables.IsEmpty())
            {
                tables = new Dictionary<string, TableConfig>(0);
            }
            tablesRuleConfig = tables;
        }

        public void Visit(DmlSelectStatement node)
        {
            var verdictGroup = true;
            var exprList = node.GetSelectExprListWithoutAlias();
            if (_verdictGroupFunc)
            {
                foreach (var expr in exprList)
                {
                    if (IsGroupFuncPassthroughSelect(expr))
                        continue;

                    _groupFuncType = GroupCancel;
                    verdictGroup = false;
                    break;
                }
                Limit(node.Limit);
            }

            VisitChild(2, false, _verdictGroupFunc && verdictGroup, exprList);

            TableReference tr = node.Tables;
            VisitChild(1, _verdictColumn, _verdictGroupFunc && verdictGroup, tr);

            var where = node.Where;
            VisitChild(2, _verdictColumn, false, where);

            var group = node.Group;
            VisitChild(2, false, false, group);

            var having = node.Having;
            VisitChild(2, _verdictColumn, false, having);

            var order = node.Order;
            VisitChild(2, false, false, order);
        }

        public void Visit(DmlSelectUnionStatement node)
        {
            VisitChild(2, false, false, node.OrderBy);
            VisitChild(2, false, false, node.SelectStmtList);
        }

        public void Visit(DmlUpdateStatement node)
        {
            TableReference tr = node.TableRefs;
            VisitChild(1, false, false, tr);

            var assignmentList = node.Values;
            if (assignmentList != null && !assignmentList.IsEmpty())
            {
                IList<IAstNode> list = new List<IAstNode>(assignmentList.Count * 2);
                foreach (var p in assignmentList)
                {
                    if (p == null)
                    {
                        continue;
                    }
                    list.Add(p.Key);
                    list.Add(p.Value);
                }
                VisitChild(2, false, false, list);
            }

            var where = node.Where;
            VisitChild(2, _verdictColumn, false, where);

            var order = node.OrderBy;
            VisitChild(2, false, false, order);
        }

        public void Visit(DmlDeleteStatement node)
        {
            TableReference tr = node.TableRefs;
            var tbs = node.TableNames;
            if (tr == null)
            {
                var table = tbs[0];
                TableAsTableFactor(table);
            }
            else
            {
                VisitChild(1, _verdictColumn, false, tr);
                foreach (var tb in tbs)
                {
                    if (tb is Wildcard)
                    {
                        var trim = tb.TrimParent(2, _trimSchema);
                        _schemaTrimmed = _schemaTrimmed || trim == Identifier.ParentTrimed;
                        _customedSchema = _customedSchema || trim == Identifier.ParentIgnored;
                    }
                    else
                    {
                        var trim = tb.TrimParent(1, _trimSchema);
                        _schemaTrimmed = _schemaTrimmed || trim == Identifier.ParentTrimed;
                        _customedSchema = _customedSchema || trim == Identifier.ParentIgnored;
                    }
                }
            }

            var where = node.WhereCondition;
            VisitChild(2, _verdictColumn, false, where);

            if (tr == null)
            {
                var order = node.OrderBy;
                VisitChild(2, false, false, order);
            }
        }

        public void Visit(DmlInsertStatement node)
        {
            InsertReplace(node);
            var dup = node.DuplicateUpdate;
            if (dup == null)
                return;

            var duplist = new IAstNode[dup.Count * 2];
            var i = 0;
            foreach (var p in dup)
            {
                Identifier key = null;
                IExpression value = null;
                if (p != null)
                {
                    key = p.Key;
                    value = p.Value;
                }
                duplist[i++] = key;
                duplist[i++] = value;
            }
            VisitChild(2, false, false, duplist);
        }

        public void Visit(DmlReplaceStatement node)
        {
            InsertReplace(node);
        }

        public void Visit(DdlTruncateStatement node)
        {
            DdlTable(node.Table, 1);
        }

        public void Visit(DdlAlterTableStatement node)
        {
            DdlTable(node.Table, 1);
        }

        public void Visit(DdlCreateIndexStatement node)
        {
            DdlTable(node.Table, 1);
            DdlTable(node.IndexName, 1);
        }

        public void Visit(DdlCreateTableStatement node)
        {
            DdlTable(node.Table, 1);
        }

        public void Visit(DdlRenameTableStatement node)
        {
            var list = node.PairList;
            IList<Identifier> idl = new List<Identifier>(list.Count * 2);
            foreach (var p in list)
            {
                if (p == null)
                    continue;

                if (p.Key != null)
                {
                    AddTable(p.Key.IdTextUpUnescape);
                    idl.Add(p.Key);
                }
                idl.Add(p.Value);
            }
            VisitChild(1, false, false, idl);
        }

        public void Visit(DdlDropIndexStatement node)
        {
            DdlTable(node.Table, 1);
            DdlTable(node.IndexName, 1);
        }

        public void Visit(DdlDropTableStatement node)
        {
            VisitChild(1, false, false, node.TableNames);

            var tbs = node.TableNames;
            if (tbs == null)
                return;

            foreach (var tb in tbs)
            {
                AddTable(tb.IdTextUpUnescape);
            }
        }

        public void Visit(BetweenAndExpression node)
        {
            var fst = node.First;
            var snd = node.Second;
            var trd = node.Third;
            VisitChild(2, false, false, fst, snd, trd);

            if (_verdictColumn && !node.IsNot && fst is Identifier)
            {
                var col = (Identifier)fst;
                var table = _tableAlias.GetValue(col.GetLevelUnescapeUpName(2) ?? Null_Alias_Key);
                if (IsRuledColumn(table, col.IdTextUpUnescape))
                {
                    var e1 = snd.Evaluation(evaluationParameter);
                    var e2 = trd.Evaluation(evaluationParameter);
                    if (e1 != ExpressionConstants.Unevaluatable
                        && e2 != ExpressionConstants.Unevaluatable
                        && e1 != null
                        && e2 != null)
                    {
                        if (CompareEvaluatedValue(e1, e2))
                        {
                            AddColumnValue(table, col.IdTextUpUnescape, e1, node, null);
                        }
                    }
                }
            }
        }

        public void Visit(ComparisionIsExpression node)
        {
            var operand = node.Operand;
            VisitChild(2, false, false, operand);
            if (!_verdictColumn || !(operand is Identifier))
                return;

            var col = (Identifier)operand;
            var table = _tableAlias.GetValue(col.GetLevelUnescapeUpName(2) ?? Null_Alias_Key);
            if (!IsRuledColumn(table, col.IdTextUpUnescape))
                return;

            switch (node.Mode)
            {
                case ComparisionIsExpression.IsFalse:
                    {
                        AddColumnValue(table, col.IdTextUpUnescape, LiteralBoolean.False, node, null);
                        break;
                    }

                case ComparisionIsExpression.IsTrue:
                    {
                        AddColumnValue(table, col.IdTextUpUnescape, LiteralBoolean.True, node, null);
                        break;
                    }

                case ComparisionIsExpression.IsNull:
                    {
                        AddColumnValue(table, col.IdTextUpUnescape, null, node, null);
                        break;
                    }
            }
        }

        public void Visit(InExpressionList node)
        {
            VisitChild(2, false, false, node.ExprList);
        }

        public void Visit(BinaryOperatorExpression node)
        {
            var left = node.LeftOprand;
            var right = node.RightOprand;
            VisitChild(2, false, false, left, right);
        }

        public void Visit(PolyadicOperatorExpression node)
        {
        }

        // QS_TODO
        public void Visit(ComparisionEqualsExpression node)
        {
            var left = node.LeftOprand;
            var right = node.RightOprand;
            VisitChild(2, false, false, left, right);

            if (!_verdictColumn)
                return;

            if (left is Identifier)
            {
                ComparisionEquals((Identifier)left, right.Evaluation(evaluationParameter), false,
                    node);
            }
            else if (right is Identifier)
            {
                ComparisionEquals((Identifier)right, left.Evaluation(evaluationParameter), false,
                    node);
            }
        }

        public void Visit(ComparisionNullSafeEqualsExpression node)
        {
            var left = node.LeftOprand;
            var right = node.RightOprand;
            VisitChild(2, false, false, left, right);

            if (!_verdictColumn)
                return;

            if (left is Identifier)
            {
                ComparisionEquals((Identifier)left,
                    right.Evaluation(evaluationParameter),
                    true, node);
            }
            else if (right is Identifier)
            {
                ComparisionEquals((Identifier)right,
                    left.Evaluation(evaluationParameter),
                    true, node);
            }
        }

        public void Visit(InExpression node)
        {
            var left = node.LeftOprand;
            var right = node.RightOprand;
            VisitChild(2, false, false, left, right);

            if (!_verdictColumn || node.IsNot || !(left is Identifier) || !(right is InExpressionList))
                return;

            var col = (Identifier)left;
            var colName = col.IdTextUpUnescape;
            var table = _tableAlias.GetValue(col.GetLevelUnescapeUpName(2) ?? Null_Alias_Key);

            if (!IsRuledColumn(table, colName))
                return;

            var valList = EnsureColumnValueList(EnsureColumnValueByTable(table), colName);
            var valMap = EnsureColumnValueIndexObjMap(EnsureColumnValueIndexByTable(table), colName);
            var inlist = (InExpressionList)right;
            foreach (var expr in inlist.ExprList)
            {
                var value = expr.Evaluation(evaluationParameter);
                if (value != ExpressionConstants.Unevaluatable)
                {
                    valList.Add(value);
                    AddIntoColumnValueIndex(valMap, value, expr, node);
                }
            }
        }

        public void Visit(LogicalAndExpression node)
        {
            for (int i = 0, len = node.Arity; i < len; ++i)
            {
                var oprand = node.GetOperand(i);
                VisitChild(2, _verdictColumn && IsVerdictPassthroughWhere(oprand), false, oprand);
            }
        }

        public void Visit(LogicalOrExpression node)
        {
            for (int i = 0, len = node.Arity; i < len; ++i)
            {
                var oprand = node.GetOperand(i);
                VisitChild(2, _verdictColumn && IsVerdictPassthroughWhere(oprand), false, oprand);
            }
        }

        public void Visit(Count node)
        {
            VisitChild(2, false, false, node.Arguments);
            if (!_verdictGroupFunc)
                return;

            if (_groupFuncType != GroupNon && _groupFuncType != GroupSum || node.IsDistinct)
                _groupFuncType = GroupCancel;
            else
                _groupFuncType = GroupSum;
        }

        public void Visit(Sum node)
        {
            VisitChild(2, false, false, node.Arguments);
            if (!_verdictGroupFunc)
                return;

            if (_groupFuncType != GroupNon && _groupFuncType != GroupSum || node.IsDistinct)
                _groupFuncType = GroupCancel;
            else
                _groupFuncType = GroupSum;
        }

        public void Visit(Max node)
        {
            VisitChild(2, false, false, node.Arguments);
            if (!_verdictGroupFunc)
                return;

            if (_groupFuncType != GroupNon && _groupFuncType != GroupMax)
                _groupFuncType = GroupCancel;
            else
                _groupFuncType = GroupMax;
        }

        public void Visit(Min node)
        {
            VisitChild(2, false, false, node.Arguments);
            if (!_verdictGroupFunc)
                return;

            if (_groupFuncType != GroupNon && _groupFuncType != GroupMin)
                _groupFuncType = GroupCancel;
            else
                _groupFuncType = GroupMin;
        }

        public void Visit(Identifier node)
        {
            var trim = node.TrimParent(_idLevel, _trimSchema);
            _schemaTrimmed = _schemaTrimmed || trim == Identifier.ParentTrimed;
            _customedSchema = _customedSchema || trim == Identifier.ParentIgnored;
        }

        public void Visit(InnerJoin node)
        {
            var tr1 = node.LeftTableRef;
            var tr2 = node.RightTableRef;
            var on = node.OnCond;
            VisitChild(1, _verdictColumn, _verdictGroupFunc, tr1, tr2);
            VisitChild(2, _verdictColumn && IsVerdictPassthroughWhere(on), false, on);
        }

        public void Visit(NaturalJoin node)
        {
            var tr1 = node.LeftTableRef;
            var tr2 = node.RightTableRef;
            VisitChild(1, _verdictColumn, _verdictGroupFunc, tr1, tr2);
        }

        public void Visit(OuterJoin node)
        {
            var tr1 = node.LeftTableRef;
            var tr2 = node.RightTableRef;
            var on = node.OnCond;
            VisitChild(1, _verdictColumn, _verdictGroupFunc, tr1, tr2);
            VisitChild(2, _verdictColumn && IsVerdictPassthroughWhere(on), false, on);
        }

        public void Visit(StraightJoin node)
        {
            var tr1 = node.LeftTableRef;
            var tr2 = node.RightTableRef;
            var on = node.OnCond;
            VisitChild(1, _verdictColumn, _verdictGroupFunc, tr1, tr2);
            VisitChild(2, _verdictColumn && IsVerdictPassthroughWhere(on), false, on);
        }

        public void Visit(TableReferences node)
        {
            var list = node.TableReferenceList;
            VisitChild(1, _verdictColumn, _verdictGroupFunc, list);
        }

        public void Visit(SubqueryFactor node)
        {
            var query = node.Subquery;
            VisitChild(2, _verdictColumn, _verdictGroupFunc, query);
        }

        public void Visit(TableRefFactor node)
        {
            //TODO Visit(TableRefFactor node) _NULL_ALIAS_
            var table = node.Table;
            VisitChild(1, false, false, table);

            var tableName = table.IdTextUpUnescape;
            AddTable(tableName);
            var alias = node.GetAliasUnescapeUppercase();
            if (alias == null)
            {
                _tableAlias[Null_Alias_Key] = tableName;
                _tableAlias[tableName] = tableName;
            }
            else
            {
                if (!_tableAlias.ContainsKey(Null_Alias_Key))
                {
                    _tableAlias[Null_Alias_Key] = tableName;
                }
                _tableAlias[alias] = tableName;
            }
        }

        public void Visit(Dual dual)
        {
            _dual = true;
        }

        // ------------------------------------------------------------------------------
        public void Visit(LikeExpression node)
        {
            VisitChild(2, false, false, node.First, node.Second, node.Third);
        }

        public void Visit(CollateExpression node)
        {
            VisitChild(2, false, false, node.StringValue);
        }

        public void Visit(UserExpression node)
        {
        }

        public void Visit(UnaryOperatorExpression node)
        {
            VisitChild(2, false, false, node.Operand);
        }

        public void Visit(FunctionExpression node)
        {
            VisitChild(2, false, false, node.Arguments);
        }

        public void Visit(Char node)
        {
            VisitChild(2, false, false, node.Arguments);
        }

        public void Visit(Convert node)
        {
            VisitChild(2, false, false, node.Arguments);
        }

        public void Visit(Trim node)
        {
            VisitChild(2, false, false, node.Arguments);
        }

        public void Visit(Cast
            node)
        {
            VisitChild(2, false, false, node.Arguments);
            VisitChild(2, false, false, node.TypeInfo1, node.TypeInfo2);
        }

        public void Visit(Avg node)
        {
            VisitChild(2, false, false, node.Arguments);
        }

        public void Visit(GroupConcat node)
        {
            VisitChild(2, false, false, node.Arguments);
        }

        public void Visit(IntervalPrimary node)
        {
            VisitChild(2, false, false, node.Quantity);
        }

        public void Visit(Extract node)
        {
            VisitChild(2, false, false, node.Arguments);
        }

        public void Visit(Timestampdiff node)
        {
            VisitChild(2, false, false, node.Arguments);
        }

        public void Visit(Timestampadd node)
        {
            VisitChild(2, false, false, node.Arguments);
        }

        public void Visit(GetFormat node)
        {
        }

        public void Visit(LiteralBitField node)
        {
        }

        public void Visit(LiteralBoolean node)
        {
        }

        public void Visit(LiteralHexadecimal node)
        {
        }

        public void Visit(LiteralNull node)
        {
        }

        public void Visit(LiteralNumber node)
        {
        }

        public void Visit(LiteralString node)
        {
        }

        public void Visit(CaseWhenOperatorExpression node)
        {
            VisitChild(2, false, false, node.Comparee, node.ElseResult);
            var whenPairList = node.WhenList;
            if (whenPairList == null || whenPairList.IsEmpty())
            {
                return;
            }

            var list = new List<IExpression>(whenPairList.Count * 2);
            foreach (var pair in whenPairList)
            {
                if (pair == null)
                {
                    continue;
                }
                list.Add(pair.Key);
                list.Add(pair.Value);
            }
            VisitChild(2, false, false, list);
        }

        public void Visit(DefaultValue node)
        {
        }

        public void Visit(ExistsPrimary node)
        {
            VisitChild(2, false, false, node.Subquery);
        }

        public void Visit(PlaceHolder node)
        {
        }

        public void Visit(MatchExpression node)
        {
            VisitChild(2, false, false, node.Columns);
            VisitChild(2, false, false, node.Pattern);
        }

        public void Visit(ParamMarker node)
        {
        }

        public void Visit(RowExpression node)
        {
            VisitChild(2, false, false, node.RowExprList);
        }

        public void Visit(SysVarPrimary node)
        {
        }

        public void Visit(UsrDefVarPrimary node)
        {
        }

        public void Visit(IndexHint node)
        {
        }

        public void Visit(GroupBy node)
        {
            SortPairList(node.OrderByList);
        }

        public void Visit(OrderBy node)
        {
            SortPairList(node.OrderByList);
        }

        public void Visit(Limit node)
        {
        }

        public void Visit(ColumnDefinition node)
        {
        }

        public void Visit(IndexOption node)
        {
        }

        public void Visit(IndexColumnName node)
        {
        }

        public void Visit(TableOptions node)
        {
        }

        public void Visit(DdlAlterTableStatement.AlterSpecification node)
        {
        }

        public void Visit(DataType node)
        {
        }

        public void Visit(ShowAuthors node)
        {
        }

        public void Visit(ShowBinaryLog node)
        {
        }

        public void Visit(ShowBinLogEvent node)
        {
        }

        public void Visit(ShowCharaterSet node)
        {
        }

        public void Visit(ShowCollation node)
        {
        }

        public void Visit(ShowColumns node)
        {
            TableMetaRead(node.Table);
        }

        public void Visit(ShowContributors node)
        {
        }

        public void Visit(ShowCreate node)
        {
            if (node.CreateType == CreateType.Table)
            {
                TableMetaRead(node.Id);
            }
        }

        public void Visit(ShowDatabases node)
        {
        }

        public void Visit(ShowEngine node)
        {
        }

        public void Visit(ShowEngines node)
        {
        }

        public void Visit(ShowErrors node)
        {
        }

        public void Visit(ShowEvents node)
        {
            if (node.Schema != null)
            {
                _schemaTrimmed = true;
                node.Schema = null;
            }
            TableMetaRead(null);
        }

        public void Visit(ShowFunctionCode node)
        {
        }

        public void Visit(ShowFunctionStatus node)
        {
        }

        public void Visit(ShowGrants node)
        {
        }

        public void Visit(ShowIndex node)
        {
            TableMetaRead(node.Table);
        }

        public void Visit(ShowMasterStatus node)
        {
        }

        public void Visit(ShowOpenTables node)
        {
            if (node.Schema != null)
            {
                _schemaTrimmed = true;
                node.Schema = null;
            }
            TableMetaRead(null);
        }

        public void Visit(ShowPlugins node)
        {
        }

        public void Visit(ShowPrivileges node)
        {
        }

        public void Visit(ShowProcedureCode node)
        {
        }

        public void Visit(ShowProcedureStatus node)
        {
        }

        public void Visit(ShowProcesslist node)
        {
        }

        public void Visit(ShowProfile node)
        {
        }

        public void Visit(ShowProfiles node)
        {
        }

        public void Visit(ShowSlaveHosts node)
        {
        }

        public void Visit(ShowSlaveStatus node)
        {
        }

        public void Visit(ShowStatus node)
        {
        }

        public void Visit(ShowTables node)
        {
            if (node.Schema != null)
            {
                _schemaTrimmed = true;
                node.Schema = null;
            }
            _rewriteField = true;
            TableMetaRead(null);
        }

        public void Visit(ShowTableStatus node)
        {
            if (node.Database != null)
            {
                _schemaTrimmed = true;
                node.Database = null;
            }
            TableMetaRead(null);
        }

        public void Visit(ShowTriggers node)
        {
            if (node.Schema != null)
            {
                _schemaTrimmed = true;
                node.Schema = null;
            }
            TableMetaRead(null);
        }

        public void Visit(ShowVariables node)
        {
        }

        public void Visit(ShowWarnings node)
        {
        }

        public void Visit(DescTableStatement node)
        {
            TableMetaRead(node.Table);
        }

        public void Visit(DalSetStatement node)
        {
        }

        public void Visit(DalSetNamesStatement node)
        {
        }

        public void Visit(DalSetCharacterSetStatement node)
        {
        }

        public void Visit(DmlCallStatement node)
        {
        }

        public void Visit(MTSSetTransactionStatement node)
        {
        }

        public void Visit(MTSSavepointStatement node)
        {
        }

        public void Visit(MTSReleaseStatement node)
        {
        }

        public void Visit(MTSRollbackStatement node)
        {
        }

        public void Visit(ExtDdlCreatePolicy node)
        {
        }

        public void Visit(ExtDdlDropPolicy node)
        {
        }

        private static bool IsVerdictPassthroughWhere(IExpression node)
        {
            if (node == null)
            {
                return false;
            }
            return VerdictPassThroughWhere.Contains(node.GetType());
        }

        private static bool IsGroupFuncPassthroughSelect(IExpression node)
        {
            if (node == null)
            {
                return false;
            }
            return GroupFuncPassThroughSelect.Contains(node.GetType());
        }

        public static bool IsPartitionKeyOperandSingle(IExpression expr, IAstNode parent)
        {
            return parent == null
                   && expr is IReplacableExpression
                   && PartitionOperandSingle.Contains(expr.GetType());
        }

        public static bool IsPartitionKeyOperandIn(IExpression expr, IAstNode parent)
        {
            return expr != null && parent is InExpression;
        }

        // ---output------------------------------------------------------------------
        public bool IsDual()
        {
            return _dual;
        }

        public bool IsCustomedSchema()
        {
            return _customedSchema;
        }

        public bool IsTableMetaRead()
        {
            return _tableMetaRead;
        }

        public bool IsNeedRewriteField()
        {
            return _rewriteField;
        }

        /// <returns>null for statement not table meta read</returns>
        public string[] GetMetaReadTable()
        {
            if (!IsTableMetaRead())
                return null;

            var tables = columnValue.Keys;
            return tables.IsEmpty() ? EmptyStringArray : tables.ToArray();
        }

        public IDictionary<string, string> GetTableAlias()
        {
            return _tableAlias;
        }

        /// <returns>-1 for no limit</returns>
        public long GetLimitOffset()
        {
            return _limitOffset;
        }

        /// <returns>-1 for no limit</returns>
        public long GetLimitSize()
        {
            return _limitSize;
        }

        /// <returns>
        ///     <see cref="GroupNon" />
        ///     or
        ///     <see cref="GroupSum" />
        ///     or
        ///     <see cref="GroupMin" />
        ///     or
        ///     <see cref="GroupMax" />
        /// </returns>
        public int GetGroupFuncType()
        {
            return _groupFuncType;
        }

        public bool IsSchemaTrimmed()
        {
            return _schemaTrimmed;
        }

        /// <returns>never null</returns>
        public IDictionary<string, ColumnValueType> GetColumnIndex(string tableNameUp)
        {
            if (columnValueIndex == null)
                return new Dictionary<string, ColumnValueType>(0);

            var index = columnValueIndex.GetValue(tableNameUp);
            if (index == null || index.IsEmpty())
                return new Dictionary<string, ColumnValueType>(0);

            return index;
        }

        /// <returns><code>table -&gt; null</code> is possible</returns>
        public IDictionary<string, IDictionary<string, IList<object>>> GetColumnValue()
        {
            return columnValue;
        }

        private void AddTable(string tableNameUp)
        {
            AddTable(tableNameUp, 2);
        }

        /// <param name="initColumnMapSize">0 for emptyMap</param>
        private void AddTable(string tableNameUp, int initColumnMapSize)
        {
            if (columnValue.ContainsKey(tableNameUp))
                return;

            var colMap = new Dictionary<string, IList<object>>(initColumnMapSize > 0 ? initColumnMapSize : 0);
            columnValue[tableNameUp] = colMap;
        }

        private void AddColumnValueIndex(string table,
            string column,
            object value,
            IExpression expr,
            IAstNode parent)
        {
            var colMap = EnsureColumnValueIndexByTable(table);
            var valMap = EnsureColumnValueIndexObjMap(colMap, column);
            AddIntoColumnValueIndex(valMap, value, expr, parent);
        }

        private void AddIntoColumnValueIndex(ColumnValueType valMap,
            object value,
            IExpression expr,
            IAstNode parent)
        {
            var exprSet = value == null ? null : valMap.GetValue(value);
            if (exprSet == null)
            {
                exprSet = new HashSet<Pair<IExpression, IAstNode>>();
                valMap[value ?? Null_Alias_Key] = exprSet;
            }
            var pair = new Pair<IExpression, IAstNode>(expr, parent);
            exprSet.Add(pair);
        }

        private IDictionary<string, ColumnValueType> EnsureColumnValueIndexByTable(string table)
        {
            if (columnValueIndex == null)
                columnValueIndex = new Dictionary<string, IDictionary<string, ColumnValueType>>();

            var colMap = columnValueIndex.GetValue(table);
            if (colMap == null)
            {
                colMap = new Dictionary<string, ColumnValueType>();
                columnValueIndex[table] = colMap;
            }
            return colMap;
        }

        private ColumnValueType EnsureColumnValueIndexObjMap(
            IDictionary<string, ColumnValueType> colMap,
            string column)
        {
            var valMap = column == null ? null : colMap.GetValue(column);
            if (valMap == null)
            {
                valMap = new Dictionary<object, ICollection<Pair<IExpression, IAstNode>>>();
                colMap[column ?? Null_Alias_Key] = valMap;
            }
            return valMap;
        }

        private void AddColumnValue(string tableNameUp,
            string columnNameUp,
            object value,
            IExpression expr,
            IAstNode parent)
        {
            var colVals = EnsureColumnValueByTable(tableNameUp);
            EnsureColumnValueList(colVals, columnNameUp).Add(value);
            AddColumnValueIndex(tableNameUp, columnNameUp, value, expr, parent);
        }

        private IDictionary<string, IList<object>> EnsureColumnValueByTable(string tableNameUp)
        {
            var colVals = columnValue.GetValue(tableNameUp);
            if (colVals == null)
            {
                colVals = new Dictionary<string, IList<object>>();
                columnValue[tableNameUp] = colVals;
            }
            return colVals;
        }

        private IList<object> EnsureColumnValueList(IDictionary<string, IList<object>> columnValue, string column)
        {
            var list = columnValue.GetValue(column);
            if (list == null)
            {
                list = new List<object>();
                columnValue[column] = list;
            }
            return list;
        }

        public PartitionKeyVisitor SetTrimSchema(string trimSchema)
        {
            if (trimSchema != null)
                _trimSchema = trimSchema.ToUpper();

            return this;
        }

        private bool IsRuledColumn(string tableNameUp, string columnNameUp)
        {
            if (tableNameUp == null)
                return false;

            var config = tablesRuleConfig.GetValue(tableNameUp);
            return config != null && config.ExistsColumn(columnNameUp);
        }

        private void VisitChild(int idLevel,
            bool verdictColumn,
            bool verdictGroupFunc,
            params IAstNode[] nodes)
        {
            if (nodes == null || nodes.Length <= 0)
                return;

            VisitChild(idLevel, verdictColumn, verdictGroupFunc, new List<IAstNode>(nodes));
        }

        private void VisitChild<TNode>(int idLevel,
            bool verdictColumn,
            bool verdictGroupFunc,
            IList<TNode> nodes)
            where TNode : IAstNode
        {
            if (nodes == null || nodes.IsEmpty())
            {
                return;
            }
            var oldLevel = _idLevel;
            var oldVerdict = _verdictColumn;
            var oldverdictGroupFunc = _verdictGroupFunc;
            _idLevel = idLevel;
            _verdictColumn = verdictColumn;
            _verdictGroupFunc = verdictGroupFunc;
            try
            {
                foreach (IAstNode node in nodes)
                {
                    if (node != null)
                    {
                        node.Accept(this);
                    }
                }
            }
            finally
            {
                _verdictColumn = oldVerdict;
                _idLevel = oldLevel;
                _verdictGroupFunc = oldverdictGroupFunc;
            }
        }

        // --------------------------------------------------------------------------------
        private void Limit(Limit limit)
        {
            if (limit == null)
                return;

            var ls = limit.Size;
            if (ls is IExpression)
                ls = ((IExpression)ls).Evaluation(evaluationParameter);

            if (ls is int)
                _limitSize = (int)ls;

            var lo = limit.Offset;
            if (lo is IExpression)
                lo = ((IExpression)lo).Evaluation(evaluationParameter);

            if (lo is int)
                _limitOffset = (int)lo;
        }

        private void TableAsTableFactor(Identifier table)
        {
            var trim = table.TrimParent(1, _trimSchema);
            _schemaTrimmed = _schemaTrimmed || trim == Identifier.ParentTrimed;
            _customedSchema = _customedSchema || trim == Identifier.ParentIgnored;

            var tableName = table.IdTextUpUnescape;
            _tableAlias[Null_Alias_Key] = tableName;
            _tableAlias[tableName] = tableName;
            AddTable(tableName);
        }

        private void InsertReplace(DmlInsertReplaceStatement node)
        {
            var table = node.Table;
            var collist = node.ColumnNameList;
            var query = node.Select;
            var rows = node.RowList;
            TableAsTableFactor(table);

            var tableName = table.IdTextUpUnescape;
            VisitChild(2, false, false, collist);
            if (query != null)
            {
                query.Accept(this);
                return;
            }

            foreach (var row in rows)
            {
                VisitChild(2, false, false, row);
            }

            var colVals = EnsureColumnValueByTable(tableName);
            var colValsIndex = EnsureColumnValueIndexByTable(tableName);
            if (collist == null)
                return;

            for (var i = 0; i < collist.Count; ++i)
            {
                var colName = collist[i].IdTextUpUnescape;
                if (IsRuledColumn(tableName, colName))
                {
                    var valueList = EnsureColumnValueList(colVals, colName);
                    var valMap = EnsureColumnValueIndexObjMap(colValsIndex, colName);
                    foreach (var row_1 in rows)
                    {
                        var expr = row_1.RowExprList[i];
                        var value = expr == null ? null : expr.Evaluation(evaluationParameter);
                        if (value != ExpressionConstants.Unevaluatable)
                        {
                            valueList.Add(value);
                            AddIntoColumnValueIndex(valMap, value, row_1, node);
                        }
                    }
                }
            }
        }

        private void DdlTable(Identifier id, int idLevel)
        {
            VisitChild(idLevel, false, false, id);
            AddTable(id.IdTextUpUnescape);
        }

        /// <param name="obj1">not null</param>
        /// <param name="obj2">not null</param>
        private static bool CompareEvaluatedValue(object obj1, object obj2)
        {
            if (obj1.Equals(obj2))
                return true;

            try
            {
                var pair = ExprEvalUtils.ConvertNum2SameLevel(obj1, obj2);
                return pair.Key.Equals(pair.Value);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void ComparisionEquals(Identifier col, object value, bool nullsafe, IExpression node)
        {
            if (value == ExpressionConstants.Unevaluatable || (!nullsafe && value == null))
                return;

            var table = _tableAlias.GetValue(col.GetLevelUnescapeUpName(2) ?? Null_Alias_Key);
            if (IsRuledColumn(table, col.IdTextUpUnescape))
            {
                AddColumnValue(table, col.IdTextUpUnescape, value, node, null);
            }
        }

        private void SortPairList(IList<Pair<IExpression, SortOrder>> list)
        {
            if (list == null || list.IsEmpty())
                return;

            var exprs = new IExpression[list.Count];
            var i = 0;
            foreach (var p in list)
            {
                exprs[i] = p.Key;
                ++i;
            }
            VisitChild(2, false, false, new List<IExpression>(exprs));
        }

        private void TableMetaRead(Identifier table)
        {
            if (table != null)
            {
                VisitChild(1, false, false, table);
                AddTable(table.IdTextUpUnescape, 0);
            }
            _tableMetaRead = true;
        }
    }
}