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

namespace Tup.Cobar4Net.Route
{
    /// <author>xianmao.hexm</author>
    public sealed class RouteResultsetNode
    {
        public static readonly int DefaultReplicaIndex = -1;

        private readonly string name;

        private readonly int replicaIndex;

        private readonly string statement;

        public RouteResultsetNode(string name, string statement)
            : this(name, DefaultReplicaIndex, statement)
        {
        }

        public RouteResultsetNode(string name, int index, string statement)
        {
            // 数据节点名称
            // 数据源编号
            // 执行的语句
            this.name = name;
            this.replicaIndex = index;
            this.statement = statement;
        }

        public string GetName()
        {
            return name;
        }

        public int GetReplicaIndex()
        {
            return replicaIndex;
        }

        public string GetStatement()
        {
            return statement;
        }

        public override int GetHashCode()
        {
            return name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }
            if (obj is RouteResultsetNode)
            {
                RouteResultsetNode rrn = (RouteResultsetNode)obj;
                if (replicaIndex == rrn.GetReplicaIndex() && Equals(name, rrn.GetName()))
                {
                    return true;
                }
            }
            return false;
        }

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            s.Append(name).Append('.');
            if (replicaIndex < 0)
            {
                s.Append("default");
            }
            else
            {
                s.Append(replicaIndex);
            }
            s.Append('{').Append(statement).Append('}');
            return s.ToString();
        }

        private static bool Equals(string str1, string str2)
        {
            if (str1 == null)
            {
                return str2 == null;
            }
            return str1.Equals(str2);
        }
    }
}