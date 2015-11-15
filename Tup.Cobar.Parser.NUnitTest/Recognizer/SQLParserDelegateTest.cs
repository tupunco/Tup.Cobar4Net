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
using System;
using Tup.Cobar.Parser.Ast.Stmt;
using Tup.Cobar.Parser.Ast.Stmt.Dml;
using Tup.Cobar.Parser.Recognizer.Mysql.Syntax;

namespace Tup.Cobar.Parser.Recognizer
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    [NUnit.Framework.TestFixture(Category = "SQLParserDelegateTest")]
    public class SQLParserDelegateTest : AbstractSyntaxTest
    {
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        [NUnit.Framework.Test]
        public virtual void TestProperlyEnd()
        {
            string sql = "select * from tb1;";
            SQLStatement stmt = SQLParserDelegate.Parse(sql);
            NUnit.Framework.Assert.AreEqual(typeof(DMLSelectStatement), stmt.GetType());
            sql = "select * from tb1 ;;;  ";
            stmt = SQLParserDelegate.Parse(sql);
            NUnit.Framework.Assert.AreEqual(typeof(DMLSelectStatement), stmt.GetType());
            sql = "select * from tb1 /***/  ";
            stmt = SQLParserDelegate.Parse(sql);
            NUnit.Framework.Assert.AreEqual(typeof(DMLSelectStatement), stmt.GetType());
            sql = "select * from tb1 ,  ";
            try
            {
                stmt = SQLParserDelegate.Parse(sql);
                Assert.Fail("should detect inproperly end");
            }
            catch (SQLSyntaxErrorException)
            {
            }
            sql = "select * from tb1 ;,  ";
            try
            {
                stmt = SQLParserDelegate.Parse(sql);
                Assert.Fail("should detect inproperly end");
            }
            catch (SQLSyntaxErrorException)
            {
            }
        }
    }
}