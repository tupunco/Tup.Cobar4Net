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
using Tup.Cobar4Net.Parser.Visitor;

namespace Tup.Cobar4Net.Parser.Ast.Expression.Primary.Literal
{
    /// <summary>literal date is also possible</summary>
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public class LiteralString : Literal
    {
        /// <param name="stringValue">
        ///     content of stringValue, excluded of head and tail "'". e.g. for
        ///     stringValue token of "'don\\'t'", argument of stringValue is "don\\'t"
        /// </param>
        public LiteralString(string introducer, string stringValue, bool nChars)
        {
            Introducer = introducer;
            if (stringValue == null)
            {
                throw new ArgumentException("argument stringValue is null!");
            }
            StringValue = stringValue;
            IsNChars = nChars;
        }

        public virtual string Introducer { get; }

        public virtual string StringValue { get; }

        public virtual bool IsNChars { get; }

        public virtual string GetUnescapedString()
        {
            return GetUnescapedString(StringValue, false);
        }

        public virtual string GetUnescapedString(bool toUppercase)
        {
            return GetUnescapedString(StringValue, toUppercase);
        }

        public static string GetUnescapedString(string @string)
        {
            return GetUnescapedString(@string, false);
        }

        public static string GetUnescapedString(string @string, bool toUppercase)
        {
            var sb = new StringBuilder();
            var chars = @string.ToCharArray();
            for (var i = 0; i < chars.Length; ++i)
            {
                var c = chars[i];
                if (c == '\\')
                {
                    switch (c = chars[++i])
                    {
                        case '0':
                        {
                            sb.Append('\0');
                            break;
                        }

                        case 'b':
                        {
                            sb.Append('\b');
                            break;
                        }

                        case 'n':
                        {
                            sb.Append('\n');
                            break;
                        }

                        case 'r':
                        {
                            sb.Append('\r');
                            break;
                        }

                        case 't':
                        {
                            sb.Append('\t');
                            break;
                        }

                        case 'Z':
                        {
                            sb.Append((char)26);
                            break;
                        }

                        default:
                        {
                            sb.Append(c);
                            break;
                        }
                    }
                }
                else
                {
                    if (c == '\'')
                    {
                        ++i;
                        sb.Append('\'');
                    }
                    else
                    {
                        if (toUppercase && c >= 'a' && c <= 'z')
                        {
                            c -= (char)32;
                        }
                        sb.Append(c);
                    }
                }
            }
            return sb.ToString();
        }

        protected override object EvaluationInternal(IDictionary<object, object> parameters)
        {
            if (StringValue == null)
            {
                return null;
            }
            return GetUnescapedString();
        }

        public override void Accept(ISqlAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}