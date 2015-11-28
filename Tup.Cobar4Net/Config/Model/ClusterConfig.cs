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
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class ClusterConfig
    {
        private readonly IDictionary<string, CobarNodeConfig> nodes = null;

        private readonly IDictionary<string, IList<string>> groups = null;

        public IDictionary<string, CobarNodeConfig> Nodes
        {
            get { return nodes; }
        }

        public IDictionary<string, IList<string>> Groups
        {
            get { return groups; }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="groups"></param>
        public ClusterConfig(IDictionary<string, CobarNodeConfig> nodes,
                             IDictionary<string, IList<string>> groups)
        {
            this.nodes = nodes;
            this.groups = groups;
        }

        public virtual IDictionary<string, CobarNodeConfig> GetNodes()
        {
            return nodes;
        }

        public virtual IDictionary<string, IList<string>> GetGroups()
        {
            return groups;
        }

        public override string ToString()
        {
            return string.Format("[ClusterConfig nodes:[{0}], groups:[{1}]]",
                                    string.Join(",", nodes ?? new Dictionary<string, CobarNodeConfig>(0)),
                                    string.Join(",", (groups ?? new Dictionary<string, IList<string>>(0))
                                                                        .Select(x => string.Format("<{0}, [{1}]>", x.Key,
                                                                                                    string.Join(",", x.Value)))));
        }
    }
}
