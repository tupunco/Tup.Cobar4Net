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
* (created at 2011-1-21)
*/

using System.Collections.Generic;

using Tup.Cobar.SqlParser.Visitor;

namespace Tup.Cobar.SqlParser.Ast.Expression.Primary.Literal
{
    /**
     * @author <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
     */
    public class LiteralBoolean : Literal
    {
        public static readonly int TRUE = 1;
        public static readonly int FALSE = 0;

        private readonly bool value;

        public LiteralBoolean(bool value) : base()
        {
            this.value = value;
        }

        public bool isTrue()
        {
            return value;
        }

        protected override object evaluationInternal(IDictionary<Expression, Expression> parameters)
        {
            return value ? TRUE : FALSE;
        }

        public override void accept(SQLASTVisitor visitor)
        {
            visitor.visit(this);
        }
    }
}
