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

#if CONFG_BEAN
using System.Linq;
using System.ComponentModel;
#endif

namespace Tup.Cobar4Net.Config.Util
{
    /// <summary>
    /// Parameter Mapping
    /// </summary>
    public class ParameterMapping
    {
        private static readonly IDictionary<Type, PropertyInfo[]> descriptors = new Dictionary<Type, PropertyInfo[]>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="@object"></param>
        /// <param name="parameter">ÐèÒª StringComparer.OrdinalIgnoreCase</param>
        /// <exception cref="System.MemberAccessException"/>
        /// <exception cref="System.Reflection.TargetInvocationException"/>
        public static void Mapping(object @object, IDictionary<string, object> parameter)
        {
            ThrowHelper.ThrowIfNull(@object, "object");
            ThrowHelper.ThrowIfNull(parameter, "parameter");

            var type = @object.GetType();
            var pds = GetDescriptors(type);
            PropertyInfo pd = null;
            Type cls = null;
            object obj = null;
            object value = null;
            for (int i = 0; i < pds.Length; i++)
            {
                pd = pds[i];

                obj = parameter.GetValue(pd.Name);
                value = obj;
                cls = pd.PropertyType;
                if (obj is string)
                {
                    string @string = (string)obj;
                    if (!@string.IsEmpty())
                    {
                        @string = ConfigUtil.Filter(@string);
                    }
                    if (IsPrimitiveType(cls))
                    {
                        value = Convert(cls, @string);
                    }
                }
#if CONFG_BEAN
                else if (obj is BeanConfig)
                {
                    value = CreateBean((BeanConfig)obj);
                }
                else if (obj is IList<BeanConfig>)
                {
                    var list = new List<object>();
                    foreach (var beanconfig in (IList<BeanConfig>)obj)
                    {
                        list.Add(CreateBean(beanconfig));
                    }
                    value = list.ToArray();
                }
#endif
                if (cls != null && value != null)
                {
                    pd.SetValue(@object, value, null);
                }
            }
        }

#if CONFG_BEAN
     /// <exception cref="System.MemberAccessException"/>
        /// <exception cref="System.Reflection.TargetInvocationException"/>
        public static object CreateBean(BeanConfig config)
        {
            object bean = config.Create(true);
            if (bean is IDictionary<string, object>)
            {
                var map = (IDictionary<string, object>)bean;
                foreach (var entry in config.GetParams())
                {
                    string key = entry.Key;
                    object value = entry.Value;
                    if (value is BeanConfig)
                    {
                        var mapBeanConfig = (BeanConfig)entry.Value;
                        value = mapBeanConfig.Create(true);
                        Mapping(value, mapBeanConfig.GetParams());
                    }
                    map[key] = value;
                }
            }
            else if (bean is IList<object>)
            {
                //
            }
            else
            {
                Mapping(bean, config.GetParams());
            }
            return bean;
        }
#endif

        private static PropertyInfo[] GetDescriptors(Type clazz)
        {
            var pds2 = descriptors.GetValue(clazz);
            if (null == pds2)
            {

                pds2 = clazz.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty);
                descriptors[clazz] = pds2;
            }

            return pds2;
        }
        private static object Convert(Type cls, string @string)
        {
            // System.Convert.ChangeType(initialValue, targetType, culture);
            object value = null;
            if (cls == typeof(string))
            {
                value = @string;
            }
            else if (cls == typeof(Type))
            {
                value = Type.GetType(@string);
            }
            else
            {
                value = System.Convert.ChangeType(@string, cls);
            }
            return value;
        }
        private static bool IsConvertible(Type t)
        {
#if !PORTABLE
            return typeof(IConvertible).IsAssignableFrom(t);
#else
            return (
                t == typeof(bool) || t == typeof(byte) || t == typeof(char) || t == typeof(DateTime) || t == typeof(decimal) || t == typeof(double) || t == typeof(short) || t == typeof(int) ||
                t == typeof(long) || t == typeof(sbyte) || t == typeof(float) || t == typeof(string) || t == typeof(ushort) || t == typeof(uint) || t == typeof(ulong) || t.IsEnum());
#endif
        }
        //private static ISet<Type> s_PrimitiveType_Set = new HashSet<Type>(new Type[]
        //{
        //    typeof(string), typeof(bool), typeof(byte),
        //    typeof(short), typeof(int), typeof(long),
        //    typeof(double), typeof(float), typeof(bool),
        //    typeof(byte), typeof(short), typeof(int),
        //    typeof(long), typeof(float), typeof(double),
        //    typeof(Type)
        //});
        private static bool IsPrimitiveType(Type cls)
        {
            return IsConvertible(cls) || cls == typeof(Type);
            //return s_PrimitiveType_Set.Contains(cls);
        }
    }
}
