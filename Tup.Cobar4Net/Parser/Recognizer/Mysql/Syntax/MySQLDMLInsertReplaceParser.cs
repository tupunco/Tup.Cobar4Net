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
using Tup.Cobar4Net.Parser.Recognizer.Mysql.Lexer;

namespace Tup.Cobar4Net.Parser.Recognizer.Mysql.Syntax
{
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public abstract class MySqlDmlInsertReplaceParser : MySqlDmlParser
    {
        public MySqlDmlInsertReplaceParser(MySqlLexer lexer, MySqlExprParser exprParser)
            : base(lexer, exprParser)
        {
        }

        /// <exception cref="System.SqlSyntaxErrorException" />
        protected virtual IList<RowExpression> RowList()
        {
            IList<RowExpression> valuesList;
            var tempRowValue = RowValue();
            if (lexer.Token() == MySqlToken.PuncComma)
            {
                valuesList = new List<RowExpression>();
                valuesList.Add(new RowExpression(tempRowValue));
                for (; lexer.Token() == MySqlToken.PuncComma;)
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
        /// <exception cref="System.SqlSyntaxErrorException" />
        private IList<IExpression> RowValue()
        {
            Match(MySqlToken.PuncLeftParen);
            if (lexer.Token() == MySqlToken.PuncRightParen)
            {
                return new List<IExpression>(0);
            }
            IList<IExpression> row;
            var expr = exprParser.Expression();
            if (lexer.Token() == MySqlToken.PuncComma)
            {
                row = new List<IExpression>();
                row.Add(expr);
                for (; lexer.Token() == MySqlToken.PuncComma;)
                {
                    lexer.NextToken();
                    expr = exprParser.Expression();
                    row.Add(expr);
                }
            }
            else
            {
                row = new List<IExpression>(1);
                row.Add(expr);
            }
            Match(MySqlToken.PuncRightParen);
            return row;
        }
    }
}