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
using Sharpen;
using Tup.Cobar.Parser.Visitor;

namespace Tup.Cobar.Parser.Ast.Expression.Arithmeic
{
	/// <summary><code>higherExpr 'DIV' higherExpr</code></summary>
	/// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
	public class ArithmeticIntegerDivideExpression : ArithmeticBinaryOperatorExpression
	{
		public ArithmeticIntegerDivideExpression(Tup.Cobar.Parser.Ast.Expression.Expression
			 leftOprand, Tup.Cobar.Parser.Ast.Expression.Expression rightOprand)
			: base(leftOprand, rightOprand, ExpressionConstants.PrecedenceArithmeticFactorOp)
		{
		}

		public override string GetOperator()
		{
			return "DIV";
		}

		public override void Accept(SQLASTVisitor visitor)
		{
			visitor.Visit(this);
		}

		public override int Calculate(int integer1, int integer2)
		{
			throw new NotSupportedException();
		}

		public override long Calculate(long long1, long long2)
		{
			throw new NotSupportedException();
		}

		//public override Number Calculate(BigInteger bigint1, BigInteger bigint2)
		//{
		//	throw new NotSupportedException();
		//}

		public override double Calculate(double bigDecimal1, double bigDecimal2)
		{
			throw new NotSupportedException();
		}
	}
}
