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

namespace Tup.Cobar4Net.Parser.Ast.Expression.Comparison
{
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public class ComparisionIsExpression : AbstractExpression, IReplacableExpression
    {
        public const int IsNull = 1;

        public const int IsTrue = 2;

        public const int IsFalse = 3;

        public const int IsUnknown = 4;

        public const int IsNotNull = 5;

        public const int IsNotTrue = 6;

        public const int IsNotFalse = 7;

        public const int IsNotUnknown = 8;

        private IExpression _replaceExpr;

        /// <param name="mode">
        ///     <see cref="IsNull" />
        ///     or
        ///     <see cref="IsTrue" />
        ///     or
        ///     <see cref="IsFalse" />
        ///     or
        ///     <see cref="IsUnknown" />
        ///     or
        ///     <see cref="IsNotNull" />
        ///     or
        ///     <see cref="IsNotTrue" />
        ///     or
        ///     <see cref="IsNotFalse" />
        ///     or
        ///     <see cref="IsNotUnknown" />
        /// </param>
        public ComparisionIsExpression(IExpression operand, int mode)
        {
            Operand = operand;
            Mode = mode;
        }

        public virtual int Mode { get; }

        public virtual IExpression Operand { get; }

        public override int Precedence
        {
            get { return ExpressionConstants.PrecedenceComparision; }
        }

        public virtual IExpression ReplaceExpr
        {
            set { _replaceExpr = value; }
        }

        public virtual void ClearReplaceExpr()
        {
            _replaceExpr = null;
        }

        public override void Accept(ISqlAstVisitor visitor)
        {
            if (_replaceExpr == null)
            {
                visitor.Visit(this);
            }
            else
            {
                _replaceExpr.Accept(visitor);
            }
        }

        protected override object EvaluationInternal(IDictionary<object, object> parameters)
        {
            return ExpressionConstants.Unevaluatable;
        }
    }
}