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
using Tup.Cobar4Net.Parser.Ast.Stmt.Ddl;
using Tup.Cobar4Net.Parser.Recognizer.Mysql.Lexer;

namespace Tup.Cobar4Net.Parser.Recognizer.Mysql.Syntax
{
    /// <author>
    ///     <a href="mailto:danping.yudp@alibaba-inc.com">YU Danping</a>
    /// </author>
    [TestFixture(Category = "MySqlDdlParserTest")]
    public class MySqlDdlParserTest : AbstractSyntaxTest
    {
        /// <exception cref="System.Exception" />
        [Test]
        public virtual void TestDdlStmt()
        {
            var sql = "alTer ignore table tb_name";
            var lexer = new MySqlLexer(sql);
            var parser = new MySqlDdlParser(lexer, new MySqlExprParser(lexer));
            var dst = parser.DdlStmt();
            sql = "alTeR table tb_name";
            lexer = new MySqlLexer(sql);
            parser = new MySqlDdlParser(lexer, new MySqlExprParser(lexer));
            dst = parser.DdlStmt();
            sql = "crEate temporary tabLe if not exists tb_name";
            lexer = new MySqlLexer(sql);
            parser = new MySqlDdlParser(lexer, new MySqlExprParser(lexer));
            dst = parser.DdlStmt();
            sql = "crEate tabLe if not exists tb_name";
            lexer = new MySqlLexer(sql);
            parser = new MySqlDdlParser(lexer, new MySqlExprParser(lexer));
            dst = parser.DdlStmt();
            sql = "crEate temporary tabLe tb_name";
            lexer = new MySqlLexer(sql);
            parser = new MySqlDdlParser(lexer, new MySqlExprParser(lexer));
            dst = parser.DdlStmt();
            sql = "crEate unique index index_name on tb(col(id)) desc";
            lexer = new MySqlLexer(sql);
            parser = new MySqlDdlParser(lexer, new MySqlExprParser(lexer));
            dst = parser.DdlStmt();
            sql = "crEate fulltext index index_name on tb(col(id))";
            lexer = new MySqlLexer(sql);
            parser = new MySqlDdlParser(lexer, new MySqlExprParser(lexer));
            dst = parser.DdlStmt();
            sql = "crEate spatial index index_name on tb(col(id))";
            lexer = new MySqlLexer(sql);
            parser = new MySqlDdlParser(lexer, new MySqlExprParser(lexer));
            dst = parser.DdlStmt();
            sql = "crEate index index_name using hash on tb(col(id))";
            lexer = new MySqlLexer(sql);
            parser = new MySqlDdlParser(lexer, new MySqlExprParser(lexer));
            dst = parser.DdlStmt();
            sql = "drop index index_name on tb1";
            lexer = new MySqlLexer(sql);
            parser = new MySqlDdlParser(lexer, new MySqlExprParser(lexer));
            dst = parser.DdlStmt();
            var output = Output2MySql(dst, sql);
            Assert.AreEqual("DROP INDEX index_name ON tb1", output);
            sql = "drop temporary tabLe if exists tb1,tb2,tb3 restrict";
            lexer = new MySqlLexer(sql);
            parser = new MySqlDdlParser(lexer, new MySqlExprParser(lexer));
            dst = parser.DdlStmt();
            output = Output2MySql(dst, sql);
            Assert.AreEqual("DROP TEMPORARY TABLE IF EXISTS tb1, tb2, tb3 RESTRICT", output);
            sql = "drop temporary tabLe if exists tb1,tb2,tb3 cascade";
            lexer = new MySqlLexer(sql);
            parser = new MySqlDdlParser(lexer, new MySqlExprParser(lexer));
            dst = parser.DdlStmt();
            output = Output2MySql(dst, sql);
            Assert.AreEqual("DROP TEMPORARY TABLE IF EXISTS tb1, tb2, tb3 CASCADE", output);
            sql = "drop temporary tabLe if exists tb1 cascade";
            lexer = new MySqlLexer(sql);
            parser = new MySqlDdlParser(lexer, new MySqlExprParser(lexer));
            dst = parser.DdlStmt();
            output = Output2MySql(dst, sql);
            Assert.AreEqual("DROP TEMPORARY TABLE IF EXISTS tb1 CASCADE", output);
            sql = "drop tabLe if exists tb1 cascade";
            lexer = new MySqlLexer(sql);
            parser = new MySqlDdlParser(lexer, new MySqlExprParser(lexer));
            dst = parser.DdlStmt();
            output = Output2MySql(dst, sql);
            Assert.AreEqual("DROP TABLE IF EXISTS tb1 CASCADE", output);
            sql = "drop temporary tabLe tb1 cascade";
            lexer = new MySqlLexer(sql);
            parser = new MySqlDdlParser(lexer, new MySqlExprParser(lexer));
            dst = parser.DdlStmt();
            output = Output2MySql(dst, sql);
            Assert.AreEqual("DROP TEMPORARY TABLE tb1 CASCADE", output);
            sql = "rename table tb1 to ntb1,tb2 to ntb2";
            lexer = new MySqlLexer(sql);
            parser = new MySqlDdlParser(lexer, new MySqlExprParser(lexer));
            dst = parser.DdlStmt();
            output = Output2MySql(dst, sql);
            Assert.AreEqual("RENAME TABLE tb1 TO ntb1, tb2 TO ntb2", output);
            sql = "rename table tb1 to ntb1";
            lexer = new MySqlLexer(sql);
            parser = new MySqlDdlParser(lexer, new MySqlExprParser(lexer));
            dst = parser.DdlStmt();
            output = Output2MySql(dst, sql);
            Assert.AreEqual("RENAME TABLE tb1 TO ntb1", output);
        }

        /// <exception cref="System.Exception" />
        [Test]
        public virtual void TestTruncate()
        {
            var sql = "Truncate table tb1";
            var lexer = new MySqlLexer(sql);
            var parser = new MySqlDdlParser(lexer, new MySqlExprParser(lexer));
            IDdlStatement trun = parser.Truncate();
            parser.Match(MySqlToken.Eof);
            var output = Output2MySql(trun, sql);
            Assert.AreEqual("TRUNCATE TABLE tb1", output);
            sql = "Truncate tb1";
            lexer = new MySqlLexer(sql);
            parser = new MySqlDdlParser(lexer, new MySqlExprParser(lexer));
            trun = parser.Truncate();
            parser.Match(MySqlToken.Eof);
            output = Output2MySql(trun, sql);
            Assert.AreEqual("TRUNCATE TABLE tb1", output);
        }
    }
}