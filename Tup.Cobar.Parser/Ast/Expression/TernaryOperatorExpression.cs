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

namespace Tup.Cobar.Parser.Ast.Expression
{
    /// <summary>an operator with arity of 3</summary>
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public abstract class TernaryOperatorExpression : AbstractExpression
    {
        private readonly Tup.Cobar.Parser.Ast.Expression.Expression first;

        private readonly Tup.Cobar.Parser.Ast.Expression.Expression second;

        private readonly Tup.Cobar.Parser.Ast.Expression.Expression third;

        public TernaryOperatorExpression(Tup.Cobar.Parser.Ast.Expression.Expression first
            , Tup.Cobar.Parser.Ast.Expression.Expression second, Tup.Cobar.Parser.Ast.Expression.Expression
             third)
        {
            this.first = first;
            this.second = second;
            this.third = third;
        }

        public virtual Tup.Cobar.Parser.Ast.Expression.Expression GetFirst()
        {
            return first;
        }

        public virtual Tup.Cobar.Parser.Ast.Expression.Expression GetSecond()
        {
            return second;
        }

        public virtual Tup.Cobar.Parser.Ast.Expression.Expression GetThird()
        {
            return third;
        }

        protected override object EvaluationInternal(IDictionary<Expression, Expression> parameters
            )
        {
            return Unevaluatable;
        }
    }
}