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
using Tup.Cobar.Parser.Util;
using Expr = Tup.Cobar.Parser.Ast.Expression.Expression;

namespace Tup.Cobar.Parser.Recognizer.Mysql.Syntax
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class MySQLDMLUpdateParser : MySQLDMLParser
    {
        public MySQLDMLUpdateParser(MySQLLexer lexer, MySQLExprParser exprParser)
            : base(lexer, exprParser)
        {
        }

        /// <summary>
        /// nothing has been pre-consumed <code><pre>
        /// 'UPDATE' 'LOW_PRIORITY'? 'IGNORE'? table_reference
        /// 'SET' colName ('='|':=') (expr|'DEFAULT') (',' colName ('='|':=') (expr|'DEFAULT'))
        /// ('WHERE' cond)?
        /// {singleTable}? =&gt; ('ORDER' 'BY' orderBy)?  ('LIMIT' count)?
        /// </pre></code>
        /// </summary>
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        public virtual DMLUpdateStatement Update()
        {
            Match(MySQLToken.KwUpdate);
            bool lowPriority = false;
            bool ignore = false;
            if (lexer.Token() == MySQLToken.KwLowPriority)
            {
                lexer.NextToken();
                lowPriority = true;
            }
            if (lexer.Token() == MySQLToken.KwIgnore)
            {
                lexer.NextToken();
                ignore = true;
            }
            TableReferences tableRefs = TableRefs();
            Match(MySQLToken.KwSet);
            IList<Pair<Identifier, Expr>> values;
            Identifier col = Identifier();
            Match(MySQLToken.OpEquals, MySQLToken.OpAssign);
            Expr expr = exprParser.Expression();
            if (lexer.Token() == MySQLToken.PuncComma)
            {
                values = new List<Pair<Identifier, Expr>>();
                values.Add(new Pair<Identifier, Expr>(col, expr));
                for (; lexer.Token() == MySQLToken.PuncComma;)
                {
                    lexer.NextToken();
                    col = Identifier();
                    Match(MySQLToken.OpEquals, MySQLToken.OpAssign);
                    expr = exprParser.Expression();
                    values.Add(new Pair<Identifier, Expr>(col, expr));
                }
            }
            else
            {
                values = new List<Pair<Identifier, Expr>>(1);
                values.Add(new Pair<Identifier, Expr>(col, expr));
            }
            Expr where = null;
            if (lexer.Token() == MySQLToken.KwWhere)
            {
                lexer.NextToken();
                where = exprParser.Expression();
            }
            OrderBy orderBy = null;
            Limit limit = null;
            if (tableRefs.IsSingleTable())
            {
                orderBy = OrderBy();
                limit = Limit();
            }
            return new DMLUpdateStatement(lowPriority, ignore, tableRefs, values, where, orderBy, limit);
        }
    }
}