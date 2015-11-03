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

namespace Tup.Cobar.SqlParser.Util
{
    /**
     * @author <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
     * @author shaojin.wensj
     */
    public static class CharTypes
    {
        private readonly static bool[] hexFlags = new bool[256];
        private readonly static bool[] identifierFlags = new bool[256];
        private readonly static bool[] whitespaceFlags = new bool[256];

        static CharTypes()
        {
            #region hexFlags
            for (char c = (char)0; c < hexFlags.Length; ++c)
            {
                if (c >= 'A' && c <= 'F')
                {
                    hexFlags[c] = true;
                }
                else if (c >= 'a' && c <= 'f')
                {
                    hexFlags[c] = true;
                }
                else if (c >= '0' && c <= '9')
                {
                    hexFlags[c] = true;
                }
            }
            #endregion

            #region identifierFlags
            for (char c = (char)0; c < identifierFlags.Length; ++c)
            {
                if (c >= 'A' && c <= 'Z')
                {
                    identifierFlags[c] = true;
                }
                else if (c >= 'a' && c <= 'z')
                {
                    identifierFlags[c] = true;
                }
                else if (c >= '0' && c <= '9')
                {
                    identifierFlags[c] = true;
                }
            }
            // identifierFlags['`'] = true;
            identifierFlags['_'] = true;
            identifierFlags['$'] = true;
            #endregion

            #region whitespaceFlags
            whitespaceFlags[' '] = true;
            whitespaceFlags['\n'] = true;
            whitespaceFlags['\r'] = true;
            whitespaceFlags['\t'] = true;
            whitespaceFlags['\f'] = true;
            whitespaceFlags['\b'] = true;
            #endregion
        }

        public static bool isHex(char c)
        {
            return c < 256 && hexFlags[c];
        }

        public static bool isDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        public static bool isIdentifierChar(char c)
        {
            return c > identifierFlags.Length || identifierFlags[c];
        }

        /**
         * @return false if {@link MySQLLexer#EOI}
         */
        public static bool isWhitespace(char c)
        {
            return c <= whitespaceFlags.Length && whitespaceFlags[c];
        }
    }
}