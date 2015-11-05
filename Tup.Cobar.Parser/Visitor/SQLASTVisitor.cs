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

//import com.alibaba.cobar.parser.ast.expression.BinaryOperatorExpression;
//import com.alibaba.cobar.parser.ast.expression.PolyadicOperatorExpression;
//import com.alibaba.cobar.parser.ast.expression.UnaryOperatorExpression;
//import com.alibaba.cobar.parser.ast.expression.comparison.BetweenAndExpression;
//import com.alibaba.cobar.parser.ast.expression.comparison.ComparisionEqualsExpression;
//import com.alibaba.cobar.parser.ast.expression.comparison.ComparisionIsExpression;
//import com.alibaba.cobar.parser.ast.expression.comparison.ComparisionNullSafeEqualsExpression;
//import com.alibaba.cobar.parser.ast.expression.comparison.InExpression;
//import com.alibaba.cobar.parser.ast.expression.logical.LogicalAndExpression;
//import com.alibaba.cobar.parser.ast.expression.logical.LogicalOrExpression;
//import com.alibaba.cobar.parser.ast.expression.misc.InExpressionList;
//import com.alibaba.cobar.parser.ast.expression.misc.UserExpression;
//import com.alibaba.cobar.parser.ast.expression.primary.CaseWhenOperatorExpression;
//import com.alibaba.cobar.parser.ast.expression.primary.DefaultValue;
//import com.alibaba.cobar.parser.ast.expression.primary.ExistsPrimary;
//import com.alibaba.cobar.parser.ast.expression.primary.Identifier;
//import com.alibaba.cobar.parser.ast.expression.primary.MatchExpression;
//import com.alibaba.cobar.parser.ast.expression.primary.ParamMarker;
//import com.alibaba.cobar.parser.ast.expression.primary.PlaceHolder;
//import com.alibaba.cobar.parser.ast.expression.primary.RowExpression;
//import com.alibaba.cobar.parser.ast.expression.primary.SysVarPrimary;
//import com.alibaba.cobar.parser.ast.expression.primary.UsrDefVarPrimary;
//import com.alibaba.cobar.parser.ast.expression.primary.function.FunctionExpression;
//import com.alibaba.cobar.parser.ast.expression.primary.function.cast.Cast;
//import com.alibaba.cobar.parser.ast.expression.primary.function.cast.Convert;
//import com.alibaba.cobar.parser.ast.expression.primary.function.datetime.Extract;
//import com.alibaba.cobar.parser.ast.expression.primary.function.datetime.GetFormat;
//import com.alibaba.cobar.parser.ast.expression.primary.function.datetime.Timestampadd;
//import com.alibaba.cobar.parser.ast.expression.primary.function.datetime.Timestampdiff;
//import com.alibaba.cobar.parser.ast.expression.primary.function.groupby.Avg;
//import com.alibaba.cobar.parser.ast.expression.primary.function.groupby.Count;
//import com.alibaba.cobar.parser.ast.expression.primary.function.groupby.GroupConcat;
//import com.alibaba.cobar.parser.ast.expression.primary.function.groupby.Max;
//import com.alibaba.cobar.parser.ast.expression.primary.function.groupby.Min;
//import com.alibaba.cobar.parser.ast.expression.primary.function.groupby.Sum;
//import com.alibaba.cobar.parser.ast.expression.primary.function.string.Char;
//import com.alibaba.cobar.parser.ast.expression.primary.function.string.Trim;
//import com.alibaba.cobar.parser.ast.expression.primary.literal.IntervalPrimary;
//import com.alibaba.cobar.parser.ast.expression.primary.literal.LiteralBitField;
//import com.alibaba.cobar.parser.ast.expression.primary.literal.LiteralBoolean;
//import com.alibaba.cobar.parser.ast.expression.primary.literal.LiteralHexadecimal;
//import com.alibaba.cobar.parser.ast.expression.primary.literal.LiteralNull;
//import com.alibaba.cobar.parser.ast.expression.primary.literal.LiteralNumber;
//import com.alibaba.cobar.parser.ast.expression.primary.literal.LiteralString;
//import com.alibaba.cobar.parser.ast.expression.string.LikeExpression;
//import com.alibaba.cobar.parser.ast.expression.type.CollateExpression;
//import com.alibaba.cobar.parser.ast.fragment.GroupBy;
//import com.alibaba.cobar.parser.ast.fragment.Limit;
//import com.alibaba.cobar.parser.ast.fragment.OrderBy;
//import com.alibaba.cobar.parser.ast.fragment.ddl.ColumnDefinition;
//import com.alibaba.cobar.parser.ast.fragment.ddl.TableOptions;
//import com.alibaba.cobar.parser.ast.fragment.ddl.datatype.DataType;
//import com.alibaba.cobar.parser.ast.fragment.ddl.index.IndexColumnName;
//import com.alibaba.cobar.parser.ast.fragment.ddl.index.IndexOption;
//import com.alibaba.cobar.parser.ast.fragment.tableref.Dual;
//import com.alibaba.cobar.parser.ast.fragment.tableref.IndexHint;
//import com.alibaba.cobar.parser.ast.fragment.tableref.InnerJoin;
//import com.alibaba.cobar.parser.ast.fragment.tableref.NaturalJoin;
//import com.alibaba.cobar.parser.ast.fragment.tableref.OuterJoin;
//import com.alibaba.cobar.parser.ast.fragment.tableref.StraightJoin;
//import com.alibaba.cobar.parser.ast.fragment.tableref.SubqueryFactor;
//import com.alibaba.cobar.parser.ast.fragment.tableref.TableRefFactor;
//import com.alibaba.cobar.parser.ast.fragment.tableref.TableReferences;
//import com.alibaba.cobar.parser.ast.stmt.dal.DALSetCharacterSetStatement;
//import com.alibaba.cobar.parser.ast.stmt.dal.DALSetNamesStatement;
//import com.alibaba.cobar.parser.ast.stmt.dal.DALSetStatement;
//import com.alibaba.cobar.parser.ast.stmt.dal.ShowAuthors;
//import com.alibaba.cobar.parser.ast.stmt.dal.ShowBinLogEvent;
//import com.alibaba.cobar.parser.ast.stmt.dal.ShowBinaryLog;
//import com.alibaba.cobar.parser.ast.stmt.dal.ShowCharaterSet;
//import com.alibaba.cobar.parser.ast.stmt.dal.ShowCollation;
//import com.alibaba.cobar.parser.ast.stmt.dal.ShowColumns;
//import com.alibaba.cobar.parser.ast.stmt.dal.ShowContributors;
//import com.alibaba.cobar.parser.ast.stmt.dal.ShowCreate;
//import com.alibaba.cobar.parser.ast.stmt.dal.ShowDatabases;
//import com.alibaba.cobar.parser.ast.stmt.dal.ShowEngine;
//import com.alibaba.cobar.parser.ast.stmt.dal.ShowEngines;
//import com.alibaba.cobar.parser.ast.stmt.dal.ShowErrors;
//import com.alibaba.cobar.parser.ast.stmt.dal.ShowEvents;
//import com.alibaba.cobar.parser.ast.stmt.dal.ShowFunctionCode;
//import com.alibaba.cobar.parser.ast.stmt.dal.ShowFunctionStatus;
//import com.alibaba.cobar.parser.ast.stmt.dal.ShowGrants;
//import com.alibaba.cobar.parser.ast.stmt.dal.ShowIndex;
//import com.alibaba.cobar.parser.ast.stmt.dal.ShowMasterStatus;
//import com.alibaba.cobar.parser.ast.stmt.dal.ShowOpenTables;
//import com.alibaba.cobar.parser.ast.stmt.dal.ShowPlugins;
//import com.alibaba.cobar.parser.ast.stmt.dal.ShowPrivileges;
//import com.alibaba.cobar.parser.ast.stmt.dal.ShowProcedureCode;
//import com.alibaba.cobar.parser.ast.stmt.dal.ShowProcedureStatus;
//import com.alibaba.cobar.parser.ast.stmt.dal.ShowProcesslist;
//import com.alibaba.cobar.parser.ast.stmt.dal.ShowProfile;
//import com.alibaba.cobar.parser.ast.stmt.dal.ShowProfiles;
//import com.alibaba.cobar.parser.ast.stmt.dal.ShowSlaveHosts;
//import com.alibaba.cobar.parser.ast.stmt.dal.ShowSlaveStatus;
//import com.alibaba.cobar.parser.ast.stmt.dal.ShowStatus;
//import com.alibaba.cobar.parser.ast.stmt.dal.ShowTableStatus;
//import com.alibaba.cobar.parser.ast.stmt.dal.ShowTables;
//import com.alibaba.cobar.parser.ast.stmt.dal.ShowTriggers;
//import com.alibaba.cobar.parser.ast.stmt.dal.ShowVariables;
//import com.alibaba.cobar.parser.ast.stmt.dal.ShowWarnings;
//import com.alibaba.cobar.parser.ast.stmt.ddl.DDLAlterTableStatement;
//import com.alibaba.cobar.parser.ast.stmt.ddl.DDLAlterTableStatement.AlterSpecification;
//import com.alibaba.cobar.parser.ast.stmt.ddl.DDLCreateIndexStatement;
//import com.alibaba.cobar.parser.ast.stmt.ddl.DDLCreateTableStatement;
//import com.alibaba.cobar.parser.ast.stmt.ddl.DDLDropIndexStatement;
//import com.alibaba.cobar.parser.ast.stmt.ddl.DDLDropTableStatement;
//import com.alibaba.cobar.parser.ast.stmt.ddl.DDLRenameTableStatement;
//import com.alibaba.cobar.parser.ast.stmt.ddl.DDLTruncateStatement;
//import com.alibaba.cobar.parser.ast.stmt.ddl.DescTableStatement;
//import com.alibaba.cobar.parser.ast.stmt.dml.DMLCallStatement;
//import com.alibaba.cobar.parser.ast.stmt.dml.DMLDeleteStatement;
//import com.alibaba.cobar.parser.ast.stmt.dml.DMLInsertStatement;
//import com.alibaba.cobar.parser.ast.stmt.dml.DMLReplaceStatement;
//import com.alibaba.cobar.parser.ast.stmt.dml.DMLSelectStatement;
//import com.alibaba.cobar.parser.ast.stmt.dml.DMLSelectUnionStatement;
//import com.alibaba.cobar.parser.ast.stmt.dml.DMLUpdateStatement;
//import com.alibaba.cobar.parser.ast.stmt.extension.ExtDDLCreatePolicy;
//import com.alibaba.cobar.parser.ast.stmt.extension.ExtDDLDropPolicy;
//import com.alibaba.cobar.parser.ast.stmt.mts.MTSReleaseStatement;
//import com.alibaba.cobar.parser.ast.stmt.mts.MTSRollbackStatement;
//import com.alibaba.cobar.parser.ast.stmt.mts.MTSSavepointStatement;
//import com.alibaba.cobar.parser.ast.stmt.mts.MTSSetTransactionStatement;

using Tup.Cobar.Parser.Ast.Expression;
using Tup.Cobar.Parser.Ast.Expression.Logical;
using Tup.Cobar.Parser.Ast.Expression.Misc;
using Tup.Cobar.Parser.Ast.Expression.Primary.Function;
using Tup.Cobar.Parser.Ast.Expression.Primary.Literal;

/**
* (created at 2011-5-30)
*/

namespace Tup.Cobar.Parser.Visitor
{
    /**
     * @author <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
     */

    public interface SQLASTVisitor
    {
        //void Visit(BetweenAndExpression node);

        //void Visit(ComparisionIsExpression node);

        void Visit(InExpressionList node);

        //void Visit(LikeExpression node);

        //void Visit(CollateExpression node);

        void Visit(UserExpression node);

        void Visit(UnaryOperatorExpression node);

        void Visit(BinaryOperatorExpression node);

        //void Visit(PolyadicOperatorExpression node);

        void Visit(LogicalAndExpression node);

        void Visit(LogicalOrExpression node);

        //void Visit(ComparisionEqualsExpression node);

        //void Visit(ComparisionNullSafeEqualsExpression node);

        //void Visit(InExpression node);

        // -------------------------------------------------------
        void Visit(FunctionExpression node);

        //void Visit(Char node);

        //void Visit(Convert node);

        //void Visit(Trim node);

        //void Visit(Cast node);

        //void Visit(Avg node);

        //void Visit(Max node);

        //void Visit(Min node);

        //void Visit(Sum node);

        //void Visit(Count node);

        //void Visit(GroupConcat node);

        //void Visit(Extract node);

        //void Visit(Timestampdiff node);

        //void Visit(Timestampadd node);

        //void Visit(GetFormat node);

        //// -------------------------------------------------------
        void Visit(IntervalPrimary node);

        //void Visit(LiteralBitField node);

        void Visit(LiteralBoolean node);

        //void Visit(LiteralHexadecimal node);

        //void Visit(LiteralNull node);

        //void Visit(LiteralNumber node);

        //void Visit(LiteralString node);

        //void Visit(CaseWhenOperatorExpression node);

        //void Visit(DefaultValue node);

        //void Visit(ExistsPrimary node);

        //void Visit(PlaceHolder node);

        //void Visit(Identifier node);

        //void Visit(MatchExpression node);

        //void Visit(ParamMarker node);

        //void Visit(RowExpression node);

        //void Visit(SysVarPrimary node);

        //void Visit(UsrDefVarPrimary node);

        //// -------------------------------------------------------
        //void Visit(IndexHint node);

        //void Visit(InnerJoin node);

        //void Visit(NaturalJoin node);

        //void Visit(OuterJoin node);

        //void Visit(StraightJoin node);

        //void Visit(SubqueryFactor node);

        //void Visit(TableReferences node);

        //void Visit(TableRefFactor node);

        //void Visit(Dual dual);

        //void Visit(GroupBy node);

        //void Visit(Limit node);

        //void Visit(OrderBy node);

        //void Visit(ColumnDefinition node);

        //void Visit(IndexOption node);

        //void Visit(IndexColumnName node);

        //void Visit(TableOptions node);

        //void Visit(AlterSpecification node);

        //void Visit(DataType node);

        //// -------------------------------------------------------
        //void Visit(ShowAuthors node);

        //void Visit(ShowBinaryLog node);

        //void Visit(ShowBinLogEvent node);

        //void Visit(ShowCharaterSet node);

        //void Visit(ShowCollation node);

        //void Visit(ShowColumns node);

        //void Visit(ShowContributors node);

        //void Visit(ShowCreate node);

        //void Visit(ShowDatabases node);

        //void Visit(ShowEngine node);

        //void Visit(ShowEngines node);

        //void Visit(ShowErrors node);

        //void Visit(ShowEvents node);

        //void Visit(ShowFunctionCode node);

        //void Visit(ShowFunctionStatus node);

        //void Visit(ShowGrants node);

        //void Visit(ShowIndex node);

        //void Visit(ShowMasterStatus node);

        //void Visit(ShowOpenTables node);

        //void Visit(ShowPlugins node);

        //void Visit(ShowPrivileges node);

        //void Visit(ShowProcedureCode node);

        //void Visit(ShowProcedureStatus node);

        //void Visit(ShowProcesslist node);

        //void Visit(ShowProfile node);

        //void Visit(ShowProfiles node);

        //void Visit(ShowSlaveHosts node);

        //void Visit(ShowSlaveStatus node);

        //void Visit(ShowStatus node);

        //void Visit(ShowTables node);

        //void Visit(ShowTableStatus node);

        //void Visit(ShowTriggers node);

        //void Visit(ShowVariables node);

        //void Visit(ShowWarnings node);

        //void Visit(DescTableStatement node);

        //void Visit(DALSetStatement node);

        //void Visit(DALSetNamesStatement node);

        //void Visit(DALSetCharacterSetStatement node);

        //// -------------------------------------------------------
        //void Visit(DMLCallStatement node);

        //void Visit(DMLDeleteStatement node);

        //void Visit(DMLInsertStatement node);

        //void Visit(DMLReplaceStatement node);

        //void Visit(DMLSelectStatement node);

        //void Visit(DMLSelectUnionStatement node);

        //void Visit(DMLUpdateStatement node);

        //void Visit(MTSSetTransactionStatement node);

        //void Visit(MTSSavepointStatement node);

        //void Visit(MTSReleaseStatement node);

        //void Visit(MTSRollbackStatement node);

        //void Visit(DDLTruncateStatement node);

        //void Visit(DDLAlterTableStatement node);

        //void Visit(DDLCreateIndexStatement node);

        //void Visit(DDLCreateTableStatement node);

        //void Visit(DDLRenameTableStatement node);

        //void Visit(DDLDropIndexStatement node);

        //void Visit(DDLDropTableStatement node);

        //void Visit(ExtDDLCreatePolicy node);

        //void Visit(ExtDDLDropPolicy node);
    }
}