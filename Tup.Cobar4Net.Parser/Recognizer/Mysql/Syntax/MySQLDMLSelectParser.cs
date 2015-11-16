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

using System.Collections.Generic;
using Tup.Cobar4Net.Parser.Ast.Fragment;
using Tup.Cobar4Net.Parser.Ast.Fragment.Tableref;
using Tup.Cobar4Net.Parser.Ast.Stmt.Dml;
using Tup.Cobar4Net.Parser.Recognizer.Mysql.Lexer;
using Tup.Cobar4Net.Parser.Util;
using Expr = Tup.Cobar4Net.Parser.Ast.Expression.Expression;

namespace Tup.Cobar4Net.Parser.Recognizer.Mysql.Syntax
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class MySQLDMLSelectParser : MySQLDMLParser
    {
        public MySQLDMLSelectParser(MySQLLexer lexer, MySQLExprParser exprParser)
            : base(lexer, exprParser)
        {
            this.exprParser.SetSelectParser(this);
        }

        private enum SpecialIdentifier
        {
            None = 0,
            SqlBufferResult,
            SqlCache,
            SqlNoCache
        }

        private static readonly IDictionary<string, SpecialIdentifier> specialIdentifiers = new Dictionary<string, SpecialIdentifier>();

        static MySQLDMLSelectParser()
        {
            specialIdentifiers["SQL_BUFFER_RESULT"] = SpecialIdentifier.SqlBufferResult;
            specialIdentifiers["SQL_CACHE"] = SpecialIdentifier.SqlCache;
            specialIdentifiers["SQL_NO_CACHE"] = SpecialIdentifier.SqlNoCache;
        }

        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        private DMLSelectStatement.SelectOption SelectOption()
        {
            for (DMLSelectStatement.SelectOption option = new DMLSelectStatement.SelectOption
                (); ; lexer.NextToken())
            {
                switch (lexer.Token())
                {
                    case MySQLToken.KwAll:
                        {
                            option.resultDup = DMLSelectStatement.SelectDuplicationStrategy.All;
                            goto outer_break;
                        }

                    case MySQLToken.KwDistinct:
                        {
                            option.resultDup = DMLSelectStatement.SelectDuplicationStrategy.Distinct;
                            goto outer_break;
                        }

                    case MySQLToken.KwDistinctrow:
                        {
                            option.resultDup = DMLSelectStatement.SelectDuplicationStrategy.Distinctrow;
                            goto outer_break;
                        }

                    case MySQLToken.KwHighPriority:
                        {
                            option.highPriority = true;
                            goto outer_break;
                        }

                    case MySQLToken.KwStraightJoin:
                        {
                            option.straightJoin = true;
                            goto outer_break;
                        }

                    case MySQLToken.KwSqlSmallResult:
                        {
                            option.resultSize = DMLSelectStatement.SmallOrBigResult.SqlSmallResult;
                            goto outer_break;
                        }

                    case MySQLToken.KwSqlBigResult:
                        {
                            option.resultSize = DMLSelectStatement.SmallOrBigResult.SqlBigResult;
                            goto outer_break;
                        }

                    case MySQLToken.KwSqlCalcFoundRows:
                        {
                            option.sqlCalcFoundRows = true;
                            goto outer_break;
                        }

                    case MySQLToken.Identifier:
                        {
                            string optionStringUp = lexer.StringValueUppercase();
                            var specialId = specialIdentifiers.GetValue(optionStringUp);
                            if (specialId != SpecialIdentifier.None)
                            {
                                switch (specialId)
                                {
                                    case SpecialIdentifier.SqlBufferResult:
                                        {
                                            if (option.sqlBufferResult)
                                            {
                                                return option;
                                            }
                                            option.sqlBufferResult = true;
                                            goto outer_break;
                                        }

                                    case SpecialIdentifier.SqlCache:
                                        {
                                            if (option.queryCache != DMLSelectStatement.QueryCacheStrategy.Undef)
                                            {
                                                return option;
                                            }
                                            option.queryCache = DMLSelectStatement.QueryCacheStrategy.SqlCache;
                                            goto outer_break;
                                        }

                                    case SpecialIdentifier.SqlNoCache:
                                        {
                                            if (option.queryCache != DMLSelectStatement.QueryCacheStrategy.Undef)
                                            {
                                                return option;
                                            }
                                            option.queryCache = DMLSelectStatement.QueryCacheStrategy.SqlNoCache;
                                            goto outer_break;
                                        }
                                }
                            }
                            goto default;
                        }

                    default:
                        {
                            return option;
                        }
                }
            outer_break:;
            }
        }

        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        private IList<Pair<Expr, string>> SelectExprList()
        {
            Expr expr = exprParser.Expression();
            string alias = As();
            IList<Pair<Expr, string>> list;
            if (lexer.Token() == MySQLToken.PuncComma)
            {
                list = new List<Pair<Expr, string>>();
                list.Add(new Pair<Expr, string>(expr, alias));
            }
            else
            {
                list = new List<Pair<Expr, string>>(1);
                list.Add(new Pair<Expr, string>(expr, alias));
                return list;
            }
            for (; lexer.Token() == MySQLToken.PuncComma; list.Add(new Pair<Expr, string>(expr, alias)))
            {
                lexer.NextToken();
                expr = exprParser.Expression();
                alias = As();
            }
            return list;
        }

        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        public override DMLSelectStatement Select()
        {
            Match(MySQLToken.KwSelect);
            DMLSelectStatement.SelectOption option = SelectOption();
            IList<Pair<Expr, string>> exprList = SelectExprList();
            TableReferences tables = null;
            Expr where = null;
            GroupBy group = null;
            Expr having = null;
            OrderBy order = null;
            Limit limit = null;
            bool dual = false;
            if (lexer.Token() == MySQLToken.KwFrom)
            {
                if (lexer.NextToken() == MySQLToken.KwDual)
                {
                    lexer.NextToken();
                    dual = true;
                    IList<TableReference> trs = new List<TableReference>(1);
                    trs.Add(new Dual());
                    tables = new TableReferences(trs);
                }
                else
                {
                    tables = TableRefs();
                }
            }
            if (lexer.Token() == MySQLToken.KwWhere)
            {
                lexer.NextToken();
                where = exprParser.Expression();
            }
            if (!dual)
            {
                group = GroupBy();
                if (lexer.Token() == MySQLToken.KwHaving)
                {
                    lexer.NextToken();
                    having = exprParser.Expression();
                }
                order = OrderBy();
            }
            limit = Limit();
            if (!dual)
            {
                switch (lexer.Token())
                {
                    case MySQLToken.KwFor:
                        {
                            lexer.NextToken();
                            Match(MySQLToken.KwUpdate);
                            option.lockMode = DMLSelectStatement.LockMode.ForUpdate;
                            break;
                        }

                    case MySQLToken.KwLock:
                        {
                            lexer.NextToken();
                            Match(MySQLToken.KwIn);
                            MatchIdentifier("SHARE");
                            MatchIdentifier("MODE");
                            option.lockMode = DMLSelectStatement.LockMode.LockInShareMode;
                            break;
                        }
                }
            }
            return new DMLSelectStatement(option, exprList, tables, where, group, having, order, limit);
        }

        /// <summary>
        /// first token is either
        /// <see cref="Tup.Cobar4Net.Parser.Recognizer.Mysql.MySQLToken.KwSelect"/>
        /// or
        /// <see cref="Tup.Cobar4Net.Parser.Recognizer.Mysql.MySQLToken.PuncLeftParen"/>
        /// which has been scanned but not yet
        /// consumed
        /// </summary>
        /// <returns>
        ///
        /// <see cref="Tup.Cobar4Net.Parser.Ast.Stmt.Dml.DMLSelectStatement"/>
        /// or
        /// <see cref="Tup.Cobar4Net.Parser.Ast.Stmt.Dml.DMLSelectUnionStatement"/>
        /// </returns>
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        public virtual DMLQueryStatement SelectUnion()
        {
            DMLSelectStatement select = SelectPrimary();
            DMLQueryStatement query = BuildUnionSelect(select);
            return query;
        }
    }
}