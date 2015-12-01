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

namespace Tup.Cobar4Net.Parser.Ast.Expression
{
    /// <summary>
    ///     an operator with arity of n<br />
    ///     associative and commutative<br />
    ///     non-polyadic operator with same precedence is not exist
    /// </summary>
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public abstract class PolyadicOperatorExpression : AbstractExpression
    {
        protected readonly int precedence;
        protected IList<IExpression> operands;

        protected PolyadicOperatorExpression(int precedence)
            : this(precedence, true)
        {
        }

        protected PolyadicOperatorExpression(int precedence, bool leftCombine)
            : this(precedence, 4)
        {
        }

        protected PolyadicOperatorExpression(int precedence, int initArity)
        {
            this.precedence = precedence;
            operands = new List<IExpression>(initArity);
        }

        public virtual int Arity
        {
            get { return operands.Count; }
        }

        public override int Precedence
        {
            get { return precedence; }
        }

        public abstract string Operator { get; }

        /// <returns>this</returns>
        public virtual PolyadicOperatorExpression AppendOperand(IExpression operand)
        {
            if (operand == null)
            {
                return this;
            }
            if (GetType().IsAssignableFrom(operand.GetType()))
            {
                var sub = (PolyadicOperatorExpression)operand;
                operands.AddRange(sub.operands);
            }
            else
            {
                operands.Add(operand);
            }
            return this;
        }

        /// <param name="index">start from 0</param>
        public virtual IExpression GetOperand(int index)
        {
            if (index >= operands.Count)
            {
                throw new ArgumentException("only contains " + operands.Count + " operands," + index
                                            + " is out of bound");
            }
            return operands[index];
        }

        protected override object EvaluationInternal(IDictionary<object, object> parameters)
        {
            return ExpressionConstants.Unevaluatable;
        }
    }
}