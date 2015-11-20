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

        private readonly string statement;

        private RouteResultsetNode[] nodes;

        private int flag;

        private long limitSize;

        public RouteResultset(string stmt)
        {
            // 原始语句
            // 路由结果节点
            // 结果集的处理标识，比如：合并，相加等。
            this.statement = stmt;
            this.limitSize = -1;
        }

        public string GetStatement()
        {
            return statement;
        }

        public RouteResultsetNode[] GetNodes()
        {
            return nodes;
        }

        public void SetNodes(RouteResultsetNode[] nodes)
        {
            this.nodes = nodes;
        }

        public int GetFlag()
        {
            return flag;
        }

        public void SetFlag(int flag)
        {
            this.flag = flag;
        }

        /// <returns>-1 if no limit</returns>
        public long GetLimitSize()
        {
            return limitSize;
        }

        public void SetLimitSize(long limitSize)
        {
            this.limitSize = limitSize;
        }

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            s.Append(statement).Append(", route={");
            if (nodes != null)
            {
                for (int i = 0; i < nodes.Length; ++i)
                {
                    s.Append("\n ").Append(FormatUtil.Format(i + 1, 3));
                    s.Append(" -> ").Append(nodes[i]);
                }
            }
            s.Append("\n}");
            return s.ToString();
        }
    }
}