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
using Tup.Cobar4Net.Parser.Util;

namespace Tup.Cobar4Net.Parser.Recognizer.Mysql.Syntax
{
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public class MySqlDmlInsertParser : MySqlDmlInsertReplaceParser
    {
        public MySqlDmlInsertParser(MySqlLexer lexer, MySqlExprParser exprParser)
            : base(lexer, exprParser)
        {
        }

        /// <summary>
        ///     nothing has been pre-consumed
        ///     <code><pre>
        ///         'INSERT' ('LOW_PRIORITY'|'DELAYED'|'HIGH_PRIORITY')? 'IGNORE'? 'INTO'? tbname
        ///         (  'SET' colName ('='|':=') (expr|'DEFAULT') (',' colName ('='|':=') (expr|'DEFAULT'))
        ///         | '(' (  colName (',' colName)* ')' ( ('VALUES'|'VALUE') value (',' value)
        ///         | '(' 'SELECT' ...
        /// </summary>
        /// <remarks>
        ///     nothing has been pre-consumed
        ///     <code><pre>
        ///         'INSERT' ('LOW_PRIORITY'|'DELAYED'|'HIGH_PRIORITY')? 'IGNORE'? 'INTO'? tbname
        ///         (  'SET' colName ('='|':=') (expr|'DEFAULT') (',' colName ('='|':=') (expr|'DEFAULT'))
        ///         | '(' (  colName (',' colName)* ')' ( ('VALUES'|'VALUE') value (',' value)
        ///         | '(' 'SELECT' ... ')'
        ///         | 'SELECT' ...
        ///         )
        ///         | 'SELECT' ... ')'
        ///         )
        ///         |('VALUES'|'VALUE') value  ( ',' value )
        ///         | 'SELECT' ...
        ///         )
        ///         ( 'ON' 'DUPLICATE' 'KEY' 'UPDATE' colName ('='|':=') expr ( ',' colName ('='|':=') expr)* )?
        ///         value := '(' (expr|'DEFAULT') ( ',' (expr|'DEFAULT'))* ')'
        ///     </pre></code>
        /// </remarks>
        /// <exception cref="System.SqlSyntaxErrorException" />
        public virtual DmlInsertStatement Insert()
        {
            Match(MySqlToken.KwInsert);
            var mode = InsertMode.Undef;
            var ignore = false;
            switch (lexer.Token())
            {
                case MySqlToken.KwLowPriority:
                {
                    lexer.NextToken();
                    mode = InsertMode.Low;
                    break;
                }

                case MySqlToken.KwDelayed:
                {
                    lexer.NextToken();
                    mode = InsertMode.Delay;
                    break;
                }

                case MySqlToken.KwHighPriority:
                {
                    lexer.NextToken();
                    mode = InsertMode.High;
                    break;
                }
            }
            if (lexer.Token() == MySqlToken.KwIgnore)
            {
                ignore = true;
                lexer.NextToken();
            }
            if (lexer.Token() == MySqlToken.KwInto)
            {
                lexer.NextToken();
            }
            var table = Identifier();
            IList<Pair<Identifier, IExpression>> dupUpdate;
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
                    dupUpdate = OnDuplicateUpdate();
                    return new DmlInsertStatement(mode, ignore, table, columnNameList, rowList, dupUpdate);
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
                    dupUpdate = OnDuplicateUpdate();
                    return new DmlInsertStatement(mode, ignore, table, columnNameList, rowList, dupUpdate);
                }

                case MySqlToken.KwSelect:
                {
                    columnNameList = null;
                    select = Select();
                    dupUpdate = OnDuplicateUpdate();
                    return new DmlInsertStatement(mode, ignore, table, columnNameList, select, dupUpdate);
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
                            dupUpdate = OnDuplicateUpdate();
                            return new DmlInsertStatement(mode, ignore, table, columnNameList, select, dupUpdate);
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
                            dupUpdate = OnDuplicateUpdate();
                            return new DmlInsertStatement(mode, ignore, table, columnNameList, select, dupUpdate);
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
                    dupUpdate = OnDuplicateUpdate();
                    return new DmlInsertStatement(mode, ignore, table, columnNameList, rowList, dupUpdate);
                }
            }
            throw Err("unexpected token for insert: " + lexer.Token());
        }

        /// <returns>null for not exist</returns>
        /// <exception cref="System.SqlSyntaxErrorException" />
        private IList<Pair<Identifier, IExpression>> OnDuplicateUpdate()
        {
            if (lexer.Token() != MySqlToken.KwOn)
            {
                return null;
            }
            lexer.NextToken();
            MatchIdentifier("DUPLICATE");
            Match(MySqlToken.KwKey);
            Match(MySqlToken.KwUpdate);
            IList<Pair<Identifier, IExpression>> list;
            var col = Identifier();
            Match(MySqlToken.OpEquals, MySqlToken.OpAssign);
            var expr = exprParser.Expression();
            if (lexer.Token() == MySqlToken.PuncComma)
            {
                list = new List<Pair<Identifier, IExpression>>();
                list.Add(new Pair<Identifier, IExpression>(col, expr));
                for (; lexer.Token() == MySqlToken.PuncComma;)
                {
                    lexer.NextToken();
                    col = Identifier();
                    Match(MySqlToken.OpEquals, MySqlToken.OpAssign);
                    expr = exprParser.Expression();
                    list.Add(new Pair<Identifier, IExpression>(col, expr));
                }
                return list;
            }
            list = new List<Pair<Identifier, IExpression>>(1);
            list.Add(new Pair<Identifier, IExpression>(col, expr));
            return list;
        }
    }
}