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
using Tup.Cobar.Parser.Ast.Expression.Primary;
using Tup.Cobar.Parser.Ast.Fragment;
using Tup.Cobar.Parser.Ast.Fragment.Tableref;
using Tup.Cobar.Parser.Ast.Stmt.Dml;
using Tup.Cobar.Parser.Recognizer.Mysql.Lexer;

namespace Tup.Cobar.Parser.Recognizer.Mysql.Syntax
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class MySQLDMLDeleteParser : MySQLDMLParser
    {
        public MySQLDMLDeleteParser(MySQLLexer lexer, MySQLExprParser exprParser)
            : base(lexer, exprParser)
        {
        }

        private enum SpecialIdentifier
        {
            Quick
        }

        private static readonly IDictionary<string, SpecialIdentifier> specialIdentifiers = new Dictionary<string, SpecialIdentifier>();

        static MySQLDMLDeleteParser()
        {
            specialIdentifiers["QUICK"] = SpecialIdentifier.Quick;
        }

        /// <summary>
        /// first token is
        /// <see cref="Tup.Cobar.Parser.Recognizer.Mysql.MySQLToken.KwDelete"/>
        /// <code><pre>
        /// 'DELETE' 'LOW_PRIORITY'? 'QUICK'? 'IGNORE'? (
        /// 'FROM' tid ( (',' tid)* 'USING' table_refs ('WHERE' cond)?
        /// | ('WHERE' cond)? ('ORDER' 'BY' ids)? ('LIMIT' count)?  )  // single table
        /// | tid (',' tid)* 'FROM' table_refs ('WHERE' cond)? )
        /// </pre></code>
        /// </summary>
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        public virtual DMLDeleteStatement Delete()
        {
            Match(MySQLToken.KwDelete);
            bool lowPriority = false;
            bool quick = false;
            bool ignore = false;
            for (; ; lexer.NextToken())
            {
                switch (lexer.Token())
                {
                    case MySQLToken.KwLowPriority:
                        {
                            lowPriority = true;
                            break;
                        }

                    case MySQLToken.KwIgnore:
                        {
                            ignore = true;
                            break;
                        }

                    case MySQLToken.Identifier:
                        {
                            SpecialIdentifier si = specialIdentifiers.GetValue(lexer.StringValueUppercase());
                            if (SpecialIdentifier.Quick == si)
                            {
                                quick = true;
                                break;
                            }
                            goto default;
                        }

                    default:
                        {
                            goto loopOpt_break;
                        }
                }
                //loopOpt_continue:;
            }
        loopOpt_break:;
            IList<Identifier> tempList;
            TableReferences tempRefs;
            Tup.Cobar.Parser.Ast.Expression.Expression tempWhere;
            if (lexer.Token() == MySQLToken.KwFrom)
            {
                lexer.NextToken();
                Identifier id = Identifier();
                tempList = new List<Identifier>(1);
                tempList.Add(id);
                switch (lexer.Token())
                {
                    case MySQLToken.PuncComma:
                        {
                            tempList = BuildIdList(id);
                            goto case MySQLToken.KwUsing;
                        }

                    case MySQLToken.KwUsing:
                        {
                            lexer.NextToken();
                            tempRefs = TableRefs();
                            if (lexer.Token() == MySQLToken.KwWhere)
                            {
                                lexer.NextToken();
                                tempWhere = exprParser.Expression();
                                return new DMLDeleteStatement(lowPriority, quick, ignore, tempList, tempRefs, tempWhere);
                            }
                            return new DMLDeleteStatement(lowPriority, quick, ignore, tempList, tempRefs);
                        }

                    case MySQLToken.KwWhere:
                    case MySQLToken.KwOrder:
                    case MySQLToken.KwLimit:
                        {
                            break;
                        }

                    default:
                        {
                            return new DMLDeleteStatement(lowPriority, quick, ignore, id);
                        }
                }
                tempWhere = null;
                OrderBy orderBy = null;
                Limit limit = null;
                if (lexer.Token() == MySQLToken.KwWhere)
                {
                    lexer.NextToken();
                    tempWhere = exprParser.Expression();
                }
                if (lexer.Token() == MySQLToken.KwOrder)
                {
                    orderBy = OrderBy();
                }
                if (lexer.Token() == MySQLToken.KwLimit)
                {
                    limit = Limit();
                }
                return new DMLDeleteStatement(lowPriority, quick, ignore, id, tempWhere, orderBy, limit);
            }
            tempList = IdList();
            Match(MySQLToken.KwFrom);
            tempRefs = TableRefs();
            if (lexer.Token() == MySQLToken.KwWhere)
            {
                lexer.NextToken();
                tempWhere = exprParser.Expression();
                return new DMLDeleteStatement(lowPriority, quick, ignore, tempList, tempRefs, tempWhere);
            }
            return new DMLDeleteStatement(lowPriority, quick, ignore, tempList, tempRefs);
        }
    }
}