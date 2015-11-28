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
using System.Linq;

namespace Tup.Cobar4Net.Config.Model
{
    /// <summary>∏Ù¿Î«¯≈‰÷√∂®“Â</summary>
    /// <author>haiqing.zhuhq 2012-4-17</author>
    public sealed class QuarantineConfig
    {
        private readonly IDictionary<string, ICollection<string>> hosts;

        public QuarantineConfig()
        {
            hosts = new Dictionary<string, ICollection<string>>();
        }

        public IDictionary<string, ICollection<string>> Hosts
        {
            get { return hosts; }
        }

        public IDictionary<string, ICollection<string>> GetHosts()
        {
            return hosts;
        }

        public override string ToString()
        {
            return string.Format("[QuarantineConfig hosts:[{0}]]",
                                    string.Join(",", (hosts ?? new Dictionary<string, ICollection<string>>(0))
                                                                        .Select(x => string.Format("<{0}, [{1}]>", x.Key,
                                                                                                    string.Join(",", x.Value)))));
        }
    }
}
