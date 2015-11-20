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
using Tup.Cobar4Net.Config.Loader;
using Tup.Cobar4Net.Config.Model;
using Tup.Cobar4Net.Config.Model.Rule;

namespace Tup.Cobar4Net.Config.Loader.Xml
{
	/// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
	public class XMLConfigLoader : ConfigLoader
	{
		/// <summary>unmodifiable</summary>
		private readonly ICollection<RuleConfig> rules;

		/// <summary>unmodifiable</summary>
		private readonly IDictionary<string, RuleAlgorithm> functions;

		/// <summary>unmodifiable</summary>
		private readonly IDictionary<string, DataSourceConfig> dataSources;

		/// <summary>unmodifiable</summary>
		private readonly IDictionary<string, DataNodeConfig> dataNodes;

		/// <summary>unmodifiable</summary>
		private readonly IDictionary<string, SchemaConfig> schemas;

		private readonly SystemConfig system;

		/// <summary>unmodifiable</summary>
		private readonly IDictionary<string, UserConfig> users;

		private readonly QuarantineConfig quarantine;

		private readonly ClusterConfig cluster;

		public XMLConfigLoader(SchemaLoader schemaLoader)
		{
			this.functions = Sharpen.Collections.UnmodifiableMap(schemaLoader.GetFunctions());
			this.dataSources = schemaLoader.GetDataSources();
			this.dataNodes = schemaLoader.GetDataNodes();
			this.schemas = schemaLoader.GetSchemas();
			this.rules = schemaLoader.ListRuleConfig();
			schemaLoader = null;
			XMLServerLoader serverLoader = new XMLServerLoader();
			this.system = serverLoader.GetSystem();
			this.users = serverLoader.GetUsers();
			this.quarantine = serverLoader.GetQuarantine();
			this.cluster = serverLoader.GetCluster();
		}

		public virtual ClusterConfig GetClusterConfig()
		{
			return cluster;
		}

		public virtual QuarantineConfig GetQuarantineConfig()
		{
			return quarantine;
		}

		public virtual UserConfig GetUserConfig(string user)
		{
			return users[user];
		}

		public virtual IDictionary<string, UserConfig> GetUserConfigs()
		{
			return users;
		}

		public virtual SystemConfig GetSystemConfig()
		{
			return system;
		}

		public virtual IDictionary<string, RuleAlgorithm> GetRuleFunction()
		{
			return functions;
		}

		public virtual ICollection<RuleConfig> ListRuleConfig()
		{
			return rules;
		}

		public virtual IDictionary<string, SchemaConfig> GetSchemaConfigs()
		{
			return schemas;
		}

		public virtual IDictionary<string, DataNodeConfig> GetDataNodes()
		{
			return dataNodes;
		}

		public virtual IDictionary<string, DataSourceConfig> GetDataSources()
		{
			return dataSources;
		}

		public virtual SchemaConfig GetSchemaConfig(string schema)
		{
			return schemas[schema];
		}
	}
}
