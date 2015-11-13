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
using Tup.Cobar.Parser.Ast.Expression.Primary.Literal;
using Tup.Cobar.Parser.Util;
using Tup.Cobar.Parser.Visitor;

namespace Tup.Cobar.Parser.Ast.Expression.Logical
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class LogicalXORExpression : BinaryOperatorExpression
    {
        public LogicalXORExpression(Expression left, Expression right)
            : base(left, right, ExpressionConstants.PrecedenceLogicalXor)
        {
        }

        public override string GetOperator()
        {
            return "XOR";
        }

        protected override object EvaluationInternal(IDictionary<object, Expression> parameters
            )
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
            bool b1 = ExprEvalUtils.Obj2bool(left);
            bool b2 = ExprEvalUtils.Obj2bool(right);
            return b1 != b2 ? LiteralBoolean.True : LiteralBoolean.False;
        }

        public override void Accept(SQLASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}