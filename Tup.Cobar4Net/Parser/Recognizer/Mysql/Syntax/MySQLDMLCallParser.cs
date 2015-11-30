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
using Tup.Cobar4Net.Parser.Ast.Stmt.Dml;
using Tup.Cobar4Net.Parser.Recognizer.Mysql.Lexer;

namespace Tup.Cobar4Net.Parser.Recognizer.Mysql.Syntax
{
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public class MySqlDmlCallParser : MySqlDmlParser
    {
        public MySqlDmlCallParser(MySqlLexer lexer, MySqlExprParser exprParser)
            : base(lexer, exprParser)
        {
        }

        /// <exception cref="System.SqlSyntaxErrorException" />
        public virtual DmlCallStatement Call()
        {
            Match(MySqlToken.KwCall);
            var procedure = Identifier();
            Match(MySqlToken.PuncLeftParen);
            if (lexer.Token() == MySqlToken.PuncRightParen)
            {
                lexer.NextToken();
                return new DmlCallStatement(procedure);
            }
            IList<IExpression> arguments;
            var expr = exprParser.Expression();
            switch (lexer.Token())
            {
                case MySqlToken.PuncComma:
                {
                    arguments = new List<IExpression>();
                    arguments.Add(expr);
                    for (; lexer.Token() == MySqlToken.PuncComma;)
                    {
                        lexer.NextToken();
                        expr = exprParser.Expression();
                        arguments.Add(expr);
                    }
                    Match(MySqlToken.PuncRightParen);
                    return new DmlCallStatement(procedure, arguments);
                }

                case MySqlToken.PuncRightParen:
                {
                    lexer.NextToken();
                    arguments = new List<IExpression>(1);
                    arguments.Add(expr);
                    return new DmlCallStatement(procedure, arguments);
                }

                default:
                {
                    throw Err("expect ',' or ')' after first argument of procedure");
                }
            }
        }
    }
}