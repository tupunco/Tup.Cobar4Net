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

using Sharpen;
using System.Collections.Generic;
using Tup.Cobar.Parser.Ast.Expression.Primary.Literal;
using Tup.Cobar.Parser.Util;
using Tup.Cobar.Parser.Visitor;
using Expr = Tup.Cobar.Parser.Ast.Expression.Expression;

namespace Tup.Cobar.Parser.Ast.Expression.Comparison
{
    /// <summary><code>higherPreExpr '=' higherPreExpr</code></summary>
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class ComparisionEqualsExpression : BinaryOperatorExpression, ReplacableExpression
    {
        public ComparisionEqualsExpression(Expr leftOprand, Expr rightOprand)
            : base(leftOprand, rightOprand, ExpressionConstants.PrecedenceComparision)
        {
        }

        public override string GetOperator()
        {
            return "=";
        }

        public override object Evaluation(IDictionary<object, Expression> parameters)
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
            if (left is Number || right is Number)
            {
                Pair<Number, Number> pair = ExprEvalUtils.ConvertNum2SameLevel(left, right);
                left = pair.GetKey();
                right = pair.GetValue();
            }
            return left.Equals(right) ? LiteralBoolean.True : LiteralBoolean.False;
        }

        private Expr replaceExpr;

        public virtual void SetReplaceExpr(Expr replaceExpr)
        {
            this.replaceExpr = replaceExpr;
        }

        public virtual void ClearReplaceExpr()
        {
            this.replaceExpr = null;
        }

        public override void Accept(SQLASTVisitor visitor)
        {
            if (replaceExpr == null)
            {
                visitor.Visit(this);
            }
            else
            {
                replaceExpr.Accept(visitor);
            }
        }
    }
}