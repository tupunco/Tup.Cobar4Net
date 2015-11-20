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
using Tup.Cobar4Net.Config.Model.Rule;
using Tup.Cobar4Net.Util;

namespace Tup.Cobar4Net.Config.Model
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class TableConfig
    {
        private readonly string name;

        private readonly string[] dataNodes;

        private readonly TableRuleConfig rule;

        private readonly ICollection<string> columnIndex;

        private readonly bool ruleRequired;

        public TableConfig(string name, string dataNode, TableRuleConfig rule, bool ruleRequired)
        {
            if (name == null)
            {
                throw new ArgumentException("table name is null");
            }
            this.name = name.ToUpper();
            this.dataNodes = SplitUtil.Split(dataNode, ',', '$', '-', '[', ']');
            if (this.dataNodes == null || this.dataNodes.Length <= 0)
            {
                throw new ArgumentException("invalid table dataNodes: " + dataNode);
            }
            this.rule = rule;
            this.columnIndex = BuildColumnIndex(rule);
            this.ruleRequired = ruleRequired;
        }

        public virtual bool ExistsColumn(string columnNameUp)
        {
            return columnIndex.Contains(columnNameUp);
        }

        /// <returns>upper-case</returns>
        public virtual string GetName()
        {
            return name;
        }

        public virtual string[] GetDataNodes()
        {
            return dataNodes;
        }

        public virtual bool IsRuleRequired()
        {
            return ruleRequired;
        }

        public virtual TableRuleConfig GetRule()
        {
            return rule;
        }

        private static ICollection<string> BuildColumnIndex(TableRuleConfig rule)
        {
            if (rule == null)
            {
                return new HashSet<string>();
            }
            IList<RuleConfig> rs = rule.GetRules();
            if (rs == null || rs.IsEmpty())
            {
                return new HashSet<string>();
            }
            var columnIndex = new HashSet<string>();
            foreach (RuleConfig r in rs)
            {
                IList<string> columns = r.GetColumns();
                if (columns != null)
                {
                    foreach (string col in columns)
                    {
                        if (col != null)
                        {
                            columnIndex.Add(col.ToUpper());
                        }
                    }
                }
            }
            return columnIndex;
        }
    }
}
