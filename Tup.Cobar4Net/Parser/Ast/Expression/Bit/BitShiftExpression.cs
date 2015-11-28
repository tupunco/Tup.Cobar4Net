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

using Tup.Cobar4Net.Parser.Visitor;

namespace Tup.Cobar4Net.Parser.Ast.Expression.Bit
{
    /// <summary><code>higherExpr ( ('&lt;&lt;'|'&gt;&gt;') higherExpr )+</code></summary>
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class BitShiftExpression : BinaryOperatorExpression
    {
        private readonly bool negative;

        /// <param name="negative">true if right shift</param>
        public BitShiftExpression(bool negative, Expression leftOprand, Expression rightOprand)
            : base(leftOprand, rightOprand, ExpressionConstants.PrecedenceBitShift)
        {
            this.negative = negative;
        }

        public virtual bool IsRightShift()
        {
            return negative;
        }

        public override string GetOperator()
        {
            return negative ? ">>" : "<<";
        }

        public override void Accept(SQLASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}