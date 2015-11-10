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

using Tup.Cobar.Parser.Visitor;
using Expr = Tup.Cobar.Parser.Ast.Expression.Expression;

namespace Tup.Cobar.Parser.Ast.Expression.Comparison
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class BetweenAndExpression : TernaryOperatorExpression, ReplacableExpression
    {
        private readonly bool not;

        public BetweenAndExpression(bool not,
            Expr comparee,
            Expr notLessThan,
            Expr notGreaterThan)
            : base(comparee, notLessThan, notGreaterThan)
        {
            this.not = not;
        }

        public virtual bool IsNot()
        {
            return not;
        }

        public override int GetPrecedence()
        {
            return ExpressionConstants.PrecedenceBetweenAnd;
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