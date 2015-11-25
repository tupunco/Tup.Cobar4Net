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

namespace Tup.Cobar4Net.Config.Model
{
    /// <summary>系统基础配置项</summary>
    /// <author>xianmao.hexm 2011-1-11 下午02:14:04</author>
    public sealed class SystemConfig
    {
        private const int DefaultPort = 8066;

        private const int DefaultManagerPort = 9066;

        private const string DefaultCharset = "UTF-8";

        private static readonly int DefaultProcessors = Environment.ProcessorCount;

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

        public int ServerPort
        {
            get { return serverPort; }
            set { serverPort = value; }
        }

        private int managerPort;

        public int ManagerPort
        {
            get { return managerPort; }
            set { managerPort = value; }
        }

        private string charset;

        public string Charset
        {
            get { return charset; }
            set { charset = value; }
        }

        private int processors;

        public int Processors
        {
            get { return processors; }
            set { processors = value; }
        }

        private int processorHandler;

        public int ProcessorHandler
        {
            get { return processorHandler; }
            set { processorHandler = value; }
        }

        private int processorExecutor;

        public int ProcessorExecutor
        {
            get { return processorExecutor; }
            set { processorExecutor = value; }
        }

        private int initExecutor;

        public int InitExecutor
        {
            get { return initExecutor; }
            set { initExecutor = value; }
        }

        private int timerExecutor;

        public int TimerExecutor
        {
            get { return timerExecutor; }
            set { timerExecutor = value; }
        }

        private int managerExecutor;

        public int ManagerExecutor
        {
            get { return managerExecutor; }
            set { managerExecutor = value; }
        }

        private long idleTimeout;

        public long IdleTimeout
        {
            get { return idleTimeout; }
            set { idleTimeout = value; }
        }

        private long processorCheckPeriod;

        public long ProcessorCheckPeriod
        {
            get { return processorCheckPeriod; }
            set { processorCheckPeriod = value; }
        }

        private long dataNodeIdleCheckPeriod;

        public long DataNodeIdleCheckPeriod
        {
            get { return dataNodeIdleCheckPeriod; }
            set { dataNodeIdleCheckPeriod = value; }
        }

        private long dataNodeHeartbeatPeriod;

        public long DataNodeHeartbeatPeriod
        {
            get { return dataNodeHeartbeatPeriod; }
            set { dataNodeHeartbeatPeriod = value; }
        }

        private string clusterHeartbeatUser;

        public string ClusterHeartbeatUser
        {
            get { return clusterHeartbeatUser; }
            set { clusterHeartbeatUser = value; }
        }

        private string clusterHeartbeatPass;

        public string ClusterHeartbeatPass
        {
            get { return clusterHeartbeatPass; }
            set { clusterHeartbeatPass = value; }
        }

        private long clusterHeartbeatPeriod;

        public long ClusterHeartbeatPeriod
        {
            get { return clusterHeartbeatPeriod; }
            set { clusterHeartbeatPeriod = value; }
        }

        private long clusterHeartbeatTimeout;

        public long ClusterHeartbeatTimeout
        {
            get { return clusterHeartbeatTimeout; }
            set { clusterHeartbeatTimeout = value; }
        }

        private int clusterHeartbeatRetry;

        public int ClusterHeartbeatRetry
        {
            get { return clusterHeartbeatRetry; }
            set { clusterHeartbeatRetry = value; }
        }

        private int txIsolation;

        public int TxIsolation
        {
            get { return txIsolation; }
            set { txIsolation = value; }
        }

        private int parserCommentVersion;

        public int ParserCommentVersion
        {
            get { return parserCommentVersion; }
            set { parserCommentVersion = value; }
        }

        private int sqlRecordCount;

        public int SqlRecordCount
        {
            get { return sqlRecordCount; }
            set { sqlRecordCount = value; }
        }

        public SystemConfig()
        {
            this.serverPort = DefaultPort;
            this.managerPort = DefaultManagerPort;
            this.charset = DefaultCharset;

            this.processors = DefaultProcessors;
            this.processorHandler = DefaultProcessors;
            this.processorExecutor = DefaultProcessors;
            this.managerExecutor = DefaultProcessors;
            this.timerExecutor = DefaultProcessors;
            this.initExecutor = DefaultProcessors;

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

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[SystemConfig ServerPort={0}, ManagerPort={1}, Charset={2}, Processors={3}, ProcessorHandler={4}, ProcessorExecutor={5}, InitExecutor={6}, TimerExecutor={7}, ManagerExecutor={8}, IdleTimeout={9}, ProcessorCheckPeriod={10}, DataNodeIdleCheckPeriod={11}, DataNodeHeartbeatPeriod={12}, ClusterHeartbeatUser={13}, ClusterHeartbeatPass={14}, ClusterHeartbeatPeriod={15}, ClusterHeartbeatTimeout={16}, ClusterHeartbeatRetry={17}, TxIsolation={18}, ParserCommentVersion={19}, SqlRecordCount={20}]",
                                        serverPort, managerPort, charset,
                                        processors, processorHandler, processorExecutor,
                                        initExecutor, timerExecutor, managerExecutor,
                                        idleTimeout, processorCheckPeriod,
                                        dataNodeIdleCheckPeriod, dataNodeHeartbeatPeriod,
                                        clusterHeartbeatUser, clusterHeartbeatPass,
                                        clusterHeartbeatPeriod, clusterHeartbeatTimeout,
                                        clusterHeartbeatRetry, txIsolation,
                                        parserCommentVersion, sqlRecordCount);
        }
    }
}
