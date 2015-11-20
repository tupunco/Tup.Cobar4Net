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
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    [NUnit.Framework.TestFixture(Category = "PartitionByStringTest")]
    public class PartitionByStringTest
    {
        [NUnit.Framework.Test]
        public virtual void TestPartition()
        {
            PartitionByString sut = new PartitionByString("test   ", (IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                >)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(
                false)));
            sut.SetCacheEvalRst(false);
            sut.SetHashSlice("-2:");
            sut.SetPartitionCount("1024");
            sut.SetPartitionLength("1");
            sut.Init();
            NUnit.Framework.Assert.AreEqual((int)Execute(sut, "12"), (int)Execute(sut, "012")
                );
            NUnit.Framework.Assert.AreEqual((int)Execute(sut, "112"), (int)Execute(sut, "012"
                ));
            NUnit.Framework.Assert.AreEqual((int)Execute(sut, "2"), (int)Execute(sut, "2"));
            sut = new PartitionByString("test   ", (IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                >)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(
                false)));
            sut.SetCacheEvalRst(false);
            sut.SetHashSlice("-2:-1");
            sut.SetPartitionCount("1024");
            sut.SetPartitionLength("1");
            sut.Init();
            NUnit.Framework.Assert.AreEqual(49, (int)Execute(sut, "012"));
            NUnit.Framework.Assert.AreEqual(49, (int)Execute(sut, "12"));
            NUnit.Framework.Assert.AreEqual(49, (int)Execute(sut, "15"));
            NUnit.Framework.Assert.AreEqual(0, (int)Execute(sut, "2"));
            NUnit.Framework.Assert.AreEqual(56, (int)Execute(sut, "888888"));
            NUnit.Framework.Assert.AreEqual(56, (int)Execute(sut, "89"));
            NUnit.Framework.Assert.AreEqual(56, (int)Execute(sut, "780"));
            sut = new PartitionByString("test   ", (IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                >)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(
                false)));
            sut.SetCacheEvalRst(false);
            sut.SetHashSlice("1:-1");
            sut.SetPartitionCount("1024");
            sut.SetPartitionLength("1");
            sut.Init();
            NUnit.Framework.Assert.AreEqual(49, (int)Execute(sut, "012"));
            NUnit.Framework.Assert.AreEqual(49, (int)Execute(sut, "219"));
            NUnit.Framework.Assert.AreEqual(0, (int)Execute(sut, "2"));
            NUnit.Framework.Assert.AreEqual(512, (int)Execute(sut, "888888"));
        }

        /// <summary>start == end , except 0:0,</summary>
        [NUnit.Framework.Test]
        public virtual void TestPartitionStartEqEnd()
        {
            // 同号，不越界
            PartitionByString sut = new PartitionByString("test   ", (IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                >)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(
                false)));
            sut.SetCacheEvalRst(false);
            sut.SetHashSlice("1:1");
            sut.SetPartitionCount("1024");
            sut.SetPartitionLength("1");
            sut.Init();
            NUnit.Framework.Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            NUnit.Framework.Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            sut = new PartitionByString("test   ", (IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                >)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(
                false)));
            sut.SetCacheEvalRst(false);
            sut.SetHashSlice("-5:-5");
            sut.SetPartitionCount("1024");
            sut.SetPartitionLength("1");
            sut.Init();
            NUnit.Framework.Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            NUnit.Framework.Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            // 异号，不越界
            sut = new PartitionByString("test   ", (IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                >)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(
                false)));
            sut.SetCacheEvalRst(false);
            sut.SetHashSlice("3:-7");
            sut.SetPartitionCount("1024");
            sut.SetPartitionLength("1");
            sut.Init();
            NUnit.Framework.Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            NUnit.Framework.Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            sut = new PartitionByString("test   ", (IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                >)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(
                false)));
            sut.SetCacheEvalRst(false);
            sut.SetHashSlice("5:-5");
            sut.SetPartitionCount("1024");
            sut.SetPartitionLength("1");
            sut.Init();
            NUnit.Framework.Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            NUnit.Framework.Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            // 同号，边界值
            sut = new PartitionByString("test   ", (IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                >)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(
                false)));
            sut.SetCacheEvalRst(false);
            sut.SetHashSlice("0:0");
            sut.SetPartitionCount("1024");
            sut.SetPartitionLength("1");
            sut.Init();
            NUnit.Framework.Assert.AreEqual(641, (int)Execute(sut, "skkdifisd-"));
            NUnit.Framework.Assert.AreEqual(74, (int)Execute(sut, "sdsdfsafaw"));
            sut = new PartitionByString("test   ", (IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                >)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(
                false)));
            sut.SetCacheEvalRst(false);
            sut.SetHashSlice("10:10");
            sut.SetPartitionCount("1024");
            sut.SetPartitionLength("1");
            sut.Init();
            NUnit.Framework.Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            NUnit.Framework.Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            // 异号，边界值
            sut = new PartitionByString("test   ", (IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                >)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(
                false)));
            sut.SetCacheEvalRst(false);
            sut.SetHashSlice("0:-10");
            sut.SetPartitionCount("1024");
            sut.SetPartitionLength("1");
            sut.Init();
            NUnit.Framework.Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            NUnit.Framework.Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            sut = new PartitionByString("test   ", (IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                >)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(
                false)));
            sut.SetCacheEvalRst(false);
            sut.SetHashSlice("-1:9");
            sut.SetPartitionCount("1024");
            sut.SetPartitionLength("1");
            sut.Init();
            NUnit.Framework.Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            NUnit.Framework.Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            // 同号，越界
            sut = new PartitionByString("test   ", (IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                >)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(
                false)));
            sut.SetCacheEvalRst(false);
            sut.SetHashSlice("-15:-15");
            sut.SetPartitionCount("1024");
            sut.SetPartitionLength("1");
            sut.Init();
            NUnit.Framework.Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            NUnit.Framework.Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            sut = new PartitionByString("test   ", (IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                >)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(
                false)));
            sut.SetCacheEvalRst(false);
            sut.SetHashSlice("15:15");
            sut.SetPartitionCount("1024");
            sut.SetPartitionLength("1");
            sut.Init();
            NUnit.Framework.Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            NUnit.Framework.Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
        }

        // 异号，越界，不存在
        /// <summary>if end==0, then end = key.length</summary>
        [NUnit.Framework.Test]
        public virtual void TestPartitionStartLtEnd()
        {
            // 同号，不越界
            PartitionByString sut = new PartitionByString("test   ", (IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                >)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(
                false)));
            sut.SetCacheEvalRst(false);
            sut.SetHashSlice("6:1");
            sut.SetPartitionCount("1024");
            sut.SetPartitionLength("1");
            sut.Init();
            NUnit.Framework.Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            NUnit.Framework.Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            sut = new PartitionByString("test   ", (IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                >)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(
                false)));
            sut.SetCacheEvalRst(false);
            sut.SetHashSlice("-5:-8");
            sut.SetPartitionCount("1024");
            sut.SetPartitionLength("1");
            sut.Init();
            NUnit.Framework.Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            NUnit.Framework.Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            // 异号，不越界
            sut = new PartitionByString("test   ", (IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                >)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(
                false)));
            sut.SetCacheEvalRst(false);
            sut.SetHashSlice("9:-9");
            sut.SetPartitionCount("1024");
            sut.SetPartitionLength("1");
            sut.Init();
            NUnit.Framework.Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            NUnit.Framework.Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            sut = new PartitionByString("test   ", (IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                >)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(
                false)));
            sut.SetCacheEvalRst(false);
            sut.SetHashSlice("-1:2");
            sut.SetPartitionCount("1024");
            sut.SetPartitionLength("1");
            sut.Init();
            NUnit.Framework.Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            NUnit.Framework.Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            // 同号，边界值， 双边界
            sut = new PartitionByString("test   ", (IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                >)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(
                false)));
            sut.SetCacheEvalRst(false);
            sut.SetHashSlice("9:0");
            sut.SetPartitionCount("1024");
            sut.SetPartitionLength("1");
            sut.Init();
            NUnit.Framework.Assert.AreEqual(119, (int)Execute(sut, "qiycgsrmkw"));
            NUnit.Framework.Assert.AreEqual(104, (int)Execute(sut, "tbctwicjyh"));
            sut = new PartitionByString("test   ", (IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                >)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(
                false)));
            sut.SetCacheEvalRst(false);
            sut.SetHashSlice("-1:-10");
            sut.SetPartitionCount("1024");
            sut.SetPartitionLength("1");
            sut.Init();
            NUnit.Framework.Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            NUnit.Framework.Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            // 同号，边界值， 单边界
            sut = new PartitionByString("test   ", (IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                >)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(
                false)));
            sut.SetCacheEvalRst(false);
            sut.SetHashSlice("5:0");
            sut.SetPartitionCount("1024");
            sut.SetPartitionLength("1");
            sut.Init();
            NUnit.Framework.Assert.AreEqual(176, (int)Execute(sut, "kducgalemc"));
            NUnit.Framework.Assert.AreEqual(182, (int)Execute(sut, "1icuwixjsn"));
            sut = new PartitionByString("test   ", (IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                >)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(
                false)));
            sut.SetCacheEvalRst(false);
            sut.SetHashSlice("9:5");
            sut.SetPartitionCount("1024");
            sut.SetPartitionLength("1");
            sut.Init();
            NUnit.Framework.Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            NUnit.Framework.Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            sut = new PartitionByString("test   ", (IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                >)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(
                false)));
            sut.SetCacheEvalRst(false);
            sut.SetHashSlice("-7:-10");
            sut.SetPartitionCount("1024");
            sut.SetPartitionLength("1");
            sut.Init();
            NUnit.Framework.Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            NUnit.Framework.Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            sut = new PartitionByString("test   ", (IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                >)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(
                false)));
            sut.SetCacheEvalRst(false);
            sut.SetHashSlice("-1:-4");
            sut.SetPartitionCount("1024");
            sut.SetPartitionLength("1");
            sut.Init();
            NUnit.Framework.Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            NUnit.Framework.Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            // 异号，边界值，双边界
            sut = new PartitionByString("test   ", (IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                >)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(
                false)));
            sut.SetCacheEvalRst(false);
            sut.SetHashSlice("9:-10");
            sut.SetPartitionCount("1024");
            sut.SetPartitionLength("1");
            sut.Init();
            NUnit.Framework.Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            NUnit.Framework.Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            sut = new PartitionByString("test   ", (IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                >)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(
                false)));
            sut.SetCacheEvalRst(false);
            sut.SetHashSlice("-1:0");
            sut.SetPartitionCount("1024");
            sut.SetPartitionLength("1");
            sut.Init();
            NUnit.Framework.Assert.AreEqual(108, (int)Execute(sut, "tcjsyckxhl"));
            NUnit.Framework.Assert.AreEqual(106, (int)Execute(sut, "1uxhklsycj"));
            // 异号，边界值，单边界
            sut = new PartitionByString("test   ", (IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                >)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(
                false)));
            sut.SetCacheEvalRst(false);
            sut.SetHashSlice("4:-10");
            sut.SetPartitionCount("1024");
            sut.SetPartitionLength("1");
            sut.Init();
            NUnit.Framework.Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            NUnit.Framework.Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            sut = new PartitionByString("test   ", (IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                >)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(
                false)));
            sut.SetCacheEvalRst(false);
            sut.SetHashSlice("-6:0");
            sut.SetPartitionCount("1024");
            sut.SetPartitionLength("1");
            sut.Init();
            NUnit.Framework.Assert.AreEqual(631, (int)Execute(sut, "1kckdlxhxw"));
            NUnit.Framework.Assert.AreEqual(864, (int)Execute(sut, "nhyjklouqj"));
            sut = new PartitionByString("test   ", (IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                >)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(
                false)));
            sut.SetCacheEvalRst(false);
            sut.SetHashSlice("9:-5");
            sut.SetPartitionCount("1024");
            sut.SetPartitionLength("1");
            sut.Init();
            NUnit.Framework.Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            NUnit.Framework.Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            sut = new PartitionByString("test   ", (IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                >)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(
                false)));
            sut.SetCacheEvalRst(false);
            sut.SetHashSlice("-1:5");
            sut.SetPartitionCount("1024");
            sut.SetPartitionLength("1");
            sut.Init();
            NUnit.Framework.Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            NUnit.Framework.Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            // 同号，双越界
            sut = new PartitionByString("test   ", (IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                >)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(
                false)));
            sut.SetCacheEvalRst(false);
            sut.SetHashSlice("15:11");
            sut.SetPartitionCount("1024");
            sut.SetPartitionLength("1");
            sut.Init();
            NUnit.Framework.Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            NUnit.Framework.Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            sut = new PartitionByString("test   ", (IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                >)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(
                false)));
            sut.SetCacheEvalRst(false);
            sut.SetHashSlice("-15:-20");
            sut.SetPartitionCount("1024");
            sut.SetPartitionLength("1");
            sut.Init();
            NUnit.Framework.Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            NUnit.Framework.Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            // 同号，单越界
            sut = new PartitionByString("test   ", (IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                >)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(
                false)));
            sut.SetCacheEvalRst(false);
            sut.SetHashSlice("-8:-15");
            sut.SetPartitionCount("1024");
            sut.SetPartitionLength("1");
            sut.Init();
            NUnit.Framework.Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            NUnit.Framework.Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            sut = new PartitionByString("test   ", (IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                >)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(
                false)));
            sut.SetCacheEvalRst(false);
            sut.SetHashSlice("15:6");
            sut.SetPartitionCount("1024");
            sut.SetPartitionLength("1");
            sut.Init();
            NUnit.Framework.Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            NUnit.Framework.Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            // 异号，双越界
            sut = new PartitionByString("test   ", (IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                >)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(
                false)));
            sut.SetCacheEvalRst(false);
            sut.SetHashSlice("19:-20");
            sut.SetPartitionCount("1024");
            sut.SetPartitionLength("1");
            sut.Init();
            NUnit.Framework.Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            NUnit.Framework.Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            // 异号，单越界
            sut = new PartitionByString("test   ", (IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                >)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(
                false)));
            sut.SetCacheEvalRst(false);
            sut.SetHashSlice("15:-8");
            sut.SetPartitionCount("1024");
            sut.SetPartitionLength("1");
            sut.Init();
            NUnit.Framework.Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            NUnit.Framework.Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            sut = new PartitionByString("test   ", (IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                >)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(
                false)));
            sut.SetCacheEvalRst(false);
            sut.SetHashSlice("6:-15");
            sut.SetPartitionCount("1024");
            sut.SetPartitionLength("1");
            sut.Init();
            NUnit.Framework.Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            NUnit.Framework.Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
        }

        [NUnit.Framework.Test]
        public virtual void TestPartitionStartgtEnd()
        {
            string testKey = "abcdefghij";
            // 同号，不越界
            PartitionByString sut = new PartitionByString("test   ", (IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                >)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(
                false)));
            sut.SetCacheEvalRst(false);
            sut.SetHashSlice("1:6");
            sut.SetPartitionCount("1024");
            sut.SetPartitionLength("1");
            sut.Init();
            NUnit.Framework.Assert.AreEqual(36, (int)Execute(sut, testKey));
            NUnit.Framework.Assert.AreEqual(36, (int)Execute(sut, "a" + Sharpen.Runtime.Substring
                (testKey, 1, 6) + "abcd"));
            NUnit.Framework.Assert.AreEqual(36, (int)Execute(sut, "b" + Sharpen.Runtime.Substring
                (testKey, 1, 6) + "sila"));
            NUnit.Framework.Assert.IsTrue((36 != (int)Execute(sut, "c" + Sharpen.Runtime.Substring
                (testKey, 1, 5) + "sil2")));
            sut = new PartitionByString("test   ", (IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                >)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(
                false)));
            sut.SetCacheEvalRst(false);
            sut.SetHashSlice("-8:-5");
            sut.SetPartitionCount("1024");
            sut.SetPartitionLength("1");
            sut.Init();
            NUnit.Framework.Assert.AreEqual(36, (int)Execute(sut, testKey));
            NUnit.Framework.Assert.AreEqual(36, (int)Execute(sut, "12" + Sharpen.Runtime.Substring
                (testKey, 2, 5) + "12345"));
            NUnit.Framework.Assert.AreEqual(36, (int)Execute(sut, "45" + Sharpen.Runtime.Substring
                (testKey, 2, 5) + "78923"));
            // 异号，不越界
            sut = new PartitionByString("test   ", (IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                >)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(
                false)));
            sut.SetCacheEvalRst(false);
            sut.SetHashSlice("-9:9");
            sut.SetPartitionCount("1024");
            sut.SetPartitionLength("1");
            sut.Init();
            NUnit.Framework.Assert.AreEqual(260, (int)Execute(sut, "a" + Sharpen.Runtime.Substring
                (testKey, 1, 9) + "8"));
            NUnit.Framework.Assert.AreEqual(260, (int)Execute(sut, "f" + Sharpen.Runtime.Substring
                (testKey, 1, 9) + "*"));
            sut = new PartitionByString("test   ", (IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                >)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(
                false)));
            sut.SetCacheEvalRst(false);
            sut.SetHashSlice("2:-1");
            sut.SetPartitionCount("1024");
            sut.SetPartitionLength("1");
            sut.Init();
            NUnit.Framework.Assert.AreEqual(934, (int)Execute(sut, "ab" + Sharpen.Runtime.Substring
                (testKey, 2, 9) + "8"));
            NUnit.Framework.Assert.AreEqual(934, (int)Execute(sut, "fj" + Sharpen.Runtime.Substring
                (testKey, 2, 9) + "*"));
            // 同号，边界值， 双边界
            sut = new PartitionByString("test   ", (IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                >)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(
                false)));
            sut.SetCacheEvalRst(false);
            sut.SetHashSlice("0:9");
            sut.SetPartitionCount("1024");
            sut.SetPartitionLength("1");
            sut.Init();
            NUnit.Framework.Assert.AreEqual(101, (int)Execute(sut, Sharpen.Runtime.Substring(
                testKey, 0, 9) + "#"));
            NUnit.Framework.Assert.AreEqual(101, (int)Execute(sut, Sharpen.Runtime.Substring(
                testKey, 0, 9) + "*"));
            sut = new PartitionByString("test   ", (IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                >)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(
                false)));
            sut.SetCacheEvalRst(false);
            sut.SetHashSlice("-10:-1");
            sut.SetPartitionCount("1024");
            sut.SetPartitionLength("1");
            sut.Init();
            NUnit.Framework.Assert.AreEqual(101, (int)Execute(sut, Sharpen.Runtime.Substring(
                testKey, 0, 9) + "#"));
            NUnit.Framework.Assert.AreEqual(101, (int)Execute(sut, Sharpen.Runtime.Substring(
                testKey, 0, 9) + "*"));
            // 同号，边界值， 单边界
            sut = new PartitionByString("test   ", (IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                >)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(
                false)));
            sut.SetCacheEvalRst(false);
            sut.SetHashSlice("0:5");
            sut.SetPartitionCount("1024");
            sut.SetPartitionLength("1");
            sut.Init();
            NUnit.Framework.Assert.AreEqual(99, (int)Execute(sut, Sharpen.Runtime.Substring(testKey
                , 0, 5) + "#uiyt"));
            NUnit.Framework.Assert.AreEqual(99, (int)Execute(sut, Sharpen.Runtime.Substring(testKey
                , 0, 5) + "*rfsj"));
            sut = new PartitionByString("test   ", (IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                >)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(
                false)));
            sut.SetCacheEvalRst(false);
            sut.SetHashSlice("5:9");
            sut.SetPartitionCount("1024");
            sut.SetPartitionLength("1");
            sut.Init();
            NUnit.Framework.Assert.AreEqual(386, (int)Execute(sut, "#uiyt" + Sharpen.Runtime.Substring
                (testKey, 5, 9) + "a"));
            NUnit.Framework.Assert.AreEqual(386, (int)Execute(sut, "*rfsj" + Sharpen.Runtime.Substring
                (testKey, 5, 9) + "%"));
            sut = new PartitionByString("test   ", (IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                >)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(
                false)));
            sut.SetCacheEvalRst(false);
            sut.SetHashSlice("-10:-7");
            sut.SetPartitionCount("1024");
            sut.SetPartitionLength("1");
            sut.Init();
            NUnit.Framework.Assert.AreEqual(36, (int)Execute(sut, Sharpen.Runtime.Substring(testKey
                , 0, 5) + "#uiyt45"));
            NUnit.Framework.Assert.AreEqual(36, (int)Execute(sut, Sharpen.Runtime.Substring(testKey
                , 0, 5) + "*rfsjkm"));
            sut = new PartitionByString("test   ", (IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                >)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(
                false)));
            sut.SetCacheEvalRst(false);
            sut.SetHashSlice("-4:-1");
            sut.SetPartitionCount("1024");
            sut.SetPartitionLength("1");
            sut.Init();
            NUnit.Framework.Assert.AreEqual(936, (int)Execute(sut, "#uiyt4" + Sharpen.Runtime.Substring
                (testKey, 5, 9) + "a"));
            NUnit.Framework.Assert.AreEqual(936, (int)Execute(sut, "*rfsj$" + Sharpen.Runtime.Substring
                (testKey, 5, 9) + "%"));
            // 异号，边界值，双边界
            sut = new PartitionByString("test   ", (IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                >)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(
                false)));
            sut.SetCacheEvalRst(false);
            sut.SetHashSlice("-10:9");
            sut.SetPartitionCount("1024");
            sut.SetPartitionLength("1");
            sut.Init();
            NUnit.Framework.Assert.AreEqual(101, (int)Execute(sut, Sharpen.Runtime.Substring(
                testKey, 0, 9) + "a"));
            NUnit.Framework.Assert.AreEqual(101, (int)Execute(sut, Sharpen.Runtime.Substring(
                testKey, 0, 9) + "%"));
            sut = new PartitionByString("test   ", (IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                >)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(
                false)));
            sut.SetCacheEvalRst(false);
            sut.SetHashSlice("0:-1");
            sut.SetPartitionCount("1024");
            sut.SetPartitionLength("1");
            sut.Init();
            NUnit.Framework.Assert.AreEqual(101, (int)Execute(sut, Sharpen.Runtime.Substring(
                testKey, 0, 9) + "a"));
            NUnit.Framework.Assert.AreEqual(101, (int)Execute(sut, Sharpen.Runtime.Substring(
                testKey, 0, 9) + "%"));
            // 异号，边界值，单边界
            sut = new PartitionByString("test   ", (IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                >)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(
                false)));
            sut.SetCacheEvalRst(false);
            sut.SetHashSlice("-10:4");
            sut.SetPartitionCount("1024");
            sut.SetPartitionLength("1");
            sut.Init();
            NUnit.Framework.Assert.AreEqual(66, (int)Execute(sut, Sharpen.Runtime.Substring(testKey
                , 0, 4) + "asdebh"));
            NUnit.Framework.Assert.AreEqual(66, (int)Execute(sut, Sharpen.Runtime.Substring(testKey
                , 0, 4) + "%^&*()"));
            sut = new PartitionByString("test   ", (IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                >)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(
                false)));
            sut.SetCacheEvalRst(false);
            sut.SetHashSlice("0:-6");
            sut.SetPartitionCount("1024");
            sut.SetPartitionLength("1");
            sut.Init();
            NUnit.Framework.Assert.AreEqual(66, (int)Execute(sut, Sharpen.Runtime.Substring(testKey
                , 0, 4) + "asdebh"));
            NUnit.Framework.Assert.AreEqual(66, (int)Execute(sut, Sharpen.Runtime.Substring(testKey
                , 0, 4) + "%^&*()"));
            sut = new PartitionByString("test   ", (IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                >)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(
                false)));
            sut.SetCacheEvalRst(false);
            sut.SetHashSlice("-5:9");
            sut.SetPartitionCount("1024");
            sut.SetPartitionLength("1");
            sut.Init();
            NUnit.Framework.Assert.AreEqual(386, (int)Execute(sut, "#uiyt" + Sharpen.Runtime.Substring
                (testKey, 5, 9) + "a"));
            NUnit.Framework.Assert.AreEqual(386, (int)Execute(sut, "*rfsj" + Sharpen.Runtime.Substring
                (testKey, 5, 9) + "%"));
            sut = new PartitionByString("test   ", (IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                >)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(
                false)));
            sut.SetCacheEvalRst(false);
            sut.SetHashSlice("5:-1");
            sut.SetPartitionCount("1024");
            sut.SetPartitionLength("1");
            sut.Init();
            NUnit.Framework.Assert.AreEqual(386, (int)Execute(sut, "#uiyt" + Sharpen.Runtime.Substring
                (testKey, 5, 9) + "a"));
            NUnit.Framework.Assert.AreEqual(386, (int)Execute(sut, "*rfsj" + Sharpen.Runtime.Substring
                (testKey, 5, 9) + "%"));
            // 同号，双越界
            sut = new PartitionByString("test   ", (IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                >)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(
                false)));
            sut.SetCacheEvalRst(false);
            sut.SetHashSlice("11:15");
            sut.SetPartitionCount("1024");
            sut.SetPartitionLength("1");
            sut.Init();
            NUnit.Framework.Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            NUnit.Framework.Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            sut = new PartitionByString("test   ", (IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                >)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(
                false)));
            sut.SetCacheEvalRst(false);
            sut.SetHashSlice("-20:-15");
            sut.SetPartitionCount("1024");
            sut.SetPartitionLength("1");
            sut.Init();
            NUnit.Framework.Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            NUnit.Framework.Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            // 同号，单越界
            sut = new PartitionByString("test   ", (IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                >)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(
                false)));
            sut.SetCacheEvalRst(false);
            sut.SetHashSlice("-15:-8");
            sut.SetPartitionCount("1024");
            sut.SetPartitionLength("1");
            sut.Init();
            NUnit.Framework.Assert.AreEqual(33, (int)Execute(sut, Sharpen.Runtime.Substring(testKey
                , 0, 2) + "dskfdijc"));
            NUnit.Framework.Assert.AreEqual(33, (int)Execute(sut, Sharpen.Runtime.Substring(testKey
                , 0, 2) + "cuiejdjj"));
            NUnit.Framework.Assert.AreEqual(129, (int)Execute(sut, "$%cuiejdjj"));
            sut = new PartitionByString("test   ", (IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                >)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(
                false)));
            sut.SetCacheEvalRst(false);
            sut.SetHashSlice("6:15");
            sut.SetPartitionCount("1024");
            sut.SetPartitionLength("1");
            sut.Init();
            NUnit.Framework.Assert.AreEqual(450, (int)Execute(sut, "#uiyt#" + Sharpen.Runtime.Substring
                (testKey, 6, 10)));
            NUnit.Framework.Assert.AreEqual(450, (int)Execute(sut, "*rfsj*" + Sharpen.Runtime.Substring
                (testKey, 6, 10)));
            NUnit.Framework.Assert.AreEqual(345, (int)Execute(sut, "#uiyt#" + "dkug"));
            // 异号，双越界
            sut = new PartitionByString("test   ", (IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                >)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(
                false)));
            sut.SetCacheEvalRst(false);
            sut.SetHashSlice("-20:19");
            sut.SetPartitionCount("1024");
            sut.SetPartitionLength("1");
            sut.Init();
            NUnit.Framework.Assert.AreEqual(165, (int)Execute(sut, testKey));
            NUnit.Framework.Assert.AreEqual(725, (int)Execute(sut, "1" + Sharpen.Runtime.Substring
                (testKey, 1, 10)));
            // 异号，单越界
            sut = new PartitionByString("test   ", (IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                >)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(
                false)));
            sut.SetCacheEvalRst(false);
            sut.SetHashSlice("-8:15");
            sut.SetPartitionCount("1024");
            sut.SetPartitionLength("1");
            sut.Init();
            NUnit.Framework.Assert.AreEqual(388, (int)Execute(sut, "1q" + Sharpen.Runtime.Substring
                (testKey, 2, 10)));
            NUnit.Framework.Assert.AreEqual(388, (int)Execute(sut, "sd" + Sharpen.Runtime.Substring
                (testKey, 2, 10)));
            sut = new PartitionByString("test   ", (IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                >)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(
                false)));
            sut.SetCacheEvalRst(false);
            sut.SetHashSlice("-15:6");
            sut.SetPartitionCount("1024");
            sut.SetPartitionLength("1");
            sut.Init();
            NUnit.Framework.Assert.AreEqual(99, (int)Execute(sut, Sharpen.Runtime.Substring(testKey
                , 0, 6) + "abcd"));
            NUnit.Framework.Assert.AreEqual(99, (int)Execute(sut, Sharpen.Runtime.Substring(testKey
                , 0, 6) + "efgh"));
        }

        [NUnit.Framework.Test]
        public virtual void TestPartitionNoStartOrNoEnd()
        {
            string testKey = "abcdefghij";
            // 无start， 不越界
            PartitionByString sut = new PartitionByString("test   ", (IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                >)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(
                false)));
            sut.SetCacheEvalRst(false);
            sut.SetHashSlice(":6");
            sut.SetPartitionCount("1024");
            sut.SetPartitionLength("1");
            sut.Init();
            NUnit.Framework.Assert.AreEqual(99, (int)Execute(sut, testKey));
            NUnit.Framework.Assert.AreEqual(99, (int)Execute(sut, Sharpen.Runtime.Substring(testKey
                , 0, 6) + "abcd"));
            NUnit.Framework.Assert.AreEqual(99, (int)Execute(sut, Sharpen.Runtime.Substring(testKey
                , 0, 6) + "sila"));
            NUnit.Framework.Assert.IsTrue((99 != (int)Execute(sut, "c" + Sharpen.Runtime.Substring
                (testKey, 1, 5) + "sil2")));
            sut = new PartitionByString("test   ", (IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                >)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(
                false)));
            sut.SetCacheEvalRst(false);
            sut.SetHashSlice(":-4");
            sut.SetPartitionCount("1024");
            sut.SetPartitionLength("1");
            sut.Init();
            NUnit.Framework.Assert.AreEqual(99, (int)Execute(sut, testKey));
            NUnit.Framework.Assert.AreEqual(99, (int)Execute(sut, Sharpen.Runtime.Substring(testKey
                , 0, 6) + "abcd"));
            NUnit.Framework.Assert.AreEqual(99, (int)Execute(sut, Sharpen.Runtime.Substring(testKey
                , 0, 6) + "sila"));
            NUnit.Framework.Assert.IsTrue((99 != (int)Execute(sut, "c" + Sharpen.Runtime.Substring
                (testKey, 1, 5) + "sil2")));
            // 无start， 越界
            sut = new PartitionByString("test   ", (IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                >)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(
                false)));
            sut.SetCacheEvalRst(false);
            sut.SetHashSlice(":15");
            sut.SetPartitionCount("1024");
            sut.SetPartitionLength("1");
            sut.Init();
            NUnit.Framework.Assert.AreEqual(165, (int)Execute(sut, testKey));
            NUnit.Framework.Assert.AreEqual(647, (int)Execute(sut, "b" + testKey));
            sut = new PartitionByString("test   ", (IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                >)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(
                false)));
            sut.SetCacheEvalRst(false);
            sut.SetHashSlice(":-15");
            sut.SetPartitionCount("1024");
            sut.SetPartitionLength("1");
            sut.Init();
            NUnit.Framework.Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            NUnit.Framework.Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            // 无end， 不越界
            sut = new PartitionByString("test   ", (IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                >)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(
                false)));
            sut.SetCacheEvalRst(false);
            sut.SetHashSlice("2:");
            sut.SetPartitionCount("1024");
            sut.SetPartitionLength("1");
            sut.Init();
            NUnit.Framework.Assert.AreEqual(388, (int)Execute(sut, testKey));
            NUnit.Framework.Assert.AreEqual(388, (int)Execute(sut, "ab" + Sharpen.Runtime.Substring
                (testKey, 2, 10)));
            NUnit.Framework.Assert.AreEqual(388, (int)Execute(sut, "e&" + Sharpen.Runtime.Substring
                (testKey, 2, 10)));
            NUnit.Framework.Assert.IsTrue((388 != (int)Execute(sut, "c" + Sharpen.Runtime.Substring
                (testKey, 1, 5) + "sil2")));
            sut = new PartitionByString("test   ", (IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                >)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(
                false)));
            sut.SetCacheEvalRst(false);
            sut.SetHashSlice("-5:");
            sut.SetPartitionCount("1024");
            sut.SetPartitionLength("1");
            sut.Init();
            NUnit.Framework.Assert.AreEqual(808, (int)Execute(sut, testKey));
            NUnit.Framework.Assert.AreEqual(808, (int)Execute(sut, "abT*1" + Sharpen.Runtime.Substring
                (testKey, 5, 10)));
            NUnit.Framework.Assert.AreEqual(808, (int)Execute(sut, "ab^^!" + Sharpen.Runtime.Substring
                (testKey, 5, 10)));
            // 无end， 越界
            sut = new PartitionByString("test   ", (IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                >)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(
                false)));
            sut.SetCacheEvalRst(false);
            sut.SetHashSlice("-15:");
            sut.SetPartitionCount("1024");
            sut.SetPartitionLength("1");
            sut.Init();
            NUnit.Framework.Assert.AreEqual(165, (int)Execute(sut, testKey));
            NUnit.Framework.Assert.AreEqual(647, (int)Execute(sut, "b" + testKey));
            sut = new PartitionByString("test   ", (IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                >)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(
                false)));
            sut.SetCacheEvalRst(false);
            sut.SetHashSlice("15:");
            sut.SetPartitionCount("1024");
            sut.SetPartitionLength("1");
            sut.Init();
            NUnit.Framework.Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            NUnit.Framework.Assert.AreEqual(0, (int)Execute(sut, Sharpen.Runtime.Substring(UUID
                .RandomUUID().ToString(), 0, 10)));
            // 无start 无end
            sut = new PartitionByString("test   ", (IList<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                >)ListUtil.CreateList(new PlaceHolder("member_id", "MEMBER_ID").SetCacheEvalRst(
                false)));
            sut.SetCacheEvalRst(false);
            sut.SetHashSlice(":");
            sut.SetPartitionCount("1024");
            sut.SetPartitionLength("1");
            sut.Init();
            NUnit.Framework.Assert.AreEqual(165, (int)Execute(sut, testKey));
            NUnit.Framework.Assert.AreEqual(452, (int)Execute(sut, "b" + Sharpen.Runtime.Substring
                (testKey, 1)));
        }

        private static int Execute(PartitionByString sut, string key)
        {
            var map = new Dictionary<object, object>(1);
            map["MEMBER_ID"] = key;
            int v = (int)sut.Evaluation(map);
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
