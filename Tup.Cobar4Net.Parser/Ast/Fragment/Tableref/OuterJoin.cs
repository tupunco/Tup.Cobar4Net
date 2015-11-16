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
using Tup.Cobar4Net.Parser.Visitor;
using Expr = Tup.Cobar4Net.Parser.Ast.Expression.Expression;

namespace Tup.Cobar4Net.Parser.Ast.Fragment.Tableref
{
    /// <summary>left or right join</summary>
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class OuterJoin : TableReference
    {
        private static IList<string> EnsureListType(IList<string> list)
        {
            if (list == null)
            {
                return null;
            }
            if (list.IsEmpty())
            {
                return new List<string>(0);
            }
            if (list is List<string>)
            {
                return list;
            }
            return new List<string>(list);
        }

        /// <summary>
        /// for MySQL, only <code>LEFT</code> and <code>RIGHT</code> outer join are
        /// supported
        /// </summary>
        private readonly bool isLeftJoin;

        private readonly TableReference leftTableRef;

        private readonly TableReference rightTableRef;

        private readonly Expr onCond;

        private readonly IList<string> @using;

        private OuterJoin(bool isLeftJoin,
            TableReference leftTableRef,
            TableReference rightTableRef,
            Expr onCond,
            IList<string> @using)
        {
            this.isLeftJoin = isLeftJoin;
            this.leftTableRef = leftTableRef;
            this.rightTableRef = rightTableRef;
            this.onCond = onCond;
            this.@using = EnsureListType(@using);
        }

        public OuterJoin(bool isLeftJoin,
            TableReference leftTableRef,
            TableReference rightTableRef,
            Expr onCond)
            : this(isLeftJoin, leftTableRef, rightTableRef, onCond, null)
        {
        }

        public OuterJoin(bool isLeftJoin,
            TableReference leftTableRef,
            TableReference rightTableRef,
            IList<string> @using)
            : this(isLeftJoin, leftTableRef, rightTableRef, null, @using)
        {
        }

        public virtual bool IsLeftJoin()
        {
            return isLeftJoin;
        }

        public virtual TableReference GetLeftTableRef()
        {
            return leftTableRef;
        }

        public virtual TableReference GetRightTableRef()
        {
            return rightTableRef;
        }

        public virtual Expr GetOnCond()
        {
            return onCond;
        }

        public virtual IList<string> GetUsing()
        {
            return @using;
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