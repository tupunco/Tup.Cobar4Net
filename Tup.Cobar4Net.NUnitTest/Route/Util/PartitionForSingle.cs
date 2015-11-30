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

namespace Tup.Cobar4Net.Route.Util
{
    /// <summary>数据分区工具单独版本，请使用singleton的模式调用。</summary>
    /// <author>xianmao.hexm 2009-3-16 上午11:56:45</author>
    public sealed class PartitionForSingle
    {
        private const int PartitionLength = 1024;

        private const int DefaultHashLength = 8;

        private const long AndValue = PartitionLength - 1;

        private readonly int[] segment = new int[PartitionLength];

        /// <param name="count">表示定义的分区数</param>
        /// <param name="length">
        ///     表示对应每个分区的取值长度
        ///     <p>
        ///         注意：其中count,length两个数组的长度必须是一致的。
        ///     </p>
        /// </param>
        public PartitionForSingle(int[] count, int[] length)
        {
            // 分区长度:数据段分布定义，其中取模的数一定要是2^n， 因为这里使用x % 2^n == x & (2^n - 1)等式，来优化性能。
            // %转换为&操作的换算数值
            // 分区线段
            if (count == null || length == null || (count.Length != length.Length))
            {
                throw new Exception("error,check your _hintScope & scopeLength definition.");
            }
            var segmentLength = 0;
            for (var i = 0; i < count.Length; i++)
            {
                segmentLength += count[i];
            }
            var scopeSegment = new int[segmentLength + 1];
            var index = 0;
            for (var i_1 = 0; i_1 < count.Length; i_1++)
            {
                for (var j = 0; j < count[i_1]; j++)
                {
                    scopeSegment[++index] = scopeSegment[index - 1] + length[i_1];
                }
            }
            if (scopeSegment[scopeSegment.Length - 1] != PartitionLength)
            {
                throw new Exception("error,check your partitionScope definition.");
            }
            // 数据映射操作
            for (var i_2 = 1; i_2 < scopeSegment.Length; i_2++)
            {
                for (var j = scopeSegment[i_2 - 1]; j < scopeSegment[i_2]; j++)
                {
                    segment[j] = i_2 - 1;
                }
            }
        }

        public int Partition(long h)
        {
            return segment[(int)(h & AndValue)];
        }

        public int Partition(string key)
        {
            return segment[(int)(Hash(key) & AndValue)];
        }

        private static long Hash(string s)
        {
            long h = 0;
            var len = s.Length;
            for (var i = 0; i < DefaultHashLength && i < len; i++)
            {
                h = (h << 5) - h + s[i];
            }
            return h;
        }

        // for test
        public static void Main(string[] args)
        {
            //TODO PartitionForSingle Main
            // 拆分为16份，每份长度均为：64。
            // _hintScope _hintScope = new _hintScope(new int[] { 16 }, new int[] { 64 });
            // // 拆分为23份，前8份长度为：8，后15份长度为：64。
            // _hintScope _hintScope = new _hintScope(new int[] { 8, 15 }, new int[] { 8, 64 });
            // // 拆分为128份，每份长度均为：8。
            // _hintScope _hintScope = new _hintScope(new int[] { 128 }, new int[] { 8 });
            var p = new PartitionForSingle(new[] {8, 15}, new[] {8, 64});
            var memberId = "xianmao.hexm";
            var value = 0;
            var st = Runtime.CurrentTimeMillis();
            for (var i = 0; i < 10000000; i++)
            {
                value = p.Partition(memberId);
            }
            var et = Runtime.CurrentTimeMillis();
            Console.Out.WriteLine("value:" + value + ",take time:" + (et - st) + " ms.");
        }
    }
}