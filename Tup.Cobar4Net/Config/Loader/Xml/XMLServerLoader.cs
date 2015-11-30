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
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public class XmlServerLoader
    {
        private const string DefaultDtd = "/server.dtd";

        private const string DefaultXml = "/server.xml";

        private readonly IDictionary<string, UserConfig> users;

        public XmlServerLoader()
        {
            System = new SystemConfig();
            users = new Dictionary<string, UserConfig>();
            Quarantine = new QuarantineConfig();

            Load();
        }

        public SystemConfig System { get; }

        public IDictionary<string, UserConfig> Users
        {
            get { return users.IsEmpty() ? new Dictionary<string, UserConfig>(0) : users.AsReadOnly(); }
        }

        public QuarantineConfig Quarantine { get; }

        public ClusterConfig Cluster { get; private set; }

        private void Load()
        {
            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(DefaultXml);

                var nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
                nsmgr.AddNamespace("cobar", "http://cobar.alibaba.com/");
                var xmlNodeList = xmlDoc.SelectNodes("cobar:server", nsmgr);
                if (xmlNodeList == null)
                    return;

                var root = xmlNodeList.Item(0) as XmlElement;
                LoadSystem(root);

                #region Load ClusterConfig

                var port = System.ServerPort;
                var nodes = LoadClusterNode(root, port).AsReadOnly();
                var groups = LoadClusterGroup(root, nodes).AsReadOnly();
                Cluster = new ClusterConfig(nodes, groups);

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
                if (node.NodeType != XmlNodeType.Element)
                    continue;

                element = (XmlElement)node;
                var name = element.GetAttribute("name").Trim();
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
                var weight = wei.ParseToInt32(1);
                if (weight <= 0)
                {
                    throw new ConfigException("weight should be > 0 in host:" + host + " weight:" + weight);
                }

                nodes[name] = new CobarNodeConfig(name, host, port, weight);
                hostSet.Add(host);
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
                if (node.NodeType != XmlNodeType.Element)
                    continue;

                e = (XmlElement)node;
                var groupName = e.GetAttribute("name").Trim();
                if (groups.ContainsKey(groupName))
                {
                    throw new ConfigException("group duplicated : " + groupName);
                }

                var props = ConfigUtil.LoadElements(e);
                var value = (string)props.GetValue("nodeList");
                if (value.IsEmpty())
                {
                    throw new ConfigException("group should contain 'nodeList'");
                }

                var sList = SplitUtil.Split(value, ',', true);
                if (null == sList || sList.Length == 0)
                {
                    throw new ConfigException("group should contain 'nodeList'");
                }
                foreach (var s in sList)
                {
                    if (!nodes.ContainsKey(s))
                    {
                        throw new ConfigException("[ node :" + s + "] in [ group:" + groupName + "] doesn't exist!");
                    }
                }

                groups[groupName] = sList;
            }
            if (!groups.ContainsKey("default"))
            {
                groups["default"] = new List<string>(nodes.Keys);
            }
            return groups;
        }

        /// <exception cref="System.MemberAccessException" />
        /// <exception cref="System.Reflection.TargetInvocationException" />
        private void LoadSystem(XmlElement root)
        {
            var list = root.GetElementsByTagName("system");
            for (int i = 0, n = list.Count; i < n; i++)
            {
                var node = list.Item(i);
                if (node.NodeType != XmlNodeType.Element)
                    continue;

                var props = ConfigUtil.LoadElements((XmlElement)node);
                ParameterMapping.Mapping(System, props);
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
                if (node.NodeType != XmlNodeType.Element)
                    continue;

                e = (XmlElement)node;
                var name = e.GetAttribute("name");
                var user = new UserConfig();
                user.Name = name;
                var props = ConfigUtil.LoadElements(e);
                user.Password = (string)props.GetValue("password");

                var schemas = (string)props.GetValue("schemas");
                if (schemas != null)
                {
                    var strArray = SplitUtil.Split(schemas, ',', true);
                    user.Schemas = new HashSet<string>(strArray);
                }
                if (users.ContainsKey(name))
                {
                    throw new ConfigException("user " + name + " duplicated!");
                }
                users[name] = user;
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
                if (node.NodeType != XmlNodeType.Element)
                    continue;

                e = (XmlElement)node;

                var host = e.GetAttribute("name").Trim();
                if (Quarantine.Hosts.ContainsKey(host))
                {
                    throw new ConfigException("host duplicated : " + host);
                }

                var props = ConfigUtil.LoadElements(e);
                var users = SplitUtil.Split((string)props["user"], ',', true);
                var set = new HashSet<string>();
                if (null != users)
                {
                    foreach (var user in users)
                    {
                        var uc = this.users.GetValue(user);
                        if (null == uc)
                        {
                            throw new ConfigException("[user: " + user + "] doesn't exist in [host: " + host + "]");
                        }
                        if (null == uc.Schemas || uc.Schemas.Count == 0)
                        {
                            throw new ConfigException("[host: " + host + "] contains one root privileges user: " + user);
                        }
                        if (set.Contains(user))
                        {
                            throw new ConfigException("[host: " + host + "] contains duplicate user: " + user);
                        }
                        set.Add(user);
                    }
                }
                Quarantine.Hosts[host] = set;
            }
        }
    }
}