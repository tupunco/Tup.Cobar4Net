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
using System.Reflection;
using Sharpen;
using Sharpen.Reflect;

namespace Tup.Cobar4Net.Config.Util
{
	public class ReflectionProvider
	{
		[System.NonSerialized]
		private IDictionary<Type, byte[]> serializedDataCache = Sharpen.Collections.SynchronizedMap
			(new Dictionary<Type, byte[]>());

		[System.NonSerialized]
		private FieldDictionary fieldDictionary = new FieldDictionary();

		public virtual object NewInstance(Type type)
		{
			try
			{
				Constructor<object>[] c = type.GetDeclaredConstructors();
				for (int i = 0; i < c.Length; i++)
				{
					if (c[i].GetParameterTypes().Length == 0)
					{
						if (!Modifier.IsPublic(c[i].GetModifiers()))
						{
						}
						return c[i].NewInstance(new object[0]);
					}
				}
				if (typeof(Serializable).IsAssignableFrom(type))
				{
					return InstantiateUsingSerialization(type);
				}
				else
				{
					throw new ObjectAccessException("Cannot construct " + type.FullName + " as it does not have a no-args constructor"
						);
				}
			}
			catch (InstantiationException e)
			{
				throw new ObjectAccessException("Cannot construct " + type.FullName, e);
			}
			catch (MemberAccessException e)
			{
				throw new ObjectAccessException("Cannot construct " + type.FullName, e);
			}
			catch (TargetInvocationException e)
			{
				if (e.InnerException is RuntimeException)
				{
					throw (RuntimeException)e.InnerException;
				}
				else
				{
					if (e.InnerException is Error)
					{
						throw (Error)e.InnerException;
					}
					else
					{
						throw new ObjectAccessException("Constructor for " + type.FullName + " threw an exception"
							, e.InnerException);
					}
				}
			}
		}

		public virtual void VisitSerializableFields(object @object, Visitor visitor)
		{
			for (IEnumerator<FieldInfo> iterator = fieldDictionary.SerializableFieldsFor(@object
				.GetType()); iterator.MoveNext(); )
			{
				FieldInfo field = iterator.Current;
				if (!FieldModifiersSupported(field))
				{
					continue;
				}
				ValidateFieldAccess(field);
				try
				{
					object value = field.GetValue(@object);
					visitor.Visit(field.Name, field.FieldType, field.DeclaringType, value);
				}
				catch (ArgumentException e)
				{
					throw new ObjectAccessException("Could not get field " + field.GetType() + "." + 
						field.Name, e);
				}
				catch (MemberAccessException e)
				{
					throw new ObjectAccessException("Could not get field " + field.GetType() + "." + 
						field.Name, e);
				}
			}
		}

		public virtual void WriteField(object @object, string fieldName, object value, Type
			 definedIn)
		{
			FieldInfo field = fieldDictionary.Field(@object.GetType(), fieldName, definedIn);
			ValidateFieldAccess(field);
			try
			{
				field.SetValue(@object, value);
			}
			catch (ArgumentException e)
			{
				throw new ObjectAccessException("Could not set field " + field.Name + "@" + @object
					.GetType(), e);
			}
			catch (MemberAccessException e)
			{
				throw new ObjectAccessException("Could not set field " + field.Name + "@" + @object
					.GetType(), e);
			}
		}

		public virtual void InvokeMethod(object @object, string methodName, object value, 
			Type definedIn)
		{
			try
			{
				MethodInfo method = @object.GetType().GetMethod(methodName, new Type[] { value.GetType
					() });
				method.Invoke(@object, new object[] { value });
			}
			catch (Exception e)
			{
				throw new ObjectAccessException("Could not invoke " + @object.GetType() + "." + methodName
					, e);
			}
		}

		public virtual Type GetFieldType(object @object, string fieldName, Type definedIn
			)
		{
			return fieldDictionary.Field(@object.GetType(), fieldName, definedIn).FieldType;
		}

		public virtual bool FieldDefinedInClass(string fieldName, Type type)
		{
			try
			{
				FieldInfo field = fieldDictionary.Field(type, fieldName, null);
				return FieldModifiersSupported(field);
			}
			catch (ObjectAccessException)
			{
				return false;
			}
		}

		public virtual FieldInfo GetField(Type definedIn, string fieldName)
		{
			return fieldDictionary.Field(definedIn, fieldName, null);
		}

		private object InstantiateUsingSerialization(Type type)
		{
			try
			{
				byte[] data;
				if (serializedDataCache.Contains(type))
				{
					data = serializedDataCache[type];
				}
				else
				{
					ByteArrayOutputStream bytes = new ByteArrayOutputStream();
					DataOutputStream stream = new DataOutputStream(bytes);
					stream.WriteShort(ObjectStreamConstants.StreamMagic);
					stream.WriteShort(ObjectStreamConstants.StreamVersion);
					stream.WriteByte(ObjectStreamConstants.TcObject);
					stream.WriteByte(ObjectStreamConstants.TcClassdesc);
					stream.WriteUTF(type.FullName);
					stream.WriteLong(ObjectStreamClass.Lookup(type).GetSerialVersionUID());
					stream.WriteByte(2);
					// classDescFlags (2 = Serializable)
					stream.WriteShort(0);
					// field count
					stream.WriteByte(ObjectStreamConstants.TcEndblockdata);
					stream.WriteByte(ObjectStreamConstants.TcNull);
					data = bytes.ToByteArray();
					serializedDataCache[type] = data;
				}
				ObjectInputStream @in = new ObjectInputStream(new ByteArrayInputStream(data));
				return @in.ReadObject();
			}
			catch (IOException e)
			{
				throw new ObjectAccessException("Cannot create " + type.FullName + " by JDK serialization"
					, e);
			}
			catch (TypeLoadException e)
			{
				throw new ObjectAccessException("Cannot find class " + e.Message);
			}
		}

		private bool FieldModifiersSupported(FieldInfo field)
		{
			return !(Modifier.IsStatic(field.GetModifiers()) || Modifier.IsTransient(field.GetModifiers
				()));
		}

		private void ValidateFieldAccess(FieldInfo field)
		{
			if (Modifier.IsFinal(field.GetModifiers()))
			{
				if (JVMInfo.Is15())
				{
				}
				else
				{
					throw new ObjectAccessException("Invalid final field " + field.DeclaringType.FullName
						 + "." + field.Name);
				}
			}
		}

		private object ReadResolve()
		{
			serializedDataCache = Sharpen.Collections.SynchronizedMap(new Dictionary<Type, byte
				[]>());
			fieldDictionary = new FieldDictionary();
			return this;
		}
	}
}
