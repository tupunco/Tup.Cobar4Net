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
using Tup.Cobar4Net.Parser.Visitor;

namespace Tup.Cobar4Net.Parser.Ast.Expression.Arithmeic
{
    /// <summary><code>higherExpr '*' higherExpr</code></summary>
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class ArithmeticMultiplyExpression : ArithmeticBinaryOperatorExpression
    {
        public ArithmeticMultiplyExpression(IExpression leftOprand,
                                            IExpression rightOprand)
            : base(leftOprand, rightOprand, ExpressionConstants.PrecedenceArithmeticFactorOp)
        {
        }

        public override string Operator
        {
            get { return "*"; }
        }

        public override void Accept(ISqlAstVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override Number Calculate(int integer1, int integer2)
        {
            throw new NotSupportedException();
        }

        public override Number Calculate(long long1, long long2)
        {
            throw new NotSupportedException();
        }

        public override Number Calculate(BigInteger bigint1, BigInteger bigint2)
        {
            throw new NotSupportedException();
        }

        public override Number Calculate(BigDecimal bigDecimal1, BigDecimal bigDecimal2)
        {
            throw new NotSupportedException();
        }
    }
}
