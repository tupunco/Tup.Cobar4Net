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
/**
 * (created at 2011-4-12)
 */
using System.Collections.Generic;

using Tup.Cobar.SqlParser.Visitor;

namespace Tup.Cobar.SqlParser.Ast.Expression.Primary.Function
{
    /**
     * @author <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
     */
    public abstract class FunctionExpression : PrimaryExpression
    {
        protected static List<Expression> wrapList(Expression expr)
        {
            var list = new List<Expression>(1);
            list.Add(expr);
            return list;
        }

        /**
         * <code>this</code> function object being called is a prototype
         */
        public abstract FunctionExpression constructFunction(List<Expression> arguments);

        protected readonly string functionName;
        protected readonly IList<Expression> arguments;

        public FunctionExpression(string functionName, IList<Expression> arguments) : base()
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
                    this.arguments = new List<Expression>(arguments);
                }
            }
        }

        public void init()
        {
        }

        /**
         * @return never null
         */
        public IList<Expression> getArguments()
        {
            return arguments;
        }

        public string getFunctionName()
        {
            return functionName;
        }

        public override Expression setCacheEvalRst(bool cacheEvalRst)
        {
            return base.setCacheEvalRst(cacheEvalRst);
        }

        public override void accept(SQLASTVisitor visitor)
        {
            visitor.visit(this);
        }
    }
}