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
using Tup.Cobar.Parser.Util;
using Tup.Cobar.Parser.Visitor;

namespace Tup.Cobar.Parser.Ast.Expression.Primary
{
    /// <summary><code>'CASE' value? ('WHEN' condition 'THEN' result)+ ('ELSE' result)? 'END' </code>
    /// 	</summary>
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class CaseWhenOperatorExpression : PrimaryExpression
    {
        private readonly Expression comparee;

        private readonly IList<Pair<Expression, Expression>> whenList;

        private readonly Expression elseResult;

        /// <param name="whenList">never null or empry; no pair contains null key or value</param>
        /// <param name="comparee">
        /// null for format of <code>CASE WHEN ...</code>, otherwise,
        /// <code>CASE comparee WHEN ...</code>
        /// </param>
        public CaseWhenOperatorExpression(Expression comparee,
            IList<Pair<Expression, Expression>> whenList,
            Expression elseResult)
            : base()
        {
            this.comparee = comparee;
            if (whenList is List<Pair<Expression, Expression>>)
            {
                this.whenList = whenList;
            }
            else
            {
                this.whenList = new List<Pair<Expression, Expression>>(whenList);
            }
            this.elseResult = elseResult;
        }

        public virtual Expression GetComparee()
        {
            return comparee;
        }

        /// <returns>never null or empty; no pair contains null key or value</returns>
        public virtual IList<Pair<Expression, Expression
            >> GetWhenList()
        {
            return whenList;
        }

        public virtual Expression GetElseResult()
        {
            return elseResult;
        }

        public override void Accept(SQLASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}