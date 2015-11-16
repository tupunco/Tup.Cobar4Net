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

using System;
using System.Collections.Generic;
using System.Text;

using Tup.Cobar4Net.Parser.Ast.Stmt;
using Tup.Cobar4Net.Parser.Ast.Stmt.Ddl;
using Tup.Cobar4Net.Parser.Recognizer.Mysql;
using Tup.Cobar4Net.Parser.Recognizer.Mysql.Lexer;
using Tup.Cobar4Net.Parser.Recognizer.Mysql.Syntax;

namespace Tup.Cobar4Net.Parser.Recognizer
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public sealed class SQLParserDelegate
    {
        private enum SpecialIdentifier
        {
            None = 0,
            Rollback,
            Savepoint,
            Truncate
        }

        private static readonly IDictionary<string, SpecialIdentifier>
            specialIdentifiers = new Dictionary<string, SpecialIdentifier>();

        static SQLParserDelegate()
        {
            specialIdentifiers["TRUNCATE"] = SpecialIdentifier.Truncate;
            specialIdentifiers["SAVEPOINT"] = SpecialIdentifier.Savepoint;
            specialIdentifiers["ROLLBACK"] = SpecialIdentifier.Rollback;
        }

        private static bool IsEOFedDDL(SQLStatement stmt)
        {
            if (stmt is DDLStatement)
            {
                if (stmt is DDLCreateIndexStatement)
                {
                    return false;
                }
            }
            return true;
        }

        private static string BuildErrorMsg(Exception e, MySQLLexer lexer, string sql)
        {
            var sb = new StringBuilder("You have an error in your SQL syntax; Error occurs around this fragment: ");
            int ch = lexer.GetCurrentIndex();
            int from = ch - 16;
            if (from < 0)
            {
                from = 0;
            }
            int to = ch + 9;
            if (to >= sql.Length)
            {
                to = sql.Length - 1;
            }
            string fragment = Sharpen.Runtime.Substring(sql, from, to + 1);
            sb.Append('{').Append(fragment).Append('}').Append(". Error cause: " + e.Message);
            return sb.ToString();
        }

        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        public static SQLStatement Parse(string sql, MySQLLexer lexer, string charset)
        {
            try
            {
                SQLStatement stmt = null;
                bool isEOF = true;
                MySQLExprParser exprParser = new MySQLExprParser(lexer, charset);
                switch (lexer.Token())
                {
                    case MySQLToken.KwDesc:
                    case MySQLToken.KwDescribe:
                        {
                            stmt = new MySQLDALParser(lexer, exprParser).Desc();
                            goto stmtSwitch_break;
                        }

                    case MySQLToken.KwSelect:
                    case MySQLToken.PuncLeftParen:
                        {
                            stmt = new MySQLDMLSelectParser(lexer, exprParser).SelectUnion();
                            goto stmtSwitch_break;
                        }

                    case MySQLToken.KwDelete:
                        {
                            stmt = new MySQLDMLDeleteParser(lexer, exprParser).Delete();
                            goto stmtSwitch_break;
                        }

                    case MySQLToken.KwInsert:
                        {
                            stmt = new MySQLDMLInsertParser(lexer, exprParser).Insert();
                            goto stmtSwitch_break;
                        }

                    case MySQLToken.KwReplace:
                        {
                            stmt = new MySQLDMLReplaceParser(lexer, exprParser).Replace();
                            goto stmtSwitch_break;
                        }

                    case MySQLToken.KwUpdate:
                        {
                            stmt = new MySQLDMLUpdateParser(lexer, exprParser).Update();
                            goto stmtSwitch_break;
                        }

                    case MySQLToken.KwCall:
                        {
                            stmt = new MySQLDMLCallParser(lexer, exprParser).Call();
                            goto stmtSwitch_break;
                        }

                    case MySQLToken.KwSet:
                        {
                            stmt = new MySQLDALParser(lexer, exprParser).Set();
                            goto stmtSwitch_break;
                        }

                    case MySQLToken.KwShow:
                        {
                            stmt = new MySQLDALParser(lexer, exprParser).Show();
                            goto stmtSwitch_break;
                        }

                    case MySQLToken.KwAlter:
                    case MySQLToken.KwCreate:
                    case MySQLToken.KwDrop:
                    case MySQLToken.KwRename:
                        {
                            stmt = new MySQLDDLParser(lexer, exprParser).DdlStmt();
                            isEOF = IsEOFedDDL(stmt);
                            goto stmtSwitch_break;
                        }

                    case MySQLToken.KwRelease:
                        {
                            stmt = new MySQLMTSParser(lexer).Release();
                            goto stmtSwitch_break;
                        }

                    case MySQLToken.Identifier:
                        {
                            SpecialIdentifier si = SpecialIdentifier.None;
                            if ((si = specialIdentifiers[lexer.StringValueUppercase()]) != SpecialIdentifier.None)
                            {
                                switch (si)
                                {
                                    case SpecialIdentifier.Truncate:
                                        {
                                            stmt = new MySQLDDLParser(lexer, exprParser).Truncate();
                                            goto stmtSwitch_break;
                                        }

                                    case SpecialIdentifier.Savepoint:
                                        {
                                            stmt = new MySQLMTSParser(lexer).Savepoint();
                                            goto stmtSwitch_break;
                                        }

                                    case SpecialIdentifier.Rollback:
                                        {
                                            stmt = new MySQLMTSParser(lexer).Rollback();
                                            goto stmtSwitch_break;
                                        }
                                }
                            }
                            goto default;
                        }

                    default:
                        {
                            throw new SQLSyntaxErrorException("sql is not a supported statement");
                        }
                }
            stmtSwitch_break:;
                if (isEOF)
                {
                    while (lexer.Token() == MySQLToken.PuncSemicolon)
                    {
                        lexer.NextToken();
                    }
                    if (lexer.Token() != MySQLToken.Eof)
                    {
                        throw new SQLSyntaxErrorException("SQL syntax error!");
                    }
                }
                return stmt;
            }
            catch (Exception e)
            {
                throw new SQLSyntaxErrorException(BuildErrorMsg(e, lexer, sql), e);
            }
        }

        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        public static SQLStatement Parse(string sql, string charset)
        {
            return Parse(sql, new MySQLLexer(sql), charset);
        }

        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        public static SQLStatement Parse(string sql)
        {
            return Parse(sql, MySQLParser.DefaultCharset);
        }
    }
}