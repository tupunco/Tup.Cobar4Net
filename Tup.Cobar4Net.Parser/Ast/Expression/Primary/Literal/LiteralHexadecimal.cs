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

using Sharpen;
using System;
using System.Collections.Generic;
using System.Text;
using Tup.Cobar4Net.Parser.Util;
using Tup.Cobar4Net.Parser.Visitor;

namespace Tup.Cobar4Net.Parser.Ast.Expression.Primary.Literal
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class LiteralHexadecimal : Literal
    {
        private byte[] bytes = null;

        private readonly string introducer;

        private readonly string charset;

        private readonly char[] @string = null;

        private readonly int offset;

        private readonly int size;

        /// <param name="introducer">e.g. "_latin1"</param>
        /// <param name="string">e.g. "select x'89df'"</param>
        /// <param name="offset">e.g. 9</param>
        /// <param name="size">e.g. 4</param>
        public LiteralHexadecimal(string introducer, char[] @string, int offset, int size, string charset)
            : base()
        {
            if (@string == null || offset + size > @string.Length)
            {
                throw new ArgumentException("hex text is invalid");
            }
            if (charset == null)
            {
                throw new ArgumentException("charset is null");
            }
            this.introducer = introducer;
            this.charset = charset;
            this.@string = @string;
            this.offset = offset;
            this.size = size;
        }

        public virtual string GetText()
        {
            return new string(@string, offset, size);
        }

        public virtual string GetIntroducer()
        {
            return introducer;
        }

        public virtual void AppendTo(StringBuilder sb)
        {
            sb.Append(@string, offset, size);
        }

        protected override object EvaluationInternal(IDictionary<object, Expression> parameters)
        {
            this.bytes = ParseString.HexString2Bytes(@string, offset, size);
            return Runtime.GetStringForBytes(bytes, introducer == null ? charset :
                                                Runtime.Substring(introducer, 1));
        }

        public override void Accept(SQLASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}