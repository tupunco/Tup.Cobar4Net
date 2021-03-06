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
using Tup.Cobar4Net.Parser.Visitor;

namespace Tup.Cobar4Net.Parser.Ast.Expression.Logical
{
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public class LogicalXORExpression : BinaryOperatorExpression
    {
        public LogicalXORExpression(IExpression left, IExpression right)
            : base(left, right, ExpressionConstants.PrecedenceLogicalXor)
        {
        }

        public override string Operator
        {
            get { return "XOR"; }
        }

        protected override object EvaluationInternal(IDictionary<object, object> parameters)
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
            var b1 = ExprEvalUtils.Obj2bool(left);
            var b2 = ExprEvalUtils.Obj2bool(right);
            return b1 != b2 ? LiteralBoolean.True : LiteralBoolean.False;
        }

        public override void Accept(ISqlAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}