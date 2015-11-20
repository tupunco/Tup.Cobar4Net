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
using Sharpen.Reflect;
using Tup.Cobar4Net.Config.Model.Rule;
using Tup.Cobar4Net.Config.Util;
using Tup.Cobar4Net.Util;

namespace Tup.Cobar4Net.Config.Loader.Xml
{
	/// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
	public class XMLRuleLoader
	{
		private const string DefaultDtd = "/rule.dtd";

		private const string DefaultXml = "/rule.xml";

		private readonly IDictionary<string, TableRuleConfig> tableRules;

		private readonly ICollection<RuleConfig> rules;

		private readonly IDictionary<string, RuleAlgorithm> functions;

		public XMLRuleLoader(string ruleFile)
		{
			this.rules = new HashSet<RuleConfig>();
			this.tableRules = new Dictionary<string, TableRuleConfig>();
			this.functions = new Dictionary<string, RuleAlgorithm>();
			Load(DefaultDtd, ruleFile == null ? DefaultXml : ruleFile);
		}

		public XMLRuleLoader()
			: this(null)
		{
		}

		public virtual IDictionary<string, TableRuleConfig> GetTableRules()
		{
			return (IDictionary<string, TableRuleConfig>)(tableRules.IsEmpty() ? Sharpen.Collections
				.EmptyMap() : tableRules);
		}

		public virtual ICollection<RuleConfig> ListRuleConfig()
		{
			return (ICollection<RuleConfig>)((rules == null || rules.IsEmpty()) ? Sharpen.Collections
				.EmptySet() : rules);
		}

		public virtual IDictionary<string, RuleAlgorithm> GetFunctions()
		{
			return (IDictionary<string, RuleAlgorithm>)(functions.IsEmpty() ? Sharpen.Collections
				.EmptyMap() : functions);
		}

		private void Load(string dtdFile, string xmlFile)
		{
			InputStream dtd = null;
			InputStream xml = null;
			try
			{
				dtd = typeof(Tup.Cobar4Net.Config.Loader.Xml.XMLRuleLoader).GetResourceAsStream(dtdFile
					);
				xml = typeof(Tup.Cobar4Net.Config.Loader.Xml.XMLRuleLoader).GetResourceAsStream(xmlFile
					);
				Element root = ConfigUtil.GetDocument(dtd, xml).GetDocumentElement();
				LoadFunctions(root);
				LoadTableRules(root);
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

		/// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
		private void LoadTableRules(Element root)
		{
			NodeList list = root.GetElementsByTagName("tableRule");
			for (int i = 0; i < n; ++i)
			{
				Node node = list.Item(i);
				if (node is Element)
				{
					Element e = (Element)node;
					string name = e.GetAttribute("name");
					if (tableRules.Contains(name))
					{
						throw new ConfigException("table rule " + name + " duplicated!");
					}
					NodeList ruleNodes = e.GetElementsByTagName("rule");
					int length = ruleNodes.GetLength();
					IList<RuleConfig> ruleList = new List<RuleConfig>(length);
					for (int j = 0; j < length; ++j)
					{
						RuleConfig rule = LoadRule((Element)ruleNodes.Item(j));
						ruleList.Add(rule);
						rules.Add(rule);
					}
					tableRules[name] = new TableRuleConfig(name, ruleList);
				}
			}
		}

		/// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
		private RuleConfig LoadRule(Element element)
		{
			Element columnsEle = ConfigUtil.LoadElement(element, "columns");
			string[] columns = SplitUtil.Split(columnsEle.GetTextContent(), ',', true);
			for (int i = 0; i < columns.Length; ++i)
			{
				columns[i] = columns[i].ToUpper();
			}
			Element algorithmEle = ConfigUtil.LoadElement(element, "algorithm");
			string algorithm = algorithmEle.GetTextContent();
			return new RuleConfig(columns, algorithm);
		}

		/// <exception cref="System.TypeLoadException"/>
		/// <exception cref="Sharpen.InstantiationException"/>
		/// <exception cref="System.MemberAccessException"/>
		/// <exception cref="System.Reflection.TargetInvocationException"/>
		private void LoadFunctions(Element root)
		{
			NodeList list = root.GetElementsByTagName("function");
			for (int i = 0; i < n; ++i)
			{
				Node node = list.Item(i);
				if (node is Element)
				{
					Element e = (Element)node;
					string name = e.GetAttribute("name");
					if (functions.Contains(name))
					{
						throw new ConfigException("rule function " + name + " duplicated!");
					}
					string clazz = e.GetAttribute("class");
					RuleAlgorithm function = CreateFunction(name, clazz);
					ParameterMapping.Mapping(function, ConfigUtil.LoadElements(e));
					functions[name] = function;
				}
			}
		}

		/// <exception cref="System.TypeLoadException"/>
		/// <exception cref="Sharpen.InstantiationException"/>
		/// <exception cref="System.MemberAccessException"/>
		/// <exception cref="System.Reflection.TargetInvocationException"/>
		private RuleAlgorithm CreateFunction(string name, string clazz)
		{
			Type clz = Sharpen.Runtime.GetType(clazz);
			if (!typeof(RuleAlgorithm).IsAssignableFrom(clz))
			{
				throw new ArgumentException("rule function must implements " + typeof(RuleAlgorithm
					).FullName + ", name=" + name);
			}
			Constructor<object> constructor = null;
			foreach (Constructor<object> cons in clz.GetConstructors())
			{
				Type[] paraClzs = cons.GetParameterTypes();
				if (paraClzs != null && paraClzs.Length == 1)
				{
					Type paraClzs1 = paraClzs[0];
					if (typeof(string).IsAssignableFrom(paraClzs1))
					{
						constructor = cons;
						break;
					}
				}
			}
			if (constructor == null)
			{
				throw new ConfigException("function " + name + " with class of " + clazz + " must have a constructor with one parameter: String funcName"
					);
			}
			return (RuleAlgorithm)constructor.NewInstance(name);
		}
	}
}
