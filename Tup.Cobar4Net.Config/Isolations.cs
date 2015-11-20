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
    /// <summary>事务隔离级别定义</summary>
    /// <author>xianmao.hexm</author>
    public abstract class Isolations
    {
        public const int ReadUncommitted = 1;

        public const int ReadCommitted = 2;

        public const int RepeatedRead = 3;

        public const int Serializable = 4;
    }

    public static class IsolationsConstants
    {
    }
}