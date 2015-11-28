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
using Tup.Cobar4Net.Parser.Visitor;

namespace Tup.Cobar4Net.Parser.Ast.Expression.Primary.Literal
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class LiteralBitField : Literal
    {
        private readonly string text;

        private readonly string introducer;

        /// <param name="introducer">e.g. "_latin1"</param>
        /// <param name="bitFieldText">e.g. "01010"</param>
        public LiteralBitField(string introducer, string bitFieldText)
        {
            if (bitFieldText == null)
            {
                throw new ArgumentException("bit text is null");
            }
            this.introducer = introducer;
            this.text = bitFieldText;
        }

        public virtual string GetText()
        {
            return text;
        }

        public virtual string GetIntroducer()
        {
            return introducer;
        }

        public override void Accept(SQLASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}