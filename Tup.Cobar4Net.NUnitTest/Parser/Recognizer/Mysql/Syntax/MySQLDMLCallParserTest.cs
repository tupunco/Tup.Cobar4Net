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

using Tup.Cobar4Net.Parser.Ast.Stmt.Dml;
using Tup.Cobar4Net.Parser.Recognizer.Mysql.Lexer;

namespace Tup.Cobar4Net.Parser.Recognizer.Mysql.Syntax
{
    /// <author><a href="mailto:danping.yudp@alibaba-inc.com">YU Danping</a></author>
    [TestFixture(Category = "MySQLDMLCallParserTest")]
    public class MySQLDMLCallParserTest : AbstractSyntaxTest
    {
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        [Test]
        public virtual void TestCall()
        {
            string sql = "call p(?,?) ";
            MySQLLexer lexer = new MySQLLexer(sql);
            MySQLDMLCallParser parser = new MySQLDMLCallParser(lexer, new MySQLExprParser(lexer
                ));
            DMLCallStatement calls = parser.Call();
            parser.Match(MySQLToken.Eof);
            string output = Output2MySQL(calls, sql);
            Assert.AreEqual("CALL p(?, ?)", output);
            sql = "call p(@var1,'@var2',var3)";
            lexer = new MySQLLexer(sql);
            parser = new MySQLDMLCallParser(lexer, new MySQLExprParser(lexer));
            calls = parser.Call();
            parser.Match(MySQLToken.Eof);
            output = Output2MySQL(calls, sql);
            Assert.AreEqual("CALL p(@var1, '@var2', var3)", output);
            sql = "call p()";
            lexer = new MySQLLexer(sql);
            parser = new MySQLDMLCallParser(lexer, new MySQLExprParser(lexer));
            calls = parser.Call();
            parser.Match(MySQLToken.Eof);
            output = Output2MySQL(calls, sql);
            Assert.AreEqual("CALL p()", output);
            sql = "call p(?)";
            lexer = new MySQLLexer(sql);
            parser = new MySQLDMLCallParser(lexer, new MySQLExprParser(lexer));
            calls = parser.Call();
            parser.Match(MySQLToken.Eof);
            output = Output2MySQL(calls, sql);
            Assert.AreEqual("CALL p(?)", output);
        }
    }
}
