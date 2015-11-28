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

using Sharpen;
using System;
using System.Text;

namespace Tup.Cobar4Net.Route.Hint
{
    /// <summary>Stateless</summary>
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public abstract class HintParser
    {
        protected internal static bool IsDigit(char c)
        {
            switch (c)
            {
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    {
                        return true;
                    }

                default:
                    {
                        return false;
                    }
            }
        }

        /// <summary>
        /// hint's
        /// <see cref="CobarHint.GetCurrentIndex()"/>
        /// will be changed to index of
        /// next char after process
        /// </summary>
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        public abstract void Process(CobarHint hint, string hintName, string sql);

        private void SkipSpace(CobarHint hint, string sql)
        {
            int ci = hint.GetCurrentIndex();
            for (;;)
            {
                switch (sql[ci])
                {
                    case ' ':
                    case '\t':
                    case '\n':
                    case '\r':
                        {
                            hint.IncreaseCurrentIndex();
                            ++ci;
                            break;
                        }
                    default:
                        {
                            goto skip_break;
                        }
                }
            }
        skip_break:;
        }

        protected internal virtual char CurrentChar(CobarHint hint, string sql)
        {
            SkipSpace(hint, sql);
            return sql[hint.GetCurrentIndex()];
        }

        /// <summary>current char is not separator</summary>
        protected internal virtual char NextChar(CobarHint hint, string sql)
        {
            SkipSpace(hint, sql);
            SkipSpace(hint.IncreaseCurrentIndex(), sql);
            return sql[hint.GetCurrentIndex()];
        }

        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        protected internal virtual object ParsePrimary(CobarHint hint, string sql)
        {
            char c = CurrentChar(hint, sql);
            int ci = hint.GetCurrentIndex();
            switch (c)
            {
                case '\'':
                    {
                        var sb = new StringBuilder();
                        for (++ci; ; ++ci)
                        {
                            c = sql[ci];
                            switch (c)
                            {
                                case '\'':
                                    {
                                        hint.SetCurrentIndex(ci + 1);
                                        return sb.ToString();
                                    }

                                case '\\':
                                    {
                                        c = sql[++ci];
                                        goto default;
                                    }

                                default:
                                    {
                                        sb.Append(c);
                                        break;
                                    }
                            }
                        }
#pragma warning disable CS0162 // 检测到无法访问的代码
                        goto case 'n';
#pragma warning restore CS0162 // 检测到无法访问的代码
                    }

                case 'n':
                case 'N':
                    {
                        hint.SetCurrentIndex(ci + "null".Length);
                        return null;
                    }

                default:
                    {
                        if (IsDigit(c))
                        {
                            int start = ci++;
                            for (; IsDigit(sql[ci]); ++ci)
                            {
                            }
                            hint.SetCurrentIndex(ci);
                            return long.Parse(Runtime.Substring(sql, start, ci));
                        }
                        throw new SQLSyntaxErrorException("unknown primary in hint: " + sql);
                    }
            }
        }
    }
}