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

namespace Tup.Cobar.Parser.Ast.Expression.Primary.Literal
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class LiteralBoolean : Tup.Cobar.Parser.Ast.Expression.Primary.Literal.Literal
    {
        public static readonly int True = 1;

        public static readonly int False = 0;

        private readonly bool value;

        public LiteralBoolean(bool value)
            : base()
        {
            this.value = value;
        }

        public virtual bool IsTrue()
        {
            return value;
        }

        protected override object EvaluationInternal(IDictionary<Expression, Expression> parameters
            )
        {
            return value ? True : False;
        }

        public override void Accept(SQLASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}