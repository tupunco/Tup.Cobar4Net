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
using Tup.Cobar4Net.Parser.Ast.Expression.Primary;
using Tup.Cobar4Net.Parser.Visitor;

namespace Tup.Cobar4Net.Parser.Ast.Fragment.Tableref
{
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public class TableRefFactor : AliasableTableReference
    {
        /// <summary>e.g.</summary>
        /// <remarks>e.g. <code>"`db2`.`tb1`"</code> is possible</remarks>
        private readonly Identifier table;

        public TableRefFactor(Identifier table, string alias, IList<IndexHint> hintList)
            : base(alias)
        {
            this.table = table;
            if (hintList == null || hintList.IsEmpty())
            {
                HintList = new List<IndexHint>(0);
            }
            else if (hintList is List<IndexHint>)
            {
                HintList = hintList;
            }
            else
            {
                HintList = new List<IndexHint>(hintList);
            }
        }

        public TableRefFactor(Identifier table, IList<IndexHint> hintList)
            : this(table, null, hintList)
        {
        }

        public virtual Identifier Table
        {
            get { return table; }
        }

        public virtual IList<IndexHint> HintList { get; }

        public override bool IsSingleTable
        {
            get { return true; }
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