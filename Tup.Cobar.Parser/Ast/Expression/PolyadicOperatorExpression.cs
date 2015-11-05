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

namespace Tup.Cobar.Parser.Ast.Expression
{
    /// <summary>
    /// an operator with arity of n<br/>
    /// associative and commutative<br/>
    /// non-polyadic operator with same precedence is not exist
    /// </summary>
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public abstract class PolyadicOperatorExpression : AbstractExpression
    {
        protected internal IList<Tup.Cobar.Parser.Ast.Expression.Expression> operands;

        protected internal readonly int precedence;

        public PolyadicOperatorExpression(int precedence)
            : this(precedence, true)
        {
        }

        public PolyadicOperatorExpression(int precedence, bool leftCombine)
            : this(precedence, 4)
        {
        }

        public PolyadicOperatorExpression(int precedence, int initArity)
        {
            this.precedence = precedence;
            this.operands = new List<Tup.Cobar.Parser.Ast.Expression.Expression>(initArity);
        }

        /// <returns>this</returns>
        public virtual Tup.Cobar.Parser.Ast.Expression.PolyadicOperatorExpression AppendOperand
            (Tup.Cobar.Parser.Ast.Expression.Expression operand)
        {
            if (operand == null)
            {
                return this;
            }
            if (GetType().IsAssignableFrom(operand.GetType()))
            {
                Tup.Cobar.Parser.Ast.Expression.PolyadicOperatorExpression sub = (Tup.Cobar.Parser.Ast.Expression.PolyadicOperatorExpression
                    )operand;
                operands.AddRange(sub.operands);
            }
            else
            {
                operands.Add(operand);
            }
            return this;
        }

        /// <param name="index">start from 0</param>
        public virtual Tup.Cobar.Parser.Ast.Expression.Expression GetOperand(int index)
        {
            if (index >= operands.Count)
            {
                throw new ArgumentException("only contains " + operands.Count + " operands," + index
                     + " is out of bound");
            }
            return operands[index];
        }

        public virtual int GetArity()
        {
            return operands.Count;
        }

        public override int GetPrecedence()
        {
            return precedence;
        }

        public abstract string GetOperator();

        protected override object EvaluationInternal(IDictionary<Expression, Expression> parameters
            )
        {
            return Unevaluatable;
        }
    }
}