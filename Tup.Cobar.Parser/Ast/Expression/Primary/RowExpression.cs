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

using System.Collections;
using System.Collections.Generic;

using Tup.Cobar.Parser.Visitor;

namespace Tup.Cobar.Parser.Ast.Expression.Primary
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class RowExpression : PrimaryExpression
    {
        private readonly IList<Expression> rowExprList;

        public RowExpression(IList<Expression> rowExprList)
        {
            if (rowExprList == null || rowExprList.IsEmpty())
            {
                this.rowExprList = new List<Expression>(0);
            }
            else
            {
                if (rowExprList is List<Expression>)
                {
                    this.rowExprList = rowExprList;
                }
                else
                {
                    this.rowExprList = new List<Expression>(rowExprList);
                }
            }
        }

        /// <returns>never null</returns>
        public virtual IList<Expression> GetRowExprList()
        {
            return rowExprList;
        }

        public override void Accept(SQLASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}