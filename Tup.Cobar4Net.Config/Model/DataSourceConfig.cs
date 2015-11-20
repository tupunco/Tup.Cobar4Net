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
using System.Text;

namespace Tup.Cobar4Net.Config.Model
{
	/// <summary>描述一个数据源的配置</summary>
	/// <author>xianmao.hexm 2011-1-11 下午02:14:38</author>
	public sealed class DataSourceConfig
	{
		private const int DefaultSqlRecordCount = 10;

		private string name;

		private string type;

		private string host;

		private int port;

		private string user;

		private string password;

		private string database;

		private string sqlMode;

		private int sqlRecordCount = DefaultSqlRecordCount;

		public string GetName()
		{
			return name;
		}

		public void SetName(string name)
		{
			this.name = name;
		}

		public string GetSourceType()
		{
			return type;
		}

		public void SetType(string type)
		{
			this.type = type;
		}

		public string GetHost()
		{
			return host;
		}

		public void SetHost(string host)
		{
			this.host = host;
		}

		public int GetPort()
		{
			return port;
		}

		public void SetPort(int port)
		{
			this.port = port;
		}

		public string GetUser()
		{
			return user;
		}

		public void SetUser(string user)
		{
			this.user = user;
		}

		public string GetPassword()
		{
			return password;
		}

		public void SetPassword(string password)
		{
			this.password = password;
		}

		public string GetDatabase()
		{
			return database;
		}

		public void SetDatabase(string database)
		{
			this.database = database;
		}

		public string GetSqlMode()
		{
			return sqlMode;
		}

		public void SetSqlMode(string sqlMode)
		{
			this.sqlMode = sqlMode;
		}

		public int GetSqlRecordCount()
		{
			return sqlRecordCount;
		}

		public void SetSqlRecordCount(int sqlRecordCount)
		{
			this.sqlRecordCount = sqlRecordCount;
		}

		public override string ToString()
		{
			return new StringBuilder().Append("[name=").Append(name).Append(",host=").Append(
				host).Append(",port=").Append(port).Append(",database=").Append(database).Append
				(']').ToString();
		}
	}
}
