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

namespace Tup.Cobar4Net.Route.Hint
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public sealed class SimpleHintParser : HintParser
    {
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        public override void Process(CobarHint hint, string hintName, string sql)
        {
            object value = ParsePrimary(hint, sql);
            if (value is long)
            {
                value = ((long)value);
            }
            var properties = new Dictionary<string, object>(1);
            properties[hintName] = value;
            try
            {
                switch (hintName)
                {
                    case "table":
                        hint.SetTable(value as string);
                        break;
                    case "replica":
                        hint.SetReplica(Convert.ToInt32(value));
                        break;
                    default:
                        throw new NotSupportedException(string.Format("hintName:{0},value:{1}", hintName, value));
                }
                //INFO---SimpleHintParser
                //ParameterMapping.Mapping(hint, properties);
            }
            catch (Exception t)
            {
                throw new SQLSyntaxErrorException(t);
            }
        }
    }
}