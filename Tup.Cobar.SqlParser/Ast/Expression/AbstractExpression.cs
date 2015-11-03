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
* (created at 2011-7-25)
*/

using System;
using System.Collections.Generic;

using Tup.Cobar.SqlParser.Visitor;

namespace Tup.Cobar.SqlParser.Ast.Expression
{
    /**
     * @author <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
     */
    public abstract class AbstractExpression : Expression
    {
        public const int PRECEDENCE_QUERY = 0;
        public const int PRECEDENCE_ASSIGNMENT = 1;
        public const int PRECEDENCE_LOGICAL_OR = 2;
        public const int PRECEDENCE_LOGICAL_XOR = 3;
        public const int PRECEDENCE_LOGICAL_AND = 4;
        public const int PRECEDENCE_LOGICAL_NOT = 5;
        public const int PRECEDENCE_BETWEEN_AND = 6;
        public const int PRECEDENCE_COMPARISION = 7;
        public const int PRECEDENCE_ANY_ALL_SUBQUERY = 8;
        public const int PRECEDENCE_BIT_OR = 8;
        public const int PRECEDENCE_BIT_AND = 10;
        public const int PRECEDENCE_BIT_SHIFT = 11;
        public const int PRECEDENCE_ARITHMETIC_TERM_OP = 12;
        public const int PRECEDENCE_ARITHMETIC_FACTOR_OP = 13;
        public const int PRECEDENCE_BIT_XOR = 14;
        public const int PRECEDENCE_UNARY_OP = 15;
        public const int PRECEDENCE_BINARY = 16;
        public const int PRECEDENCE_COLLATE = 17;
        public const int PRECEDENCE_PRIMARY = 19;

        private bool cacheEvalRst = true;
        private bool evaluated;
        private object evaluationCache;

        public virtual Expression setCacheEvalRst(bool cacheEvalRst)
        {
            this.cacheEvalRst = cacheEvalRst;
            return this;
        }

        public object evaluation(IDictionary<Expression, Expression> parameters)
        {
            if (cacheEvalRst)
            {
                if (evaluated)
                {
                    return evaluationCache;
                }
                evaluationCache = evaluationInternal(parameters);
                evaluated = true;
                return evaluationCache;
            }
            return evaluationInternal(parameters);
        }

        protected abstract object evaluationInternal(IDictionary<Expression, Expression> parameters);

        public abstract int getPrecedence();

        public abstract void accept(SQLASTVisitor visitor);

        #region Unevaluatable
        /// <summary>
        /// Unevaluatable Expression
        /// </summary>
        protected Expression UNEVALUATABLE = new UnevaluatableExpression();

        /// <summary>
        /// Unevaluatable Expression
        /// </summary>
        private class UnevaluatableExpression : AbstractExpression
        {
            public override void accept(SQLASTVisitor visitor)
            {
                throw new NotImplementedException();
            }

            public override int getPrecedence()
            {
                throw new NotImplementedException();
            }

            protected override object evaluationInternal(IDictionary<Expression, Expression> parameters)
            {
                throw new NotImplementedException();
            }
        }
        #endregion
    }
}