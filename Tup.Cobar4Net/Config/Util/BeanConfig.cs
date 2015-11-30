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
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Tup.Cobar4Net.Config.Util
{
    public class BeanConfig : ICloneable
    {
        private IDictionary<string, object> @params =
            new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        public virtual string Name { get; set; }

        public virtual string ClassName { get; set; }

        public virtual IDictionary<string, object> Params
        {
            get { return @params; }
            set { @params = value; }
        }

        public object Clone()
        {
            BeanConfig bc = null;
            try
            {
                bc = (BeanConfig) Activator.CreateInstance(GetType());
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
            bc.ClassName = ClassName;
            bc.Name = Name;
            var @params = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            @params.AddRange(@params);
            bc.@params = @params;
            return bc;
        }

        /// <exception cref="System.MemberAccessException" />
        /// <exception cref="System.Reflection.TargetInvocationException" />
        public virtual object Create(bool initEarly)
        {
            ThrowHelper.ThrowIfNull(ClassName, "className");

            object obj = null;
            try
            {
                obj = Activator.CreateInstance(ProfileType.GetType(ClassName));
            }
            catch (TypeLoadException e)
            {
                throw new ConfigException(e);
            }
            ParameterMapping.Mapping(obj, Params);
            if (initEarly && obj is IInitializable)
            {((IInitializable) obj).Init();
            }
            return obj;
        }

        public override int GetHashCode()
        {
            var hashcode = 37;
#pragma warning disable RECS0025 // Non-readonly field referenced in 'GetHashCode()'
            hashcode += Name == null ? 0 : Name.GetHashCode();
            hashcode += ClassName == null ? 0 : ClassName.GetHashCode();
            hashcode += Params == null ? 0 : Params.GetHashCode();
#pragma warning restore RECS0025 // Non-readonly field referenced in 'GetHashCode()'
            return hashcode;
        }

        public override bool Equals(object obj)
        {
            if (obj is BeanConfig)
            {
                var entity = (BeanConfig) obj;
                var isEquals = Equals(Name, entity.Name);
                isEquals = isEquals && Equals(ClassName, entity.ClassName);
#pragma warning disable RECS0030 // Suggests using the class declaring a static function when calling it
                isEquals = isEquals && Equals(Params, entity.Params);
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