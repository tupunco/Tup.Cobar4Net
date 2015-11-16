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
using Tup.Cobar4Net.Parser.Ast.Expression.Primary.Literal;
using Tup.Cobar4Net.Parser.Util;

namespace Tup.Cobar4Net.Parser.Ast.Expression.Logical
{
    /// <summary><code>'NOT' higherExpr</code></summary>
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class LogicalNotExpression : UnaryOperatorExpression
    {
        public LogicalNotExpression(Expression operand)
            : base(operand, ExpressionConstants.PrecedenceLogicalNot)
        {
        }

        public override string GetOperator()
        {
            return "NOT";
        }

        protected override object EvaluationInternal(IDictionary<object, Expression> parameters
            )
        {
            object operand = GetOperand().Evaluation(parameters);
            if (operand == null)
            {
                return null;
            }
            if (operand == Unevaluatable)
            {
                return Unevaluatable;
            }
            bool @bool = ExprEvalUtils.Obj2bool(operand);
            return @bool ? LiteralBoolean.False : LiteralBoolean.True;
        }
    }
}