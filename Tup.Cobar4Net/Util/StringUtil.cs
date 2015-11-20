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
using System.Text;

namespace Tup.Cobar4Net.Util
{
    /// <author>xianmao.hexm 2011-5-9 下午02:40:29</author>
    public class StringUtil
    {
        private static readonly byte[] EmptyByteArray = new byte[0];

        private static readonly Random Random = new Random();

        private static readonly char[] Chars = new char[] { '1', '2', '3', '4', '5', '6',
            '7', '8', '9', '0', 'q', 'w', 'e', 'r', 't', 'y', 'u', 'i', 'o', 'p', 'a', 's',
            'd', 'f', 'g', 'h', 'j', 'k', 'l', 'z', 'x', 'c', 'v', 'b', 'n', 'm', 'Q', 'W',
            'E', 'R', 'T', 'Y', 'U', 'I', 'O', 'P', 'A', 'S', 'D', 'F', 'G', 'H', 'J', 'K',
            'L', 'Z', 'X', 'C', 'V', 'B', 'N', 'M' };

        /// <summary>字符串hash算法：s[0]*31^(n-1) + s[1]*31^(n-2) + ...</summary>
        /// <remarks>
        /// 字符串hash算法：s[0]*31^(n-1) + s[1]*31^(n-2) + ... + s[n-1] <br />
        /// 其中s[]为字符串的字符数组，换算成程序的表达式为：<br />
        /// h = 31*h + s.charAt(i); =&gt; h = (h <&lt; 5) - h + s.charAt(i); &lt;br>
        /// </remarks>
        /// <param name="start">hash for s.substring(start, end)</param>
        /// <param name="end">hash for s.substring(start, end)</param>
        public static long Hash(string s, int start, int end)
        {
            if (start < 0)
            {
                start = 0;
            }
            if (end > s.Length)
            {
                end = s.Length;
            }
            long h = 0;
            for (int i = start; i < end; ++i)
            {
                h = (h << 5) - h + s[i];
            }
            return h;
        }

        public static byte[] Encode(string src, string charset)
        {
            if (src == null)
            {
                return null;
            }
            try
            {
                return Sharpen.Runtime.GetBytesForString(src, charset);
            }
            catch (EncoderFallbackException)
            {
                return Sharpen.Runtime.GetBytesForString(src);
            }
        }

        public static string Decode(byte[] src, string charset)
        {
            return Decode(src, 0, src.Length, charset);
        }

        public static string Decode(byte[] src, int offset, int length, string charset)
        {
            try
            {
                return Sharpen.Runtime.GetStringForBytes(src, offset, length, charset);
            }
            catch (EncoderFallbackException)
            {
                return Sharpen.Runtime.GetStringForBytes(src, offset, length);
            }
        }

        public static string GetRandomString(int size)
        {
            StringBuilder s = new StringBuilder(size);
            int len = Chars.Length;
            for (int i = 0; i < size; i++)
            {
                int x = Random.Next();
                s.Append(Chars[(x < 0 ? -x : x) % len]);
            }
            return s.ToString();
        }

        public static string SafeToString(object @object)
        {
            try
            {
                return @object.ToString();
            }
            catch (Exception t)
            {
                return "<toString() failure: " + t + ">";
            }
        }

        public static bool IsEmpty(string str)
        {
            return ((str == null) || (str.Length == 0));
        }

        public static byte[] HexString2Bytes(char[] hexString, int offset, int length)
        {
            if (hexString == null)
            {
                return null;
            }
            if (length == 0)
            {
                return EmptyByteArray;
            }
            bool odd = length << 31 == int.MinValue;
            byte[] bs = new byte[odd ? (length + 1) >> 1 : length >> 1];
            for (int i = offset, limit = offset + length; i < limit; ++i)
            {
                char high;
                char low;
                if (i == offset && odd)
                {
                    high = '0';
                    low = hexString[i];
                }
                else
                {
                    high = hexString[i];
                    low = hexString[++i];
                }
                int b;
                switch (high)
                {
                    case '0':
                        {
                            b = 0;
                            break;
                        }

                    case '1':
                        {
                            b = unchecked((int)(0x10));
                            break;
                        }

                    case '2':
                        {
                            b = unchecked((int)(0x20));
                            break;
                        }

                    case '3':
                        {
                            b = unchecked((int)(0x30));
                            break;
                        }

                    case '4':
                        {
                            b = unchecked((int)(0x40));
                            break;
                        }

                    case '5':
                        {
                            b = unchecked((int)(0x50));
                            break;
                        }

                    case '6':
                        {
                            b = unchecked((int)(0x60));
                            break;
                        }

                    case '7':
                        {
                            b = unchecked((int)(0x70));
                            break;
                        }

                    case '8':
                        {
                            b = unchecked((int)(0x80));
                            break;
                        }

                    case '9':
                        {
                            b = unchecked((int)(0x90));
                            break;
                        }

                    case 'a':
                    case 'A':
                        {
                            b = unchecked((int)(0xa0));
                            break;
                        }

                    case 'b':
                    case 'B':
                        {
                            b = unchecked((int)(0xb0));
                            break;
                        }

                    case 'c':
                    case 'C':
                        {
                            b = unchecked((int)(0xc0));
                            break;
                        }

                    case 'd':
                    case 'D':
                        {
                            b = unchecked((int)(0xd0));
                            break;
                        }

                    case 'e':
                    case 'E':
                        {
                            b = unchecked((int)(0xe0));
                            break;
                        }

                    case 'f':
                    case 'F':
                        {
                            b = unchecked((int)(0xf0));
                            break;
                        }

                    default:
                        {
                            throw new ArgumentException("illegal hex-string: " + new string(hexString, offset
                                , length));
                        }
                }
                switch (low)
                {
                    case '0':
                        {
                            break;
                        }

                    case '1':
                        {
                            b += 1;
                            break;
                        }

                    case '2':
                        {
                            b += 2;
                            break;
                        }

                    case '3':
                        {
                            b += 3;
                            break;
                        }

                    case '4':
                        {
                            b += 4;
                            break;
                        }

                    case '5':
                        {
                            b += 5;
                            break;
                        }

                    case '6':
                        {
                            b += 6;
                            break;
                        }

                    case '7':
                        {
                            b += 7;
                            break;
                        }

                    case '8':
                        {
                            b += 8;
                            break;
                        }

                    case '9':
                        {
                            b += 9;
                            break;
                        }

                    case 'a':
                    case 'A':
                        {
                            b += 10;
                            break;
                        }

                    case 'b':
                    case 'B':
                        {
                            b += 11;
                            break;
                        }

                    case 'c':
                    case 'C':
                        {
                            b += 12;
                            break;
                        }

                    case 'd':
                    case 'D':
                        {
                            b += 13;
                            break;
                        }

                    case 'e':
                    case 'E':
                        {
                            b += 14;
                            break;
                        }

                    case 'f':
                    case 'F':
                        {
                            b += 15;
                            break;
                        }

                    default:
                        {
                            throw new ArgumentException("illegal hex-string: " + new string(hexString, offset
                                , length));
                        }
                }
                bs[(i - offset) >> 1] = unchecked((byte)b);
            }
            return bs;
        }

        public static string DumpAsHex(byte[] src, int length)
        {
            StringBuilder @out = new StringBuilder(length * 4);
            int p = 0;
            int rows = length / 8;
            for (int i = 0; (i < rows) && (p < length); i++)
            {
                int ptemp = p;
                for (int j = 0; j < 8; j++)
                {
                    string hexVal = Sharpen.Extensions.ToHexString(src[ptemp] & unchecked((int)(0xff)));
                    if (hexVal.Length == 1)
                    {
                        @out.Append('0');
                    }
                    @out.Append(hexVal).Append(' ');
                    ptemp++;
                }
                @out.Append("    ");
                for (int j_1 = 0; j_1 < 8; j_1++)
                {
                    int b = unchecked((int)(0xff)) & src[p];
                    if (b > 32 && b < 127)
                    {
                        @out.Append((char)b).Append(' ');
                    }
                    else
                    {
                        @out.Append(". ");
                    }
                    p++;
                }
                @out.Append('\n');
            }
            int n = 0;
            for (int i_1 = p; i_1 < length; i_1++)
            {
                string hexVal = Sharpen.Extensions.ToHexString(src[i_1] & unchecked((int)(0xff)));
                if (hexVal.Length == 1)
                {
                    @out.Append('0');
                }
                @out.Append(hexVal).Append(' ');
                n++;
            }
            for (int i_2 = n; i_2 < 8; i_2++)
            {
                @out.Append("   ");
            }
            @out.Append("    ");
            for (int i_3 = p; i_3 < length; i_3++)
            {
                int b = unchecked((int)(0xff)) & src[i_3];
                if (b > 32 && b < 127)
                {
                    @out.Append((char)b).Append(' ');
                }
                else
                {
                    @out.Append(". ");
                }
            }
            @out.Append('\n');
            return @out.ToString();
        }

        //public static byte[] EscapeEasternUnicodeByteStream(byte[] src, string srcString,
        //    int offset, int length)
        //{
        //    if ((src == null) || (src.Length == 0))
        //    {
        //        return src;
        //    }
        //    int bytesLen = src.Length;
        //    int bufIndex = 0;
        //    int strIndex = 0;
        //    ByteArrayOutputStream @out = new ByteArrayOutputStream(bytesLen);
        //    while (true)
        //    {
        //        if (srcString[strIndex] == '\\')
        //        {
        //            // write it out as-is
        //            @out.Write(src[bufIndex++]);
        //        }
        //        else
        //        {
        //            // Grab the first byte
        //            int loByte = src[bufIndex];
        //            if (loByte < 0)
        //            {
        //                loByte += 256;
        //            }
        //            // adjust for signedness/wrap-around
        //            @out.Write(loByte);
        //            // We always write the first byte
        //            if (loByte >= unchecked((int)(0x80)))
        //            {
        //                if (bufIndex < (bytesLen - 1))
        //                {
        //                    int hiByte = src[bufIndex + 1];
        //                    if (hiByte < 0)
        //                    {
        //                        hiByte += 256;
        //                    }
        //                    // adjust for signedness/wrap-around
        //                    @out.Write(hiByte);
        //                    // write the high byte here, and
        //                    // increment the index for the high
        //                    // byte
        //                    bufIndex++;
        //                    if (hiByte == unchecked((int)(0x5C)))
        //                    {
        //                        @out.Write(hiByte);
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                // escape 0x5c if necessary
        //                if (loByte == unchecked((int)(0x5c)))
        //                {
        //                    if (bufIndex < (bytesLen - 1))
        //                    {
        //                        int hiByte = src[bufIndex + 1];
        //                        if (hiByte < 0)
        //                        {
        //                            hiByte += 256;
        //                        }
        //                        // adjust for signedness/wrap-around
        //                        if (hiByte == unchecked((int)(0x62)))
        //                        {
        //                            // we need to escape the 0x5c
        //                            @out.Write(unchecked((int)(0x5c)));
        //                            @out.Write(unchecked((int)(0x62)));
        //                            bufIndex++;
        //                        }
        //                    }
        //                }
        //            }
        //            bufIndex++;
        //        }
        //        if (bufIndex >= bytesLen)
        //        {
        //            break;
        //        }
        //        // we're done
        //        strIndex++;
        //    }
        //    return @out.ToByteArray();
        //}

        public static string ToString(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                return string.Empty;
            }
            var buffer = new StringBuilder();
            foreach (byte byt in bytes)
            {
                buffer.Append((char)byt);
            }
            return buffer.ToString();
        }

        public static bool EqualsIgnoreCase(string str1, string str2)
        {
            if (str1 == null)
            {
                return str2 == null;
            }
            return Sharpen.Runtime.EqualsIgnoreCase(str1, str2);
        }

        public static int CountChar(string str, char c)
        {
            if (str == null || str.IsEmpty())
            {
                return 0;
            }
            int len = str.Length;
            int cnt = 0;
            for (int i = 0; i < len; ++i)
            {
                if (c == str[i])
                {
                    ++cnt;
                }
            }
            return cnt;
        }

        public static string ReplaceOnce(string text, string repl, string with)
        {
            return Replace(text, repl, with, 1);
        }

        public static string Replace(string text, string repl, string with)
        {
            return Replace(text, repl, with, -1);
        }

        public static string Replace(string text, string repl, string with, int max)
        {
            if ((text == null) || (repl == null) || (with == null) || (repl.Length == 0) || (
                max == 0))
            {
                return text;
            }
            var buf = new StringBuilder(text.Length);
            int start = 0;
            int end = 0;
            while ((end = text.IndexOf(repl, start, StringComparison.Ordinal)) != -1)
            {
                buf.Append(Sharpen.Runtime.Substring(text, start, end)).Append(with);
                start = end + repl.Length;
                if (--max == 0)
                {
                    break;
                }
            }
            buf.Append(Sharpen.Runtime.Substring(text, start));
            return buf.ToString();
        }

        public static string ReplaceChars(string str, char searchChar, char replaceChar)
        {
            if (str == null)
            {
                return null;
            }
            return str.Replace(searchChar, replaceChar);
        }

        public static string ReplaceChars(string str, string searchChars, string replaceChars)
        {
            if ((str == null) || (str.Length == 0) || (searchChars == null) || (searchChars.Length
                 == 0))
            {
                return str;
            }
            char[] chars = str.ToCharArray();
            int len = chars.Length;
            bool modified = false;
            for (int i = 0, isize = searchChars.Length; i < isize; i++)
            {
                char searchChar = searchChars[i];
                if ((replaceChars == null) || (i >= replaceChars.Length))
                {
                    // 删除
                    int pos = 0;
                    for (int j = 0; j < len; j++)
                    {
                        if (chars[j] != searchChar)
                        {
                            chars[pos++] = chars[j];
                        }
                        else
                        {
                            modified = true;
                        }
                    }
                    len = pos;
                }
                else
                {
                    // 替换
                    for (int j = 0; j < len; j++)
                    {
                        if (chars[j] == searchChar)
                        {
                            chars[j] = replaceChars[i];
                            modified = true;
                        }
                    }
                }
            }
            if (!modified)
            {
                return str;
            }
            return new string(chars, 0, len);
        }
    }
}