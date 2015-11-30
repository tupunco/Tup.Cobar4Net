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

namespace Tup.Cobar4Net.Config.Model.Rule
{
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public class TableRuleConfig
    {
        private readonly string name;

        private readonly IList<RuleConfig> rules;

        public TableRuleConfig(string name, IList<RuleConfig> rules)
        {
            if (name == null)
            {
                throw new ArgumentException("name is null");
            }
            this.name = name;
            if (rules == null || rules.IsEmpty())
            {
                throw new ArgumentException("no rule is found");
            }
            this.rules = new List<RuleConfig>(rules).AsReadOnly();
        }

        public virtual string Name
        {
            get { return name; }
        }

        /// <value>unmodifiable</value>
        public virtual IList<RuleConfig> Rules
        {
            get { return rules; }
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[TableRuleConfig name:{0}, rules:{1}]",
                name, string.Join(",", rules ?? new RuleConfig[0]));
        }
    }
}