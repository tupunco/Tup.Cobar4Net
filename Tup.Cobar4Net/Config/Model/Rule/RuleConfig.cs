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
    public class RuleConfig
    {
        private readonly string algorithm;
        private readonly IList<string> columns;

        private IRuleAlgorithm ruleAlgorithm;

        public RuleConfig(string[] columns, string algorithm)
        {
            if (algorithm == null)
            {
                throw new ArgumentException("algorithm is null");
            }
            this.algorithm = algorithm;
            if (columns == null || columns.Length <= 0)
            {
                throw new ArgumentException("no rule column is found");
            }
            IList<string> list = new List<string>(columns.Length);
            foreach (var column in columns)
            {
                if (column == null)
                {
                    throw new ArgumentException("column value is null: " + columns);
                }
                list.Add(column.ToUpper());
            }
            this.columns = new List<string>(list).AsReadOnly();
        }

        public virtual IRuleAlgorithm RuleAlgorithm
        {
            get { return ruleAlgorithm; }
            set { ruleAlgorithm = value; }
        }

        /// <value>unmodifiable, upper-case</value>
        public virtual IList<string> Columns
        {
            get { return columns; }
        }

        /// <value>never null</value>
        public virtual string Algorithm
        {
            get { return algorithm; }
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[RuleConfig columns:[{0}], algorithm:{1}, ruleAlgorithm:{2}]",
                string.Join(",", columns ?? new string[0]), algorithm, ruleAlgorithm);
        }
    }
}