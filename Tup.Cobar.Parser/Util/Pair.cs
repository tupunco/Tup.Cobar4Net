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

using System.Text;

namespace Tup.Cobar.Parser.Util
{
    /// <summary>(created at 2010-7-21)</summary>
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public sealed class Pair<K, V>
    {
        private readonly K key;

        private readonly V value;

        public Pair(K key, V value)
        {
            this.key = key;
            this.value = value;
        }

        public K GetKey()
        {
            return key;
        }

        public V GetValue()
        {
            return value;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("(").Append(key).Append(", ").Append(value).Append(")");
            return sb.ToString();
        }

        private const int HashConst = 37;

        public override int GetHashCode()
        {
            int hash = 17;
            if (key == null)
            {
                hash += HashConst;
            }
            else
            {
                hash = hash << 5 + hash << 1 + hash + key.GetHashCode();
            }
            if (value == null)
            {
                hash += HashConst;
            }
            else
            {
                hash = hash << 5 + hash << 1 + hash + value.GetHashCode();
            }
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }
            if (!(obj is Tup.Cobar.Parser.Util.Pair<K, V>))
            {
                return false;
            }
            var that = (Tup.Cobar.Parser.Util.Pair<K, V>)obj;
            return IsEquals(this.key, that.key) && IsEquals(this.value, that.value);
        }

        private bool IsEquals(object o1, object o2)
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