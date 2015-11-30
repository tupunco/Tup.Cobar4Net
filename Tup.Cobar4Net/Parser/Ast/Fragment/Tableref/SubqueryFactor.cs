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
using Tup.Cobar4Net.Parser.Ast.Expression.Misc;
using Tup.Cobar4Net.Parser.Visitor;

namespace Tup.Cobar4Net.Parser.Ast.Fragment.Tableref
{
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public class SubqueryFactor : AliasableTableReference
    {
        public SubqueryFactor(IQueryExpression subquery, string alias)
            : base(alias)
        {
            if (alias == null)
            {
                throw new ArgumentException("alias is required for _subquery factor");
            }
            if (subquery == null)
            {
                throw new ArgumentException("_subquery is null");
            }
            Subquery = subquery;
        }

        public virtual IQueryExpression Subquery { get; }

        public override bool IsSingleTable
        {
            get { return false; }
        }

        public override int Precedence
        {
            get { return PrecedenceFactor; }
        }

        public override object RemoveLastConditionElement()
        {
            return null;
        }

        public override void Accept(ISqlAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}