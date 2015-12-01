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

using Tup.Cobar4Net.Parser.Ast.Expression.Misc;
using Tup.Cobar4Net.Parser.Visitor;

namespace Tup.Cobar4Net.Parser.Ast.Expression.Comparison
{
    /// <summary>
    ///     <code>higherPreExpr (NOT)? IN ( '(' expr (',' expr)* ')' | subquery )</code>
    /// </summary>
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public class InExpression : BinaryOperatorExpression, IReplacableExpression
    {
        private readonly bool _not;

        private IExpression _replaceExpr;

        /// <param name="rightOprand">
        ///     <see cref="Tup.Cobar4Net.Parser.Ast.Expression.Misc.IQueryExpression" />
        ///     or
        ///     <see cref="Tup.Cobar4Net.Parser.Ast.Expression.Misc.InExpressionList" />
        /// </param>
        public InExpression(bool not, IExpression leftOprand, IExpression rightOprand)
            : base(leftOprand, rightOprand, ExpressionConstants.PrecedenceComparision)
        {
            _not = not;
        }

        public virtual bool IsNot
        {
            get { return _not; }
        }

        public override string Operator
        {
            get { return _not ? "NOT IN" : "IN"; }
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

        public virtual InExpressionList GetInExpressionList()
        {
            if (rightOprand is InExpressionList)
            {
                return (InExpressionList)rightOprand;
            }
            return null;
        }

        public virtual IQueryExpression GetQueryExpression()
        {
            if (rightOprand is IQueryExpression)
            {
                return (IQueryExpression)rightOprand;
            }
            return null;
        }
    }
}