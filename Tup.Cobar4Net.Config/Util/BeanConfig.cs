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

#if CONFG_BEAN
System;
using System.Collections.Generic;
using Sharpen;
using Tup.Cobar4Net.Util;
using System.Reflection;

namespace Tup.Cobar4Net.Config.Util
{
    public class BeanConfig : ICloneable
    {
        //private static readonly ReflectionProvider refProvider = new ReflectionProvider();

        private string name;

        private string className;

        private IDictionary<string, object> @params = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string ClassName
        {
            get { return className; }
            set { className = value; }
        }

        public IDictionary<string, object> Params
        {
            get { return @params; }
            set { @params = value; }
        }

        public virtual string GetName()
        {
            return name;
        }

        public virtual void SetName(string name)
        {
            this.name = name;
        }

        public virtual string GetClassName()
        {
            return className;
        }

        public virtual void SetClassName(string beanObject)
        {
            this.className = beanObject;
        }

        public virtual IDictionary<string, object> GetParams()
        {
            return @params;
        }

        public virtual void SetParams(IDictionary<string, object> @params)
        {
            this.@params = @params;
        }

        /// <exception cref="System.MemberAccessException"/>
        /// <exception cref="System.Reflection.TargetInvocationException"/>
        public virtual object Create(bool initEarly)
        {
            object obj = null;
            try
            {
                obj = Activator.CreateInstance(Type.GetType(className));
            }
            catch (TypeLoadException e)
            {
                throw new ConfigException(e);
            }
            ParameterMapping.Mapping(obj, @params);
            if (initEarly && (obj is Initializable))
            {
                ((Initializable)obj).Init();
            }
            return obj;
        }

        public object Clone()
        {
            BeanConfig bc = null;
            try
            {
                bc = (BeanConfig)Activator.CreateInstance(GetType());
            }
            catch (TargetInvocationException e)
            {
                throw new ConfigException(e);
            }
            catch (MemberAccessException e)
            {
                throw new ConfigException(e);
            }
            if (bc == null)
            {
                return null;
            }
            bc.className = className;
            bc.name = name;
            var @params = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            @params.AddRange(this.@params);
            bc.@params = @params;
            return bc;
        }

        public override int GetHashCode()
        {
            int hashcode = 37;
#pragma warning disable RECS0025 // Non-readonly field referenced in 'GetHashCode()'
            hashcode += (name == null ? 0 : name.GetHashCode());
            hashcode += (className == null ? 0 : className.GetHashCode());
            hashcode += (@params == null ? 0 : @params.GetHashCode());
#pragma warning restore RECS0025 // Non-readonly field referenced in 'GetHashCode()'
            return hashcode;
        }

        public override bool Equals(object obj)
        {
            if (obj is BeanConfig)
            {
                var entity = (BeanConfig)obj;
                bool isEquals = Equals(name, entity.name);
                isEquals = isEquals && Equals(className, entity.GetClassName());
#pragma warning disable RECS0030 // Suggests using the class declaring a static function when calling it
                isEquals = isEquals && (ObjectUtil.Equals(@params, entity.@params));
#pragma warning restore RECS0030 // Suggests using the class declaring a static function when calling it
                return isEquals;
            }
            return false;
        }

        private static bool Equals(string str1, string str2)
        {
            if (str1 == null)
                return str2 == null;

            return str1.Equals(str2);
        }
    }
}
#endif