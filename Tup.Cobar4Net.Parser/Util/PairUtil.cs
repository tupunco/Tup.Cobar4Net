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

namespace Tup.Cobar4Net.Parser.Util
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public sealed class PairUtil
    {
        private const int DefaultIndex = -1;

        /// <summary>
        /// "2" -&gt; (0,2)<br/>
        /// "1:2" -&gt; (1,2)<br/>
        /// "1:" -&gt; (1,0)<br/>
        /// "-1:" -&gt; (-1,0)<br/>
        /// ":-1" -&gt; (0,-1)<br/>
        /// ":" -&gt; (0,0)<br/>
        /// </summary>
        public static Pair<int, int> SequenceSlicing(string slice)
        {
            int ind = slice.IndexOf(':');
            if (ind < 0)
            {
                int i = System.Convert.ToInt32(slice.Trim());
                if (i >= 0)
                {
                    return new Pair<int, int>(0, i);
                }
                else
                {
                    return new Pair<int, int>(i, 0);
                }
            }
            string left = Sharpen.Runtime.Substring(slice, 0, ind).Trim();
            string right = Sharpen.Runtime.Substring(slice, 1 + ind).Trim();
            int start;
            int end;
            if (left.Length <= 0)
            {
                start = 0;
            }
            else
            {
                start = System.Convert.ToInt32(left);
            }
            if (right.Length <= 0)
            {
                end = 0;
            }
            else
            {
                end = System.Convert.ToInt32(right);
            }
            return new Pair<int, int>(start, end);
        }

        /// <summary>
        /// <pre>
        /// 将名字和索引用进行分 当src = "offer_group[4]", l='[', r=']'时，
        /// 返回的Piar<String,Integer>("offer", 4);
        /// 当src = "offer_group", l='[', r=']'时，
        /// 返回Pair<String, Integer>("offer",-1);
        /// </pre>
        /// </summary>
        public static Pair<string, int> SplitIndex(string src, char l, char r)
        {
            if (src == null)
            {
                return null;
            }
            int length = src.Length;
            if (length == 0)
            {
                return new Pair<string, int>(string.Empty, DefaultIndex);
            }
            if (src[length - 1] != r)
            {
                return new Pair<string, int>(src, DefaultIndex);
            }
            int offset = src.LastIndexOf(l);
            if (offset == -1)
            {
                return new Pair<string, int>(src, DefaultIndex);
            }
            int index = DefaultIndex;
            try
            {
                index = System.Convert.ToInt32(Sharpen.Runtime.Substring(src, offset + 1, length - 1));
            }
            catch (FormatException)
            {
                return new Pair<string, int>(src, DefaultIndex);
            }
            return new Pair<string, int>(Sharpen.Runtime.Substring(src, 0, offset), index);
        }
    }
}