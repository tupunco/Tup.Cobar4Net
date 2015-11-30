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

namespace Tup.Cobar4Net.Parser.Ast.Expression.Primary.Function.Cast
{
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public class Cast : FunctionExpression
    {
        /// <param name="expr">never null</param>
        public Cast(IExpression expr, string typeName, IExpression typeInfo1, IExpression typeInfo2)
            : base("CAST", WrapList(expr))
        {
            if (null == typeName)
            {
                throw new ArgumentException("typeName is null");
            }
            TypeName = typeName;
            TypeInfo1 = typeInfo1;
            TypeInfo2 = typeInfo2;
        }

        /// <value>never null</value>
        public virtual IExpression Expr
        {
            get { return Arguments[0]; }
        }

        /// <value>never null</value>
        public virtual string TypeName { get; }

        public virtual IExpression TypeInfo1 { get; }

        public virtual IExpression TypeInfo2 { get; }

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