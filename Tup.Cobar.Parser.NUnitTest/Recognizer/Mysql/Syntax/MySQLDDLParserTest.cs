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

using Tup.Cobar.Parser.Ast.Stmt.Ddl;
using Tup.Cobar.Parser.Recognizer.Mysql.Lexer;

namespace Tup.Cobar.Parser.Recognizer.Mysql.Syntax
{
    /// <author><a href="mailto:danping.yudp@alibaba-inc.com">YU Danping</a></author>
    [NUnit.Framework.TestFixture(Category = "MySQLDDLParserTest")]
    public class MySQLDDLParserTest : AbstractSyntaxTest
    {
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestTruncate()
        {
            string sql = "Truncate table tb1";
            MySQLLexer lexer = new MySQLLexer(sql);
            MySQLDDLParser parser = new MySQLDDLParser(lexer, new MySQLExprParser(lexer));
            DDLStatement trun = (DDLTruncateStatement)parser.Truncate();
            parser.Match(MySQLToken.Eof);
            string output = Output2MySQL(trun, sql);
            NUnit.Framework.Assert.AreEqual("TRUNCATE TABLE tb1", output);
            sql = "Truncate tb1";
            lexer = new MySQLLexer(sql);
            parser = new MySQLDDLParser(lexer, new MySQLExprParser(lexer));
            trun = (DDLTruncateStatement)parser.Truncate();
            parser.Match(MySQLToken.Eof);
            output = Output2MySQL(trun, sql);
            NUnit.Framework.Assert.AreEqual("TRUNCATE TABLE tb1", output);
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestDDLStmt()
        {
            string sql = "alTer ignore table tb_name";
            MySQLLexer lexer = new MySQLLexer(sql);
            MySQLDDLParser parser = new MySQLDDLParser(lexer, new MySQLExprParser(lexer));
            DDLStatement dst = parser.DdlStmt();
            sql = "alTeR table tb_name";
            lexer = new MySQLLexer(sql);
            parser = new MySQLDDLParser(lexer, new MySQLExprParser(lexer));
            dst = parser.DdlStmt();
            sql = "crEate temporary tabLe if not exists tb_name";
            lexer = new MySQLLexer(sql);
            parser = new MySQLDDLParser(lexer, new MySQLExprParser(lexer));
            dst = parser.DdlStmt();
            sql = "crEate tabLe if not exists tb_name";
            lexer = new MySQLLexer(sql);
            parser = new MySQLDDLParser(lexer, new MySQLExprParser(lexer));
            dst = parser.DdlStmt();
            sql = "crEate temporary tabLe tb_name";
            lexer = new MySQLLexer(sql);
            parser = new MySQLDDLParser(lexer, new MySQLExprParser(lexer));
            dst = parser.DdlStmt();
            sql = "crEate unique index index_name on tb(col(id)) desc";
            lexer = new MySQLLexer(sql);
            parser = new MySQLDDLParser(lexer, new MySQLExprParser(lexer));
            dst = parser.DdlStmt();
            sql = "crEate fulltext index index_name on tb(col(id))";
            lexer = new MySQLLexer(sql);
            parser = new MySQLDDLParser(lexer, new MySQLExprParser(lexer));
            dst = parser.DdlStmt();
            sql = "crEate spatial index index_name on tb(col(id))";
            lexer = new MySQLLexer(sql);
            parser = new MySQLDDLParser(lexer, new MySQLExprParser(lexer));
            dst = parser.DdlStmt();
            sql = "crEate index index_name using hash on tb(col(id))";
            lexer = new MySQLLexer(sql);
            parser = new MySQLDDLParser(lexer, new MySQLExprParser(lexer));
            dst = parser.DdlStmt();
            sql = "drop index index_name on tb1";
            lexer = new MySQLLexer(sql);
            parser = new MySQLDDLParser(lexer, new MySQLExprParser(lexer));
            dst = parser.DdlStmt();
            string output = Output2MySQL(dst, sql);
            NUnit.Framework.Assert.AreEqual("DROP INDEX index_name ON tb1", output);
            sql = "drop temporary tabLe if exists tb1,tb2,tb3 restrict";
            lexer = new MySQLLexer(sql);
            parser = new MySQLDDLParser(lexer, new MySQLExprParser(lexer));
            dst = parser.DdlStmt();
            output = Output2MySQL(dst, sql);
            NUnit.Framework.Assert.AreEqual("DROP TEMPORARY TABLE IF EXISTS tb1, tb2, tb3 RESTRICT"
                , output);
            sql = "drop temporary tabLe if exists tb1,tb2,tb3 cascade";
            lexer = new MySQLLexer(sql);
            parser = new MySQLDDLParser(lexer, new MySQLExprParser(lexer));
            dst = parser.DdlStmt();
            output = Output2MySQL(dst, sql);
            NUnit.Framework.Assert.AreEqual("DROP TEMPORARY TABLE IF EXISTS tb1, tb2, tb3 CASCADE"
                , output);
            sql = "drop temporary tabLe if exists tb1 cascade";
            lexer = new MySQLLexer(sql);
            parser = new MySQLDDLParser(lexer, new MySQLExprParser(lexer));
            dst = parser.DdlStmt();
            output = Output2MySQL(dst, sql);
            NUnit.Framework.Assert.AreEqual("DROP TEMPORARY TABLE IF EXISTS tb1 CASCADE", output
                );
            sql = "drop tabLe if exists tb1 cascade";
            lexer = new MySQLLexer(sql);
            parser = new MySQLDDLParser(lexer, new MySQLExprParser(lexer));
            dst = parser.DdlStmt();
            output = Output2MySQL(dst, sql);
            NUnit.Framework.Assert.AreEqual("DROP TABLE IF EXISTS tb1 CASCADE", output);
            sql = "drop temporary tabLe tb1 cascade";
            lexer = new MySQLLexer(sql);
            parser = new MySQLDDLParser(lexer, new MySQLExprParser(lexer));
            dst = parser.DdlStmt();
            output = Output2MySQL(dst, sql);
            NUnit.Framework.Assert.AreEqual("DROP TEMPORARY TABLE tb1 CASCADE", output);
            sql = "rename table tb1 to ntb1,tb2 to ntb2";
            lexer = new MySQLLexer(sql);
            parser = new MySQLDDLParser(lexer, new MySQLExprParser(lexer));
            dst = parser.DdlStmt();
            output = Output2MySQL(dst, sql);
            NUnit.Framework.Assert.AreEqual("RENAME TABLE tb1 TO ntb1, tb2 TO ntb2", output);
            sql = "rename table tb1 to ntb1";
            lexer = new MySQLLexer(sql);
            parser = new MySQLDDLParser(lexer, new MySQLExprParser(lexer));
            dst = parser.DdlStmt();
            output = Output2MySQL(dst, sql);
            NUnit.Framework.Assert.AreEqual("RENAME TABLE tb1 TO ntb1", output);
        }
    }
}