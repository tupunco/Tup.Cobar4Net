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

using Deveel.Math;
using System;
using System.Collections.Generic;
using Tup.Cobar4Net.Parser.Util;

namespace Tup.Cobar4Net.Parser.Ast.Expression.Arithmeic
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public abstract class ArithmeticBinaryOperatorExpression
        : BinaryOperatorExpression, IBinaryOperandCalculator
    {
        protected ArithmeticBinaryOperatorExpression(IExpression leftOprand, IExpression rightOprand, int precedence)
            : base(leftOprand, rightOprand, precedence, true)
        {
        }

        protected override object EvaluationInternal(IDictionary<object, object> parameters)
        {
            object left = LeftOprand.Evaluation(parameters);
            object right = rightOprand.Evaluation(parameters);
            if (left == null || right == null)
            {
                return null;
            }
            if (left == ExpressionConstants.Unevaluatable || right == ExpressionConstants.Unevaluatable)
            {
                return ExpressionConstants.Unevaluatable;
            }
            var pair = ExprEvalUtils.ConvertNum2SameLevel(left, right);
            return ExprEvalUtils.Calculate(this, pair.Key, pair.Value);
        }

        public abstract Number Calculate(int integer1, int integer2);

        public abstract Number Calculate(long long1, long long2);

        public abstract Number Calculate(BigInteger bigint1, BigInteger bigint2);

        public abstract Number Calculate(BigDecimal bigDecimal1, BigDecimal bigDecimal2);
    }
}
