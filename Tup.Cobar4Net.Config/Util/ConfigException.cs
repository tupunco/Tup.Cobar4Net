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

namespace Tup.Cobar4Net.Config.Util
{
    /// <author>xianmao.hexm 2011-1-10 下午07:07:46</author>
    [System.Serializable]
    public class ConfigException : Exception
    {
        private const long serialVersionUID = -180146385688342818L;

        public ConfigException()
        {
        }

        public ConfigException(string message, Exception cause)
            : base(message, cause)
        {
        }

        public ConfigException(string message)
            : base(message)
        {
        }

        public ConfigException(Exception cause)
            : base(string.Empty, cause)
        {
        }
    }
}