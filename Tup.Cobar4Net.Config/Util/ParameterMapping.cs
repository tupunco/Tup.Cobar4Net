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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Java.Beans;
using Sharpen;
using Tup.Cobar4Net.Util;
using System.ComponentModel;

namespace Tup.Cobar4Net.Config.Util
{
	public class ParameterMapping
	{
		private static readonly IDictionary<Type, PropertyDescriptor[]> descriptors = new 
			Dictionary<Type, PropertyDescriptor[]>();

		/// <exception cref="System.MemberAccessException"/>
		/// <exception cref="System.Reflection.TargetInvocationException"/>
		public static void Mapping(object @object, IDictionary<string, object> parameter)
		{
			PropertyDescriptor[] pds = GetDescriptors(@object.GetType());
			for (int i = 0; i < pds.Length; i++)
			{
				PropertyDescriptor pd = pds[i];
				object obj = parameter[pd.GetName()];
				object value = obj;
				Type cls = pd.GetPropertyType();
				if (obj is string)
				{
					string @string = (string)obj;
					if (!StringUtil.IsEmpty(@string))
					{
						@string = ConfigUtil.Filter(@string);
					}
					if (IsPrimitiveType(cls))
					{
						value = Convert(cls, @string);
					}
				}
				else
				{
					if (obj is BeanConfig)
					{
						value = CreateBean((BeanConfig)obj);
					}
					else
					{
						if (obj is BeanConfig[])
						{
							IList<object> list = new List<object>();
							foreach (BeanConfig beanconfig in (BeanConfig[])obj)
							{
								list.Add(CreateBean(beanconfig));
							}
							value = Sharpen.Collections.ToArray(list);
						}
					}
				}
				if (cls != null)
				{
					if (value != null)
					{
						MethodInfo method = pd.GetWriteMethod();
						if (method != null)
						{
							method.Invoke(@object, new object[] { value });
						}
					}
				}
			}
		}

		/// <exception cref="System.MemberAccessException"/>
		/// <exception cref="System.Reflection.TargetInvocationException"/>
		public static object CreateBean(BeanConfig config)
		{
			object bean = config.Create(true);
			if (bean is IDictionary)
			{
				IDictionary<string, object> map = (IDictionary<string, object>)bean;
				foreach (KeyValuePair<string, object> entry in config.GetParams())
				{
					string key = entry.Key;
					object value = entry.Value;
					if (value is BeanConfig)
					{
						BeanConfig mapBeanConfig = (BeanConfig)entry.Value;
						value = mapBeanConfig.Create(true);
						Mapping(value, mapBeanConfig.GetParams());
					}
					map[key] = value;
				}
			}
			else
			{
				if (bean is IList)
				{
				}
				else
				{
					Mapping(bean, config.GetParams());
				}
			}
			return bean;
		}

		private static PropertyDescriptor[] GetDescriptors(Type clazz)
		{
			PropertyDescriptor[] pds;
			IList<PropertyDescriptor> list;
			PropertyDescriptor[] pds2 = descriptors[clazz];
			if (null == pds2)
			{
				try
				{
					BeanInfo beanInfo = Introspector.GetBeanInfo(clazz);
					pds = beanInfo.GetPropertyDescriptors();
					list = new List<PropertyDescriptor>();
					for (int i = 0; i < pds.Length; i++)
					{
						if (null != pds[i].GetPropertyType())
						{
							list.Add(pds[i]);
						}
					}
					pds2 = new PropertyDescriptor[list.Count];
					Sharpen.Collections.ToArray(list, pds2);
				}
				catch (IntrospectionException ie)
				{
					Sharpen.Runtime.PrintStackTrace(ie);
					pds2 = new PropertyDescriptor[0];
				}
			}
			descriptors[clazz] = pds2;
			return (pds2);
		}

		private static object Convert(Type cls, string @string)
		{
			MethodInfo method = null;
			object value = null;
			if (cls.Equals(typeof(string)))
			{
				value = @string;
			}
			else
			{
				if (cls.Equals(typeof(bool)))
				{
					value = Sharpen.Extensions.ValueOf(@string);
				}
				else
				{
					if (cls.Equals(typeof(byte)))
					{
						value = byte.ValueOf(@string);
					}
					else
					{
						if (cls.Equals(typeof(short)))
						{
							value = short.ValueOf(@string);
						}
						else
						{
							if (cls.Equals(typeof(int)))
							{
								value = Sharpen.Extensions.ValueOf(@string);
							}
							else
							{
								if (cls.Equals(typeof(long)))
								{
									value = Sharpen.Extensions.ValueOf(@string);
								}
								else
								{
									if (cls.Equals(typeof(double)))
									{
										value = double.ValueOf(@string);
									}
									else
									{
										if (cls.Equals(typeof(float)))
										{
											value = float.ValueOf(@string);
										}
										else
										{
											if ((cls.Equals(typeof(bool))) || (cls.Equals(typeof(byte))) || (cls.Equals(typeof(
												short))) || (cls.Equals(typeof(int))) || (cls.Equals(typeof(long))) || (cls.Equals
												(typeof(float))) || (cls.Equals(typeof(double))))
											{
												try
												{
													method = cls.GetMethod("valueOf", new Type[] { typeof(string) });
													value = method.Invoke(null, new object[] { @string });
												}
												catch
												{
													value = null;
												}
											}
											else
											{
												if (cls.Equals(typeof(Type)))
												{
													try
													{
														value = Sharpen.Runtime.GetType(@string);
													}
													catch (TypeLoadException e)
													{
														throw new ConfigException(e);
													}
												}
												else
												{
													value = null;
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			return (value);
		}

		private static bool IsPrimitiveType(Type cls)
		{
			if (cls.Equals(typeof(string)) || cls.Equals(typeof(bool)) || cls.Equals(typeof(byte
				)) || cls.Equals(typeof(short)) || cls.Equals(typeof(int)) || cls.Equals(typeof(
				long)) || cls.Equals(typeof(double)) || cls.Equals(typeof(float)) || cls.Equals(
				typeof(bool)) || cls.Equals(typeof(byte)) || cls.Equals(typeof(short)) || cls.Equals
				(typeof(int)) || cls.Equals(typeof(long)) || cls.Equals(typeof(float)) || cls.Equals
				(typeof(double)) || cls.Equals(typeof(Type)))
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}
