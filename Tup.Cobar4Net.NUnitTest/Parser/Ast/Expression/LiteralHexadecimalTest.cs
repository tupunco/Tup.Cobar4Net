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
using Tup.Cobar4Net.Parser.Ast.Expression.Primary.Literal;
using Tup.Cobar4Net.Parser.Recognizer.Mysql.Lexer;
using Tup.Cobar4Net.Parser.Recognizer.Mysql.Syntax;

namespace Tup.Cobar4Net.Parser.Ast.Expression
{
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    [TestFixture(Category = "LiteralHexadecimalTest")]
    public class LiteralHexadecimalTest
    {
        /// <exception cref="System.Exception" />
        [Test]
        public virtual void TestUtf8()
        {
            var sql = "x'E982B1E7A195275C73'";
            var hex = (LiteralHexadecimal)new MySqlExprParser(new MySqlLexer(sql), "utf-8").Expression();
            Assert.AreEqual("邱硕'\\s", hex.Evaluation(null));
            sql = "x'd0A'";
            hex = (LiteralHexadecimal)new MySqlExprParser(new MySqlLexer(sql), "utf-8").Expression();
            Assert.AreEqual("\r\n", hex.Evaluation(null));
        }
    }
}