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
using Tup.Cobar4Net.Config.Model;
using Tup.Cobar4Net.Config.Model.Rule;

namespace Tup.Cobar4Net.Config.Loader
{
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public interface IConfigLoader
    {
        IDictionary<string, IRuleAlgorithm> RuleFunction { get; }

        ICollection<RuleConfig> RuleConfigList { get; }

        IDictionary<string, SchemaConfig> SchemaConfigs { get; }

        IDictionary<string, DataNodeConfig> DataNodes { get; }

        IDictionary<string, DataSourceConfig> DataSources { get; }

        SystemConfig SystemConfig { get; }

        IDictionary<string, UserConfig> UserConfigs { get; }

        QuarantineConfig QuarantineConfig { get; }

        ClusterConfig ClusterConfig { get; }

        SchemaConfig GetSchemaConfig(string schema);

        UserConfig GetUserConfig(string user);
    }
}