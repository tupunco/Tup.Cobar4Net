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

using System.Collections.Generic;
using System.Linq;

namespace Tup.Cobar4Net.Config.Model
{
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public class SchemaConfig
    {
        public SchemaConfig(string name,
                            string dataNode,
                            string group,
                            bool keepSqlSchema,
                            IDictionary<string, TableConfig> tables)
        {
            Name = name;
            DataNode = dataNode;
            Group = group;
            Tables = tables;
            IsNoSharding = tables == null || tables.IsEmpty();
            MetaDataNodes = BuildMetaDataNodes();
            AllDataNodes = BuildAllDataNodes();
            IsKeepSqlSchema = keepSqlSchema;
        }

        public bool IsKeepSqlSchema { get; }

        public string Name { get; }

        public string DataNode { get; }

        public string Group { get; }

        public IDictionary<string, TableConfig> Tables { get; }

        public bool IsNoSharding { get; }

        public string[] MetaDataNodes { get; }

        public ICollection<string> AllDataNodes { get; }

        public string RandomDataNode
        {
            get
            {
                if (AllDataNodes == null || AllDataNodes.IsEmpty())
                {
                    return null;
                }
                return AllDataNodes.FirstOrDefault();
                //return allDataNodes.GetEnumerator().Current;
            }
        }

        /// <summary>ȡ�ú��в�ͬMeta��Ϣ�����ݽڵ�,�����ͱ�ṹ��</summary>
        private string[] BuildMetaDataNodes()
        {
            var set = new HashSet<string>();
            if (!DataNode.IsEmpty())
            {
                set.Add(DataNode);
            }
            if (!IsNoSharding)
            {
                foreach (var tc in Tables.Values)
                {
                    set.Add(tc.DataNodes[0]);
                }
            }
            return set.ToArray();
        }

        /// <summary>ȡ�ø�schema���������ݽڵ�</summary>
        private ICollection<string> BuildAllDataNodes()
        {
            var set = new HashSet<string>();
            if (!DataNode.IsEmpty())
            {
                set.Add(DataNode);
            }
            if (!IsNoSharding)
            {
                foreach (var tc in Tables.Values)
                {
                    set.AddRange(tc.DataNodes);
                }
            }
            return set;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return
                string.Format(
                    "[SchemaConfig Name={0}, DataNode={1}, Group={2}, Tables={3}, NoSharding={4}, MetaDataNodes={5}, KeepSqlSchema={6}, AllDataNodes={7}]",
                    Name, DataNode, Group,
                    string.Join(",", Tables ?? new Dictionary<string, TableConfig>(0)),
                    IsNoSharding,
                    string.Join(",", MetaDataNodes ?? new string[0]),
                    IsKeepSqlSchema,
                    string.Join(",", AllDataNodes ?? new string[0]));
        }
    }
}