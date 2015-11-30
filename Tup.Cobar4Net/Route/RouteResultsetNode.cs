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

        public RouteResultsetNode(string name, string statement)
            : this(name, DefaultReplicaIndex, statement)
        {
        }

        public RouteResultsetNode(string name, int index, string statement)
        {
            // 数据节点名称
            // 数据源编号
            // 执行的语句
            Name = name;
            ReplicaIndex = index;
            Statement = statement;
        }

        public string Name { get; }

        public int ReplicaIndex { get; }

        public string Statement { get; }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }
            if (obj is RouteResultsetNode)
            {
                var rrn = (RouteResultsetNode)obj;
                if (ReplicaIndex == rrn.ReplicaIndex && Equals(Name, rrn.Name))
                {
                    return true;
                }
            }
            return false;
        }

        public override string ToString()
        {
            var s = new StringBuilder();
            s.Append(Name).Append('.');
            if (ReplicaIndex < 0)
            {
                s.Append("default");
            }
            else
            {
                s.Append(ReplicaIndex);
            }
            s.Append('{').Append(Statement).Append('}');
            return s.ToString();
        }

        private static bool Equals(string str1, string str2)
        {
            if (str1 == null)
                return str2 == null;

            return str1.Equals(str2);
        }
    }
}