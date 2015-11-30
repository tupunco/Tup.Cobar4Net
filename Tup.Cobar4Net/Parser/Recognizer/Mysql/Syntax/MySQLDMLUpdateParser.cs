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
using Tup.Cobar4Net.Parser.Ast.Stmt.Dml;
using Tup.Cobar4Net.Parser.Recognizer.Mysql.Lexer;
using Tup.Cobar4Net.Parser.Util;

namespace Tup.Cobar4Net.Parser.Recognizer.Mysql.Syntax
{
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public class MySqlDmlUpdateParser : MySqlDmlParser
    {
        public MySqlDmlUpdateParser(MySqlLexer lexer, MySqlExprParser exprParser)
            : base(lexer, exprParser)
        {
        }

        /// <summary>
        ///     nothing has been pre-consumed
        ///     <code><pre>
        ///         'UPDATE' 'LOW_PRIORITY'? 'IGNORE'? table_reference
        ///         'SET' colName ('='|':=') (expr|'DEFAULT') (',' colName ('='|':=') (expr|'DEFAULT'))
        ///         ('WHERE' cond)?
        ///         {singleTable}? =&gt; ('ORDER' 'BY' orderBy)?  ('LIMIT' Count)?
        ///     </pre></code>
        /// </summary>
        /// <exception cref="System.SqlSyntaxErrorException" />
        public virtual DmlUpdateStatement Update()
        {
            Match(MySqlToken.KwUpdate);
            var lowPriority = false;
            var ignore = false;
            if (lexer.Token() == MySqlToken.KwLowPriority)
            {
                lexer.NextToken();
                lowPriority = true;
            }
            if (lexer.Token() == MySqlToken.KwIgnore)
            {
                lexer.NextToken();
                ignore = true;
            }
            var tableRefs = TableRefs();
            Match(MySqlToken.KwSet);
            IList<Pair<Identifier, IExpression>> values;
            var col = Identifier();
            Match(MySqlToken.OpEquals, MySqlToken.OpAssign);
            var expr = exprParser.Expression();
            if (lexer.Token() == MySqlToken.PuncComma)
            {
                values = new List<Pair<Identifier, IExpression>>();
                values.Add(new Pair<Identifier, IExpression>(col, expr));
                for (; lexer.Token() == MySqlToken.PuncComma;)
                {
                    lexer.NextToken();
                    col = Identifier();
                    Match(MySqlToken.OpEquals, MySqlToken.OpAssign);
                    expr = exprParser.Expression();
                    values.Add(new Pair<Identifier, IExpression>(col, expr));
                }
            }
            else
            {
                values = new List<Pair<Identifier, IExpression>>(1);
                values.Add(new Pair<Identifier, IExpression>(col, expr));
            }
            IExpression where = null;
            if (lexer.Token() == MySqlToken.KwWhere)
            {
                lexer.NextToken();
                where = exprParser.Expression();
            }
            OrderBy orderBy = null;
            Limit limit = null;
            if (tableRefs.IsSingleTable)
            {
                orderBy = OrderBy();
                limit = Limit();
            }
            return new DmlUpdateStatement(lowPriority, ignore, tableRefs, values, where, orderBy, limit);
        }
    }
}