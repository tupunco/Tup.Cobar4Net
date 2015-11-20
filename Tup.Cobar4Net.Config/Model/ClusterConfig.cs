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
//using Org.W3c.Dom;
//using Sharpen;
//using Tup.Cobar4Net.Config.Util;
//using Tup.Cobar4Net.Util;

namespace Tup.Cobar4Net.Config.Model
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class ClusterConfig
    {
        //TODO ClusterConfig
        private readonly IDictionary<string, CobarNodeConfig> nodes = null;

        private readonly IDictionary<string, IList<string>> groups = null;

        //public ClusterConfig(Element root, int port)
        //{
        //	nodes =  Sharpen.Collections.UnmodifiableMap(LoadNode(root, port));
        //	groups = Sharpen.Collections.UnmodifiableMap(LoadGroup(root, nodes));
        //}

        public virtual IDictionary<string, CobarNodeConfig> GetNodes()
        {
            return nodes;
        }

        public virtual IDictionary<string, IList<string>> GetGroups()
        {
            return groups;
        }

        //private static IDictionary<string, CobarNodeConfig> LoadNode(Element root, int port
        //	)
        //{
        //	IDictionary<string, CobarNodeConfig> nodes = new Dictionary<string, CobarNodeConfig
        //		>();
        //	NodeList list = root.GetElementsByTagName("node");
        //	ICollection<string> hostSet = new HashSet<string>();
        //	for (int i = 0; i < n; i++)
        //	{
        //		Node node = list.Item(i);
        //		if (node is Element)
        //		{
        //			Element element = (Element)node;
        //			string name = element.GetAttribute("name").Trim();
        //			if (nodes.Contains(name))
        //			{
        //				throw new ConfigException("node name duplicated :" + name);
        //			}
        //			IDictionary<string, object> props = ConfigUtil.LoadElements(element);
        //			string host = (string)props["host"];
        //			if (null == host || string.Empty.Equals(host))
        //			{
        //				throw new ConfigException("host empty in node: " + name);
        //			}
        //			if (hostSet.Contains(host))
        //			{
        //				throw new ConfigException("node host duplicated :" + host);
        //			}
        //			string wei = (string)props["weight"];
        //			if (null == wei || string.Empty.Equals(wei))
        //			{
        //				throw new ConfigException("weight should not be null in host:" + host);
        //			}
        //			int weight = Sharpen.Extensions.ValueOf(wei);
        //			if (weight <= 0)
        //			{
        //				throw new ConfigException("weight should be > 0 in host:" + host + " weight:" + weight
        //					);
        //			}
        //			CobarNodeConfig conf = new CobarNodeConfig(name, host, port, weight);
        //			nodes[name] = conf;
        //			hostSet.Add(host);
        //		}
        //	}
        //	return nodes;
        //}

        //private static IDictionary<string, IList<string>> LoadGroup(Element root, IDictionary
        //	<string, CobarNodeConfig> nodes)
        //{
        //	IDictionary<string, IList<string>> groups = new Dictionary<string, IList<string>>
        //		();
        //	NodeList list = root.GetElementsByTagName("group");
        //	for (int i = 0; i < n; i++)
        //	{
        //		Node node = list.Item(i);
        //		if (node is Element)
        //		{
        //			Element e = (Element)node;
        //			string groupName = e.GetAttribute("name").Trim();
        //			if (groups.Contains(groupName))
        //			{
        //				throw new ConfigException("group duplicated : " + groupName);
        //			}
        //			IDictionary<string, object> props = ConfigUtil.LoadElements(e);
        //			string value = (string)props["nodeList"];
        //			if (null == value || string.Empty.Equals(value))
        //			{
        //				throw new ConfigException("group should contain 'nodeList'");
        //			}
        //			string[] sList = SplitUtil.Split(value, ',', true);
        //			if (null == sList || sList.Length == 0)
        //			{
        //				throw new ConfigException("group should contain 'nodeList'");
        //			}
        //			foreach (string s in sList)
        //			{
        //				if (!nodes.Contains(s))
        //				{
        //					throw new ConfigException("[ node :" + s + "] in [ group:" + groupName + "] doesn't exist!"
        //						);
        //				}
        //			}
        //			IList<string> nodeList = Arrays.AsList(sList);
        //			groups[groupName] = nodeList;
        //		}
        //	}
        //	if (!groups.Contains("default"))
        //	{
        //		IList<string> nodeList = new List<string>(nodes.Keys);
        //		groups["default"] = nodeList;
        //	}
        //	return groups;
        //}
    }
}
