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
using Tup.Cobar4Net.Parser.Util;

namespace Tup.Cobar4Net.Route.Hint
{
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public sealed class PartitionOperandHintParser : HintParser
    {
        private static string[] Convert2String(object[] objs)
        {
            var strings = new string[objs.Length];
            for (var i = 0; i < objs.Length; ++i)
            {
                strings[i] = (string)objs[i];
            }
            return strings;
        }

        /// <exception cref="System.SqlSyntaxErrorException" />
        public override void Process(CobarHint hint, string hintName, string sql)
        {
            string[] columns;
            if (NextChar(hint, sql) == '[')
            {
                hint.IncreaseCurrentIndex();
                columns = Convert2String(ParseArray(hint, sql, -1));
            }
            else
            {
                columns = new string[1];
                columns[0] = (string)ParsePrimary(hint, sql);
            }
            object[][] values;
            switch (NextChar(hint, sql))
            {
                case '[':
                {
                    if (columns.Length == 1)
                    {
                        hint.IncreaseCurrentIndex();
                        var vs = ParseArray(hint, sql, -1);
                        values = new object[vs.Length][];
                        for (var i = 0; i < vs.Length; ++i)
                        {
                            values[i] = new object[1] {vs[i]};
                            //values[i][0] = vs[i];
                        }
                    }
                    else
                    {
                        values = ParseArrayArray(hint, sql, columns.Length);
                    }
                    break;
                }

                default:
                {
                    if (columns.Length == 1)
                    {
                        values = new object[1][] {new object[1]};
                        values[0][0] = ParsePrimary(hint, sql);
                    }
                    else
                    {
                        throw new SqlSyntaxErrorException("err for partitionOperand: " + sql);
                    }
                    break;
                }
            }
            hint.PartitionOperand = new Pair<string[], object[][]>(columns, values);
            if (CurrentChar(hint, sql) == ')')
            {
                hint.IncreaseCurrentIndex();
            }
        }

        /// <summary>
        ///     current char is char after '[', after call, current char is char after
        ///     ']'
        /// </summary>
        /// <param name="len">less than 0 for array length unknown</param>
        /// <exception cref="System.SqlSyntaxErrorException" />
        private object[] ParseArray(CobarHint hint, string sql, int len)
        {
            object[] rst = null;
            IList<object> list = null;
            if (len >= 0)
            {
                rst = new object[len];
            }
            else
            {
                list = new List<object>();
            }
            for (var i = 0;; ++i)
            {
                var obj = ParsePrimary(hint, sql);
                if (len >= 0)
                {
                    rst[i] = obj;
                }
                else
                {
                    list.Add(obj);
                }
                switch (CurrentChar(hint, sql))
                {
                    case ']':
                    {
                        hint.IncreaseCurrentIndex();
                        if (len >= 0)
                            return rst;
                        return list.ToArray();
                    }
                    case ',':
                    {
                        hint.IncreaseCurrentIndex();
                        break;
                    }

                    default:
                    {
                        throw new SqlSyntaxErrorException("err for partitionOperand array: " + sql);
                    }
                }
            }
        }

        /// <summary>current char is '[[', after call, current char is char after ']]'</summary>
        /// <exception cref="System.SqlSyntaxErrorException" />
        private object[][] ParseArrayArray(CobarHint hint, string sql, int columnNum)
        {
            if (NextChar(hint, sql) == '[')
            {
                IList<object[]> list = new List<object[]>();
                for (;;)
                {
                    NextChar(hint, sql);
                    list.Add(ParseArray(hint, sql, columnNum));
                    var c = CurrentChar(hint, sql);
                    switch (c)
                    {
                        case ']':
                        {
                            hint.IncreaseCurrentIndex();
                            return list.ToArray();
                        }

                        case ',':
                        {
                            NextChar(hint, sql);
                            break;
                        }

                        default:
                        {
                            throw new SqlSyntaxErrorException("err for partitionOperand array[]: " + sql);
                        }
                    }
                }
            }
            var rst = new[] {new object[columnNum]};
            rst[0] = ParseArray(hint, sql, columnNum);
            return rst;
        }
    }
}