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

using Tup.Cobar4Net.Parser.Visitor;

namespace Tup.Cobar4Net.Parser.Ast.Fragment.Tableref
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class NaturalJoin : TableReference
    {
        private readonly bool isOuter;

        /// <summary>
        /// make sense only if
        /// <see cref="isOuter"/>
        /// is true. Eigher <code>LEFT</code> or
        /// <code>RIGHT</code>
        /// </summary>
        private readonly bool isLeft;

        private readonly TableReference leftTableRef;

        private readonly TableReference rightTableRef;

        public NaturalJoin(bool isOuter, bool isLeft,
            TableReference leftTableRef,
            TableReference rightTableRef)
            : base()
        {
            this.isOuter = isOuter;
            this.isLeft = isLeft;
            this.leftTableRef = leftTableRef;
            this.rightTableRef = rightTableRef;
        }

        public virtual bool IsOuter()
        {
            return isOuter;
        }

        public virtual bool IsLeft()
        {
            return isLeft;
        }

        public virtual TableReference GetLeftTableRef()
        {
            return leftTableRef;
        }

        public virtual TableReference GetRightTableRef()
        {
            return rightTableRef;
        }

        public override object RemoveLastConditionElement()
        {
            return null;
        }

        public override bool IsSingleTable()
        {
            return false;
        }

        public override int GetPrecedence()
        {
            return TableReference.PrecedenceJoin;
        }

        public override void Accept(SQLASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}