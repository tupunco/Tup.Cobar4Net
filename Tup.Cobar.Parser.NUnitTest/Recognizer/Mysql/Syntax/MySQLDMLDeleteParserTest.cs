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
    [NUnit.Framework.TestFixture(Category = "MySQLDMLDeleteParserTest")]
    public class MySQLDMLDeleteParserTest : AbstractSyntaxTest
    {
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        [NUnit.Framework.Test]
        public virtual void TestDelete1()
        {
            string sql = "deLetE LOW_PRIORITY from id1.id , id using t1 a where col1 =? ";
            MySQLLexer lexer = new MySQLLexer(sql);
            MySQLDMLDeleteParser parser = new MySQLDMLDeleteParser(lexer, new MySQLExprParser
                (lexer));
            DMLDeleteStatement delete = parser.Delete();
            parser.Match(MySQLToken.Eof);
            string output = Output2MySQL(delete, sql);
            NUnit.Framework.Assert.AreEqual("DELETE LOW_PRIORITY id1.id, id FROM t1 AS A WHERE col1 = ?"
                , output);
            sql = "deLetE from id1.id  using t1  ";
            lexer = new MySQLLexer(sql);
            parser = new MySQLDMLDeleteParser(lexer, new MySQLExprParser(lexer));
            delete = parser.Delete();
            parser.Match(MySQLToken.Eof);
            output = Output2MySQL(delete, sql);
            NUnit.Framework.Assert.AreEqual("DELETE id1.id FROM t1", output);
            sql = "delete from offer.*,wp_image.* using offer a,wp_image b where a.member_id=b.member_id and a.member_id='abc' ";
            lexer = new MySQLLexer(sql);
            parser = new MySQLDMLDeleteParser(lexer, new MySQLExprParser(lexer));
            delete = parser.Delete();
            parser.Match(MySQLToken.Eof);
            output = Output2MySQL(delete, sql);
            NUnit.Framework.Assert.AreEqual("DELETE offer.*, wp_image.* FROM offer AS A, wp_image AS B WHERE "
                 + "a.member_id = b.member_id AND a.member_id = 'abc'", output);
            sql = "deLetE from id1.id where col1='adf' limit 1,?";
            lexer = new MySQLLexer(sql);
            parser = new MySQLDMLDeleteParser(lexer, new MySQLExprParser(lexer));
            delete = parser.Delete();
            parser.Match(MySQLToken.Eof);
            output = Output2MySQL(delete, sql);
            NUnit.Framework.Assert.AreEqual("DELETE FROM id1.id WHERE col1 = 'adf' LIMIT 1, ?"
                , output);
            sql = "deLetE from id where col1='adf' ordEr by d liMit ? offset 2";
            lexer = new MySQLLexer(sql);
            parser = new MySQLDMLDeleteParser(lexer, new MySQLExprParser(lexer));
            delete = parser.Delete();
            parser.Match(MySQLToken.Eof);
            output = Output2MySQL(delete, sql);
            NUnit.Framework.Assert.AreEqual("DELETE FROM id WHERE col1 = 'adf' ORDER BY d LIMIT 2, ?"
                , output);
            sql = "deLetE id.* from t1,t2 where col1='adf'            and col2=1";
            lexer = new MySQLLexer(sql);
            parser = new MySQLDMLDeleteParser(lexer, new MySQLExprParser(lexer));
            delete = parser.Delete();
            parser.Match(MySQLToken.Eof);
            output = Output2MySQL(delete, sql);
            NUnit.Framework.Assert.AreEqual("DELETE id.* FROM t1, t2 WHERE col1 = 'adf' AND col2 = 1"
                , output);
            sql = "deLetE id,id.t from t1";
            lexer = new MySQLLexer(sql);
            parser = new MySQLDMLDeleteParser(lexer, new MySQLExprParser(lexer));
            delete = parser.Delete();
            parser.Match(MySQLToken.Eof);
            output = Output2MySQL(delete, sql);
            NUnit.Framework.Assert.AreEqual("DELETE id, id.t FROM t1", output);
            sql = "deLetE from t1 where t1.id1='abc' order by a limit 5";
            lexer = new MySQLLexer(sql);
            parser = new MySQLDMLDeleteParser(lexer, new MySQLExprParser(lexer));
            delete = parser.Delete();
            parser.Match(MySQLToken.Eof);
            output = Output2MySQL(delete, sql);
            NUnit.Framework.Assert.AreEqual("DELETE FROM t1 WHERE t1.id1 = 'abc' ORDER BY a LIMIT 0, 5"
                , output);
            sql = "deLetE from t1";
            lexer = new MySQLLexer(sql);
            parser = new MySQLDMLDeleteParser(lexer, new MySQLExprParser(lexer));
            delete = parser.Delete();
            parser.Match(MySQLToken.Eof);
            output = Output2MySQL(delete, sql);
            NUnit.Framework.Assert.AreEqual("DELETE FROM t1", output);
            sql = "deLetE ignore tb1.*,id1.t from t1";
            lexer = new MySQLLexer(sql);
            parser = new MySQLDMLDeleteParser(lexer, new MySQLExprParser(lexer));
            delete = parser.Delete();
            parser.Match(MySQLToken.Eof);
            output = Output2MySQL(delete, sql);
            NUnit.Framework.Assert.AreEqual("DELETE IGNORE tb1.*, id1.t FROM t1", output);
            sql = "deLetE quick tb1.*,id1.t from t1";
            lexer = new MySQLLexer(sql);
            parser = new MySQLDMLDeleteParser(lexer, new MySQLExprParser(lexer));
            delete = parser.Delete();
            parser.Match(MySQLToken.Eof);
            output = Output2MySQL(delete, sql);
            NUnit.Framework.Assert.AreEqual("DELETE QUICK tb1.*, id1.t FROM t1", output);
        }
    }
}