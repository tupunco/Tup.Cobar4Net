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

using Tup.Cobar4Net.Parser.Ast.Expression;
using Tup.Cobar4Net.Parser.Visitor;

namespace Tup.Cobar4Net.Parser.Ast.Fragment.Tableref
{
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public class StraightJoin : TableReference
    {
        private IExpression onCond;

        public StraightJoin(TableReference leftTableRef,
                            TableReference rightTableRef,
                            IExpression onCond)
        {
            LeftTableRef = leftTableRef;
            RightTableRef = rightTableRef;
            this.onCond = onCond;
        }

        public StraightJoin(TableReference leftTableRef,
                            TableReference rightTableRef)
            : this(leftTableRef, rightTableRef, null)
        {
        }

        public virtual TableReference LeftTableRef { get; }

        public virtual TableReference RightTableRef { get; }

        public virtual IExpression OnCond
        {
            get { return onCond; }
        }

        public override bool IsSingleTable
        {
            get { return false; }
        }

        public override int Precedence
        {
            get { return PrecedenceJoin; }
        }

        public override object RemoveLastConditionElement()
        {
            if (onCond == null)
                return null;

            object obj = onCond;
            onCond = null;
            return obj;
        }

        public override void Accept(ISqlAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}