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
using Expr = Tup.Cobar4Net.Parser.Ast.Expression.Expression;

namespace Tup.Cobar4Net.Parser.Ast.Expression.Comparison
{
    /// <summary><code>higherPreExpr (NOT)? IN ( '(' expr (',' expr)* ')' | subquery )</code>
    /// 	</summary>
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class InExpression : BinaryOperatorExpression, ReplacableExpression
    {
        private readonly bool not;

        /// <param name="rightOprand">
        ///
        /// <see cref="Tup.Cobar4Net.Parser.Ast.Expression.Misc.QueryExpression"/>
        /// or
        /// <see cref="Tup.Cobar4Net.Parser.Ast.Expression.Misc.InExpressionList"/>
        /// </param>
        public InExpression(bool not, Expr leftOprand, Expr rightOprand)
            : base(leftOprand, rightOprand, ExpressionConstants.PrecedenceComparision)
        {
            this.not = not;
        }

        public virtual bool IsNot()
        {
            return not;
        }

        public virtual InExpressionList GetInExpressionList()
        {
            if (rightOprand is InExpressionList)
            {
                return (InExpressionList)rightOprand;
            }
            return null;
        }

        public virtual QueryExpression GetQueryExpression()
        {
            if (rightOprand is QueryExpression)
            {
                return (QueryExpression)rightOprand;
            }
            return null;
        }

        public override string GetOperator()
        {
            return not ? "NOT IN" : "IN";
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