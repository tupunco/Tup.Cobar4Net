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
using System.Xml;

using Tup.Cobar4Net.Config.Model;
using Tup.Cobar4Net.Config.Util;
using Tup.Cobar4Net.Util;

namespace Tup.Cobar4Net.Config.Loader.Xml
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class XMLServerLoader
    {
        private const string DefaultDtd = "/server.dtd";

        private const string DefaultXml = "/server.xml";

        private readonly SystemConfig system;

        private readonly IDictionary<string, UserConfig> users;

        private readonly QuarantineConfig quarantine;

        private ClusterConfig cluster;

        public XMLServerLoader()
        {
            this.system = new SystemConfig();
            this.users = new Dictionary<string, UserConfig>();
            this.quarantine = new QuarantineConfig();

            this.Load();
        }

        public virtual SystemConfig GetSystem()
        {
            return system;
        }

        public virtual IDictionary<string, UserConfig> GetUsers()
        {
            return users.IsEmpty() ? new Dictionary<string, UserConfig>(0) : users.AsReadOnly();
        }

        public virtual QuarantineConfig GetQuarantine()
        {
            return quarantine;
        }

        public virtual ClusterConfig GetCluster()
        {
            return cluster;
        }

        private void Load()
        {
            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(DefaultXml);

                var nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
                nsmgr.AddNamespace("cobar", "http://cobar.alibaba.com/");
                var root = xmlDoc.SelectNodes("cobar:server", nsmgr).Item(0) as XmlElement;

                LoadSystem(root);

                #region Load ClusterConfig
                var port = system.GetServerPort();
                var nodes = LoadClusterNode(root, port).AsReadOnly();
                var groups = LoadClusterGroup(root, nodes).AsReadOnly();
                this.cluster = new ClusterConfig(nodes, groups);
                #endregion

                LoadQuarantine(root);
            }
            catch (Exception e)
            {
                throw new ConfigException(e);
            }
        }

        private static IDictionary<string, CobarNodeConfig> LoadClusterNode(XmlElement root, int port)
        {
            var nodes = new Dictionary<string, CobarNodeConfig>();
            var list = root.GetElementsByTagName("node");
            ICollection<string> hostSet = new HashSet<string>();
            XmlNode node = null;
            XmlElement element = null;
            for (int i = 0, n = list.Count; i < n; i++)
            {
                node = list.Item(i);
                if (node.NodeType == XmlNodeType.Element)
                {
                    element = (XmlElement)node;
                    string name = element.GetAttribute("name").Trim();
                    if (nodes.ContainsKey(name))
                    {
                        throw new ConfigException("node name duplicated :" + name);
                    }

                    var props = ConfigUtil.LoadElements(element);
                    var host = (string)props.GetValue("host");
                    if (host.IsEmpty())
                    {
                        throw new ConfigException("host empty in node: " + name);
                    }
                    if (hostSet.Contains(host))
                    {
                        throw new ConfigException("node host duplicated :" + host);
                    }
                    var wei = (string)props.GetValue("weight");
                    if (wei.IsEmpty())
                    {
                        throw new ConfigException("weight should not be null in host:" + host);
                    }
                    int weight = wei.ParseToInt32(1);
                    if (weight <= 0)
                    {
                        throw new ConfigException("weight should be > 0 in host:" + host + " weight:" + weight);
                    }

                    nodes[name] = new CobarNodeConfig(name, host, port, weight);
                    hostSet.Add(host);
                }
            }
            return nodes;
        }

        private static IDictionary<string, IList<string>> LoadClusterGroup(XmlElement root,
                                                                    IDictionary<string, CobarNodeConfig> nodes)
        {
            var groups = new Dictionary<string, IList<string>>();
            var list = root.GetElementsByTagName("group");
            XmlNode node = null;
            XmlElement e = null;
            for (int i = 0, n = list.Count; i < n; i++)
            {
                node = list.Item(i);
                if (node.NodeType == XmlNodeType.Element)
                {
                    e = (XmlElement)node;
                    string groupName = e.GetAttribute("name").Trim();
                    if (groups.ContainsKey(groupName))
                    {
                        throw new ConfigException("group duplicated : " + groupName);
                    }

                    var props = ConfigUtil.LoadElements(e);
                    string value = (string)props.GetValue("nodeList");
                    if (value.IsEmpty())
                    {
                        throw new ConfigException("group should contain 'nodeList'");
                    }

                    string[] sList = SplitUtil.Split(value, ',', true);
                    if (null == sList || sList.Length == 0)
                    {
                        throw new ConfigException("group should contain 'nodeList'");
                    }
                    foreach (string s in sList)
                    {
                        if (!nodes.ContainsKey(s))
                        {
                            throw new ConfigException("[ node :" + s + "] in [ group:" + groupName + "] doesn't exist!");
                        }
                    }

                    groups[groupName] = sList;
                }
            }
            if (!groups.ContainsKey("default"))
            {
                groups["default"] = new List<string>(nodes.Keys);
            }
            return groups;
        }

        /// <exception cref="System.MemberAccessException"/>
        /// <exception cref="System.Reflection.TargetInvocationException"/>
        private void LoadSystem(XmlElement root)
        {
            var list = root.GetElementsByTagName("system");
            for (int i = 0, n = list.Count; i < n; i++)
            {
                var node = list.Item(i);
                if (node.NodeType == XmlNodeType.Element)
                {
                    var props = ConfigUtil.LoadElements((XmlElement)node);
                    ParameterMapping.Mapping(system, props);
                }
            }
        }

        private void LoadUsers(XmlElement root)
        {
            var list = root.GetElementsByTagName("user");
            XmlNode node = null;
            XmlElement e = null;
            for (int i = 0, n = list.Count; i < n; i++)
            {
                node = list.Item(i);
                if (node.NodeType == XmlNodeType.Element)
                {
                    e = (XmlElement)node;
                    var name = e.GetAttribute("name");
                    var user = new UserConfig();
                    user.SetName(name);
                    var props = ConfigUtil.LoadElements(e);
                    user.SetPassword((string)props.GetValue("password"));

                    string schemas = (string)props.GetValue("schemas");
                    if (schemas != null)
                    {
                        string[] strArray = SplitUtil.Split(schemas, ',', true);
                        user.SetSchemas(new HashSet<string>(strArray));
                    }
                    if (users.ContainsKey(name))
                    {
                        throw new ConfigException("user " + name + " duplicated!");
                    }
                    users[name] = user;
                }
            }
        }

        private void LoadQuarantine(XmlElement root)
        {
            var list = root.GetElementsByTagName("host");
            XmlNode node = null;
            XmlElement e = null;
            for (int i = 0, n = list.Count; i < n; i++)
            {
                node = list.Item(i);
                if (node.NodeType == XmlNodeType.Element)
                {
                    e = (XmlElement)node;

                    var host = e.GetAttribute("name").Trim();
                    if (quarantine.GetHosts().ContainsKey(host))
                    {
                        throw new ConfigException("host duplicated : " + host);
                    }

                    var props = ConfigUtil.LoadElements(e);
                    string[] users = SplitUtil.Split((string)props["user"], ',', true);
                    HashSet<string> set = new HashSet<string>();
                    if (null != users)
                    {
                        foreach (string user in users)
                        {
                            var uc = this.users.GetValue(user);
                            if (null == uc)
                            {
                                throw new ConfigException("[user: " + user + "] doesn't exist in [host: " + host + "]");
                            }
                            if (null == uc.GetSchemas() || uc.GetSchemas().Count == 0)
                            {
                                throw new ConfigException("[host: " + host + "] contains one root privileges user: " + user);
                            }
                            if (set.Contains(user))
                            {
                                throw new ConfigException("[host: " + host + "] contains duplicate user: " + user);
                            }
                            else
                            {
                                set.Add(user);
                            }
                        }
                    }
                    quarantine.GetHosts()[host] = set;
                }
            }
        }
    }
}
