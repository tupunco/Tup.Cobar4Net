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
using Deveel.Math;
using Tup.Cobar4Net.Parser.Visitor;

namespace Tup.Cobar4Net.Parser.Ast.Expression.Arithmeic
{
    /// <summary>
    ///     <code>higherExpr ('MOD'|'%') higherExpr</code>
    /// </summary>
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public class ArithmeticModExpression : ArithmeticBinaryOperatorExpression
    {
        public ArithmeticModExpression(IExpression leftOprand,
                                       IExpression rightOprand)
            : base(leftOprand, rightOprand, ExpressionConstants.PrecedenceArithmeticFactorOp)
        {
        }

        public override string Operator
        {
            get { return "%"; }
        }

        public override void Accept(ISqlAstVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override Number Calculate(int integer1, int integer2)
        {
            if (integer1 == 0 && integer2 == 0)
            {
                return 0;
            }
            var i1 = integer1;
            var i2 = integer2;
            if (i2 == 0)
            {
                return 0;
            }
            return i1%i2;
        }

        public override Number Calculate(long long1, long long2)
        {
            if (long1 == 0 && long2 == 0)
            {
                return 0;
            }
            var i1 = long1;
            var i2 = long2;
            if (i2 == 0)
            {
                return 0;
            }
            return i1%i2;
        }

        public override Number Calculate(BigInteger bigint1, BigInteger bigint2)
        {
            if (bigint1 == null || bigint2 == null)
            {
                return 0;
            }
            var comp = bigint2.CompareTo(BigInteger.Zero);
            if (comp == 0)
            {
                return 0;
            }
            if (comp < 0)
            {
                return bigint1.Negate().Mod(bigint2).Negate();
            }
            return bigint1.Mod(bigint2);
        }

        public override Number Calculate(BigDecimal bigDecimal1, BigDecimal bigDecimal2)
        {
            throw new NotSupportedException();
        }
    }
}