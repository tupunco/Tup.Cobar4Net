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
 * (created at 2011-4-13)
 */

using System;
using System.Collections.Generic;

namespace Tup.Cobar.SqlParser.Ast.Expression
{
    /**
     * an operator with arity of 3<br/>
     * left conbine in default
     * 
     * @author <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
     */
    public abstract class BinaryOperatorExpression : AbstractExpression
    {
        protected Expression leftOprand;
        protected Expression rightOprand;
        protected int precedence;
        protected bool leftCombine;

        /**
         * {@link #leftCombine} is true
         */
        protected BinaryOperatorExpression(Expression leftOprand, Expression rightOprand, int precedence)
        {
            this.leftOprand = leftOprand;
            this.rightOprand = rightOprand;
            this.precedence = precedence;
            leftCombine = true;
        }

        protected BinaryOperatorExpression(Expression leftOprand, Expression rightOprand, int precedence,
                                       bool leftCombine)
        {
            this.leftOprand = leftOprand;
            this.rightOprand = rightOprand;
            this.precedence = precedence;
            this.leftCombine = leftCombine;
        }

        public Expression getLeftOprand()
        {
            return leftOprand;
        }

        public Expression getRightOprand()
        {
            return rightOprand;
        }

        public override int getPrecedence()
        {
            return precedence;
        }

        public bool isLeftCombine()
        {
            return leftCombine;
        }

        public abstract String getOperator();

        protected override object evaluationInternal(IDictionary<Expression, Expression> parameters)
        {
            return UNEVALUATABLE;
        }
    }
}