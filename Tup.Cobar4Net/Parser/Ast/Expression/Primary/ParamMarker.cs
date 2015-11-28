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

namespace Tup.Cobar4Net.Parser.Ast.Expression.Primary
{
    /// <summary><code>'?'</code></summary>
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class ParamMarker : PrimaryExpression
    {
        private readonly int paramIndex;

        /// <param name="paramIndex">start from 1</param>
        public ParamMarker(int paramIndex)
        {
            this.paramIndex = paramIndex;
        }

        /// <returns>start from 1</returns>
        public virtual int GetParamIndex()
        {
            return paramIndex;
        }

        public override int GetHashCode()
        {
            return paramIndex;
        }

        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }
            if (obj is ParamMarker)
            {
                ParamMarker that = (ParamMarker)obj;
                return this.paramIndex == that.paramIndex;
            }
            return false;
        }

        protected override object EvaluationInternal(IDictionary<object, object> parameters)
        {
            return parameters.GetValue(paramIndex);
        }

        public override void Accept(SQLASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}