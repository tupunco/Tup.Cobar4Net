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

using NUnit.Framework;
using Tup.Cobar4Net.Parser.Recognizer.Mysql.Lexer;

namespace Tup.Cobar4Net.Parser.Recognizer.Mysql.Syntax
{
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    [TestFixture(Category = "MySqlDmlUpdateParserTest")]
    public class MySqlDmlUpdateParserTest : AbstractSyntaxTest
    {
        /// <summary>
        ///     nothing has been pre-consumed
        ///     <code><pre>
        ///         'UPDATE' 'LOW_PRIORITY'? 'IGNORE'? table_reference
        ///         'SET' colName ('='|'=') (expr|'DEFAULT') (',' colName ('='|'=') (expr|'DEFAULT'))
        ///         ('WHERE' cond)?
        ///         {singleTable}? =&gt; ('ORDER' 'BY' orderBy)?  ('LIMIT' Count)?
        ///     </pre></code>
        /// </summary>
        /// <exception cref="System.SqlSyntaxErrorException" />
        [Test]
        public virtual void TestUpdate()
        {
            var sql = "upDate LOw_PRIORITY IGNORE test.t1 sEt t1.col1=?, col2=DefaulT";
            var lexer = new MySqlLexer(sql);
            var parser = new MySqlDmlUpdateParser(lexer, new MySqlExprParser(lexer));
            var update = parser.Update();
            var output = Output2MySql(update, sql);
            Assert.IsNotNull(update);
            Assert.AreEqual("UPDATE LOW_PRIORITY IGNORE test.t1 SET t1.col1 = ?, col2 = DEFAULT", output);
            sql = "upDate  IGNORE (t1) set col2=DefaulT order bY t1.col2 ";
            lexer = new MySqlLexer(sql);
            parser = new MySqlDmlUpdateParser(lexer, new MySqlExprParser(lexer));
            update = parser.Update();
            output = Output2MySql(update, sql);
            Assert.AreEqual("UPDATE IGNORE t1 SET col2 = DEFAULT ORDER BY t1.col2", output);
            sql = "upDate   (test.t1) SET col2=DefaulT order bY t1.col2 limit ? offset 1";
            lexer = new MySqlLexer(sql);
            parser = new MySqlDmlUpdateParser(lexer, new MySqlExprParser(lexer));
            update = parser.Update();
            output = Output2MySql(update, sql);
            Assert.AreEqual("UPDATE test.t1 SET col2 = DEFAULT ORDER BY t1.col2 LIMIT 1, ?", output);
            sql = "upDate LOW_PRIORITY  t1, test.t2 SET col2=DefaulT , col2='123''4'";
            lexer = new MySqlLexer(sql);
            parser = new MySqlDmlUpdateParser(lexer, new MySqlExprParser(lexer));
            update = parser.Update();
            output = Output2MySql(update, sql);
            Assert.AreEqual("UPDATE LOW_PRIORITY t1, test.t2 SET col2 = DEFAULT, col2 = '123\\'4'", output);
            sql = "upDate LOW_PRIORITY  t1, test.t2 SET col2:=DefaulT , col2='123''4' where id='a'";
            lexer = new MySqlLexer(sql);
            parser = new MySqlDmlUpdateParser(lexer, new MySqlExprParser(lexer));
            update = parser.Update();
            output = Output2MySql(update, sql);
            Assert.AreEqual("UPDATE LOW_PRIORITY t1, test.t2 SET col2 = DEFAULT, col2 = '123\\'4' WHERE id = 'a'", output);
        }
    }
}