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

namespace Tup.Cobar4Net.Route.Util
{
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public sealed class PermutationUtil
    {
        public static ICollection<string> PermutateSQL(string delimiter, params string[] frag)
        {
            return new PermutationGenerator(frag)
                .SetDelimiter(delimiter).PermutateSql();
        }

        public sealed class PermutationGenerator
        {
            private readonly IList<string> _fragments;
            private string _delimiter = ", ";

            public PermutationGenerator(params string[] frag)
            {
                if (frag == null || frag.Length <= 0)
                {
                    throw new ArgumentException();
                }
                IList<string> list = new List<string>(frag.Length);
                foreach (var f in frag)
                {
                    list.Add(f);
                }
                _fragments = list;
            }

            public PermutationGenerator SetDelimiter(string delimiter)
            {
                _delimiter = delimiter;
                return this;
            }

            public ICollection<string> PermutateSql()
            {
                return Gen(_fragments);
            }

            private ICollection<string> Gen(IList<string> frag)
            {
                if (frag.Count == 1)
                {
                    return new HashSet<string>(frag);
                }
                ICollection<string> rst = new HashSet<string>();
                for (var i = 0; i < frag.Count; ++i)
                {
                    var prefix = frag[i] + _delimiter;
                    IList<string> fragnew = new List<string>();
                    for (var j = 0; j < frag.Count; ++j)
                    {
                        if (j != i)
                        {
                            fragnew.Add(frag[j]);
                        }
                    }
                    var smallP = Gen(fragnew);
                    foreach (var s in smallP)
                    {
                        rst.Add(prefix + s);
                    }
                }
                return rst;
            }
        }
    }
}