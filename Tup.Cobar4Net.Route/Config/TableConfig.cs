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
using Tup.Cobar4Net.Util;

namespace Tup.Cobar4Net.Route.Config
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    /// <author>xianmao.hexm</author>
    public sealed class TableConfig
    {
        private readonly string name;

        private readonly string[] dataNodes;

        private readonly TableRuleConfig rule;

        private readonly ICollection<string> columnIndex;

        private readonly bool ruleRequired;

        public TableConfig(string name, string dataNode, TableRuleConfig rule, bool ruleRequired)
        {
            this.name = name;
            this.dataNodes = SplitUtil.Split(dataNode, ',', '$', '-', '[', ']');
            if (this.dataNodes == null || this.dataNodes.Length <= 0)
            {
                throw new ArgumentException("invalid table dataNodes: " + dataNode);
            }
            this.rule = rule;
            this.columnIndex = BuildColumnIndex(rule);
            this.ruleRequired = ruleRequired;
        }

        public bool ExistsColumn(string columnNameUp)
        {
            return columnIndex.Contains(columnNameUp);
        }

        public string GetName()
        {
            return name;
        }

        public string[] GetDataNodes()
        {
            return dataNodes;
        }

        public bool IsRuleRequired()
        {
            return ruleRequired;
        }

        public TableRuleConfig GetRule()
        {
            return rule;
        }

        private static ICollection<string> BuildColumnIndex(TableRuleConfig rule)
        {
            if (rule == null)
            {
                return new HashSet<string>();
            }
            TableRuleConfig.RuleConfig[] rs = rule.GetRules();
            if (rs == null || rs.Length <= 0)
            {
                return new HashSet<string>();
            }
            ICollection<string> columnIndex = new HashSet<string>();
            foreach (TableRuleConfig.RuleConfig r in rs)
            {
                string[] columns = r.GetColumns();
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