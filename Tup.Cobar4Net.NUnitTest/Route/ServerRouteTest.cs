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
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Sharpen;
using Tup.Cobar4Net.Config.Loader;
using Tup.Cobar4Net.Config.Model;
using Tup.Cobar4Net.Config.Util;
using Tup.Cobar4Net.Parser.Ast.Expression.Primary;
using Tup.Cobar4Net.Parser.Ast.Stmt;
using Tup.Cobar4Net.Parser.Ast.Stmt.Dml;
using Tup.Cobar4Net.Parser.Recognizer;
using Tup.Cobar4Net.Route.Config;
using Tup.Cobar4Net.Route.Util;

namespace Tup.Cobar4Net.Route
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    [NUnit.Framework.TestFixture(Category = "ServerRouteTest")]
    public class ServerRouteTest : AbstractAliasConvert
    {
        //TODO ServerRouteTest schemaMap
        protected internal IDictionary<string, SchemaConfig> schemaMap = null;

        //public ServerRouteTest()
        //{
        //    string schemaFile = "/route/schema.xml";
        //    string ruleFile = "/route/rule.xml";
        //    SchemaLoader schemaLoader = new XMLSchemaLoader(schemaFile, ruleFile);
        //    try
        //    {
        //        RouteRuleInitializer.InitRouteRule(schemaLoader);
        //    }
        //    catch (SQLSyntaxErrorException e)
        //    {
        //        throw new ConfigException(e);
        //    }
        //    catch (RuntimeException ee)
        //    {
        //        throw;
        //    }
        //    schemaMap = schemaLoader.GetSchemas();
        //}

        ///// <exception cref="System.Exception"/>
        //protected override void SetUp()
        //{
        //}

        // super.setUp();
        // schemaMap = CobarServer.getInstance().getConfig().getSchemas();
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestRouteInsertShort()
        {
            string sql = "inSErt into offer_detail (`offer_id`, gmt) values (123,now())";
            SchemaConfig schema = schemaMap["cndb"];
            RouteResultset rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(1, rrs.GetNodes().Length);
            NUnit.Framework.Assert.AreEqual(-1l, rrs.GetLimitSize());
            NUnit.Framework.Assert.AreEqual((int)RouteResultsetNode.DefaultReplicaIndex, rrs.
                GetNodes()[0].GetReplicaIndex());
            NUnit.Framework.Assert.AreEqual("detail_dn[15]", rrs.GetNodes()[0].GetName());
            NUnit.Framework.Assert.AreEqual("inSErt into offer_detail (`offer_id`, gmt) values (123,now())"
                , rrs.GetNodes()[0].GetStatement());
            sql = "inSErt into offer_detail ( gmt) values (now())";
            schema = schemaMap["cndb"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(128, rrs.GetNodes().Length);
            sql = "inSErt into offer_detail (offer_id, gmt) values (123,now())";
            schema = schemaMap["cndb"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(1, rrs.GetNodes().Length);
            NUnit.Framework.Assert.AreEqual(-1l, rrs.GetLimitSize());
            NUnit.Framework.Assert.AreEqual((int)RouteResultsetNode.DefaultReplicaIndex, rrs.
                GetNodes()[0].GetReplicaIndex());
            NUnit.Framework.Assert.AreEqual("detail_dn[15]", rrs.GetNodes()[0].GetName());
            NUnit.Framework.Assert.AreEqual("inSErt into offer_detail (offer_id, gmt) values (123,now())"
                , rrs.GetNodes()[0].GetStatement());
            sql = "insert into offer(group_id,offer_id,member_id)values(234,123,'abc')";
            schema = schemaMap["cndb"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(1, rrs.GetNodes().Length);
            NUnit.Framework.Assert.AreEqual(-1l, rrs.GetLimitSize());
            NUnit.Framework.Assert.AreEqual((int)RouteResultsetNode.DefaultReplicaIndex, rrs.
                GetNodes()[0].GetReplicaIndex());
            NUnit.Framework.Assert.AreEqual("offer_dn[12]", rrs.GetNodes()[0].GetName());
            NUnit.Framework.Assert.AreEqual("insert into offer(group_id,offer_id,member_id)values(234,123,'abc')"
                , rrs.GetNodes()[0].GetStatement());
            sql = "insert into offer (group_id, offer_id, gmt) values (234,123,now())";
            schema = schemaMap["cndb"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(1, rrs.GetNodes().Length);
            NUnit.Framework.Assert.AreEqual(-1l, rrs.GetLimitSize());
            NUnit.Framework.Assert.AreEqual((int)RouteResultsetNode.DefaultReplicaIndex, rrs.
                GetNodes()[0].GetReplicaIndex());
            NUnit.Framework.Assert.AreEqual("offer_dn[44]", rrs.GetNodes()[0].GetName());
            NUnit.Framework.Assert.AreEqual("insert into offer (group_id, offer_id, gmt) values (234,123,now())"
                , rrs.GetNodes()[0].GetStatement());
            sql = "insert into offer (offer_id, group_id, gmt) values (123,234,now())";
            schema = schemaMap["cndb"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(1, rrs.GetNodes().Length);
            NUnit.Framework.Assert.AreEqual(-1l, rrs.GetLimitSize());
            NUnit.Framework.Assert.AreEqual((int)RouteResultsetNode.DefaultReplicaIndex, rrs.
                GetNodes()[0].GetReplicaIndex());
            NUnit.Framework.Assert.AreEqual("offer_dn[44]", rrs.GetNodes()[0].GetName());
            NUnit.Framework.Assert.AreEqual("insert into offer (offer_id, group_id, gmt) values (123,234,now())"
                , rrs.GetNodes()[0].GetStatement());
            sql = "insert into offer (offer_id, group_id, gmt) values (234,123,now())";
            schema = schemaMap["cndb"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(1, rrs.GetNodes().Length);
            NUnit.Framework.Assert.AreEqual(-1l, rrs.GetLimitSize());
            NUnit.Framework.Assert.AreEqual((int)RouteResultsetNode.DefaultReplicaIndex, rrs.
                GetNodes()[0].GetReplicaIndex());
            NUnit.Framework.Assert.AreEqual("offer_dn[44]", rrs.GetNodes()[0].GetName());
            NUnit.Framework.Assert.AreEqual("insert into offer (offer_id, group_id, gmt) values (234,123,now())"
                , rrs.GetNodes()[0].GetStatement());
            sql = "insert into wp_image (member_id,gmt) values ('pavarotti17',now())";
            schema = schemaMap["cndb"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(1, rrs.GetNodes().Length);
            NUnit.Framework.Assert.AreEqual(-1l, rrs.GetLimitSize());
            NUnit.Framework.Assert.AreEqual((int)RouteResultsetNode.DefaultReplicaIndex, rrs.
                GetNodes()[0].GetReplicaIndex());
            NUnit.Framework.Assert.AreEqual("offer_dn[123]", rrs.GetNodes()[0].GetName());
            NUnit.Framework.Assert.AreEqual("insert into wp_image (member_id,gmt) values ('pavarotti17',now())"
                , rrs.GetNodes()[0].GetStatement());
            sql = "insert low_priority into offer set offer_id=123,  group_id=234,gmt=now() on duplicate key update `dual`=1";
            schema = schemaMap["cndb"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(1, rrs.GetNodes().Length);
            NUnit.Framework.Assert.AreEqual(-1l, rrs.GetLimitSize());
            NUnit.Framework.Assert.AreEqual((int)RouteResultsetNode.DefaultReplicaIndex, rrs.
                GetNodes()[0].GetReplicaIndex());
            NUnit.Framework.Assert.AreEqual("offer_dn[44]", rrs.GetNodes()[0].GetName());
            NUnit.Framework.Assert.AreEqual("insert low_priority into offer set offer_id=123,  group_id=234,gmt=now() on duplicate key update `dual`=1"
                , rrs.GetNodes()[0].GetStatement());
            sql = "update ignore wp_image set name='abc',gmt=now()where `select`='abc'";
            schema = schemaMap["cndb"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(1, rrs.GetNodes().Length);
            NUnit.Framework.Assert.AreEqual(-1l, rrs.GetLimitSize());
            NUnit.Framework.Assert.AreEqual((int)RouteResultsetNode.DefaultReplicaIndex, rrs.
                GetNodes()[0].GetReplicaIndex());
            NUnit.Framework.Assert.AreEqual("offer_dn[12]", rrs.GetNodes()[0].GetName());
            NUnit.Framework.Assert.AreEqual("update ignore wp_image set name='abc',gmt=now()where `select`='abc'"
                , rrs.GetNodes()[0].GetStatement());
            sql = "delete from offer.*,wp_image.* using offer a,wp_image b where a.member_id=b.member_id and a.member_id='abc' ";
            schema = schemaMap["cndb"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(1, rrs.GetNodes().Length);
            NUnit.Framework.Assert.AreEqual(-1l, rrs.GetLimitSize());
            NUnit.Framework.Assert.AreEqual((int)RouteResultsetNode.DefaultReplicaIndex, rrs.
                GetNodes()[0].GetReplicaIndex());
            NUnit.Framework.Assert.AreEqual("offer_dn[12]", rrs.GetNodes()[0].GetName());
            NUnit.Framework.Assert.AreEqual("delete from offer.*,wp_image.* using offer a,wp_image b where a.member_id=b.member_id and a.member_id='abc' "
                , rrs.GetNodes()[0].GetStatement());
        }

        private static IDictionary<string, RouteResultsetNode> GetNodeMap(RouteResultset
            rrs, int expectSize)
        {
            RouteResultsetNode[] routeNodes = rrs.GetNodes();
            NUnit.Framework.Assert.AreEqual(expectSize, routeNodes.Length);
            var nodeMap = new Dictionary<string, RouteResultsetNode>(expectSize);
            for (int i = 0; i < expectSize; i++)
            {
                RouteResultsetNode routeNode = routeNodes[i];
                nodeMap[routeNode.GetName()] = routeNode;
            }
            NUnit.Framework.Assert.AreEqual(expectSize, nodeMap.Count);
            return nodeMap;
        }

        private interface NodeNameDeconstructor
        {
            int GetNodeIndex(string name);
        }

        private class NodeNameAsserter : ServerRouteTest.NodeNameDeconstructor
        {
            private string[] expectNames;

            public NodeNameAsserter()
            {
            }

            public NodeNameAsserter(params string[] expectNames)
            {
                NUnit.Framework.Assert.IsNotNull(expectNames);
                this.expectNames = expectNames;
            }

            protected internal virtual void SetNames(string[] expectNames)
            {
                NUnit.Framework.Assert.IsNotNull(expectNames);
                this.expectNames = expectNames;
            }

            public virtual void AssertRouteNodeNames(ICollection<string> nodeNames)
            {
                NUnit.Framework.Assert.IsNotNull(nodeNames);
                NUnit.Framework.Assert.AreEqual(expectNames.Length, nodeNames.Count);
                foreach (string name in expectNames)
                {
                    NUnit.Framework.Assert.IsTrue(nodeNames.Contains(name));
                }
            }

            public virtual int GetNodeIndex(string name)
            {
                for (int i = 0; i < expectNames.Length; ++i)
                {
                    if (name.Equals(expectNames[i]))
                    {
                        return i;
                    }
                }
                throw new NotSupportedException("route node " + name + " dosn't exist!");
            }
        }

        private class IndexedNodeNameAsserter : ServerRouteTest.NodeNameAsserter
        {
            /// <param name="from">included</param>
            /// <param name="to">excluded</param>
            public IndexedNodeNameAsserter(string prefix, int from, int to)
                : base()
            {
                string[] names = new string[to - from];
                for (int i = 0; i < names.Length; ++i)
                {
                    names[i] = prefix + "[" + (i + from) + "]";
                }
                SetNames(names);
            }
        }

        private interface ReplicaAsserter
        {
            void AssertReplica(int nodeIndex, int replica);
        }

        private class RouteNodeAsserter
        {
            private ServerRouteTest.NodeNameDeconstructor deconstructor;

            private ServerRouteTest.SQLAsserter sqlAsserter;

            private ServerRouteTest.ReplicaAsserter replicaAsserter;

            public RouteNodeAsserter(ServerRouteTest.NodeNameDeconstructor deconstructor, ServerRouteTest.SQLAsserter
                 sqlAsserter)
            {
                this.deconstructor = deconstructor;
                this.sqlAsserter = sqlAsserter;
                this.replicaAsserter = new _ReplicaAsserter_266();
            }

            private sealed class _ReplicaAsserter_266 : ServerRouteTest.ReplicaAsserter
            {
                public _ReplicaAsserter_266()
                {
                }

                public void AssertReplica(int nodeIndex, int replica)
                {
                    NUnit.Framework.Assert.AreEqual(RouteResultsetNode.DefaultReplicaIndex, replica);
                }
            }

            public RouteNodeAsserter(ServerRouteTest.NodeNameDeconstructor deconstructor, ServerRouteTest.SQLAsserter
                 sqlAsserter, ServerRouteTest.ReplicaAsserter replicaAsserter)
            {
                this.deconstructor = deconstructor;
                this.sqlAsserter = sqlAsserter;
                this.replicaAsserter = replicaAsserter;
            }

            /// <exception cref="System.Exception"/>
            public virtual void AssertNode(RouteResultsetNode node)
            {
                int nodeIndex = deconstructor.GetNodeIndex(node.GetName());
                sqlAsserter.AssertSQL(node.GetStatement(), nodeIndex);
                replicaAsserter.AssertReplica(nodeIndex, node.GetReplicaIndex());
            }
        }

        private interface SQLAsserter
        {
            /// <exception cref="System.Exception"/>
            void AssertSQL(string sql, int nodeIndex);
        }

        private class SimpleSQLAsserter : ServerRouteTest.SQLAsserter
        {
            private IDictionary<int, ICollection<string>> map = new Dictionary<int, ICollection
                <string>>();

            public virtual ServerRouteTest.SimpleSQLAsserter AddExpectSQL(int nodeIndex, string
                 sql)
            {
                ICollection<string> set = map[nodeIndex];
                if (set == null)
                {
                    set = new HashSet<string>();
                    map[nodeIndex] = set;
                }
                set.Add(sql);
                return this;
            }

            public virtual ServerRouteTest.SimpleSQLAsserter AddExpectSQL(int nodeIndex, params
                string[] sql)
            {
                foreach (string s in sql)
                {
                    AddExpectSQL(nodeIndex, s);
                }
                return this;
            }

            public virtual ServerRouteTest.SimpleSQLAsserter AddExpectSQL(int nodeIndex, string
                 prefix, PermutationUtil.PermutationGenerator pg, string suffix)
            {
                ICollection<string> ss = pg.PermutateSQL();
                foreach (string s in ss)
                {
                    AddExpectSQL(nodeIndex, prefix + s + suffix);
                }
                return this;
            }

            /// <exception cref="System.Exception"/>
            public virtual void AssertSQL(string sql, int nodeIndex)
            {
                NUnit.Framework.Assert.IsNotNull(map[nodeIndex]);
                NUnit.Framework.Assert.IsTrue(map[nodeIndex].Contains(sql));
            }
        }

        private abstract class ParseredSQLAsserter : ServerRouteTest.SQLAsserter
        {
            /// <exception cref="System.Exception"/>
            public virtual void AssertSQL(string sql, int nodeIndex)
            {
                SQLStatement stmt = SQLParserDelegate.Parse(sql);
                AssertAST(stmt, nodeIndex);
            }

            protected internal abstract void AssertAST(SQLStatement stmt, int nodeIndex);
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestRouteInsertLong()
        {
            StringBuilder sb = new StringBuilder("insert into offer_detail (offer_id, gmt) values "
                );
            for (int i = 0; i < 1024; ++i)
            {
                if (i > 0)
                {
                    sb.Append(", ");
                }
                sb.Append("(" + i + ", now())");
            }
            SchemaConfig schema = schemaMap["cndb"];
            RouteResultset rrs = ServerRouter.Route(schema, sb.ToString(), null, null);
            NUnit.Framework.Assert.AreEqual(-1l, rrs.GetLimitSize());
            IDictionary<string, RouteResultsetNode> nodeMap = GetNodeMap(rrs, 128);
            ServerRouteTest.IndexedNodeNameAsserter nameAsserter = new ServerRouteTest.IndexedNodeNameAsserter
                ("detail_dn", 0, 128);
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            ServerRouteTest.RouteNodeAsserter asserter = new ServerRouteTest.RouteNodeAsserter
                (nameAsserter, new _ParseredSQLAsserter_351());
            foreach (RouteResultsetNode node in nodeMap.Values)
            {
                asserter.AssertNode(node);
            }
        }

        private sealed class _ParseredSQLAsserter_351 : ServerRouteTest.ParseredSQLAsserter
        {
            public _ParseredSQLAsserter_351()
            {
            }

            protected internal override void AssertAST(SQLStatement stmt, int nodeIndex)
            {
                DMLInsertStatement insert = (DMLInsertStatement)stmt;
                IList<RowExpression> rows = insert.GetRowList();
                NUnit.Framework.Assert.IsNotNull(rows);
                NUnit.Framework.Assert.AreEqual(8, rows.Count);
                IList<int> vals = new List<int>(8);
                foreach (RowExpression row in rows)
                {
                    int val = (int)((Number)row.GetRowExprList()[0].Evaluation(null));
                    vals.Add(val);
                }
                NUnit.Framework.Assert.AreEqual(8, vals.Count);
                for (int i = 8 * nodeIndex; i < 8 * nodeIndex + 8; ++i)
                {
                    NUnit.Framework.Assert.IsTrue(vals.Contains(i));
                }
            }
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestRoute()
        {
            string sql = "select * from offer.wp_image where member_id='pavarotti17' or member_id='1qq'";
            SchemaConfig schema = schemaMap["cndb"];
            RouteResultset rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(-1l, rrs.GetLimitSize());
            NUnit.Framework.Assert.AreEqual((int)RouteResultsetNode.DefaultReplicaIndex, rrs.
                GetNodes()[0].GetReplicaIndex());
            IDictionary<string, RouteResultsetNode> nodeMap = GetNodeMap(rrs, 2);
            ServerRouteTest.NodeNameAsserter nameAsserter = new ServerRouteTest.NodeNameAsserter
                ("offer_dn[123]", "offer_dn[66]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            ServerRouteTest.SimpleSQLAsserter sqlAsserter = new ServerRouteTest.SimpleSQLAsserter
                ();
            sqlAsserter.AddExpectSQL(0, "SELECT * FROM wp_image WHERE member_id = 'pavarotti17' OR FALSE"
                ).AddExpectSQL(1, "SELECT * FROM wp_image WHERE FALSE OR member_id = '1qq'");
            ServerRouteTest.RouteNodeAsserter asserter = new ServerRouteTest.RouteNodeAsserter
                (nameAsserter, sqlAsserter);
            foreach (RouteResultsetNode node in nodeMap.Values)
            {
                asserter.AssertNode(node);
            }
            sql = "select * from independent where member='abc'";
            schema = schemaMap["cndb"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            nodeMap = GetNodeMap(rrs, 128);
            nameAsserter = new ServerRouteTest.IndexedNodeNameAsserter("independent_dn", 0, 128
                );
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new ServerRouteTest.SimpleSQLAsserter();
            for (int i = 0; i < 128; ++i)
            {
                sqlAsserter.AddExpectSQL(i, "select * from independent where member='abc'");
            }
            asserter = new ServerRouteTest.RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (RouteResultsetNode node_1 in nodeMap.Values)
            {
                asserter.AssertNode(node_1);
            }
            sql = "select * from independent A where cndb.a.member='abc'";
            schema = schemaMap["cndb"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            nodeMap = GetNodeMap(rrs, 128);
            nameAsserter = new ServerRouteTest.IndexedNodeNameAsserter("independent_dn", 0, 128
                );
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new ServerRouteTest.SimpleSQLAsserter();
            for (int i_1 = 0; i_1 < 128; ++i_1)
            {
                sqlAsserter.AddExpectSQL(i_1, "SELECT * FROM independent AS A WHERE a.member = 'abc'"
                    );
            }
            asserter = new ServerRouteTest.RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (RouteResultsetNode node_2 in nodeMap.Values)
            {
                asserter.AssertNode(node_2);
            }
            sql = "select * from tb where member='abc'";
            schema = schemaMap["cndb"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(1, rrs.GetNodes().Length);
            NUnit.Framework.Assert.AreEqual(-1l, rrs.GetLimitSize());
            NUnit.Framework.Assert.AreEqual((int)RouteResultsetNode.DefaultReplicaIndex, rrs.
                GetNodes()[0].GetReplicaIndex());
            NUnit.Framework.Assert.AreEqual("cndb_dn", rrs.GetNodes()[0].GetName());
            NUnit.Framework.Assert.AreEqual("select * from tb where member='abc'", rrs.GetNodes
                ()[0].GetStatement());
            sql = "select * from offer.wp_image where member_id is null";
            schema = schemaMap["cndb"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(1, rrs.GetNodes().Length);
            NUnit.Framework.Assert.AreEqual(-1l, rrs.GetLimitSize());
            NUnit.Framework.Assert.AreEqual((int)RouteResultsetNode.DefaultReplicaIndex, rrs.
                GetNodes()[0].GetReplicaIndex());
            NUnit.Framework.Assert.AreEqual("offer_dn[48]", rrs.GetNodes()[0].GetName());
            NUnit.Framework.Assert.AreEqual("SELECT * FROM wp_image WHERE member_id IS NULL",
                rrs.GetNodes()[0].GetStatement());
            sql = "select * from offer.wp_image where member_id between 'pavarotti17' and 'pavarotti17'";
            schema = schemaMap["cndb"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(1, rrs.GetNodes().Length);
            NUnit.Framework.Assert.AreEqual(-1l, rrs.GetLimitSize());
            NUnit.Framework.Assert.AreEqual((int)RouteResultsetNode.DefaultReplicaIndex, rrs.
                GetNodes()[0].GetReplicaIndex());
            NUnit.Framework.Assert.AreEqual("offer_dn[123]", rrs.GetNodes()[0].GetName());
            NUnit.Framework.Assert.AreEqual("SELECT * FROM wp_image WHERE member_id BETWEEN 'pavarotti17' AND 'pavarotti17'"
                , rrs.GetNodes()[0].GetStatement());
            sql = "select * from  offer A where a.member_id='abc' union select * from product_visit b where B.offer_id =123";
            schema = schemaMap["cndb"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(128, rrs.GetNodes().Length);
            NUnit.Framework.Assert.AreEqual(-1l, rrs.GetLimitSize());
            for (int i_2 = 0; i_2 < 128; i_2++)
            {
                NUnit.Framework.Assert.AreEqual((int)RouteResultsetNode.DefaultReplicaIndex, rrs.
                    GetNodes()[i_2].GetReplicaIndex());
                NUnit.Framework.Assert.AreEqual("offer_dn[" + i_2 + "]", rrs.GetNodes()[i_2].GetName
                    ());
                NUnit.Framework.Assert.AreEqual("select * from  offer A where a.member_id='abc' union select * from product_visit b where B.offer_id =123"
                    , rrs.GetNodes()[i_2].GetStatement());
            }
            sql = "update offer.offer a join offer_detail b set id=123 where a.offer_id=b.offer_id and a.offer_id=123 and group_id=234";
            schema = schemaMap["cndb"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(1, rrs.GetNodes().Length);
            NUnit.Framework.Assert.AreEqual(-1l, rrs.GetLimitSize());
            NUnit.Framework.Assert.AreEqual((int)RouteResultsetNode.DefaultReplicaIndex, rrs.
                GetNodes()[0].GetReplicaIndex());
            NUnit.Framework.Assert.AreEqual("offer_dn[44]", rrs.GetNodes()[0].GetName());
            NUnit.Framework.Assert.AreEqual("UPDATE offer AS " + AliasConvert("a") + " INNER JOIN offer_detail AS "
                 + AliasConvert("b") + " SET id = 123 WHERE a.offer_id = b.offer_id AND a.offer_id = 123 AND group_id = 234"
                , rrs.GetNodes()[0].GetStatement());
            sql = "update    offer./*kjh*/offer a join offer_detail B set id:=123 where A.offer_id=b.offer_id and b.offer_id=123 and group_id=234";
            schema = schemaMap["cndb"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(1, rrs.GetNodes().Length);
            NUnit.Framework.Assert.AreEqual(-1l, rrs.GetLimitSize());
            NUnit.Framework.Assert.AreEqual((int)RouteResultsetNode.DefaultReplicaIndex, rrs.
                GetNodes()[0].GetReplicaIndex());
            NUnit.Framework.Assert.AreEqual("detail_dn[15]", rrs.GetNodes()[0].GetName());
            NUnit.Framework.Assert.AreEqual("UPDATE offer AS " + AliasConvert("a") + " INNER JOIN offer_detail AS "
                 + AliasConvert("b") + " SET id = 123 WHERE A.offer_id = b.offer_id AND b.offer_id = 123 AND group_id = 234"
                , rrs.GetNodes()[0].GetStatement());
            sql = "select * from offer.wp_image where member_id in ('pavarotti17', 'qaa') or offer.wp_image.member_id='1qq'";
            schema = schemaMap["cndb"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(-1l, rrs.GetLimitSize());
            nodeMap = GetNodeMap(rrs, 3);
            nameAsserter = new ServerRouteTest.NodeNameAsserter("offer_dn[123]", "offer_dn[10]"
                , "offer_dn[66]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new ServerRouteTest.SimpleSQLAsserter();
            sqlAsserter.AddExpectSQL(0, "SELECT * FROM wp_image WHERE member_id IN ('pavarotti17') OR FALSE"
                ).AddExpectSQL(1, "SELECT * FROM wp_image WHERE member_id IN ('qaa') OR FALSE").
                AddExpectSQL(2, "SELECT * FROM wp_image WHERE FALSE OR wp_image.member_id = '1qq'"
                );
            asserter = new ServerRouteTest.RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (RouteResultsetNode node_3 in nodeMap.Values)
            {
                asserter.AssertNode(node_3);
            }
            sql = "select * from offer.wp_image,tb2 as t2 where member_id in ('pavarotti17', 'qaa') or offer.wp_image.member_id='1qq'";
            schema = schemaMap["cndb"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(3, rrs.GetNodes().Length);
            NUnit.Framework.Assert.AreEqual(-1l, rrs.GetLimitSize());
            nodeMap = GetNodeMap(rrs, 3);
            nameAsserter = new ServerRouteTest.NodeNameAsserter("offer_dn[123]", "offer_dn[10]"
                , "offer_dn[66]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new ServerRouteTest.SimpleSQLAsserter();
            sqlAsserter.AddExpectSQL(0, "SELECT * FROM wp_image, tb2 AS " + AliasConvert("t2"
                ) + " WHERE member_id IN ('pavarotti17') OR FALSE").AddExpectSQL(1, "SELECT * FROM wp_image, tb2 AS "
                 + AliasConvert("t2") + " WHERE member_id IN ('qaa') OR FALSE").AddExpectSQL(2,
                "SELECT * FROM wp_image, tb2 AS " + AliasConvert("t2") + " WHERE FALSE OR wp_image.member_id = '1qq'"
                );
            asserter = new ServerRouteTest.RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (RouteResultsetNode node_4 in nodeMap.Values)
            {
                asserter.AssertNode(node_4);
            }
            sql = "select * from offer.wp_image,tb2 as t2 where member_id in ('pavarotti17', 'sf', 's22f', 'sdddf', 'sd') ";
            schema = schemaMap["cndb"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(-1l, rrs.GetLimitSize());
            nodeMap = GetNodeMap(rrs, 4);
            nameAsserter = new ServerRouteTest.NodeNameAsserter("offer_dn[123]", "offer_dn[126]"
                , "offer_dn[74]", "offer_dn[26]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new ServerRouteTest.SimpleSQLAsserter();
            sqlAsserter.AddExpectSQL(0, "SELECT * FROM wp_image, tb2 AS " + AliasConvert("t2"
                ) + " WHERE member_id IN ('pavarotti17')").AddExpectSQL(1, "SELECT * FROM wp_image, tb2 AS "
                 + AliasConvert("t2") + " WHERE member_id IN ('sdddf')").AddExpectSQL(2, "SELECT * FROM wp_image, tb2 AS "
                 + AliasConvert("t2") + " WHERE member_id IN ('sf', 'sd')", "SELECT * FROM wp_image, tb2 AS "
                 + AliasConvert("t2") + " WHERE member_id IN ('sd', 'sf')").AddExpectSQL(3, "SELECT * FROM wp_image, tb2 AS "
                 + AliasConvert("t2") + " WHERE member_id IN ('s22f')");
            asserter = new ServerRouteTest.RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (RouteResultsetNode node_5 in nodeMap.Values)
            {
                asserter.AssertNode(node_5);
            }
            sql = "select * from tb2 as t2 ,offer.wp_image where member_id in ('pavarotti17', 'qaa') or offer.wp_image.member_id='1qq'";
            schema = schemaMap["cndb"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(-1l, rrs.GetLimitSize());
            nodeMap = GetNodeMap(rrs, 3);
            nameAsserter = new ServerRouteTest.NodeNameAsserter("offer_dn[123]", "offer_dn[10]"
                , "offer_dn[66]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new ServerRouteTest.SimpleSQLAsserter();
            sqlAsserter.AddExpectSQL(0, "SELECT * FROM tb2 AS " + AliasConvert("t2") + ", wp_image WHERE member_id IN ('pavarotti17') OR FALSE"
                ).AddExpectSQL(1, "SELECT * FROM tb2 AS " + AliasConvert("t2") + ", wp_image WHERE member_id IN ('qaa') OR FALSE"
                ).AddExpectSQL(2, "SELECT * FROM tb2 AS " + AliasConvert("t2") + ", wp_image WHERE FALSE OR wp_image.member_id = '1qq'"
                );
            asserter = new ServerRouteTest.RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (RouteResultsetNode node_6 in nodeMap.Values)
            {
                asserter.AssertNode(node_6);
            }
            sql = "select * from tb2 as t2 ,offer.wp_image where member_id in ('pavarotti17', 'qaa') or offer.wp_image.member_id='1qq' and t2.member_id='123'";
            schema = schemaMap["cndb"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(-1l, rrs.GetLimitSize());
            nodeMap = GetNodeMap(rrs, 3);
            nameAsserter = new ServerRouteTest.NodeNameAsserter("offer_dn[123]", "offer_dn[10]"
                , "offer_dn[66]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new ServerRouteTest.SimpleSQLAsserter();
            sqlAsserter.AddExpectSQL(0, "SELECT * FROM tb2 AS " + AliasConvert("t2") + ", wp_image WHERE member_id IN ('pavarotti17') OR FALSE AND t2.member_id = '123'"
                ).AddExpectSQL(1, "SELECT * FROM tb2 AS " + AliasConvert("t2") + ", wp_image WHERE member_id IN ('qaa') OR FALSE AND t2.member_id = '123'"
                ).AddExpectSQL(2, "SELECT * FROM tb2 AS " + AliasConvert("t2") + ", wp_image WHERE FALSE OR wp_image.member_id = '1qq' AND t2.member_id = '123'"
                );
            asserter = new ServerRouteTest.RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (RouteResultsetNode node_7 in nodeMap.Values)
            {
                asserter.AssertNode(node_7);
            }
            sql = "select * from wp_image wB inner join offer.offer o on wB.member_id=O.member_ID where wB.member_iD='pavarotti17' and o.id=3";
            schema = schemaMap["cndb"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(-1l, rrs.GetLimitSize());
            NUnit.Framework.Assert.AreEqual(1, rrs.GetNodes().Length);
            NUnit.Framework.Assert.AreEqual((int)RouteResultsetNode.DefaultReplicaIndex, rrs.
                GetNodes()[0].GetReplicaIndex());
            NUnit.Framework.Assert.AreEqual("offer_dn[123]", rrs.GetNodes()[0].GetName());
            NUnit.Framework.Assert.AreEqual("SELECT * FROM wp_image AS " + AliasConvert("wB")
                 + " INNER JOIN offer AS " + AliasConvert("o") + " ON wB.member_id = O.member_ID WHERE wB.member_iD = 'pavarotti17' AND o.id = 3"
                , rrs.GetNodes()[0].GetStatement());
            sql = "select * from wp_image w inner join offer o on w.member_id=O.member_ID where w.member_iD in ('pavarotti17','13') and o.id=3";
            schema = schemaMap["cndb"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(-1l, rrs.GetLimitSize());
            nodeMap = GetNodeMap(rrs, 2);
            nameAsserter = new ServerRouteTest.NodeNameAsserter("offer_dn[123]", "offer_dn[68]"
                );
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new ServerRouteTest.SimpleSQLAsserter();
            sqlAsserter.AddExpectSQL(0, "SELECT * FROM wp_image AS " + AliasConvert("w") + " INNER JOIN offer AS "
                 + AliasConvert("o") + " ON w.member_id = O.member_ID WHERE w.member_iD IN ('pavarotti17') AND o.id = 3"
                ).AddExpectSQL(1, "SELECT * FROM wp_image AS " + AliasConvert("w") + " INNER JOIN offer AS "
                 + AliasConvert("o") + " ON w.member_id = O.member_ID WHERE w.member_iD IN ('13') AND o.id = 3"
                );
            asserter = new ServerRouteTest.RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (RouteResultsetNode node_8 in nodeMap.Values)
            {
                asserter.AssertNode(node_8);
            }
            sql = "insert into wp_image (member_id,gmt) values ('pavarotti17',now()),('123',now())";
            schema = schemaMap["cndb"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(-1l, rrs.GetLimitSize());
            nodeMap = GetNodeMap(rrs, 2);
            nameAsserter = new ServerRouteTest.NodeNameAsserter("offer_dn[123]", "offer_dn[70]"
                );
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new ServerRouteTest.SimpleSQLAsserter();
            sqlAsserter.AddExpectSQL(0, "INSERT INTO wp_image (member_id, gmt) VALUES ('pavarotti17', NOW())"
                ).AddExpectSQL(1, "INSERT INTO wp_image (member_id, gmt) VALUES ('123', NOW())");
            asserter = new ServerRouteTest.RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (RouteResultsetNode node_9 in nodeMap.Values)
            {
                asserter.AssertNode(node_9);
            }
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestDuplicatePartitionKey()
        {
            string sql = "select * from offer.wp_image where member_id in ('pavarotti17', 'qaa') or offer.wp_image.member_id='1qq' or member_id='1qq'";
            SchemaConfig schema = schemaMap["cndb"];
            RouteResultset rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(-1l, rrs.GetLimitSize());
            IDictionary<string, RouteResultsetNode> nodeMap = GetNodeMap(rrs, 3);
            ServerRouteTest.NodeNameAsserter nameAsserter = new ServerRouteTest.NodeNameAsserter
                ("offer_dn[123]", "offer_dn[10]", "offer_dn[66]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            ServerRouteTest.SimpleSQLAsserter sqlAsserter = new ServerRouteTest.SimpleSQLAsserter
                ();
            sqlAsserter.AddExpectSQL(0, "SELECT * FROM wp_image WHERE member_id IN ('pavarotti17') OR FALSE OR FALSE"
                ).AddExpectSQL(1, "SELECT * FROM wp_image WHERE member_id IN ('qaa') OR FALSE OR FALSE"
                ).AddExpectSQL(2, "SELECT * FROM wp_image WHERE FALSE OR wp_image.member_id = '1qq' OR member_id = '1qq'"
                );
            ServerRouteTest.RouteNodeAsserter asserter = new ServerRouteTest.RouteNodeAsserter
                (nameAsserter, sqlAsserter);
            foreach (RouteResultsetNode node in nodeMap.Values)
            {
                asserter.AssertNode(node);
            }
            sql = "insert into wp_image (id, member_id, gmt) values (1,'pavarotti17',now()),(2,'pavarotti17',now()),(3,'qaa',now())";
            schema = schemaMap["cndb"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(-1l, rrs.GetLimitSize());
            nodeMap = GetNodeMap(rrs, 2);
            nameAsserter = new ServerRouteTest.NodeNameAsserter("offer_dn[123]", "offer_dn[10]"
                );
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new ServerRouteTest.SimpleSQLAsserter();
            sqlAsserter.AddExpectSQL(0, "INSERT INTO wp_image (id, member_id, gmt) VALUES (2, 'pavarotti17', NOW()), (1, 'pavarotti17', NOW())"
                , "INSERT INTO wp_image (id, member_id, gmt) VALUES (1, 'pavarotti17', NOW()), (2, 'pavarotti17', NOW())"
                ).AddExpectSQL(1, "INSERT INTO wp_image (id, member_id, gmt) VALUES (3, 'qaa', NOW())"
                );
            asserter = new ServerRouteTest.RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (RouteResultsetNode node_1 in nodeMap.Values)
            {
                asserter.AssertNode(node_1);
            }
            sql = "select * from offer.wp_image where member_id in ('pavarotti17','pavarotti17', 'qaa') or offer.wp_image.member_id='pavarotti17'";
            schema = schemaMap["cndb"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(-1l, rrs.GetLimitSize());
            NUnit.Framework.Assert.AreEqual(2, rrs.GetNodes().Length);
            nodeMap = GetNodeMap(rrs, 2);
            nameAsserter = new ServerRouteTest.NodeNameAsserter("offer_dn[123]", "offer_dn[10]"
                );
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new ServerRouteTest.SimpleSQLAsserter();
            sqlAsserter.AddExpectSQL(0, "SELECT * FROM wp_image WHERE member_id IN ('pavarotti17', 'pavarotti17') OR wp_image.member_id = 'pavarotti17'"
                ).AddExpectSQL(1, "SELECT * FROM wp_image WHERE member_id IN ('qaa') OR FALSE");
            asserter = new ServerRouteTest.RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (RouteResultsetNode node_2 in nodeMap.Values)
            {
                asserter.AssertNode(node_2);
            }
            sql = "select * from offer.`wp_image` where `member_id` in ('pavarotti17','pavarotti17', 'qaa') or member_id in ('pavarotti17','1qq','pavarotti17') or offer.wp_image.member_id='pavarotti17'";
            schema = schemaMap["cndb"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(-1l, rrs.GetLimitSize());
            NUnit.Framework.Assert.AreEqual(3, rrs.GetNodes().Length);
            nodeMap = GetNodeMap(rrs, 3);
            nameAsserter = new ServerRouteTest.NodeNameAsserter("offer_dn[123]", "offer_dn[10]"
                , "offer_dn[66]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new ServerRouteTest.SimpleSQLAsserter();
            sqlAsserter.AddExpectSQL(0, "SELECT * FROM `wp_image` WHERE `member_id` IN ('pavarotti17', 'pavarotti17') OR member_id IN ('pavarotti17', 'pavarotti17') OR wp_image.member_id = 'pavarotti17'"
                ).AddExpectSQL(1, "SELECT * FROM `wp_image` WHERE `member_id` IN ('qaa') OR FALSE OR FALSE"
                ).AddExpectSQL(2, "SELECT * FROM `wp_image` WHERE FALSE OR member_id IN ('1qq') OR FALSE"
                );
            asserter = new ServerRouteTest.RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (RouteResultsetNode node_3 in nodeMap.Values)
            {
                asserter.AssertNode(node_3);
            }
            sql = "insert into offer_detail (offer_id, gmt) values (123,now()),(123,now()+1),(234,now()),(123,now()),(345,now()),(122+1,now()),(456,now())";
            schema = schemaMap["cndb"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(-1l, rrs.GetLimitSize());
            nodeMap = GetNodeMap(rrs, 4);
            nameAsserter = new ServerRouteTest.NodeNameAsserter("detail_dn[29]", "detail_dn[43]"
                , "detail_dn[57]", "detail_dn[15]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new ServerRouteTest.SimpleSQLAsserter();
            sqlAsserter.AddExpectSQL(0, "INSERT INTO offer_detail (offer_id, gmt) VALUES (234, NOW())"
                ).AddExpectSQL(1, "INSERT INTO offer_detail (offer_id, gmt) VALUES (345, NOW())"
                ).AddExpectSQL(2, "INSERT INTO offer_detail (offer_id, gmt) VALUES (456, NOW())"
                ).AddExpectSQL(3, "INSERT INTO offer_detail (offer_id, gmt) VALUES ", new PermutationUtil.PermutationGenerator
                ("(123, NOW())", "(123, NOW() + 1)", "(122 + 1, NOW())", "(123, NOW())").SetDelimiter
                (", "), string.Empty);
            asserter = new ServerRouteTest.RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (RouteResultsetNode node_4 in nodeMap.Values)
            {
                asserter.AssertNode(node_4);
            }
            sql = "insert into offer (offer_id, group_id, gmt) values " + "(123, 123, now()),(123, 234, now()),(123, 345, now()),(123, 456, now())"
                 + ",(234, 123, now()),(234, 234, now()),(234, 345, now()),(234, 456, now())" +
                ",(345, 123, now()),(345, 234, now()),(345, 345, now()),(345, 456, now())" + ",(456, 123, now()),(456, 234, now()),(456, 345, now()),(456, 456, now())";
            schema = schemaMap["cndb"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(-1l, rrs.GetLimitSize());
            nodeMap = GetNodeMap(rrs, 7);
            nameAsserter = new ServerRouteTest.NodeNameAsserter("offer_dn[58]", "offer_dn[100]"
                , "offer_dn[86]", "offer_dn[72]", "offer_dn[114]", "offer_dn[44]", "offer_dn[30]"
                );
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new ServerRouteTest.SimpleSQLAsserter();
            sqlAsserter.AddExpectSQL(0, "INSERT INTO offer (offer_id, group_id, gmt) VALUES "
                , new PermutationUtil.PermutationGenerator("(345, 123, NOW())", "(123, 345, NOW())"
                , "(234, 234, NOW())").SetDelimiter(", "), string.Empty).AddExpectSQL(1, "INSERT INTO offer (offer_id, group_id, gmt) VALUES "
                , new PermutationUtil.PermutationGenerator("(345, 456, NOW())", "(456, 345, NOW())"
                ).SetDelimiter(", "), string.Empty).AddExpectSQL(2, "INSERT INTO offer (offer_id, group_id, gmt) VALUES "
                , new PermutationUtil.PermutationGenerator("(456, 234, NOW())", "(234, 456, NOW())"
                , "(345, 345, NOW())").SetDelimiter(", "), string.Empty).AddExpectSQL(3, "INSERT INTO offer (offer_id, group_id, gmt) VALUES "
                , new PermutationUtil.PermutationGenerator("(123, 456, NOW())", "(345, 234, NOW())"
                , "(234, 345, NOW())", "(456, 123, NOW())").SetDelimiter(", "), string.Empty).AddExpectSQL
                (4, "INSERT INTO offer (offer_id, group_id, gmt) VALUES ", new PermutationUtil.PermutationGenerator
                ("(456, 456, NOW())").SetDelimiter(", "), string.Empty).AddExpectSQL(5, "INSERT INTO offer (offer_id, group_id, gmt) VALUES "
                , new PermutationUtil.PermutationGenerator("(234, 123, NOW())", "(123, 234, NOW())"
                ).SetDelimiter(", "), string.Empty).AddExpectSQL(6, "INSERT INTO offer (offer_id, group_id, gmt) VALUES "
                , new PermutationUtil.PermutationGenerator("(123, 123, NOW())").SetDelimiter(", "
                ), string.Empty);
            asserter = new ServerRouteTest.RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (RouteResultsetNode node_5 in nodeMap.Values)
            {
                asserter.AssertNode(node_5);
            }
            sql = "select * from offer where (offer_id, group_id ) = (123,234)";
            schema = schemaMap["cndb"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(-1l, rrs.GetLimitSize());
            NUnit.Framework.Assert.AreEqual(128, rrs.GetNodes().Length);
            for (int i = 0; i < 128; i++)
            {
                NUnit.Framework.Assert.AreEqual((int)RouteResultsetNode.DefaultReplicaIndex, rrs.
                    GetNodes()[i].GetReplicaIndex());
                NUnit.Framework.Assert.AreEqual("offer_dn[" + i + "]", rrs.GetNodes()[i].GetName(
                    ));
                NUnit.Framework.Assert.AreEqual("select * from offer where (offer_id, group_id ) = (123,234)"
                    , rrs.GetNodes()[i].GetStatement());
            }
            sql = "select * from offer where offer_id=123 and group_id=234";
            schema = schemaMap["cndb"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(-1l, rrs.GetLimitSize());
            NUnit.Framework.Assert.AreEqual(1, rrs.GetNodes().Length);
            NUnit.Framework.Assert.AreEqual((int)RouteResultsetNode.DefaultReplicaIndex, rrs.
                GetNodes()[0].GetReplicaIndex());
            NUnit.Framework.Assert.AreEqual("offer_dn[44]", rrs.GetNodes()[0].GetName());
            NUnit.Framework.Assert.AreEqual("select * from offer where offer_id=123 and group_id=234"
                , rrs.GetNodes()[0].GetStatement());
            // WITHOUT SQL CHANGE unless schema is appeared
            sql = "select * from  cndb.offer where false" + " or offer_id=123 and group_id=123 or offer_id=123 and group_id=234 or offer_id=123 and group_id=345 or offer_id=123 and group_id=456  "
                 + " or offer_id=234 and group_id=123 or offer_id=234 and group_id=234 or offer_id=234 and group_id=345 or offer_id=234 and group_id=456  "
                 + " or offer_id=345 and group_id=123 or offer_id=345 and group_id=234 or offer_id=345 and group_id=345 or offer_id=345 and group_id=456  "
                 + " or offer_id=456 and group_id=123 or offer_id=456 and group_id=234 or offer_id=456 and group_id=345 or offer_id=456 and group_id=456  ";
            schema = schemaMap["cndb"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(-1l, rrs.GetLimitSize());
            string sqlTemp = "SELECT * FROM offer WHERE FALSE OR offer_id = 123 AND group_id = 123 OR offer_id = 123 AND group_id = 234 OR offer_id = 123 AND group_id = 345 OR offer_id = 123 AND group_id = 456 OR offer_id = 234 AND group_id = 123 OR offer_id = 234 AND group_id = 234 OR offer_id = 234 AND group_id = 345 OR offer_id = 234 AND group_id = 456 OR offer_id = 345 AND group_id = 123 OR offer_id = 345 AND group_id = 234 OR offer_id = 345 AND group_id = 345 OR offer_id = 345 AND group_id = 456 OR offer_id = 456 AND group_id = 123 OR offer_id = 456 AND group_id = 234 OR offer_id = 456 AND group_id = 345 OR offer_id = 456 AND group_id = 456";
            nodeMap = GetNodeMap(rrs, 7);
            nameAsserter = new ServerRouteTest.NodeNameAsserter("offer_dn[58]", "offer_dn[100]"
                , "offer_dn[86]", "offer_dn[72]", "offer_dn[114]", "offer_dn[44]", "offer_dn[30]"
                );
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new ServerRouteTest.SimpleSQLAsserter();
            sqlAsserter.AddExpectSQL(0, sqlTemp).AddExpectSQL(1, sqlTemp).AddExpectSQL(2, sqlTemp
                ).AddExpectSQL(3, sqlTemp).AddExpectSQL(4, sqlTemp).AddExpectSQL(5, sqlTemp).AddExpectSQL
                (6, sqlTemp);
            asserter = new ServerRouteTest.RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (RouteResultsetNode node_6 in nodeMap.Values)
            {
                asserter.AssertNode(node_6);
            }
            sql = "select * from  offer where false" + " or offer_id=123 and group_id=123" +
                " or group_id=123 and offer_id=234" + " or offer_id=123 and group_id=345" + " or offer_id=123 and group_id=456  ";
            schema = schemaMap["cndb"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(-1l, rrs.GetLimitSize());
            sqlTemp = "select * from  offer where false or offer_id=123 and group_id=123 or group_id=123 and offer_id=234 or offer_id=123 and group_id=345 or offer_id=123 and group_id=456  ";
            nodeMap = GetNodeMap(rrs, 4);
            nameAsserter = new ServerRouteTest.NodeNameAsserter("offer_dn[72]", "offer_dn[58]"
                , "offer_dn[44]", "offer_dn[30]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new ServerRouteTest.SimpleSQLAsserter();
            sqlAsserter.AddExpectSQL(0, sqlTemp).AddExpectSQL(1, sqlTemp).AddExpectSQL(2, sqlTemp
                ).AddExpectSQL(3, sqlTemp);
            asserter = new ServerRouteTest.RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (RouteResultsetNode node_7 in nodeMap.Values)
            {
                asserter.AssertNode(node_7);
            }
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestGroupLimit()
        {
            SchemaConfig schema = schemaMap["cndb"];
            string sql = "select count(*) from wp_image where member_id = 'pavarotti17'";
            RouteResultset rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(-1L, rrs.GetLimitSize());
            NUnit.Framework.Assert.AreEqual(0, rrs.GetFlag());
            NUnit.Framework.Assert.AreEqual(1, rrs.GetNodes().Length);
            NUnit.Framework.Assert.AreEqual((int)RouteResultsetNode.DefaultReplicaIndex, rrs.
                GetNodes()[0].GetReplicaIndex());
            NUnit.Framework.Assert.AreEqual("offer_dn[123]", rrs.GetNodes()[0].GetName());
            NUnit.Framework.Assert.AreEqual("select count(*) from wp_image where member_id = 'pavarotti17'"
                , rrs.GetNodes()[0].GetStatement());
            sql = "select count(*) from wp_image where member_id in ('pavarotti17','qaa')";
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(-1L, rrs.GetLimitSize());
            if (rrs.GetNodes().Length > 1)
            {
                NUnit.Framework.Assert.AreEqual(RouteResultset.SumFlag, rrs.GetFlag());
            }
            IDictionary<string, RouteResultsetNode> nodeMap = GetNodeMap(rrs, 2);
            ServerRouteTest.NodeNameAsserter nameAsserter = new ServerRouteTest.NodeNameAsserter
                ("offer_dn[123]", "offer_dn[10]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            ServerRouteTest.SimpleSQLAsserter sqlAsserter = new ServerRouteTest.SimpleSQLAsserter
                ();
            sqlAsserter.AddExpectSQL(0, "SELECT COUNT(*) FROM wp_image WHERE member_id IN ('pavarotti17')"
                ).AddExpectSQL(1, "SELECT COUNT(*) FROM wp_image WHERE member_id IN ('qaa')");
            ServerRouteTest.RouteNodeAsserter asserter = new ServerRouteTest.RouteNodeAsserter
                (nameAsserter, sqlAsserter);
            foreach (RouteResultsetNode node in nodeMap.Values)
            {
                asserter.AssertNode(node);
            }
            sql = "select min(id) from wp_image where member_id in ('pavarotti17','qaa') limit 99";
            rrs = ServerRouter.Route(schema, sql, null, null);
            if (rrs.GetNodes().Length > 1)
            {
                NUnit.Framework.Assert.AreEqual(RouteResultset.MinFlag, rrs.GetFlag());
            }
            if (rrs.GetNodes().Length > 1)
            {
                NUnit.Framework.Assert.AreEqual(99L, rrs.GetLimitSize());
            }
            nodeMap = GetNodeMap(rrs, 2);
            nameAsserter = new ServerRouteTest.NodeNameAsserter("offer_dn[123]", "offer_dn[10]"
                );
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new ServerRouteTest.SimpleSQLAsserter();
            sqlAsserter.AddExpectSQL(0, "SELECT MIN(id) FROM wp_image WHERE member_id IN ('pavarotti17') LIMIT 0, 99"
                ).AddExpectSQL(1, "SELECT MIN(id) FROM wp_image WHERE member_id IN ('qaa') LIMIT 0, 99"
                );
            asserter = new ServerRouteTest.RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (RouteResultsetNode node_1 in nodeMap.Values)
            {
                asserter.AssertNode(node_1);
            }
            sql = "select max(offer_id) from offer.wp_image where member_id in ('pavarotti17','pavarotti17', 'qaa') or member_id in ('pavarotti17','1qq','pavarotti17') or offer.wp_image.member_id='pavarotti17' limit 99 offset 1";
            rrs = ServerRouter.Route(schema, sql, null, null);
            if (rrs.GetNodes().Length > 1)
            {
                NUnit.Framework.Assert.AreEqual(RouteResultset.MaxFlag, rrs.GetFlag());
            }
            if (rrs.GetNodes().Length > 1)
            {
                NUnit.Framework.Assert.AreEqual(99L, rrs.GetLimitSize());
            }
            nodeMap = GetNodeMap(rrs, 3);
            nameAsserter = new ServerRouteTest.NodeNameAsserter("offer_dn[123]", "offer_dn[10]"
                , "offer_dn[66]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new ServerRouteTest.SimpleSQLAsserter();
            sqlAsserter.AddExpectSQL(0, "SELECT MAX(offer_id) FROM wp_image WHERE member_id IN ('pavarotti17', 'pavarotti17') OR member_id IN ('pavarotti17', 'pavarotti17') OR wp_image.member_id = 'pavarotti17' LIMIT 1, 99"
                ).AddExpectSQL(1, "SELECT MAX(offer_id) FROM wp_image WHERE member_id IN ('qaa') OR FALSE OR FALSE LIMIT 1, 99"
                ).AddExpectSQL(2, "SELECT MAX(offer_id) FROM wp_image WHERE FALSE OR member_id IN ('1qq') OR FALSE LIMIT 1, 99"
                );
            asserter = new ServerRouteTest.RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (RouteResultsetNode node_2 in nodeMap.Values)
            {
                asserter.AssertNode(node_2);
            }
            sql = "select count(*) from (select * from wp_image) w, (select * from offer) o "
                 + " where o.member_id=w.member_id and o.member_id='pavarotti17' limit 99";
            rrs = ServerRouter.Route(schema, sql, null, null);
            if (rrs.GetNodes().Length > 1)
            {
                NUnit.Framework.Assert.AreEqual(RouteResultset.SumFlag, rrs.GetFlag());
            }
            if (rrs.GetNodes().Length > 1)
            {
                NUnit.Framework.Assert.AreEqual(99L, rrs.GetLimitSize());
            }
            nodeMap = GetNodeMap(rrs, 128);
            nameAsserter = new ServerRouteTest.IndexedNodeNameAsserter("offer_dn", 0, 128);
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new ServerRouteTest.SimpleSQLAsserter();
            for (int i = 0; i < 128; ++i)
            {
                sqlAsserter.AddExpectSQL(i, "select count(*) from (select * from wp_image) w, (select * from offer) o  where o.member_id=w.member_id and o.member_id='pavarotti17' limit 99"
                    );
            }
            asserter = new ServerRouteTest.RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (RouteResultsetNode node_3 in nodeMap.Values)
            {
                asserter.AssertNode(node_3);
            }
            sql = "select count(*) from (select * from wp_image) w, (select * from offer limit 99) o "
                 + " where o.member_id=w.member_id and o.member_id='pavarotti17' ";
            rrs = ServerRouter.Route(schema, sql, null, null);
            if (rrs.GetNodes().Length > 1)
            {
                NUnit.Framework.Assert.AreEqual(RouteResultset.SumFlag, rrs.GetFlag());
            }
            if (rrs.GetNodes().Length > 1)
            {
                NUnit.Framework.Assert.AreEqual(99L, rrs.GetLimitSize());
            }
            nodeMap = GetNodeMap(rrs, 128);
            nameAsserter = new ServerRouteTest.IndexedNodeNameAsserter("offer_dn", 0, 128);
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new ServerRouteTest.SimpleSQLAsserter();
            for (int i_1 = 0; i_1 < 128; ++i_1)
            {
                sqlAsserter.AddExpectSQL(i_1, "select count(*) from (select * from wp_image) w, (select * from offer limit 99) o  where o.member_id=w.member_id and o.member_id='pavarotti17' "
                    );
            }
            asserter = new ServerRouteTest.RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (RouteResultsetNode node_4 in nodeMap.Values)
            {
                asserter.AssertNode(node_4);
            }
            sql = "select count(*) from (select * from wp_image where member_id='abc' or member_id='pavarotti17' limit 100) w, (select * from offer_detail where offer_id='123') o "
                 + " where o.member_id=w.member_id and o.member_id='pavarotti17' limit 99";
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(RouteResultset.SumFlag, rrs.GetFlag());
            NUnit.Framework.Assert.AreEqual(100L, rrs.GetLimitSize());
            nodeMap = GetNodeMap(rrs, 2);
            nameAsserter = new ServerRouteTest.NodeNameAsserter("offer_dn[12]", "offer_dn[123]"
                );
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new ServerRouteTest.SimpleSQLAsserter();
            sqlAsserter.AddExpectSQL(0, "SELECT COUNT(*) FROM (SELECT * FROM wp_image WHERE member_id = 'abc' OR FALSE LIMIT 0, 100) AS "
                 + AliasConvert("w") + ", (SELECT * FROM offer_detail WHERE offer_id = '123') AS "
                 + AliasConvert("o") + " WHERE o.member_id = w.member_id AND o.member_id = 'pavarotti17' LIMIT 0, 99"
                ).AddExpectSQL(1, "SELECT COUNT(*) FROM (SELECT * FROM wp_image WHERE FALSE OR member_id = 'pavarotti17' LIMIT 0, 100) AS "
                 + AliasConvert("w") + ", (SELECT * FROM offer_detail WHERE offer_id = '123') AS "
                 + AliasConvert("o") + " WHERE o.member_id = w.member_id AND o.member_id = 'pavarotti17' LIMIT 0, 99"
                );
            asserter = new ServerRouteTest.RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (RouteResultsetNode node_5 in nodeMap.Values)
            {
                asserter.AssertNode(node_5);
            }
            sql = "select count(*) from (select * from(select * from offer_detail where offer_id='123' or offer_id='234' limit 88)offer  where offer.member_id='abc' limit 60) w "
                 + " where w.member_id ='pavarotti17' limit 99";
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(88L, rrs.GetLimitSize());
            NUnit.Framework.Assert.AreEqual(RouteResultset.SumFlag, rrs.GetFlag());
            nodeMap = GetNodeMap(rrs, 2);
            nameAsserter = new ServerRouteTest.NodeNameAsserter("detail_dn[29]", "detail_dn[15]"
                );
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new ServerRouteTest.SimpleSQLAsserter();
            sqlAsserter.AddExpectSQL(0, "SELECT COUNT(*) FROM (SELECT * FROM (SELECT * FROM offer_detail WHERE FALSE OR offer_id = '234' LIMIT 0, 88) AS "
                 + AliasConvert("offer") + " WHERE offer.member_id = 'abc' LIMIT 0, 60) AS " + AliasConvert
                ("w") + " WHERE w.member_id = 'pavarotti17' LIMIT 0, 99").AddExpectSQL(1, "SELECT COUNT(*) FROM (SELECT * FROM (SELECT * FROM offer_detail WHERE offer_id = '123' OR FALSE LIMIT 0, 88) AS "
                 + AliasConvert("offer") + " WHERE offer.member_id = 'abc' LIMIT 0, 60) AS " + AliasConvert
                ("w") + " WHERE w.member_id = 'pavarotti17' LIMIT 0, 99");
            asserter = new ServerRouteTest.RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (RouteResultsetNode node_6 in nodeMap.Values)
            {
                asserter.AssertNode(node_6);
            }
            sql = "select count(*) from (select * from(select max(id) from offer_detail where offer_id='123' or offer_id='234' limit 88)offer  where offer.member_id='abc' limit 60) w "
                 + " where w.member_id ='pavarotti17' limit 99";
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(88L, rrs.GetLimitSize());
            NUnit.Framework.Assert.AreEqual(0, rrs.GetFlag());
            nodeMap = GetNodeMap(rrs, 2);
            nameAsserter = new ServerRouteTest.NodeNameAsserter("detail_dn[29]", "detail_dn[15]"
                );
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new ServerRouteTest.SimpleSQLAsserter();
            sqlAsserter.AddExpectSQL(0, "SELECT COUNT(*) FROM (SELECT * FROM (SELECT MAX(id) FROM offer_detail WHERE FALSE OR offer_id = '234' LIMIT 0, 88) AS "
                 + AliasConvert("offer") + " WHERE offer.member_id = 'abc' LIMIT 0, 60) AS " + AliasConvert
                ("w") + " WHERE w.member_id = 'pavarotti17' LIMIT 0, 99").AddExpectSQL(1, "SELECT COUNT(*) FROM (SELECT * FROM (SELECT MAX(id) FROM offer_detail WHERE offer_id = '123' OR FALSE LIMIT 0, 88) AS "
                 + AliasConvert("offer") + " WHERE offer.member_id = 'abc' LIMIT 0, 60) AS " + AliasConvert
                ("w") + " WHERE w.member_id = 'pavarotti17' LIMIT 0, 99");
            asserter = new ServerRouteTest.RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (RouteResultsetNode node_7 in nodeMap.Values)
            {
                asserter.AssertNode(node_7);
            }
            sql = "select * from (select * from(select max(id) from offer_detail where offer_id='123' or offer_id='234' limit 88)offer  where offer.member_id='abc' limit 60) w "
                 + " where w.member_id ='pavarotti17' limit 99";
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(88L, rrs.GetLimitSize());
            NUnit.Framework.Assert.AreEqual(RouteResultset.MaxFlag, rrs.GetFlag());
            nodeMap = GetNodeMap(rrs, 2);
            nameAsserter = new ServerRouteTest.NodeNameAsserter("detail_dn[29]", "detail_dn[15]"
                );
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new ServerRouteTest.SimpleSQLAsserter();
            sqlAsserter.AddExpectSQL(0, "SELECT * FROM (SELECT * FROM (SELECT MAX(id) FROM offer_detail WHERE FALSE OR offer_id = '234' LIMIT 0, 88) AS "
                 + AliasConvert("offer") + " WHERE offer.member_id = 'abc' LIMIT 0, 60) AS " + AliasConvert
                ("w") + " WHERE w.member_id = 'pavarotti17' LIMIT 0, 99").AddExpectSQL(1, "SELECT * FROM (SELECT * FROM (SELECT MAX(id) FROM offer_detail WHERE offer_id = '123' OR FALSE LIMIT 0, 88) AS "
                 + AliasConvert("offer") + " WHERE offer.member_id = 'abc' LIMIT 0, 60) AS " + AliasConvert
                ("w") + " WHERE w.member_id = 'pavarotti17' LIMIT 0, 99");
            asserter = new ServerRouteTest.RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (RouteResultsetNode node_8 in nodeMap.Values)
            {
                asserter.AssertNode(node_8);
            }
            sql = "select * from (select count(*) from(select * from offer_detail where offer_id='123' or offer_id='234' limit 88)offer  where offer.member_id='abc' limit 60) w "
                 + " where w.member_id ='pavarotti17' limit 99";
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(88L, rrs.GetLimitSize());
            NUnit.Framework.Assert.AreEqual(RouteResultset.SumFlag, rrs.GetFlag());
            nodeMap = GetNodeMap(rrs, 2);
            nameAsserter = new ServerRouteTest.NodeNameAsserter("detail_dn[29]", "detail_dn[15]"
                );
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new ServerRouteTest.SimpleSQLAsserter();
            sqlAsserter.AddExpectSQL(0, "SELECT * FROM (SELECT COUNT(*) FROM (SELECT * FROM offer_detail WHERE FALSE OR offer_id = '234' LIMIT 0, 88) AS "
                 + AliasConvert("offer") + " WHERE offer.member_id = 'abc' LIMIT 0, 60) AS " + AliasConvert
                ("w") + " WHERE w.member_id = 'pavarotti17' LIMIT 0, 99").AddExpectSQL(1, "SELECT * FROM (SELECT COUNT(*) FROM (SELECT * FROM offer_detail WHERE offer_id = '123' OR FALSE LIMIT 0, 88) AS "
                 + AliasConvert("offer") + " WHERE offer.member_id = 'abc' LIMIT 0, 60) AS " + AliasConvert
                ("w") + " WHERE w.member_id = 'pavarotti17' LIMIT 0, 99");
            asserter = new ServerRouteTest.RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (RouteResultsetNode node_9 in nodeMap.Values)
            {
                asserter.AssertNode(node_9);
            }
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestDimension2Route()
        {
            SchemaConfig schema = schemaMap["cndb"];
            string sql = "select * from product_visit where member_id='pavarotti17' and product_id=2345";
            RouteResultset rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(-1l, rrs.GetLimitSize());
            NUnit.Framework.Assert.AreEqual(1, rrs.GetNodes().Length);
            NUnit.Framework.Assert.AreEqual((int)RouteResultsetNode.DefaultReplicaIndex, rrs.
                GetNodes()[0].GetReplicaIndex());
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual("offer_dn[9]", rrs.GetNodes()[0].GetName());
            NUnit.Framework.Assert.AreEqual("select * from product_visit where member_id='pavarotti17' and product_id=2345"
                , rrs.GetNodes()[0].GetStatement());
            sql = "select * from product_visit where member_id='pavarotti17' ";
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(-1l, rrs.GetLimitSize());
            IDictionary<string, RouteResultsetNode> nodeMap = GetNodeMap(rrs, 8);
            ServerRouteTest.NodeNameAsserter nameAsserter = new ServerRouteTest.NodeNameAsserter
                ("offer_dn[25]", "offer_dn[17]", "offer_dn[9]", "offer_dn[1]", "offer_dn[29]", "offer_dn[21]"
                , "offer_dn[5]", "offer_dn[13]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            ServerRouteTest.SimpleSQLAsserter sqlAsserter = new ServerRouteTest.SimpleSQLAsserter
                ();
            sqlAsserter.AddExpectSQL(0, "SELECT * FROM product_visit WHERE member_id = 'pavarotti17'"
                ).AddExpectSQL(1, "SELECT * FROM product_visit WHERE member_id = 'pavarotti17'")
                .AddExpectSQL(2, "SELECT * FROM product_visit WHERE member_id = 'pavarotti17'").
                AddExpectSQL(3, "SELECT * FROM product_visit WHERE member_id = 'pavarotti17'").AddExpectSQL
                (4, "SELECT * FROM product_visit WHERE member_id = 'pavarotti17'").AddExpectSQL(
                5, "SELECT * FROM product_visit WHERE member_id = 'pavarotti17'").AddExpectSQL(6
                , "SELECT * FROM product_visit WHERE member_id = 'pavarotti17'").AddExpectSQL(7,
                "SELECT * FROM product_visit WHERE member_id = 'pavarotti17'");
            ServerRouteTest.RouteNodeAsserter asserter = new ServerRouteTest.RouteNodeAsserter
                (nameAsserter, sqlAsserter);
            foreach (RouteResultsetNode node in nodeMap.Values)
            {
                asserter.AssertNode(node);
            }
            sql = "select * from product_visit where member_id='abc' ";
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(-1l, rrs.GetLimitSize());
            nodeMap = GetNodeMap(rrs, 8);
            nameAsserter = new ServerRouteTest.NodeNameAsserter("offer_dn[0]", "offer_dn[4]",
                "offer_dn[8]", "offer_dn[12]", "offer_dn[16]", "offer_dn[20]", "offer_dn[24]", "offer_dn[28]"
                );
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new ServerRouteTest.SimpleSQLAsserter();
            sqlAsserter.AddExpectSQL(0, "SELECT * FROM product_visit WHERE member_id = 'abc'"
                ).AddExpectSQL(1, "SELECT * FROM product_visit WHERE member_id = 'abc'").AddExpectSQL
                (2, "SELECT * FROM product_visit WHERE member_id = 'abc'").AddExpectSQL(3, "SELECT * FROM product_visit WHERE member_id = 'abc'"
                ).AddExpectSQL(4, "SELECT * FROM product_visit WHERE member_id = 'abc'").AddExpectSQL
                (5, "SELECT * FROM product_visit WHERE member_id = 'abc'").AddExpectSQL(6, "SELECT * FROM product_visit WHERE member_id = 'abc'"
                ).AddExpectSQL(7, "SELECT * FROM product_visit WHERE member_id = 'abc'");
            asserter = new ServerRouteTest.RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (RouteResultsetNode node_1 in nodeMap.Values)
            {
                asserter.AssertNode(node_1);
            }
            sql = "delete from product_visit where member_id='pavarotti17' or Member_id between 'abc' and 'abc'";
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(-1l, rrs.GetLimitSize());
            nodeMap = GetNodeMap(rrs, 16);
            nameAsserter = new ServerRouteTest.NodeNameAsserter("offer_dn[0]", "offer_dn[4]",
                "offer_dn[8]", "offer_dn[12]", "offer_dn[16]", "offer_dn[20]", "offer_dn[24]", "offer_dn[28]"
                , "offer_dn[1]", "offer_dn[5]", "offer_dn[9]", "offer_dn[13]", "offer_dn[17]", "offer_dn[21]"
                , "offer_dn[25]", "offer_dn[29]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new ServerRouteTest.SimpleSQLAsserter();
            sqlAsserter.AddExpectSQL(0, "DELETE FROM product_visit WHERE FALSE OR Member_id BETWEEN 'abc' AND 'abc'"
                ).AddExpectSQL(1, "DELETE FROM product_visit WHERE FALSE OR Member_id BETWEEN 'abc' AND 'abc'"
                ).AddExpectSQL(2, "DELETE FROM product_visit WHERE FALSE OR Member_id BETWEEN 'abc' AND 'abc'"
                ).AddExpectSQL(3, "DELETE FROM product_visit WHERE FALSE OR Member_id BETWEEN 'abc' AND 'abc'"
                ).AddExpectSQL(4, "DELETE FROM product_visit WHERE FALSE OR Member_id BETWEEN 'abc' AND 'abc'"
                ).AddExpectSQL(5, "DELETE FROM product_visit WHERE FALSE OR Member_id BETWEEN 'abc' AND 'abc'"
                ).AddExpectSQL(6, "DELETE FROM product_visit WHERE FALSE OR Member_id BETWEEN 'abc' AND 'abc'"
                ).AddExpectSQL(7, "DELETE FROM product_visit WHERE FALSE OR Member_id BETWEEN 'abc' AND 'abc'"
                ).AddExpectSQL(8, "DELETE FROM product_visit WHERE member_id = 'pavarotti17' OR FALSE"
                ).AddExpectSQL(9, "DELETE FROM product_visit WHERE member_id = 'pavarotti17' OR FALSE"
                ).AddExpectSQL(10, "DELETE FROM product_visit WHERE member_id = 'pavarotti17' OR FALSE"
                ).AddExpectSQL(11, "DELETE FROM product_visit WHERE member_id = 'pavarotti17' OR FALSE"
                ).AddExpectSQL(12, "DELETE FROM product_visit WHERE member_id = 'pavarotti17' OR FALSE"
                ).AddExpectSQL(13, "DELETE FROM product_visit WHERE member_id = 'pavarotti17' OR FALSE"
                ).AddExpectSQL(14, "DELETE FROM product_visit WHERE member_id = 'pavarotti17' OR FALSE"
                ).AddExpectSQL(15, "DELETE FROM product_visit WHERE member_id = 'pavarotti17' OR FALSE"
                );
            asserter = new ServerRouteTest.RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (RouteResultsetNode node_2 in nodeMap.Values)
            {
                asserter.AssertNode(node_2);
            }
            sql = "select * from product_visit where  product_id=2345 ";
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(-1l, rrs.GetLimitSize());
            nodeMap = GetNodeMap(rrs, 4);
            nameAsserter = new ServerRouteTest.NodeNameAsserter("offer_dn[8]", "offer_dn[9]",
                "offer_dn[10]", "offer_dn[11]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new ServerRouteTest.SimpleSQLAsserter();
            sqlAsserter.AddExpectSQL(0, "SELECT * FROM product_visit WHERE product_id = 2345"
                ).AddExpectSQL(1, "SELECT * FROM product_visit WHERE product_id = 2345").AddExpectSQL
                (2, "SELECT * FROM product_visit WHERE product_id = 2345").AddExpectSQL(3, "SELECT * FROM product_visit WHERE product_id = 2345"
                );
            asserter = new ServerRouteTest.RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (RouteResultsetNode node_3 in nodeMap.Values)
            {
                asserter.AssertNode(node_3);
            }
            sql = "select * from product_visit where  product_id=1234 ";
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(-1l, rrs.GetLimitSize());
            nodeMap = GetNodeMap(rrs, 4);
            nameAsserter = new ServerRouteTest.NodeNameAsserter("offer_dn[4]", "offer_dn[5]",
                "offer_dn[6]", "offer_dn[7]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new ServerRouteTest.SimpleSQLAsserter();
            sqlAsserter.AddExpectSQL(0, "SELECT * FROM product_visit WHERE product_id = 1234"
                ).AddExpectSQL(1, "SELECT * FROM product_visit WHERE product_id = 1234").AddExpectSQL
                (2, "SELECT * FROM product_visit WHERE product_id = 1234").AddExpectSQL(3, "SELECT * FROM product_visit WHERE product_id = 1234"
                );
            asserter = new ServerRouteTest.RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (RouteResultsetNode node_4 in nodeMap.Values)
            {
                asserter.AssertNode(node_4);
            }
            sql = "select * from product_visit where  product_id=1234 or product_id=2345 ";
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(-1l, rrs.GetLimitSize());
            nodeMap = GetNodeMap(rrs, 8);
            nameAsserter = new ServerRouteTest.NodeNameAsserter("offer_dn[4]", "offer_dn[5]",
                "offer_dn[6]", "offer_dn[7]", "offer_dn[8]", "offer_dn[9]", "offer_dn[10]", "offer_dn[11]"
                );
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new ServerRouteTest.SimpleSQLAsserter();
            sqlAsserter.AddExpectSQL(0, "SELECT * FROM product_visit WHERE product_id = 1234 OR FALSE"
                ).AddExpectSQL(1, "SELECT * FROM product_visit WHERE product_id = 1234 OR FALSE"
                ).AddExpectSQL(2, "SELECT * FROM product_visit WHERE product_id = 1234 OR FALSE"
                ).AddExpectSQL(3, "SELECT * FROM product_visit WHERE product_id = 1234 OR FALSE"
                ).AddExpectSQL(4, "SELECT * FROM product_visit WHERE FALSE OR product_id = 2345"
                ).AddExpectSQL(5, "SELECT * FROM product_visit WHERE FALSE OR product_id = 2345"
                ).AddExpectSQL(6, "SELECT * FROM product_visit WHERE FALSE OR product_id = 2345"
                ).AddExpectSQL(7, "SELECT * FROM product_visit WHERE FALSE OR product_id = 2345"
                );
            asserter = new ServerRouteTest.RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (RouteResultsetNode node_5 in nodeMap.Values)
            {
                asserter.AssertNode(node_5);
            }
            sql = "select * from product_visit where  product_id in (1234,2345) ";
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(-1l, rrs.GetLimitSize());
            nodeMap = GetNodeMap(rrs, 8);
            nameAsserter = new ServerRouteTest.NodeNameAsserter("offer_dn[4]", "offer_dn[5]",
                "offer_dn[6]", "offer_dn[7]", "offer_dn[8]", "offer_dn[9]", "offer_dn[10]", "offer_dn[11]"
                );
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new ServerRouteTest.SimpleSQLAsserter();
            sqlAsserter.AddExpectSQL(0, "SELECT * FROM product_visit WHERE product_id IN (1234)"
                ).AddExpectSQL(1, "SELECT * FROM product_visit WHERE product_id IN (1234)").AddExpectSQL
                (2, "SELECT * FROM product_visit WHERE product_id IN (1234)").AddExpectSQL(3, "SELECT * FROM product_visit WHERE product_id IN (1234)"
                ).AddExpectSQL(4, "SELECT * FROM product_visit WHERE product_id IN (2345)").AddExpectSQL
                (5, "SELECT * FROM product_visit WHERE product_id IN (2345)").AddExpectSQL(6, "SELECT * FROM product_visit WHERE product_id IN (2345)"
                ).AddExpectSQL(7, "SELECT * FROM product_visit WHERE product_id IN (2345)");
            asserter = new ServerRouteTest.RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (RouteResultsetNode node_6 in nodeMap.Values)
            {
                asserter.AssertNode(node_6);
            }
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestBackquotedColumn()
        {
            SchemaConfig schema = schemaMap["cndb"];
            string sql = "select * from wp_image where `seLect`='pavarotti17' ";
            RouteResultset rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(-1l, rrs.GetLimitSize());
            NUnit.Framework.Assert.AreEqual(1, rrs.GetNodes().Length);
            NUnit.Framework.Assert.AreEqual((int)RouteResultsetNode.DefaultReplicaIndex, rrs.
                GetNodes()[0].GetReplicaIndex());
            NUnit.Framework.Assert.AreEqual("offer_dn[123]", rrs.GetNodes()[0].GetName());
            NUnit.Framework.Assert.AreEqual("select * from wp_image where `seLect`='pavarotti17' "
                , rrs.GetNodes()[0].GetStatement());
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestTableMetaRead()
        {
            SchemaConfig schema = schemaMap["cndb"];
            string sql = "desc offer";
            RouteResultset rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(-1L, rrs.GetLimitSize());
            NUnit.Framework.Assert.AreEqual(1, rrs.GetNodes().Length);
            NUnit.Framework.Assert.AreEqual((int)RouteResultsetNode.DefaultReplicaIndex, rrs.
                GetNodes()[0].GetReplicaIndex());
            NUnit.Framework.Assert.AreEqual("offer_dn[0]", rrs.GetNodes()[0].GetName());
            NUnit.Framework.Assert.AreEqual("desc offer", rrs.GetNodes()[0].GetStatement());
            sql = "desc cndb.offer";
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(-1L, rrs.GetLimitSize());
            NUnit.Framework.Assert.AreEqual(1, rrs.GetNodes().Length);
            NUnit.Framework.Assert.AreEqual((int)RouteResultsetNode.DefaultReplicaIndex, rrs.
                GetNodes()[0].GetReplicaIndex());
            NUnit.Framework.Assert.AreEqual("offer_dn[0]", rrs.GetNodes()[0].GetName());
            NUnit.Framework.Assert.AreEqual("DESC offer", rrs.GetNodes()[0].GetStatement());
            sql = "SHOW FULL COLUMNS FROM  offer  IN db_name WHERE true";
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(-1L, rrs.GetLimitSize());
            NUnit.Framework.Assert.AreEqual(1, rrs.GetNodes().Length);
            NUnit.Framework.Assert.AreEqual((int)RouteResultsetNode.DefaultReplicaIndex, rrs.
                GetNodes()[0].GetReplicaIndex());
            NUnit.Framework.Assert.AreEqual("offer_dn[0]", rrs.GetNodes()[0].GetName());
            NUnit.Framework.Assert.AreEqual("SHOW FULL COLUMNS FROM offer WHERE TRUE", rrs.GetNodes
                ()[0].GetStatement());
            sql = "SHOW FULL COLUMNS FROM  db.offer  IN db_name WHERE true";
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(-1L, rrs.GetLimitSize());
            NUnit.Framework.Assert.AreEqual(1, rrs.GetNodes().Length);
            NUnit.Framework.Assert.AreEqual((int)RouteResultsetNode.DefaultReplicaIndex, rrs.
                GetNodes()[0].GetReplicaIndex());
            NUnit.Framework.Assert.AreEqual("offer_dn[0]", rrs.GetNodes()[0].GetName());
            NUnit.Framework.Assert.AreEqual("SHOW FULL COLUMNS FROM offer WHERE TRUE", rrs.GetNodes
                ()[0].GetStatement());
            sql = "SHOW INDEX  IN offer FROM  db_name";
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(-1L, rrs.GetLimitSize());
            NUnit.Framework.Assert.AreEqual(1, rrs.GetNodes().Length);
            NUnit.Framework.Assert.AreEqual((int)RouteResultsetNode.DefaultReplicaIndex, rrs.
                GetNodes()[0].GetReplicaIndex());
            NUnit.Framework.Assert.AreEqual("offer_dn[0]", rrs.GetNodes()[0].GetName());
            NUnit.Framework.Assert.AreEqual("SHOW INDEX IN offer", rrs.GetNodes()[0].GetStatement
                ());
            sql = "SHOW TABLES from db_name like 'solo'";
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(-1L, rrs.GetLimitSize());
            IDictionary<string, RouteResultsetNode> nodeMap = GetNodeMap(rrs, 4);
            ServerRouteTest.NodeNameAsserter nameAsserter = new ServerRouteTest.NodeNameAsserter
                ("detail_dn[0]", "offer_dn[0]", "cndb_dn", "independent_dn[0]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            ServerRouteTest.SimpleSQLAsserter sqlAsserter = new ServerRouteTest.SimpleSQLAsserter
                ();
            sqlAsserter.AddExpectSQL(0, "SHOW TABLES LIKE 'solo'").AddExpectSQL(1, "SHOW TABLES LIKE 'solo'"
                ).AddExpectSQL(2, "SHOW TABLES LIKE 'solo'").AddExpectSQL(3, "SHOW TABLES LIKE 'solo'"
                );
            ServerRouteTest.RouteNodeAsserter asserter = new ServerRouteTest.RouteNodeAsserter
                (nameAsserter, sqlAsserter);
            foreach (RouteResultsetNode node in nodeMap.Values)
            {
                asserter.AssertNode(node);
            }
            sql = "SHOW TABLES in db_name ";
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(-1L, rrs.GetLimitSize());
            nodeMap = GetNodeMap(rrs, 4);
            nameAsserter = new ServerRouteTest.NodeNameAsserter("detail_dn[0]", "offer_dn[0]"
                , "cndb_dn", "independent_dn[0]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new ServerRouteTest.SimpleSQLAsserter();
            sqlAsserter.AddExpectSQL(0, "SHOW TABLES").AddExpectSQL(1, "SHOW TABLES").AddExpectSQL
                (2, "SHOW TABLES").AddExpectSQL(3, "SHOW TABLES");
            asserter = new ServerRouteTest.RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (RouteResultsetNode node_1 in nodeMap.Values)
            {
                asserter.AssertNode(node_1);
            }
            sql = "SHOW TABLeS ";
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(-1L, rrs.GetLimitSize());
            nodeMap = GetNodeMap(rrs, 4);
            nameAsserter = new ServerRouteTest.NodeNameAsserter("detail_dn[0]", "offer_dn[0]"
                , "cndb_dn", "independent_dn[0]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new ServerRouteTest.SimpleSQLAsserter();
            sqlAsserter.AddExpectSQL(0, "SHOW TABLeS ").AddExpectSQL(1, "SHOW TABLeS ").AddExpectSQL
                (2, "SHOW TABLeS ").AddExpectSQL(3, "SHOW TABLeS ");
            asserter = new ServerRouteTest.RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (RouteResultsetNode node_2 in nodeMap.Values)
            {
                asserter.AssertNode(node_2);
            }
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestCobarHint()
        {
            SchemaConfig schema = schemaMap["cndb"];
            string sql = "  /*!cobar: $dataNodeId=2.1, $table='offer'*/ select * from `dual`";
            RouteResultset rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(-1L, rrs.GetLimitSize());
            NUnit.Framework.Assert.AreEqual(1, rrs.GetNodes().Length);
            NUnit.Framework.Assert.AreEqual(1, rrs.GetNodes()[0].GetReplicaIndex());
            NUnit.Framework.Assert.AreEqual("offer_dn[2]", rrs.GetNodes()[0].GetName());
            NUnit.Framework.Assert.AreEqual(" select * from `dual`", rrs.GetNodes()[0].GetStatement
                ());
            sql = "/*!cobar: $dataNodeId=2.1, $table='offer', $replica =2*/ select * from `dual`";
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(-1L, rrs.GetLimitSize());
            NUnit.Framework.Assert.AreEqual(1, rrs.GetNodes().Length);
            NUnit.Framework.Assert.AreEqual(1, rrs.GetNodes()[0].GetReplicaIndex());
            NUnit.Framework.Assert.AreEqual("offer_dn[2]", rrs.GetNodes()[0].GetName());
            NUnit.Framework.Assert.AreEqual(" select * from `dual`", rrs.GetNodes()[0].GetStatement
                ());
            sql = "/*!cobar: $dataNodeId   = [ 1,2,5.2]  , $table =  'offer'   */ select * from `dual`";
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(-1L, rrs.GetLimitSize());
            IDictionary<string, RouteResultsetNode> nodeMap = GetNodeMap(rrs, 3);
            ServerRouteTest.NodeNameAsserter nameAsserter = new ServerRouteTest.NodeNameAsserter
                ("offer_dn[1]", "offer_dn[2]", "offer_dn[5]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            ServerRouteTest.SimpleSQLAsserter sqlAsserter = new ServerRouteTest.SimpleSQLAsserter
                ();
            sqlAsserter.AddExpectSQL(0, " select * from `dual`").AddExpectSQL(1, " select * from `dual`"
                ).AddExpectSQL(2, " select * from `dual`");
            ServerRouteTest.RouteNodeAsserter asserter = new ServerRouteTest.RouteNodeAsserter
                (nameAsserter, sqlAsserter, new _ReplicaAsserter_1468());
            foreach (RouteResultsetNode node in nodeMap.Values)
            {
                asserter.AssertNode(node);
            }
            sql = "/*!cobar: $dataNodeId   = [ 1,2,5.2]  , $table =  'offer'  , $replica =1 */ select * from `dual`";
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(-1L, rrs.GetLimitSize());
            NUnit.Framework.Assert.AreEqual(3, rrs.GetNodes().Length);
            nodeMap = GetNodeMap(rrs, 3);
            nameAsserter = new ServerRouteTest.NodeNameAsserter("offer_dn[1]", "offer_dn[2]",
                "offer_dn[5]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new ServerRouteTest.SimpleSQLAsserter();
            sqlAsserter.AddExpectSQL(0, " select * from `dual`").AddExpectSQL(1, " select * from `dual`"
                ).AddExpectSQL(2, " select * from `dual`");
            asserter = new ServerRouteTest.RouteNodeAsserter(nameAsserter, sqlAsserter, new _ReplicaAsserter_1492
                ());
            foreach (RouteResultsetNode node_1 in nodeMap.Values)
            {
                asserter.AssertNode(node_1);
            }
            sql = "/*!cobar: $partitionOperand=( 'member_id' = 'pavarotti17'), $table='offer'*/ select * from `dual`";
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(-1L, rrs.GetLimitSize());
            NUnit.Framework.Assert.AreEqual(1, rrs.GetNodes().Length);
            NUnit.Framework.Assert.AreEqual((int)RouteResultsetNode.DefaultReplicaIndex, rrs.
                GetNodes()[0].GetReplicaIndex());
            NUnit.Framework.Assert.AreEqual("offer_dn[123]", rrs.GetNodes()[0].GetName());
            NUnit.Framework.Assert.AreEqual(" select * from `dual`", rrs.GetNodes()[0].GetStatement
                ());
            sql = "/*!cobar:$partitionOperand =   ( 'member_id' = ['pavarotti17'  ,   'qaa' ]  ), $table='offer'  , $replica =  2*/  select * from `dual`";
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(-1L, rrs.GetLimitSize());
            nodeMap = GetNodeMap(rrs, 2);
            nameAsserter = new ServerRouteTest.NodeNameAsserter("offer_dn[123]", "offer_dn[10]"
                );
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new ServerRouteTest.SimpleSQLAsserter();
            sqlAsserter.AddExpectSQL(0, "  select * from `dual`").AddExpectSQL(1, "  select * from `dual`"
                );
            asserter = new ServerRouteTest.RouteNodeAsserter(nameAsserter, sqlAsserter, new _ReplicaAsserter_1521
                ());
            foreach (RouteResultsetNode node_2 in nodeMap.Values)
            {
                asserter.AssertNode(node_2);
            }
            sql = "/*!cobar:$partitionOperand = ( ['group_id','offer_id'] = [234,4]), $table='offer'*/ select * from `dual`";
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(-1L, rrs.GetLimitSize());
            NUnit.Framework.Assert.AreEqual(1, rrs.GetNodes().Length);
            NUnit.Framework.Assert.AreEqual((int)RouteResultsetNode.DefaultReplicaIndex, rrs.
                GetNodes()[0].GetReplicaIndex());
            NUnit.Framework.Assert.AreEqual("offer_dn[29]", rrs.GetNodes()[0].GetName());
            NUnit.Framework.Assert.AreEqual(" select * from `dual`", rrs.GetNodes()[0].GetStatement
                ());
            sql = "/*!cobar:$partitionOperand=(['offer_id','group_id']=[[123,3],[234,4]]), $table='offer'  , $replica =2*/ select * from `dual`";
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(-1L, rrs.GetLimitSize());
            nodeMap = GetNodeMap(rrs, 2);
            nameAsserter = new ServerRouteTest.NodeNameAsserter("offer_dn[29]", "offer_dn[15]"
                );
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new ServerRouteTest.SimpleSQLAsserter();
            sqlAsserter.AddExpectSQL(0, " select * from `dual`").AddExpectSQL(1, " select * from `dual`"
                );
            asserter = new ServerRouteTest.RouteNodeAsserter(nameAsserter, sqlAsserter, new _ReplicaAsserter_1547
                ());
            foreach (RouteResultsetNode node_3 in nodeMap.Values)
            {
                asserter.AssertNode(node_3);
            }
            sql = "/*!cobar:$partitionOperand=(['group_id','offer_id']=[[123,3], [ 234,4 ] ]), $table='offer'  */ select * from `dual`";
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(-1L, rrs.GetLimitSize());
            nodeMap = GetNodeMap(rrs, 2);
            nameAsserter = new ServerRouteTest.NodeNameAsserter("offer_dn[29]", "offer_dn[15]"
                );
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new ServerRouteTest.SimpleSQLAsserter();
            sqlAsserter.AddExpectSQL(0, " select * from `dual`").AddExpectSQL(1, " select * from `dual`"
                );
            asserter = new ServerRouteTest.RouteNodeAsserter(nameAsserter, sqlAsserter, new _ReplicaAsserter_1565
                ());
            foreach (RouteResultsetNode node_4 in nodeMap.Values)
            {
                asserter.AssertNode(node_4);
            }
            sql = "/*!cobar:$partitionOperand=(['offer_id','NON_EXistence']=[[123,3],[234,4]]), $table='offer'  , $replica =2*/ select * from `dual`";
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(-1L, rrs.GetLimitSize());
            nodeMap = GetNodeMap(rrs, 128);
            nameAsserter = new ServerRouteTest.IndexedNodeNameAsserter("offer_dn", 0, 128);
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new ServerRouteTest.SimpleSQLAsserter();
            for (int i = 0; i < 128; i++)
            {
                sqlAsserter.AddExpectSQL(i, " select * from `dual`");
            }
            asserter = new ServerRouteTest.RouteNodeAsserter(nameAsserter, sqlAsserter, new _ReplicaAsserter_1585
                ());
            foreach (RouteResultsetNode node_5 in nodeMap.Values)
            {
                asserter.AssertNode(node_5);
            }
            sql = "/*!cobar:  $dataNodeId   = 1  ,$table =  'wp_image'*/ select * from `dual`";
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(-1L, rrs.GetLimitSize());
            nodeMap = GetNodeMap(rrs, 1);
            nameAsserter = new ServerRouteTest.NodeNameAsserter("offer_dn[1]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new ServerRouteTest.SimpleSQLAsserter();
            sqlAsserter.AddExpectSQL(0, " select * from `dual`");
            asserter = new ServerRouteTest.RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (RouteResultsetNode node_6 in nodeMap.Values)
            {
                asserter.AssertNode(node_6);
            }
            sql = "/*!cobar:  $dataNodeId   = [0,3]  ,$table =  'wp_image'*/ select * from `dual`";
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(-1L, rrs.GetLimitSize());
            nodeMap = GetNodeMap(rrs, 2);
            nameAsserter = new ServerRouteTest.NodeNameAsserter("offer_dn[0]", "offer_dn[3]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new ServerRouteTest.SimpleSQLAsserter();
            sqlAsserter.AddExpectSQL(0, " select * from `dual`").AddExpectSQL(1, " select * from `dual`"
                );
            asserter = new ServerRouteTest.RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (RouteResultsetNode node_7 in nodeMap.Values)
            {
                asserter.AssertNode(node_7);
            }
            sql = "/*!cobar:  $table =  'wp_image'*/ select * from `dual`";
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(-1L, rrs.GetLimitSize());
            nodeMap = GetNodeMap(rrs, 128);
            nameAsserter = new ServerRouteTest.IndexedNodeNameAsserter("offer_dn", 0, 128);
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new ServerRouteTest.SimpleSQLAsserter();
            for (int i_1 = 0; i_1 < 128; i_1++)
            {
                sqlAsserter.AddExpectSQL(i_1, " select * from `dual`");
            }
            asserter = new ServerRouteTest.RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (RouteResultsetNode node_8 in nodeMap.Values)
            {
                asserter.AssertNode(node_8);
            }
            sql = "/*!cobar:  $dataNodeId   = 0  ,$table =  'independent'*/ select * from `dual`";
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(-1L, rrs.GetLimitSize());
            nodeMap = GetNodeMap(rrs, 1);
            nameAsserter = new ServerRouteTest.NodeNameAsserter("independent_dn[0]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new ServerRouteTest.SimpleSQLAsserter();
            sqlAsserter.AddExpectSQL(0, " select * from `dual`");
            asserter = new ServerRouteTest.RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (RouteResultsetNode node_9 in nodeMap.Values)
            {
                asserter.AssertNode(node_9);
            }
            sql = "/*!cobar:  $dataNodeId   = [ 1,2,5]  ,$table =  'independent'*/ select * from `dual`";
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(-1L, rrs.GetLimitSize());
            nodeMap = GetNodeMap(rrs, 3);
            nameAsserter = new ServerRouteTest.NodeNameAsserter("independent_dn[1]", "independent_dn[2]"
                , "independent_dn[5]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new ServerRouteTest.SimpleSQLAsserter();
            sqlAsserter.AddExpectSQL(0, " select * from `dual`").AddExpectSQL(1, " select * from `dual`"
                ).AddExpectSQL(2, " select * from `dual`");
            asserter = new ServerRouteTest.RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (RouteResultsetNode node_10 in nodeMap.Values)
            {
                asserter.AssertNode(node_10);
            }
            sql = "/*!cobar:  $table =  'independent'*/ select * from `dual`";
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(-1L, rrs.GetLimitSize());
            nodeMap = GetNodeMap(rrs, 128);
            nameAsserter = new ServerRouteTest.IndexedNodeNameAsserter("independent_dn", 0, 128
                );
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new ServerRouteTest.SimpleSQLAsserter();
            for (int i_2 = 0; i_2 < 128; i_2++)
            {
                sqlAsserter.AddExpectSQL(i_2, " select * from `dual`");
            }
            asserter = new ServerRouteTest.RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (RouteResultsetNode node_11 in nodeMap.Values)
            {
                asserter.AssertNode(node_11);
            }
            sql = "/*!cobar:$partitionOperand=(['member_id','NON_EXistence']=[['pavarotti17'],['qaa',4]]), $table='offer'  , $replica=2*/ select * from `dual`";
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(-1L, rrs.GetLimitSize());
            nodeMap = GetNodeMap(rrs, 2);
            nameAsserter = new ServerRouteTest.NodeNameAsserter("offer_dn[123]", "offer_dn[10]"
                );
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new ServerRouteTest.SimpleSQLAsserter();
            sqlAsserter.AddExpectSQL(0, " select * from `dual`").AddExpectSQL(1, " select * from `dual`"
                );
            asserter = new ServerRouteTest.RouteNodeAsserter(nameAsserter, sqlAsserter, new _ReplicaAsserter_1687
                ());
            foreach (RouteResultsetNode node_12 in nodeMap.Values)
            {
                asserter.AssertNode(node_12);
            }
            sql = "/*!cobar:$partitionOperand=(['offer_id','NON_EXistence']=[[123,3],[234,4]]), $table='non_existence'  , $replica=2*/ select * from `dual`";
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(-1L, rrs.GetLimitSize());
            NUnit.Framework.Assert.AreEqual(1, rrs.GetNodes().Length);
            NUnit.Framework.Assert.AreEqual(2, rrs.GetNodes()[0].GetReplicaIndex());
            NUnit.Framework.Assert.AreEqual("cndb_dn", rrs.GetNodes()[0].GetName());
            NUnit.Framework.Assert.AreEqual(" select * from `dual`", rrs.GetNodes()[0].GetStatement
                ());
            sql = "/*!cobar:$partitionOperand=(['offer_id','group_id']=[[123,3],[234,4]]), $table='non_existence'  , $replica=2*/ select * from `dual`";
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(-1L, rrs.GetLimitSize());
            NUnit.Framework.Assert.AreEqual(1, rrs.GetNodes().Length);
            NUnit.Framework.Assert.AreEqual(2, rrs.GetNodes()[0].GetReplicaIndex());
            NUnit.Framework.Assert.AreEqual("cndb_dn", rrs.GetNodes()[0].GetName());
            NUnit.Framework.Assert.AreEqual(" select * from `dual`", rrs.GetNodes()[0].GetStatement
                ());
            schema = schemaMap["dubbo"];
            sql = "/*!cobar: $replica=2*/ select * from `dual`";
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(-1L, rrs.GetLimitSize());
            NUnit.Framework.Assert.AreEqual(1, rrs.GetNodes().Length);
            NUnit.Framework.Assert.AreEqual(2, rrs.GetNodes()[0].GetReplicaIndex());
            NUnit.Framework.Assert.AreEqual("dubbo_dn", rrs.GetNodes()[0].GetName());
            NUnit.Framework.Assert.AreEqual(" select * from `dual`", rrs.GetNodes()[0].GetStatement
                ());
            schema = schemaMap["dubbo"];
            sql = "/*!cobar: $dataNodeId = [ 0.1],$replica=2*/ select * from `dual`";
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(1, rrs.GetNodes().Length);
            NUnit.Framework.Assert.AreEqual(1, rrs.GetNodes()[0].GetReplicaIndex());
            NUnit.Framework.Assert.AreEqual("dubbo_dn", rrs.GetNodes()[0].GetName());
            NUnit.Framework.Assert.AreEqual(" select * from `dual`", rrs.GetNodes()[0].GetStatement
                ());
            schema = schemaMap["dubbo"];
            sql = "/*!cobar: */ select * from `dual`";
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(1, rrs.GetNodes().Length);
            NUnit.Framework.Assert.AreEqual((int)RouteResultsetNode.DefaultReplicaIndex, rrs.
                GetNodes()[0].GetReplicaIndex());
            NUnit.Framework.Assert.AreEqual("dubbo_dn", rrs.GetNodes()[0].GetName());
            NUnit.Framework.Assert.AreEqual(" select * from `dual`", rrs.GetNodes()[0].GetStatement
                ());
        }

        private sealed class _ReplicaAsserter_1468 : ServerRouteTest.ReplicaAsserter
        {
            public _ReplicaAsserter_1468()
            {
            }

            public void AssertReplica(int nodeIndex, int replica)
            {
                if (nodeIndex.Equals(2))
                {
                    NUnit.Framework.Assert.AreEqual(2, replica);
                }
                else
                {
                    NUnit.Framework.Assert.AreEqual(RouteResultsetNode.DefaultReplicaIndex, replica);
                }
            }
        }

        private sealed class _ReplicaAsserter_1492 : ServerRouteTest.ReplicaAsserter
        {
            public _ReplicaAsserter_1492()
            {
            }

            public void AssertReplica(int nodeIndex, int replica)
            {
                if (nodeIndex.Equals(2))
                {
                    NUnit.Framework.Assert.AreEqual(2, replica);
                }
                else
                {
                    NUnit.Framework.Assert.AreEqual(1, replica);
                }
            }
        }

        private sealed class _ReplicaAsserter_1521 : ServerRouteTest.ReplicaAsserter
        {
            public _ReplicaAsserter_1521()
            {
            }

            public void AssertReplica(int nodeIndex, int replica)
            {
                NUnit.Framework.Assert.AreEqual(2, replica);
            }
        }

        private sealed class _ReplicaAsserter_1547 : ServerRouteTest.ReplicaAsserter
        {
            public _ReplicaAsserter_1547()
            {
            }

            public void AssertReplica(int nodeIndex, int replica)
            {
                NUnit.Framework.Assert.AreEqual(2, replica);
            }
        }

        private sealed class _ReplicaAsserter_1565 : ServerRouteTest.ReplicaAsserter
        {
            public _ReplicaAsserter_1565()
            {
            }

            public void AssertReplica(int nodeIndex, int replica)
            {
                NUnit.Framework.Assert.AreEqual(RouteResultsetNode.DefaultReplicaIndex, replica);
            }
        }

        private sealed class _ReplicaAsserter_1585 : ServerRouteTest.ReplicaAsserter
        {
            public _ReplicaAsserter_1585()
            {
            }

            public void AssertReplica(int nodeIndex, int replica)
            {
                NUnit.Framework.Assert.AreEqual(2, replica);
            }
        }

        private sealed class _ReplicaAsserter_1687 : ServerRouteTest.ReplicaAsserter
        {
            public _ReplicaAsserter_1687()
            {
            }

            public void AssertReplica(int nodeIndex, int replica)
            {
                NUnit.Framework.Assert.AreEqual(2, replica);
            }
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestConfigSchema()
        {
            try
            {
                SchemaConfig schema = schemaMap["config"];
                string sql = "select * from offer where offer_id=1";
                ServerRouter.Route(schema, sql, null, null);
                NUnit.Framework.Assert.IsFalse(true);
            }
            catch (Exception)
            {
            }
            try
            {
                SchemaConfig schema = schemaMap["config"];
                string sql = "select * from offer where col11111=1";
                ServerRouter.Route(schema, sql, null, null);
                NUnit.Framework.Assert.IsFalse(true);
            }
            catch (Exception)
            {
            }
            try
            {
                SchemaConfig schema = schemaMap["config"];
                string sql = "select * from offer ";
                ServerRouter.Route(schema, sql, null, null);
                NUnit.Framework.Assert.IsFalse(true);
            }
            catch (Exception)
            {
            }
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestIgnoreSchema()
        {
            SchemaConfig schema = schemaMap["ignoreSchemaTest"];
            string sql = "select * from offer where offer_id=1";
            RouteResultset rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual("cndb_dn", rrs.GetNodes()[0].GetName());
            NUnit.Framework.Assert.AreEqual(sql, rrs.GetNodes()[0].GetStatement());
            sql = "select * from ignoreSchemaTest.offer where ignoreSchemaTest.offer.offer_id=1";
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual("SELECT * FROM offer WHERE offer.offer_id = 1", rrs
                .GetNodes()[0].GetStatement());
            sql = "select * from ignoreSchemaTest2.offer where ignoreSchemaTest2.offer.offer_id=1";
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(sql, rrs.GetNodes()[0].GetStatement());
            sql = "select * from ignoreSchemaTest2.offer a,ignoreSchemaTest.offer b  where ignoreSchemaTest2.offer.offer_id=1";
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual("SELECT * FROM ignoreSchemaTest2.offer AS " + AliasConvert
                ("a") + ", offer AS " + AliasConvert("b") + " WHERE ignoreSchemaTest2.offer.offer_id = 1"
                , rrs.GetNodes()[0].GetStatement());
            schema = schemaMap["ignoreSchemaTest0"];
            sql = "select * from offer where offer_id=1";
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(sql, rrs.GetNodes()[0].GetStatement());
            sql = "select * from ignoreSchemaTest0.offer where ignoreSchemaTest.offer.offer_id=1";
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual("SELECT * FROM offer WHERE ignoreSchemaTest.offer.offer_id = 1"
                , rrs.GetNodes()[0].GetStatement());
            sql = "insert into offer (group_id, offer_id, gmt) values (234,123,now())";
            schema = schemaMap["ignoreSchemaTest0"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(1, rrs.GetNodes().Length);
            NUnit.Framework.Assert.AreEqual(-1l, rrs.GetLimitSize());
            NUnit.Framework.Assert.AreEqual((int)RouteResultsetNode.DefaultReplicaIndex, rrs.
                GetNodes()[0].GetReplicaIndex());
            NUnit.Framework.Assert.AreEqual("offer_dn[44]", rrs.GetNodes()[0].GetName());
            NUnit.Framework.Assert.AreEqual("insert into offer (group_id, offer_id, gmt) values (234,123,now())"
                , rrs.GetNodes()[0].GetStatement());
            sql = "insert into ignoreSchemaTest0.offer (group_id, offer_id, gmt) values (234,123,now())";
            schema = schemaMap["ignoreSchemaTest0"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(1, rrs.GetNodes().Length);
            NUnit.Framework.Assert.AreEqual(-1l, rrs.GetLimitSize());
            NUnit.Framework.Assert.AreEqual((int)RouteResultsetNode.DefaultReplicaIndex, rrs.
                GetNodes()[0].GetReplicaIndex());
            NUnit.Framework.Assert.AreEqual("offer_dn[44]", rrs.GetNodes()[0].GetName());
            NUnit.Framework.Assert.AreEqual("INSERT INTO offer (group_id, offer_id, gmt) VALUES (234, 123, NOW())"
                , rrs.GetNodes()[0].GetStatement());
            sql = "insert into ignoreSchemaTest2.offer (group_id, offer_id, gmt) values (234,123,now())";
            schema = schemaMap["ignoreSchemaTest0"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual("cndb_dn", rrs.GetNodes()[0].GetName());
            NUnit.Framework.Assert.AreEqual(sql, rrs.GetNodes()[0].GetStatement());
            sql = "insert into ignoreSchemaTest2.offer (ignoreSchemaTest0.offer.group_id, offer_id, gmt) values (234,123,now())";
            schema = schemaMap["ignoreSchemaTest0"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual("cndb_dn", rrs.GetNodes()[0].GetName());
            NUnit.Framework.Assert.AreEqual("INSERT INTO ignoreSchemaTest2.offer (offer.group_id, offer_id, gmt) VALUES (234, 123, NOW())"
                , rrs.GetNodes()[0].GetStatement());
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestNonPartitionSQL()
        {
            SchemaConfig schema = schemaMap["cndb"];
            string sql = "  select * from `dual`";
            RouteResultset rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(1, rrs.GetNodes().Length);
            NUnit.Framework.Assert.AreEqual((int)RouteResultsetNode.DefaultReplicaIndex, rrs.
                GetNodes()[0].GetReplicaIndex());
            NUnit.Framework.Assert.AreEqual("cndb_dn", rrs.GetNodes()[0].GetName());
            NUnit.Framework.Assert.AreEqual("  select * from `dual`", rrs.GetNodes()[0].GetStatement
                ());
            schema = schemaMap["dubbo"];
            sql = "  select * from `dual`";
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(1, rrs.GetNodes().Length);
            NUnit.Framework.Assert.AreEqual((int)RouteResultsetNode.DefaultReplicaIndex, rrs.
                GetNodes()[0].GetReplicaIndex());
            NUnit.Framework.Assert.AreEqual("dubbo_dn", rrs.GetNodes()[0].GetName());
            NUnit.Framework.Assert.AreEqual("  select * from `dual`", rrs.GetNodes()[0].GetStatement
                ());
            schema = schemaMap["dubbo"];
            sql = "  select * from dubbo.`dual`";
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(1, rrs.GetNodes().Length);
            NUnit.Framework.Assert.AreEqual((int)RouteResultsetNode.DefaultReplicaIndex, rrs.
                GetNodes()[0].GetReplicaIndex());
            NUnit.Framework.Assert.AreEqual("dubbo_dn", rrs.GetNodes()[0].GetName());
            NUnit.Framework.Assert.AreEqual("  select * from dubbo.`dual`", rrs.GetNodes()[0]
                .GetStatement());
            sql = "SHOW TABLES from db_name like 'solo'";
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(-1L, rrs.GetLimitSize());
            NUnit.Framework.Assert.AreEqual((int)RouteResultsetNode.DefaultReplicaIndex, rrs.
                GetNodes()[0].GetReplicaIndex());
            NUnit.Framework.Assert.AreEqual(1, rrs.GetNodes().Length);
            NUnit.Framework.Assert.AreEqual("dubbo_dn", rrs.GetNodes()[0].GetName());
            NUnit.Framework.Assert.AreEqual("SHOW TABLES from db_name like 'solo'", rrs.GetNodes
                ()[0].GetStatement());
            sql = "desc cndb.offer";
            rrs = ServerRouter.Route(schema, sql, null, null);
            NUnit.Framework.Assert.AreEqual(-1L, rrs.GetLimitSize());
            NUnit.Framework.Assert.AreEqual((int)RouteResultsetNode.DefaultReplicaIndex, rrs.
                GetNodes()[0].GetReplicaIndex());
            NUnit.Framework.Assert.AreEqual(1, rrs.GetNodes().Length);
            NUnit.Framework.Assert.AreEqual("dubbo_dn", rrs.GetNodes()[0].GetName());
            NUnit.Framework.Assert.AreEqual("desc cndb.offer", rrs.GetNodes()[0].GetStatement
                ());
            schema = schemaMap["cndb"];
            sql = "SHOW fulL TaBLES from db_name like 'solo'";
            rrs = ServerRouter.Route(schema, sql, null, null);
            IDictionary<string, RouteResultsetNode> nodeMap = GetNodeMap(rrs, 4);
            ServerRouteTest.NodeNameAsserter nameAsserter = new ServerRouteTest.NodeNameAsserter
                ("detail_dn[0]", "offer_dn[0]", "cndb_dn", "independent_dn[0]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            ServerRouteTest.SimpleSQLAsserter sqlAsserter = new ServerRouteTest.SimpleSQLAsserter
                ();
            sqlAsserter.AddExpectSQL(0, "SHOW FULL TABLES LIKE 'solo'").AddExpectSQL(1, "SHOW FULL TABLES LIKE 'solo'"
                ).AddExpectSQL(2, "SHOW FULL TABLES LIKE 'solo'").AddExpectSQL(3, "SHOW FULL TABLES LIKE 'solo'"
                );
            ServerRouteTest.RouteNodeAsserter asserter = new ServerRouteTest.RouteNodeAsserter
                (nameAsserter, sqlAsserter);
            foreach (RouteResultsetNode node in nodeMap.Values)
            {
                asserter.AssertNode(node);
            }
        }
    }
}
