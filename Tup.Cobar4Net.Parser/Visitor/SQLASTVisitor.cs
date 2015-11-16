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

namespace Tup.Cobar4Net.Parser.Visitor
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public interface SQLASTVisitor
    {
        void Visit(BetweenAndExpression node);

        void Visit(InExpressionList node);

        void Visit(LikeExpression node);

        void Visit(CollateExpression node);

        void Visit(UserExpression node);

        void Visit(UnaryOperatorExpression node);

        void Visit(BinaryOperatorExpression node);

        void Visit(PolyadicOperatorExpression node);

        void Visit(LogicalAndExpression node);

        void Visit(LogicalOrExpression node);

        void Visit(ComparisionIsExpression node);

        void Visit(ComparisionEqualsExpression node);

        void Visit(ComparisionNullSafeEqualsExpression node);

        void Visit(InExpression node);

        // -------------------------------------------------------
        void Visit(FunctionExpression node);

        void Visit(Char node);

        void Visit(Convert node);

        void Visit(Trim node);

        void Visit(Cast node);

        void Visit(Avg node);

        void Visit(Max node);

        void Visit(Min node);

        void Visit(Sum node);

        void Visit(Count node);

        void Visit(GroupConcat node);

        void Visit(Extract node);

        void Visit(Timestampdiff node);

        void Visit(Timestampadd node);

        void Visit(GetFormat node);

        // -------------------------------------------------------
        void Visit(IntervalPrimary node);

        void Visit(LiteralBitField node);

        void Visit(LiteralBoolean node);

        void Visit(LiteralHexadecimal node);

        void Visit(LiteralNull node);

        void Visit(LiteralNumber node);

        void Visit(LiteralString node);

        void Visit(CaseWhenOperatorExpression node);

        void Visit(DefaultValue node);

        void Visit(ExistsPrimary node);

        void Visit(PlaceHolder node);

        void Visit(Identifier node);

        void Visit(MatchExpression node);

        void Visit(ParamMarker node);

        void Visit(RowExpression node);

        void Visit(SysVarPrimary node);

        void Visit(UsrDefVarPrimary node);

        // -------------------------------------------------------
        void Visit(IndexHint node);

        void Visit(InnerJoin node);

        void Visit(NaturalJoin node);

        void Visit(OuterJoin node);

        void Visit(StraightJoin node);

        void Visit(SubqueryFactor node);

        void Visit(TableReferences node);

        void Visit(TableRefFactor node);

        void Visit(Dual dual);

        void Visit(GroupBy node);

        void Visit(Limit node);

        void Visit(OrderBy node);

        void Visit(ColumnDefinition node);

        void Visit(IndexOption node);

        void Visit(IndexColumnName node);

        void Visit(TableOptions node);

        void Visit(DataType node);

        // -------------------------------------------------------
        void Visit(ShowAuthors node);

        void Visit(ShowBinaryLog node);

        void Visit(ShowBinLogEvent node);

        void Visit(ShowCharaterSet node);

        void Visit(ShowCollation node);

        void Visit(ShowColumns node);

        void Visit(ShowContributors node);

        void Visit(ShowCreate node);

        void Visit(ShowDatabases node);

        void Visit(ShowEngine node);

        void Visit(ShowEngines node);

        void Visit(ShowErrors node);

        void Visit(ShowEvents node);

        void Visit(ShowFunctionCode node);

        void Visit(ShowFunctionStatus node);

        void Visit(ShowGrants node);

        void Visit(ShowIndex node);

        void Visit(ShowMasterStatus node);

        void Visit(ShowOpenTables node);

        void Visit(ShowPlugins node);

        void Visit(ShowPrivileges node);

        void Visit(ShowProcedureCode node);

        void Visit(ShowProcedureStatus node);

        void Visit(ShowProcesslist node);

        void Visit(ShowProfile node);

        void Visit(ShowProfiles node);

        void Visit(ShowSlaveHosts node);

        void Visit(ShowSlaveStatus node);

        void Visit(ShowStatus node);

        void Visit(ShowTables node);

        void Visit(ShowTableStatus node);

        void Visit(ShowTriggers node);

        void Visit(ShowVariables node);

        void Visit(ShowWarnings node);

        void Visit(DALSetStatement node);

        void Visit(DALSetNamesStatement node);

        void Visit(DALSetCharacterSetStatement node);

        // -------------------------------------------------------
        void Visit(DMLCallStatement node);

        void Visit(DMLDeleteStatement node);

        void Visit(DMLInsertStatement node);

        void Visit(DMLReplaceStatement node);

        void Visit(DMLSelectStatement node);

        void Visit(DMLSelectUnionStatement node);

        void Visit(DMLUpdateStatement node);

        void Visit(MTSSetTransactionStatement node);

        void Visit(MTSSavepointStatement node);

        void Visit(MTSReleaseStatement node);

        void Visit(MTSRollbackStatement node);

        void Visit(DDLTruncateStatement node);

        void Visit(DDLAlterTableStatement node);

        void Visit(DDLCreateIndexStatement node);

        void Visit(DDLCreateTableStatement node);

        void Visit(DDLRenameTableStatement node);

        void Visit(DDLDropIndexStatement node);

        void Visit(DDLDropTableStatement node);

        void Visit(DDLAlterTableStatement.AlterSpecification node);

        void Visit(DescTableStatement node);

        void Visit(ExtDDLCreatePolicy node);

        void Visit(ExtDDLDropPolicy node);
    }
}