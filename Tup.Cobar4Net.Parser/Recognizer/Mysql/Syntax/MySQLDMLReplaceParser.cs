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
using Tup.Cobar4Net.Parser.Ast.Expression.Misc;
using Tup.Cobar4Net.Parser.Ast.Expression.Primary;
using Tup.Cobar4Net.Parser.Ast.Stmt.Dml;
using Tup.Cobar4Net.Parser.Recognizer.Mysql.Lexer;

namespace Tup.Cobar4Net.Parser.Recognizer.Mysql.Syntax
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class MySQLDMLReplaceParser : MySQLDMLInsertReplaceParser
    {
        public MySQLDMLReplaceParser(MySQLLexer lexer, MySQLExprParser exprParser)
            : base(lexer, exprParser)
        {
        }

        /// <summary>
        /// nothing has been pre-consumed <code><pre>
        /// 'REPLACE' ('LOW_PRIORITY' | 'DELAYED')? ('INTO')? tableName
        /// (  'SET' colName ('='|':=') (expr|'DEFAULT') (',' colName ('='|':=') (expr|'DEFAULT'))
        /// | '(' (  colName (','colName)* ')' (  '(' 'SELECT' ...
        /// </summary>
        /// <remarks>
        /// nothing has been pre-consumed <code><pre>
        /// 'REPLACE' ('LOW_PRIORITY' | 'DELAYED')? ('INTO')? tableName
        /// (  'SET' colName ('='|':=') (expr|'DEFAULT') (',' colName ('='|':=') (expr|'DEFAULT'))
        /// | '(' (  colName (','colName)* ')' (  '(' 'SELECT' ... ')'
        /// | 'SELECT' ...
        /// |('VALUES'|'VALUE') value ( ',' value )
        /// )
        /// | 'SELECT' ... ')'
        /// )
        /// | 'SELECT' ...
        /// |('VALUES'|'VALUE') value ( ',' value )
        /// )
        /// value := '(' (expr|'DEFAULT') ( ',' (expr|'DEFAULT'))* ')'
        /// </pre></code>
        /// </remarks>
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        public virtual DMLReplaceStatement Replace()
        {
            Match(MySQLToken.KwReplace);
            DMLReplaceStatement.ReplaceMode mode = DMLReplaceStatement.ReplaceMode.Undef;
            switch (lexer.Token())
            {
                case MySQLToken.KwLowPriority:
                    {
                        lexer.NextToken();
                        mode = DMLReplaceStatement.ReplaceMode.Low;
                        break;
                    }

                case MySQLToken.KwDelayed:
                    {
                        lexer.NextToken();
                        mode = DMLReplaceStatement.ReplaceMode.Delay;
                        break;
                    }
            }
            if (lexer.Token() == MySQLToken.KwInto)
            {
                lexer.NextToken();
            }
            Identifier table = Identifier();
            IList<Identifier> columnNameList;
            IList<RowExpression> rowList;
            QueryExpression select;
            IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression> tempRowValue;
            switch (lexer.Token())
            {
                case MySQLToken.KwSet:
                    {
                        lexer.NextToken();
                        columnNameList = new List<Identifier>();
                        tempRowValue = new List<Tup.Cobar4Net.Parser.Ast.Expression.Expression>();
                        for (; ; lexer.NextToken())
                        {
                            Identifier id = Identifier();
                            Match(MySQLToken.OpEquals, MySQLToken.OpAssign);
                            Tup.Cobar4Net.Parser.Ast.Expression.Expression expr = exprParser.Expression();
                            columnNameList.Add(id);
                            tempRowValue.Add(expr);
                            if (lexer.Token() != MySQLToken.PuncComma)
                            {
                                break;
                            }
                        }
                        rowList = new List<RowExpression>(1);
                        rowList.Add(new RowExpression(tempRowValue));
                        return new DMLReplaceStatement(mode, table, columnNameList, rowList);
                    }

                case MySQLToken.Identifier:
                    {
                        if (!"VALUE".Equals(lexer.StringValueUppercase()))
                        {
                            break;
                        }
                        goto case MySQLToken.KwValues;
                    }

                case MySQLToken.KwValues:
                    {
                        lexer.NextToken();
                        columnNameList = null;
                        rowList = RowList();
                        return new DMLReplaceStatement(mode, table, columnNameList, rowList);
                    }

                case MySQLToken.KwSelect:
                    {
                        columnNameList = null;
                        select = Select();
                        return new DMLReplaceStatement(mode, table, columnNameList, select);
                    }

                case MySQLToken.PuncLeftParen:
                    {
                        switch (lexer.NextToken())
                        {
                            case MySQLToken.PuncLeftParen:
                            case MySQLToken.KwSelect:
                                {
                                    columnNameList = null;
                                    select = SelectPrimary();
                                    Match(MySQLToken.PuncRightParen);
                                    return new DMLReplaceStatement(mode, table, columnNameList, select);
                                }
                        }
                        columnNameList = IdList();
                        Match(MySQLToken.PuncRightParen);
                        switch (lexer.Token())
                        {
                            case MySQLToken.PuncLeftParen:
                            case MySQLToken.KwSelect:
                                {
                                    select = SelectPrimary();
                                    return new DMLReplaceStatement(mode, table, columnNameList, select);
                                }

                            case MySQLToken.KwValues:
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
                        return new DMLReplaceStatement(mode, table, columnNameList, rowList);
                    }
            }
            throw Err("unexpected token for replace: " + lexer.Token());
        }
    }
}