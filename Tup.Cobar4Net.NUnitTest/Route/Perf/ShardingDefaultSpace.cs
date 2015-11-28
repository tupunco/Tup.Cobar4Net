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

using Sharpen;
using Tup.Cobar4Net.Config.Model;

namespace Tup.Cobar4Net.Route.Perf
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class ShardingDefaultSpace
    {
        private SchemaConfig schema;

        /// <exception cref="System.Exception"/>
        public ShardingDefaultSpace()
        {
        }

        // schema =
        // CobarServer.getInstance().getConfig().getSchemas().get("cndb");
        /// <summary>路由到defaultSpace的�?�能测试</summary>
        /// <exception cref="System.Data.Sql.SQLNonTransientException"/>
        public virtual void TestDefaultSpace()
        {
            SchemaConfig schema = this.GetSchema();
            string sql = "insert into xoffer (member_id, gmt_create) values ('1','2001-09-13 20:20:33')";
            for (int i = 0; i < 1000000; i++)
            {
                ServerRouter.Route(schema, sql, null, null);
            }
        }

        protected internal virtual SchemaConfig GetSchema()
        {
            return schema;
        }

        /// <exception cref="System.Exception"/>
        public static void Main(string[] args)
        {
            Tup.Cobar4Net.Route.Perf.ShardingDefaultSpace test = new Tup.Cobar4Net.Route.Perf.ShardingDefaultSpace
                ();
            Runtime.CurrentTimeMillis();
            long start = Runtime.CurrentTimeMillis();
            test.TestDefaultSpace();
            long end = Runtime.CurrentTimeMillis();
            System.Console.Out.WriteLine("take " + (end - start) + " ms.");
        }
    }
}
