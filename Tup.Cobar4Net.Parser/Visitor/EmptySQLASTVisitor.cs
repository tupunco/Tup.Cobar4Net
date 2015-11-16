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

using System.Collections;
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
using Tup.Cobar4Net.Parser.Ast.Fragment.Ddl.Datatype;
using Tup.Cobar4Net.Parser.Ast.Fragment.Ddl.Index;
using Tup.Cobar4Net.Parser.Ast.Fragment.Tableref;
using Tup.Cobar4Net.Parser.Ast.Stmt.Dal;
using Tup.Cobar4Net.Parser.Ast.Stmt.Ddl;
using Tup.Cobar4Net.Parser.Ast.Stmt.Dml;
using Tup.Cobar4Net.Parser.Ast.Stmt.Extension;
using Tup.Cobar4Net.Parser.Ast.Stmt.Mts;
using Tup.Cobar4Net.Parser.Util;

namespace Tup.Cobar4Net.Parser.Visitor
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class EmptySQLASTVisitor : SQLASTVisitor
    {
        private void VisitInternal(object obj)
        {
            if (obj == null)
            {
                return;
            }
            if (obj is ASTNode)
            {
                ((ASTNode)obj).Accept(this);
            }
            else
            {
                if (obj is ICollection)
                {
                    foreach (object o in (ICollection)obj)
                    {
                        VisitInternal(o);
                    }
                }
                else
                {
                    //INFO EmptySQLASTVisitor.VisitInternal
                    var t = obj.GetType();
                    if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Pair<,>))
                    {
                        VisitInternal(t.GetMethod("GetKey").Invoke(obj, new object[0]));
                        VisitInternal(t.GetMethod("GetValue").Invoke(obj, new object[0]));
                    }
                    //if (obj is Pair)
                    //{
                    //    VisitInternal(((Pair)obj).GetKey());
                    //    VisitInternal(((Pair)obj).GetValue());
                    //}
                }
            }
        }

        public virtual void Visit(BetweenAndExpression node)
        {
            VisitInternal(node.GetFirst());
            VisitInternal(node.GetSecond());
            VisitInternal(node.GetThird());
        }

        public virtual void Visit(ComparisionIsExpression node)
        {
            VisitInternal(node.GetOperand());
        }

        public virtual void Visit(InExpressionList node)
        {
            VisitInternal(node.GetList());
        }

        public virtual void Visit(LikeExpression node)
        {
            VisitInternal(node.GetFirst());
            VisitInternal(node.GetSecond());
            VisitInternal(node.GetThird());
        }

        public virtual void Visit(CollateExpression node)
        {
            VisitInternal(node.GetString());
        }

        public virtual void Visit(UserExpression node)
        {
        }

        public virtual void Visit(UnaryOperatorExpression node)
        {
            VisitInternal(node.GetOperand());
        }

        public virtual void Visit(BinaryOperatorExpression node)
        {
            VisitInternal(node.GetLeftOprand());
            VisitInternal(node.GetRightOprand());
        }

        public virtual void Visit(PolyadicOperatorExpression node)
        {
            for (int i = 0, len = node.GetArity(); i < len; ++i)
            {
                VisitInternal(node.GetOperand(i));
            }
        }

        public virtual void Visit(LogicalAndExpression node)
        {
            Visit((PolyadicOperatorExpression)node);
        }

        public virtual void Visit(LogicalOrExpression node)
        {
            Visit((PolyadicOperatorExpression)node);
        }

        public virtual void Visit(ComparisionEqualsExpression node)
        {
            Visit((BinaryOperatorExpression)node);
        }

        public virtual void Visit(ComparisionNullSafeEqualsExpression node)
        {
            Visit((BinaryOperatorExpression)node);
        }

        public virtual void Visit(InExpression node)
        {
            Visit((BinaryOperatorExpression)node);
        }

        public virtual void Visit(FunctionExpression node)
        {
            VisitInternal(node.GetArguments());
        }

        public virtual void Visit(Char node)
        {
            Visit((FunctionExpression)node);
        }

        public virtual void Visit(Convert node)
        {
            Visit((FunctionExpression)node);
        }

        public virtual void Visit(Trim node)
        {
            Visit((FunctionExpression)node);
            VisitInternal(node.GetRemainString());
            VisitInternal(node.GetString());
        }

        public virtual void Visit(Cast node)
        {
            Visit((FunctionExpression)node);
            VisitInternal(node.GetExpr());
            VisitInternal(node.GetTypeInfo1());
            VisitInternal(node.GetTypeInfo2());
        }

        public virtual void Visit(Avg node)
        {
            Visit((FunctionExpression)node);
        }

        public virtual void Visit(Max node)
        {
            Visit((FunctionExpression)node);
        }

        public virtual void Visit(Min node)
        {
            Visit((FunctionExpression)node);
        }

        public virtual void Visit(Sum node)
        {
            Visit((FunctionExpression)node);
        }

        public virtual void Visit(Count node)
        {
            Visit((FunctionExpression)node);
        }

        public virtual void Visit(GroupConcat node)
        {
            Visit((FunctionExpression)node);
            VisitInternal(node.GetAppendedColumnNames());
            VisitInternal(node.GetOrderBy());
        }

        public virtual void Visit(Timestampdiff node)
        {
        }

        public virtual void Visit(Timestampadd node)
        {
        }

        public virtual void Visit(Extract node)
        {
        }

        public virtual void Visit(GetFormat node)
        {
        }

        public virtual void Visit(IntervalPrimary node)
        {
            VisitInternal(node.GetQuantity());
        }

        public virtual void Visit(LiteralBitField node)
        {
        }

        public virtual void Visit(LiteralBoolean node)
        {
        }

        public virtual void Visit(LiteralHexadecimal node)
        {
        }

        public virtual void Visit(LiteralNull node)
        {
        }

        public virtual void Visit(LiteralNumber node)
        {
        }

        public virtual void Visit(LiteralString node)
        {
        }

        public virtual void Visit(CaseWhenOperatorExpression node)
        {
            VisitInternal(node.GetComparee());
            VisitInternal(node.GetElseResult());
            VisitInternal(node.GetWhenList());
        }

        public virtual void Visit(DefaultValue node)
        {
        }

        public virtual void Visit(ExistsPrimary node)
        {
            VisitInternal(node.GetSubquery());
        }

        public virtual void Visit(PlaceHolder node)
        {
        }

        public virtual void Visit(Identifier node)
        {
        }

        public virtual void Visit(MatchExpression node)
        {
            VisitInternal(node.GetColumns());
            VisitInternal(node.GetPattern());
        }

        public virtual void Visit(ParamMarker node)
        {
        }

        public virtual void Visit(RowExpression node)
        {
            VisitInternal(node.GetRowExprList());
        }

        public virtual void Visit(SysVarPrimary node)
        {
        }

        public virtual void Visit(UsrDefVarPrimary node)
        {
        }

        public virtual void Visit(IndexHint node)
        {
        }

        public virtual void Visit(InnerJoin node)
        {
            VisitInternal(node.GetLeftTableRef());
            VisitInternal(node.GetOnCond());
            VisitInternal(node.GetRightTableRef());
        }

        public virtual void Visit(NaturalJoin node)
        {
            VisitInternal(node.GetLeftTableRef());
            VisitInternal(node.GetRightTableRef());
        }

        public virtual void Visit(OuterJoin node)
        {
            VisitInternal(node.GetLeftTableRef());
            VisitInternal(node.GetOnCond());
            VisitInternal(node.GetRightTableRef());
        }

        public virtual void Visit(StraightJoin node)
        {
            VisitInternal(node.GetLeftTableRef());
            VisitInternal(node.GetOnCond());
            VisitInternal(node.GetRightTableRef());
        }

        public virtual void Visit(SubqueryFactor node)
        {
            VisitInternal(node.GetSubquery());
        }

        public virtual void Visit(TableReferences node)
        {
            VisitInternal(node.GetTableReferenceList());
        }

        public virtual void Visit(TableRefFactor node)
        {
            VisitInternal(node.GetHintList());
            VisitInternal(node.GetTable());
        }

        public virtual void Visit(Dual dual)
        {
        }

        public virtual void Visit(GroupBy node)
        {
            VisitInternal(node.GetOrderByList());
        }

        public virtual void Visit(Limit node)
        {
            VisitInternal(node.GetOffset());
            VisitInternal(node.GetSize());
        }

        public virtual void Visit(OrderBy node)
        {
            VisitInternal(node.GetOrderByList());
        }

        public virtual void Visit(ColumnDefinition columnDefinition)
        {
        }

        public virtual void Visit(IndexOption indexOption)
        {
        }

        public virtual void Visit(IndexColumnName indexColumnName)
        {
        }

        public virtual void Visit(TableOptions node)
        {
        }

        public virtual void Visit(DDLAlterTableStatement.AlterSpecification node)
        {
        }

        public virtual void Visit(DataType node)
        {
        }

        public virtual void Visit(ShowAuthors node)
        {
        }

        public virtual void Visit(ShowBinaryLog node)
        {
        }

        public virtual void Visit(ShowBinLogEvent node)
        {
            VisitInternal(node.GetLimit());
            VisitInternal(node.GetPos());
        }

        public virtual void Visit(ShowCharaterSet node)
        {
            VisitInternal(node.GetWhere());
        }

        public virtual void Visit(ShowCollation node)
        {
            VisitInternal(node.GetWhere());
        }

        public virtual void Visit(ShowColumns node)
        {
            VisitInternal(node.GetTable());
            VisitInternal(node.GetWhere());
        }

        public virtual void Visit(ShowContributors node)
        {
        }

        public virtual void Visit(ShowCreate node)
        {
            VisitInternal(node.GetId());
        }

        public virtual void Visit(ShowDatabases node)
        {
            VisitInternal(node.GetWhere());
        }

        public virtual void Visit(ShowEngine node)
        {
        }

        public virtual void Visit(ShowEngines node)
        {
        }

        public virtual void Visit(ShowErrors node)
        {
            VisitInternal(node.GetLimit());
        }

        public virtual void Visit(ShowEvents node)
        {
            VisitInternal(node.GetSchema());
            VisitInternal(node.GetWhere());
        }

        public virtual void Visit(ShowFunctionCode node)
        {
            VisitInternal(node.GetFunctionName());
        }

        public virtual void Visit(ShowFunctionStatus node)
        {
            VisitInternal(node.GetWhere());
        }

        public virtual void Visit(ShowGrants node)
        {
            VisitInternal(node.GetUser());
        }

        public virtual void Visit(ShowIndex node)
        {
            VisitInternal(node.GetTable());
        }

        public virtual void Visit(ShowMasterStatus node)
        {
        }

        public virtual void Visit(ShowOpenTables node)
        {
            VisitInternal(node.GetSchema());
            VisitInternal(node.GetWhere());
        }

        public virtual void Visit(ShowPlugins node)
        {
        }

        public virtual void Visit(ShowPrivileges node)
        {
        }

        public virtual void Visit(ShowProcedureCode node)
        {
            VisitInternal(node.GetProcedureName());
        }

        public virtual void Visit(ShowProcedureStatus node)
        {
            VisitInternal(node.GetWhere());
        }

        public virtual void Visit(ShowProcesslist node)
        {
        }

        public virtual void Visit(ShowProfile node)
        {
            VisitInternal(node.GetForQuery());
            VisitInternal(node.GetLimit());
        }

        public virtual void Visit(ShowProfiles node)
        {
        }

        public virtual void Visit(ShowSlaveHosts node)
        {
        }

        public virtual void Visit(ShowSlaveStatus node)
        {
        }

        public virtual void Visit(ShowStatus node)
        {
            VisitInternal(node.GetWhere());
        }

        public virtual void Visit(ShowTables node)
        {
            VisitInternal(node.GetSchema());
            VisitInternal(node.GetWhere());
        }

        public virtual void Visit(ShowTableStatus node)
        {
            VisitInternal(node.GetDatabase());
            VisitInternal(node.GetWhere());
        }

        public virtual void Visit(ShowTriggers node)
        {
            VisitInternal(node.GetSchema());
            VisitInternal(node.GetWhere());
        }

        public virtual void Visit(ShowVariables node)
        {
            VisitInternal(node.GetWhere());
        }

        public virtual void Visit(ShowWarnings node)
        {
            VisitInternal(node.GetLimit());
        }

        public virtual void Visit(DescTableStatement node)
        {
            VisitInternal(node.GetTable());
        }

        public virtual void Visit(DALSetStatement node)
        {
            VisitInternal(node.GetAssignmentList());
        }

        public virtual void Visit(DALSetNamesStatement node)
        {
        }

        public virtual void Visit(DALSetCharacterSetStatement node)
        {
        }

        public virtual void Visit(DMLCallStatement node)
        {
            VisitInternal(node.GetArguments());
            VisitInternal(node.GetProcedure());
        }

        public virtual void Visit(DMLDeleteStatement node)
        {
            VisitInternal(node.GetLimit());
            VisitInternal(node.GetOrderBy());
            VisitInternal(node.GetTableNames());
            VisitInternal(node.GetTableRefs());
            VisitInternal(node.GetWhereCondition());
        }

        public virtual void Visit(DMLInsertStatement node)
        {
            VisitInternal(node.GetColumnNameList());
            VisitInternal(node.GetDuplicateUpdate());
            VisitInternal(node.GetRowList());
            VisitInternal(node.GetSelect());
            VisitInternal(node.GetTable());
        }

        public virtual void Visit(DMLReplaceStatement node)
        {
            VisitInternal(node.GetColumnNameList());
            VisitInternal(node.GetRowList());
            VisitInternal(node.GetSelect());
            VisitInternal(node.GetTable());
        }

        public virtual void Visit(DMLSelectStatement node)
        {
            VisitInternal(node.GetGroup());
            VisitInternal(node.GetHaving());
            VisitInternal(node.GetLimit());
            VisitInternal(node.GetOrder());
            VisitInternal(node.GetSelectExprList());
            VisitInternal(node.GetTables());
            VisitInternal(node.GetWhere());
        }

        public virtual void Visit(DMLSelectUnionStatement node)
        {
            VisitInternal(node.GetLimit());
            VisitInternal(node.GetOrderBy());
            VisitInternal(node.GetSelectStmtList());
        }

        public virtual void Visit(DMLUpdateStatement node)
        {
            VisitInternal(node.GetLimit());
            VisitInternal(node.GetOrderBy());
            VisitInternal(node.GetTableRefs());
            VisitInternal(node.GetValues());
            VisitInternal(node.GetWhere());
        }

        public virtual void Visit(MTSSetTransactionStatement node)
        {
        }

        public virtual void Visit(MTSSavepointStatement node)
        {
            VisitInternal(node.GetSavepoint());
        }

        public virtual void Visit(MTSReleaseStatement node)
        {
            VisitInternal(node.GetSavepoint());
        }

        public virtual void Visit(MTSRollbackStatement node)
        {
            VisitInternal(node.GetSavepoint());
        }

        public virtual void Visit(DDLTruncateStatement node)
        {
            VisitInternal(node.GetTable());
        }

        public virtual void Visit(DDLAlterTableStatement node)
        {
            VisitInternal(node.GetTable());
        }

        public virtual void Visit(DDLCreateIndexStatement node)
        {
            VisitInternal(node.GetIndexName());
            VisitInternal(node.GetTable());
        }

        public virtual void Visit(DDLCreateTableStatement node)
        {
            VisitInternal(node.GetTable());
        }

        public virtual void Visit(DDLRenameTableStatement node)
        {
            VisitInternal(node.GetList());
        }

        public virtual void Visit(DDLDropIndexStatement node)
        {
            VisitInternal(node.GetIndexName());
            VisitInternal(node.GetTable());
        }

        public virtual void Visit(DDLDropTableStatement node)
        {
            VisitInternal(node.GetTableNames());
        }

        public virtual void Visit(ExtDDLCreatePolicy node)
        {
        }

        public virtual void Visit(ExtDDLDropPolicy node)
        {
        }
    }
}