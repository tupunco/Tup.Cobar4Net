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
using Tup.Cobar4Net.Parser.Visitor;

namespace Tup.Cobar4Net.Parser.Ast.Expression.Primary.Function.Groupby
{
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public class GroupConcat : FunctionExpression
    {
        public GroupConcat(bool distinct,
                           IList<IExpression> exprList,
                           IExpression orderBy,
                           bool isDesc,
                           IList<IExpression> appendedColumnNames,
                           string separator)
            : base("GROUP_CONCAT", exprList)
        {
            IsDistinct = distinct;
            OrderBy = orderBy;
            IsDesc = isDesc;
            if (appendedColumnNames == null || appendedColumnNames.IsEmpty())
            {
                AppendedColumnNames = new List<IExpression>(0);
            }
            else if (appendedColumnNames is List<IExpression>)
            {
                AppendedColumnNames = appendedColumnNames;
            }
            else
            {
                AppendedColumnNames = new List<IExpression>(
                    appendedColumnNames);
            }

            Separator = separator ?? ",";
        }

        public virtual bool IsDistinct { get; }

        public virtual IExpression OrderBy { get; }

        public virtual bool IsDesc { get; }

        public virtual IList<IExpression> AppendedColumnNames { get; }

        public virtual string Separator { get; }

        public override FunctionExpression ConstructFunction(IList<IExpression> arguments)
        {
            throw new NotSupportedException("function of char has special arguments");
        }

        public override void Accept(ISqlAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}