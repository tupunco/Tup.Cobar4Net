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
using Tup.Cobar4Net.Parser.Ast.Fragment.Tableref;
using Tup.Cobar4Net.Parser.Ast.Stmt.Dal;
using Tup.Cobar4Net.Parser.Ast.Stmt.Ddl;
using Tup.Cobar4Net.Parser.Ast.Stmt.Dml;
using Tup.Cobar4Net.Parser.Ast.Stmt.Extension;
using Tup.Cobar4Net.Parser.Ast.Stmt.Mts;
using Tup.Cobar4Net.Parser.Util;

namespace Tup.Cobar4Net.Parser.Visitor
{
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public class EmptySqlAstVisitor : ISqlAstVisitor
    {
        public virtual void Visit(BetweenAndExpression node)
        {
            VisitInternal(node.First);
            VisitInternal(node.Second);
            VisitInternal(node.Third);
        }

        public virtual void Visit(ComparisionIsExpression node)
        {
            VisitInternal(node.Operand);
        }

        public virtual void Visit(InExpressionList node)
        {
            VisitInternal(node.ExprList);
        }

        public virtual void Visit(LikeExpression node)
        {
            VisitInternal(node.First);
            VisitInternal(node.Second);
            VisitInternal(node.Third);
        }

        public virtual void Visit(CollateExpression node)
        {
            VisitInternal(node.StringValue);
        }

        public virtual void Visit(UserExpression node)
        {
        }

        public virtual void Visit(UnaryOperatorExpression node)
        {
            VisitInternal(node.Operand);
        }

        public virtual void Visit(BinaryOperatorExpression node)
        {
            VisitInternal(node.LeftOprand);
            VisitInternal(node.RightOprand);
        }

        public virtual void Visit(PolyadicOperatorExpression node)
        {
            for (int i = 0, len = node.Arity; i < len; ++i)
            {
                VisitInternal(node.GetOperand(i));
            }
        }

        public virtual void Visit(LogicalAndExpression node)
        {
            Visit((PolyadicOperatorExpression) node);
        }

        public virtual void Visit(LogicalOrExpression node)
        {
            Visit((PolyadicOperatorExpression) node);
        }

        public virtual void Visit(ComparisionEqualsExpression node)
        {
            Visit((BinaryOperatorExpression) node);
        }

        public virtual void Visit(ComparisionNullSafeEqualsExpression node)
        {
            Visit((BinaryOperatorExpression) node);
        }

        public virtual void Visit(InExpression node)
        {
            Visit((BinaryOperatorExpression) node);
        }

        public virtual void Visit(FunctionExpression node)
        {
            VisitInternal(node.Arguments);
        }

        public virtual void Visit(Char node)
        {
            Visit((FunctionExpression) node);
        }

        public virtual void Visit(Convert node)
        {
            Visit((FunctionExpression) node);
        }

        public virtual void Visit(Trim node)
        {
            Visit((FunctionExpression) node);
            VisitInternal(node.RemainString);
            VisitInternal(node.StringValue);
        }

        public virtual void Visit(Cast node)
        {
            Visit((FunctionExpression) node);
            VisitInternal(node.Expr);
            VisitInternal(node.TypeInfo1);
            VisitInternal(node.TypeInfo2);
        }

        public virtual void Visit(Avg node)
        {
            Visit((FunctionExpression) node);
        }

        public virtual void Visit(Max node)
        {
            Visit((FunctionExpression) node);
        }

        public virtual void Visit(Min node)
        {
            Visit((FunctionExpression) node);
        }

        public virtual void Visit(Sum node)
        {
            Visit((FunctionExpression) node);
        }

        public virtual void Visit(Count node)
        {
            Visit((FunctionExpression) node);
        }

        public virtual void Visit(GroupConcat node)
        {
            Visit((FunctionExpression) node);
            VisitInternal(node.AppendedColumnNames);
            VisitInternal(node.OrderBy);
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
            VisitInternal(node.Quantity);
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
            VisitInternal(node.Comparee);
            VisitInternal(node.ElseResult);
            VisitInternal(node.WhenList);
        }

        public virtual void Visit(DefaultValue node)
        {
        }

        public virtual void Visit(ExistsPrimary node)
        {
            VisitInternal(node.Subquery);
        }

        public virtual void Visit(PlaceHolder node)
        {
        }

        public virtual void Visit(Identifier node)
        {
        }

        public virtual void Visit(MatchExpression node)
        {
            VisitInternal(node.Columns);
            VisitInternal(node.Pattern);
        }

        public virtual void Visit(ParamMarker node)
        {
        }

        public virtual void Visit(RowExpression node)
        {
            VisitInternal(node.RowExprList);
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
            VisitInternal(node.LeftTableRef);
            VisitInternal(node.OnCond);
            VisitInternal(node.RightTableRef);
        }

        public virtual void Visit(NaturalJoin node)
        {
            VisitInternal(node.LeftTableRef);
            VisitInternal(node.RightTableRef);
        }

        public virtual void Visit(OuterJoin node)
        {
            VisitInternal(node.LeftTableRef);
            VisitInternal(node.OnCond);
            VisitInternal(node.RightTableRef);
        }

        public virtual void Visit(StraightJoin node)
        {
            VisitInternal(node.LeftTableRef);
            VisitInternal(node.OnCond);
            VisitInternal(node.RightTableRef);
        }

        public virtual void Visit(SubqueryFactor node)
        {
            VisitInternal(node.Subquery);
        }

        public virtual void Visit(TableReferences node)
        {
            VisitInternal(node.TableReferenceList);
        }

        public virtual void Visit(TableRefFactor node)
        {
            VisitInternal(node.HintList);
            VisitInternal(node.Table);
        }

        public virtual void Visit(Dual dual)
        {
        }

        public virtual void Visit(GroupBy node)
        {
            VisitInternal(node.OrderByList);
        }

        public virtual void Visit(Limit node)
        {
            VisitInternal(node.Offset);
            VisitInternal(node.Size);
        }

        public virtual void Visit(OrderBy node)
        {
            VisitInternal(node.OrderByList);
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

        public virtual void Visit(DdlAlterTableStatement.AlterSpecification node)
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
            VisitInternal(node.Limit);
            VisitInternal(node.Pos);
        }

        public virtual void Visit(ShowCharaterSet node)
        {
            VisitInternal(node.Where);
        }

        public virtual void Visit(ShowCollation node)
        {
            VisitInternal(node.Where);
        }

        public virtual void Visit(ShowColumns node)
        {
            VisitInternal(node.Table);
            VisitInternal(node.Where);
        }

        public virtual void Visit(ShowContributors node)
        {
        }

        public virtual void Visit(ShowCreate node)
        {
            VisitInternal(node.Id);
        }

        public virtual void Visit(ShowDatabases node)
        {
            VisitInternal(node.Where);
        }

        public virtual void Visit(ShowEngine node)
        {
        }

        public virtual void Visit(ShowEngines node)
        {
        }

        public virtual void Visit(ShowErrors node)
        {
            VisitInternal(node.Limit);
        }

        public virtual void Visit(ShowEvents node)
        {
            VisitInternal(node.Schema);
            VisitInternal(node.Where);
        }

        public virtual void Visit(ShowFunctionCode node)
        {
            VisitInternal(node.FunctionName);
        }

        public virtual void Visit(ShowFunctionStatus node)
        {
            VisitInternal(node.Where);
        }

        public virtual void Visit(ShowGrants node)
        {
            VisitInternal(node.User);
        }

        public virtual void Visit(ShowIndex node)
        {
            VisitInternal(node.Table);
        }

        public virtual void Visit(ShowMasterStatus node)
        {
        }

        public virtual void Visit(ShowOpenTables node)
        {
            VisitInternal(node.Schema);
            VisitInternal(node.Where);
        }

        public virtual void Visit(ShowPlugins node)
        {
        }

        public virtual void Visit(ShowPrivileges node)
        {
        }

        public virtual void Visit(ShowProcedureCode node)
        {
            VisitInternal(node.ProcedureName);
        }

        public virtual void Visit(ShowProcedureStatus node)
        {
            VisitInternal(node.Where);
        }

        public virtual void Visit(ShowProcesslist node)
        {
        }

        public virtual void Visit(ShowProfile node)
        {
            VisitInternal(node.ForQuery);
            VisitInternal(node.Limit);
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
            VisitInternal(node.Where);
        }

        public virtual void Visit(ShowTables node)
        {
            VisitInternal(node.Schema);
            VisitInternal(node.Where);
        }

        public virtual void Visit(ShowTableStatus node)
        {
            VisitInternal(node.Database);
            VisitInternal(node.Where);
        }

        public virtual void Visit(ShowTriggers node)
        {
            VisitInternal(node.Schema);
            VisitInternal(node.Where);
        }

        public virtual void Visit(ShowVariables node)
        {
            VisitInternal(node.Where);
        }

        public virtual void Visit(ShowWarnings node)
        {
            VisitInternal(node.Limit);
        }

        public virtual void Visit(DescTableStatement node)
        {
            VisitInternal(node.Table);
        }

        public virtual void Visit(DalSetStatement node)
        {
            VisitInternal(node.AssignmentList);
        }

        public virtual void Visit(DalSetNamesStatement node)
        {
        }

        public virtual void Visit(DalSetCharacterSetStatement node)
        {
        }

        public virtual void Visit(DmlCallStatement node)
        {
            VisitInternal(node.GetArguments());
            VisitInternal(node.GetProcedure());
        }

        public virtual void Visit(DmlDeleteStatement node)
        {
            VisitInternal(node.Limit);
            VisitInternal(node.OrderBy);
            VisitInternal(node.TableNames);
            VisitInternal(node.TableRefs);
            VisitInternal(node.WhereCondition);
        }

        public virtual void Visit(DmlInsertStatement node)
        {
            VisitInternal(node.ColumnNameList);
            VisitInternal(node.DuplicateUpdate);
            VisitInternal(node.RowList);
            VisitInternal(node.Select);
            VisitInternal(node.Table);
        }

        public virtual void Visit(DmlReplaceStatement node)
        {
            VisitInternal(node.ColumnNameList);
            VisitInternal(node.RowList);
            VisitInternal(node.Select);
            VisitInternal(node.Table);
        }

        public virtual void Visit(DmlSelectStatement node)
        {
            VisitInternal(node.Group);
            VisitInternal(node.Having);
            VisitInternal(node.Limit);
            VisitInternal(node.Order);
            VisitInternal(node.SelectExprList);
            VisitInternal(node.Tables);
            VisitInternal(node.Where);
        }

        public virtual void Visit(DmlSelectUnionStatement node)
        {
            VisitInternal(node.Limit);
            VisitInternal(node.OrderBy);
            VisitInternal(node.SelectStmtList);
        }

        public virtual void Visit(DmlUpdateStatement node)
        {
            VisitInternal(node.Limit);
            VisitInternal(node.OrderBy);
            VisitInternal(node.TableRefs);
            VisitInternal(node.Values);
            VisitInternal(node.Where);
        }

        public virtual void Visit(MTSSetTransactionStatement node)
        {
        }

        public virtual void Visit(MTSSavepointStatement node)
        {
            VisitInternal(node.Savepoint);
        }

        public virtual void Visit(MTSReleaseStatement node)
        {
            VisitInternal(node.Savepoint);
        }

        public virtual void Visit(MTSRollbackStatement node)
        {
            VisitInternal(node.Savepoint);
        }

        public virtual void Visit(DdlTruncateStatement node)
        {
            VisitInternal(node.Table);
        }

        public virtual void Visit(DdlAlterTableStatement node)
        {
            VisitInternal(node.Table);
        }

        public virtual void Visit(DdlCreateIndexStatement node)
        {
            VisitInternal(node.IndexName);
            VisitInternal(node.Table);
        }

        public virtual void Visit(DdlCreateTableStatement node)
        {
            VisitInternal(node.Table);
        }

        public virtual void Visit(DdlRenameTableStatement node)
        {
            VisitInternal(node.PairList);
        }

        public virtual void Visit(DdlDropIndexStatement node)
        {
            VisitInternal(node.IndexName);
            VisitInternal(node.Table);
        }

        public virtual void Visit(DdlDropTableStatement node)
        {
            VisitInternal(node.TableNames);
        }

        public virtual void Visit(ExtDdlCreatePolicy node)
        {
        }

        public virtual void Visit(ExtDdlDropPolicy node)
        {
        }

        private void VisitInternal(object obj)
        {
            if (obj == null)
                return;

            if (obj is IAstNode)
            {
                ((IAstNode) obj).Accept(this);
            }
            else if (obj is ICollection)
            {
                foreach (var o in (ICollection) obj)
                {
                    VisitInternal(o);
                }
            }
            else
            {
                //INFO EmptySqlAstVisitor.VisitInternal
                var t = obj.GetType();
                if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof (Pair<,>))
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
}