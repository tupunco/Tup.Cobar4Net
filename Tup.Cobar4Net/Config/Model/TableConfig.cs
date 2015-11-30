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
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public class TableConfig
    {
        private readonly ICollection<string> columnIndex;

        public TableConfig(string name, string dataNode, TableRuleConfig rule, bool ruleRequired)
        {
            if (name == null)
            {
                throw new ArgumentException("table name is null");
            }
            Name = name.ToUpper();
            var dataNodes = SplitUtil.Split(dataNode, ',', '$', '-', '[', ']');
            if (dataNodes == null || dataNodes.Length <= 0)
            {
                throw new ArgumentException("invalid table dataNodes: " + dataNode);
            }
            DataNodes = dataNodes;
            Rule = rule;
            columnIndex = BuildColumnIndex(rule);
            IsRuleRequired = ruleRequired;
        }

        /// <value>upper-case</value>
        public string Name { get; }

        public string[] DataNodes { get; }

        public bool IsRuleRequired { get; }

        public TableRuleConfig Rule { get; }

        public bool ExistsColumn(string columnNameUp)
        {
            return columnIndex.Contains(columnNameUp);
        }

        private static ICollection<string> BuildColumnIndex(TableRuleConfig rule)
        {
            if (rule == null)
            {
                return new HashSet<string>();
            }
            var rs = rule.Rules;
            if (rs == null || rs.IsEmpty())
            {
                return new HashSet<string>();
            }
            var columnIndex = new HashSet<string>();
            foreach (var r in rs)
            {
                var columns = r.Columns;
                if (columns != null)
                {
                    foreach (var col in columns)
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

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[TableConfig Name={0}, DataNodes={1}, Rule={2}, ColumnIndex={3}, RuleRequired={4}]",
                Name, string.Join(",", DataNodes ?? new string[0]),
                Rule, string.Format(",", columnIndex ?? new string[0]),
                IsRuleRequired);
        }
    }
}