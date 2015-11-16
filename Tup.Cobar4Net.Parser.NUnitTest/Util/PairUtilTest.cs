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

namespace Tup.Cobar4Net.Parser.Util
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    [NUnit.Framework.TestFixture(Category = "PairUtilTest")]
    public class PairUtilTest
    {
        [NUnit.Framework.Test]
        public virtual void TestSequenceSlicing()
        {
            NUnit.Framework.Assert.AreEqual(new Pair<int, int>(0, 2), PairUtil.SequenceSlicing
                ("2"));
            NUnit.Framework.Assert.AreEqual(new Pair<int, int>(1, 2), PairUtil.SequenceSlicing
                ("1: 2"));
            NUnit.Framework.Assert.AreEqual(new Pair<int, int>(1, 0), PairUtil.SequenceSlicing
                (" 1 :"));
            NUnit.Framework.Assert.AreEqual(new Pair<int, int>(-1, 0), PairUtil.SequenceSlicing
                ("-1: "));
            NUnit.Framework.Assert.AreEqual(new Pair<int, int>(-1, 0), PairUtil.SequenceSlicing
                (" -1:0"));
            NUnit.Framework.Assert.AreEqual(new Pair<int, int>(0, 0), PairUtil.SequenceSlicing
                (" :"));
        }

        [NUnit.Framework.Test]
        public virtual void SplitIndexTest()
        {
            string src1 = "offer_group[10]";
            Pair<string, int> pair1 = PairUtil.SplitIndex(src1, '[', ']');
            NUnit.Framework.Assert.AreEqual("offer_group", pair1.GetKey());
            NUnit.Framework.Assert.AreEqual(Sharpen.Extensions.ValueOf(10), pair1.GetValue());
            string src2 = "offer_group";
            Pair<string, int> pair2 = PairUtil.SplitIndex(src2, '[', ']');
            NUnit.Framework.Assert.AreEqual("offer_group", pair2.GetKey());
            NUnit.Framework.Assert.AreEqual(Sharpen.Extensions.ValueOf(-1), pair2.GetValue());
        }
    }
}