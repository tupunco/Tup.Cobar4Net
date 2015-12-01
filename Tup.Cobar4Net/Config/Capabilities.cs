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
    /// <summary>处理能力标识定义</summary>
    /// <author>xianmao.hexm</author>
    public abstract class Capabilities
    {
        /// <summary>
        ///     server capabilities
        ///     <pre>
        ///         server:        11110111 11111111
        ///         client_cmd: 11 10100110 10000101
        ///         client_jdbc:10 10100010 10001111
        /// </summary>
        /// <seealso>
        ///     http://dev.mysql.com/doc/refman/5.1/en/mysql-real-connect.html
        ///     </pre>
        /// </seealso>
        public const int ClientLongPassword = 1;

        public const int ClientFoundRows = 2;

        public const int ClientLongFlag = 4;

        public const int ClientConnectWithDb = 8;

        public const int ClientNoSchema = 16;

        public const int ClientCompress = 32;

        public const int ClientOdbc = 64;

        public const int ClientLocalFiles = 128;

        public const int ClientIgnoreSpace = 256;

        public const int ClientProtocol41 = 512;

        public const int ClientInteractive = 1024;

        public const int ClientSsl = 2048;

        public const int ClientIgnoreSigpipe = 4096;

        public const int ClientTransactions = 8192;

        public const int ClientReserved = 16384;

        public const int ClientSecureConnection = 32768;

        public const int ClientMultiStatements = 65536;

        public const int ClientMultiResults = 131072;
        // new more secure passwords
        // Found instead of affected rows
        // 返回找到（匹配）的行数，而不是改变了的行数。
        // Get all column flags
        // One can specify db on connect
        // Don't allow database.table.column
        // 不允许“数据库名.表名.列名”这样的语法。这是对于ODBC的设置。
        // 当使用这样的语法时解析器会产生一个错误，这对于一些ODBC的程序限制bug来说是有用的。
        // Can use compression protocol
        // 使用压缩协议
        // Odbc client
        // Can use LOAD DATA LOCAL
        // Ignore spaces before '('
        // 允许在函数名后使用空格。所有函数名可以预留字。
        // New 4.1 protocol This is an interactive client
        // This is an interactive client
        // 允许使用关闭连接之前的不活动交互超时的描述，而不是等待超时秒数。
        // 客户端的会话等待超时变量变为交互超时变量。
        // Switch to SSL after handshake
        // 使用SSL。这个设置不应该被应用程序设置，他应该是在客户端库内部是设置的。
        // 可以在调用mysql_real_connect()之前调用mysql_ssl_set()来代替设置。
        // IGNORE sigpipes
        // 阻止客户端库安装一个SIGPIPE信号处理器。
        // 这个可以用于当应用程序已经安装该处理器的时候避免与其发生冲突。
        // Client knows about transactions
        // Old flag for 4.1 protocol
        // New 4.1 authentication
        // Enable/disable multi-stmt support
        // 通知服务器客户端可以发送多条语句（由分号分隔）。如果该标志为没有被设置，多条语句执行。
        // Enable/disable multi-results
        // 通知服务器客户端可以处理由多语句或者存储过程执行生成的多结果集。
        // 当打开CLIENT_MULTI_STATEMENTS时，这个标志自动的被打开。
    }

    public static class CapabilitiesConstants
    {
    }
}