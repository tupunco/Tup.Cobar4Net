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
using Tup.Cobar4Net.Parser.Ast.Expression.Misc;
using Tup.Cobar4Net.Parser.Ast.Expression.Primary;
using Tup.Cobar4Net.Parser.Ast.Stmt.Dml;
using Tup.Cobar4Net.Parser.Recognizer.Mysql.Lexer;

namespace Tup.Cobar4Net.Parser.Recognizer.Mysql.Syntax
{
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public class MySqlDmlReplaceParser : MySqlDmlInsertReplaceParser
    {
        public MySqlDmlReplaceParser(MySqlLexer lexer, MySqlExprParser exprParser)
            : base(lexer, exprParser)
        {
        }

        /// <summary>
        ///     nothing has been pre-consumed
        ///     <code><pre>
        ///         'REPLACE' ('LOW_PRIORITY' | 'DELAYED')? ('INTO')? tableName
        ///         (  'SET' colName ('='|':=') (expr|'DEFAULT') (',' colName ('='|':=') (expr|'DEFAULT'))
        ///         | '(' (  colName (','colName)* ')' (  '(' 'SELECT' ...
        /// </summary>
        /// <remarks>
        ///     nothing has been pre-consumed
        ///     <code><pre>
        ///         'REPLACE' ('LOW_PRIORITY' | 'DELAYED')? ('INTO')? tableName
        ///         (  'SET' colName ('='|':=') (expr|'DEFAULT') (',' colName ('='|':=') (expr|'DEFAULT'))
        ///         | '(' (  colName (','colName)* ')' (  '(' 'SELECT' ... ')'
        ///         | 'SELECT' ...
        ///         |('VALUES'|'VALUE') value ( ',' value )
        ///         )
        ///         | 'SELECT' ... ')'
        ///         )
        ///         | 'SELECT' ...
        ///         |('VALUES'|'VALUE') value ( ',' value )
        ///         )
        ///         value := '(' (expr|'DEFAULT') ( ',' (expr|'DEFAULT'))* ')'
        ///     </pre></code>
        /// </remarks>
        /// <exception cref="System.SqlSyntaxErrorException" />
        public virtual DmlReplaceStatement Replace()
        {
            Match(MySqlToken.KwReplace);
            var mode = ReplaceMode.Undef;
            switch (lexer.Token())
            {
                case MySqlToken.KwLowPriority:
                {
                    lexer.NextToken();
                    mode = ReplaceMode.Low;
                    break;
                }

                case MySqlToken.KwDelayed:
                {
                    lexer.NextToken();
                    mode = ReplaceMode.Delay;
                    break;
                }
            }
            if (lexer.Token() == MySqlToken.KwInto)
            {
                lexer.NextToken();
            }
            var table = Identifier();
            IList<Identifier> columnNameList;
            IList<RowExpression> rowList;
            IQueryExpression select;
            IList<IExpression> tempRowValue;
            switch (lexer.Token())
            {
                case MySqlToken.KwSet:
                {
                    lexer.NextToken();
                    columnNameList = new List<Identifier>();
                    tempRowValue = new List<IExpression>();
                    for (;; lexer.NextToken())
                    {
                        var id = Identifier();
                        Match(MySqlToken.OpEquals, MySqlToken.OpAssign);
                        var expr = exprParser.Expression();
                        columnNameList.Add(id);
                        tempRowValue.Add(expr);
                        if (lexer.Token() != MySqlToken.PuncComma)
                        {
                            break;
                        }
                    }
                    rowList = new List<RowExpression>(1);
                    rowList.Add(new RowExpression(tempRowValue));
                    return new DmlReplaceStatement(mode, table, columnNameList, rowList);
                }

                case MySqlToken.Identifier:
                {
                    if (!"VALUE".Equals(lexer.GetStringValueUppercase()))
                    {
                        break;
                    }
                    goto case MySqlToken.KwValues;
                }

                case MySqlToken.KwValues:
                {
                    lexer.NextToken();
                    columnNameList = null;
                    rowList = RowList();
                    return new DmlReplaceStatement(mode, table, columnNameList, rowList);
                }

                case MySqlToken.KwSelect:
                {
                    columnNameList = null;
                    select = Select();
                    return new DmlReplaceStatement(mode, table, columnNameList, select);
                }

                case MySqlToken.PuncLeftParen:
                {
                    switch (lexer.NextToken())
                    {
                        case MySqlToken.PuncLeftParen:
                        case MySqlToken.KwSelect:
                        {
                            columnNameList = null;
                            select = SelectPrimary();
                            Match(MySqlToken.PuncRightParen);
                            return new DmlReplaceStatement(mode, table, columnNameList, select);
                        }
                    }
                    columnNameList = IdList();
                    Match(MySqlToken.PuncRightParen);
                    switch (lexer.Token())
                    {
                        case MySqlToken.PuncLeftParen:
                        case MySqlToken.KwSelect:
                        {
                            select = SelectPrimary();
                            return new DmlReplaceStatement(mode, table, columnNameList, select);
                        }

                        case MySqlToken.KwValues:
                        {
                            lexer.NextToken();
                            break;
                        }

                        default:
                        {
                            MatchIdentifier("VALUE");
                            break;
                        }
                    }
                    rowList = RowList();
                    return new DmlReplaceStatement(mode, table, columnNameList, rowList);
                }
            }
            throw Err("unexpected token for replace: " + lexer.Token());
        }
    }
}