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
using System.Reflection;
using Sharpen;

namespace Tup.Cobar4Net.Config.Util
{
	public class FieldDictionary
	{
		private readonly IDictionary<string, IDictionary<string, FieldInfo>> nameCache = 
			Sharpen.Collections.SynchronizedMap(new Dictionary<string, IDictionary<string, FieldInfo
			>>());

		private readonly IDictionary<string, IDictionary<FieldDictionary.FieldKey, FieldInfo
			>> keyCache = Sharpen.Collections.SynchronizedMap(new Dictionary<string, IDictionary
			<FieldDictionary.FieldKey, FieldInfo>>());

		/// <summary>Returns an iterator for all serializable fields for some class</summary>
		/// <param name="cls">the class you are interested on</param>
		/// <returns>an iterator for its serializable fields</returns>
		public virtual IEnumerator<FieldInfo> SerializableFieldsFor(Type cls)
		{
			return BuildMap(cls, true).Values.GetEnumerator();
		}

		/// <summary>Returns an specific field of some class.</summary>
		/// <remarks>
		/// Returns an specific field of some class. If definedIn is null, it searchs
		/// for the field named 'name' inside the class cls. If definedIn is
		/// different than null, tries to find the specified field name in the
		/// specified class cls which should be defined in class definedIn (either
		/// equals cls or a one of it's superclasses)
		/// </remarks>
		/// <param name="cls">the class where the field is to be searched</param>
		/// <param name="name">the field name</param>
		/// <param name="definedIn">
		/// the superclass (or the class itself) of cls where the
		/// field was defined
		/// </param>
		/// <returns>the field itself</returns>
		public virtual FieldInfo Field(Type cls, string name, Type definedIn)
		{
			IDictionary<object, FieldInfo> fields = BuildMap(cls, definedIn != null);
			FieldInfo field = fields[definedIn != null ? new FieldDictionary.FieldKey(name, definedIn
				, 0) : name];
			if (field == null)
			{
				throw new ObjectAccessException("No such field " + cls.FullName + "." + name);
			}
			else
			{
				return field;
			}
		}

		private IDictionary<object, FieldInfo> BuildMap(Type cls, bool tupleKeyed)
		{
			string clsName = cls.FullName;
			if (!nameCache.Contains(clsName))
			{
				lock (keyCache)
				{
					if (!nameCache.Contains(clsName))
					{
						// double check
						IDictionary<string, FieldInfo> keyedByFieldName = new Dictionary<string, FieldInfo
							>();
						IDictionary<FieldDictionary.FieldKey, FieldInfo> keyedByFieldKey = new OrderRetainingMap
							<FieldDictionary.FieldKey, FieldInfo>();
						while (!typeof(object).Equals(cls))
						{
							FieldInfo[] fields = Sharpen.Runtime.GetDeclaredFields(cls);
							if (JVMInfo.ReverseFieldDefinition())
							{
								for (int i = fields.Length >> 1; i-- > 0; )
								{
									int idx = fields.Length - i - 1;
									FieldInfo field = fields[i];
									fields[i] = fields[idx];
									fields[idx] = field;
								}
							}
							for (int i_1 = 0; i_1 < fields.Length; i_1++)
							{
								FieldInfo field = fields[i_1];
								if (!keyedByFieldName.Contains(field.Name))
								{
									keyedByFieldName[field.Name] = field;
								}
								keyedByFieldKey[new FieldDictionary.FieldKey(field.Name, field.DeclaringType, i_1
									)] = field;
							}
							cls = cls.BaseType;
						}
						nameCache[clsName] = keyedByFieldName;
						keyCache[clsName] = keyedByFieldKey;
					}
				}
			}
			return tupleKeyed ? keyCache[clsName] : nameCache[clsName];
		}

		private class FieldKey
		{
			private string fieldName;

			private Type declaringClass;

			private int depth;

			private int order;

			public FieldKey(string fieldName, Type declaringClass, int order)
			{
				this.fieldName = fieldName;
				this.declaringClass = declaringClass;
				this.order = order;
				Type c = declaringClass;
				int i = 0;
				while (c.BaseType != null)
				{
					i++;
					c = c.BaseType;
				}
				depth = i;
			}

			public override bool Equals(object o)
			{
				if (this == o)
				{
					return true;
				}
				if (!(o is FieldDictionary.FieldKey))
				{
					return false;
				}
				FieldDictionary.FieldKey fieldKey = (FieldDictionary.FieldKey)o;
				if (declaringClass != null ? !declaringClass.Equals(fieldKey.declaringClass) : fieldKey
					.declaringClass != null)
				{
					return false;
				}
				if (fieldName != null ? !fieldName.Equals(fieldKey.fieldName) : fieldKey.fieldName
					 != null)
				{
					return false;
				}
				return true;
			}

			public override int GetHashCode()
			{
				int result;
				result = (fieldName != null ? fieldName.GetHashCode() : 0);
				result = 29 * result + (declaringClass != null ? declaringClass.GetHashCode() : 0
					);
				return result;
			}

			public override string ToString()
			{
				return "FieldKey{" + "order=" + order + ", writer=" + depth + ", declaringClass="
					 + declaringClass + ", fieldName='" + fieldName + "'" + "}";
			}
		}
	}
}
