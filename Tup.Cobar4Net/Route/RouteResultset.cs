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
using Tup.Cobar4Net.Util;

namespace Tup.Cobar4Net.Route
{
    /// <author>xianmao.hexm</author>
    public sealed class RouteResultset
    {
        public const int SumFlag = 1;

        public const int MinFlag = 2;

        public const int MaxFlag = 3;

        public const int RewriteField = 4;

        public RouteResultset(string stmt)
        {
            Statement = stmt;
            LimitSize = -1;
        }

        public string Statement { get; }

        public RouteResultsetNode[] Nodes { get; set; }

        public int Flag { get; set; }

        /// <value>-1 if no limit</value>
        public long LimitSize { get; set; }

        public override string ToString()
        {
            var s = new StringBuilder();
            s.Append(Statement).Append(", route={");
            if (Nodes != null)
            {
                for (var i = 0; i < Nodes.Length; ++i)
                {
                    s.Append("\n ").Append(FormatUtil.Format(i + 1, 3));
                    s.Append(" -> ").Append(Nodes[i]);
                }
            }
            s.Append("\n}");
            return s.ToString();
        }
    }
}