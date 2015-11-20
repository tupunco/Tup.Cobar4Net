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
using System.Collections.Generic;
using NUnit.Framework;
using Sharpen;

namespace Tup.Cobar4Net.Route
{
    /// <author>xianmao.hexm</author>
    [NUnit.Framework.TestFixture(Category = "RouteResultsetNodeTest")]
    public class RouteResultsetNodeTest
    {
        [NUnit.Framework.Test]
        public virtual void TestMapKeyValue()
        {
            var map = new Dictionary<RouteResultsetNode, string>();
            var rrn = new RouteResultsetNode("test", "select * from t1 limit 1");
            var rrn2 = new RouteResultsetNode("test", 1, "select * from t2 limit 1");
            map[rrn] = rrn.GetStatement();
            map[rrn2] = rrn2.GetStatement();
            NUnit.Framework.Assert.AreEqual(2, map.Count);
            for (int i = 0; i < 100; i++)
            {
                NUnit.Framework.Assert.AreEqual("select * from t1 limit 1", map[rrn]);
                NUnit.Framework.Assert.AreEqual("select * from t2 limit 1", map[rrn2]);
            }
        }
    }
}
