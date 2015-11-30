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

namespace Tup.Cobar4Net.Parser.Ast.Expression.Comparison
{
    /// <summary>
    ///     <code>higherPreExpr '=' higherPreExpr</code>
    /// </summary>
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public class ComparisionEqualsExpression : BinaryOperatorExpression, IReplacableExpression
    {
        private IExpression _replaceExpr;

        public ComparisionEqualsExpression(IExpression leftOprand, IExpression rightOprand)
            : base(leftOprand, rightOprand, ExpressionConstants.PrecedenceComparision)
        {
        }

        public override string Operator
        {
            get { return "="; }
        }

        public override object Evaluation(IDictionary<object, object> parameters)
        {
            var left = LeftOprand.Evaluation(parameters);
            var right = rightOprand.Evaluation(parameters);
            if (left == null || right == null)
            {
                return null;
            }
            if (left == ExpressionConstants.Unevaluatable || right == ExpressionConstants.Unevaluatable)
            {
                return ExpressionConstants.Unevaluatable;
            }
            if (left is Number || right is Number)
            {
                var pair = ExprEvalUtils.ConvertNum2SameLevel(left, right);
                left = pair.Key;
                right = pair.Value;
            }
            return left.Equals(right) ? LiteralBoolean.True : LiteralBoolean.False;
        }

        public virtual IExpression ReplaceExpr
        {
            set { _replaceExpr = value; }
        }

        public virtual void ClearReplaceExpr()
        {
            _replaceExpr = null;
        }

        public override void Accept(ISqlAstVisitor visitor)
        {
            if (_replaceExpr == null)
            {
                visitor.Visit(this);
            }
            else
            {
                _replaceExpr.Accept(visitor);
            }
        }
    }
}