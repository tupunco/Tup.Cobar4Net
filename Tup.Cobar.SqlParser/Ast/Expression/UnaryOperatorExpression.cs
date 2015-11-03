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

/**
* (created at 2011-1-14)
*/
using System;
using System.Collections.Generic;
using Tup.Cobar.SqlParser.Visitor;

namespace Tup.Cobar.SqlParser.Ast.Expression
{

    /**
     * @author <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
     */
    public abstract class UnaryOperatorExpression : AbstractExpression
    {
        private readonly Expression operand;
        protected readonly int precedence;

        public UnaryOperatorExpression(Expression operand, int precedence)
        {
            if (operand == null)
                throw new ArgumentException("operand is null");

            this.operand = operand;
            this.precedence = precedence;
        }

        public Expression getOperand()
        {
            return operand;
        }

        public override int getPrecedence()
        {
            return precedence;
        }

        public abstract string getOperator();

        protected override object evaluationInternal(IDictionary<Expression, Expression> parameters)
        {
            return UNEVALUATABLE;
        }

        public override void accept(SQLASTVisitor visitor)
        {
            visitor.visit(this);
        }
    }
}
