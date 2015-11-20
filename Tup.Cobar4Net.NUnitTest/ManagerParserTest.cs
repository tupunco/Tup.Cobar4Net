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
using NUnit.Framework;
using Sharpen;
using Tup.Cobar.Manager.Parser;

namespace Tup.Cobar4Net.Parser
{
	/// <author>xianmao.hexm</author>
	[NUnit.Framework.TestFixture]
	public class ManagerParserTest
	{
		[NUnit.Framework.Test]
		public virtual void TestIsSelect()
		{
			NUnit.Framework.Assert.AreEqual(ManagerParse.Select, unchecked((int)(0xff)) & ManagerParse
				.Parse("select * from offer limit 1"));
			NUnit.Framework.Assert.AreEqual(ManagerParse.Select, unchecked((int)(0xff)) & ManagerParse
				.Parse("SELECT * FROM OFFER LIMIT 1"));
			NUnit.Framework.Assert.AreEqual(ManagerParse.Select, unchecked((int)(0xff)) & ManagerParse
				.Parse("SELECT * FROM OFFER limit 1"));
		}

		[NUnit.Framework.Test]
		public virtual void TestIsSet()
		{
			NUnit.Framework.Assert.AreEqual(ManagerParse.Set, ManagerParse.Parse("set names utf8"
				));
			NUnit.Framework.Assert.AreEqual(ManagerParse.Set, ManagerParse.Parse("SET NAMES UTF8"
				));
			NUnit.Framework.Assert.AreEqual(ManagerParse.Set, ManagerParse.Parse("set NAMES utf8"
				));
		}

		[NUnit.Framework.Test]
		public virtual void TestIsShow()
		{
			NUnit.Framework.Assert.AreEqual(ManagerParse.Show, unchecked((int)(0xff)) & ManagerParse
				.Parse("show databases"));
			NUnit.Framework.Assert.AreEqual(ManagerParse.Show, unchecked((int)(0xff)) & ManagerParse
				.Parse("SHOW DATABASES"));
			NUnit.Framework.Assert.AreEqual(ManagerParse.Show, unchecked((int)(0xff)) & ManagerParse
				.Parse("SHOW databases"));
		}

		[NUnit.Framework.Test]
		public virtual void TestShowCommand()
		{
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.Command, ManagerParseShow.Parse(
				"show @@command", 5));
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.Command, ManagerParseShow.Parse(
				"SHOW @@COMMAND", 5));
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.Command, ManagerParseShow.Parse(
				"show @@COMMAND", 5));
		}

		[NUnit.Framework.Test]
		public virtual void TestShowConnection()
		{
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.Connection, ManagerParseShow.Parse
				("show @@connection", 5));
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.Connection, ManagerParseShow.Parse
				("SHOW @@CONNECTION", 5));
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.Connection, ManagerParseShow.Parse
				("show @@CONNECTION", 5));
		}

		[NUnit.Framework.Test]
		public virtual void TestShowConnectionSQL()
		{
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.ConnectionSql, ManagerParseShow.
				Parse("show @@connection.sql", 5));
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.ConnectionSql, ManagerParseShow.
				Parse("SHOW @@CONNECTION.SQL", 5));
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.ConnectionSql, ManagerParseShow.
				Parse("show @@CONNECTION.Sql", 5));
		}

		[NUnit.Framework.Test]
		public virtual void TestShowDatabase()
		{
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.Database, ManagerParseShow.Parse
				("show @@database", 5));
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.Database, ManagerParseShow.Parse
				("SHOW @@DATABASE", 5));
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.Database, ManagerParseShow.Parse
				("show @@DATABASE", 5));
		}

		[NUnit.Framework.Test]
		public virtual void TestShowDataNode()
		{
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.Datanode, ManagerParseShow.Parse
				("show @@datanode", 5));
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.Datanode, ManagerParseShow.Parse
				("SHOW @@DATANODE", 5));
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.Datanode, ManagerParseShow.Parse
				("show @@DATANODE", 5));
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.Datanode, ManagerParseShow.Parse
				("show @@DATANODE   ", 5));
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.DatanodeWhere, unchecked((int)(0xff
				)) & ManagerParseShow.Parse("show @@DATANODE WHERE SCHEMA=1", 5));
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.DatanodeWhere, unchecked((int)(0xff
				)) & ManagerParseShow.Parse("show @@DATANODE WHERE schema =1", 5));
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.DatanodeWhere, unchecked((int)(0xff
				)) & ManagerParseShow.Parse("show @@DATANODE WHERE SCHEMA= 1", 5));
		}

		[NUnit.Framework.Test]
		public virtual void TestShowDataSource()
		{
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.Datasource, ManagerParseShow.Parse
				("show @@datasource", 5));
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.Datasource, ManagerParseShow.Parse
				("SHOW @@DATASOURCE", 5));
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.Datasource, ManagerParseShow.Parse
				(" show  @@DATASOURCE ", 5));
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.Datasource, ManagerParseShow.Parse
				(" show  @@DATASOURCE   ", 5));
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.DatasourceWhere, unchecked((int)
				(0xff)) & ManagerParseShow.Parse(" show  @@DATASOURCE where datanode = 1", 5));
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.DatasourceWhere, unchecked((int)
				(0xff)) & ManagerParseShow.Parse(" show  @@DATASOURCE where datanode=1", 5));
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.DatasourceWhere, unchecked((int)
				(0xff)) & ManagerParseShow.Parse(" show  @@DATASOURCE WHERE datanode = 1", 5));
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.DatasourceWhere, unchecked((int)
				(0xff)) & ManagerParseShow.Parse(" show  @@DATASOURCE where DATAnode= 1 ", 5));
		}

		[NUnit.Framework.Test]
		public virtual void TestShowHelp()
		{
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.Help, ManagerParseShow.Parse("show @@help"
				, 5));
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.Help, ManagerParseShow.Parse("SHOW @@HELP"
				, 5));
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.Help, ManagerParseShow.Parse("show @@HELP"
				, 5));
		}

		[NUnit.Framework.Test]
		public virtual void TestShowHeartbeat()
		{
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.Heartbeat, ManagerParseShow.Parse
				("show @@heartbeat", 5));
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.Heartbeat, ManagerParseShow.Parse
				("SHOW @@hearTBeat ", 5));
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.Heartbeat, ManagerParseShow.Parse
				("  show   @@HEARTBEAT  ", 6));
		}

		[NUnit.Framework.Test]
		public virtual void TestShowParser()
		{
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.Parser, ManagerParseShow.Parse("show @@parser"
				, 5));
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.Parser, ManagerParseShow.Parse("SHOW @@PARSER"
				, 5));
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.Parser, ManagerParseShow.Parse("show @@PARSER"
				, 5));
		}

		[NUnit.Framework.Test]
		public virtual void TestShowProcessor()
		{
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.Processor, ManagerParseShow.Parse
				("show @@processor", 5));
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.Processor, ManagerParseShow.Parse
				("SHOW @@PROCESSOR", 5));
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.Processor, ManagerParseShow.Parse
				("show @@PROCESSOR", 5));
		}

		[NUnit.Framework.Test]
		public virtual void TestShowRouter()
		{
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.Router, ManagerParseShow.Parse("show @@router"
				, 5));
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.Router, ManagerParseShow.Parse("SHOW @@ROUTER"
				, 5));
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.Router, ManagerParseShow.Parse("show @@ROUTER"
				, 5));
		}

		[NUnit.Framework.Test]
		public virtual void TestShowServer()
		{
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.Server, ManagerParseShow.Parse("show @@server"
				, 5));
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.Server, ManagerParseShow.Parse("SHOW @@SERVER"
				, 5));
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.Server, ManagerParseShow.Parse("show @@SERVER"
				, 5));
		}

		[NUnit.Framework.Test]
		public virtual void TestShowThreadPool()
		{
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.Threadpool, ManagerParseShow.Parse
				("show @@threadPool", 5));
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.Threadpool, ManagerParseShow.Parse
				("SHOW @@THREADPOOL", 5));
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.Threadpool, ManagerParseShow.Parse
				("show @@THREADPOOL", 5));
		}

		[NUnit.Framework.Test]
		public virtual void TestShowBackend()
		{
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.Backend, ManagerParseShow.Parse(
				"show @@backend", 5));
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.Backend, ManagerParseShow.Parse(
				"SHOW @@BACkend;", 5));
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.Backend, ManagerParseShow.Parse(
				"show @@BACKEND ", 5));
		}

		[NUnit.Framework.Test]
		public virtual void TestShowTimeCurrent()
		{
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.TimeCurrent, ManagerParseShow.Parse
				("show @@time.current", 5));
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.TimeCurrent, ManagerParseShow.Parse
				("SHOW @@TIME.CURRENT", 5));
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.TimeCurrent, ManagerParseShow.Parse
				("show @@TIME.current", 5));
		}

		[NUnit.Framework.Test]
		public virtual void TestShowTimeStartUp()
		{
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.TimeStartup, ManagerParseShow.Parse
				("show @@time.startup", 5));
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.TimeStartup, ManagerParseShow.Parse
				("SHOW @@TIME.STARTUP", 5));
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.TimeStartup, ManagerParseShow.Parse
				("show @@TIME.startup", 5));
		}

		[NUnit.Framework.Test]
		public virtual void TestShowVersion()
		{
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.Version, ManagerParseShow.Parse(
				"show @@version", 5));
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.Version, ManagerParseShow.Parse(
				"SHOW @@VERSION", 5));
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.Version, ManagerParseShow.Parse(
				"show @@VERSION", 5));
		}

		[NUnit.Framework.Test]
		public virtual void TestShowSQL()
		{
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.Sql, ManagerParseShow.Parse("show @@sql where id = -1079800749"
				, 5));
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.Sql, ManagerParseShow.Parse("SHOW @@SQL WHERE ID = -1079800749"
				, 5));
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.Sql, ManagerParseShow.Parse("show @@Sql WHERE ID = -1079800749"
				, 5));
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.Sql, ManagerParseShow.Parse("show @@sql where id=-1079800749"
				, 5));
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.Sql, ManagerParseShow.Parse("show @@sql where id   =-1079800749 "
				, 5));
		}

		[NUnit.Framework.Test]
		public virtual void TestShowSQLDetail()
		{
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.SqlDetail, ManagerParseShow.Parse
				("show @@sql.detail where id = -1079800749", 5));
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.SqlDetail, ManagerParseShow.Parse
				("SHOW @@SQL.DETAIL WHERE ID = -1079800749", 5));
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.SqlDetail, ManagerParseShow.Parse
				("show @@SQL.DETAIL WHERE ID = -1079800749", 5));
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.SqlDetail, ManagerParseShow.Parse
				("show @@sql.detail where id=1079800749 ", 5));
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.SqlDetail, ManagerParseShow.Parse
				("show @@sql.detail where id= -1079800749", 5));
		}

		[NUnit.Framework.Test]
		public virtual void TestShowSQLExecute()
		{
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.SqlExecute, ManagerParseShow.Parse
				("show @@sql.execute", 5));
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.SqlExecute, ManagerParseShow.Parse
				("SHOW @@SQL.EXECUTE", 5));
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.SqlExecute, ManagerParseShow.Parse
				("show @@SQL.EXECUTE", 5));
		}

		[NUnit.Framework.Test]
		public virtual void TestShowSQLSlow()
		{
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.SqlSlow, ManagerParseShow.Parse(
				"show @@sql.slow", 5));
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.SqlSlow, ManagerParseShow.Parse(
				"SHOW @@SQL.SLOW", 5));
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.SqlSlow, ManagerParseShow.Parse(
				"SHOW @@sql.slow", 5));
		}

		[NUnit.Framework.Test]
		public virtual void TestShowVariables()
		{
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.Variables, ManagerParseShow.Parse
				("show variables", 5));
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.Variables, ManagerParseShow.Parse
				("SHOW VARIABLES", 5));
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.Variables, ManagerParseShow.Parse
				("show VARIABLES", 5));
		}

		[NUnit.Framework.Test]
		public virtual void TestShowCollation()
		{
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.Collation, ManagerParseShow.Parse
				("show collation", 5));
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.Collation, ManagerParseShow.Parse
				("SHOW COLLATION", 5));
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.Collation, ManagerParseShow.Parse
				("show COLLATION", 5));
		}

		[NUnit.Framework.Test]
		public virtual void TestSwitchPool()
		{
			NUnit.Framework.Assert.AreEqual(ManagerParse.Switch, unchecked((int)(0xff)) & ManagerParse
				.Parse("switch @@pool offer2$0-2"));
			NUnit.Framework.Assert.AreEqual(ManagerParse.Switch, unchecked((int)(0xff)) & ManagerParse
				.Parse("SWITCH @@POOL offer2$0-2"));
			NUnit.Framework.Assert.AreEqual(ManagerParse.Switch, unchecked((int)(0xff)) & ManagerParse
				.Parse("switch @@pool offer2$0-2 :2"));
		}

		[NUnit.Framework.Test]
		public virtual void TestComment()
		{
			NUnit.Framework.Assert.AreEqual(ManagerParse.Switch, unchecked((int)(0xff)) & ManagerParse
				.Parse("/* abc */switch @@pool offer2$0-2"));
			NUnit.Framework.Assert.AreEqual(ManagerParse.Show, unchecked((int)(0xff)) & ManagerParse
				.Parse(" /** 111**/Show @@help"));
			NUnit.Framework.Assert.AreEqual(ManagerParse.Select, unchecked((int)(0xff)) & ManagerParse
				.Parse(" /***/ select * from t "));
		}

		[NUnit.Framework.Test]
		public virtual void TestShowWhitComment()
		{
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.Variables, ManagerParseShow.Parse
				(" /** 111**/show variables", " /** 111**/show".Length));
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.Variables, ManagerParseShow.Parse
				(" /**111**/ SHOW VARIABLES", " /** 111**/show".Length));
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.Variables, ManagerParseShow.Parse
				(" /**111**/ SHOW variables", " /** 111**/show".Length));
		}

		[NUnit.Framework.Test]
		public virtual void TestStop()
		{
			NUnit.Framework.Assert.AreEqual(ManagerParse.Stop, unchecked((int)(0xff)) & ManagerParse
				.Parse("stop @@"));
			NUnit.Framework.Assert.AreEqual(ManagerParse.Stop, unchecked((int)(0xff)) & ManagerParse
				.Parse(" STOP "));
		}

		[NUnit.Framework.Test]
		public virtual void TestStopHeartBeat()
		{
			NUnit.Framework.Assert.AreEqual(ManagerParseStop.Heartbeat, ManagerParseStop.Parse
				("stop @@heartbeat ds:1000", 4));
			NUnit.Framework.Assert.AreEqual(ManagerParseStop.Heartbeat, ManagerParseStop.Parse
				(" STOP  @@HEARTBEAT ds:1000", 5));
			NUnit.Framework.Assert.AreEqual(ManagerParseStop.Heartbeat, ManagerParseStop.Parse
				(" STOP  @@heartbeat ds:1000", 5));
		}

		[NUnit.Framework.Test]
		public virtual void TestReload()
		{
			NUnit.Framework.Assert.AreEqual(ManagerParse.Reload, unchecked((int)(0xff)) & ManagerParse
				.Parse("reload @@"));
			NUnit.Framework.Assert.AreEqual(ManagerParse.Reload, unchecked((int)(0xff)) & ManagerParse
				.Parse(" RELOAD "));
		}

		[NUnit.Framework.Test]
		public virtual void TestReloadConfig()
		{
			NUnit.Framework.Assert.AreEqual(ManagerParseReload.Config, ManagerParseReload.Parse
				("reload @@config", 7));
			NUnit.Framework.Assert.AreEqual(ManagerParseReload.Config, ManagerParseReload.Parse
				(" RELOAD  @@CONFIG ", 7));
			NUnit.Framework.Assert.AreEqual(ManagerParseReload.Config, ManagerParseReload.Parse
				(" RELOAD  @@config ", 7));
		}

		[NUnit.Framework.Test]
		public virtual void TestReloadRoute()
		{
			NUnit.Framework.Assert.AreEqual(ManagerParseReload.Route, ManagerParseReload.Parse
				("reload @@route", 7));
			NUnit.Framework.Assert.AreEqual(ManagerParseReload.Route, ManagerParseReload.Parse
				(" RELOAD  @@ROUTE ", 7));
			NUnit.Framework.Assert.AreEqual(ManagerParseReload.Route, ManagerParseReload.Parse
				(" RELOAD  @@route ", 7));
		}

		[NUnit.Framework.Test]
		public virtual void TestReloadUser()
		{
			NUnit.Framework.Assert.AreEqual(ManagerParseReload.User, ManagerParseReload.Parse
				("reload @@user", 7));
			NUnit.Framework.Assert.AreEqual(ManagerParseReload.User, ManagerParseReload.Parse
				(" RELOAD  @@USER ", 7));
			NUnit.Framework.Assert.AreEqual(ManagerParseReload.User, ManagerParseReload.Parse
				(" RELOAD  @@user ", 7));
		}

		[NUnit.Framework.Test]
		public virtual void TestRollback()
		{
			NUnit.Framework.Assert.AreEqual(ManagerParse.Rollback, unchecked((int)(0xff)) & ManagerParse
				.Parse("rollback @@"));
			NUnit.Framework.Assert.AreEqual(ManagerParse.Rollback, unchecked((int)(0xff)) & ManagerParse
				.Parse(" ROLLBACK "));
		}

		[NUnit.Framework.Test]
		public virtual void TestOnOff()
		{
			NUnit.Framework.Assert.AreEqual(ManagerParse.Online, ManagerParse.Parse("online "
				));
			NUnit.Framework.Assert.AreEqual(ManagerParse.Online, ManagerParse.Parse(" Online"
				));
			NUnit.Framework.Assert.AreEqual(ManagerParse.Other, ManagerParse.Parse(" Online2"
				));
			NUnit.Framework.Assert.AreEqual(ManagerParse.Other, ManagerParse.Parse("Online2 "
				));
			NUnit.Framework.Assert.AreEqual(ManagerParse.Offline, ManagerParse.Parse(" Offline"
				));
			NUnit.Framework.Assert.AreEqual(ManagerParse.Offline, ManagerParse.Parse("offLine\t"
				));
			NUnit.Framework.Assert.AreEqual(ManagerParse.Other, ManagerParse.Parse("onLin"));
			NUnit.Framework.Assert.AreEqual(ManagerParse.Other, ManagerParse.Parse(" onlin"));
		}

		[NUnit.Framework.Test]
		public virtual void TestRollbackConfig()
		{
			NUnit.Framework.Assert.AreEqual(ManagerParseRollback.Config, ManagerParseRollback
				.Parse("rollback @@config", 8));
			NUnit.Framework.Assert.AreEqual(ManagerParseRollback.Config, ManagerParseRollback
				.Parse(" ROLLBACK  @@CONFIG ", 9));
			NUnit.Framework.Assert.AreEqual(ManagerParseRollback.Config, ManagerParseRollback
				.Parse(" ROLLBACK  @@config ", 9));
		}

		[NUnit.Framework.Test]
		public virtual void TestRollbackUser()
		{
			NUnit.Framework.Assert.AreEqual(ManagerParseRollback.User, ManagerParseRollback.Parse
				("rollback @@user", 9));
			NUnit.Framework.Assert.AreEqual(ManagerParseRollback.User, ManagerParseRollback.Parse
				(" ROLLBACK  @@USER ", 9));
			NUnit.Framework.Assert.AreEqual(ManagerParseRollback.User, ManagerParseRollback.Parse
				(" ROLLBACK  @@user ", 9));
		}

		[NUnit.Framework.Test]
		public virtual void TestRollbackRoute()
		{
			NUnit.Framework.Assert.AreEqual(ManagerParseRollback.Route, ManagerParseRollback.
				Parse("rollback @@route", 9));
			NUnit.Framework.Assert.AreEqual(ManagerParseRollback.Route, ManagerParseRollback.
				Parse(" ROLLBACK  @@ROUTE ", 9));
			NUnit.Framework.Assert.AreEqual(ManagerParseRollback.Route, ManagerParseRollback.
				Parse(" ROLLBACK  @@route ", 9));
		}

		[NUnit.Framework.Test]
		public virtual void TestGetWhere()
		{
			NUnit.Framework.Assert.AreEqual("123", ManagerParseShow.GetWhereParameter("where id = 123"
				));
			NUnit.Framework.Assert.AreEqual("datanode", ManagerParseShow.GetWhereParameter("where datanode =    datanode"
				));
			NUnit.Framework.Assert.AreEqual("schema", ManagerParseShow.GetWhereParameter("where schema =schema   "
				));
		}

		[NUnit.Framework.Test]
		public virtual void TestShowSlowSchema()
		{
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.SlowSchema, unchecked((int)(0xff
				)) & ManagerParseShow.Parse("show @@slow where schema=a", 5));
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.SlowSchema, unchecked((int)(0xff
				)) & ManagerParseShow.Parse("  SHOW @@SLOW   WHERE SCHEMA=B", 6));
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.SlowSchema, unchecked((int)(0xff
				)) & ManagerParseShow.Parse(" show @@slow  WHERE  SCHEMA  = a ", 5));
		}

		[NUnit.Framework.Test]
		public virtual void TestShowSlowDataNode()
		{
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.SlowDatanode, unchecked((int)(0xff
				)) & ManagerParseShow.Parse("show @@slow where datanode= a", 5));
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.SlowDatanode, unchecked((int)(0xff
				)) & ManagerParseShow.Parse("SHOW @@SLOW WHERE DATANODE= A", 5));
			NUnit.Framework.Assert.AreEqual(ManagerParseShow.SlowDatanode, unchecked((int)(0xff
				)) & ManagerParseShow.Parse(" show @@SLOW where DATANODE= b ", 5));
		}

		[NUnit.Framework.Test]
		public virtual void TestclearSlowSchema()
		{
			NUnit.Framework.Assert.AreEqual(ManagerParseClear.SlowSchema, unchecked((int)(0xff
				)) & ManagerParseClear.Parse("clear @@slow where schema=s", 5));
			NUnit.Framework.Assert.AreEqual(ManagerParseClear.SlowSchema, unchecked((int)(0xff
				)) & ManagerParseClear.Parse("CLEAR @@SLOW WHERE SCHEMA= S", 5));
			NUnit.Framework.Assert.AreEqual(ManagerParseClear.SlowSchema, unchecked((int)(0xff
				)) & ManagerParseClear.Parse("CLEAR @@slow where SCHEMA= s", 5));
		}

		[NUnit.Framework.Test]
		public virtual void TestclearSlowDataNode()
		{
			NUnit.Framework.Assert.AreEqual(ManagerParseClear.SlowDatanode, unchecked((int)(0xff
				)) & ManagerParseClear.Parse("clear @@slow where datanode=d", 5));
			NUnit.Framework.Assert.AreEqual(ManagerParseClear.SlowDatanode, unchecked((int)(0xff
				)) & ManagerParseClear.Parse("CLEAR @@SLOW WHERE DATANODE= D", 5));
			NUnit.Framework.Assert.AreEqual(ManagerParseClear.SlowDatanode, unchecked((int)(0xff
				)) & ManagerParseClear.Parse("clear @@SLOW where  DATANODE= d", 5));
		}
	}
}
