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

using System.Text;

namespace Tup.Cobar4Net.Util
{
    /// <summary>格式化工具</summary>
    /// <author>xianmao.hexm</author>
    /// <version>2008-11-24 下午12:58:17</version>
    public static class FormatUtil
    {
        public const int AlignRight = 0;

        public const int AlignLeft = 1;

        private const char defaultSplitChar = ' ';

        private static readonly string[] timeFormat = {"d ", "h ", "m ", "s ", "ms"};

        // 右对齐格式化字符串
        // 左对齐格式化字符串
        /// <summary>格式化后返回的字符串</summary>
        /// <param name="s">需要格式化的原始字符串，默认按左对齐。</param>
        /// <param name="fillLength">填充长度</param>
        /// <returns>String</returns>
        public static string Format(string s, int fillLength)
        {
            return Format(s, fillLength, defaultSplitChar, AlignLeft);
        }

        /// <summary>格式化后返回的字符串</summary>
        /// <param name="i">需要格式化的数字类型，默认按右对齐。</param>
        /// <param name="fillLength">填充长度</param>
        /// <returns>String</returns>
        public static string Format(int i, int fillLength)
        {
            return Format(i.ToString(), fillLength, defaultSplitChar, AlignRight);
        }

        /// <summary>格式化后返回的字符串</summary>
        /// <param name="l">需要格式化的数字类型，默认按右对齐。</param>
        /// <param name="fillLength">填充长度</param>
        /// <returns>String</returns>
        public static string Format(long l, int fillLength)
        {
            return Format(l.ToString(), fillLength, defaultSplitChar, AlignRight);
        }

        /// <param name="s">需要格式化的原始字符串</param>
        /// <param name="fillLength">填充长度</param>
        /// <param name="fillChar">填充的字符</param>
        /// <param name="align">填充方式（左边填充还是右边填充）</param>
        /// <returns>String</returns>
        public static string Format(string s, int fillLength, char fillChar, int align)
        {
            if (s == null)
            {
                s = string.Empty;
            }
            else
            {
                s = s.Trim();
            }
            var charLen = fillLength - s.Length;
            if (charLen > 0)
            {
                var fills = new char[charLen];
                for (var i = 0; i < charLen; i++)
                {
                    fills[i] = fillChar;
                }
                var str = new StringBuilder(s);
                switch (align)
                {
                    case AlignRight:
                    {
                        str.Insert(0, fills);
                        break;
                    }

                    case AlignLeft:
                    {
                        str.Append(fills);
                        break;
                    }

                    default:
                    {
                        str.Append(fills);
                        break;
                    }
                }
                return str.ToString();
            }
            return s;
        }

        /// <summary>
        ///     格式化时间输出
        ///     <p>
        ///         1d 15h 4m 15s 987ms
        ///     </p>
        /// </summary>
        public static string FormatTime(long millis, int precision)
        {
            var la = new long[5];
            la[0] = millis/86400000;
            // days
            la[1] = millis/3600000%24;
            // hours
            la[2] = millis/60000%60;
            // minutes
            la[3] = millis/1000%60;
            // seconds
            la[4] = millis%1000;
            // ms
            var index = 0;
            for (var i = 0; i < la.Length; i++)
            {
                if (la[i] != 0)
                {
                    index = i;
                    break;
                }
            }
            var buf = new StringBuilder();
            var validLength = la.Length - index;
            for (var i_1 = 0; i_1 < validLength && i_1 < precision; i_1++)
            {
                buf.Append(la[index]).Append(timeFormat[index]);
                index++;
            }
            return buf.ToString();
        }
    }
}