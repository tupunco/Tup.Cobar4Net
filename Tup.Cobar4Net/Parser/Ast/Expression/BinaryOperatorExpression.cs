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

namespace Tup.Cobar4Net.Parser.Ast.Expression
{
    /// <summary>
    ///     an operator with arity of 3<br />
    ///     left conbine in default
    /// </summary>
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public abstract class BinaryOperatorExpression : AbstractExpression
    {
        protected readonly bool leftCombine;

        protected readonly IExpression leftOprand;

        protected readonly int precedence;

        protected readonly IExpression rightOprand;

        /// <summary>
        ///     <see cref="leftCombine" />
        ///     is true
        /// </summary>
        protected BinaryOperatorExpression(IExpression leftOprand,
            IExpression rightOprand,
            int precedence)
        {
            this.leftOprand = leftOprand;
            this.rightOprand = rightOprand;
            this.precedence = precedence;
            leftCombine = true;
        }

        protected BinaryOperatorExpression(IExpression leftOprand,
            IExpression rightOprand,
            int precedence,
            bool leftCombine)
        {
            this.leftOprand = leftOprand;
            this.rightOprand = rightOprand;
            this.precedence = precedence;
            this.leftCombine = leftCombine;
        }

        public virtual IExpression LeftOprand
        {
            get { return leftOprand; }
        }

        public virtual IExpression RightOprand
        {
            get { return rightOprand; }
        }

        public override int Precedence
        {
            get { return precedence; }
        }

        public virtual bool IsLeftCombine
        {
            get { return leftCombine; }
        }

        public abstract string Operator { get; }

        protected override object EvaluationInternal(IDictionary<object, object> parameters)
        {
            return ExpressionConstants.Unevaluatable;
        }
    }
}