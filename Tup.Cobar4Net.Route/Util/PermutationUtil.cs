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
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public sealed class PermutationUtil
    {
        public static ICollection<string> PermutateSQL(string delimiter, params string[] frag)
        {
            return new PermutationUtil.PermutationGenerator(frag)
                                      .SetDelimiter(delimiter).PermutateSQL();
        }

        public sealed class PermutationGenerator
        {
            private string delimiter = ", ";

            private IList<string> fragments;

            public PermutationGenerator(params string[] frag)
            {
                if (frag == null || frag.Length <= 0)
                {
                    throw new ArgumentException();
                }
                IList<string> list = new List<string>(frag.Length);
                foreach (string f in frag)
                {
                    list.Add(f);
                }
                this.fragments = list;
            }

            public PermutationUtil.PermutationGenerator SetDelimiter(string delimiter)
            {
                this.delimiter = delimiter;
                return this;
            }

            public ICollection<string> PermutateSQL()
            {
                return Gen(fragments);
            }

            private ICollection<string> Gen(IList<string> frag)
            {
                if (frag.Count == 1)
                {
                    return new HashSet<string>(frag);
                }
                ICollection<string> rst = new HashSet<string>();
                for (int i = 0; i < frag.Count; ++i)
                {
                    string prefix = frag[i] + delimiter;
                    IList<string> fragnew = new List<string>();
                    for (int j = 0; j < frag.Count; ++j)
                    {
                        if (j != i)
                        {
                            fragnew.Add(frag[j]);
                        }
                    }
                    ICollection<string> smallP = Gen(fragnew);
                    foreach (string s in smallP)
                    {
                        rst.Add(prefix + s);
                    }
                }
                return rst;
            }
        }
    }
}