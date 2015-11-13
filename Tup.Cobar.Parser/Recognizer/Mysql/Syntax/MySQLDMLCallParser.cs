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
using Tup.Cobar.Parser.Ast.Stmt.Dml;
using Tup.Cobar.Parser.Recognizer.Mysql.Lexer;

namespace Tup.Cobar.Parser.Recognizer.Mysql.Syntax
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class MySQLDMLCallParser : MySQLDMLParser
    {
        public MySQLDMLCallParser(MySQLLexer lexer, MySQLExprParser exprParser)
            : base(lexer, exprParser)
        {
        }

        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        public virtual DMLCallStatement Call()
        {
            Match(MySQLToken.KwCall);
            Identifier procedure = Identifier();
            Match(MySQLToken.PuncLeftParen);
            if (lexer.Token() == MySQLToken.PuncRightParen)
            {
                lexer.NextToken();
                return new DMLCallStatement(procedure);
            }
            IList<Tup.Cobar.Parser.Ast.Expression.Expression> arguments;
            Tup.Cobar.Parser.Ast.Expression.Expression expr = exprParser.Expression();
            switch (lexer.Token())
            {
                case MySQLToken.PuncComma:
                    {
                        arguments = new List<Tup.Cobar.Parser.Ast.Expression.Expression>();
                        arguments.Add(expr);
                        for (; lexer.Token() == MySQLToken.PuncComma;)
                        {
                            lexer.NextToken();
                            expr = exprParser.Expression();
                            arguments.Add(expr);
                        }
                        Match(MySQLToken.PuncRightParen);
                        return new DMLCallStatement(procedure, arguments);
                    }

                case MySQLToken.PuncRightParen:
                    {
                        lexer.NextToken();
                        arguments = new List<Tup.Cobar.Parser.Ast.Expression.Expression>(1);
                        arguments.Add(expr);
                        return new DMLCallStatement(procedure, arguments);
                    }

                default:
                    {
                        throw Err("expect ',' or ')' after first argument of procedure");
                    }
            }
        }
    }
}