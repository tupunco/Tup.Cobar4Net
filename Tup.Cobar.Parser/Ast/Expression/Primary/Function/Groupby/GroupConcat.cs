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

using System;
using System.Collections.Generic;
using Tup.Cobar.Parser.Visitor;

namespace Tup.Cobar.Parser.Ast.Expression.Primary.Function.Groupby
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class GroupConcat : FunctionExpression
    {
        private readonly bool distinct;

        private readonly Expression orderBy;

        private readonly bool isDesc;

        private readonly IList<Expression> appendedColumnNames;

        private readonly string separator;

        public GroupConcat(bool distinct, IList<Expression> exprList, Expression orderBy, bool isDesc, IList
            <Tup.Cobar.Parser.Ast.Expression.Expression> appendedColumnNames, string separator
            )
            : base("GROUP_CONCAT", exprList)
        {
            this.distinct = distinct;
            this.orderBy = orderBy;
            this.isDesc = isDesc;
            if (appendedColumnNames == null || appendedColumnNames.IsEmpty())
            {
                this.appendedColumnNames = new List<Expression>(0);
            }
            else
            {
                if (appendedColumnNames is List<Expression>)
                {
                    this.appendedColumnNames = appendedColumnNames;
                }
                else
                {
                    this.appendedColumnNames = new List<Tup.Cobar.Parser.Ast.Expression.Expression>(
                        appendedColumnNames);
                }
            }
            this.separator = separator == null ? "," : separator;
        }

        public virtual bool IsDistinct()
        {
            return distinct;
        }

        public virtual Expression GetOrderBy()
        {
            return orderBy;
        }

        public virtual bool IsDesc()
        {
            return isDesc;
        }

        public virtual IList<Expression> GetAppendedColumnNames
            ()
        {
            return appendedColumnNames;
        }

        public virtual string GetSeparator()
        {
            return separator;
        }

        public override FunctionExpression ConstructFunction(IList<Expression> arguments)
        {
            throw new NotSupportedException("function of char has special arguments");
        }

        public override void Accept(SQLASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}