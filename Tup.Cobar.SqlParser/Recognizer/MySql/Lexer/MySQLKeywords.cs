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
 * Project: fastjson
 *
 * File Created at 2010-12-2
 *
 * Copyright 1999-2100 Alibaba.com Corporation Limited.
 * All rights reserved.
 *
 * This software is the confidential and proprietary information of
 * Alibaba Company. ("Confidential Information").  You shall not
 * disclose such Confidential Information and shall use it only in
 * accordance with the terms of the license agreement you entered into
 * with Alibaba.com.
 */

using System;
using System.Collections.Generic;

namespace Tup.Cobar.SqlParser.Recognizer.MySql.Lexer
{
    /**
     * @author <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
     */

    internal static class MySQLKeywords
    {
        //public static readonly MySQLKeywords DEFAULT_KEYWORDS = new MySQLKeywords();

        private static readonly Dictionary<String, MySQLToken> keywords = new Dictionary<String, MySQLToken>(230);

        static MySQLKeywords()
        {
            var values = (MySQLToken[])Enum.GetValues(typeof(MySQLToken));
            var names = Enum.GetNames(typeof(MySQLToken));
            var name = string.Empty;
            var kw = "KW_";
            var kwLen = kw.Length;

            for (int i = 0; i < names.Length; i++)
            {
                name = names[i];
                if (name.StartsWith(kw))
                {
                    keywords.Add(name.Substring(kwLen), values[i]);
                }
            }
            keywords.Add("NULL", MySQLToken.LITERAL_NULL);
            keywords.Add("FALSE", MySQLToken.LITERAL_BOOL_FALSE);
            keywords.Add("TRUE", MySQLToken.LITERAL_BOOL_TRUE);
        }

        /**
         * @param keyUpperCase must be uppercase
         * @return <code>KeyWord</code> or {@link MySQLToken#LITERAL_NULL NULL} or
         *         {@link MySQLToken#LITERAL_BOOL_FALSE FALSE} or
         *         {@link MySQLToken#LITERAL_BOOL_TRUE TRUE}
         */

        public static MySQLToken getKeyword(string keyUpperCase)
        {
            var outValue = MySQLToken.None;
            keywords.TryGetValue(keyUpperCase, out outValue);
            return outValue;
        }
    }
}