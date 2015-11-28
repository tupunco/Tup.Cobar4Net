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
using Tup.Cobar4Net.Util;
using System.Xml;
using System;

#if CONFG_BEAN
using System.Text;
using Sharpen;
#endif

namespace Tup.Cobar4Net.Config.Util
{
    /// <author>xianmao.hexm 2011-1-10 下午03:35:06</author>
    public class ConfigUtil
    {
        public static string Filter(string text)
        {
            return text;
            //TODO ConfigUtil return Filter(text, Runtime.GetProperties());
        }

        //public static string Filter(string text, Properties properties)
        //{
        //    StringBuilder s = new StringBuilder();
        //    int cur = 0;
        //    int textLen = text.Length;
        //    int propStart = -1;
        //    int propStop = -1;
        //    string propName = null;
        //    string propValue = null;
        //    for (; cur < textLen; cur = propStop + 1)
        //    {
        //        propStart = text.IndexOf("${", cur, System.StringComparison.Ordinal);
        //        if (propStart < 0)
        //        {
        //            break;
        //        }
        //        s.Append(Sharpen.Runtime.Substring(text, cur, propStart));
        //        propStop = text.IndexOf("}", propStart, System.StringComparison.Ordinal);
        //        if (propStop < 0)
        //        {
        //            throw new ConfigException("Unterminated property: " +
        //                                            Sharpen.Runtime.Substring(text, propStart));
        //        }
        //        propName = Sharpen.Runtime.Substring(text, propStart + 2, propStop);
        //        propValue = properties.GetProperty(propName);
        //        if (propValue == null)
        //        {
        //            s.Append("${").Append(propName).Append('}');
        //        }
        //        else
        //        {
        //            s.Append(propValue);
        //        }
        //    }
        //    return s.Append(Sharpen.Runtime.Substring(text, cur)).ToString();
        //}

        ///// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
        ///// <exception cref="Org.Xml.Sax.SAXException"/>
        ///// <exception cref="System.IO.IOException"/>
        //public static Document GetDocument(InputStream dtd, InputStream xml)
        //{
        //	DocumentBuilderFactory factory = DocumentBuilderFactory.NewInstance();
        //	factory.SetValidating(true);
        //	factory.SetNamespaceAware(false);
        //	DocumentBuilder builder = factory.NewDocumentBuilder();
        //	builder.SetEntityResolver(new _EntityResolver_86(dtd));
        //	builder.SetErrorHandler(new _ErrorHandler_92());
        //	return builder.Parse(xml);
        //}

        //private sealed class _EntityResolver_86 : EntityResolver
        //{
        //	public _EntityResolver_86(InputStream dtd)
        //	{
        //		this.dtd = dtd;
        //	}

        //	public InputSource ResolveEntity(string publicId, string systemId)
        //	{
        //		return new InputSource(dtd);
        //	}

        //	private readonly InputStream dtd;
        //}

        //private sealed class _ErrorHandler_92 : ErrorHandler
        //{
        //	public _ErrorHandler_92()
        //	{
        //	}

        //	public void Warning(SAXParseException e)
        //	{
        //	}

        //	/// <exception cref="Org.Xml.Sax.SAXException"/>
        //	public void Error(SAXParseException e)
        //	{
        //		throw e;
        //	}

        //	/// <exception cref="Org.Xml.Sax.SAXException"/>
        //	public void FatalError(SAXParseException e)
        //	{
        //		throw e;
        //	}
        //}

        //public static IDictionary<string, object> LoadAttributes(Element e)
        //{
        //	IDictionary<string, object> map = new Dictionary<string, object>();
        //	NamedNodeMap nm = e.GetAttributes();
        //	for (int j = 0; j < nm.GetLength(); j++)
        //	{
        //		Node n = nm.Item(j);
        //		if (n is Attr)
        //		{
        //			Attr attr = (Attr)n;
        //			map[attr.GetName()] = attr.GetNodeValue();
        //		}
        //	}
        //	return map;
        //}

        public static XmlElement LoadElement(XmlElement parent, string tagName)
        {
            var nodeList = parent.GetElementsByTagName(tagName);
            if (nodeList.Count > 1)
                throw new ConfigException(tagName + " elements length  over one!");

            if (nodeList.Count == 1)
                return (XmlElement)nodeList.Item(0);
            else
                return null;
        }

        public static IDictionary<string, object> LoadElements(XmlElement parent)
        {
            var map = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            var children = parent.ChildNodes;
            XmlNode node = null;
            XmlElement e = null;
            for (int i = 0; i < children.Count; i++)
            {
                node = children.Item(i);
                if (node.NodeType == XmlNodeType.Element)
                {
                    e = (XmlElement)node;
                    string name = e.Name;
                    if ("property".Equals(name))
                    {
                        var key = e.GetAttribute("name");

#if CONFG_BEAN
                        var nl = e.GetElementsByTagName("bean");
                        if (nl.Count == 0 )
#endif
                        {
                            //|| (nl.Count == 1 && nl.Item(0).NodeType == XmlNodeType.Text)
                            string value = e.InnerText;
                            map[key] = StringUtil.IsEmpty(value) ? null : value.Trim();
                        }
#if CONFG_BEAN
else
                        {
                            map[key] = LoadBean((XmlElement)nl.Item(0));
                        }
#endif
                    }
                }
            }
            return map;
        }

#if CONFG_BEAN
        public static BeanConfig LoadBean(XmlElement parent, string tagName)
        {
            var nodeList = parent.GetElementsByTagName(tagName);
            if (nodeList.Count > 1)
            {
                throw new ConfigException(tagName + " elements length over one!");
            }
            return LoadBean((XmlElement)nodeList.Item(0));
        }

        public static BeanConfig LoadBean(XmlElement e)
        {
            if (e == null)
                return null;

            var bean = new BeanConfig();
            bean.SetName(e.GetAttribute("name"));
            var element = LoadElement(e, "className");

            if (element != null)
                bean.SetClassName(element.InnerText);
            else
                bean.SetClassName(e.GetAttribute("class"));

            bean.SetParams(LoadElements(e));
            return bean;
        }
#endif
    }
}
