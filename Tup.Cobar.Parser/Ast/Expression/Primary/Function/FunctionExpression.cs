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
using Tup.Cobar.Parser.Visitor;

namespace Tup.Cobar.Parser.Ast.Expression.Primary.Function
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public abstract class FunctionExpression : PrimaryExpression
    {
        protected internal static IList<Expression> WrapList
            (Expression expr)
        {
            IList<Expression> list = new List<Expression >(1);
            list.Add(expr);
            return list;
        }

        /// <summary><code>this</code> function object being called is a prototype</summary>
        public abstract Tup.Cobar.Parser.Ast.Expression.Primary.Function.FunctionExpression
             ConstructFunction(IList<Expression> arguments);

        protected readonly string functionName;

        protected readonly IList<Expression> arguments;

        public FunctionExpression(string functionName, IList<Expression> arguments)
            : base()
        {
            this.functionName = functionName;
            if (arguments == null || arguments.IsEmpty())
            {
                this.arguments = new List<Expression>(0);
            }
            else
            {
                if (arguments is List<Expression>)
                {
                    this.arguments = arguments;
                }
                else
                {
                    this.arguments = new List<Tup.Cobar.Parser.Ast.Expression.Expression>(arguments);
                }
            }
        }

        public virtual void Init()
        {
        }

        /// <returns>never null</returns>
        public virtual IList<Expression> GetArguments()
        {
            return arguments;
        }

        public virtual string GetFunctionName()
        {
            return functionName;
        }

        public override Expression SetCacheEvalRst(bool cacheEvalRst
            )
        {
            return base.SetCacheEvalRst(cacheEvalRst);
        }

        public override void Accept(SQLASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}