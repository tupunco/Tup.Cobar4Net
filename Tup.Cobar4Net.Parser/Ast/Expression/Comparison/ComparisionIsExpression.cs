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
using Tup.Cobar4Net.Parser.Visitor;
using Expr = Tup.Cobar4Net.Parser.Ast.Expression.Expression;

namespace Tup.Cobar4Net.Parser.Ast.Expression.Comparison
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class ComparisionIsExpression : AbstractExpression, ReplacableExpression
    {
        public const int IsNull = 1;

        public const int IsTrue = 2;

        public const int IsFalse = 3;

        public const int IsUnknown = 4;

        public const int IsNotNull = 5;

        public const int IsNotTrue = 6;

        public const int IsNotFalse = 7;

        public const int IsNotUnknown = 8;

        private readonly int mode;

        private readonly Expr operand;

        /// <param name="mode">
        ///
        /// <see cref="IsNull"/>
        /// or
        /// <see cref="IsTrue"/>
        /// or
        /// <see cref="IsFalse"/>
        /// or
        /// <see cref="IsUnknown"/>
        /// or
        /// <see cref="IsNotNull"/>
        /// or
        /// <see cref="IsNotTrue"/>
        /// or
        /// <see cref="IsNotFalse"/>
        /// or
        /// <see cref="IsNotUnknown"/>
        /// </param>
        public ComparisionIsExpression(Expr operand, int mode)
        {
            this.operand = operand;
            this.mode = mode;
        }

        public virtual int GetMode()
        {
            return mode;
        }

        public virtual Expr GetOperand()
        {
            return operand;
        }

        public override int GetPrecedence()
        {
            return ExpressionConstants.PrecedenceComparision;
        }

        protected override object EvaluationInternal(IDictionary<object, Expression> parameters)
        {
            return Unevaluatable;
        }

        private Expr replaceExpr;

        public virtual void SetReplaceExpr(Expr replaceExpr)
        {
            this.replaceExpr = replaceExpr;
        }

        public virtual void ClearReplaceExpr()
        {
            this.replaceExpr = null;
        }

        public override void Accept(SQLASTVisitor visitor)
        {
            if (replaceExpr == null)
            {
                visitor.Visit(this);
            }
            else
            {
                replaceExpr.Accept(visitor);
            }
        }
    }
}