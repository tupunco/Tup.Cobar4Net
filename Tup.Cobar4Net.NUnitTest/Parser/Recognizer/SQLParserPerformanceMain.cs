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

namespace Tup.Cobar4Net.Parser.Recognizer
{
    /// <author>xianmao.hexm</author>
    [TestFixture(Category = "SqlParserPerformanceMain")]
    public class SqlParserPerformanceMain
    {
        //[Test]
        //public virtual void TestMain()
        //{
        //    Main(null);
        //    Assert.IsTrue(true);
        //}

        /// <exception cref="System.SqlSyntaxErrorException" />
        public static void Performance()
        {
            var sql = "select id,member_id,gmt_create from offer where member_id in ('1','22','333','1124','4525')";
            for (var i = 0; i < 1000000; i++)
            {
                SqlParserDelegate.Parse(sql);
            }
        }

        /// <exception cref="System.SqlSyntaxErrorException" />
        public static void Main(string[] args)
        {
            Performance();
        }
    }
}