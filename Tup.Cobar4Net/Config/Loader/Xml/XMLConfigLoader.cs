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

namespace Tup.Cobar4Net.Config.Loader.Xml
{
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public class XMLConfigLoader : IConfigLoader
    {
        /// <summary>unmodifiable</summary>
        private readonly IDictionary<string, DataNodeConfig> dataNodes;

        /// <summary>unmodifiable</summary>
        private readonly IDictionary<string, DataSourceConfig> dataSources;

        /// <summary>unmodifiable</summary>
        private readonly IDictionary<string, IRuleAlgorithm> functions;

        /// <summary>unmodifiable</summary>
        private readonly ICollection<RuleConfig> rules;

        /// <summary>unmodifiable</summary>
        private readonly IDictionary<string, SchemaConfig> schemas;

        /// <summary>unmodifiable</summary>
        private readonly IDictionary<string, UserConfig> users;

        public XMLConfigLoader(ISchemaLoader schemaLoader)
        {
            functions = new Dictionary<string, IRuleAlgorithm>(schemaLoader.Functions).AsReadOnly();
            dataSources = schemaLoader.DataSources;
            dataNodes = schemaLoader.DataNodes;
            schemas = schemaLoader.Schemas;
            rules = schemaLoader.RuleConfigList;
            schemaLoader = null;

            var serverLoader = new XmlServerLoader();
            SystemConfig = serverLoader.System;
            users = serverLoader.Users;
            QuarantineConfig = serverLoader.Quarantine;
            ClusterConfig = serverLoader.Cluster;
            serverLoader = null;
        }

        public ClusterConfig ClusterConfig { get; }

        public QuarantineConfig QuarantineConfig { get; }

        public UserConfig GetUserConfig(string user)
        {
            return users.GetValue(user);
        }

        public IDictionary<string, UserConfig> UserConfigs
        {
            get { return users; }
        }

        public SystemConfig SystemConfig { get; }

        public IDictionary<string, IRuleAlgorithm> RuleFunction
        {
            get { return functions; }
        }

        public ICollection<RuleConfig> RuleConfigList
        {
            get { return rules; }
        }

        public IDictionary<string, SchemaConfig> SchemaConfigs
        {
            get { return schemas; }
        }

        public IDictionary<string, DataNodeConfig> DataNodes
        {
            get { return dataNodes; }
        }

        public IDictionary<string, DataSourceConfig> DataSources
        {
            get { return dataSources; }
        }

        public SchemaConfig GetSchemaConfig(string schema)
        {
            return schemas.GetValue(schema);
        }
    }
}