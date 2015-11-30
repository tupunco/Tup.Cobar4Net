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
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    /// <author>xianmao.hexm</author>
    public sealed class TableConfig
    {
        private readonly ICollection<string> _columnIndex;

        public TableConfig(string name, string dataNode, TableRuleConfig rule, bool ruleRequired)
        {
            Name = name;
            DataNodes = SplitUtil.Split(dataNode, ',', '$', '-', '[', ']');
            if (DataNodes == null || DataNodes.Length <= 0)
            {
                throw new ArgumentException("invalid table _dataNodes: " + dataNode);
            }
            Rule = rule;
            _columnIndex = BuildColumnIndex(rule);
            IsRuleRequired = ruleRequired;
        }

        public string Name { get; }

        public string[] DataNodes { get; }

        public bool IsRuleRequired { get; }

        public TableRuleConfig Rule { get; }

        public bool ExistsColumn(string columnNameUp)
        {
            return _columnIndex.Contains(columnNameUp);
        }

        private static ICollection<string> BuildColumnIndex(TableRuleConfig rule)
        {
            if (rule == null)
                return new HashSet<string>();

            var rs = rule.Rules;
            if (rs == null || rs.Length <= 0)
                return new HashSet<string>();

            ICollection<string> columnIndex = new HashSet<string>();
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
    }
}