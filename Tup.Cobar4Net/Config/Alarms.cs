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

namespace Tup.Cobar4Net.Config
{
    /// <summary>Cobar报警关键词定义</summary>
    /// <author>xianmao.hexm 2012-4-19</author>
    public abstract class Alarms
    {
        /// <summary>默认报警关键词</summary>
        public const string Default = "#!Cobar#";

        /// <summary>集群无有效的节点可提供服务</summary>
        public const string ClusterEmpty = "#!CLUSTER_EMPTY#";

        /// <summary>数据节点的数据源发生切换</summary>
        public const string DatanodeSwitch = "#!DN_SWITCH#";

        /// <summary>隔离区非法用户访问</summary>
        public const string QuarantineAttack = "#!QT_ATTACK#";
    }

    public static class AlarmsConstants
    {
    }
}