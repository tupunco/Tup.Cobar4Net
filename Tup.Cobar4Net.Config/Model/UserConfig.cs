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

namespace Tup.Cobar4Net.Config.Model
{
    /// <author>xianmao.hexm 2011-1-11 ÏÂÎç02:26:09</author>
    public class UserConfig
    {
        private string name;
        private string password;
        private ICollection<string> schemas;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        public ICollection<string> Schemas
        {
            get { return schemas; }
            set { schemas = value; }
        }

        public virtual string GetName()
        {
            return name;
        }

        public virtual void SetName(string name)
        {
            this.name = name;
        }

        public virtual string GetPassword()
        {
            return password;
        }

        public virtual void SetPassword(string password)
        {
            this.password = password;
        }

        public virtual ICollection<string> GetSchemas()
        {
            return schemas;
        }

        public virtual void SetSchemas(ICollection<string> schemas)
        {
            this.schemas = schemas;
        }

        public override string ToString()
        {
            return string.Format("[UserConfig Name={0}, Password={1}, Schemas={2}]",
                name, password,
                string.Join(",", schemas ?? new string[0]));
        }
    }
}
