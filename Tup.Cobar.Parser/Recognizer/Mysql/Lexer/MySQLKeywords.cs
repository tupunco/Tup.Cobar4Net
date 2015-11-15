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
using System.Text.RegularExpressions;

namespace Tup.Cobar.Parser.Recognizer.Mysql.Lexer
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    internal class MySQLKeywords
    {
        public static readonly Tup.Cobar.Parser.Recognizer.Mysql.Lexer.MySQLKeywords DefaultKeywords
             = new Tup.Cobar.Parser.Recognizer.Mysql.Lexer.MySQLKeywords();

        private readonly IDictionary<string, MySQLToken> keywords = new Dictionary<string
            , MySQLToken>(230);

        private MySQLKeywords()
        {
            var values = (MySQLToken[])Enum.GetValues(typeof(MySQLToken));
            var names = Enum.GetNames(typeof(MySQLToken));
            var name = string.Empty;
            var kw = "Kw";
            var kwLen = kw.Length;

            var tReg = new Regex(@"([a-z])([A-Z])");
            for (int i = 0; i < names.Length; i++)
            {
                name = names[i];
                if (name.StartsWith(kw, StringComparison.OrdinalIgnoreCase))
                {
                    keywords.Add(tReg.Replace(name.Substring(kwLen), "$1_$2").ToUpper(), values[i]);
                }
            }
            keywords.Add("NULL", MySQLToken.LiteralNull);
            keywords.Add("FALSE", MySQLToken.LiteralBoolFalse);
            keywords.Add("TRUE", MySQLToken.LiteralBoolTrue);
        }

        /// <param name="keyUpperCase">must be uppercase</param>
        /// <returns>
        /// <code>KeyWord</code> or
        /// <see cref="Tup.Cobar.Parser.Recognizer.Mysql.MySQLToken.LiteralNull">NULL</see>
        /// or
        /// <see cref="Tup.Cobar.Parser.Recognizer.Mysql.MySQLToken.LiteralBoolFalse">FALSE</see>
        /// or
        /// <see cref="Tup.Cobar.Parser.Recognizer.Mysql.MySQLToken.LiteralBoolTrue">TRUE</see>
        /// </returns>
        public virtual MySQLToken GetKeyword(string keyUpperCase)
        {
            return keywords.GetValue(keyUpperCase);
        }
    }
}