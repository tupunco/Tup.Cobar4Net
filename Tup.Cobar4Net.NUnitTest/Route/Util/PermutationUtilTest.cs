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
using System.Collections.Generic;
using NUnit.Framework;
using Sharpen;

namespace Tup.Cobar4Net.Route.Util
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    [NUnit.Framework.TestFixture(Category = "PermutationUtilTest")]
    public class PermutationUtilTest
    {
        [NUnit.Framework.Test]
        public virtual void TestPermutate()
        {
            ICollection<string> set = PermutationUtil.PermutateSQL("-", "1");
            NUnit.Framework.Assert.AreEqual(1, set.Count);
            NUnit.Framework.Assert.IsTrue(set.Contains("1"));
            set = PermutationUtil.PermutateSQL("-", "1", "1");
            NUnit.Framework.Assert.AreEqual(1, set.Count);
            NUnit.Framework.Assert.IsTrue(set.Contains("1-1"));
            set = PermutationUtil.PermutateSQL("-", "1", "2");
            NUnit.Framework.Assert.AreEqual(2, set.Count);
            NUnit.Framework.Assert.IsTrue(set.Contains("1-2"));
            NUnit.Framework.Assert.IsTrue(set.Contains("2-1"));
            set = PermutationUtil.PermutateSQL("-", "1", "2", "2");
            NUnit.Framework.Assert.AreEqual(3, set.Count);
            NUnit.Framework.Assert.IsTrue(set.Contains("1-2-2"));
            NUnit.Framework.Assert.IsTrue(set.Contains("2-1-2"));
            NUnit.Framework.Assert.IsTrue(set.Contains("2-2-1"));
            set = PermutationUtil.PermutateSQL("-", "1", "2", "3");
            NUnit.Framework.Assert.AreEqual(6, set.Count);
            NUnit.Framework.Assert.IsTrue(set.Contains("1-2-3"));
            NUnit.Framework.Assert.IsTrue(set.Contains("1-3-2"));
            NUnit.Framework.Assert.IsTrue(set.Contains("2-1-3"));
            NUnit.Framework.Assert.IsTrue(set.Contains("2-3-1"));
            NUnit.Framework.Assert.IsTrue(set.Contains("3-2-1"));
            NUnit.Framework.Assert.IsTrue(set.Contains("3-1-2"));
        }

        [NUnit.Framework.Test]
        public virtual void TestPermutateNull()
        {
            try
            {
                PermutationUtil.PermutateSQL("-");
                NUnit.Framework.Assert.IsFalse(true);
            }
            catch (ArgumentException)
            {
                NUnit.Framework.Assert.IsTrue(true);
            }
            catch
            {
                NUnit.Framework.Assert.IsFalse(true);
            }
        }
    }
}
