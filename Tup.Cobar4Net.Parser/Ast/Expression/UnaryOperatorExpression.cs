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
using Tup.Cobar4Net.Parser.Visitor;
using Expr = Tup.Cobar4Net.Parser.Ast.Expression.Expression;

namespace Tup.Cobar4Net.Parser.Ast.Expression
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public abstract class UnaryOperatorExpression : AbstractExpression
    {
        private readonly Expr operand;

        protected readonly int precedence;

        public UnaryOperatorExpression(Expr operand, int precedence)
        {
            if (operand == null)
            {
                throw new ArgumentException("operand is null");
            }
            this.operand = operand;
            this.precedence = precedence;
        }

        public virtual Expr GetOperand()
        {
            return operand;
        }

        public override int GetPrecedence()
        {
            return precedence;
        }

        public abstract string GetOperator();

        protected override object EvaluationInternal(IDictionary<object, object> parameters)
        {
            return ExpressionConstants.Unevaluatable;
        }

        public override void Accept(SQLASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}