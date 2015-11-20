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
	/// <author>haiqing.zhuhq 2012-3-21</author>
	/// <author>xianmao.hexm</author>
	public sealed class CobarNodeConfig
	{
		private string name;

		private string host;

		private int port;

		private int weight;

		public CobarNodeConfig(string name, string host, int port, int weight)
		{
			this.name = name;
			this.host = host;
			this.port = port;
			this.weight = weight;
		}

		public string GetName()
		{
			return name;
		}

		public void SetName(string name)
		{
			this.name = name;
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

		public int GetWeight()
		{
			return weight;
		}

		public void SetWeight(int weight)
		{
			this.weight = weight;
		}

		public override string ToString()
		{
			return new StringBuilder().Append("[name=").Append(name).Append(",host=").Append(
				host).Append(",port=").Append(port).Append(",weight=").Append(weight).Append(']'
				).ToString();
		}
	}
}
