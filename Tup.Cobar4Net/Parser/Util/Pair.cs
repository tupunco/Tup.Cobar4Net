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

using System.Diagnostics.CodeAnalysis;

namespace Tup.Cobar4Net.Parser.Util
{
    /// <summary>(created at 2010-7-21)</summary>
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public sealed class Pair<TKey, TValue>
    {
        private const int HashConst = 37;

        public Pair(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }

        public TKey Key { get; }

        public TValue Value { get; }

        public override string ToString()
        {
            return string.Format("[Pair Key={0}, Value={1}]", Key, Value);
        }

        [SuppressMessage("Potential Code Quality Issues", "RECS0017:Possible compare of value type with 'null'",
            Justification = "<¹ÒÆð>")]
        public override int GetHashCode()
        {
            var hash = 17;
            if (Key == null)
            {
                hash += HashConst;
            }
            else
            {
                hash = hash << 5 + hash << 1 + hash + Key.GetHashCode();
            }
            if (Value == null)
            {
                hash += HashConst;
            }
            else
            {
                hash = hash << 5 + hash << 1 + hash + Value.GetHashCode();
            }
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }
            if (!(obj is Pair<TKey, TValue>))
            {
                return false;
            }
            var that = (Pair<TKey, TValue>)obj;
            return IsEquals(Key, that.Key) && IsEquals(Value, that.Value);
        }

        private static bool IsEquals(object o1, object o2)
        {
            if (o1 == o2)
            {
                return true;
            }
            if (o1 == null)
            {
                return o2 == null;
            }
            return o1.Equals(o2);
        }
    }
}