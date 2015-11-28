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
using Tup.Cobar4Net.Parser.Ast.Expression.Primary.Function.Datetime;
using Tup.Cobar4Net.Parser.Ast.Expression.Primary.Function.Groupby;
using Tup.Cobar4Net.Parser.Ast.Expression.Primary.Function.String;
using Tup.Cobar4Net.Parser.Ast.Expression.Primary.Literal;
using Tup.Cobar4Net.Parser.Ast.Expression.String;
using Tup.Cobar4Net.Parser.Ast.Expression.Type;
using Tup.Cobar4Net.Parser.Ast.Fragment;
using Tup.Cobar4Net.Parser.Ast.Fragment.Ddl;
using Tup.Cobar4Net.Parser.Ast.Fragment.Ddl.Datatype;
using Tup.Cobar4Net.Parser.Ast.Fragment.Ddl.Index;
using Tup.Cobar4Net.Parser.Ast.Fragment.Tableref;
using Tup.Cobar4Net.Parser.Ast.Stmt.Dal;
using Tup.Cobar4Net.Parser.Ast.Stmt.Ddl;
using Tup.Cobar4Net.Parser.Ast.Stmt.Dml;
using Tup.Cobar4Net.Parser.Ast.Stmt.Extension;
using Tup.Cobar4Net.Parser.Ast.Stmt.Mts;
using Tup.Cobar4Net.Parser.Util;
using Tup.Cobar4Net.Parser.Visitor;
using Expr = Tup.Cobar4Net.Parser.Ast.Expression.Expression;

namespace Tup.Cobar4Net.Route.Visitor
{
    using ColumnValueType = IDictionary<object, ICollection<Pair<Expr, ASTNode>>>;

    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public sealed class PartitionKeyVisitor : SQLASTVisitor
    {
        private static readonly ICollection<Type> VerdictPassThroughWhere = new HashSet<Type>();

        private static readonly ICollection<Type> GroupFuncPassThroughSelect = new HashSet<Type>();

        private static readonly ICollection<Type> PartitionOperandSingle = new HashSet<Type>();

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

        private static bool IsVerdictPassthroughWhere(Expr node)
        {
            if (node == null)
            {
                return false;
            }
            return VerdictPassThroughWhere.Contains(node.GetType());
        }

        private static bool IsGroupFuncPassthroughSelect(Expr node)
        {
            if (node == null)
            {
                return false;
            }
            return GroupFuncPassThroughSelect.Contains(node.GetType());
        }

        public static bool IsPartitionKeyOperandSingle(Expr expr, ASTNode parent)
        {
            return parent == null
                && expr is ReplacableExpression
                && PartitionOperandSingle.Contains(expr.GetType());
        }

        public static bool IsPartitionKeyOperandIn(Expr expr, ASTNode parent)
        {
            return expr != null && parent is InExpression;
        }

        public const int GroupCancel = -1;

        public const int GroupNon = 0;

        public const int GroupSum = 1;

        public const int GroupMin = 2;

        public const int GroupMax = 3;

        private bool dual = false;

        private int groupFuncType = GroupNon;

        private long limitSize = -1L;

        private long limitOffset = -1L;

        private bool tableMetaRead;

        private bool rewriteField = false;

        private bool schemaTrimmed = false;

        private bool customedSchema = false;

        /// <summary>{tableNameUp -&gt; {columnNameUp -&gt; columnValues}}, obj[] never null</summary>
        private IDictionary<string, IDictionary<string, IList<object>>> columnValue
                                = new Dictionary<string, IDictionary<string, IList<object>>>();

        /// <summary>{table -&gt; {column -&gt; {value -&gt; [(expr,parentExpr)]}}}</summary>
        private IDictionary<string, IDictionary<string, ColumnValueType>> columnValueIndex = null;

        private static readonly string Null_Alias_Key = "_NULL_ALIAS_";

        private IDictionary<string, string> tableAlias = new Dictionary<string, string>();

        private static readonly string[] EmptyStringArray = new string[0];

        // ---output------------------------------------------------------------------
        public bool IsDual()
        {
            return dual;
        }

        public bool IsCustomedSchema()
        {
            return customedSchema;
        }

        public bool IsTableMetaRead()
        {
            return tableMetaRead;
        }

        public bool IsNeedRewriteField()
        {
            return rewriteField;
        }

        /// <returns>null for statement not table meta read</returns>
        public string[] GetMetaReadTable()
        {
            if (IsTableMetaRead())
            {
                var tables = columnValue.Keys;
                if (tables == null || tables.IsEmpty())
                {
                    return EmptyStringArray;
                }
                return tables.ToArray();
                //string[] array = new string[tables.Count];
                //using (var iter = tables.GetEnumerator())
                //{
                //    for (int i = 0; i < array.Length; ++i)
                //    {
                //        array[i] = iter.Current;
                //    }
                //}
                //return array;
            }
            return null;
        }

        public IDictionary<string, string> GetTableAlias()
        {
            return tableAlias;
        }

        /// <returns>-1 for no limit</returns>
        public long GetLimitOffset()
        {
            return limitOffset;
        }

        /// <returns>-1 for no limit</returns>
        public long GetLimitSize()
        {
            return limitSize;
        }

        /// <returns>
        ///
        /// <see cref="GroupNon"/>
        /// or
        /// <see cref="GroupSum"/>
        /// or
        /// <see cref="GroupMin"/>
        /// or
        /// <see cref="GroupMax"/>
        /// </returns>
        public int GetGroupFuncType()
        {
            return groupFuncType;
        }

        public bool IsSchemaTrimmed()
        {
            return schemaTrimmed;
        }

        /// <returns>never null</returns>
        public IDictionary<string, ColumnValueType> GetColumnIndex(string tableNameUp)
        {
            if (columnValueIndex == null)
            {
                return new Dictionary<string, ColumnValueType>(0);
            }
            var index = columnValueIndex.GetValue(tableNameUp);
            if (index == null || index.IsEmpty())
            {
                return new Dictionary<string, ColumnValueType>(0);
            }
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
            {
                return;
            }
            IDictionary<string, IList<object>> colMap = null;
            if (initColumnMapSize > 0)
            {
                colMap = new Dictionary<string, IList<object>>(initColumnMapSize);
            }
            else
            {
                colMap = new Dictionary<string, IList<object>>(0);
            }
            columnValue[tableNameUp] = colMap;
        }

        private void AddColumnValueIndex(string table,
            string column,
            object value,
            Expr expr,
            ASTNode parent)
        {
            var colMap = EnsureColumnValueIndexByTable(table);
            var valMap = EnsureColumnValueIndexObjMap(colMap, column);
            AddIntoColumnValueIndex(valMap, value, expr, parent);
        }

        private void AddIntoColumnValueIndex(ColumnValueType valMap,
            object value,
            Expr expr,
            ASTNode parent)
        {
            var exprSet = value == null ? null : valMap.GetValue(value);
            if (exprSet == null)
            {
                exprSet = new HashSet<Pair<Expr, ASTNode>>();
                valMap[value ?? Null_Alias_Key] = exprSet;
            }
            var pair = new Pair<Expr, ASTNode>(expr, parent);
            exprSet.Add(pair);
        }

        private IDictionary<string, ColumnValueType> EnsureColumnValueIndexByTable(string table)
        {
            if (columnValueIndex == null)
            {
                columnValueIndex = new Dictionary<string, IDictionary<string, ColumnValueType>>();
            }
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
                valMap = new Dictionary<object, ICollection<Pair<Expr, ASTNode>>>();
                colMap[column ?? Null_Alias_Key] = valMap;
            }
            return valMap;
        }

        private void AddColumnValue(string tableNameUp,
            string columnNameUp,
            object value,
            Expr expr,
            ASTNode parent)
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

        private readonly IDictionary<object, object> evaluationParameter = new Dictionary<object, object>();

        private readonly IDictionary<string, TableConfig> tablesRuleConfig;

        private bool verdictColumn = true;

        private int idLevel = 2;

        private bool verdictGroupFunc = true;

        private string trimSchema;

        public PartitionKeyVisitor(IDictionary<string, TableConfig> tables)
        {
            // ---temp
            // state------------------------------------------------------------------
            if (tables == null || tables.IsEmpty())
            {
                tables = new Dictionary<string, TableConfig>(0);
            }
            this.tablesRuleConfig = tables;
        }

        public PartitionKeyVisitor SetTrimSchema(string trimSchema)
        {
            if (trimSchema != null)
            {
                this.trimSchema = trimSchema.ToUpper();
            }
            return this;
        }

        private bool IsRuledColumn(string tableNameUp, string columnNameUp)
        {
            if (tableNameUp == null)
            {
                return false;
            }
            var config = tablesRuleConfig.GetValue(tableNameUp);
            if (config != null)
            {
                return config.ExistsColumn(columnNameUp);
            }
            return false;
        }

        private void VisitChild(int idLevel,
            bool verdictColumn,
            bool verdictGroupFunc,
            params ASTNode[] nodes)
        {
            if (nodes == null || nodes.Length <= 0)
                return;

            VisitChild(idLevel, verdictColumn, verdictGroupFunc, new List<ASTNode>(nodes));
        }

        private void VisitChild<TNode>(int idLevel,
            bool verdictColumn,
            bool verdictGroupFunc,
            IList<TNode> nodes)
            where TNode : ASTNode
        {
            if (nodes == null || nodes.IsEmpty())
            {
                return;
            }
            int oldLevel = this.idLevel;
            bool oldVerdict = this.verdictColumn;
            bool oldverdictGroupFunc = this.verdictGroupFunc;
            this.idLevel = idLevel;
            this.verdictColumn = verdictColumn;
            this.verdictGroupFunc = verdictGroupFunc;
            try
            {
                foreach (ASTNode node in nodes)
                {
                    if (node != null)
                    {
                        node.Accept(this);
                    }
                }
            }
            finally
            {
                this.verdictColumn = oldVerdict;
                this.idLevel = oldLevel;
                this.verdictGroupFunc = oldverdictGroupFunc;
            }
        }

        // --------------------------------------------------------------------------------
        private void Limit(Limit limit)
        {
            if (limit != null)
            {
                object ls = limit.GetSize();
                if (ls is Expr)
                {
                    ls = ((Expr)ls).Evaluation(evaluationParameter);
                }
                if (ls is int)
                {
                    limitSize = ((int)ls);
                }
                object lo = limit.GetOffset();
                if (lo is Expr)
                {
                    lo = ((Expr)lo).Evaluation(evaluationParameter);
                }
                if (lo is int)
                {
                    this.limitOffset = ((int)lo);
                }
            }
        }

        public void Visit(DMLSelectStatement node)
        {
            bool verdictGroup = true;
            var exprList = node.GetSelectExprListWithoutAlias();
            if (verdictGroupFunc)
            {
                foreach (Expr expr in exprList)
                {
                    if (!IsGroupFuncPassthroughSelect(expr))
                    {
                        groupFuncType = GroupCancel;
                        verdictGroup = false;
                        break;
                    }
                }
                Limit(node.GetLimit());
            }
            VisitChild(2, false, verdictGroupFunc && verdictGroup, exprList);
            TableReference tr = node.GetTables();
            VisitChild(1, verdictColumn, verdictGroupFunc && verdictGroup, tr);
            Expr where = node.GetWhere();
            VisitChild(2, verdictColumn, false, where);
            GroupBy group = node.GetGroup();
            VisitChild(2, false, false, group);
            Expr having = node.GetHaving();
            VisitChild(2, verdictColumn, false, having);
            OrderBy order = node.GetOrder();
            VisitChild(2, false, false, order);
        }

        public void Visit(DMLSelectUnionStatement node)
        {
            VisitChild(2, false, false, node.GetOrderBy());
            VisitChild(2, false, false, node.GetSelectStmtList());
        }

        public void Visit(DMLUpdateStatement node)
        {
            TableReference tr = node.GetTableRefs();
            VisitChild(1, false, false, tr);
            var assignmentList = node.GetValues();
            if (assignmentList != null && !assignmentList.IsEmpty())
            {
                IList<ASTNode> list = new List<ASTNode>(assignmentList.Count * 2);
                foreach (Pair<Identifier, Expr> p in assignmentList)
                {
                    if (p == null)
                    {
                        continue;
                    }
                    list.Add(p.GetKey());
                    list.Add(p.GetValue());
                }
                VisitChild(2, false, false, list);
            }
            Expr where = node.GetWhere();
            VisitChild(2, verdictColumn, false, where);
            OrderBy order = node.GetOrderBy();
            VisitChild(2, false, false, order);
        }

        private void TableAsTableFactor(Identifier table)
        {
            int trim = table.TrimParent(1, trimSchema);
            schemaTrimmed = schemaTrimmed || trim == Identifier.ParentTrimed;
            customedSchema = customedSchema || trim == Identifier.ParentIgnored;
            string tableName = table.GetIdTextUpUnescape();
            tableAlias[Null_Alias_Key] = tableName;
            tableAlias[tableName] = tableName;
            AddTable(tableName);
        }

        public void Visit(DMLDeleteStatement node)
        {
            TableReference tr = node.GetTableRefs();
            IList<Identifier> tbs = node.GetTableNames();
            if (tr == null)
            {
                Identifier table = tbs[0];
                TableAsTableFactor(table);
            }
            else
            {
                VisitChild(1, verdictColumn, false, tr);
                foreach (Identifier tb in tbs)
                {
                    if (tb is Wildcard)
                    {
                        int trim = tb.TrimParent(2, trimSchema);
                        schemaTrimmed = schemaTrimmed || trim == Identifier.ParentTrimed;
                        customedSchema = customedSchema || trim == Identifier.ParentIgnored;
                    }
                    else
                    {
                        int trim = tb.TrimParent(1, trimSchema);
                        schemaTrimmed = schemaTrimmed || trim == Identifier.ParentTrimed;
                        customedSchema = customedSchema || trim == Identifier.ParentIgnored;
                    }
                }
            }
            Expr where = node.GetWhereCondition();
            VisitChild(2, verdictColumn, false, where);
            if (tr == null)
            {
                OrderBy order = node.GetOrderBy();
                VisitChild(2, false, false, order);
            }
        }

        private void InsertReplace(DMLInsertReplaceStatement node)
        {
            Identifier table = node.GetTable();
            IList<Identifier> collist = node.GetColumnNameList();
            QueryExpression query = node.GetSelect();
            IList<RowExpression> rows = node.GetRowList();
            TableAsTableFactor(table);
            string tableName = table.GetIdTextUpUnescape();
            VisitChild(2, false, false, collist);
            if (query != null)
            {
                query.Accept(this);
                return;
            }
            foreach (RowExpression row in rows)
            {
                VisitChild(2, false, false, row);
            }
            var colVals = EnsureColumnValueByTable(tableName);
            var colValsIndex = EnsureColumnValueIndexByTable(tableName);
            if (collist != null)
            {
                for (int i = 0; i < collist.Count; ++i)
                {
                    string colName = collist[i].GetIdTextUpUnescape();
                    if (IsRuledColumn(tableName, colName))
                    {
                        IList<object> valueList = EnsureColumnValueList(colVals, colName);
                        var valMap = EnsureColumnValueIndexObjMap(colValsIndex, colName);
                        foreach (RowExpression row_1 in rows)
                        {
                            Expr expr = row_1.GetRowExprList()[i];
                            object value = expr == null ? null : expr.Evaluation(evaluationParameter);
                            if (value != ExpressionConstants.Unevaluatable)
                            {
                                valueList.Add(value);
                                AddIntoColumnValueIndex(valMap, value, row_1, node);
                            }
                        }
                    }
                }
            }
        }

        public void Visit(DMLInsertStatement node)
        {
            InsertReplace(node);
            var dup = node.GetDuplicateUpdate();
            if (dup != null)
            {
                ASTNode[] duplist = new ASTNode[dup.Count * 2];
                int i = 0;
                foreach (Pair<Identifier, Expr> p in dup)
                {
                    Identifier key = null;
                    Expr value = null;
                    if (p != null)
                    {
                        key = p.GetKey();
                        value = p.GetValue();
                    }
                    duplist[i++] = key;
                    duplist[i++] = value;
                }
                VisitChild(2, false, false, duplist);
            }
        }

        public void Visit(DMLReplaceStatement node)
        {
            InsertReplace(node);
        }

        private void DdlTable(Identifier id, int idLevel)
        {
            VisitChild(idLevel, false, false, id);
            AddTable(id.GetIdTextUpUnescape());
        }

        public void Visit(DDLTruncateStatement node)
        {
            DdlTable(node.GetTable(), 1);
        }

        public void Visit(DDLAlterTableStatement node)
        {
            DdlTable(node.GetTable(), 1);
        }

        public void Visit(DDLCreateIndexStatement node)
        {
            DdlTable(node.GetTable(), 1);
            DdlTable(node.GetIndexName(), 1);
        }

        public void Visit(DDLCreateTableStatement node)
        {
            DdlTable(node.GetTable(), 1);
        }

        public void Visit(DDLRenameTableStatement node)
        {
            var list = node.GetList();
            IList<Identifier> idl = new List<Identifier>(list.Count * 2);
            foreach (Pair<Identifier, Identifier> p in list)
            {
                if (p != null)
                {
                    if (p.GetKey() != null)
                    {
                        AddTable(p.GetKey().GetIdTextUpUnescape());
                        idl.Add(p.GetKey());
                    }
                    idl.Add(p.GetValue());
                }
            }
            VisitChild(1, false, false, idl);
        }

        public void Visit(DDLDropIndexStatement node)
        {
            DdlTable(node.GetTable(), 1);
            DdlTable(node.GetIndexName(), 1);
        }

        public void Visit(DDLDropTableStatement node)
        {
            VisitChild(1, false, false, node.GetTableNames());
            IList<Identifier> tbs = node.GetTableNames();
            if (tbs != null)
            {
                foreach (Identifier tb in tbs)
                {
                    AddTable(tb.GetIdTextUpUnescape());
                }
            }
        }

        public void Visit(BetweenAndExpression node)
        {
            Expr fst = node.GetFirst();
            Expr snd = node.GetSecond();
            Expr trd = node.GetThird();
            VisitChild(2, false, false, fst, snd, trd);
            if (verdictColumn && !node.IsNot() && fst is Identifier)
            {
                Identifier col = (Identifier)fst;
                string table = tableAlias.GetValue(col.GetLevelUnescapeUpName(2) ?? Null_Alias_Key);
                if (IsRuledColumn(table, col.GetIdTextUpUnescape()))
                {
                    object e1 = snd.Evaluation(evaluationParameter);
                    object e2 = trd.Evaluation(evaluationParameter);
                    if (e1 != ExpressionConstants.Unevaluatable
                        && e2 != ExpressionConstants.Unevaluatable
                        && e1 != null
                        && e2 != null)
                    {
                        if (CompareEvaluatedValue(e1, e2))
                        {
                            AddColumnValue(table, col.GetIdTextUpUnescape(), e1, node, null);
                        }
                    }
                }
            }
        }

        /// <param name="obj1">not null</param>
        /// <param name="obj2">not null</param>
        private static bool CompareEvaluatedValue(object obj1, object obj2)
        {
            if (obj1.Equals(obj2))
            {
                return true;
            }
            try
            {
                Pair<Number, Number> pair = ExprEvalUtils.ConvertNum2SameLevel(obj1, obj2);
                return pair.GetKey().Equals(pair.GetValue());
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void Visit(ComparisionIsExpression node)
        {
            Expr operand = node.GetOperand();
            VisitChild(2, false, false, operand);
            if (verdictColumn && (operand is Identifier))
            {
                Identifier col = (Identifier)operand;
                string table = tableAlias.GetValue(col.GetLevelUnescapeUpName(2) ?? Null_Alias_Key);
                if (IsRuledColumn(table, col.GetIdTextUpUnescape()))
                {
                    switch (node.GetMode())
                    {
                        case ComparisionIsExpression.IsFalse:
                            {
                                AddColumnValue(table, col.GetIdTextUpUnescape(), LiteralBoolean.False, node, null);
                                break;
                            }

                        case ComparisionIsExpression.IsTrue:
                            {
                                AddColumnValue(table, col.GetIdTextUpUnescape(), LiteralBoolean.True, node, null);
                                break;
                            }

                        case ComparisionIsExpression.IsNull:
                            {
                                AddColumnValue(table, col.GetIdTextUpUnescape(), null, node, null);
                                break;
                            }
                    }
                }
            }
        }

        public void Visit(InExpressionList node)
        {
            VisitChild(2, false, false, node.GetList());
        }

        public void Visit(BinaryOperatorExpression node)
        {
            Expr left = node.GetLeftOprand();
            Expr right = node.GetRightOprand();
            VisitChild(2, false, false, left, right);
        }

        public void Visit(PolyadicOperatorExpression node)
        {
        }

        // QS_TODO
        public void Visit(ComparisionEqualsExpression node)
        {
            Expr left = node.GetLeftOprand();
            Expr right = node.GetRightOprand();
            VisitChild(2, false, false, left, right);
            if (verdictColumn)
            {
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
        }

        public void Visit(ComparisionNullSafeEqualsExpression node)
        {
            Expr left = node.GetLeftOprand();
            Expr right = node.GetRightOprand();
            VisitChild(2, false, false, left, right);
            if (verdictColumn)
            {
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
        }

        private void ComparisionEquals(Identifier col, object value, bool nullsafe, Expr node)
        {
            if (value != ExpressionConstants.Unevaluatable
                && (nullsafe || value != null))
            {
                string table = tableAlias.GetValue(col.GetLevelUnescapeUpName(2) ?? Null_Alias_Key);
                if (IsRuledColumn(table, col.GetIdTextUpUnescape()))
                {
                    AddColumnValue(table, col.GetIdTextUpUnescape(), value, node, null);
                }
            }
        }

        public void Visit(InExpression node)
        {
            Expr left = node.GetLeftOprand();
            Expr right = node.GetRightOprand();
            VisitChild(2, false, false, left, right);
            if (verdictColumn && !node.IsNot() && left is Identifier && right is InExpressionList)
            {
                var col = (Identifier)left;
                string colName = col.GetIdTextUpUnescape();
                string table = tableAlias.GetValue(col.GetLevelUnescapeUpName(2) ?? Null_Alias_Key);
                if (IsRuledColumn(table, colName))
                {
                    var valList = EnsureColumnValueList(EnsureColumnValueByTable(table), colName);
                    var valMap = EnsureColumnValueIndexObjMap(EnsureColumnValueIndexByTable(table), colName);
                    var inlist = (InExpressionList)right;
                    foreach (Expr expr in inlist.GetList())
                    {
                        object value = expr.Evaluation(evaluationParameter);
                        if (value != ExpressionConstants.Unevaluatable)
                        {
                            valList.Add(value);
                            AddIntoColumnValueIndex(valMap, value, expr, node);
                        }
                    }
                }
            }
        }

        public void Visit(LogicalAndExpression node)
        {
            for (int i = 0, len = node.GetArity(); i < len; ++i)
            {
                Expr oprand = node.GetOperand(i);
                VisitChild(2, verdictColumn && IsVerdictPassthroughWhere(oprand), false, oprand);
            }
        }

        public void Visit(LogicalOrExpression node)
        {
            for (int i = 0, len = node.GetArity(); i < len; ++i)
            {
                Expr oprand = node.GetOperand(i);
                VisitChild(2, verdictColumn && IsVerdictPassthroughWhere(oprand), false, oprand);
            }
        }

        public void Visit(Count node)
        {
            VisitChild(2, false, false, node.GetArguments());
            if (verdictGroupFunc)
            {
                if (groupFuncType != GroupNon && groupFuncType != GroupSum || node.IsDistinct())
                {
                    groupFuncType = GroupCancel;
                }
                else
                {
                    groupFuncType = GroupSum;
                }
            }
        }

        public void Visit(Sum node)
        {
            VisitChild(2, false, false, node.GetArguments());
            if (verdictGroupFunc)
            {
                if (groupFuncType != GroupNon && groupFuncType != GroupSum || node.IsDistinct())
                {
                    groupFuncType = GroupCancel;
                }
                else
                {
                    groupFuncType = GroupSum;
                }
            }
        }

        public void Visit(Max node)
        {
            VisitChild(2, false, false, node.GetArguments());
            if (verdictGroupFunc)
            {
                if (groupFuncType != GroupNon && groupFuncType != GroupMax)
                {
                    groupFuncType = GroupCancel;
                }
                else
                {
                    groupFuncType = GroupMax;
                }
            }
        }

        public void Visit(Min node)
        {
            VisitChild(2, false, false, node.GetArguments());
            if (verdictGroupFunc)
            {
                if (groupFuncType != GroupNon && groupFuncType != GroupMin)
                {
                    groupFuncType = GroupCancel;
                }
                else
                {
                    groupFuncType = GroupMin;
                }
            }
        }

        public void Visit(Identifier node)
        {
            int trim = node.TrimParent(idLevel, trimSchema);
            schemaTrimmed = schemaTrimmed || trim == Identifier.ParentTrimed;
            customedSchema = customedSchema || trim == Identifier.ParentIgnored;
        }

        public void Visit(InnerJoin node)
        {
            TableReference tr1 = node.GetLeftTableRef();
            TableReference tr2 = node.GetRightTableRef();
            Expr on = node.GetOnCond();
            VisitChild(1, verdictColumn, verdictGroupFunc, tr1, tr2);
            VisitChild(2, verdictColumn && IsVerdictPassthroughWhere(on), false, on);
        }

        public void Visit(NaturalJoin node)
        {
            TableReference tr1 = node.GetLeftTableRef();
            TableReference tr2 = node.GetRightTableRef();
            VisitChild(1, verdictColumn, verdictGroupFunc, tr1, tr2);
        }

        public void Visit(OuterJoin node)
        {
            TableReference tr1 = node.GetLeftTableRef();
            TableReference tr2 = node.GetRightTableRef();
            Expr on = node.GetOnCond();
            VisitChild(1, verdictColumn, verdictGroupFunc, tr1, tr2);
            VisitChild(2, verdictColumn && IsVerdictPassthroughWhere(on), false, on);
        }

        public void Visit(StraightJoin node)
        {
            TableReference tr1 = node.GetLeftTableRef();
            TableReference tr2 = node.GetRightTableRef();
            Expr on = node.GetOnCond();
            VisitChild(1, verdictColumn, verdictGroupFunc, tr1, tr2);
            VisitChild(2, verdictColumn && IsVerdictPassthroughWhere(on), false, on);
        }

        public void Visit(TableReferences node)
        {
            IList<TableReference> list = node.GetTableReferenceList();
            VisitChild(1, verdictColumn, verdictGroupFunc, list);
        }

        public void Visit(SubqueryFactor node)
        {
            QueryExpression query = node.GetSubquery();
            VisitChild(2, verdictColumn, verdictGroupFunc, query);
        }

        public void Visit(TableRefFactor node)
        {
            //TODO Visit(TableRefFactor node) _NULL_ALIAS_
            Identifier table = node.GetTable();
            VisitChild(1, false, false, table);
            string tableName = table.GetIdTextUpUnescape();
            AddTable(tableName);
            string alias = node.GetAliasUnescapeUppercase();
            if (alias == null)
            {
                tableAlias[Null_Alias_Key] = tableName;
                tableAlias[tableName] = tableName;
            }
            else
            {
                if (!tableAlias.ContainsKey(Null_Alias_Key))
                {
                    tableAlias[Null_Alias_Key] = tableName;
                }
                tableAlias[alias] = tableName;
            }
        }

        public void Visit(Dual dual)
        {
            this.dual = true;
        }

        // ------------------------------------------------------------------------------
        public void Visit(LikeExpression node)
        {
            VisitChild(2, false, false, node.GetFirst(), node.GetSecond(), node.GetThird());
        }

        public void Visit(CollateExpression node)
        {
            VisitChild(2, false, false, node.GetString());
        }

        public void Visit(UserExpression node)
        {
        }

        public void Visit(UnaryOperatorExpression node)
        {
            VisitChild(2, false, false, node.GetOperand());
        }

        public void Visit(FunctionExpression node)
        {
            VisitChild(2, false, false, node.GetArguments());
        }

        public void Visit(Parser.Ast.Expression.Primary.Function.String.Char node)
        {
            VisitChild(2, false, false, node.GetArguments());
        }

        public void Visit(Parser.Ast.Expression.Primary.Function.Cast.Convert node)
        {
            VisitChild(2, false, false, node.GetArguments());
        }

        public void Visit(Trim node)
        {
            VisitChild(2, false, false, node.GetArguments());
        }

        public void Visit(Tup.Cobar4Net.Parser.Ast.Expression.Primary.Function.Cast.Cast
            node)
        {
            VisitChild(2, false, false, node.GetArguments());
            VisitChild(2, false, false, node.GetTypeInfo1(), node.GetTypeInfo2());
        }

        public void Visit(Avg node)
        {
            VisitChild(2, false, false, node.GetArguments());
        }

        public void Visit(GroupConcat node)
        {
            VisitChild(2, false, false, node.GetArguments());
        }

        public void Visit(IntervalPrimary node)
        {
            VisitChild(2, false, false, node.GetQuantity());
        }

        public void Visit(Extract node)
        {
            VisitChild(2, false, false, node.GetArguments());
        }

        public void Visit(Timestampdiff node)
        {
            VisitChild(2, false, false, node.GetArguments());
        }

        public void Visit(Timestampadd node)
        {
            VisitChild(2, false, false, node.GetArguments());
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
            VisitChild(2, false, false, node.GetComparee(), node.GetElseResult());
            IList<Pair<Expr, Expr>> whenPairList = node.GetWhenList();
            if (whenPairList == null || whenPairList.IsEmpty())
            {
                return;
            }
            IList<Expr> list = new List<Expr>(whenPairList.Count * 2);
            foreach (Pair<Expr, Expr> pair in whenPairList)
            {
                if (pair == null)
                {
                    continue;
                }
                list.Add(pair.GetKey());
                list.Add(pair.GetValue());
            }
            VisitChild(2, false, false, list);
        }

        public void Visit(DefaultValue node)
        {
        }

        public void Visit(ExistsPrimary node)
        {
            VisitChild(2, false, false, node.GetSubquery());
        }

        public void Visit(PlaceHolder node)
        {
        }

        public void Visit(MatchExpression node)
        {
            VisitChild(2, false, false, node.GetColumns());
            VisitChild(2, false, false, node.GetPattern());
        }

        public void Visit(ParamMarker node)
        {
        }

        public void Visit(RowExpression node)
        {
            VisitChild(2, false, false, node.GetRowExprList());
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
            SortPairList(node.GetOrderByList());
        }

        public void Visit(OrderBy node)
        {
            SortPairList(node.GetOrderByList());
        }

        private void SortPairList(IList<Pair<Expr, SortOrder>> list)
        {
            if (list == null || list.IsEmpty())
            {
                return;
            }
            Expr[] exprs = new Expr[list.Count];
            int i = 0;
            foreach (Pair<Expr, SortOrder> p in list)
            {
                exprs[i] = p.GetKey();
                ++i;
            }
            VisitChild(2, false, false, new List<Expr>(exprs));
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

        public void Visit(DDLAlterTableStatement.AlterSpecification node)
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
            TableMetaRead(node.GetTable());
        }

        public void Visit(ShowContributors node)
        {
        }

        public void Visit(ShowCreate node)
        {
            if (node.GetCreateType() == ShowCreate.CreateType.Table)
            {
                TableMetaRead(node.GetId());
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
            if (node.GetSchema() != null)
            {
                schemaTrimmed = true;
                node.SetSchema(null);
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
            TableMetaRead(node.GetTable());
        }

        public void Visit(ShowMasterStatus node)
        {
        }

        public void Visit(ShowOpenTables node)
        {
            if (node.GetSchema() != null)
            {
                schemaTrimmed = true;
                node.SetSchema(null);
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
            if (node.GetSchema() != null)
            {
                schemaTrimmed = true;
                node.SetSchema(null);
            }
            rewriteField = true;
            TableMetaRead(null);
        }

        public void Visit(ShowTableStatus node)
        {
            if (node.GetDatabase() != null)
            {
                schemaTrimmed = true;
                node.SetDatabase(null);
            }
            TableMetaRead(null);
        }

        public void Visit(ShowTriggers node)
        {
            if (node.GetSchema() != null)
            {
                schemaTrimmed = true;
                node.SetSchema(null);
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
            TableMetaRead(node.GetTable());
        }

        private void TableMetaRead(Identifier table)
        {
            if (table != null)
            {
                VisitChild(1, false, false, table);
                AddTable(table.GetIdTextUpUnescape(), 0);
            }
            tableMetaRead = true;
        }

        public void Visit(DALSetStatement node)
        {
        }

        public void Visit(DALSetNamesStatement node)
        {
        }

        public void Visit(DALSetCharacterSetStatement node)
        {
        }

        public void Visit(DMLCallStatement node)
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

        public void Visit(ExtDDLCreatePolicy node)
        {
        }

        public void Visit(ExtDDLDropPolicy node)
        {
        }
    }
}