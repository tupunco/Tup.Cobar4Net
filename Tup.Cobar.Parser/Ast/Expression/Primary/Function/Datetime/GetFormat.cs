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

namespace Tup.Cobar.Parser.Ast.Expression.Primary.Function.Datetime
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class GetFormat : FunctionExpression
    {
        public enum FormatType
        {
            Date,
            Time,
            Datetime
        }

        private GetFormat.FormatType formatType;

        public GetFormat(GetFormat.FormatType type, Tup.Cobar.Parser.Ast.Expression.Expression
             format)
            : base("GET_FORMAT", WrapList(format))
        {
            this.formatType = type;
        }

        public virtual GetFormat.FormatType GetFormatType()
        {
            return formatType;
        }

        public virtual Expression GetFormatExpr()
        {
            return arguments[0];
        }

        public override FunctionExpression ConstructFunction(IList<Tup.Cobar.Parser.Ast.Expression.Expression
            > arguments)
        {
            throw new NotSupportedException("function of GetFormat has special arguments");
        }

        public override void Accept(SQLASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}