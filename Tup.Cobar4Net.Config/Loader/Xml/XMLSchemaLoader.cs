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
using System.IO;
using System.Text;
using Org.W3c.Dom;
using Sharpen;
using Tup.Cobar4Net.Config.Loader;
using Tup.Cobar4Net.Config.Model;
using Tup.Cobar4Net.Config.Model.Rule;
using Tup.Cobar4Net.Config.Util;
using Tup.Cobar4Net.Util;

namespace Tup.Cobar4Net.Config.Loader.Xml
{
	/// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
	public class XMLSchemaLoader : SchemaLoader
	{
		private const string DefaultDtd = "/schema.dtd";

		private const string DefaultXml = "/schema.xml";

		private readonly IDictionary<string, TableRuleConfig> tableRules;

		private readonly ICollection<RuleConfig> rules;

		private readonly IDictionary<string, RuleAlgorithm> functions;

		private readonly IDictionary<string, DataSourceConfig> dataSources;

		private readonly IDictionary<string, DataNodeConfig> dataNodes;

		private readonly IDictionary<string, SchemaConfig> schemas;

		public XMLSchemaLoader(string schemaFile, string ruleFile)
		{
			XMLRuleLoader ruleLoader = new XMLRuleLoader(ruleFile);
			this.rules = ruleLoader.ListRuleConfig();
			this.tableRules = ruleLoader.GetTableRules();
			this.functions = ruleLoader.GetFunctions();
			ruleLoader = null;
			this.dataSources = new Dictionary<string, DataSourceConfig>();
			this.dataNodes = new Dictionary<string, DataNodeConfig>();
			this.schemas = new Dictionary<string, SchemaConfig>();
			this.Load(DefaultDtd, schemaFile == null ? DefaultXml : schemaFile);
		}

		public XMLSchemaLoader()
			: this(null, null)
		{
		}

		public virtual IDictionary<string, TableRuleConfig> GetTableRules()
		{
			return tableRules;
		}

		public virtual IDictionary<string, RuleAlgorithm> GetFunctions()
		{
			return functions;
		}

		public virtual IDictionary<string, DataSourceConfig> GetDataSources()
		{
			return (IDictionary<string, DataSourceConfig>)(dataSources.IsEmpty() ? Sharpen.Collections
				.EmptyMap() : dataSources);
		}

		public virtual IDictionary<string, DataNodeConfig> GetDataNodes()
		{
			return (IDictionary<string, DataNodeConfig>)(dataNodes.IsEmpty() ? Sharpen.Collections
				.EmptyMap() : dataNodes);
		}

		public virtual IDictionary<string, SchemaConfig> GetSchemas()
		{
			return (IDictionary<string, SchemaConfig>)(schemas.IsEmpty() ? Sharpen.Collections
				.EmptyMap() : schemas);
		}

		public virtual ICollection<RuleConfig> ListRuleConfig()
		{
			return rules;
		}

		private void Load(string dtdFile, string xmlFile)
		{
			InputStream dtd = null;
			InputStream xml = null;
			try
			{
				dtd = typeof(Tup.Cobar4Net.Config.Loader.Xml.XMLSchemaLoader).GetResourceAsStream
					(dtdFile);
				xml = typeof(Tup.Cobar4Net.Config.Loader.Xml.XMLSchemaLoader).GetResourceAsStream
					(xmlFile);
				Element root = ConfigUtil.GetDocument(dtd, xml).GetDocumentElement();
				LoadDataSources(root);
				LoadDataNodes(root);
				LoadSchemas(root);
			}
			catch (ConfigException e)
			{
				throw;
			}
			catch (Exception e)
			{
				throw new ConfigException(e);
			}
			finally
			{
				if (dtd != null)
				{
					try
					{
						dtd.Close();
					}
					catch (IOException)
					{
					}
				}
				if (xml != null)
				{
					try
					{
						xml.Close();
					}
					catch (IOException)
					{
					}
				}
			}
		}

		private void LoadSchemas(Element root)
		{
			NodeList list = root.GetElementsByTagName("schema");
			for (int i = 0; i < n; i++)
			{
				Element schemaElement = (Element)list.Item(i);
				string name = schemaElement.GetAttribute("name");
				string dataNode = schemaElement.GetAttribute("dataNode");
				// 在非空的情况下检查dataNode是否存在
				if (dataNode != null && dataNode.Length != 0)
				{
					CheckDataNodeExists(dataNode);
				}
				else
				{
					dataNode = string.Empty;
				}
				// 确保非空
				string group = "default";
				if (schemaElement.HasAttribute("group"))
				{
					group = schemaElement.GetAttribute("group").Trim();
				}
				IDictionary<string, TableConfig> tables = LoadTables(schemaElement);
				if (schemas.Contains(name))
				{
					throw new ConfigException("schema " + name + " duplicated!");
				}
				bool keepSqlSchema = false;
				if (schemaElement.HasAttribute("keepSqlSchema"))
				{
					keepSqlSchema = System.Boolean.Parse(schemaElement.GetAttribute("keepSqlSchema").
						Trim());
				}
				schemas[name] = new SchemaConfig(name, dataNode, group, keepSqlSchema, tables);
			}
		}

		private IDictionary<string, TableConfig> LoadTables(Element node)
		{
			IDictionary<string, TableConfig> tables = new Dictionary<string, TableConfig>();
			NodeList nodeList = node.GetElementsByTagName("table");
			for (int i = 0; i < nodeList.GetLength(); i++)
			{
				Element tableElement = (Element)nodeList.Item(i);
				string name = tableElement.GetAttribute("name").ToUpper();
				string dataNode = tableElement.GetAttribute("dataNode");
				TableRuleConfig tableRule = null;
				if (tableElement.HasAttribute("rule"))
				{
					string ruleName = tableElement.GetAttribute("rule");
					tableRule = tableRules[ruleName];
					if (tableRule == null)
					{
						throw new ConfigException("rule " + ruleName + " is not found!");
					}
				}
				bool ruleRequired = false;
				if (tableElement.HasAttribute("ruleRequired"))
				{
					ruleRequired = System.Boolean.Parse(tableElement.GetAttribute("ruleRequired"));
				}
				string[] tableNames = SplitUtil.Split(name, ',', true);
				foreach (string tableName in tableNames)
				{
					TableConfig table = new TableConfig(tableName, dataNode, tableRule, ruleRequired);
					CheckDataNodeExists(table.GetDataNodes());
					if (tables.Contains(table.GetName()))
					{
						throw new ConfigException("table " + tableName + " duplicated!");
					}
					tables[table.GetName()] = table;
				}
			}
			return tables;
		}

		private void CheckDataNodeExists(params string[] nodes)
		{
			if (nodes == null || nodes.Length < 1)
			{
				return;
			}
			foreach (string node in nodes)
			{
				if (!dataNodes.Contains(node))
				{
					throw new ConfigException("dataNode '" + node + "' is not found!");
				}
			}
		}

		private void LoadDataNodes(Element root)
		{
			NodeList list = root.GetElementsByTagName("dataNode");
			for (int i = 0; i < n; i++)
			{
				Element element = (Element)list.Item(i);
				string dnNamePrefix = element.GetAttribute("name");
				IList<DataNodeConfig> confList = new List<DataNodeConfig>();
				try
				{
					Element dsElement = FindPropertyByName(element, "dataSource");
					if (dsElement == null)
					{
						throw new ArgumentNullException("dataNode xml Element with name of " + dnNamePrefix
							 + " has no dataSource Element");
					}
					NodeList dataSourceList = dsElement.GetElementsByTagName("dataSourceRef");
					string[][] dataSources = new string[dataSourceList.GetLength()][];
					for (int j = 0; j < m; ++j)
					{
						Element @ref = (Element)dataSourceList.Item(j);
						string dsString = @ref.GetTextContent();
						dataSources[j] = SplitUtil.Split(dsString, ',', '$', '-', '[', ']');
					}
					if (dataSources.Length <= 0)
					{
						throw new ConfigException("no dataSourceRef defined!");
					}
					foreach (string[] dss in dataSources)
					{
						if (dss.Length != dataSources[0].Length)
						{
							throw new ConfigException("dataSource number not equals!");
						}
					}
					for (int k = 0; k < limit; ++k)
					{
						StringBuilder dsString = new StringBuilder();
						for (int dsIndex = 0; dsIndex < dataSources.Length; ++dsIndex)
						{
							if (dsIndex > 0)
							{
								dsString.Append(',');
							}
							dsString.Append(dataSources[dsIndex][k]);
						}
						DataNodeConfig conf = new DataNodeConfig();
						ParameterMapping.Mapping(conf, ConfigUtil.LoadElements(element));
						confList.Add(conf);
						switch (k)
						{
							case 0:
							{
								conf.SetName((limit == 1) ? dnNamePrefix : dnNamePrefix + "[" + k + "]");
								break;
							}

							default:
							{
								conf.SetName(dnNamePrefix + "[" + k + "]");
								break;
							}
						}
						conf.SetDataSource(dsString.ToString());
					}
				}
				catch (Exception e)
				{
					throw new ConfigException("dataNode " + dnNamePrefix + " define error", e);
				}
				foreach (DataNodeConfig conf_1 in confList)
				{
					if (dataNodes.Contains(conf_1.GetName()))
					{
						throw new ConfigException("dataNode " + conf_1.GetName() + " duplicated!");
					}
					dataNodes[conf_1.GetName()] = conf_1;
				}
			}
		}

		private void LoadDataSources(Element root)
		{
			NodeList list = root.GetElementsByTagName("dataSource");
			for (int i = 0; i < n; ++i)
			{
				Element element = (Element)list.Item(i);
				List<DataSourceConfig> dscList = new List<DataSourceConfig>();
				string dsNamePrefix = element.GetAttribute("name");
				try
				{
					string dsType = element.GetAttribute("type");
					Element locElement = FindPropertyByName(element, "location");
					if (locElement == null)
					{
						throw new ArgumentNullException("dataSource xml Element with name of " + dsNamePrefix
							 + " has no location Element");
					}
					NodeList locationList = locElement.GetElementsByTagName("location");
					int dsIndex = 0;
					for (int j = 0; j < m; ++j)
					{
						string locStr = ((Element)locationList.Item(j)).GetTextContent();
						int colonIndex = locStr.IndexOf(':');
						int slashIndex = locStr.IndexOf('/');
						string dsHost = Sharpen.Runtime.Substring(locStr, 0, colonIndex).Trim();
						int dsPort = System.Convert.ToInt32(Sharpen.Runtime.Substring(locStr, colonIndex 
							+ 1, slashIndex).Trim());
						string[] schemas = SplitUtil.Split(Sharpen.Runtime.Substring(locStr, slashIndex +
							 1).Trim(), ',', '$', '-');
						foreach (string dsSchema in schemas)
						{
							DataSourceConfig dsConf = new DataSourceConfig();
							ParameterMapping.Mapping(dsConf, ConfigUtil.LoadElements(element));
							dscList.Add(dsConf);
							switch (dsIndex)
							{
								case 0:
								{
									dsConf.SetName(dsNamePrefix);
									break;
								}

								case 1:
								{
									dscList[0].SetName(dsNamePrefix + "[0]");
									goto default;
								}

								default:
								{
									dsConf.SetName(dsNamePrefix + "[" + dsIndex + "]");
									break;
								}
							}
							dsConf.SetType(dsType);
							dsConf.SetDatabase(dsSchema);
							dsConf.SetHost(dsHost);
							dsConf.SetPort(dsPort);
							++dsIndex;
						}
					}
				}
				catch (Exception e)
				{
					throw new ConfigException("dataSource " + dsNamePrefix + " define error", e);
				}
				foreach (DataSourceConfig dsConf_1 in dscList)
				{
					if (dataSources.Contains(dsConf_1.GetName()))
					{
						throw new ConfigException("dataSource name " + dsConf_1.GetName() + "duplicated!"
							);
					}
					dataSources[dsConf_1.GetName()] = dsConf_1;
				}
			}
		}

		private static Element FindPropertyByName(Element bean, string name)
		{
			NodeList propertyList = bean.GetElementsByTagName("property");
			for (int j = 0; j < m; ++j)
			{
				Node node = propertyList.Item(j);
				if (node is Element)
				{
					Element p = (Element)node;
					if (name.Equals(p.GetAttribute("name")))
					{
						return p;
					}
				}
			}
			return null;
		}
	}
}
