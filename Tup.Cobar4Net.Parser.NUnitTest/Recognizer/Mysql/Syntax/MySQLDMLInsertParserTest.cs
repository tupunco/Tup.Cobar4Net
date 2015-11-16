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

using Tup.Cobar4Net.Parser.Ast.Stmt.Dml;
using Tup.Cobar4Net.Parser.Recognizer.Mysql.Lexer;

namespace Tup.Cobar4Net.Parser.Recognizer.Mysql.Syntax
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    [NUnit.Framework.TestFixture(Category = "MySQLDMLInsertParserTest")]
    public class MySQLDMLInsertParserTest : AbstractSyntaxTest
    {
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        [NUnit.Framework.Test]
        public virtual void TestInsert()
        {
            string sql = "insErt HIGH_PRIORITY intO test.t1 seT t1.id1=?, id2 := '123'";
            MySQLLexer lexer = new MySQLLexer(sql);
            MySQLDMLInsertParser parser = new MySQLDMLInsertParser(lexer, new MySQLExprParser
                (lexer));
            DMLInsertStatement insert = parser.Insert();
            parser.Match(MySQLToken.Eof);
            string output = Output2MySQL(insert, sql);
            NUnit.Framework.Assert.IsNotNull(insert);
            NUnit.Framework.Assert.AreEqual("INSERT HIGH_PRIORITY INTO test.t1 (t1.id1, id2) VALUES (?, '123')"
                , output);
            sql = "insErt  IGNORE test.t1 seT t1.id1:=? oN dupLicatE key UPDATE ex.col1=?, col2=12";
            lexer = new MySQLLexer(sql);
            parser = new MySQLDMLInsertParser(lexer, new MySQLExprParser(lexer));
            insert = parser.Insert();
            parser.Match(MySQLToken.Eof);
            output = Output2MySQL(insert, sql);
            NUnit.Framework.Assert.AreEqual("INSERT IGNORE INTO test.t1 (t1.id1) VALUES (?) ON DUPLICATE KEY UPDATE ex.col1 = ?, col2 = 12"
                , output);
            sql = "insErt t1 value (123,?) oN dupLicatE key UPDATE ex.col1=?";
            lexer = new MySQLLexer(sql);
            parser = new MySQLDMLInsertParser(lexer, new MySQLExprParser(lexer));
            insert = parser.Insert();
            parser.Match(MySQLToken.Eof);
            output = Output2MySQL(insert, sql);
            NUnit.Framework.Assert.AreEqual("INSERT INTO t1 VALUES (123, ?) ON DUPLICATE KEY UPDATE ex.col1 = ?"
                , output);
            sql = "insErt LOW_PRIORITY t1 valueS (12e-2,1,2), (?),(default)";
            lexer = new MySQLLexer(sql);
            parser = new MySQLDMLInsertParser(lexer, new MySQLExprParser(lexer));
            insert = parser.Insert();
            parser.Match(MySQLToken.Eof);
            output = Output2MySQL(insert, sql);
            NUnit.Framework.Assert.AreEqual("INSERT LOW_PRIORITY INTO t1 VALUES (0.12, 1, 2), (?), (DEFAULT)"
                , output);
            sql = "insErt LOW_PRIORITY t1 select id from t1";
            lexer = new MySQLLexer(sql);
            parser = new MySQLDMLInsertParser(lexer, new MySQLExprParser(lexer));
            insert = parser.Insert();
            parser.Match(MySQLToken.Eof);
            output = Output2MySQL(insert, sql);
            NUnit.Framework.Assert.AreEqual("INSERT LOW_PRIORITY INTO t1 SELECT id FROM t1",
                output);
            sql = "insErt delayed t1 select id from t1";
            lexer = new MySQLLexer(sql);
            parser = new MySQLDMLInsertParser(lexer, new MySQLExprParser(lexer));
            insert = parser.Insert();
            parser.Match(MySQLToken.Eof);
            output = Output2MySQL(insert, sql);
            NUnit.Framework.Assert.AreEqual("INSERT DELAYED INTO t1 SELECT id FROM t1", output
                );
            sql = "insErt LOW_PRIORITY t1 (select id from t1) oN dupLicatE key UPDATE ex.col1=?, col2=12";
            lexer = new MySQLLexer(sql);
            parser = new MySQLDMLInsertParser(lexer, new MySQLExprParser(lexer));
            insert = parser.Insert();
            parser.Match(MySQLToken.Eof);
            output = Output2MySQL(insert, sql);
            NUnit.Framework.Assert.AreEqual("INSERT LOW_PRIORITY INTO t1 SELECT id FROM t1 ON DUPLICATE KEY UPDATE ex.col1 = ?, col2 = 12"
                , output);
            sql = "insErt LOW_PRIORITY t1 (t1.col1) valueS (123),('12''34')";
            lexer = new MySQLLexer(sql);
            parser = new MySQLDMLInsertParser(lexer, new MySQLExprParser(lexer));
            insert = parser.Insert();
            parser.Match(MySQLToken.Eof);
            output = Output2MySQL(insert, sql);
            NUnit.Framework.Assert.AreEqual("INSERT LOW_PRIORITY INTO t1 (t1.col1) VALUES (123), ('12\\'34')"
                , output);
            sql = "insErt LOW_PRIORITY t1 (col1, t1.col2) VALUE (123,'123\\'4') oN dupLicatE key UPDATE ex.col1=?";
            lexer = new MySQLLexer(sql);
            parser = new MySQLDMLInsertParser(lexer, new MySQLExprParser(lexer));
            insert = parser.Insert();
            parser.Match(MySQLToken.Eof);
            output = Output2MySQL(insert, sql);
            NUnit.Framework.Assert.AreEqual("INSERT LOW_PRIORITY INTO t1 (col1, t1.col2) VALUES (123, '123\\'4') ON DUPLICATE KEY UPDATE ex.col1 = ?"
                , output);
            sql = "insErt LOW_PRIORITY t1 (col1, t1.col2) select id from t3 oN dupLicatE key UPDATE ex.col1=?";
            lexer = new MySQLLexer(sql);
            parser = new MySQLDMLInsertParser(lexer, new MySQLExprParser(lexer));
            insert = parser.Insert();
            parser.Match(MySQLToken.Eof);
            output = Output2MySQL(insert, sql);
            NUnit.Framework.Assert.AreEqual("INSERT LOW_PRIORITY INTO t1 (col1, t1.col2) SELECT id FROM t3 ON DUPLICATE KEY UPDATE ex.col1 = ?"
                , output);
            sql = "insErt LOW_PRIORITY IGNORE intO t1 (col1) ( select id from t3) ";
            lexer = new MySQLLexer(sql);
            parser = new MySQLDMLInsertParser(lexer, new MySQLExprParser(lexer));
            insert = parser.Insert();
            parser.Match(MySQLToken.Eof);
            output = Output2MySQL(insert, sql);
            NUnit.Framework.Assert.AreEqual("INSERT LOW_PRIORITY IGNORE INTO t1 (col1) SELECT id FROM t3"
                , output);
        }
    }
}