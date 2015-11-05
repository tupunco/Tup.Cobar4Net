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
using Tup.Cobar.Parser.Ast.Expression.Primary.Literal;
using Tup.Cobar.Parser.Visitor;

namespace Tup.Cobar.Parser.Ast.Expression.Primary.Function.Datetime
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class Timestampdiff : FunctionExpression
    {
        private IntervalPrimary.Unit unit;

        public Timestampdiff(IntervalPrimary.Unit unit, IList<Tup.Cobar.Parser.Ast.Expression.Expression
            > arguments)
            : base("TIMESTAMPDIFF", arguments)
        {
            this.unit = unit;
        }

        public virtual IntervalPrimary.Unit GetUnit()
        {
            return unit;
        }

        public override FunctionExpression ConstructFunction(IList<Tup.Cobar.Parser.Ast.Expression.Expression
            > arguments)
        {
            throw new NotSupportedException("function of Timestampdiff has special arguments"
                );
        }

        public override void Accept(SQLASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}