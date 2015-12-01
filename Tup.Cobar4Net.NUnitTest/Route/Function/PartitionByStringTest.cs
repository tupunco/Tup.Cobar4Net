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
using Tup.Cobar4Net.Parser.Ast.Expression.Primary;
using Tup.Cobar4Net.Parser.Util;

namespace Tup.Cobar4Net.Route.Function
{
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    [TestFixture(Category = "PartitionByStringTest")]
    public class PartitionByStringTest
    {
        private static int Execute(PartitionByString sut, string key)
        {
            var map = new Dictionary<object, object>(1);
            map["MEMBER_ID"] = key;
            var v = (int)new Number(sut.Evaluation(map));
            return v;
        }

        private class UUID
        {
            internal static Guid RandomUUID()
            {
                return Guid.NewGuid();
            }
        }

        [Test]
        public virtual void TestPartition()
        {
            var sut = new PartitionByString("test   ",
                ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "-2:";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(Execute(sut, "12"), Execute(sut, "012"));
            Assert.AreEqual(Execute(sut, "112"), Execute(sut, "012"));
            Assert.AreEqual(Execute(sut, "2"), Execute(sut, "2"));
            sut = new PartitionByString("test   ",
                ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "-2:-1";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(49, Execute(sut, "012"));
            Assert.AreEqual(49, Execute(sut, "12"));
            Assert.AreEqual(49, Execute(sut, "15"));
            Assert.AreEqual(0, Execute(sut, "2"));
            Assert.AreEqual(56, Execute(sut, "888888"));
            Assert.AreEqual(56, Execute(sut, "89"));
            Assert.AreEqual(56, Execute(sut, "780"));
            sut = new PartitionByString("test   ",
                ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "1:-1";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(49, Execute(sut, "012"));
            Assert.AreEqual(49, Execute(sut, "219"));
            Assert.AreEqual(0, Execute(sut, "2"));
            Assert.AreEqual(512, Execute(sut, "888888"));
        }

        [Test]
        public virtual void TestPartitionNoStartOrNoEnd()
        {
            var testKey = "abcdefghij";
            // 无start， 不越界
            var sut = new PartitionByString("test   ",
                ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = ":6";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(99, Execute(sut, testKey));
            Assert.AreEqual(99, Execute(sut, Runtime.Substring(testKey, 0, 6) + "abcd"));
            Assert.AreEqual(99, Execute(sut, Runtime.Substring(testKey, 0, 6) + "sila"));
            Assert.IsTrue(99 != Execute(sut, "c" + Runtime.Substring(testKey, 1, 5) + "sil2"));
            sut = new PartitionByString("test   ",
                ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = ":-4";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(99, Execute(sut, testKey));
            Assert.AreEqual(99, Execute(sut, Runtime.Substring(testKey, 0, 6) + "abcd"));
            Assert.AreEqual(99, Execute(sut, Runtime.Substring(testKey, 0, 6) + "sila"));
            Assert.IsTrue(99 != Execute(sut, "c" + Runtime.Substring(testKey, 1, 5) + "sil2"));
            // 无start， 越界
            sut = new PartitionByString("test   ",
                ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = ":15";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(165, Execute(sut, testKey));
            Assert.AreEqual(647, Execute(sut, "b" + testKey));
            sut = new PartitionByString("test   ",
                ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = ":-15";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(0, Execute(sut, Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            Assert.AreEqual(0, Execute(sut, Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            // 无end， 不越界
            sut = new PartitionByString("test   ",
                ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "2:";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(388, Execute(sut, testKey));
            Assert.AreEqual(388, Execute(sut, "ab" + Runtime.Substring(testKey, 2, 10)));
            Assert.AreEqual(388, Execute(sut, "e&" + Runtime.Substring(testKey, 2, 10)));
            Assert.IsTrue(388 != Execute(sut, "c" + Runtime.Substring(testKey, 1, 5) + "sil2"));
            sut = new PartitionByString("test   ",
                ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "-5:";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(808, Execute(sut, testKey));
            Assert.AreEqual(808, Execute(sut, "abT*1" + Runtime.Substring(testKey, 5, 10)));
            Assert.AreEqual(808, Execute(sut, "ab^^!" + Runtime.Substring(testKey, 5, 10)));
            // 无end， 越界
            sut = new PartitionByString("test   ",
                ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "-15:";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(165, Execute(sut, testKey));
            Assert.AreEqual(647, Execute(sut, "b" + testKey));
            sut = new PartitionByString("test   ",
                ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "15:";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(0, Execute(sut, Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            Assert.AreEqual(0, Execute(sut, Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            // 无start 无end
            sut = new PartitionByString("test   ",
                ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = ":";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(165, Execute(sut, testKey));
            Assert.AreEqual(452, Execute(sut, "b" + Runtime.Substring(testKey, 1)));
        }

        /// <summary>start == end , except 0:0,</summary>
        [Test]
        public virtual void TestPartitionStartEqEnd()
        {
            // 同号，不越界
            var sut = new PartitionByString("test   ",
                ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "1:1";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(0, Execute(sut, Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            Assert.AreEqual(0, Execute(sut, Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            sut = new PartitionByString("test   ",
                ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "-5:-5";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(0, Execute(sut, Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            Assert.AreEqual(0, Execute(sut, Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            // 异号，不越界
            sut = new PartitionByString("test   ",
                ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "3:-7";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(0, Execute(sut, Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            Assert.AreEqual(0, Execute(sut, Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            sut = new PartitionByString("test   ",
                ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "5:-5";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(0, Execute(sut, Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            Assert.AreEqual(0, Execute(sut, Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            // 同号，边界值
            sut = new PartitionByString("test   ",
                ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "0:0";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(641, Execute(sut, "skkdifisd-"));
            Assert.AreEqual(74, Execute(sut, "sdsdfsafaw"));
            sut = new PartitionByString("test   ",
                ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "10:10";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(0, Execute(sut, Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            Assert.AreEqual(0, Execute(sut, Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            // 异号，边界值
            sut = new PartitionByString("test   ",
                ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "0:-10";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(0, Execute(sut, Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            Assert.AreEqual(0, Execute(sut, Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            sut = new PartitionByString("test   ",
                ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "-1:9";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(0, Execute(sut, Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            Assert.AreEqual(0, Execute(sut, Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            // 同号，越界
            sut = new PartitionByString("test   ",
                ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "-15:-15";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(0, Execute(sut, Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            Assert.AreEqual(0, Execute(sut, Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            sut = new PartitionByString("test   ",
                ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "15:15";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(0, Execute(sut, Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            Assert.AreEqual(0, Execute(sut, Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
        }

        [Test]
        public virtual void TestPartitionStartgtEnd()
        {
            var testKey = "abcdefghij";
            // 同号，不越界
            var sut = new PartitionByString("test   ",
                ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "1:6";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(36, Execute(sut, testKey));
            Assert.AreEqual(36, Execute(sut, "a" + Runtime.Substring(testKey, 1, 6) + "abcd"));
            Assert.AreEqual(36, Execute(sut, "b" + Runtime.Substring(testKey, 1, 6) + "sila"));
            Assert.IsTrue(36 != Execute(sut, "c" + Runtime.Substring(testKey, 1, 5) + "sil2"));
            sut = new PartitionByString("test   ",
                ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "-8:-5";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(36, Execute(sut, testKey));
            Assert.AreEqual(36, Execute(sut, "12" + Runtime.Substring(testKey, 2, 5) + "12345"));
            Assert.AreEqual(36, Execute(sut, "45" + Runtime.Substring(testKey, 2, 5) + "78923"));
            // 异号，不越界
            sut = new PartitionByString("test   ",
                ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "-9:9";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(260, Execute(sut, "a" + Runtime.Substring(testKey, 1, 9) + "8"));
            Assert.AreEqual(260, Execute(sut, "f" + Runtime.Substring(testKey, 1, 9) + "*"));
            sut = new PartitionByString("test   ",
                ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "2:-1";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(934, Execute(sut, "ab" + Runtime.Substring(testKey, 2, 9) + "8"));
            Assert.AreEqual(934, Execute(sut, "fj" + Runtime.Substring(testKey, 2, 9) + "*"));
            // 同号，边界值， 双边界
            sut = new PartitionByString("test   ",
                ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "0:9";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(101, Execute(sut, Runtime.Substring(
                testKey, 0, 9) + "#"));
            Assert.AreEqual(101, Execute(sut, Runtime.Substring(
                testKey, 0, 9) + "*"));
            sut = new PartitionByString("test   ",
                ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "-10:-1";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(101, Execute(sut, Runtime.Substring(
                testKey, 0, 9) + "#"));
            Assert.AreEqual(101, Execute(sut, Runtime.Substring(
                testKey, 0, 9) + "*"));
            // 同号，边界值， 单边界
            sut = new PartitionByString("test   ",
                ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "0:5";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(99, Execute(sut, Runtime.Substring(testKey, 0, 5) + "#uiyt"));
            Assert.AreEqual(99, Execute(sut, Runtime.Substring(testKey, 0, 5) + "*rfsj"));
            sut = new PartitionByString("test   ",
                ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "5:9";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(386, Execute(sut, "#uiyt" + Runtime.Substring(testKey, 5, 9) + "a"));
            Assert.AreEqual(386, Execute(sut, "*rfsj" + Runtime.Substring(testKey, 5, 9) + "%"));
            sut = new PartitionByString("test   ",
                ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "-10:-7";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(36, Execute(sut, Runtime.Substring(testKey, 0, 5) + "#uiyt45"));
            Assert.AreEqual(36, Execute(sut, Runtime.Substring(testKey, 0, 5) + "*rfsjkm"));
            sut = new PartitionByString("test   ",
                ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "-4:-1";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(936, Execute(sut, "#uiyt4" + Runtime.Substring(testKey, 5, 9) + "a"));
            Assert.AreEqual(936, Execute(sut, "*rfsj$" + Runtime.Substring(testKey, 5, 9) + "%"));
            // 异号，边界值，双边界
            sut = new PartitionByString("test   ",
                ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "-10:9";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(101, Execute(sut, Runtime.Substring(
                testKey, 0, 9) + "a"));
            Assert.AreEqual(101, Execute(sut, Runtime.Substring(
                testKey, 0, 9) + "%"));
            sut = new PartitionByString("test   ",
                ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "0:-1";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(101, Execute(sut, Runtime.Substring(
                testKey, 0, 9) + "a"));
            Assert.AreEqual(101, Execute(sut, Runtime.Substring(
                testKey, 0, 9) + "%"));
            // 异号，边界值，单边界
            sut = new PartitionByString("test   ",
                ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "-10:4";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(66, Execute(sut, Runtime.Substring(testKey, 0, 4) + "asdebh"));
            Assert.AreEqual(66, Execute(sut, Runtime.Substring(testKey, 0, 4) + "%^&*()"));
            sut = new PartitionByString("test   ",
                ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "0:-6";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(66, Execute(sut, Runtime.Substring(testKey, 0, 4) + "asdebh"));
            Assert.AreEqual(66, Execute(sut, Runtime.Substring(testKey, 0, 4) + "%^&*()"));
            sut = new PartitionByString("test   ",
                ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "-5:9";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(386, Execute(sut, "#uiyt" + Runtime.Substring(testKey, 5, 9) + "a"));
            Assert.AreEqual(386, Execute(sut, "*rfsj" + Runtime.Substring(testKey, 5, 9) + "%"));
            sut = new PartitionByString("test   ",
                ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "5:-1";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(386, Execute(sut, "#uiyt" + Runtime.Substring(testKey, 5, 9) + "a"));
            Assert.AreEqual(386, Execute(sut, "*rfsj" + Runtime.Substring(testKey, 5, 9) + "%"));
            // 同号，双越界
            sut = new PartitionByString("test   ",
                ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "11:15";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(0, Execute(sut, Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            Assert.AreEqual(0, Execute(sut, Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            sut = new PartitionByString("test   ",
                ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "-20:-15";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(0, Execute(sut, Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            Assert.AreEqual(0, Execute(sut, Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            // 同号，单越界
            sut = new PartitionByString("test   ",
                ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "-15:-8";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(33, Execute(sut, Runtime.Substring(testKey, 0, 2) + "dskfdijc"));
            Assert.AreEqual(33, Execute(sut, Runtime.Substring(testKey, 0, 2) + "cuiejdjj"));
            Assert.AreEqual(129, Execute(sut, "$%cuiejdjj"));
            sut = new PartitionByString("test   ",
                ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "6:15";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(450, Execute(sut, "#uiyt#" + Runtime.Substring(testKey, 6, 10)));
            Assert.AreEqual(450, Execute(sut, "*rfsj*" + Runtime.Substring(testKey, 6, 10)));
            Assert.AreEqual(345, Execute(sut, "#uiyt#" + "dkug"));
            // 异号，双越界
            sut = new PartitionByString("test   ",
                ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "-20:19";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(165, Execute(sut, testKey));
            Assert.AreEqual(725, Execute(sut, "1" + Runtime.Substring(testKey, 1, 10)));
            // 异号，单越界
            sut = new PartitionByString("test   ",
                ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "-8:15";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(388, Execute(sut, "1q" + Runtime.Substring(testKey, 2, 10)));
            Assert.AreEqual(388, Execute(sut, "sd" + Runtime.Substring(testKey, 2, 10)));
            sut = new PartitionByString("test   ",
                ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "-15:6";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(99, Execute(sut, Runtime.Substring(testKey, 0, 6) + "abcd"));
            Assert.AreEqual(99, Execute(sut, Runtime.Substring(testKey, 0, 6) + "efgh"));
        }

        // 异号，越界，不存在
        /// <summary>if end==0, then end = key.length</summary>
        [Test]
        public virtual void TestPartitionStartLtEnd()
        {
            // 同号，不越界
            var sut = new PartitionByString("test   ",
                ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "6:1";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(0, Execute(sut, Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            Assert.AreEqual(0, Execute(sut, Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            sut = new PartitionByString("test   ",
                ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "-5:-8";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(0, Execute(sut, Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            Assert.AreEqual(0, Execute(sut, Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            // 异号，不越界
            sut = new PartitionByString("test   ",
                ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "9:-9";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(0, Execute(sut, Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            Assert.AreEqual(0, Execute(sut, Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            sut = new PartitionByString("test   ",
                ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "-1:2";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(0, Execute(sut, Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            Assert.AreEqual(0, Execute(sut, Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            // 同号，边界值， 双边界
            sut = new PartitionByString("test   ",
                ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "9:0";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(119, Execute(sut, "qiycgsrmkw"));
            Assert.AreEqual(104, Execute(sut, "tbctwicjyh"));
            sut = new PartitionByString("test   ",
                ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "-1:-10";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(0, Execute(sut, Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            Assert.AreEqual(0, Execute(sut, Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            // 同号，边界值， 单边界
            sut = new PartitionByString("test   ",
                ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "5:0";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(176, Execute(sut, "kducgalemc"));
            Assert.AreEqual(182, Execute(sut, "1icuwixjsn"));
            sut = new PartitionByString("test   ",
                ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "9:5";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(0, Execute(sut, Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            Assert.AreEqual(0, Execute(sut, Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            sut = new PartitionByString("test   ",
                ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "-7:-10";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(0, Execute(sut, Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            Assert.AreEqual(0, Execute(sut, Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            sut = new PartitionByString("test   ",
                ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "-1:-4";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(0, Execute(sut, Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            Assert.AreEqual(0, Execute(sut, Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            // 异号，边界值，双边界
            sut = new PartitionByString("test   ",
                ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "9:-10";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(0, Execute(sut, Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            Assert.AreEqual(0, Execute(sut, Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            sut = new PartitionByString("test   ",
                ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "-1:0";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(108, Execute(sut, "tcjsyckxhl"));
            Assert.AreEqual(106, Execute(sut, "1uxhklsycj"));
            // 异号，边界值，单边界
            sut = new PartitionByString("test   ",
                ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "4:-10";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(0, Execute(sut, Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            Assert.AreEqual(0, Execute(sut, Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            sut = new PartitionByString("test   ",
                ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "-6:0";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(631, Execute(sut, "1kckdlxhxw"));
            Assert.AreEqual(864, Execute(sut, "nhyjklouqj"));
            sut = new PartitionByString("test   ",
                ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "9:-5";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(0, Execute(sut, Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            Assert.AreEqual(0, Execute(sut, Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            sut = new PartitionByString("test   ",
                ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "-1:5";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(0, Execute(sut, Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            Assert.AreEqual(0, Execute(sut, Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            // 同号，双越界
            sut = new PartitionByString("test   ",
                ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "15:11";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(0, Execute(sut, Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            Assert.AreEqual(0, Execute(sut, Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            sut = new PartitionByString("test   ",
                ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "-15:-20";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(0, Execute(sut, Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            Assert.AreEqual(0, Execute(sut, Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            // 同号，单越界
            sut = new PartitionByString("test   ",
                ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "-8:-15";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(0, Execute(sut, Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            Assert.AreEqual(0, Execute(sut, Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            sut = new PartitionByString("test   ",
                ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "15:6";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(0, Execute(sut, Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            Assert.AreEqual(0, Execute(sut, Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            // 异号，双越界
            sut = new PartitionByString("test   ",
                ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "19:-20";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(0, Execute(sut, Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            Assert.AreEqual(0, Execute(sut, Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            // 异号，单越界
            sut = new PartitionByString("test   ",
                ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "15:-8";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(0, Execute(sut, Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            Assert.AreEqual(0, Execute(sut, Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            sut = new PartitionByString("test   ",
                ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "6:-15";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(0, Execute(sut, Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            Assert.AreEqual(0, Execute(sut, Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
        }
    }
}