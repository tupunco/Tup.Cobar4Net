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
            return tableRules.IsEmpty() ? new Dictionary<string, TableRuleConfig>(0) : tableRules.AsReadOnly();
        }

        public virtual ICollection<RuleConfig> ListRuleConfig()
        {
            return rules.IsEmpty() ? new HashSet<RuleConfig>() : rules.AsReadOnly();
        }

        public virtual IDictionary<string, RuleAlgorithm> GetFunctions()
        {
            return functions.IsEmpty() ? new Dictionary<string, RuleAlgorithm>(0) : functions.AsReadOnly();
        }

        private void Load(string dtdFile, string xmlFile)
        {
            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlFile);

                var nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
                nsmgr.AddNamespace("cobar", "http://cobar.alibaba.com/");
                var root = xmlDoc.SelectNodes("cobar:rule", nsmgr).Item(0) as XmlElement;

                LoadFunctions(root);
                LoadTableRules(root);
            }
            catch (Exception e)
            {
                throw new ConfigException(e);
            }
        }

        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        private void LoadTableRules(XmlElement root)
        {
            var list = root.GetElementsByTagName("tableRule");
            XmlNode node = null;
            XmlElement e = null;
            for (int i = 0, n = list.Count; i < n; i++)
            {
                node = list.Item(i);
                if (node.NodeType == XmlNodeType.Element)
                {
                    e = (XmlElement)node;
                    string name = e.GetAttribute("name");
                    if (tableRules.ContainsKey(name))
                    {
                        throw new ConfigException("table rule " + name + " duplicated!");
                    }

                    var ruleNodes = e.GetElementsByTagName("rule");
                    var length = ruleNodes.Count;
                    var ruleList = new List<RuleConfig>(length);
                    for (int j = 0; j < length; ++j)
                    {
                        var rule = LoadRule((XmlElement)ruleNodes.Item(j));
                        ruleList.Add(rule);
                        rules.Add(rule);
                    }
                    tableRules[name] = new TableRuleConfig(name, ruleList);
                }
            }
        }

        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        private RuleConfig LoadRule(XmlElement element)
        {
            var columnsEle = ConfigUtil.LoadElement(element, "columns");
            var columns = SplitUtil.Split(columnsEle.InnerText, ',', true);
            for (int i = 0; i < columns.Length; ++i)
            {
                columns[i] = columns[i].ToUpper();
            }
            var algorithmEle = ConfigUtil.LoadElement(element, "algorithm");
            var algorithm = algorithmEle.InnerText;
            return new RuleConfig(columns, algorithm);
        }

        /// <exception cref="System.TypeLoadException"/>
        /// <exception cref="Sharpen.InstantiationException"/>
        /// <exception cref="System.MemberAccessException"/>
        /// <exception cref="System.Reflection.TargetInvocationException"/>
        private void LoadFunctions(XmlElement root)
        {
            var list = root.GetElementsByTagName("function");
            XmlNode node = null;
            XmlElement e = null;
            for (int i = 0, n = list.Count; i < n; i++)
            {
                node = list.Item(i);
                if (node.NodeType == XmlNodeType.Element)
                {
                    e = (XmlElement)node;
                    string name = e.GetAttribute("name");
                    if (functions.ContainsKey(name))
                    {
                        throw new ConfigException("rule function " + name + " duplicated!");
                    }
                    var clazz = e.GetAttribute("class");
                    var function = CreateFunction(name, clazz);

                    ParameterMapping.Mapping(function, ConfigUtil.LoadElements(e));

                    functions[name] = function;
                }
            }
        }

        /// <exception cref="ConfigException"/>
        /// <exception cref="System.TypeLoadException"/>
        /// <exception cref="System.MemberAccessException"/>
        /// <exception cref="System.Reflection.TargetInvocationException"/>
        private RuleAlgorithm CreateFunction(string name, string clazz)
        {
            var clz = Type.GetType(clazz);
            if (!typeof(RuleAlgorithm).IsAssignableFrom(clz))
            {
                throw new ArgumentException("rule function must implements "
                                                + typeof(RuleAlgorithm).FullName + ", name=" + name);
            }

            var constructor = clz.GetConstructor(new Type[] { typeof(string) });
            if (constructor == null)
            {
                throw new ConfigException("function " + name + " with class of " + clazz + " must have a constructor with one parameter: String funcName");
            }
            return (RuleAlgorithm)constructor.Invoke(new object[] { name });
        }
    }
}
