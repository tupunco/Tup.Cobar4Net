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
using Tup.Cobar4Net.Parser.Visitor;

namespace Tup.Cobar4Net.Parser.Ast.Expression.Primary.Function
{
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public abstract class FunctionExpression : PrimaryExpression
    {
        protected readonly IList<IExpression> arguments;

        protected readonly string functionName;

        public FunctionExpression(string functionName, IList<IExpression> arguments)
        {
            this.functionName = functionName;
            if (arguments == null || arguments.IsEmpty())
            {
                this.arguments = new List<IExpression>(0);
            }
            else if (arguments is List<IExpression>)
            {
                this.arguments = arguments;
            }
            else
            {
                this.arguments = new List<IExpression>(arguments);
            }
        }

        /// <value>never null</value>
        public virtual IList<IExpression> Arguments
        {
            get { return arguments; }
        }

        public virtual string FunctionName
        {
            get { return functionName; }
        }

        protected static IList<IExpression> WrapList(IExpression expr)
        {
            return new List<IExpression>(1) {expr};
        }

        /// <summary><code>this</code> function object being called is a prototype</summary>
        public abstract FunctionExpression ConstructFunction(IList<IExpression> arguments);

        public virtual void Init()
        {
        }

        public override void Accept(ISqlAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}