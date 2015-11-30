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

        private const long DefaultIdleTimeout = 8*3600*1000L;

        private const long DefaultProcessorCheckPeriod = 15*1000L;

        private const long DefaultDatanodeIdleCheckPeriod = 60*1000L;

        private const long DefaultDatanodeHeartbeatPeriod = 10*1000L;

        private const long DefaultClusterHeartbeatPeriod = 5*1000L;

        private const long DefaultClusterHeartbeatTimeout = 10*1000L;

        private const int DefaultClusterHeartbeatRetry = 10;

        private const string DefaultClusterHeartbeatUser = "_HEARTBEAT_USER_";

        private const string DefaultClusterHeartbeatPass = "_HEARTBEAT_PASS_";

        private const int DefaultParserCommentVersion = 50148;

        private const int DefaultSqlRecordCount = 10;

        private static readonly int DefaultProcessors = Environment.ProcessorCount;


        public SystemConfig()
        {
            ServerPort = DefaultPort;
            ManagerPort = DefaultManagerPort;
            Charset = DefaultCharset;

            Processors = DefaultProcessors;
            ProcessorHandler = DefaultProcessors;
            ProcessorExecutor = DefaultProcessors;
            ManagerExecutor = DefaultProcessors;
            TimerExecutor = DefaultProcessors;
            InitExecutor = DefaultProcessors;

            IdleTimeout = DefaultIdleTimeout;
            ProcessorCheckPeriod = DefaultProcessorCheckPeriod;
            DataNodeIdleCheckPeriod = DefaultDatanodeIdleCheckPeriod;
            DataNodeHeartbeatPeriod = DefaultDatanodeHeartbeatPeriod;
            ClusterHeartbeatUser = DefaultClusterHeartbeatUser;
            ClusterHeartbeatPass = DefaultClusterHeartbeatPass;
            ClusterHeartbeatPeriod = DefaultClusterHeartbeatPeriod;
            ClusterHeartbeatTimeout = DefaultClusterHeartbeatTimeout;
            ClusterHeartbeatRetry = DefaultClusterHeartbeatRetry;
            TxIsolation = Isolations.RepeatedRead;
            ParserCommentVersion = DefaultParserCommentVersion;
            SqlRecordCount = DefaultSqlRecordCount;
        }

        public string Charset { get; set; }

        public int ServerPort { get; set; }

        public int ManagerPort { get; set; }

        public int Processors { get; set; }

        public int ProcessorHandler { get; set; }

        public int ProcessorExecutor { get; set; }

        public int ManagerExecutor { get; set; }

        public int TimerExecutor { get; set; }

        public int InitExecutor { get; set; }

        public long IdleTimeout { get; set; }

        public long ProcessorCheckPeriod { get; set; }

        public long DataNodeIdleCheckPeriod { get; set; }

        public long DataNodeHeartbeatPeriod { get; set; }

        public string ClusterHeartbeatUser { get; set; }

        public string ClusterHeartbeatPass { get; set; }

        public long ClusterHeartbeatPeriod { get; set; }

        public long ClusterHeartbeatTimeout { get; set; }

        public int ClusterHeartbeatRetry { get; set; }

        public int TxIsolation { get; set; }

        public int ParserCommentVersion { get; set; }

        public int SqlRecordCount { get; set; }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return
                string.Format(
                    "[System ServerPort={0}, ManagerPort={1}, Charset={2}, Processors={3}, ProcessorHandler={4}, ProcessorExecutor={5}, InitExecutor={6}, TimerExecutor={7}, ManagerExecutor={8}, IdleTimeout={9}, ProcessorCheckPeriod={10}, DataNodeIdleCheckPeriod={11}, DataNodeHeartbeatPeriod={12}, ClusterHeartbeatUser={13}, ClusterHeartbeatPass={14}, ClusterHeartbeatPeriod={15}, ClusterHeartbeatTimeout={16}, ClusterHeartbeatRetry={17}, TxIsolation={18}, ParserCommentVersion={19}, SqlRecordCount={20}]",
                    ServerPort, ManagerPort, Charset,
                    Processors, ProcessorHandler, ProcessorExecutor,
                    InitExecutor, TimerExecutor, ManagerExecutor,
                    IdleTimeout, ProcessorCheckPeriod,
                    DataNodeIdleCheckPeriod, DataNodeHeartbeatPeriod,
                    ClusterHeartbeatUser, ClusterHeartbeatPass,
                    ClusterHeartbeatPeriod, ClusterHeartbeatTimeout,
                    ClusterHeartbeatRetry, TxIsolation,
                    ParserCommentVersion, SqlRecordCount);
        }
    }
}