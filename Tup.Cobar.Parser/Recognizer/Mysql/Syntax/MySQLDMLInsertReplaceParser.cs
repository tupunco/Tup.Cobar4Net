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
using Tup.Cobar.Parser.Recognizer.Mysql.Lexer;
using Expr = Tup.Cobar.Parser.Ast.Expression.Expression;

namespace Tup.Cobar.Parser.Recognizer.Mysql.Syntax
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public abstract class MySQLDMLInsertReplaceParser : MySQLDMLParser
    {
        public MySQLDMLInsertReplaceParser(MySQLLexer lexer, MySQLExprParser exprParser)
            : base(lexer, exprParser)
        {
        }

        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        protected virtual IList<RowExpression> RowList()
        {
            IList<RowExpression> valuesList;
            IList<Expr> tempRowValue = RowValue();
            if (lexer.Token() == MySQLToken.PuncComma)
            {
                valuesList = new List<RowExpression>();
                valuesList.Add(new RowExpression(tempRowValue));
                for (; lexer.Token() == MySQLToken.PuncComma;)
                {
                    lexer.NextToken();
                    tempRowValue = RowValue();
                    valuesList.Add(new RowExpression(tempRowValue));
                }
            }
            else
            {
                valuesList = new List<RowExpression>(1);
                valuesList.Add(new RowExpression(tempRowValue));
            }
            return valuesList;
        }

        /// <summary>first token is <code>(</code></summary>
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        private IList<Expr> RowValue()
        {
            Match(MySQLToken.PuncLeftParen);
            if (lexer.Token() == MySQLToken.PuncRightParen)
            {
                return new List<Expr>(0);
            }
            IList<Expr> row;
            Expr expr = exprParser.Expression();
            if (lexer.Token() == MySQLToken.PuncComma)
            {
                row = new List<Expr>();
                row.Add(expr);
                for (; lexer.Token() == MySQLToken.PuncComma;)
                {
                    lexer.NextToken();
                    expr = exprParser.Expression();
                    row.Add(expr);
                }
            }
            else
            {
                row = new List<Expr>(1);
                row.Add(expr);
            }
            Match(MySQLToken.PuncRightParen);
            return row;
        }
    }
}