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
using System.Linq;
using System.Text;
using System.Xml;
using Sharpen;
using Tup.Cobar4Net.Config.Model;
using Tup.Cobar4Net.Config.Model.Rule;
using Tup.Cobar4Net.Config.Util;
using Tup.Cobar4Net.Util;

namespace Tup.Cobar4Net.Config.Loader.Xml
{
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public class XmlSchemaLoader : ISchemaLoader
    {
        private const string DefaultDtd = "/schema.dtd";

        private const string DefaultXml = "/schema.xml";

        private readonly IDictionary<string, DataNodeConfig> dataNodes;

        private readonly IDictionary<string, DataSourceConfig> dataSources;

        private readonly IDictionary<string, SchemaConfig> schemas;

        private readonly IDictionary<string, TableRuleConfig> tableRules;

        public XmlSchemaLoader(string schemaFile, string ruleFile)
        {
            var ruleLoader = new XMLRuleLoader(ruleFile);
            RuleConfigList = ruleLoader.RuleConfigList;
            tableRules = ruleLoader.TableRules;
            Functions = ruleLoader.Functions;
            ruleLoader = null;

            dataSources = new Dictionary<string, DataSourceConfig>();
            dataNodes = new Dictionary<string, DataNodeConfig>();
            schemas = new Dictionary<string, SchemaConfig>();
            Load(DefaultDtd, schemaFile ?? DefaultXml);
        }

        public XmlSchemaLoader()
            : this(null, null)
        {
        }

        public virtual IDictionary<string, TableRuleConfig> TableRules
        {
            get { return tableRules; }
        }

        public virtual IDictionary<string, IRuleAlgorithm> Functions { get; }

        public virtual IDictionary<string, DataSourceConfig> DataSources
        {
            get
            {
                return dataSources.IsEmpty() ? new Dictionary<string, DataSourceConfig>(0) : dataSources.AsReadOnly();
            }
        }

        public virtual IDictionary<string, DataNodeConfig> DataNodes
        {
            get { return dataNodes.IsEmpty() ? new Dictionary<string, DataNodeConfig>(0) : dataNodes.AsReadOnly(); }
        }

        public virtual IDictionary<string, SchemaConfig> Schemas
        {
            get { return schemas.IsEmpty() ? new Dictionary<string, SchemaConfig>(0) : schemas.AsReadOnly(); }
        }

        public virtual ICollection<RuleConfig> RuleConfigList { get; }

        private void Load(string dtdFile, string xmlFile)
        {
            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlFile);

                var nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
                nsmgr.AddNamespace("cobar", "http://cobar.alibaba.com/");
                var xmlNodeList = xmlDoc.SelectNodes("cobar:schema", nsmgr);
                if (xmlNodeList == null)
                    return;

                var root = xmlNodeList.Item(0) as XmlElement;

                LoadDataSources(root);
                LoadDataNodes(root);
                LoadSchemas(root);
            }
            catch (Exception e)
            {
                throw new ConfigException(e);
            }
        }

        private void LoadDataSources(XmlElement root)
        {
            var list = root.GetElementsByTagName("dataSource");
            XmlElement element = null;
            XmlElement locElement = null;
            XmlNodeList locationList = null;
            for (int i = 0, n = list.Count; i < n; i++)
            {
                element = (XmlElement)list.Item(i);
                var dscList = new List<DataSourceConfig>();
                var dsNamePrefix = element.GetAttribute("name");
                try
                {
                    var dsType = element.GetAttribute("type");
                    locElement = FindPropertyByName(element, "location");
                    if (locElement == null)
                    {
                        throw new ArgumentNullException("dataSource xml XmlElement with name of " + dsNamePrefix
                                                        + " has no location Element");
                    }

                    locationList = locElement.GetElementsByTagName("location");
                    var dsIndex = 0;
                    for (int j = 0, m = locationList.Count; j < m; ++j)
                    {
                        var locStr = ((XmlElement)locationList.Item(j)).InnerText;
                        var colonIndex = locStr.IndexOf(':');
                        var slashIndex = locStr.IndexOf('/');
                        var dsHost = Runtime.Substring(locStr, 0, colonIndex).Trim();
                        var dsPort = Convert.ToInt32(Runtime.Substring(locStr, colonIndex + 1, slashIndex).Trim());
                        var schemas = SplitUtil.Split(Runtime.Substring(locStr, slashIndex + 1).Trim(), ',', '$', '-');
                        foreach (var dsSchema in schemas)
                        {
                            var dsConf = new DataSourceConfig();
                            ParameterMapping.Mapping(dsConf, ConfigUtil.LoadElements(element));
                            dscList.Add(dsConf);

                            switch (dsIndex)
                            {
                                case 0:
                                {
                                    dsConf.Name = dsNamePrefix;
                                    break;
                                }

                                case 1:
                                {
                                    dscList[0].Name = dsNamePrefix + "[0]";
                                    goto default;
                                }

                                default:
                                {
                                    dsConf.Name = dsNamePrefix + "[" + dsIndex + "]";
                                    break;
                                }
                            }
                            dsConf.Type = dsType;
                            dsConf.Database = dsSchema;
                            dsConf.Host = dsHost;
                            dsConf.Port = dsPort;
                            ++dsIndex;
                        }
                    }
                }
                catch (Exception e)
                {
                    throw new ConfigException("dataSource " + dsNamePrefix + " define error", e);
                }

                foreach (var dsConf_1 in dscList)
                {
                    if (dataSources.ContainsKey(dsConf_1.Name))
                    {
                        throw new ConfigException("dataSource name " + dsConf_1.Name + "duplicated!");
                    }

                    dataSources[dsConf_1.Name] = dsConf_1;
                }
            }
        }

        private void LoadDataNodes(XmlElement root)
        {
            var list = root.GetElementsByTagName("dataNode");
            XmlElement element = null;
            XmlElement dsElement = null;
            XmlNodeList dataSourceList = null;
            for (int i = 0, n = list.Count; i < n; i++)
            {
                element = (XmlElement)list.Item(i);
                var dnNamePrefix = element.GetAttribute("name");
                IList<DataNodeConfig> confList = new List<DataNodeConfig>();
                try
                {
                    dsElement = FindPropertyByName(element, "dataSource");
                    if (dsElement == null)
                    {
                        throw new ArgumentNullException("dataNode xml XmlElement with name of " + dnNamePrefix
                                                        + " has no dataSource Element");
                    }

                    dataSourceList = dsElement.GetElementsByTagName("dataSourceRef");
                    var dataSources = new string[dataSourceList.Count][];
                    for (int j = 0, m = dataSourceList.Count; j < m; ++j)
                    {
                        var @ref = (XmlElement)dataSourceList.Item(j);
                        var dsString = @ref.InnerText;
                        dataSources[j] = SplitUtil.Split(dsString, ',', '$', '-', '[', ']');
                    }

                    if (dataSources.Length <= 0)
                    {
                        throw new ConfigException("no dataSourceRef defined!");
                    }
                    foreach (var dss in dataSources)
                    {
                        if (dss.Length != dataSources[0].Length)
                        {
                            throw new ConfigException("dataSource number not equals!");
                        }
                    }

                    for (int k = 0, limit = dataSources[0].Length; k < limit; ++k)
                    {
                        var dsString = new StringBuilder();
                        for (var dsIndex = 0; dsIndex < dataSources.Length; ++dsIndex)
                        {
                            if (dsIndex > 0)
                            {
                                dsString.Append(',');
                            }
                            dsString.Append(dataSources[dsIndex][k]);
                        }

                        var conf = new DataNodeConfig();
                        ParameterMapping.Mapping(conf, ConfigUtil.LoadElements(element));
                        confList.Add(conf);

                        switch (k)
                        {
                            case 0:
                            {
                                conf.Name = limit == 1 ? dnNamePrefix : dnNamePrefix + "[" + k + "]";
                                break;
                            }

                            default:
                            {
                                conf.Name = dnNamePrefix + "[" + k + "]";
                                break;
                            }
                        }
                        conf.DataSource = dsString.ToString();
                    }
                }
                catch (Exception e)
                {
                    throw new ConfigException("dataNode " + dnNamePrefix + " define error", e);
                }

                foreach (var conf_1 in confList)
                {
                    if (dataNodes.ContainsKey(conf_1.Name))
                    {
                        throw new ConfigException("dataNode " + conf_1.Name + " duplicated!");
                    }
                    dataNodes[conf_1.Name] = conf_1;
                }
            }
        }

        private void LoadSchemas(XmlElement root)
        {
            var list = root.GetElementsByTagName("schema");
            XmlElement schemaElement = null;
            IDictionary<string, TableConfig> tables = null;
            for (int i = 0, n = list.Count; i < n; i++)
            {
                schemaElement = (XmlElement)list.Item(i);
                var name = schemaElement.GetAttribute("name");
                var dataNode = schemaElement.GetAttribute("dataNode");
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
                var group = "default";
                if (schemaElement.HasAttribute("group"))
                {
                    group = schemaElement.GetAttribute("group").Trim();
                }
                tables = LoadTables(schemaElement);
                if (schemas.ContainsKey(name))
                {
                    throw new ConfigException("schema " + name + " duplicated!");
                }

                var keepSqlSchema = false;
                if (schemaElement.HasAttribute("keepSqlSchema"))
                {
                    keepSqlSchema = bool.Parse(schemaElement.GetAttribute("keepSqlSchema").Trim());
                }
                schemas[name] = new SchemaConfig(name, dataNode, group, keepSqlSchema, tables);
            }
        }

        private IDictionary<string, TableConfig> LoadTables(XmlElement node)
        {
            var tables = new Dictionary<string, TableConfig>();
            var nodeList = node.GetElementsByTagName("table");
            XmlElement tableElement = null;
            TableRuleConfig tableRule = null;
            for (var i = 0; i < nodeList.Count; i++)
            {
                tableElement = (XmlElement)nodeList.Item(i);
                var name = tableElement.GetAttribute("name").ToUpper();
                var dataNode = tableElement.GetAttribute("dataNode");
                tableRule = null;
                if (tableElement.HasAttribute("rule"))
                {
                    var ruleName = tableElement.GetAttribute("rule");
                    tableRule = tableRules[ruleName];
                    if (tableRule == null)
                    {
                        throw new ConfigException("rule " + ruleName + " is not found!");
                    }
                }

                var ruleRequired = false;
                if (tableElement.HasAttribute("ruleRequired"))
                {
                    ruleRequired = bool.Parse(tableElement.GetAttribute("ruleRequired"));
                }

                var tableNames = SplitUtil.Split(name, ',', true);
                foreach (var tableName in tableNames)
                {
                    var table = new TableConfig(tableName, dataNode, tableRule, ruleRequired);
                    CheckDataNodeExists(table.DataNodes);
                    if (tables.ContainsKey(table.Name))
                    {
                        throw new ConfigException("table " + tableName + " duplicated!");
                    }
                    tables[table.Name] = table;
                }
            }
            return tables;
        }

        private void CheckDataNodeExists(params string[] nodes)
        {
            if (nodes == null || nodes.Length < 1)
                return;

            foreach (var node in nodes)
            {
                if (!dataNodes.ContainsKey(node))
                {
                    throw new ConfigException("dataNode '" + node + "' is not found!");
                }
            }
        }

        private static XmlElement FindPropertyByName(XmlElement bean, string name)
        {
            var propertyList = bean.GetElementsByTagName("property");
            return propertyList.Cast<XmlNode>()
                               .FirstOrDefault(none => none.NodeType == XmlNodeType.Element
                                                       && ((XmlElement)none).GetAttribute("name") == name) as XmlElement;

            //XmlNode node = null;
            //XmlElement p = null;
            //for (int j = 0, m = propertyList.Count; j < m; ++j)
            //{
            //    node = propertyList.Item(j);
            //    if (node.NodeType == XmlNodeType.Element)
            //    {
            //        p = (XmlElement)node;
            //        if (name.Equals(p.GetAttribute("name")))
            //        {
            //            return p;
            //        }
            //    }
            //}
            //return null;
        }
    }
}