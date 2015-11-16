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
    public class StraightJoin : TableReference
    {
        private readonly TableReference leftTableRef;

        private readonly TableReference rightTableRef;

        private Tup.Cobar4Net.Parser.Ast.Expression.Expression onCond;

        public StraightJoin(TableReference leftTableRef,
            TableReference rightTableRef,
            Tup.Cobar4Net.Parser.Ast.Expression.Expression onCond)
            : base()
        {
            this.leftTableRef = leftTableRef;
            this.rightTableRef = rightTableRef;
            this.onCond = onCond;
        }

        public StraightJoin(TableReference leftTableRef,
            TableReference rightTableRef)
            : this(leftTableRef, rightTableRef, null)
        {
        }

        public virtual TableReference GetLeftTableRef()
        {
            return leftTableRef;
        }

        public virtual TableReference GetRightTableRef()
        {
            return rightTableRef;
        }

        public virtual Tup.Cobar4Net.Parser.Ast.Expression.Expression GetOnCond()
        {
            return onCond;
        }

        public override object RemoveLastConditionElement()
        {
            if (onCond != null)
            {
                object obj = onCond;
                onCond = null;
                return obj;
            }
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