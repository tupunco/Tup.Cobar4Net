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
using Tup.Cobar4Net.Config;

namespace Tup.Cobar4Net.Config.Model
{
    /// <summary>系统基础配置项</summary>
    /// <author>xianmao.hexm 2011-1-11 下午02:14:04</author>
    public sealed class SystemConfig
    {
        private const int DefaultPort = 8066;

        private const int DefaultManagerPort = 9066;

        private const string DefaultCharset = "UTF-8";

        //private static readonly int DefaultProcessors = Runtime.GetRuntime().AvailableProcessors			();

        private const long DefaultIdleTimeout = 8 * 3600 * 1000L;

        private const long DefaultProcessorCheckPeriod = 15 * 1000L;

        private const long DefaultDatanodeIdleCheckPeriod = 60 * 1000L;

        private const long DefaultDatanodeHeartbeatPeriod = 10 * 1000L;

        private const long DefaultClusterHeartbeatPeriod = 5 * 1000L;

        private const long DefaultClusterHeartbeatTimeout = 10 * 1000L;

        private const int DefaultClusterHeartbeatRetry = 10;

        private const string DefaultClusterHeartbeatUser = "_HEARTBEAT_USER_";

        private const string DefaultClusterHeartbeatPass = "_HEARTBEAT_PASS_";

        private const int DefaultParserCommentVersion = 50148;

        private const int DefaultSqlRecordCount = 10;

        private int serverPort;

        private int managerPort;

        private string charset;

        private int processors;

        private int processorHandler;

        private int processorExecutor;

        private int initExecutor;

        private int timerExecutor;

        private int managerExecutor;

        private long idleTimeout;

        private long processorCheckPeriod;

        private long dataNodeIdleCheckPeriod;

        private long dataNodeHeartbeatPeriod;

        private string clusterHeartbeatUser;

        private string clusterHeartbeatPass;

        private long clusterHeartbeatPeriod;

        private long clusterHeartbeatTimeout;

        private int clusterHeartbeatRetry;

        private int txIsolation;

        private int parserCommentVersion;

        private int sqlRecordCount;

        public SystemConfig()
        {
            this.serverPort = DefaultPort;
            this.managerPort = DefaultManagerPort;
            this.charset = DefaultCharset;
            //this.processors = DefaultProcessors;
            //this.processorHandler = DefaultProcessors;
            //this.processorExecutor = DefaultProcessors;
            //this.managerExecutor = DefaultProcessors;
            //this.timerExecutor = DefaultProcessors;
            //this.initExecutor = DefaultProcessors;
            this.idleTimeout = DefaultIdleTimeout;
            this.processorCheckPeriod = DefaultProcessorCheckPeriod;
            this.dataNodeIdleCheckPeriod = DefaultDatanodeIdleCheckPeriod;
            this.dataNodeHeartbeatPeriod = DefaultDatanodeHeartbeatPeriod;
            this.clusterHeartbeatUser = DefaultClusterHeartbeatUser;
            this.clusterHeartbeatPass = DefaultClusterHeartbeatPass;
            this.clusterHeartbeatPeriod = DefaultClusterHeartbeatPeriod;
            this.clusterHeartbeatTimeout = DefaultClusterHeartbeatTimeout;
            this.clusterHeartbeatRetry = DefaultClusterHeartbeatRetry;
            this.txIsolation = Isolations.RepeatedRead;
            this.parserCommentVersion = DefaultParserCommentVersion;
            this.sqlRecordCount = DefaultSqlRecordCount;
        }

        public string GetCharset()
        {
            return charset;
        }

        public void SetCharset(string charset)
        {
            this.charset = charset;
        }

        public int GetServerPort()
        {
            return serverPort;
        }

        public void SetServerPort(int serverPort)
        {
            this.serverPort = serverPort;
        }

        public int GetManagerPort()
        {
            return managerPort;
        }

        public void SetManagerPort(int managerPort)
        {
            this.managerPort = managerPort;
        }

        public int GetProcessors()
        {
            return processors;
        }

        public void SetProcessors(int processors)
        {
            this.processors = processors;
        }

        public int GetProcessorHandler()
        {
            return processorHandler;
        }

        public void SetProcessorHandler(int processorExecutor)
        {
            this.processorHandler = processorExecutor;
        }

        public int GetProcessorExecutor()
        {
            return processorExecutor;
        }

        public void SetProcessorExecutor(int processorExecutor)
        {
            this.processorExecutor = processorExecutor;
        }

        public int GetManagerExecutor()
        {
            return managerExecutor;
        }

        public void SetManagerExecutor(int managerExecutor)
        {
            this.managerExecutor = managerExecutor;
        }

        public int GetTimerExecutor()
        {
            return timerExecutor;
        }

        public void SetTimerExecutor(int timerExecutor)
        {
            this.timerExecutor = timerExecutor;
        }

        public int GetInitExecutor()
        {
            return initExecutor;
        }

        public void SetInitExecutor(int initExecutor)
        {
            this.initExecutor = initExecutor;
        }

        public long GetIdleTimeout()
        {
            return idleTimeout;
        }

        public void SetIdleTimeout(long idleTimeout)
        {
            this.idleTimeout = idleTimeout;
        }

        public long GetProcessorCheckPeriod()
        {
            return processorCheckPeriod;
        }

        public void SetProcessorCheckPeriod(long processorCheckPeriod)
        {
            this.processorCheckPeriod = processorCheckPeriod;
        }

        public long GetDataNodeIdleCheckPeriod()
        {
            return dataNodeIdleCheckPeriod;
        }

        public void SetDataNodeIdleCheckPeriod(long dataNodeIdleCheckPeriod)
        {
            this.dataNodeIdleCheckPeriod = dataNodeIdleCheckPeriod;
        }

        public long GetDataNodeHeartbeatPeriod()
        {
            return dataNodeHeartbeatPeriod;
        }

        public void SetDataNodeHeartbeatPeriod(long dataNodeHeartbeatPeriod)
        {
            this.dataNodeHeartbeatPeriod = dataNodeHeartbeatPeriod;
        }

        public string GetClusterHeartbeatUser()
        {
            return clusterHeartbeatUser;
        }

        public void SetClusterHeartbeatUser(string clusterHeartbeatUser)
        {
            this.clusterHeartbeatUser = clusterHeartbeatUser;
        }

        public string GetClusterHeartbeatPass()
        {
            return clusterHeartbeatPass;
        }

        public void SetClusterHeartbeatPass(string clusterHeartbeatPass)
        {
            this.clusterHeartbeatPass = clusterHeartbeatPass;
        }

        public long GetClusterHeartbeatPeriod()
        {
            return clusterHeartbeatPeriod;
        }

        public void SetClusterHeartbeatPeriod(long clusterHeartbeatPeriod)
        {
            this.clusterHeartbeatPeriod = clusterHeartbeatPeriod;
        }

        public long GetClusterHeartbeatTimeout()
        {
            return clusterHeartbeatTimeout;
        }

        public void SetClusterHeartbeatTimeout(long clusterHeartbeatTimeout)
        {
            this.clusterHeartbeatTimeout = clusterHeartbeatTimeout;
        }

        public int GetClusterHeartbeatRetry()
        {
            return clusterHeartbeatRetry;
        }

        public void SetClusterHeartbeatRetry(int clusterHeartbeatRetry)
        {
            this.clusterHeartbeatRetry = clusterHeartbeatRetry;
        }

        public int GetTxIsolation()
        {
            return txIsolation;
        }

        public void SetTxIsolation(int txIsolation)
        {
            this.txIsolation = txIsolation;
        }

        public int GetParserCommentVersion()
        {
            return parserCommentVersion;
        }

        public void SetParserCommentVersion(int parserCommentVersion)
        {
            this.parserCommentVersion = parserCommentVersion;
        }

        public int GetSqlRecordCount()
        {
            return sqlRecordCount;
        }

        public void SetSqlRecordCount(int sqlRecordCount)
        {
            this.sqlRecordCount = sqlRecordCount;
        }
    }
}
