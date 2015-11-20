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
using Org.W3c.Dom;
using Sharpen;
using Tup.Cobar4Net.Config.Model;
using Tup.Cobar4Net.Config.Util;
using Tup.Cobar4Net.Util;

namespace Tup.Cobar4Net.Config.Loader.Xml
{
	/// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
	public class XMLServerLoader
	{
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
			return (IDictionary<string, UserConfig>)(users.IsEmpty() ? Sharpen.Collections.EmptyMap
				() : Sharpen.Collections.UnmodifiableMap(users));
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
			InputStream dtd = null;
			InputStream xml = null;
			try
			{
				dtd = typeof(Tup.Cobar4Net.Config.Loader.Xml.XMLServerLoader).GetResourceAsStream
					("/server.dtd");
				xml = typeof(Tup.Cobar4Net.Config.Loader.Xml.XMLServerLoader).GetResourceAsStream
					("/server.xml");
				Element root = ConfigUtil.GetDocument(dtd, xml).GetDocumentElement();
				LoadSystem(root);
				LoadUsers(root);
				this.cluster = new ClusterConfig(root, system.GetServerPort());
				LoadQuarantine(root);
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

		private void LoadQuarantine(Element root)
		{
			NodeList list = root.GetElementsByTagName("host");
			for (int i = 0; i < n; i++)
			{
				Node node = list.Item(i);
				if (node is Element)
				{
					Element e = (Element)node;
					string host = e.GetAttribute("name").Trim();
					if (quarantine.GetHosts().Contains(host))
					{
						throw new ConfigException("host duplicated : " + host);
					}
					IDictionary<string, object> props = ConfigUtil.LoadElements(e);
					string[] users = SplitUtil.Split((string)props["user"], ',', true);
					HashSet<string> set = new HashSet<string>();
					if (null != users)
					{
						foreach (string user in users)
						{
							UserConfig uc = this.users[user];
							if (null == uc)
							{
								throw new ConfigException("[user: " + user + "] doesn't exist in [host: " + host 
									+ "]");
							}
							if (null == uc.GetSchemas() || uc.GetSchemas().Count == 0)
							{
								throw new ConfigException("[host: " + host + "] contains one root privileges user: "
									 + user);
							}
							if (set.Contains(user))
							{
								throw new ConfigException("[host: " + host + "] contains duplicate user: " + user
									);
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

		private void LoadUsers(Element root)
		{
			NodeList list = root.GetElementsByTagName("user");
			for (int i = 0; i < n; i++)
			{
				Node node = list.Item(i);
				if (node is Element)
				{
					Element e = (Element)node;
					string name = e.GetAttribute("name");
					UserConfig user = new UserConfig();
					user.SetName(name);
					IDictionary<string, object> props = ConfigUtil.LoadElements(e);
					user.SetPassword((string)props["password"]);
					string schemas = (string)props["schemas"];
					if (schemas != null)
					{
						string[] strArray = SplitUtil.Split(schemas, ',', true);
						user.SetSchemas(new HashSet<string>(Arrays.AsList(strArray)));
					}
					if (users.Contains(name))
					{
						throw new ConfigException("user " + name + " duplicated!");
					}
					users[name] = user;
				}
			}
		}

		/// <exception cref="System.MemberAccessException"/>
		/// <exception cref="System.Reflection.TargetInvocationException"/>
		private void LoadSystem(Element root)
		{
			NodeList list = root.GetElementsByTagName("system");
			for (int i = 0; i < n; i++)
			{
				Node node = list.Item(i);
				if (node is Element)
				{
					IDictionary<string, object> props = ConfigUtil.LoadElements((Element)node);
					ParameterMapping.Mapping(system, props);
				}
			}
		}
	}
}
