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
using Tup.Cobar.Server.Parser;

namespace Tup.Cobar4Net.Parser
{
	/// <author>xianmao.hexm</author>
	[NUnit.Framework.TestFixture]
	public class ServerParserTest
	{
		[NUnit.Framework.Test]
		public virtual void TestIsBegin()
		{
			NUnit.Framework.Assert.AreEqual(ServerParse.Begin, ServerParse.Parse("begin"));
			NUnit.Framework.Assert.AreEqual(ServerParse.Begin, ServerParse.Parse("BEGIN"));
			NUnit.Framework.Assert.AreEqual(ServerParse.Begin, ServerParse.Parse("BegIn"));
		}

		[NUnit.Framework.Test]
		public virtual void TestIsCommit()
		{
			NUnit.Framework.Assert.AreEqual(ServerParse.Commit, ServerParse.Parse("commit"));
			NUnit.Framework.Assert.AreEqual(ServerParse.Commit, ServerParse.Parse("COMMIT"));
			NUnit.Framework.Assert.AreEqual(ServerParse.Commit, ServerParse.Parse("cOmmiT "));
		}

		[NUnit.Framework.Test]
		public virtual void TestIsDelete()
		{
			NUnit.Framework.Assert.AreEqual(ServerParse.Delete, ServerParse.Parse("delete ..."
				));
			NUnit.Framework.Assert.AreEqual(ServerParse.Delete, ServerParse.Parse("DELETE ..."
				));
			NUnit.Framework.Assert.AreEqual(ServerParse.Delete, ServerParse.Parse("DeletE ..."
				));
		}

		[NUnit.Framework.Test]
		public virtual void TestIsInsert()
		{
			NUnit.Framework.Assert.AreEqual(ServerParse.Insert, ServerParse.Parse("insert ..."
				));
			NUnit.Framework.Assert.AreEqual(ServerParse.Insert, ServerParse.Parse("INSERT ..."
				));
			NUnit.Framework.Assert.AreEqual(ServerParse.Insert, ServerParse.Parse("InserT ..."
				));
		}

		[NUnit.Framework.Test]
		public virtual void TestIsReplace()
		{
			NUnit.Framework.Assert.AreEqual(ServerParse.Replace, ServerParse.Parse("replace ..."
				));
			NUnit.Framework.Assert.AreEqual(ServerParse.Replace, ServerParse.Parse("REPLACE ..."
				));
			NUnit.Framework.Assert.AreEqual(ServerParse.Replace, ServerParse.Parse("rEPLACe ..."
				));
		}

		[NUnit.Framework.Test]
		public virtual void TestIsRollback()
		{
			NUnit.Framework.Assert.AreEqual(ServerParse.Rollback, ServerParse.Parse("rollback"
				));
			NUnit.Framework.Assert.AreEqual(ServerParse.Rollback, ServerParse.Parse("ROLLBACK"
				));
			NUnit.Framework.Assert.AreEqual(ServerParse.Rollback, ServerParse.Parse("rolLBACK "
				));
		}

		[NUnit.Framework.Test]
		public virtual void TestIsSelect()
		{
			NUnit.Framework.Assert.AreEqual(ServerParse.Select, unchecked((int)(0xff)) & ServerParse
				.Parse("select ..."));
			NUnit.Framework.Assert.AreEqual(ServerParse.Select, unchecked((int)(0xff)) & ServerParse
				.Parse("SELECT ..."));
			NUnit.Framework.Assert.AreEqual(ServerParse.Select, unchecked((int)(0xff)) & ServerParse
				.Parse("sELECt ..."));
		}

		[NUnit.Framework.Test]
		public virtual void TestIsSet()
		{
			NUnit.Framework.Assert.AreEqual(ServerParse.Set, unchecked((int)(0xff)) & ServerParse
				.Parse("set ..."));
			NUnit.Framework.Assert.AreEqual(ServerParse.Set, unchecked((int)(0xff)) & ServerParse
				.Parse("SET ..."));
			NUnit.Framework.Assert.AreEqual(ServerParse.Set, unchecked((int)(0xff)) & ServerParse
				.Parse("sEt ..."));
		}

		[NUnit.Framework.Test]
		public virtual void TestIsShow()
		{
			NUnit.Framework.Assert.AreEqual(ServerParse.Show, unchecked((int)(0xff)) & ServerParse
				.Parse("show ..."));
			NUnit.Framework.Assert.AreEqual(ServerParse.Show, unchecked((int)(0xff)) & ServerParse
				.Parse("SHOW ..."));
			NUnit.Framework.Assert.AreEqual(ServerParse.Show, unchecked((int)(0xff)) & ServerParse
				.Parse("sHOw ..."));
		}

		[NUnit.Framework.Test]
		public virtual void TestIsStart()
		{
			NUnit.Framework.Assert.AreEqual(ServerParse.Start, unchecked((int)(0xff)) & ServerParse
				.Parse("start ..."));
			NUnit.Framework.Assert.AreEqual(ServerParse.Start, unchecked((int)(0xff)) & ServerParse
				.Parse("START ..."));
			NUnit.Framework.Assert.AreEqual(ServerParse.Start, unchecked((int)(0xff)) & ServerParse
				.Parse("stART ..."));
		}

		[NUnit.Framework.Test]
		public virtual void TestIsUpdate()
		{
			NUnit.Framework.Assert.AreEqual(ServerParse.Update, ServerParse.Parse("update ..."
				));
			NUnit.Framework.Assert.AreEqual(ServerParse.Update, ServerParse.Parse("UPDATE ..."
				));
			NUnit.Framework.Assert.AreEqual(ServerParse.Update, ServerParse.Parse("UPDate ..."
				));
		}

		[NUnit.Framework.Test]
		public virtual void TestIsShowDatabases()
		{
			NUnit.Framework.Assert.AreEqual(ServerParseShow.Databases, ServerParseShow.Parse(
				"show databases", 4));
			NUnit.Framework.Assert.AreEqual(ServerParseShow.Databases, ServerParseShow.Parse(
				"SHOW DATABASES", 4));
			NUnit.Framework.Assert.AreEqual(ServerParseShow.Databases, ServerParseShow.Parse(
				"SHOW databases ", 4));
		}

		[NUnit.Framework.Test]
		public virtual void TestIsShowDataSources()
		{
			NUnit.Framework.Assert.AreEqual(ServerParseShow.Datasources, ServerParseShow.Parse
				("show datasources", 4));
			NUnit.Framework.Assert.AreEqual(ServerParseShow.Datasources, ServerParseShow.Parse
				("SHOW DATASOURCES", 4));
			NUnit.Framework.Assert.AreEqual(ServerParseShow.Datasources, ServerParseShow.Parse
				("  SHOW   DATASOURCES  ", 6));
		}

		[NUnit.Framework.Test]
		public virtual void TestShowCobarStatus()
		{
			NUnit.Framework.Assert.AreEqual(ServerParseShow.CobarStatus, ServerParseShow.Parse
				("show cobar_status", 4));
			NUnit.Framework.Assert.AreEqual(ServerParseShow.CobarStatus, ServerParseShow.Parse
				("show cobar_status ", 4));
			NUnit.Framework.Assert.AreEqual(ServerParseShow.CobarStatus, ServerParseShow.Parse
				(" SHOW COBAR_STATUS", " SHOW".Length));
			NUnit.Framework.Assert.AreEqual(ServerParseShow.Other, ServerParseShow.Parse(" show cobar_statu"
				, " SHOW".Length));
			NUnit.Framework.Assert.AreEqual(ServerParseShow.Other, ServerParseShow.Parse(" show cobar_status2"
				, " SHOW".Length));
			NUnit.Framework.Assert.AreEqual(ServerParseShow.Other, ServerParseShow.Parse("Show cobar_status2 "
				, "SHOW".Length));
		}

		[NUnit.Framework.Test]
		public virtual void TestShowCobarCluster()
		{
			NUnit.Framework.Assert.AreEqual(ServerParseShow.CobarCluster, ServerParseShow.Parse
				("show cobar_cluster", 4));
			NUnit.Framework.Assert.AreEqual(ServerParseShow.CobarCluster, ServerParseShow.Parse
				("Show cobar_CLUSTER ", 4));
			NUnit.Framework.Assert.AreEqual(ServerParseShow.CobarCluster, ServerParseShow.Parse
				(" show  COBAR_cluster", 5));
			NUnit.Framework.Assert.AreEqual(ServerParseShow.Other, ServerParseShow.Parse(" show cobar_clust"
				, 5));
			NUnit.Framework.Assert.AreEqual(ServerParseShow.Other, ServerParseShow.Parse(" show cobar_cluster2"
				, 5));
			NUnit.Framework.Assert.AreEqual(ServerParseShow.Other, ServerParseShow.Parse("Show COBAR_cluster9 "
				, 4));
		}

		[NUnit.Framework.Test]
		public virtual void TestIsShowOther()
		{
			NUnit.Framework.Assert.AreEqual(ServerParseShow.Other, ServerParseShow.Parse("show ..."
				, 4));
			NUnit.Framework.Assert.AreEqual(ServerParseShow.Other, ServerParseShow.Parse("SHOW ..."
				, 4));
			NUnit.Framework.Assert.AreEqual(ServerParseShow.Other, ServerParseShow.Parse("SHOW ... "
				, 4));
		}

		[NUnit.Framework.Test]
		public virtual void TestIsSetAutocommitOn()
		{
			NUnit.Framework.Assert.AreEqual(ServerParseSet.AutocommitOn, ServerParseSet.Parse
				("set autocommit=1", 3));
			NUnit.Framework.Assert.AreEqual(ServerParseSet.AutocommitOn, ServerParseSet.Parse
				("set autoCOMMIT = 1", 3));
			NUnit.Framework.Assert.AreEqual(ServerParseSet.AutocommitOn, ServerParseSet.Parse
				("SET AUTOCOMMIT=on", 3));
			NUnit.Framework.Assert.AreEqual(ServerParseSet.AutocommitOn, ServerParseSet.Parse
				("set autoCOMMIT = ON", 3));
		}

		[NUnit.Framework.Test]
		public virtual void TestIsSetAutocommitOff()
		{
			NUnit.Framework.Assert.AreEqual(ServerParseSet.AutocommitOff, ServerParseSet.Parse
				("set autocommit=0", 3));
			NUnit.Framework.Assert.AreEqual(ServerParseSet.AutocommitOff, ServerParseSet.Parse
				("SET AUTOCOMMIT= 0", 3));
			NUnit.Framework.Assert.AreEqual(ServerParseSet.AutocommitOff, ServerParseSet.Parse
				("set autoCOMMIT =OFF", 3));
			NUnit.Framework.Assert.AreEqual(ServerParseSet.AutocommitOff, ServerParseSet.Parse
				("set autoCOMMIT = off", 3));
		}

		[NUnit.Framework.Test]
		public virtual void TestIsSetNames()
		{
			NUnit.Framework.Assert.AreEqual(ServerParseSet.Names, unchecked((int)(0xff)) & ServerParseSet
				.Parse("set names utf8", 3));
			NUnit.Framework.Assert.AreEqual(ServerParseSet.Names, unchecked((int)(0xff)) & ServerParseSet
				.Parse("SET NAMES UTF8", 3));
			NUnit.Framework.Assert.AreEqual(ServerParseSet.Names, unchecked((int)(0xff)) & ServerParseSet
				.Parse("set NAMES utf8", 3));
		}

		[NUnit.Framework.Test]
		public virtual void TestIsCharacterSetResults()
		{
			NUnit.Framework.Assert.AreEqual(ServerParseSet.CharacterSetResults, unchecked((int
				)(0xff)) & ServerParseSet.Parse("SET character_set_results  = NULL", 3));
			NUnit.Framework.Assert.AreEqual(ServerParseSet.CharacterSetResults, unchecked((int
				)(0xff)) & ServerParseSet.Parse("SET CHARACTER_SET_RESULTS= NULL", 3));
			NUnit.Framework.Assert.AreEqual(ServerParseSet.CharacterSetResults, unchecked((int
				)(0xff)) & ServerParseSet.Parse("Set chARActer_SET_RESults =  NULL", 3));
			NUnit.Framework.Assert.AreEqual(ServerParseSet.CharacterSetConnection, unchecked(
				(int)(0xff)) & ServerParseSet.Parse("Set chARActer_SET_Connection =  NULL", 3));
			NUnit.Framework.Assert.AreEqual(ServerParseSet.CharacterSetClient, unchecked((int
				)(0xff)) & ServerParseSet.Parse("Set chARActer_SET_client =  NULL", 3));
		}

		[NUnit.Framework.Test]
		public virtual void TestIsSetOther()
		{
			NUnit.Framework.Assert.AreEqual(ServerParseSet.Other, ServerParseSet.Parse("set ..."
				, 3));
			NUnit.Framework.Assert.AreEqual(ServerParseSet.Other, ServerParseSet.Parse("SET ..."
				, 3));
			NUnit.Framework.Assert.AreEqual(ServerParseSet.Other, ServerParseSet.Parse("sEt ..."
				, 3));
		}

		[NUnit.Framework.Test]
		public virtual void TestIsKill()
		{
			NUnit.Framework.Assert.AreEqual(ServerParse.Kill, unchecked((int)(0xff)) & ServerParse
				.Parse(" kill  ..."));
			NUnit.Framework.Assert.AreEqual(ServerParse.Kill, unchecked((int)(0xff)) & ServerParse
				.Parse("kill 111111 ..."));
			NUnit.Framework.Assert.AreEqual(ServerParse.Kill, unchecked((int)(0xff)) & ServerParse
				.Parse("KILL  1335505632"));
		}

		[NUnit.Framework.Test]
		public virtual void TestIsKillQuery()
		{
			NUnit.Framework.Assert.AreEqual(ServerParse.KillQuery, unchecked((int)(0xff)) & ServerParse
				.Parse(" kill query ..."));
			NUnit.Framework.Assert.AreEqual(ServerParse.KillQuery, unchecked((int)(0xff)) & ServerParse
				.Parse("kill   query 111111 ..."));
			NUnit.Framework.Assert.AreEqual(ServerParse.KillQuery, unchecked((int)(0xff)) & ServerParse
				.Parse("KILL QUERY 1335505632"));
		}

		[NUnit.Framework.Test]
		public virtual void TestIsSavepoint()
		{
			NUnit.Framework.Assert.AreEqual(ServerParse.Savepoint, ServerParse.Parse(" savepoint  ..."
				));
			NUnit.Framework.Assert.AreEqual(ServerParse.Savepoint, ServerParse.Parse("SAVEPOINT "
				));
			NUnit.Framework.Assert.AreEqual(ServerParse.Savepoint, ServerParse.Parse(" SAVEpoint   a"
				));
		}

		[NUnit.Framework.Test]
		public virtual void TestIsUse()
		{
			NUnit.Framework.Assert.AreEqual(ServerParse.Use, unchecked((int)(0xff)) & ServerParse
				.Parse(" use  ..."));
			NUnit.Framework.Assert.AreEqual(ServerParse.Use, unchecked((int)(0xff)) & ServerParse
				.Parse("USE "));
			NUnit.Framework.Assert.AreEqual(ServerParse.Use, unchecked((int)(0xff)) & ServerParse
				.Parse(" Use   a"));
		}

		[NUnit.Framework.Test]
		public virtual void TestIsStartTransaction()
		{
			NUnit.Framework.Assert.AreEqual(ServerParseStart.Transaction, ServerParseStart.Parse
				(" start transaction  ...", 6));
			NUnit.Framework.Assert.AreEqual(ServerParseStart.Transaction, ServerParseStart.Parse
				("START TRANSACTION", 5));
			NUnit.Framework.Assert.AreEqual(ServerParseStart.Transaction, ServerParseStart.Parse
				(" staRT   TRANSaction  ", 6));
		}

		[NUnit.Framework.Test]
		public virtual void TestIsSelectVersionComment()
		{
			NUnit.Framework.Assert.AreEqual(ServerParseSelect.VersionComment, ServerParseSelect
				.Parse(" select @@version_comment  ", 7));
			NUnit.Framework.Assert.AreEqual(ServerParseSelect.VersionComment, ServerParseSelect
				.Parse("SELECT @@VERSION_COMMENT", 6));
			NUnit.Framework.Assert.AreEqual(ServerParseSelect.VersionComment, ServerParseSelect
				.Parse(" selECT    @@VERSION_comment  ", 7));
		}

		[NUnit.Framework.Test]
		public virtual void TestIsSelectVersion()
		{
			NUnit.Framework.Assert.AreEqual(ServerParseSelect.Version, ServerParseSelect.Parse
				(" select version ()  ", 7));
			NUnit.Framework.Assert.AreEqual(ServerParseSelect.Version, ServerParseSelect.Parse
				("SELECT VERSION(  )", 6));
			NUnit.Framework.Assert.AreEqual(ServerParseSelect.Version, ServerParseSelect.Parse
				(" selECT    VERSION()  ", 7));
		}

		[NUnit.Framework.Test]
		public virtual void TestIsSelectDatabase()
		{
			NUnit.Framework.Assert.AreEqual(ServerParseSelect.Database, ServerParseSelect.Parse
				(" select database()  ", 7));
			NUnit.Framework.Assert.AreEqual(ServerParseSelect.Database, ServerParseSelect.Parse
				("SELECT DATABASE()", 6));
			NUnit.Framework.Assert.AreEqual(ServerParseSelect.Database, ServerParseSelect.Parse
				(" selECT    DATABASE()  ", 7));
		}

		[NUnit.Framework.Test]
		public virtual void TestIsSelectUser()
		{
			NUnit.Framework.Assert.AreEqual(ServerParseSelect.User, ServerParseSelect.Parse(" select user()  "
				, 7));
			NUnit.Framework.Assert.AreEqual(ServerParseSelect.User, ServerParseSelect.Parse("SELECT USER()"
				, 6));
			NUnit.Framework.Assert.AreEqual(ServerParseSelect.User, ServerParseSelect.Parse(" selECT    USER()  "
				, 7));
		}

		[NUnit.Framework.Test]
		public virtual void TestTxReadUncommitted()
		{
			NUnit.Framework.Assert.AreEqual(ServerParseSet.TxReadUncommitted, ServerParseSet.
				Parse("  SET SESSION TRANSACTION ISOLATION LEVEL READ  UNCOMMITTED  ", "  SET".Length
				));
			NUnit.Framework.Assert.AreEqual(ServerParseSet.TxReadUncommitted, ServerParseSet.
				Parse(" set session transaction isolation level read  uncommitted  ", " SET".Length
				));
			NUnit.Framework.Assert.AreEqual(ServerParseSet.TxReadUncommitted, ServerParseSet.
				Parse(" set session transaCTION ISOLATION LEvel read  uncommitteD ", " SET".Length
				));
		}

		[NUnit.Framework.Test]
		public virtual void TestTxReadCommitted()
		{
			NUnit.Framework.Assert.AreEqual(ServerParseSet.TxReadCommitted, ServerParseSet.Parse
				("  SET SESSION TRANSACTION ISOLATION LEVEL READ  COMMITTED  ", "  SET".Length));
			NUnit.Framework.Assert.AreEqual(ServerParseSet.TxReadCommitted, ServerParseSet.Parse
				(" set session transaction isolation level read  committed  ", " SET".Length));
			NUnit.Framework.Assert.AreEqual(ServerParseSet.TxReadCommitted, ServerParseSet.Parse
				(" set session transaCTION ISOLATION LEVel read  committed ", " SET".Length));
		}

		[NUnit.Framework.Test]
		public virtual void TestTxRepeatedRead()
		{
			NUnit.Framework.Assert.AreEqual(ServerParseSet.TxRepeatedRead, ServerParseSet.Parse
				("  SET SESSION TRANSACTION ISOLATION LEVEL REPEATABLE   READ  ", "  SET".Length
				));
			NUnit.Framework.Assert.AreEqual(ServerParseSet.TxRepeatedRead, ServerParseSet.Parse
				(" set session transaction isolation level repeatable   read  ", " SET".Length));
			NUnit.Framework.Assert.AreEqual(ServerParseSet.TxRepeatedRead, ServerParseSet.Parse
				(" set session transaction isOLATION LEVEL REPEatable   read ", " SET".Length));
		}

		[NUnit.Framework.Test]
		public virtual void TestTxSerializable()
		{
			NUnit.Framework.Assert.AreEqual(ServerParseSet.TxSerializable, ServerParseSet.Parse
				("  SET SESSION TRANSACTION ISOLATION LEVEL SERIALIZABLE  ", "  SET".Length));
			NUnit.Framework.Assert.AreEqual(ServerParseSet.TxSerializable, ServerParseSet.Parse
				(" set session transaction   isolation level serializable  ", " SET".Length));
			NUnit.Framework.Assert.AreEqual(ServerParseSet.TxSerializable, ServerParseSet.Parse
				(" set session   transaction  isOLATION LEVEL SERIAlizable ", " SET".Length));
		}

		[NUnit.Framework.Test]
		public virtual void TestIdentity()
		{
			string stmt = "select @@identity";
			int indexAfterLastInsertIdFunc = ServerParseSelect.IndexAfterIdentity(stmt, stmt.
				IndexOf('i'));
			NUnit.Framework.Assert.AreEqual(stmt.Length, indexAfterLastInsertIdFunc);
			NUnit.Framework.Assert.AreEqual(ServerParseSelect.Identity, ServerParseSelect.Parse
				(stmt, 6));
			stmt = "select  @@identity as id";
			NUnit.Framework.Assert.AreEqual(ServerParseSelect.Identity, ServerParseSelect.Parse
				(stmt, 6));
			stmt = "select  @@identitY  id";
			NUnit.Framework.Assert.AreEqual(ServerParseSelect.Identity, ServerParseSelect.Parse
				(stmt, 6));
			stmt = "select  /*foo*/@@identitY  id";
			NUnit.Framework.Assert.AreEqual(ServerParseSelect.Identity, ServerParseSelect.Parse
				(stmt, 6));
			stmt = "select/*foo*/ @@identitY  id";
			NUnit.Framework.Assert.AreEqual(ServerParseSelect.Identity, ServerParseSelect.Parse
				(stmt, 6));
			stmt = "select/*foo*/ @@identitY As id";
			NUnit.Framework.Assert.AreEqual(ServerParseSelect.Identity, ServerParseSelect.Parse
				(stmt, 6));
			stmt = "select  @@identity ,";
			NUnit.Framework.Assert.AreEqual(ServerParseSelect.Other, ServerParseSelect.Parse(
				stmt, 6));
			stmt = "select  @@identity as, ";
			NUnit.Framework.Assert.AreEqual(ServerParseSelect.Other, ServerParseSelect.Parse(
				stmt, 6));
			stmt = "select  @@identity as id  , ";
			NUnit.Framework.Assert.AreEqual(ServerParseSelect.Other, ServerParseSelect.Parse(
				stmt, 6));
			stmt = "select  @@identity ass id   ";
			NUnit.Framework.Assert.AreEqual(ServerParseSelect.Other, ServerParseSelect.Parse(
				stmt, 6));
		}

		[NUnit.Framework.Test]
		public virtual void TestLastInsertId()
		{
			string stmt = " last_insert_iD()";
			int indexAfterLastInsertIdFunc = ServerParseSelect.IndexAfterLastInsertIdFunc(stmt
				, stmt.IndexOf('l'));
			NUnit.Framework.Assert.AreEqual(stmt.Length, indexAfterLastInsertIdFunc);
			stmt = " last_insert_iD ()";
			indexAfterLastInsertIdFunc = ServerParseSelect.IndexAfterLastInsertIdFunc(stmt, stmt
				.IndexOf('l'));
			NUnit.Framework.Assert.AreEqual(stmt.Length, indexAfterLastInsertIdFunc);
			stmt = " last_insert_iD ( /**/ )";
			indexAfterLastInsertIdFunc = ServerParseSelect.IndexAfterLastInsertIdFunc(stmt, stmt
				.IndexOf('l'));
			NUnit.Framework.Assert.AreEqual(stmt.Length, indexAfterLastInsertIdFunc);
			stmt = " last_insert_iD (  )  ";
			indexAfterLastInsertIdFunc = ServerParseSelect.IndexAfterLastInsertIdFunc(stmt, stmt
				.IndexOf('l'));
			NUnit.Framework.Assert.AreEqual(stmt.LastIndexOf(')') + 1, indexAfterLastInsertIdFunc
				);
			stmt = " last_insert_id(  )";
			indexAfterLastInsertIdFunc = ServerParseSelect.IndexAfterLastInsertIdFunc(stmt, stmt
				.IndexOf('l'));
			NUnit.Framework.Assert.AreEqual(stmt.LastIndexOf(')') + 1, indexAfterLastInsertIdFunc
				);
			stmt = "last_iNsert_id(  ) ";
			indexAfterLastInsertIdFunc = ServerParseSelect.IndexAfterLastInsertIdFunc(stmt, stmt
				.IndexOf('l'));
			NUnit.Framework.Assert.AreEqual(stmt.LastIndexOf(')') + 1, indexAfterLastInsertIdFunc
				);
			stmt = " last_insert_iD";
			indexAfterLastInsertIdFunc = ServerParseSelect.IndexAfterLastInsertIdFunc(stmt, stmt
				.IndexOf('l'));
			NUnit.Framework.Assert.AreEqual(-1, indexAfterLastInsertIdFunc);
			stmt = " last_insert_i     ";
			indexAfterLastInsertIdFunc = ServerParseSelect.IndexAfterLastInsertIdFunc(stmt, stmt
				.IndexOf('l'));
			NUnit.Framework.Assert.AreEqual(-1, indexAfterLastInsertIdFunc);
			stmt = " last_insert_i    d ";
			indexAfterLastInsertIdFunc = ServerParseSelect.IndexAfterLastInsertIdFunc(stmt, stmt
				.IndexOf('l'));
			NUnit.Framework.Assert.AreEqual(-1, indexAfterLastInsertIdFunc);
			stmt = " last_insert_id (     ";
			indexAfterLastInsertIdFunc = ServerParseSelect.IndexAfterLastInsertIdFunc(stmt, stmt
				.IndexOf('l'));
			NUnit.Framework.Assert.AreEqual(-1, indexAfterLastInsertIdFunc);
			stmt = " last_insert_id(  d)     ";
			indexAfterLastInsertIdFunc = ServerParseSelect.IndexAfterLastInsertIdFunc(stmt, stmt
				.IndexOf('l'));
			NUnit.Framework.Assert.AreEqual(-1, indexAfterLastInsertIdFunc);
			stmt = " last_insert_id(  ) d    ";
			indexAfterLastInsertIdFunc = ServerParseSelect.IndexAfterLastInsertIdFunc(stmt, stmt
				.IndexOf('l'));
			NUnit.Framework.Assert.AreEqual(stmt.LastIndexOf(')') + 1, indexAfterLastInsertIdFunc
				);
			stmt = " last_insert_id(d)";
			indexAfterLastInsertIdFunc = ServerParseSelect.IndexAfterLastInsertIdFunc(stmt, stmt
				.IndexOf('l'));
			NUnit.Framework.Assert.AreEqual(-1, indexAfterLastInsertIdFunc);
			stmt = " last_insert_id(#\r\nd) ";
			indexAfterLastInsertIdFunc = ServerParseSelect.IndexAfterLastInsertIdFunc(stmt, stmt
				.IndexOf('l'));
			NUnit.Framework.Assert.AreEqual(-1, indexAfterLastInsertIdFunc);
			stmt = " last_insert_id(#\n\r) ";
			indexAfterLastInsertIdFunc = ServerParseSelect.IndexAfterLastInsertIdFunc(stmt, stmt
				.IndexOf('l'));
			NUnit.Framework.Assert.AreEqual(stmt.LastIndexOf(')') + 1, indexAfterLastInsertIdFunc
				);
			stmt = " last_insert_id (#\n\r)";
			indexAfterLastInsertIdFunc = ServerParseSelect.IndexAfterLastInsertIdFunc(stmt, stmt
				.IndexOf('l'));
			NUnit.Framework.Assert.AreEqual(stmt.LastIndexOf(')') + 1, indexAfterLastInsertIdFunc
				);
			stmt = " last_insert_id(#\n\r)";
			indexAfterLastInsertIdFunc = ServerParseSelect.IndexAfterLastInsertIdFunc(stmt, stmt
				.IndexOf('l'));
			NUnit.Framework.Assert.AreEqual(stmt.LastIndexOf(')') + 1, indexAfterLastInsertIdFunc
				);
			stmt = "select last_insert_id(#\n\r)";
			NUnit.Framework.Assert.AreEqual(ServerParseSelect.LastInsertId, ServerParseSelect
				.Parse(stmt, 6));
			stmt = "select last_insert_id(#\n\r) as id";
			NUnit.Framework.Assert.AreEqual(ServerParseSelect.LastInsertId, ServerParseSelect
				.Parse(stmt, 6));
			stmt = "select last_insert_id(#\n\r) as `id`";
			NUnit.Framework.Assert.AreEqual(ServerParseSelect.LastInsertId, ServerParseSelect
				.Parse(stmt, 6));
			stmt = "select last_insert_id(#\n\r) as 'id'";
			NUnit.Framework.Assert.AreEqual(ServerParseSelect.LastInsertId, ServerParseSelect
				.Parse(stmt, 6));
			stmt = "select last_insert_id(#\n\r)  id";
			NUnit.Framework.Assert.AreEqual(ServerParseSelect.LastInsertId, ServerParseSelect
				.Parse(stmt, 6));
			stmt = "select last_insert_id(#\n\r)  `id`";
			NUnit.Framework.Assert.AreEqual(ServerParseSelect.LastInsertId, ServerParseSelect
				.Parse(stmt, 6));
			stmt = "select last_insert_id(#\n\r)  'id'";
			NUnit.Framework.Assert.AreEqual(ServerParseSelect.LastInsertId, ServerParseSelect
				.Parse(stmt, 6));
			stmt = "select last_insert_id(#\n\r) a";
			NUnit.Framework.Assert.AreEqual(ServerParseSelect.LastInsertId, ServerParseSelect
				.Parse(stmt, 6));
			// NOTE: this should be invalid, ignore this bug
			stmt = "select last_insert_id(#\n\r) as";
			NUnit.Framework.Assert.AreEqual(ServerParseSelect.LastInsertId, ServerParseSelect
				.Parse(stmt, 6));
			stmt = "select last_insert_id(#\n\r) asd";
			NUnit.Framework.Assert.AreEqual(ServerParseSelect.LastInsertId, ServerParseSelect
				.Parse(stmt, 6));
			// NOTE: this should be invalid, ignore this bug
			stmt = "select last_insert_id(#\n\r) as 777";
			NUnit.Framework.Assert.AreEqual(ServerParseSelect.LastInsertId, ServerParseSelect
				.Parse(stmt, 6));
			// NOTE: this should be invalid, ignore this bug
			stmt = "select last_insert_id(#\n\r)  777";
			NUnit.Framework.Assert.AreEqual(ServerParseSelect.LastInsertId, ServerParseSelect
				.Parse(stmt, 6));
			stmt = "select last_insert_id(#\n\r)as `77``7`";
			NUnit.Framework.Assert.AreEqual(ServerParseSelect.LastInsertId, ServerParseSelect
				.Parse(stmt, 6));
			stmt = "select last_insert_id(#\n\r)ass";
			NUnit.Framework.Assert.AreEqual(ServerParseSelect.LastInsertId, ServerParseSelect
				.Parse(stmt, 6));
			stmt = "select last_insert_id(#\n\r)as 'a'";
			NUnit.Framework.Assert.AreEqual(ServerParseSelect.LastInsertId, ServerParseSelect
				.Parse(stmt, 6));
			stmt = "select last_insert_id(#\n\r)as 'a\\''";
			NUnit.Framework.Assert.AreEqual(ServerParseSelect.LastInsertId, ServerParseSelect
				.Parse(stmt, 6));
			stmt = "select last_insert_id(#\n\r)as 'a'''";
			NUnit.Framework.Assert.AreEqual(ServerParseSelect.LastInsertId, ServerParseSelect
				.Parse(stmt, 6));
			stmt = "select last_insert_id(#\n\r)as 'a\"'";
			NUnit.Framework.Assert.AreEqual(ServerParseSelect.LastInsertId, ServerParseSelect
				.Parse(stmt, 6));
			stmt = "   select last_insert_id(#\n\r) As 'a\"'";
			NUnit.Framework.Assert.AreEqual(ServerParseSelect.LastInsertId, ServerParseSelect
				.Parse(stmt, 9));
			stmt = "select last_insert_id(#\n\r)as 'a\"\\'";
			NUnit.Framework.Assert.AreEqual(ServerParseSelect.Other, ServerParseSelect.Parse(
				stmt, 6));
			stmt = "select last_insert_id(#\n\r)as `77``7` ,";
			NUnit.Framework.Assert.AreEqual(ServerParseSelect.Other, ServerParseSelect.Parse(
				stmt, 6));
			stmt = "select last_insert_id(#\n\r)as `77`7`";
			NUnit.Framework.Assert.AreEqual(ServerParseSelect.Other, ServerParseSelect.Parse(
				stmt, 6));
			stmt = "select last_insert_id(#\n\r) as,";
			NUnit.Framework.Assert.AreEqual(ServerParseSelect.Other, ServerParseSelect.Parse(
				stmt, 6));
			stmt = "select last_insert_id(#\n\r) ass a";
			NUnit.Framework.Assert.AreEqual(ServerParseSelect.Other, ServerParseSelect.Parse(
				stmt, 6));
			stmt = "select last_insert_id(#\n\r) as 'a";
			NUnit.Framework.Assert.AreEqual(ServerParseSelect.Other, ServerParseSelect.Parse(
				stmt, 6));
		}
	}
}
