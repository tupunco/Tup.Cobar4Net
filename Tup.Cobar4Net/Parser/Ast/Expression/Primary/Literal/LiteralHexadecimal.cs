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
using System.Text;
using Sharpen;
using Tup.Cobar4Net.Parser.Util;
using Tup.Cobar4Net.Parser.Visitor;

namespace Tup.Cobar4Net.Parser.Ast.Expression.Primary.Literal
{
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public class LiteralHexadecimal : Literal
    {
        private readonly string _charset;

        private readonly string _introducer;

        private readonly int _offset;

        private readonly int _size;

        private readonly char[] _string;
        private byte[] _bytes;

        /// <param name="introducer">e.g. "_latin1"</param>
        /// <param name="string">e.g. "select x'89df'"</param>
        /// <param name="offset">e.g. 9</param>
        /// <param name="size">e.g. 4</param>
        public LiteralHexadecimal(string introducer, char[] @string, int offset, int size, string charset)
        {
            if (@string == null || offset + size > @string.Length)
            {
                throw new ArgumentException("hex text is invalid");
            }
            if (charset == null)
            {
                throw new ArgumentException("_charset is null");
            }
            _introducer = introducer;
            _charset = charset;
            _string = @string;
            _offset = offset;
            _size = size;
        }

        public virtual string Text
        {
            get { return new string(_string, _offset, _size); }
        }

        public virtual string Introducer
        {
            get { return _introducer; }
        }

        public virtual void AppendTo(StringBuilder sb)
        {
            sb.Append(_string, _offset, _size);
        }

        protected override object EvaluationInternal(IDictionary<object, object> parameters)
        {
            _bytes = ParseString.HexString2Bytes(_string, _offset, _size);
            return Runtime.GetStringForBytes(_bytes, _introducer == null
                ? _charset
                : Runtime.Substring(_introducer, 1));
        }

        public override void Accept(ISqlAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}