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
using Tup.Cobar.Parser.Util;

namespace Tup.Cobar.Parser.Ast.Expression.Arithmeic
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public abstract class ArithmeticBinaryOperatorExpression 
        : BinaryOperatorExpression, BinaryOperandCalculator
    {
        protected internal ArithmeticBinaryOperatorExpression(Expression leftOprand, Expression rightOprand, int precedence)
            : base(leftOprand, rightOprand, precedence, true)
        {
        }

        protected override object EvaluationInternal(IDictionary<object, Expression> parameters)
        {
            object left = leftOprand.Evaluation(parameters);
            object right = rightOprand.Evaluation(parameters);
            if (left == null || right == null)
            {
                return null;
            }
            if (left == Unevaluatable || right == Unevaluatable)
            {
                return Unevaluatable;
            }
            var pair = ExprEvalUtils.ConvertNum2SameLevel(left, right);
            return ExprEvalUtils.Calculate(this, pair.GetKey(), pair.GetValue());
        }

        public abstract int Calculate(int arg1, int arg2);

        public abstract long Calculate(long arg1, long arg2);

        //public abstract Number Calculate(BigInteger arg1, BigInteger arg2);

        public abstract double Calculate(double arg1, double arg2);
    }
}
