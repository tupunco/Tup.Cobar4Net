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

namespace Tup.Cobar4Net.Parser.Recognizer.Mysql.Lexer
{
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    internal class MySqlKeywords
    {
        public static readonly MySqlKeywords DefaultKeywords = new MySqlKeywords();

        private readonly IDictionary<string, MySqlToken> keywords = InitTokenMap();

        private MySqlKeywords()
        {
        }

        private static IDictionary<string, MySqlToken> InitTokenMap()
        {
            var cKeywords = SystemUtils.GetEnumNameMapping<MySqlToken>("Kw");
            cKeywords.Add("NULL", MySqlToken.LiteralNull);
            cKeywords.Add("FALSE", MySqlToken.LiteralBoolFalse);
            cKeywords.Add("TRUE", MySqlToken.LiteralBoolTrue);

            return cKeywords;
        }

        /// <param name="keyUpperCase">must be uppercase</param>
        /// <returns>
        ///     <code>KeyWord</code> or
        ///     <see cref="MySqlToken.LiteralNull">NULL</see>
        ///     or
        ///     <see cref="MySqlToken.LiteralBoolFalse">FALSE</see>
        ///     or
        ///     <see cref="MySqlToken.LiteralBoolTrue">TRUE</see>
        /// </returns>
        public virtual MySqlToken GetKeyword(string keyUpperCase)
        {
            return keywords.GetValue(keyUpperCase);
        }
    }
}