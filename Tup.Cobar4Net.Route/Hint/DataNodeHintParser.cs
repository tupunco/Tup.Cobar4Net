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
using Tup.Cobar4Net.Parser.Util;

namespace Tup.Cobar4Net.Route.Hint
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public sealed class DataNodeHintParser : HintParser
    {
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        public override void Process(CobarHint hint, string hintName, string sql)
        {
            if (CurrentChar(hint, sql) == '[')
            {
                for (;;)
                {
                    NextChar(hint, sql);
                    Pair<int, int> pair = ParseDataNode(hint, sql);
                    hint.AddDataNode(pair.GetKey(), pair.GetValue());
                    switch (CurrentChar(hint, sql))
                    {
                        case ',':
                            {
                                continue;
                            }

                        case ']':
                            {
                                NextChar(hint, sql);
                                return;
                            }

                        default:
                            {
                                throw new SQLSyntaxErrorException("err for dataNodeId: " + sql);
                            }
                    }
                }
            }
            else
            {
                Pair<int, int> pair = ParseDataNode(hint, sql);
                hint.AddDataNode(pair.GetKey(), pair.GetValue());
            }
        }

        /// <summary>first char is not separator</summary>
        private Pair<int, int> ParseDataNode(CobarHint hint, string sql)
        {
            int start = hint.GetCurrentIndex();
            int ci = start;
            for (; IsDigit(sql[ci]); ++ci)
            {
            }
            int nodeIndex = Convert.ToInt32(Runtime.Substring(sql, start, ci));
            int replica = RouteResultsetNode.DefaultReplicaIndex;
            hint.SetCurrentIndex(ci);
            if (CurrentChar(hint, sql) == '.')
            {
                NextChar(hint, sql);
                start = hint.GetCurrentIndex();
                ci = start;
                for (; IsDigit(sql[ci]); ++ci)
                {
                }
                replica = Convert.ToInt32(Runtime.Substring(sql, start, ci));
                hint.SetCurrentIndex(ci);
            }
            return new Pair<int, int>(nodeIndex, replica);
        }
    }
}