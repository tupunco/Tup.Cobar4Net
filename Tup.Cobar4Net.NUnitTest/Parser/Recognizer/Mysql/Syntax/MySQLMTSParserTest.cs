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
using Tup.Cobar4Net.Parser.Ast.Stmt.Mts;
using Tup.Cobar4Net.Parser.Recognizer.Mysql.Lexer;

namespace Tup.Cobar4Net.Parser.Recognizer.Mysql.Syntax
{
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    [TestFixture(Category = "MySqlMtsParserTest")]
    public class MySqlMtsParserTest : AbstractSyntaxTest
    {
        /// <exception cref="System.SqlSyntaxErrorException" />
        [Test]
        public virtual void TestRelease()
        {
            var sql = "Release sAVEPOINT 1234e   ";
            var parser = new MySqlMtsParser(new MySqlLexer(sql));
            var savepoint = parser.Release();
            var output = Output2MySql(savepoint, sql);
            Assert.AreEqual("RELEASE SAVEPOINT 1234e", output);
            Assert.AreEqual("1234e", savepoint.Savepoint.IdText);
            sql = "Release SAVEPOINT sAVEPOINT";
            parser = new MySqlMtsParser(new MySqlLexer(sql));
            savepoint = parser.Release();
            output = Output2MySql(savepoint, sql);
            Assert.AreEqual("RELEASE SAVEPOINT sAVEPOINT", output);
            Assert.AreEqual("sAVEPOINT", savepoint.Savepoint.IdText);
        }

        /// <exception cref="System.SqlSyntaxErrorException" />
        [Test]
        public virtual void TestRollback()
        {
            // ROLLBACK [WORK] TO [SAVEPOINT] identifier
            // ROLLBACK [WORK] [AND [NO] CHAIN | [NO] RELEASE]
            var sql = "rollBack work  ";
            var parser = new MySqlMtsParser(new MySqlLexer(sql));
            var rollback = parser.Rollback();
            var output = Output2MySql(rollback, sql);
            Assert.AreEqual("ROLLBACK", output);
            Assert.AreEqual(CompleteType.UnDef, rollback.CompleteType);
            Assert.IsNull(rollback.Savepoint);
            sql = "rollBack  ";
            parser = new MySqlMtsParser(new MySqlLexer(sql));
            rollback = parser.Rollback();
            output = Output2MySql(rollback, sql);
            Assert.AreEqual("ROLLBACK", output);
            Assert.AreEqual(CompleteType.UnDef, rollback.CompleteType);
            Assert.IsNull(rollback.Savepoint);
            sql = "rollBack work TO savepoint 123e ";
            parser = new MySqlMtsParser(new MySqlLexer(sql));
            rollback = parser.Rollback();
            output = Output2MySql(rollback, sql);
            Assert.AreEqual("ROLLBACK TO SAVEPOINT 123e", output);
            Assert.AreEqual("123e", rollback.Savepoint.IdText);
            Assert.IsTrue(rollback.CompleteType == CompleteType.None);
            sql = "rollBack to savePOINT savepoint ";
            parser = new MySqlMtsParser(new MySqlLexer(sql));
            rollback = parser.Rollback();
            output = Output2MySql(rollback, sql);
            Assert.AreEqual("ROLLBACK TO SAVEPOINT savepoint", output);
            Assert.AreEqual("savepoint", rollback.Savepoint.IdText);
            Assert.IsTrue(rollback.CompleteType == CompleteType.None);
            sql = "rollBack to `select` ";
            parser = new MySqlMtsParser(new MySqlLexer(sql));
            rollback = parser.Rollback();
            output = Output2MySql(rollback, sql);
            Assert.AreEqual("ROLLBACK TO SAVEPOINT `select`", output);
            Assert.AreEqual("`select`", rollback.Savepoint.IdText);
            Assert.IsTrue(rollback.CompleteType == CompleteType.None);
            sql = "rollBack work to  `select` ";
            parser = new MySqlMtsParser(new MySqlLexer(sql));
            rollback = parser.Rollback();
            output = Output2MySql(rollback, sql);
            Assert.AreEqual("ROLLBACK TO SAVEPOINT `select`", output);
            Assert.AreEqual("`select`", rollback.Savepoint.IdText);
            Assert.IsTrue(rollback.CompleteType == CompleteType.None);
            sql = "rollBack work and no chaiN ";
            parser = new MySqlMtsParser(new MySqlLexer(sql));
            rollback = parser.Rollback();
            output = Output2MySql(rollback, sql);
            Assert.AreEqual("ROLLBACK AND NO CHAIN", output);
            Assert.AreEqual(CompleteType.NoChain, rollback.CompleteType);
            Assert.IsNull(rollback.Savepoint);
            sql = "rollBack work and  chaiN ";
            parser = new MySqlMtsParser(new MySqlLexer(sql));
            rollback = parser.Rollback();
            output = Output2MySql(rollback, sql);
            Assert.AreEqual("ROLLBACK AND CHAIN", output);
            Assert.AreEqual(CompleteType.Chain, rollback.CompleteType);
            Assert.IsNull(rollback.Savepoint);
            sql = "rollBack work NO release ";
            parser = new MySqlMtsParser(new MySqlLexer(sql));
            rollback = parser.Rollback();
            output = Output2MySql(rollback, sql);
            Assert.AreEqual("ROLLBACK NO RELEASE", output);
            Assert.AreEqual(CompleteType.NoRelease, rollback.CompleteType);
            Assert.IsNull(rollback.Savepoint);
            sql = "rollBack work  release ";
            parser = new MySqlMtsParser(new MySqlLexer(sql));
            rollback = parser.Rollback();
            output = Output2MySql(rollback, sql);
            Assert.AreEqual("ROLLBACK RELEASE", output);
            Assert.AreEqual(CompleteType.Release, rollback.CompleteType);
            Assert.IsNull(rollback.Savepoint);
            sql = "rollBack  and no chaiN ";
            parser = new MySqlMtsParser(new MySqlLexer(sql));
            rollback = parser.Rollback();
            output = Output2MySql(rollback, sql);
            Assert.AreEqual("ROLLBACK AND NO CHAIN", output);
            Assert.AreEqual(CompleteType.NoChain, rollback.CompleteType);
            Assert.IsNull(rollback.Savepoint);
            sql = "rollBack  and  chaiN ";
            parser = new MySqlMtsParser(new MySqlLexer(sql));
            rollback = parser.Rollback();
            output = Output2MySql(rollback, sql);
            Assert.AreEqual("ROLLBACK AND CHAIN", output);
            Assert.AreEqual(CompleteType.Chain, rollback.CompleteType);
            Assert.IsNull(rollback.Savepoint);
            sql = "rollBack  NO release ";
            parser = new MySqlMtsParser(new MySqlLexer(sql));
            rollback = parser.Rollback();
            output = Output2MySql(rollback, sql);
            Assert.AreEqual("ROLLBACK NO RELEASE", output);
            Assert.AreEqual(CompleteType.NoRelease, rollback.CompleteType);
            Assert.IsNull(rollback.Savepoint);
            sql = "rollBack   release ";
            parser = new MySqlMtsParser(new MySqlLexer(sql));
            rollback = parser.Rollback();
            output = Output2MySql(rollback, sql);
            Assert.AreEqual("ROLLBACK RELEASE", output);
            Assert.AreEqual(CompleteType.Release, rollback.CompleteType);
            Assert.IsNull(rollback.Savepoint);
        }

        /// <exception cref="System.SqlSyntaxErrorException" />
        [Test]
        public virtual void TestSavepint()
        {
            var sql = "  savepoint 123e123e";
            var parser = new MySqlMtsParser(new MySqlLexer(sql));
            var savepoint = parser.Savepoint();
            var output = Output2MySql(savepoint, sql);
            Assert.AreEqual("SAVEPOINT 123e123e", output);
            Assert.AreEqual("123e123e", savepoint.Savepoint.IdText);
            sql = "  savepoint SAVEPOINT";
            parser = new MySqlMtsParser(new MySqlLexer(sql));
            savepoint = parser.Savepoint();
            output = Output2MySql(savepoint, sql);
            Assert.AreEqual("SAVEPOINT SAVEPOINT", output);
            Assert.AreEqual("SAVEPOINT", savepoint.Savepoint.IdText);
            sql = "  savepoInt `select`";
            parser = new MySqlMtsParser(new MySqlLexer(sql));
            savepoint = parser.Savepoint();
            output = Output2MySql(savepoint, sql);
            Assert.AreEqual("SAVEPOINT `select`", output);
            Assert.AreEqual("`select`", savepoint.Savepoint.IdText);
        }
    }
}