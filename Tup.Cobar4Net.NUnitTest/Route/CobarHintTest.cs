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
using Tup.Cobar4Net.Parser.Util;
using Tup.Cobar4Net.Route.Hint;

namespace Tup.Cobar4Net.Route
{
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    [TestFixture(Category = "CobarHintTest")]
    public class CobarHintTest
    {
        /// <exception cref="System.Exception" />
        [Test]
        public virtual void TestHint1()
        {
            var sql = "  /*!cobar: $dataNodeId =2.1, $table='offer'*/ select * ";
            var hint = CobarHint.ParserCobarHint(sql, 2);
            Assert.AreEqual(" select * ", hint.OutputSql);
            Assert.AreEqual("OFFER", hint.Table);
            Assert.AreEqual(1, hint.DataNodes.Count);
            Assert.AreEqual(new Pair<int, int>(2, 1), hint.DataNodes[0]);

            sql = "  /*!cobar: $dataNodeId=0.0, $table='offer'*/ select * ";
            hint = CobarHint.ParserCobarHint(sql, 0);
            Assert.AreEqual(" select * ", hint.OutputSql);
            Assert.AreEqual("OFFER", hint.Table);
            Assert.AreEqual(1, hint.DataNodes.Count);
            Assert.AreEqual(new Pair<int, int>(0, 0), hint.DataNodes[0]);

            sql = "  /*!cobar: $dataNodeId=0, $table='offer'*/ select * ";
            hint = CobarHint.ParserCobarHint(sql, 0);
            Assert.AreEqual(" select * ", hint.OutputSql);
            Assert.AreEqual("OFFER", hint.Table);
            Assert.AreEqual(1, hint.DataNodes.Count);
            //INFO Assert.AreEqual(new Pair<int, int>(0, null), hint.GetDataNodes()[0]);
            Assert.AreEqual(new Pair<int, int>(0, -1), hint.DataNodes[0]);

            sql = "/*!cobar: $dataNodeId   = [ 1,2,5.2]  , $table =  'offer'   */ select * ";
            hint = CobarHint.ParserCobarHint(sql, 0);
            Assert.AreEqual(" select * ", hint.OutputSql);
            Assert.AreEqual("OFFER", hint.Table);
            Assert.AreEqual(3, hint.DataNodes.Count);

            sql = "/*!cobar: $partitionOperand=( 'member_id' = 'm1'), $table='offer'*/ select * ";
            hint = CobarHint.ParserCobarHint(sql, 0);
            Assert.AreEqual(" select * ", hint.OutputSql);
            Assert.AreEqual("OFFER", hint.Table);
            var pair = hint.PartitionOperand;
            Assert.AreEqual(1, pair.Key.Length);
            Assert.AreEqual("MEMBER_ID", pair.Key[0]);
            Assert.AreEqual(1, pair.Value.Length);
            Assert.AreEqual(1, pair.Value[0].Length);
            Assert.AreEqual("m1", pair.Value[0][0]);
            Assert.IsNull(hint.DataNodes);

            sql = "/*!cobar:$partitionOperand =   ( 'member_id' = ['m1'  ,   'm2' ]  ), $table='offer'  , $replica=  2*/ select * ";
            hint = CobarHint.ParserCobarHint(sql, 0);
            Assert.AreEqual(" select * ", hint.OutputSql);
            Assert.AreEqual("OFFER", hint.Table);
            Assert.AreEqual(2, hint.Replica);
            pair = hint.PartitionOperand;
            Assert.AreEqual(1, pair.Key.Length);
            Assert.AreEqual("MEMBER_ID", pair.Key[0]);
            Assert.AreEqual(2, pair.Value.Length);
            Assert.AreEqual(1, pair.Value[0].Length);
            Assert.AreEqual("m1", pair.Value[0][0]);
            Assert.AreEqual("m2", pair.Value[1][0]);
            Assert.IsNull(hint.DataNodes);

            sql = "/*!cobar:$partitionOperand=('member_id'=['m1', 'm2']),$table='offer',$replica=2*/ select * ";
            hint = CobarHint.ParserCobarHint(sql, 0);
            Assert.AreEqual(" select * ", hint.OutputSql);
            Assert.AreEqual("OFFER", hint.Table);
            Assert.AreEqual(2, hint.Replica);
            pair = hint.PartitionOperand;
            Assert.AreEqual(1, pair.Key.Length);
            Assert.AreEqual("MEMBER_ID", pair.Key[0]);
            Assert.AreEqual(2, pair.Value.Length);
            Assert.AreEqual(1, pair.Value[0].Length);
            Assert.AreEqual("m1", pair.Value[0][0]);
            Assert.AreEqual("m2", pair.Value[1][0]);
            Assert.IsNull(hint.DataNodes);

            sql = "/*!cobar:$partitionOperand = ( ['offer_id','group_id'] = [123,'3c']), $table='offer'*/ select * ";
            hint = CobarHint.ParserCobarHint(sql, 0);
            Assert.AreEqual(" select * ", hint.OutputSql);
            Assert.AreEqual("OFFER", hint.Table);
            Assert.AreEqual(RouteResultsetNode.DefaultReplicaIndex, hint.Replica);
            pair = hint.PartitionOperand;
            Assert.AreEqual(2, pair.Key.Length);
            Assert.AreEqual("OFFER_ID", pair.Key[0]);
            Assert.AreEqual("GROUP_ID", pair.Key[1]);
            Assert.AreEqual(1, pair.Value.Length);
            Assert.AreEqual(2, pair.Value[0].Length);
            Assert.AreEqual(123L, pair.Value[0][0]);
            Assert.AreEqual("3c", pair.Value[0][1]);
            Assert.IsNull(hint.DataNodes);

            sql = "/*!cobar:$partitionOperand=(['offer_id' , 'group_iD' ]=[ 123 , '3c' ]) ,$table = 'offer'*/ select * ";
            hint = CobarHint.ParserCobarHint(sql, 0);
            Assert.AreEqual(" select * ", hint.OutputSql);
            Assert.AreEqual("OFFER", hint.Table);
            Assert.AreEqual(RouteResultsetNode.DefaultReplicaIndex, hint.Replica);
            pair = hint.PartitionOperand;
            Assert.AreEqual(2, pair.Key.Length);
            Assert.AreEqual("OFFER_ID", pair.Key[0]);
            Assert.AreEqual("GROUP_ID", pair.Key[1]);
            Assert.AreEqual(1, pair.Value.Length);
            Assert.AreEqual(2, pair.Value[0].Length);
            Assert.AreEqual(123L, pair.Value[0][0]);
            Assert.AreEqual("3c", pair.Value[0][1]);
            Assert.IsNull(hint.DataNodes);

            sql = "/*!cobar:$partitionOperand=(['offer_id','group_id']=[123,'3c']),$table='offer'*/ select * ";
            hint = CobarHint.ParserCobarHint(sql, 0);
            Assert.AreEqual(" select * ", hint.OutputSql);
            Assert.AreEqual("OFFER", hint.Table);
            Assert.AreEqual(RouteResultsetNode.DefaultReplicaIndex, hint.Replica);
            pair = hint.PartitionOperand;
            Assert.AreEqual(2, pair.Key.Length);
            Assert.AreEqual("OFFER_ID", pair.Key[0]);
            Assert.AreEqual("GROUP_ID", pair.Key[1]);
            Assert.AreEqual(1, pair.Value.Length);
            Assert.AreEqual(2, pair.Value[0].Length);
            Assert.AreEqual(123L, pair.Value[0][0]);
            Assert.AreEqual("3c", pair.Value[0][1]);
            Assert.IsNull(hint.DataNodes);

            sql = "/*!cobar:$partitionOperand=(['offer_id','group_id']=[[123,'3c'],[234,'food']]), $table='offer'*/ select * ";
            hint = CobarHint.ParserCobarHint(sql, 0);
            Assert.AreEqual(" select * ", hint.OutputSql);
            Assert.AreEqual("OFFER", hint.Table);
            Assert.AreEqual(RouteResultsetNode.DefaultReplicaIndex, hint.Replica);
            pair = hint.PartitionOperand;
            Assert.AreEqual(2, pair.Key.Length);
            Assert.AreEqual("OFFER_ID", pair.Key[0]);
            Assert.AreEqual("GROUP_ID", pair.Key[1]);
            Assert.AreEqual(2, pair.Value.Length);
            Assert.AreEqual(2, pair.Value[0].Length);
            Assert.AreEqual(2, pair.Value[1].Length);
            Assert.AreEqual(123L, pair.Value[0][0]);
            Assert.AreEqual("3c", pair.Value[0][1]);
            Assert.AreEqual(234L, pair.Value[1][0]);
            Assert.AreEqual("food", pair.Value[1][1]);
            Assert.IsNull(hint.DataNodes);

            sql = "/*!cobar:$partitionOperand= ( [ 'ofFER_id','groUp_id' ]= [ [123,'3c'],[ 234,'food']]  ), $table='offer'*/select * ";
            hint = CobarHint.ParserCobarHint(sql, 0);
            Assert.AreEqual("select * ", hint.OutputSql);
            Assert.AreEqual("OFFER", hint.Table);
            Assert.AreEqual(RouteResultsetNode.DefaultReplicaIndex, hint.Replica);
            pair = hint.PartitionOperand;
            Assert.AreEqual(2, pair.Key.Length);
            Assert.AreEqual("OFFER_ID", pair.Key[0]);
            Assert.AreEqual("GROUP_ID", pair.Key[1]);
            Assert.AreEqual(2, pair.Value.Length);
            Assert.AreEqual(2, pair.Value[0].Length);
            Assert.AreEqual(2, pair.Value[1].Length);
            Assert.AreEqual(123L, pair.Value[0][0]);
            Assert.AreEqual("3c", pair.Value[0][1]);
            Assert.AreEqual(234L, pair.Value[1][0]);
            Assert.AreEqual("food", pair.Value[1][1]);
            Assert.IsNull(hint.DataNodes);

            sql = "/*!cobar:$partitionOperand=(['offer_id']=[123,234]), $table='offer'*/ select * ";
            hint = CobarHint.ParserCobarHint(sql, 0);
            Assert.AreEqual(" select * ", hint.OutputSql);
            Assert.AreEqual("OFFER", hint.Table);
            Assert.AreEqual(RouteResultsetNode.DefaultReplicaIndex, hint.Replica);
            pair = hint.PartitionOperand;
            Assert.AreEqual(1, pair.Key.Length);
            Assert.AreEqual("OFFER_ID", pair.Key[0]);
            Assert.AreEqual(2, pair.Value.Length);
            Assert.AreEqual(1, pair.Value[0].Length);
            Assert.AreEqual(1, pair.Value[1].Length);
            Assert.AreEqual(123L, pair.Value[0][0]);
            Assert.AreEqual(234L, pair.Value[1][0]);
            Assert.IsNull(hint.DataNodes);
        }
    }
}