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
using Expr = Tup.Cobar4Net.Parser.Ast.Expression.Expression;

namespace Tup.Cobar4Net.Route.Config
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public sealed class TableRuleConfig
    {
        private readonly string name;

        private readonly RuleConfig[] rules;

        public TableRuleConfig(string name, RuleConfig[] rules)
        {
            this.name = name;
            this.rules = rules;
            if (rules != null)
            {
                foreach (RuleConfig r in rules)
                {
                    r.tableRuleName = name;
                }
            }
        }

        public string GetName()
        {
            return name;
        }

        public RuleConfig[] GetRules()
        {
            return rules;
        }

        public sealed class RuleConfig
        {
            internal string tableRuleName;

            /// <summary>upper-case</summary>
            private readonly string[] columns;

            private readonly Expr algorithm;

            public RuleConfig(string[] columns, Expr
                 algorithm)
            {
                this.columns = columns == null ? new string[0] : columns;
                this.algorithm = algorithm;
            }

            public string[] GetColumns()
            {
                return columns;
            }

            public Expr GetAlgorithm()
            {
                return algorithm;
            }

            public override string ToString()
            {
                var s = new StringBuilder();
                s.Append("{tableRule:").Append(tableRuleName).Append(", columns:[");
                for (int i = 0; i < columns.Length; ++i)
                {
                    if (i > 0)
                    {
                        s.Append(", ");
                    }
                    s.Append(columns[i]);
                }
                s.Append("]}");
                return s.ToString();
            }
        }
    }
}