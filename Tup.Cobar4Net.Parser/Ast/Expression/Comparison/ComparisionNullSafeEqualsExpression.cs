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
using Tup.Cobar4Net.Parser.Ast.Expression.Primary.Literal;
using Tup.Cobar4Net.Parser.Util;
using Tup.Cobar4Net.Parser.Visitor;
using Expr = Tup.Cobar4Net.Parser.Ast.Expression.Expression;

namespace Tup.Cobar4Net.Parser.Ast.Expression.Comparison
{
    /// <summary><code>higherPreExpr '<=>' higherPreExpr</code></summary>
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class ComparisionNullSafeEqualsExpression : BinaryOperatorExpression, ReplacableExpression
    {
        public ComparisionNullSafeEqualsExpression(Expr leftOprand, Expr rightOprand)
            : base(leftOprand, rightOprand, ExpressionConstants.PrecedenceComparision)
        {
        }

        public override string GetOperator()
        {
            return "<=>";
        }

        protected override object EvaluationInternal(IDictionary<object, Expression> parameters)
        {
            object left = leftOprand.Evaluation(parameters);
            object right = rightOprand.Evaluation(parameters);
            if (left == Unevaluatable || right == Unevaluatable)
            {
                return Unevaluatable;
            }
            if (left == null)
            {
                return right == null ? LiteralBoolean.True : LiteralBoolean.False;
            }
            if (right == null)
            {
                return LiteralBoolean.False;
            }
            if (left is Number || right is Number)
            {
                var pair = ExprEvalUtils.ConvertNum2SameLevel(left, right);
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