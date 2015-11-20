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
using Tup.Cobar4Net.Util;

namespace Tup.Cobar4Net.Route.Util
{
    /// <summary>数据分区工具</summary>
    /// <author>xianmao.hexm 2009-3-16 上午11:56:45</author>
    public sealed class PartitionUtil
    {
        private const int PartitionLength = 1024;

        private const long AndValue = PartitionLength - 1;

        private readonly int[] segment = new int[PartitionLength];

        /// <summary><pre></summary>
        /// <param name="count">表示定义的分区数</param>
        /// <param name="length">
        /// 表示对应每个分区的取值长度
        /// 注意：其中count,length两个数组的长度必须是一致的。
        /// 约束：1024 = sum((count[i]*length[i])). count和length两个向量的点积恒等于1024
        /// </pre>
        /// </param>
        public PartitionUtil(int[] count, int[] length)
        {
            // 分区长度:数据段分布定义，其中取模的数一定要是2^n， 因为这里使用x % 2^n == x & (2^n - 1)等式，来优化性能。
            // %转换为&操作的换算数值
            // 分区线段
            if (count == null || length == null || (count.Length != length.Length))
            {
                throw new ArgumentException("error,check your scope & scopeLength definition.");
            }
            int segmentLength = 0;
            for (int i = 0; i < count.Length; i++)
            {
                segmentLength += count[i];
            }
            int[] ai = new int[segmentLength + 1];
            int index = 0;
            for (int i_1 = 0; i_1 < count.Length; i_1++)
            {
                for (int j = 0; j < count[i_1]; j++)
                {
                    ai[++index] = ai[index - 1] + length[i_1];
                }
            }
            if (ai[ai.Length - 1] != PartitionLength)
            {
                throw new ArgumentException("error,check your partitionScope definition.");
            }
            // 数据映射操作
            for (int i_2 = 1; i_2 < ai.Length; i_2++)
            {
                for (int j = ai[i_2 - 1]; j < ai[i_2]; j++)
                {
                    segment[j] = (i_2 - 1);
                }
            }
        }

        public int Partition(long hash)
        {
            return segment[(int)(hash & AndValue)];
        }

        public int Partition(string key, int start, int end)
        {
            return Partition(StringUtil.Hash(key, start, end));
        }
    }
}