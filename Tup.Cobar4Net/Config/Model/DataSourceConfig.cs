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

namespace Tup.Cobar4Net.Config.Model
{
    /// <summary>描述一个数据源的配置</summary>
    /// <author>xianmao.hexm 2011-1-11 下午02:14:38</author>
    public sealed class DataSourceConfig
    {
        private const int DefaultSqlRecordCount = 10;

        public string Name { get; set; }

        public string SourceType { get; private set; }

        public string Type
        {
            set { SourceType = value; }
        }

        public string Host { get; set; }

        public int Port { get; set; }

        public string User { get; set; }

        public string Password { set; get; }

        public string Database { get; set; }

        public string SqlMode { get; set; }

        public int SqlRecordCount { get; set; } = DefaultSqlRecordCount;

        public override string ToString()
        {
            return
                string.Format(
                    "[DataSourceConfig Name={0}, ProfileType={1}, Host={2}, Port={3}, User={4}, Password={5}, Database={6}, SqlMode={7}, SqlRecordCount={8}]",
                    Name, SourceType, Host, Port, User, Password, Database, SqlMode, SqlRecordCount);
        }

        //public override string ToString()
        //{
        //    return new StringBuilder().Append("[name=").Append(Name).Append(",host=").Append(
        //        Host).Append(",port=").Append(Port).Append(",database=").Append(Database).Append
        //        (']').ToString();

        //}
    }
}