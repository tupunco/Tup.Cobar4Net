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

using Tup.Cobar.Parser.Ast.Stmt.Dml;
using Tup.Cobar.Parser.Recognizer.Mysql.Lexer;

namespace Tup.Cobar.Parser.Recognizer.Mysql.Syntax
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    [NUnit.Framework.TestFixture(Category = "MySQLDMLUpdateParserTest")]
    public class MySQLDMLUpdateParserTest : AbstractSyntaxTest
    {
        /// <summary>
        /// nothing has been pre-consumed <code><pre>
        /// 'UPDATE' 'LOW_PRIORITY'? 'IGNORE'? table_reference
        /// 'SET' colName ('='|'=') (expr|'DEFAULT') (',' colName ('='|'=') (expr|'DEFAULT'))
        /// ('WHERE' cond)?
        /// {singleTable}? =&gt; ('ORDER' 'BY' orderBy)?  ('LIMIT' count)?
        /// </pre></code>
        /// </summary>
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        [NUnit.Framework.Test]
        public virtual void TestUpdate()
        {
            string sql = "upDate LOw_PRIORITY IGNORE test.t1 sEt t1.col1=?, col2=DefaulT";
            MySQLLexer lexer = new MySQLLexer(sql);
            MySQLDMLUpdateParser parser = new MySQLDMLUpdateParser(lexer, new MySQLExprParser
                (lexer));
            DMLUpdateStatement update = parser.Update();
            string output = Output2MySQL(update, sql);
            NUnit.Framework.Assert.IsNotNull(update);
            NUnit.Framework.Assert.AreEqual("UPDATE LOW_PRIORITY IGNORE test.t1 SET t1.col1 = ?, col2 = DEFAULT"
                , output);
            sql = "upDate  IGNORE (t1) set col2=DefaulT order bY t1.col2 ";
            lexer = new MySQLLexer(sql);
            parser = new MySQLDMLUpdateParser(lexer, new MySQLExprParser(lexer));
            update = parser.Update();
            output = Output2MySQL(update, sql);
            NUnit.Framework.Assert.AreEqual("UPDATE IGNORE t1 SET col2 = DEFAULT ORDER BY t1.col2"
                , output);
            sql = "upDate   (test.t1) SET col2=DefaulT order bY t1.col2 limit ? offset 1";
            lexer = new MySQLLexer(sql);
            parser = new MySQLDMLUpdateParser(lexer, new MySQLExprParser(lexer));
            update = parser.Update();
            output = Output2MySQL(update, sql);
            NUnit.Framework.Assert.AreEqual("UPDATE test.t1 SET col2 = DEFAULT ORDER BY t1.col2 LIMIT 1, ?"
                , output);
            sql = "upDate LOW_PRIORITY  t1, test.t2 SET col2=DefaulT , col2='123''4'";
            lexer = new MySQLLexer(sql);
            parser = new MySQLDMLUpdateParser(lexer, new MySQLExprParser(lexer));
            update = parser.Update();
            output = Output2MySQL(update, sql);
            NUnit.Framework.Assert.AreEqual("UPDATE LOW_PRIORITY t1, test.t2 SET col2 = DEFAULT, col2 = '123\\'4'"
                , output);
            sql = "upDate LOW_PRIORITY  t1, test.t2 SET col2:=DefaulT , col2='123''4' where id='a'";
            lexer = new MySQLLexer(sql);
            parser = new MySQLDMLUpdateParser(lexer, new MySQLExprParser(lexer));
            update = parser.Update();
            output = Output2MySQL(update, sql);
            NUnit.Framework.Assert.AreEqual("UPDATE LOW_PRIORITY t1, test.t2 SET col2 = DEFAULT, col2 = '123\\'4' WHERE id = 'a'"
                , output);
        }
    }
}