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
using Tup.Cobar4Net.Parser.Util;
using Expr = Tup.Cobar4Net.Parser.Ast.Expression.Expression;

namespace Tup.Cobar4Net.Parser.Recognizer.Mysql.Syntax
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class MySQLDMLInsertParser : MySQLDMLInsertReplaceParser
    {
        public MySQLDMLInsertParser(MySQLLexer lexer, MySQLExprParser exprParser)
            : base(lexer, exprParser)
        {
        }

        /// <summary>
        /// nothing has been pre-consumed <code><pre>
        /// 'INSERT' ('LOW_PRIORITY'|'DELAYED'|'HIGH_PRIORITY')? 'IGNORE'? 'INTO'? tbname
        /// (  'SET' colName ('='|':=') (expr|'DEFAULT') (',' colName ('='|':=') (expr|'DEFAULT'))
        /// | '(' (  colName (',' colName)* ')' ( ('VALUES'|'VALUE') value (',' value)
        /// | '(' 'SELECT' ...
        /// </summary>
        /// <remarks>
        /// nothing has been pre-consumed <code><pre>
        /// 'INSERT' ('LOW_PRIORITY'|'DELAYED'|'HIGH_PRIORITY')? 'IGNORE'? 'INTO'? tbname
        /// (  'SET' colName ('='|':=') (expr|'DEFAULT') (',' colName ('='|':=') (expr|'DEFAULT'))
        /// | '(' (  colName (',' colName)* ')' ( ('VALUES'|'VALUE') value (',' value)
        /// | '(' 'SELECT' ... ')'
        /// | 'SELECT' ...
        /// )
        /// | 'SELECT' ... ')'
        /// )
        /// |('VALUES'|'VALUE') value  ( ',' value )
        /// | 'SELECT' ...
        /// )
        /// ( 'ON' 'DUPLICATE' 'KEY' 'UPDATE' colName ('='|':=') expr ( ',' colName ('='|':=') expr)* )?
        /// value := '(' (expr|'DEFAULT') ( ',' (expr|'DEFAULT'))* ')'
        /// </pre></code>
        /// </remarks>
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        public virtual DMLInsertStatement Insert()
        {
            Match(MySQLToken.KwInsert);
            DMLInsertStatement.InsertMode mode = DMLInsertStatement.InsertMode.Undef;
            bool ignore = false;
            switch (lexer.Token())
            {
                case MySQLToken.KwLowPriority:
                    {
                        lexer.NextToken();
                        mode = DMLInsertStatement.InsertMode.Low;
                        break;
                    }

                case MySQLToken.KwDelayed:
                    {
                        lexer.NextToken();
                        mode = DMLInsertStatement.InsertMode.Delay;
                        break;
                    }

                case MySQLToken.KwHighPriority:
                    {
                        lexer.NextToken();
                        mode = DMLInsertStatement.InsertMode.High;
                        break;
                    }
            }
            if (lexer.Token() == MySQLToken.KwIgnore)
            {
                ignore = true;
                lexer.NextToken();
            }
            if (lexer.Token() == MySQLToken.KwInto)
            {
                lexer.NextToken();
            }
            Identifier table = Identifier();
            IList<Pair<Identifier, Expr>> dupUpdate;
            IList<Identifier> columnNameList;
            IList<RowExpression> rowList;
            QueryExpression select;
            IList<Expr> tempRowValue;
            switch (lexer.Token())
            {
                case MySQLToken.KwSet:
                    {
                        lexer.NextToken();
                        columnNameList = new List<Identifier>();
                        tempRowValue = new List<Expr>();
                        for (; ; lexer.NextToken())
                        {
                            Identifier id = Identifier();
                            Match(MySQLToken.OpEquals, MySQLToken.OpAssign);
                            Expr expr = exprParser.Expression();
                            columnNameList.Add(id);
                            tempRowValue.Add(expr);
                            if (lexer.Token() != MySQLToken.PuncComma)
                            {
                                break;
                            }
                        }
                        rowList = new List<RowExpression>(1);
                        rowList.Add(new RowExpression(tempRowValue));
                        dupUpdate = OnDuplicateUpdate();
                        return new DMLInsertStatement(mode, ignore, table, columnNameList, rowList, dupUpdate);
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
                        dupUpdate = OnDuplicateUpdate();
                        return new DMLInsertStatement(mode, ignore, table, columnNameList, rowList, dupUpdate);
                    }

                case MySQLToken.KwSelect:
                    {
                        columnNameList = null;
                        select = Select();
                        dupUpdate = OnDuplicateUpdate();
                        return new DMLInsertStatement(mode, ignore, table, columnNameList, select, dupUpdate);
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
                                    dupUpdate = OnDuplicateUpdate();
                                    return new DMLInsertStatement(mode, ignore, table, columnNameList, select, dupUpdate);
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
                                    dupUpdate = OnDuplicateUpdate();
                                    return new DMLInsertStatement(mode, ignore, table, columnNameList, select, dupUpdate);
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
                        dupUpdate = OnDuplicateUpdate();
                        return new DMLInsertStatement(mode, ignore, table, columnNameList, rowList, dupUpdate);
                    }
            }
            throw Err("unexpected token for insert: " + lexer.Token());
        }

        /// <returns>null for not exist</returns>
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        private IList<Pair<Identifier, Expr>> OnDuplicateUpdate()
        {
            if (lexer.Token() != MySQLToken.KwOn)
            {
                return null;
            }
            lexer.NextToken();
            MatchIdentifier("DUPLICATE");
            Match(MySQLToken.KwKey);
            Match(MySQLToken.KwUpdate);
            IList<Pair<Identifier, Expr>> list;
            Identifier col = Identifier();
            Match(MySQLToken.OpEquals, MySQLToken.OpAssign);
            Expr expr = exprParser.Expression();
            if (lexer.Token() == MySQLToken.PuncComma)
            {
                list = new List<Pair<Identifier, Expr>>();
                list.Add(new Pair<Identifier, Expr>(col
                    , expr));
                for (; lexer.Token() == MySQLToken.PuncComma;)
                {
                    lexer.NextToken();
                    col = Identifier();
                    Match(MySQLToken.OpEquals, MySQLToken.OpAssign);
                    expr = exprParser.Expression();
                    list.Add(new Pair<Identifier, Expr>(col
                        , expr));
                }
                return list;
            }
            list = new List<Pair<Identifier, Expr>>(1);
            list.Add(new Pair<Identifier, Expr>(col, expr));
            return list;
        }
    }
}