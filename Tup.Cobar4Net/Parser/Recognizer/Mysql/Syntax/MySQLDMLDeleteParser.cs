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
using Tup.Cobar4Net.Parser.Ast.Expression.Primary;
using Tup.Cobar4Net.Parser.Ast.Fragment;
using Tup.Cobar4Net.Parser.Ast.Fragment.Tableref;
using Tup.Cobar4Net.Parser.Ast.Stmt.Dml;
using Tup.Cobar4Net.Parser.Recognizer.Mysql.Lexer;

namespace Tup.Cobar4Net.Parser.Recognizer.Mysql.Syntax
{
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public class MySqlDmlDeleteParser : MySqlDmlParser
    {
        private static readonly IDictionary<string, SpecialIdentifier> specialIdentifiers =
            new Dictionary<string, SpecialIdentifier>();

        static MySqlDmlDeleteParser()
        {
            specialIdentifiers["QUICK"] = SpecialIdentifier.Quick;
        }

        public MySqlDmlDeleteParser(MySqlLexer lexer, MySqlExprParser exprParser)
            : base(lexer, exprParser)
        {
        }

        /// <summary>
        ///     first token is
        ///     <see cref="MySqlToken.KwDelete" />
        ///     <code><pre>
        ///         'DELETE' 'LOW_PRIORITY'? 'QUICK'? 'IGNORE'? (
        ///         'FROM' tid ( (',' tid)* 'USING' table_refs ('WHERE' cond)?
        ///         | ('WHERE' cond)? ('ORDER' 'BY' ids)? ('LIMIT' Count)?  )  // single table
        ///         | tid (',' tid)* 'FROM' table_refs ('WHERE' cond)? )
        ///     </pre></code>
        /// </summary>
        /// <exception cref="System.SqlSyntaxErrorException" />
        public virtual DmlDeleteStatement Delete()
        {
            Match(MySqlToken.KwDelete);
            var lowPriority = false;
            var quick = false;
            var ignore = false;
            for (;; lexer.NextToken())
            {
                switch (lexer.Token())
                {
                    case MySqlToken.KwLowPriority:
                    {
                        lowPriority = true;
                        break;
                    }

                    case MySqlToken.KwIgnore:
                    {
                        ignore = true;
                        break;
                    }

                    case MySqlToken.Identifier:
                    {
                        var si = specialIdentifiers.GetValue(lexer.GetStringValueUppercase());
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
            loopOpt_break:
            ;
            IList<Identifier> tempList;
            TableReferences tempRefs;
            IExpression tempWhere;
            if (lexer.Token() == MySqlToken.KwFrom)
            {
                lexer.NextToken();
                var id = Identifier();
                tempList = new List<Identifier>(1);
                tempList.Add(id);
                switch (lexer.Token())
                {
                    case MySqlToken.PuncComma:
                    {
                        tempList = BuildIdList(id);
                        goto case MySqlToken.KwUsing;
                    }

                    case MySqlToken.KwUsing:
                    {
                        lexer.NextToken();
                        tempRefs = TableRefs();
                        if (lexer.Token() == MySqlToken.KwWhere)
                        {
                            lexer.NextToken();
                            tempWhere = exprParser.Expression();
                            return new DmlDeleteStatement(lowPriority, quick, ignore, tempList, tempRefs, tempWhere);
                        }
                        return new DmlDeleteStatement(lowPriority, quick, ignore, tempList, tempRefs);
                    }

                    case MySqlToken.KwWhere:
                    case MySqlToken.KwOrder:
                    case MySqlToken.KwLimit:
                    {
                        break;
                    }

                    default:
                    {
                        return new DmlDeleteStatement(lowPriority, quick, ignore, id);
                    }
                }
                tempWhere = null;
                OrderBy orderBy = null;
                Limit limit = null;
                if (lexer.Token() == MySqlToken.KwWhere)
                {
                    lexer.NextToken();
                    tempWhere = exprParser.Expression();
                }
                if (lexer.Token() == MySqlToken.KwOrder)
                {
                    orderBy = OrderBy();
                }
                if (lexer.Token() == MySqlToken.KwLimit)
                {
                    limit = Limit();
                }
                return new DmlDeleteStatement(lowPriority, quick, ignore, id, tempWhere, orderBy, limit);
            }
            tempList = IdList();
            Match(MySqlToken.KwFrom);
            tempRefs = TableRefs();
            if (lexer.Token() == MySqlToken.KwWhere)
            {
                lexer.NextToken();
                tempWhere = exprParser.Expression();
                return new DmlDeleteStatement(lowPriority, quick, ignore, tempList, tempRefs, tempWhere);
            }
            return new DmlDeleteStatement(lowPriority, quick, ignore, tempList, tempRefs);
        }

        /// <summary>
        ///     MySqlDmlDeleteParser SpecialIdentifier
        /// </summary>
        private enum SpecialIdentifier
        {
            None = 0,

            Quick
        }
    }
}