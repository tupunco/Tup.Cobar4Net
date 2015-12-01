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
using Tup.Cobar4Net.Parser.Ast.Expression;
using Tup.Cobar4Net.Parser.Ast.Fragment;
using Tup.Cobar4Net.Parser.Ast.Fragment.Tableref;
using Tup.Cobar4Net.Parser.Ast.Stmt.Dml;
using Tup.Cobar4Net.Parser.Recognizer.Mysql.Lexer;
using Tup.Cobar4Net.Parser.Util;

namespace Tup.Cobar4Net.Parser.Recognizer.Mysql.Syntax
{
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public class MySqlDmlSelectParser : MySqlDmlParser
    {
        private static readonly IDictionary<string, SpecialIdentifier> specialIdentifiers =
            new Dictionary<string, SpecialIdentifier>();

        static MySqlDmlSelectParser()
        {
            specialIdentifiers["SQL_BUFFER_RESULT"] = SpecialIdentifier.SqlBufferResult;
            specialIdentifiers["SQL_CACHE"] = SpecialIdentifier.SqlCache;
            specialIdentifiers["SQL_NO_CACHE"] = SpecialIdentifier.SqlNoCache;
        }

        public MySqlDmlSelectParser(MySqlLexer lexer, MySqlExprParser exprParser)
            : base(lexer, exprParser)
        {
            this.exprParser.SetSelectParser(this);
        }

        /// <exception cref="System.SqlSyntaxErrorException" />
        private DmlSelectStatement.SelectOption SelectOption()
        {
            for (var option = new DmlSelectStatement.SelectOption();;
                 lexer.NextToken())
            {
                switch (lexer.Token())
                {
                    case MySqlToken.KwAll:
                    {
                        option.resultDup = SelectDuplicationStrategy.All;
                        goto outer_break;
                    }

                    case MySqlToken.KwDistinct:
                    {
                        option.resultDup = SelectDuplicationStrategy.Distinct;
                        goto outer_break;
                    }

                    case MySqlToken.KwDistinctrow:
                    {
                        option.resultDup = SelectDuplicationStrategy.Distinctrow;
                        goto outer_break;
                    }

                    case MySqlToken.KwHighPriority:
                    {
                        option.highPriority = true;
                        goto outer_break;
                    }

                    case MySqlToken.KwStraightJoin:
                    {
                        option.straightJoin = true;
                        goto outer_break;
                    }

                    case MySqlToken.KwSqlSmallResult:
                    {
                        option.resultSize = SelectSmallOrBigResult.SqlSmallResult;
                        goto outer_break;
                    }

                    case MySqlToken.KwSqlBigResult:
                    {
                        option.resultSize = SelectSmallOrBigResult.SqlBigResult;
                        goto outer_break;
                    }

                    case MySqlToken.KwSqlCalcFoundRows:
                    {
                        option.sqlCalcFoundRows = true;
                        goto outer_break;
                    }

                    case MySqlToken.Identifier:
                    {
                        var optionStringUp = lexer.GetStringValueUppercase();
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
                                    if (option.SelectQueryCache != SelectQueryCacheStrategy.Undef)
                                    {
                                        return option;
                                    }
                                    option.SelectQueryCache = SelectQueryCacheStrategy.SqlCache;
                                    goto outer_break;
                                }

                                case SpecialIdentifier.SqlNoCache:
                                {
                                    if (option.SelectQueryCache != SelectQueryCacheStrategy.Undef)
                                    {
                                        return option;
                                    }
                                    option.SelectQueryCache = SelectQueryCacheStrategy.SqlNoCache;
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
                outer_break:
                ;
            }
        }

        /// <exception cref="System.SqlSyntaxErrorException" />
        private IList<Pair<IExpression, string>> SelectExprList()
        {
            var expr = exprParser.Expression();
            var alias = As();
            IList<Pair<IExpression, string>> list;
            if (lexer.Token() == MySqlToken.PuncComma)
            {
                list = new List<Pair<IExpression, string>>();
                list.Add(new Pair<IExpression, string>(expr, alias));
            }
            else
            {
                list = new List<Pair<IExpression, string>>(1);
                list.Add(new Pair<IExpression, string>(expr, alias));
                return list;
            }
            for (; lexer.Token() == MySqlToken.PuncComma; list.Add(new Pair<IExpression, string>(expr, alias)))
            {
                lexer.NextToken();
                expr = exprParser.Expression();
                alias = As();
            }
            return list;
        }

        /// <exception cref="System.SqlSyntaxErrorException" />
        public override DmlSelectStatement Select()
        {
            Match(MySqlToken.KwSelect);
            var option = SelectOption();
            var exprList = SelectExprList();
            TableReferences tables = null;
            IExpression where = null;
            GroupBy group = null;
            IExpression having = null;
            OrderBy order = null;
            Limit limit = null;
            var dual = false;
            if (lexer.Token() == MySqlToken.KwFrom)
            {
                if (lexer.NextToken() == MySqlToken.KwDual)
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
            if (lexer.Token() == MySqlToken.KwWhere)
            {
                lexer.NextToken();
                where = exprParser.Expression();
            }
            if (!dual)
            {
                group = GroupBy();
                if (lexer.Token() == MySqlToken.KwHaving)
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
                    case MySqlToken.KwFor:
                    {
                        lexer.NextToken();
                        Match(MySqlToken.KwUpdate);
                        option.lockMode = LockMode.ForUpdate;
                        break;
                    }

                    case MySqlToken.KwLock:
                    {
                        lexer.NextToken();
                        Match(MySqlToken.KwIn);
                        MatchIdentifier("SHARE");
                        MatchIdentifier("MODE");
                        option.lockMode = LockMode.LockInShareMode;
                        break;
                    }
                }
            }
            return new DmlSelectStatement(option, exprList, tables, where, group, having, order, limit);
        }

        /// <summary>
        ///     first token is either
        ///     <see cref="MySqlToken.KwSelect" />
        ///     or
        ///     <see cref="MySqlToken.PuncLeftParen" />
        ///     which has been scanned but not yet
        ///     consumed
        /// </summary>
        /// <returns>
        ///     <see cref="Tup.Cobar4Net.Parser.Ast.Stmt.Dml.DmlSelectStatement" />
        ///     or
        ///     <see cref="Tup.Cobar4Net.Parser.Ast.Stmt.Dml.DmlSelectUnionStatement" />
        /// </returns>
        /// <exception cref="System.SqlSyntaxErrorException" />
        public virtual DmlQueryStatement SelectUnion()
        {
            var select = SelectPrimary();
            var query = BuildUnionSelect(select);
            return query;
        }

        private enum SpecialIdentifier
        {
            None = 0,

            SqlBufferResult,
            SqlCache,
            SqlNoCache
        }
    }
}