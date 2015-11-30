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
using Tup.Cobar4Net.Parser.Ast.Expression;

namespace Tup.Cobar4Net.Route.Config
{
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public sealed class TableRuleConfig
    {
        public TableRuleConfig(string name, RuleConfig[] rules)
        {
            Name = name;
            Rules = rules;

            if (rules != null)
            {
                foreach (var r in rules)
                {
                    r.TableRuleName = name;
                }
            }
        }

        public string Name { get; }

        public RuleConfig[] Rules { get; }
    }

    public sealed class RuleConfig
    {
        /// <summary>upper-case</summary>
        private readonly string[] _columns;

        internal string TableRuleName;

        public RuleConfig(string[] columns, IExpression algorithm)
        {
            _columns = columns ?? new string[0];
            Algorithm = algorithm;
        }

        public string[] Columns
        {
            get { return _columns; }
        }

        public IExpression Algorithm { get; }

        public override string ToString()
        {
            var s = new StringBuilder();
            s.Append("{tableRule:").Append(TableRuleName).Append(", _columns:[");
            for (var i = 0; i < _columns.Length; ++i)
            {
                if (i > 0)
                {
                    s.Append(", ");
                }
                s.Append(_columns[i]);
            }
            s.Append("]}");
            return s.ToString();
        }
    }
}