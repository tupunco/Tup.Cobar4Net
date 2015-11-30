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

namespace Tup.Cobar4Net.Parser.Util
{
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    [TestFixture(Category = "PairUtilTest")]
    public class PairUtilTest
    {
        [Test]
        public virtual void SplitIndexTest()
        {
            var src1 = "offer_group[10]";
            var pair1 = PairUtil.SplitIndex(src1, '[', ']');
            Assert.AreEqual("offer_group", pair1.Key);
            Assert.AreEqual(10, pair1.Value);
            var src2 = "offer_group";
            var pair2 = PairUtil.SplitIndex(src2, '[', ']');
            Assert.AreEqual("offer_group", pair2.Key);
            Assert.AreEqual(-1, pair2.Value);
        }

        [Test]
        public virtual void TestSequenceSlicing()
        {
            Assert.AreEqual(new Pair<int, int>(0, 2), PairUtil.SequenceSlicing("2"));
            Assert.AreEqual(new Pair<int, int>(1, 2), PairUtil.SequenceSlicing("1: 2"));
            Assert.AreEqual(new Pair<int, int>(1, 0), PairUtil.SequenceSlicing(" 1 :"));
            Assert.AreEqual(new Pair<int, int>(-1, 0), PairUtil.SequenceSlicing("-1: "));
            Assert.AreEqual(new Pair<int, int>(-1, 0), PairUtil.SequenceSlicing(" -1:0"));
            Assert.AreEqual(new Pair<int, int>(0, 0), PairUtil.SequenceSlicing(" :"));
        }
    }
}