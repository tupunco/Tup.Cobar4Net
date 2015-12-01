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
using Sharpen;
using Tup.Cobar4Net.Parser.Ast.Stmt;
using Tup.Cobar4Net.Parser.Ast.Stmt.Ddl;
using Tup.Cobar4Net.Parser.Recognizer.Mysql;
using Tup.Cobar4Net.Parser.Recognizer.Mysql.Lexer;
using Tup.Cobar4Net.Parser.Recognizer.Mysql.Syntax;

namespace Tup.Cobar4Net.Parser.Recognizer
{
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public sealed class SqlParserDelegate
    {
        private static readonly IDictionary<string, SpecialIdentifier>
            SpecialIdentifiers = new Dictionary<string, SpecialIdentifier>();

        static SqlParserDelegate()
        {
            SpecialIdentifiers["TRUNCATE"] = SpecialIdentifier.Truncate;
            SpecialIdentifiers["SAVEPOINT"] = SpecialIdentifier.Savepoint;
            SpecialIdentifiers["ROLLBACK"] = SpecialIdentifier.Rollback;
        }

        private static bool IsEOFedDdl(ISqlStatement stmt)
        {
            if (stmt is IDdlStatement && stmt is DdlCreateIndexStatement)
                return false;

            return true;
        }

        private static string BuildErrorMsg(Exception e, MySqlLexer lexer, string sql)
        {
            var sb = new StringBuilder("You have an error in your Sql syntax; Error occurs around this fragment: ");
            var ch = lexer.CurrentIndex;
            var from = ch - 16;
            if (from < 0)
            {
                from = 0;
            }
            var to = ch + 9;
            if (to >= sql.Length)
            {
                to = sql.Length - 1;
            }
            var fragment = Runtime.Substring(sql, from, to + 1);
            sb.Append('{').Append(fragment).Append('}').Append(". Error cause: " + e.Message);
            return sb.ToString();
        }

        /// <exception cref="System.SqlSyntaxErrorException" />
        public static ISqlStatement Parse(string sql, MySqlLexer lexer, string charset)
        {
            try
            {
                ISqlStatement stmt = null;
                var isEof = true;
                var exprParser = new MySqlExprParser(lexer, charset);
                switch (lexer.Token())
                {
                    case MySqlToken.KwDesc:
                    case MySqlToken.KwDescribe:
                    {
                        stmt = new MySqlDalParser(lexer, exprParser).Desc();
                        goto stmtSwitch_break;
                    }

                    case MySqlToken.KwSelect:
                    case MySqlToken.PuncLeftParen:
                    {
                        stmt = new MySqlDmlSelectParser(lexer, exprParser).SelectUnion();
                        goto stmtSwitch_break;
                    }

                    case MySqlToken.KwDelete:
                    {
                        stmt = new MySqlDmlDeleteParser(lexer, exprParser).Delete();
                        goto stmtSwitch_break;
                    }

                    case MySqlToken.KwInsert:
                    {
                        stmt = new MySqlDmlInsertParser(lexer, exprParser).Insert();
                        goto stmtSwitch_break;
                    }

                    case MySqlToken.KwReplace:
                    {
                        stmt = new MySqlDmlReplaceParser(lexer, exprParser).Replace();
                        goto stmtSwitch_break;
                    }

                    case MySqlToken.KwUpdate:
                    {
                        stmt = new MySqlDmlUpdateParser(lexer, exprParser).Update();
                        goto stmtSwitch_break;
                    }

                    case MySqlToken.KwCall:
                    {
                        stmt = new MySqlDmlCallParser(lexer, exprParser).Call();
                        goto stmtSwitch_break;
                    }

                    case MySqlToken.KwSet:
                    {
                        stmt = new MySqlDalParser(lexer, exprParser).Set();
                        goto stmtSwitch_break;
                    }

                    case MySqlToken.KwShow:
                    {
                        stmt = new MySqlDalParser(lexer, exprParser).Show();
                        goto stmtSwitch_break;
                    }

                    case MySqlToken.KwAlter:
                    case MySqlToken.KwCreate:
                    case MySqlToken.KwDrop:
                    case MySqlToken.KwRename:
                    {
                        stmt = new MySqlDdlParser(lexer, exprParser).DdlStmt();
                        isEof = IsEOFedDdl(stmt);
                        goto stmtSwitch_break;
                    }

                    case MySqlToken.KwRelease:
                    {
                        stmt = new MySqlMtsParser(lexer).Release();
                        goto stmtSwitch_break;
                    }

                    case MySqlToken.Identifier:
                    {
                        var si = SpecialIdentifier.None;
                        if ((si = SpecialIdentifiers[lexer.GetStringValueUppercase()]) != SpecialIdentifier.None)
                        {
                            switch (si)
                            {
                                case SpecialIdentifier.Truncate:
                                {
                                    stmt = new MySqlDdlParser(lexer, exprParser).Truncate();
                                    goto stmtSwitch_break;
                                }

                                case SpecialIdentifier.Savepoint:
                                {
                                    stmt = new MySqlMtsParser(lexer).Savepoint();
                                    goto stmtSwitch_break;
                                }

                                case SpecialIdentifier.Rollback:
                                {
                                    stmt = new MySqlMtsParser(lexer).Rollback();
                                    goto stmtSwitch_break;
                                }
                            }
                        }
                        goto default;
                    }

                    default:
                    {
                        throw new SqlSyntaxErrorException("sql is not a supported statement");
                    }
                }
                stmtSwitch_break:
                ;
                if (isEof)
                {
                    while (lexer.Token() == MySqlToken.PuncSemicolon)
                    {
                        lexer.NextToken();
                    }
                    if (lexer.Token() != MySqlToken.Eof)
                    {
                        throw new SqlSyntaxErrorException("Sql syntax error!");
                    }
                }
                return stmt;
            }
            catch (Exception e)
            {
                throw new SqlSyntaxErrorException(BuildErrorMsg(e, lexer, sql), e);
            }
        }

        /// <exception cref="System.SqlSyntaxErrorException" />
        public static ISqlStatement Parse(string sql, string charset)
        {
            return Parse(sql, new MySqlLexer(sql), charset);
        }

        /// <exception cref="System.SqlSyntaxErrorException" />
        public static ISqlStatement Parse(string sql)
        {
            return Parse(sql, MySqlParser.DefaultCharset);
        }

        /// <summary>
        ///     SqlParserDelegate SpecialIdentifier
        /// </summary>
        private enum SpecialIdentifier
        {
            None = 0,

            Rollback,
            Savepoint,
            Truncate
        }
    }
}