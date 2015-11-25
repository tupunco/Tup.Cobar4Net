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
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class SchemaConfig
    {
        private readonly string name;

        private readonly string dataNode;

        private readonly string group;

        private readonly IDictionary<string, TableConfig> tables = null;

        private readonly bool noSharding;

        private readonly string[] metaDataNodes;

        private readonly bool keepSqlSchema;

        private readonly ICollection<string> allDataNodes;

        public ICollection<string> AllDataNodes
        {
            get { return allDataNodes; }
        }

        public bool KeepSqlSchema
        {
            get { return keepSqlSchema; }
        }

        public string[] MetaDataNodes
        {
            get { return metaDataNodes; }
        }

        public bool NoSharding
        {
            get { return noSharding; }
        }

        public IDictionary<string, TableConfig> Tables
        {
            get { return tables; }
        }

        public string Group
        {
            get { return group; }
        }

        public string DataNode
        {
            get { return dataNode; }
        }

        public string Name
        {
            get { return name; }
        }

        public SchemaConfig(string name,
            string dataNode,
            string group,
            bool keepSqlSchema,
            IDictionary<string, TableConfig> tables)
        {
            this.name = name;
            this.dataNode = dataNode;
            this.group = group;
            this.tables = tables;
            this.noSharding = tables == null || tables.IsEmpty();
            this.metaDataNodes = BuildMetaDataNodes();
            this.allDataNodes = BuildAllDataNodes();
            this.keepSqlSchema = keepSqlSchema;
        }

        public virtual bool IsKeepSqlSchema()
        {
            return keepSqlSchema;
        }

        public virtual string GetName()
        {
            return name;
        }

        public virtual string GetDataNode()
        {
            return dataNode;
        }

        public virtual string GetGroup()
        {
            return group;
        }

        public virtual IDictionary<string, TableConfig> GetTables()
        {
            return tables;
        }

        public virtual bool IsNoSharding()
        {
            return noSharding;
        }

        public virtual string[] GetMetaDataNodes()
        {
            return metaDataNodes;
        }

        public virtual ICollection<string> GetAllDataNodes()
        {
            return allDataNodes;
        }

        public virtual string GetRandomDataNode()
        {
            if (allDataNodes == null || allDataNodes.IsEmpty())
            {
                return null;
            }
            return allDataNodes.GetEnumerator().Current;
        }

        /// <summary>取得含有不同Meta信息的数据节点,比如表和表结构。</summary>
        private string[] BuildMetaDataNodes()
        {
            var set = new HashSet<string>();
            if (!IsEmpty(dataNode))
            {
                set.Add(dataNode);
            }
            if (!noSharding)
            {
                foreach (TableConfig tc in tables.Values)
                {
                    set.Add(tc.GetDataNodes()[0]);
                }
            }
            return set.ToArray();
        }

        /// <summary>取得该schema的所有数据节点</summary>
        private ICollection<string> BuildAllDataNodes()
        {
            var set = new HashSet<string>();
            if (!IsEmpty(dataNode))
            {
                set.Add(dataNode);
            }
            if (!noSharding)
            {
                foreach (var tc in tables.Values)
                {
                    set.AddRange(tc.GetDataNodes());
                }
            }
            return set;
        }

        private static bool IsEmpty(string str)
        {
            return ((str == null) || (str.Length == 0));
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[SchemaConfig Name={0}, DataNode={1}, Group={2}, Tables={3}, NoSharding={4}, MetaDataNodes={5}, KeepSqlSchema={6}, AllDataNodes={7}]",
                                    name, dataNode, group,
                                    string.Join(",", tables ?? new Dictionary<string, TableConfig>(0)),
                                    noSharding,
                                    string.Join(",", metaDataNodes ?? new string[0]),
                                    keepSqlSchema,
                                    string.Join(",", allDataNodes ?? new string[0]));
        }
    }
}
