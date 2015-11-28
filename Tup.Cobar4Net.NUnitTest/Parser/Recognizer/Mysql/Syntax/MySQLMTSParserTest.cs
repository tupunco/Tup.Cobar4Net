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
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    [TestFixture(Category = "MySQLMTSParserTest")]
    public class MySQLMTSParserTest : AbstractSyntaxTest
    {
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        [Test]
        public virtual void TestSavepint()
        {
            string sql = "  savepoint 123e123e";
            MySQLMTSParser parser = new MySQLMTSParser(new MySQLLexer(sql));
            MTSSavepointStatement savepoint = parser.Savepoint();
            string output = Output2MySQL(savepoint, sql);
            Assert.AreEqual("SAVEPOINT 123e123e", output);
            Assert.AreEqual("123e123e", savepoint.GetSavepoint().GetIdText());
            sql = "  savepoint SAVEPOINT";
            parser = new MySQLMTSParser(new MySQLLexer(sql));
            savepoint = parser.Savepoint();
            output = Output2MySQL(savepoint, sql);
            Assert.AreEqual("SAVEPOINT SAVEPOINT", output);
            Assert.AreEqual("SAVEPOINT", savepoint.GetSavepoint().GetIdText()
                );
            sql = "  savepoInt `select`";
            parser = new MySQLMTSParser(new MySQLLexer(sql));
            savepoint = parser.Savepoint();
            output = Output2MySQL(savepoint, sql);
            Assert.AreEqual("SAVEPOINT `select`", output);
            Assert.AreEqual("`select`", savepoint.GetSavepoint().GetIdText());
        }

        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        [Test]
        public virtual void TestRelease()
        {
            string sql = "Release sAVEPOINT 1234e   ";
            MySQLMTSParser parser = new MySQLMTSParser(new MySQLLexer(sql));
            MTSReleaseStatement savepoint = parser.Release();
            string output = Output2MySQL(savepoint, sql);
            Assert.AreEqual("RELEASE SAVEPOINT 1234e", output);
            Assert.AreEqual("1234e", savepoint.GetSavepoint().GetIdText());
            sql = "Release SAVEPOINT sAVEPOINT";
            parser = new MySQLMTSParser(new MySQLLexer(sql));
            savepoint = parser.Release();
            output = Output2MySQL(savepoint, sql);
            Assert.AreEqual("RELEASE SAVEPOINT sAVEPOINT", output);
            Assert.AreEqual("sAVEPOINT", savepoint.GetSavepoint().GetIdText()
                );
        }

        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        [Test]
        public virtual void TestRollback()
        {
            // ROLLBACK [WORK] TO [SAVEPOINT] identifier
            // ROLLBACK [WORK] [AND [NO] CHAIN | [NO] RELEASE]
            string sql = "rollBack work  ";
            MySQLMTSParser parser = new MySQLMTSParser(new MySQLLexer(sql));
            MTSRollbackStatement rollback = parser.Rollback();
            string output = Output2MySQL(rollback, sql);
            Assert.AreEqual("ROLLBACK", output);
            Assert.AreEqual(MTSRollbackStatement.CompleteType.UnDef, rollback
                .GetCompleteType());
            Assert.IsNull(rollback.GetSavepoint());
            sql = "rollBack  ";
            parser = new MySQLMTSParser(new MySQLLexer(sql));
            rollback = parser.Rollback();
            output = Output2MySQL(rollback, sql);
            Assert.AreEqual("ROLLBACK", output);
            Assert.AreEqual(MTSRollbackStatement.CompleteType.UnDef, rollback
                .GetCompleteType());
            Assert.IsNull(rollback.GetSavepoint());
            sql = "rollBack work TO savepoint 123e ";
            parser = new MySQLMTSParser(new MySQLLexer(sql));
            rollback = parser.Rollback();
            output = Output2MySQL(rollback, sql);
            Assert.AreEqual("ROLLBACK TO SAVEPOINT 123e", output);
            Assert.AreEqual("123e", rollback.GetSavepoint().GetIdText());
            Assert.IsTrue(rollback.GetCompleteType() == MTSRollbackStatement.CompleteType.None);
            sql = "rollBack to savePOINT savepoint ";
            parser = new MySQLMTSParser(new MySQLLexer(sql));
            rollback = parser.Rollback();
            output = Output2MySQL(rollback, sql);
            Assert.AreEqual("ROLLBACK TO SAVEPOINT savepoint", output);
            Assert.AreEqual("savepoint", rollback.GetSavepoint().GetIdText());
            Assert.IsTrue(rollback.GetCompleteType() == MTSRollbackStatement.CompleteType.None);
            sql = "rollBack to `select` ";
            parser = new MySQLMTSParser(new MySQLLexer(sql));
            rollback = parser.Rollback();
            output = Output2MySQL(rollback, sql);
            Assert.AreEqual("ROLLBACK TO SAVEPOINT `select`", output);
            Assert.AreEqual("`select`", rollback.GetSavepoint().GetIdText());
            Assert.IsTrue(rollback.GetCompleteType() == MTSRollbackStatement.CompleteType.None);
            sql = "rollBack work to  `select` ";
            parser = new MySQLMTSParser(new MySQLLexer(sql));
            rollback = parser.Rollback();
            output = Output2MySQL(rollback, sql);
            Assert.AreEqual("ROLLBACK TO SAVEPOINT `select`", output);
            Assert.AreEqual("`select`", rollback.GetSavepoint().GetIdText());
            Assert.IsTrue(rollback.GetCompleteType() == MTSRollbackStatement.CompleteType.None);
            sql = "rollBack work and no chaiN ";
            parser = new MySQLMTSParser(new MySQLLexer(sql));
            rollback = parser.Rollback();
            output = Output2MySQL(rollback, sql);
            Assert.AreEqual("ROLLBACK AND NO CHAIN", output);
            Assert.AreEqual(MTSRollbackStatement.CompleteType.NoChain, rollback
                .GetCompleteType());
            Assert.IsNull(rollback.GetSavepoint());
            sql = "rollBack work and  chaiN ";
            parser = new MySQLMTSParser(new MySQLLexer(sql));
            rollback = parser.Rollback();
            output = Output2MySQL(rollback, sql);
            Assert.AreEqual("ROLLBACK AND CHAIN", output);
            Assert.AreEqual(MTSRollbackStatement.CompleteType.Chain, rollback
                .GetCompleteType());
            Assert.IsNull(rollback.GetSavepoint());
            sql = "rollBack work NO release ";
            parser = new MySQLMTSParser(new MySQLLexer(sql));
            rollback = parser.Rollback();
            output = Output2MySQL(rollback, sql);
            Assert.AreEqual("ROLLBACK NO RELEASE", output);
            Assert.AreEqual(MTSRollbackStatement.CompleteType.NoRelease, rollback
                .GetCompleteType());
            Assert.IsNull(rollback.GetSavepoint());
            sql = "rollBack work  release ";
            parser = new MySQLMTSParser(new MySQLLexer(sql));
            rollback = parser.Rollback();
            output = Output2MySQL(rollback, sql);
            Assert.AreEqual("ROLLBACK RELEASE", output);
            Assert.AreEqual(MTSRollbackStatement.CompleteType.Release, rollback
                .GetCompleteType());
            Assert.IsNull(rollback.GetSavepoint());
            sql = "rollBack  and no chaiN ";
            parser = new MySQLMTSParser(new MySQLLexer(sql));
            rollback = parser.Rollback();
            output = Output2MySQL(rollback, sql);
            Assert.AreEqual("ROLLBACK AND NO CHAIN", output);
            Assert.AreEqual(MTSRollbackStatement.CompleteType.NoChain, rollback
                .GetCompleteType());
            Assert.IsNull(rollback.GetSavepoint());
            sql = "rollBack  and  chaiN ";
            parser = new MySQLMTSParser(new MySQLLexer(sql));
            rollback = parser.Rollback();
            output = Output2MySQL(rollback, sql);
            Assert.AreEqual("ROLLBACK AND CHAIN", output);
            Assert.AreEqual(MTSRollbackStatement.CompleteType.Chain, rollback
                .GetCompleteType());
            Assert.IsNull(rollback.GetSavepoint());
            sql = "rollBack  NO release ";
            parser = new MySQLMTSParser(new MySQLLexer(sql));
            rollback = parser.Rollback();
            output = Output2MySQL(rollback, sql);
            Assert.AreEqual("ROLLBACK NO RELEASE", output);
            Assert.AreEqual(MTSRollbackStatement.CompleteType.NoRelease, rollback
                .GetCompleteType());
            Assert.IsNull(rollback.GetSavepoint());
            sql = "rollBack   release ";
            parser = new MySQLMTSParser(new MySQLLexer(sql));
            rollback = parser.Rollback();
            output = Output2MySQL(rollback, sql);
            Assert.AreEqual("ROLLBACK RELEASE", output);
            Assert.AreEqual(MTSRollbackStatement.CompleteType.Release, rollback
                .GetCompleteType());
            Assert.IsNull(rollback.GetSavepoint());
        }
    }
}
