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

namespace Tup.Cobar4Net.Parser.Util
{
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public static class PairUtil
    {
        private const int DefaultIndex = -1;

        /// <summary>
        ///     "2" -&gt; (0,2)<br />
        ///     "1:2" -&gt; (1,2)<br />
        ///     "1:" -&gt; (1,0)<br />
        ///     "-1:" -&gt; (-1,0)<br />
        ///     ":-1" -&gt; (0,-1)<br />
        ///     ":" -&gt; (0,0)<br />
        /// </summary>
        public static Pair<int, int> SequenceSlicing(string slice)
        {
            var ind = slice.IndexOf(':');
            if (ind < 0)
            {
                var i = Convert.ToInt32(slice.Trim());
                if (i >= 0)
                    return new Pair<int, int>(0, i);
                return new Pair<int, int>(i, 0);
            }
            var left = Runtime.Substring(slice, 0, ind).Trim();
            var right = Runtime.Substring(slice, 1 + ind).Trim();
            var start = left.Length <= 0 ? 0 : Convert.ToInt32(left);
            var end = right.Length <= 0 ? 0 : Convert.ToInt32(right);
            return new Pair<int, int>(start, end);
        }

        /// <summary>
        ///     <pre>
        ///         将名字和索引用进行分 当src = "offer_group[4]", l='[', r=']'时，
        ///         返回的Piar
        ///         <String, Integer>
        ///             ("offer", 4);
        ///             当src = "offer_group", l='[', r=']'时，
        ///             返回Pair<String, Integer>("offer",-1);
        ///     </pre>
        /// </summary>
        public static Pair<string, int> SplitIndex(string src, char l, char r)
        {
            if (src == null)
            {
                return null;
            }
            var length = src.Length;
            if (length == 0)
            {
                return new Pair<string, int>(string.Empty, DefaultIndex);
            }
            if (src[length - 1] != r)
            {
                return new Pair<string, int>(src, DefaultIndex);
            }
            var offset = src.LastIndexOf(l);
            if (offset == -1)
            {
                return new Pair<string, int>(src, DefaultIndex);
            }
            var index = DefaultIndex;
            try
            {
                index = Convert.ToInt32(Runtime.Substring(src, offset + 1, length - 1));
            }
            catch (FormatException)
            {
                return new Pair<string, int>(src, DefaultIndex);
            }
            return new Pair<string, int>(Runtime.Substring(src, 0, offset), index);
        }
    }
}