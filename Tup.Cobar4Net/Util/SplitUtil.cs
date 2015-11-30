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
using System.Linq;
using System.Text;
using Sharpen;

namespace Tup.Cobar4Net.Util
{
    /// <author>xianmao.hexm 2012-5-31</author>
    public class SplitUtil
    {
        private static readonly string[] EmptyStringArray = new string[0];

        /// <summary>
        ///     解析字符串<br />
        ///     比如:c1='$',c2='-' 输入字符串：mysql_db$0-2<br />
        ///     输出array:mysql_db[0],mysql_db[1],mysql_db[2]
        /// </summary>
        public static string[] Split2(string src, char c1, char c2)
        {
            if (src == null)
            {
                return null;
            }
            var length = src.Length;
            if (length == 0)
            {
                return EmptyStringArray;
            }
            IList<string> list = new List<string>();
            var p = Split(src, c1, true);
            if (p.Length > 1)
            {
                var scope = Split(p[1], c2, true);
                var min = Convert.ToInt32(scope[0]);
                var max = Convert.ToInt32(scope[scope.Length - 1]);
                for (var x = min; x <= max; x++)
                {
                    list.Add(new StringBuilder(p[0]).Append('[').Append(x).Append(']').ToString());
                }
            }
            else
            {
                list.Add(p[0]);
            }
            return list.ToArray();
        }

        public static string[] Split(string src)
        {
            return Split(src, null, -1);
        }

        public static string[] Split(string src, char separatorChar)
        {
            if (src == null)
            {
                return null;
            }
            var length = src.Length;
            if (length == 0)
            {
                return EmptyStringArray;
            }
            var list = new List<string>();
            var i = 0;
            var start = 0;
            var match = false;
            while (i < length)
            {
                if (src[i] == separatorChar)
                {
                    if (match)
                    {
                        list.Add(Runtime.Substring(src, start, i));
                        match = false;
                    }
                    start = ++i;
                    continue;
                }
                match = true;
                i++;
            }
            if (match)
            {
                list.Add(Runtime.Substring(src, start, i));
            }
            return list.ToArray();
        }

        public static string[] Split(string src, char separatorChar, bool trim)
        {
            if (src == null)
            {
                return null;
            }
            var length = src.Length;
            if (length == 0)
            {
                return EmptyStringArray;
            }
            IList<string> list = new List<string>();
            var i = 0;
            var start = 0;
            var match = false;
            while (i < length)
            {
                if (src[i] == separatorChar)
                {
                    if (match)
                    {
                        if (trim)
                        {
                            list.Add(Runtime.Substring(src, start, i).Trim());
                        }
                        else
                        {
                            list.Add(Runtime.Substring(src, start, i));
                        }
                        match = false;
                    }
                    start = ++i;
                    continue;
                }
                match = true;
                i++;
            }
            if (match)
            {
                if (trim)
                {
                    list.Add(Runtime.Substring(src, start, i).Trim());
                }
                else
                {
                    list.Add(Runtime.Substring(src, start, i));
                }
            }
            return list.ToArray();
        }

        public static string[] Split(string str, string separatorChars)
        {
            return Split(str, separatorChars, -1);
        }

        public static string[] Split(string src, string separatorChars, int max)
        {
            if (src == null)
            {
                return null;
            }
            var length = src.Length;
            if (length == 0)
            {
                return EmptyStringArray;
            }
            var list = new List<string>();
            var sizePlus1 = 1;
            var i = 0;
            var start = 0;
            var match = false;
            if (separatorChars == null)
            {
                // null表示使用空白作为分隔符
                while (i < length)
                {
                    if (char.IsWhiteSpace(src[i]))
                    {
                        if (match)
                        {
                            if (sizePlus1++ == max)
                            {
                                i = length;
                            }
                            list.Add(Runtime.Substring(src, start, i));
                            match = false;
                        }
                        start = ++i;
                        continue;
                    }
                    match = true;
                    i++;
                }
            }
            else
            {
                if (separatorChars.Length == 1)
                {
                    // 优化分隔符长度为1的情形
                    var sep = separatorChars[0];
                    while (i < length)
                    {
                        if (src[i] == sep)
                        {
                            if (match)
                            {
                                if (sizePlus1++ == max)
                                {
                                    i = length;
                                }
                                list.Add(Runtime.Substring(src, start, i));
                                match = false;
                            }
                            start = ++i;
                            continue;
                        }
                        match = true;
                        i++;
                    }
                }
                else
                {
                    // 一般情形
                    while (i < length)
                    {
                        if (separatorChars.IndexOf(src[i]) >= 0)
                        {
                            if (match)
                            {
                                if (sizePlus1++ == max)
                                {
                                    i = length;
                                }
                                list.Add(Runtime.Substring(src, start, i));
                                match = false;
                            }
                            start = ++i;
                            continue;
                        }
                        match = true;
                        i++;
                    }
                }
            }
            if (match)
            {
                list.Add(Runtime.Substring(src, start, i));
            }
            return list.ToArray();
        }

        /// <summary>
        ///     解析字符串，比如: <br />
        ///     1.
        /// </summary>
        /// <remarks>
        ///     解析字符串，比如: <br />
        ///     1. c1='$',c2='-',c3='[',c4=']' 输入字符串：mysql_db$0-2<br />
        ///     输出mysql_db[0],mysql_db[1],mysql_db[2]<br />
        ///     2. c1='$',c2='-',c3='#',c4='0' 输入字符串：mysql_db$0-2<br />
        ///     输出mysql_db#0,mysql_db#1,mysql_db#2<br />
        ///     3. c1='$',c2='-',c3='0',c4='0' 输入字符串：mysql_db$0-2<br />
        ///     输出mysql_db0,mysql_db1,mysql_db2<br />
        /// </remarks>
        public static string[] Split(string src, char c1, char c2, char c3, char c4)
        {
            if (src == null)
            {
                return null;
            }
            var length = src.Length;
            if (length == 0)
            {
                return EmptyStringArray;
            }
            IList<string> list = new List<string>();
            if (src.IndexOf(c1) == -1)
            {
                list.Add(src.Trim());
            }
            else
            {
                var s = Split(src, c1, true);
                var scope = Split(s[1], c2, true);
                var min = Convert.ToInt32(scope[0]);
                var max = Convert.ToInt32(scope[scope.Length - 1]);
                if (c3 == '0')
                {
                    for (var x = min; x <= max; x++)
                    {
                        list.Add(new StringBuilder(s[0]).Append(x).ToString());
                    }
                }
                else
                {
                    if (c4 == '0')
                    {
                        for (var x = min; x <= max; x++)
                        {
                            list.Add(new StringBuilder(s[0]).Append(c3).Append(x).ToString());
                        }
                    }
                    else
                    {
                        for (var x = min; x <= max; x++)
                        {
                            list.Add(new StringBuilder(s[0]).Append(c3).Append(x).Append(c4).ToString());
                        }
                    }
                }
            }
            return list.ToArray();
        }

        public static string[] Split(string src, char fi, char se, char th)
        {
            return Split(src, fi, se, th, '0', '0');
        }

        public static string[] Split(string src, char fi, char se, char th, char left, char right)
        {
            var list = new List<string>();
            var pools = Split(src, fi, true);
            for (var i = 0; i < pools.Length; i++)
            {
                if (pools[i].IndexOf(se) == -1)
                {
                    list.Add(pools[i]);
                    continue;
                }
                var s = Split(pools[i], se, th, left, right);
                for (var j = 0; j < s.Length; j++)
                {
                    list.Add(s[j]);
                }
            }
            return list.ToArray();
        }
    }
}