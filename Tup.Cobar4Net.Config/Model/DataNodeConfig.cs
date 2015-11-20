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
	/// <summary>用于描述一个数据节点的配置</summary>
	/// <author>xianmao.hexm</author>
	public sealed class DataNodeConfig
	{
		private const int DefaultPoolSize = 128;

		private const long DefaultWaitTimeout = 10 * 1000L;

		private const long DefaultIdleTimeout = 10 * 60 * 1000L;

		private const long DefaultHeartbeatTimeout = 30 * 1000L;

		private const int DefaultHeartbeatRetry = 10;

		private string name;

		private string dataSource;

		private int poolSize = DefaultPoolSize;

		private long waitTimeout = DefaultWaitTimeout;

		private long idleTimeout = DefaultIdleTimeout;

		private long heartbeatTimeout = DefaultHeartbeatTimeout;

		private int heartbeatRetry = DefaultHeartbeatRetry;

		private string heartbeatSQL;

		// 保持后端数据通道的默认最大值
		// 取得新连接的等待超时时间
		// 连接池中连接空闲超时时间
		// heartbeat config
		// 心跳超时时间
		// 检查连接发生异常到切换，重试次数
		// 静态心跳语句
		public string GetHeartbeatSQL()
		{
			return heartbeatSQL;
		}

		public void SetHeartbeatSQL(string heartbeatSQL)
		{
			this.heartbeatSQL = heartbeatSQL;
		}

		public string GetName()
		{
			return name;
		}

		public void SetName(string name)
		{
			this.name = name;
		}

		public string GetDataSource()
		{
			return dataSource;
		}

		public void SetDataSource(string dataSource)
		{
			this.dataSource = dataSource;
		}

		public int GetPoolSize()
		{
			return poolSize;
		}

		public void SetPoolSize(int poolSize)
		{
			this.poolSize = poolSize;
		}

		public long GetWaitTimeout()
		{
			return waitTimeout;
		}

		public void SetWaitTimeout(long waitTimeout)
		{
			this.waitTimeout = waitTimeout;
		}

		public long GetIdleTimeout()
		{
			return idleTimeout;
		}

		public void SetIdleTimeout(long idleTimeout)
		{
			this.idleTimeout = idleTimeout;
		}

		public long GetHeartbeatTimeout()
		{
			return heartbeatTimeout;
		}

		public void SetHeartbeatTimeout(long heartbeatTimeout)
		{
			this.heartbeatTimeout = heartbeatTimeout;
		}

		public int GetHeartbeatRetry()
		{
			return heartbeatRetry;
		}

		public void SetHeartbeatRetry(int heartbeatRetry)
		{
			this.heartbeatRetry = heartbeatRetry;
		}

		public bool IsNeedHeartbeat()
		{
			return heartbeatSQL != null;
		}
	}
}
