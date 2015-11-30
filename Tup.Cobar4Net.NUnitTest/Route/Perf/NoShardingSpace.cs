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

using System;
using Sharpen;
using Tup.Cobar4Net.Config.Model;

namespace Tup.Cobar4Net.Route.Perf
{
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public class NoShardingSpace
    {
        private readonly SchemaConfig schema = null;

        // CobarConfig conf = CobarServer.getInstance().getConfig();
        // schema = conf.getSchemas().get("dubbo");
        /// <exception cref="System.Data.Sql.SQLNonTransientException" />
        public virtual void TestDefaultSpace()
        {
            var schema = this.schema;
            var stmt = "insert into offer (member_id, gmt_create) values ('1','2001-09-13 20:20:33')";
            for (var i = 0; i < 1000000; i++)
            {
                ServerRouter.Route(schema, stmt, null, null);
            }
        }

        /// <exception cref="System.Data.Sql.SQLNonTransientException" />
        public static void Main(string[] args)
        {
            var test = new NoShardingSpace();
            Runtime.CurrentTimeMillis();
            var start = Runtime.CurrentTimeMillis();
            test.TestDefaultSpace();
            var end = Runtime.CurrentTimeMillis();
            Console.Out.WriteLine("take " + (end - start) + " ms.");
        }
    }
}