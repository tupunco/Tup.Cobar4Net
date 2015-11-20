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
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class LogicalOrExpression : PolyadicOperatorExpression
    {
        public LogicalOrExpression()
            : base(ExpressionConstants.PrecedenceLogicalOr)
        {
        }

        public override string GetOperator()
        {
            return "OR";
        }

        protected override object EvaluationInternal(IDictionary<object, object> parameters)
        {
            foreach (Expression operand in operands)
            {
                object val = operand.Evaluation(parameters);
                if (val == null)
                {
                    return null;
                }
                if (val == ExpressionConstants.Unevaluatable)
                {
                    return ExpressionConstants.Unevaluatable;
                }
                if (ExprEvalUtils.Obj2bool(val))
                {
                    return LiteralBoolean.True;
                }
            }
            return LiteralBoolean.False;
        }

        public override void Accept(SQLASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}