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
using Sharpen;

namespace Tup.Cobar4Net.Util
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public static class CollectionUtil
    {
        /// <param name="orig">if null, return intersect</param>
        public static ICollection<TItem> IntersectSet<TItem>(ICollection<TItem> orig, ICollection<TItem> intersect)
        {
            if (orig == null)
            {
                return intersect;
            }
            if (intersect == null || orig.IsEmpty())
            {
                return new HashSet<TItem>();
            }

            var set = new HashSet<TItem>();
            foreach (TItem p in orig)
            {
                if (intersect.Contains(p))
                {
                    set.Add(p);
                }
            }
            return set;
        }
    }
}
