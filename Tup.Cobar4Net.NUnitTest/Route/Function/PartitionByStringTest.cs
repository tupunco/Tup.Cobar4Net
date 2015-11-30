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
using System.Collections.Generic;
using Tup.Cobar4Net.Parser.Ast.Expression;
using Tup.Cobar4Net.Parser.Ast.Expression.Primary;
using Tup.Cobar4Net.Parser.Util;

namespace Tup.Cobar4Net.Route.Function
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    [TestFixture(Category = "PartitionByStringTest")]
    public class PartitionByStringTest
    {
        [Test]
        public virtual void TestPartition()
        {
            PartitionByString sut = new PartitionByString("test   ", (IList<IExpression>)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "-2:";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual((int)Execute(sut, "12"), (int)Execute(sut, "012"));
            Assert.AreEqual((int)Execute(sut, "112"), (int)Execute(sut, "012"));
            Assert.AreEqual((int)Execute(sut, "2"), (int)Execute(sut, "2"));
            sut = new PartitionByString("test   ", (IList<IExpression>)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "-2:-1";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(49, (int)Execute(sut, "012"));
            Assert.AreEqual(49, (int)Execute(sut, "12"));
            Assert.AreEqual(49, (int)Execute(sut, "15"));
            Assert.AreEqual(0, (int)Execute(sut, "2"));
            Assert.AreEqual(56, (int)Execute(sut, "888888"));
            Assert.AreEqual(56, (int)Execute(sut, "89"));
            Assert.AreEqual(56, (int)Execute(sut, "780"));
            sut = new PartitionByString("test   ", (IList<IExpression>)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "1:-1";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(49, (int)Execute(sut, "012"));
            Assert.AreEqual(49, (int)Execute(sut, "219"));
            Assert.AreEqual(0, (int)Execute(sut, "2"));
            Assert.AreEqual(512, (int)Execute(sut, "888888"));
        }

        /// <summary>start == end , except 0:0,</summary>
        [Test]
        public virtual void TestPartitionStartEqEnd()
        {
            // 同号，不越界
            PartitionByString sut = new PartitionByString("test   ", (IList<IExpression>)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "1:1";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            sut = new PartitionByString("test   ", (IList<IExpression>)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "-5:-5";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            // 异号，不越界
            sut = new PartitionByString("test   ", (IList<IExpression>)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "3:-7";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            sut = new PartitionByString("test   ", (IList<IExpression>)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "5:-5";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            // 同号，边界值
            sut = new PartitionByString("test   ", (IList<IExpression>)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "0:0";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(641, (int)Execute(sut, "skkdifisd-"));
            Assert.AreEqual(74, (int)Execute(sut, "sdsdfsafaw"));
            sut = new PartitionByString("test   ", (IList<IExpression>)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "10:10";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            // 异号，边界值
            sut = new PartitionByString("test   ", (IList<IExpression>)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "0:-10";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            sut = new PartitionByString("test   ", (IList<IExpression>)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "-1:9";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            // 同号，越界
            sut = new PartitionByString("test   ", (IList<IExpression>)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "-15:-15";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            sut = new PartitionByString("test   ", (IList<IExpression>)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "15:15";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
        }

        // 异号，越界，不存在
        /// <summary>if end==0, then end = key.length</summary>
        [Test]
        public virtual void TestPartitionStartLtEnd()
        {
            // 同号，不越界
            PartitionByString sut = new PartitionByString("test   ", (IList<IExpression>)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "6:1";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            sut = new PartitionByString("test   ", (IList<IExpression>)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "-5:-8";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            // 异号，不越界
            sut = new PartitionByString("test   ", (IList<IExpression>)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "9:-9";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            sut = new PartitionByString("test   ", (IList<IExpression>)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "-1:2";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            // 同号，边界值， 双边界
            sut = new PartitionByString("test   ", (IList<IExpression>)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "9:0";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(119, (int)Execute(sut, "qiycgsrmkw"));
            Assert.AreEqual(104, (int)Execute(sut, "tbctwicjyh"));
            sut = new PartitionByString("test   ", (IList<IExpression>)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "-1:-10";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            // 同号，边界值， 单边界
            sut = new PartitionByString("test   ", (IList<IExpression>)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "5:0";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(176, (int)Execute(sut, "kducgalemc"));
            Assert.AreEqual(182, (int)Execute(sut, "1icuwixjsn"));
            sut = new PartitionByString("test   ", (IList<IExpression>)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "9:5";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            sut = new PartitionByString("test   ", (IList<IExpression>)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "-7:-10";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            sut = new PartitionByString("test   ", (IList<IExpression>)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "-1:-4";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            // 异号，边界值，双边界
            sut = new PartitionByString("test   ", (IList<IExpression>)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "9:-10";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            sut = new PartitionByString("test   ", (IList<IExpression>)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "-1:0";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(108, (int)Execute(sut, "tcjsyckxhl"));
            Assert.AreEqual(106, (int)Execute(sut, "1uxhklsycj"));
            // 异号，边界值，单边界
            sut = new PartitionByString("test   ", (IList<IExpression>)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "4:-10";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            sut = new PartitionByString("test   ", (IList<IExpression>)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "-6:0";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(631, (int)Execute(sut, "1kckdlxhxw"));
            Assert.AreEqual(864, (int)Execute(sut, "nhyjklouqj"));
            sut = new PartitionByString("test   ", (IList<IExpression>)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "9:-5";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            sut = new PartitionByString("test   ", (IList<IExpression>)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "-1:5";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            // 同号，双越界
            sut = new PartitionByString("test   ", (IList<IExpression>)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "15:11";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            sut = new PartitionByString("test   ", (IList<IExpression>)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "-15:-20";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            // 同号，单越界
            sut = new PartitionByString("test   ", (IList<IExpression>)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "-8:-15";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            sut = new PartitionByString("test   ", (IList<IExpression>)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "15:6";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            // 异号，双越界
            sut = new PartitionByString("test   ", (IList<IExpression>)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "19:-20";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            // 异号，单越界
            sut = new PartitionByString("test   ", (IList<IExpression>)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "15:-8";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            sut = new PartitionByString("test   ", (IList<IExpression>)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "6:-15";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
        }

        [Test]
        public virtual void TestPartitionStartgtEnd()
        {
            string testKey = "abcdefghij";
            // 同号，不越界
            PartitionByString sut = new PartitionByString("test   ", (IList<IExpression>)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "1:6";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(36, (int)Execute(sut, testKey));
            Assert.AreEqual(36, (int)Execute(sut, "a" + Sharpen.Runtime.Substring(testKey, 1, 6) + "abcd"));
            Assert.AreEqual(36, (int)Execute(sut, "b" + Sharpen.Runtime.Substring(testKey, 1, 6) + "sila"));
            Assert.IsTrue((36 != (int)Execute(sut, "c" + Sharpen.Runtime.Substring(testKey, 1, 5) + "sil2")));
            sut = new PartitionByString("test   ", (IList<IExpression>)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "-8:-5";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(36, (int)Execute(sut, testKey));
            Assert.AreEqual(36, (int)Execute(sut, "12" + Sharpen.Runtime.Substring(testKey, 2, 5) + "12345"));
            Assert.AreEqual(36, (int)Execute(sut, "45" + Sharpen.Runtime.Substring(testKey, 2, 5) + "78923"));
            // 异号，不越界
            sut = new PartitionByString("test   ", (IList<IExpression>)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "-9:9";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(260, (int)Execute(sut, "a" + Sharpen.Runtime.Substring(testKey, 1, 9) + "8"));
            Assert.AreEqual(260, (int)Execute(sut, "f" + Sharpen.Runtime.Substring(testKey, 1, 9) + "*"));
            sut = new PartitionByString("test   ", (IList<IExpression>)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "2:-1";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(934, (int)Execute(sut, "ab" + Sharpen.Runtime.Substring(testKey, 2, 9) + "8"));
            Assert.AreEqual(934, (int)Execute(sut, "fj" + Sharpen.Runtime.Substring(testKey, 2, 9) + "*"));
            // 同号，边界值， 双边界
            sut = new PartitionByString("test   ", (IList<IExpression>)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "0:9";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(101, (int)Execute(sut, Sharpen.Runtime.Substring(
                testKey, 0, 9) + "#"));
            Assert.AreEqual(101, (int)Execute(sut, Sharpen.Runtime.Substring(
                testKey, 0, 9) + "*"));
            sut = new PartitionByString("test   ", (IList<IExpression>)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "-10:-1";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(101, (int)Execute(sut, Sharpen.Runtime.Substring(
                testKey, 0, 9) + "#"));
            Assert.AreEqual(101, (int)Execute(sut, Sharpen.Runtime.Substring(
                testKey, 0, 9) + "*"));
            // 同号，边界值， 单边界
            sut = new PartitionByString("test   ", (IList<IExpression>)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "0:5";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(99, (int)Execute(sut, Sharpen.Runtime.Substring(testKey, 0, 5) + "#uiyt"));
            Assert.AreEqual(99, (int)Execute(sut, Sharpen.Runtime.Substring(testKey, 0, 5) + "*rfsj"));
            sut = new PartitionByString("test   ", (IList<IExpression>)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "5:9";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(386, (int)Execute(sut, "#uiyt" + Sharpen.Runtime.Substring(testKey, 5, 9) + "a"));
            Assert.AreEqual(386, (int)Execute(sut, "*rfsj" + Sharpen.Runtime.Substring(testKey, 5, 9) + "%"));
            sut = new PartitionByString("test   ", (IList<IExpression>)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "-10:-7";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(36, (int)Execute(sut, Sharpen.Runtime.Substring(testKey, 0, 5) + "#uiyt45"));
            Assert.AreEqual(36, (int)Execute(sut, Sharpen.Runtime.Substring(testKey, 0, 5) + "*rfsjkm"));
            sut = new PartitionByString("test   ", (IList<IExpression>)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "-4:-1";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(936, (int)Execute(sut, "#uiyt4" + Sharpen.Runtime.Substring(testKey, 5, 9) + "a"));
            Assert.AreEqual(936, (int)Execute(sut, "*rfsj$" + Sharpen.Runtime.Substring(testKey, 5, 9) + "%"));
            // 异号，边界值，双边界
            sut = new PartitionByString("test   ", (IList<IExpression>)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "-10:9";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(101, (int)Execute(sut, Sharpen.Runtime.Substring(
                testKey, 0, 9) + "a"));
            Assert.AreEqual(101, (int)Execute(sut, Sharpen.Runtime.Substring(
                testKey, 0, 9) + "%"));
            sut = new PartitionByString("test   ", (IList<IExpression>)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "0:-1";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(101, (int)Execute(sut, Sharpen.Runtime.Substring(
                testKey, 0, 9) + "a"));
            Assert.AreEqual(101, (int)Execute(sut, Sharpen.Runtime.Substring(
                testKey, 0, 9) + "%"));
            // 异号，边界值，单边界
            sut = new PartitionByString("test   ", (IList<IExpression>)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "-10:4";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(66, (int)Execute(sut, Sharpen.Runtime.Substring(testKey, 0, 4) + "asdebh"));
            Assert.AreEqual(66, (int)Execute(sut, Sharpen.Runtime.Substring(testKey, 0, 4) + "%^&*()"));
            sut = new PartitionByString("test   ", (IList<IExpression>)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "0:-6";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(66, (int)Execute(sut, Sharpen.Runtime.Substring(testKey, 0, 4) + "asdebh"));
            Assert.AreEqual(66, (int)Execute(sut, Sharpen.Runtime.Substring(testKey, 0, 4) + "%^&*()"));
            sut = new PartitionByString("test   ", (IList<IExpression>)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "-5:9";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(386, (int)Execute(sut, "#uiyt" + Sharpen.Runtime.Substring(testKey, 5, 9) + "a"));
            Assert.AreEqual(386, (int)Execute(sut, "*rfsj" + Sharpen.Runtime.Substring(testKey, 5, 9) + "%"));
            sut = new PartitionByString("test   ", (IList<IExpression>)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "5:-1";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(386, (int)Execute(sut, "#uiyt" + Sharpen.Runtime.Substring(testKey, 5, 9) + "a"));
            Assert.AreEqual(386, (int)Execute(sut, "*rfsj" + Sharpen.Runtime.Substring(testKey, 5, 9) + "%"));
            // 同号，双越界
            sut = new PartitionByString("test   ", (IList<IExpression>)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "11:15";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            sut = new PartitionByString("test   ", (IList<IExpression>)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "-20:-15";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            // 同号，单越界
            sut = new PartitionByString("test   ", (IList<IExpression>)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "-15:-8";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(33, (int)Execute(sut, Sharpen.Runtime.Substring(testKey, 0, 2) + "dskfdijc"));
            Assert.AreEqual(33, (int)Execute(sut, Sharpen.Runtime.Substring(testKey, 0, 2) + "cuiejdjj"));
            Assert.AreEqual(129, (int)Execute(sut, "$%cuiejdjj"));
            sut = new PartitionByString("test   ", (IList<IExpression>)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "6:15";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(450, (int)Execute(sut, "#uiyt#" + Sharpen.Runtime.Substring(testKey, 6, 10)));
            Assert.AreEqual(450, (int)Execute(sut, "*rfsj*" + Sharpen.Runtime.Substring(testKey, 6, 10)));
            Assert.AreEqual(345, (int)Execute(sut, "#uiyt#" + "dkug"));
            // 异号，双越界
            sut = new PartitionByString("test   ", (IList<IExpression>)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "-20:19";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(165, (int)Execute(sut, testKey));
            Assert.AreEqual(725, (int)Execute(sut, "1" + Sharpen.Runtime.Substring(testKey, 1, 10)));
            // 异号，单越界
            sut = new PartitionByString("test   ", (IList<IExpression>)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "-8:15";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(388, (int)Execute(sut, "1q" + Sharpen.Runtime.Substring(testKey, 2, 10)));
            Assert.AreEqual(388, (int)Execute(sut, "sd" + Sharpen.Runtime.Substring(testKey, 2, 10)));
            sut = new PartitionByString("test   ", (IList<IExpression>)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "-15:6";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(99, (int)Execute(sut, Sharpen.Runtime.Substring(testKey, 0, 6) + "abcd"));
            Assert.AreEqual(99, (int)Execute(sut, Sharpen.Runtime.Substring(testKey, 0, 6) + "efgh"));
        }

        [Test]
        public virtual void TestPartitionNoStartOrNoEnd()
        {
            string testKey = "abcdefghij";
            // 无start， 不越界
            PartitionByString sut = new PartitionByString("test   ", (IList<IExpression>)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = ":6";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(99, (int)Execute(sut, testKey));
            Assert.AreEqual(99, (int)Execute(sut, Sharpen.Runtime.Substring(testKey, 0, 6) + "abcd"));
            Assert.AreEqual(99, (int)Execute(sut, Sharpen.Runtime.Substring(testKey, 0, 6) + "sila"));
            Assert.IsTrue((99 != (int)Execute(sut, "c" + Sharpen.Runtime.Substring(testKey, 1, 5) + "sil2")));
            sut = new PartitionByString("test   ", (IList<IExpression>)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = ":-4";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(99, (int)Execute(sut, testKey));
            Assert.AreEqual(99, (int)Execute(sut, Sharpen.Runtime.Substring(testKey, 0, 6) + "abcd"));
            Assert.AreEqual(99, (int)Execute(sut, Sharpen.Runtime.Substring(testKey, 0, 6) + "sila"));
            Assert.IsTrue((99 != (int)Execute(sut, "c" + Sharpen.Runtime.Substring(testKey, 1, 5) + "sil2")));
            // 无start， 越界
            sut = new PartitionByString("test   ", (IList<IExpression>)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = ":15";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(165, (int)Execute(sut, testKey));
            Assert.AreEqual(647, (int)Execute(sut, "b" + testKey));
            sut = new PartitionByString("test   ", (IList<IExpression>)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = ":-15";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            // 无end， 不越界
            sut = new PartitionByString("test   ", (IList<IExpression>)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "2:";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(388, (int)Execute(sut, testKey));
            Assert.AreEqual(388, (int)Execute(sut, "ab" + Sharpen.Runtime.Substring(testKey, 2, 10)));
            Assert.AreEqual(388, (int)Execute(sut, "e&" + Sharpen.Runtime.Substring(testKey, 2, 10)));
            Assert.IsTrue((388 != (int)Execute(sut, "c" + Sharpen.Runtime.Substring(testKey, 1, 5) + "sil2")));
            sut = new PartitionByString("test   ", (IList<IExpression>)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "-5:";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(808, (int)Execute(sut, testKey));
            Assert.AreEqual(808, (int)Execute(sut, "abT*1" + Sharpen.Runtime.Substring(testKey, 5, 10)));
            Assert.AreEqual(808, (int)Execute(sut, "ab^^!" + Sharpen.Runtime.Substring(testKey, 5, 10)));
            // 无end， 越界
            sut = new PartitionByString("test   ", (IList<IExpression>)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "-15:";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(165, (int)Execute(sut, testKey));
            Assert.AreEqual(647, (int)Execute(sut, "b" + testKey));
            sut = new PartitionByString("test   ", (IList<IExpression>)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = "15:";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            // 无start 无end
            sut = new PartitionByString("test   ", (IList<IExpression>)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(false)));
            sut.SetCacheEvalRst(false);
            sut.HashSlice = ":";
            sut.PartitionCount = "1024";
            sut.PartitionLength = "1";
            sut.Init();
            Assert.AreEqual(165, (int)Execute(sut, testKey));
            Assert.AreEqual(452, (int)Execute(sut, "b" + Sharpen.Runtime.Substring(testKey, 1)));
        }

        private static int Execute(PartitionByString sut, string key)
        {
            var map = new Dictionary<object, object>(1);
            map["MEMBER_ID"] = key;
            int v = (int)new Number(sut.Evaluation(map));
            return v;
        }

        private class UUID
        {
            internal static Guid RandomUUID()
            {
                return Guid.NewGuid();
            }
        }
    }
}
