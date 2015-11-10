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
using Expr = Tup.Cobar.Parser.Ast.Expression.Expression;

namespace Tup.Cobar.Parser.Ast.Expression
{
    /// <summary>an operator with arity of 3</summary>
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public abstract class TernaryOperatorExpression : AbstractExpression
    {
        private readonly Expr first;

        private readonly Expr second;

        private readonly Expr third;

        public TernaryOperatorExpression(Expr first, Expr second, Expr third)
        {
            this.first = first;
            this.second = second;
            this.third = third;
        }

        public virtual Expr GetFirst()
        {
            return first;
        }

        public virtual Expr GetSecond()
        {
            return second;
        }

        public virtual Expr GetThird()
        {
            return third;
        }

        protected override object EvaluationInternal(IDictionary<object, Expression> parameters)
        {
            return Unevaluatable;
        }
    }
}