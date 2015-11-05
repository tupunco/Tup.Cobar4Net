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

namespace Tup.Cobar.Parser.Ast.Expression.Primary.Function.Cast
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class Cast : FunctionExpression
    {
        private readonly string typeName;

        private readonly Tup.Cobar.Parser.Ast.Expression.Expression typeInfo1;

        private readonly Tup.Cobar.Parser.Ast.Expression.Expression typeInfo2;

        /// <param name="expr">never null</param>
        public Cast(Tup.Cobar.Parser.Ast.Expression.Expression expr, string typeName, Tup.Cobar.Parser.Ast.Expression.Expression
             typeInfo1, Tup.Cobar.Parser.Ast.Expression.Expression typeInfo2)
            : base("CAST", WrapList(expr))
        {
            if (null == typeName)
            {
                throw new ArgumentException("typeName is null");
            }
            this.typeName = typeName;
            this.typeInfo1 = typeInfo1;
            this.typeInfo2 = typeInfo2;
        }

        /// <returns>never null</returns>
        public virtual Tup.Cobar.Parser.Ast.Expression.Expression GetExpr()
        {
            return GetArguments()[0];
        }

        /// <returns>never null</returns>
        public virtual string GetTypeName()
        {
            return typeName;
        }

        public virtual Tup.Cobar.Parser.Ast.Expression.Expression GetTypeInfo1()
        {
            return typeInfo1;
        }

        public virtual Tup.Cobar.Parser.Ast.Expression.Expression GetTypeInfo2()
        {
            return typeInfo2;
        }

        public override FunctionExpression ConstructFunction(IList<Tup.Cobar.Parser.Ast.Expression.Expression
            > arguments)
        {
            throw new NotSupportedException("function of char has special arguments");
        }

        public override void Accept(SQLASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}