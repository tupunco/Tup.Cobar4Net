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
using System.IO;
using System.Text;

using NUnit.Framework;

using Tup.Cobar4Net.Config.Loader.Xml;
using Tup.Cobar4Net.Config.Model;
using Tup.Cobar4Net.Config.Util;
using Tup.Cobar4Net.Parser.Ast.Stmt;
using Tup.Cobar4Net.Parser.Ast.Stmt.Dml;
using Tup.Cobar4Net.Parser.Recognizer;
using Tup.Cobar4Net.Route.Config;
using Tup.Cobar4Net.Route.Util;

namespace Tup.Cobar4Net.Route
{
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    [TestFixture(Category = "ServerRouteTest")]
    public class ServerRouteTest : AbstractAliasConvert
    {
        protected internal IDictionary<string, SchemaConfig> schemaMap;

        public ServerRouteTest()
        {
            var schemaFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources/route/schema.xml");
            var ruleFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources/route/rule.xml");
            var schemaLoader = new XmlSchemaLoader(schemaFile, ruleFile);
            try
            {
                RouteRuleInitializer.InitRouteRule(schemaLoader);
            }
            catch (SqlSyntaxErrorException e)
            {
                throw new ConfigException(e);
            }
            catch (Exception ee)
            {
                throw ee;
            }
            schemaMap = schemaLoader.Schemas;
        }

        private static IDictionary<string, RouteResultsetNode> GetNodeMap(RouteResultset
                                                                              rrs, int expectSize)
        {
            var routeNodes = rrs.Nodes;
            Assert.AreEqual(expectSize, routeNodes.Length);
            var nodeMap = new Dictionary<string, RouteResultsetNode>(expectSize);
            for (var i = 0; i < expectSize; i++)
            {
                var routeNode = routeNodes[i];
                nodeMap[routeNode.Name] = routeNode;
            }
            Assert.AreEqual(expectSize, nodeMap.Count);
            return nodeMap;
        }

        #region INodeNameDeconstructor
        private interface INodeNameDeconstructor
        {
            int GetNodeIndex(string name);
        }

        private class NodeNameAsserter : INodeNameDeconstructor
        {
            private string[] _expectNames;

            public NodeNameAsserter()
            {
            }

            public NodeNameAsserter(params string[] expectNames)
            {
                Assert.IsNotNull(expectNames);
                this._expectNames = expectNames;
            }

            public virtual int GetNodeIndex(string name)
            {
                for (var i = 0; i < _expectNames.Length; ++i)
                {
                    if (name.Equals(_expectNames[i]))
                    {
                        return i;
                    }
                }
                throw new NotSupportedException("route node " + name + " dosn't exist!");
            }

            protected internal virtual void SetNames(string[] expectNames)
            {
                Assert.IsNotNull(expectNames);
                this._expectNames = expectNames;
            }

            public virtual void AssertRouteNodeNames(ICollection<string> nodeNames)
            {
                Assert.IsNotNull(nodeNames);
                Assert.AreEqual(_expectNames.Length, nodeNames.Count);
                foreach (var name in _expectNames)
                {
                    Assert.IsTrue(nodeNames.Contains(name));
                }
            }
        }

        private class IndexedNodeNameAsserter : NodeNameAsserter
        {
            /// <param name="from">included</param>
            /// <param name="to">excluded</param>
            public IndexedNodeNameAsserter(string prefix, int from, int to)
            {
                var names = new string[to - from];
                for (var i = 0; i < names.Length; ++i)
                {
                    names[i] = prefix + "[" + (i + from) + "]";
                }
                SetNames(names);
            }
        }
        #endregion

        #region Asserter
        private interface IReplicaAsserter
        {
            void AssertReplica(int nodeIndex, int replica);
        }

        private class RouteNodeAsserter
        {
            private readonly INodeNameDeconstructor _deconstructor;

            private readonly IReplicaAsserter _replicaAsserter;

            private readonly ISqlAsserter _sqlAsserter;

            public RouteNodeAsserter(INodeNameDeconstructor deconstructor, ISqlAsserter
                                                                              sqlAsserter)
            {
                this._deconstructor = deconstructor;
                this._sqlAsserter = sqlAsserter;
                _replicaAsserter = new _ReplicaAsserter_266();
            }

            public RouteNodeAsserter(INodeNameDeconstructor deconstructor, ISqlAsserter
                                                                              sqlAsserter,
                                     IReplicaAsserter replicaAsserter)
            {
                this._deconstructor = deconstructor;
                this._sqlAsserter = sqlAsserter;
                this._replicaAsserter = replicaAsserter;
            }

            /// <exception cref="System.Exception" />
            public virtual void AssertNode(RouteResultsetNode node)
            {
                var nodeIndex = _deconstructor.GetNodeIndex(node.Name);
                _sqlAsserter.AssertSql(node.Statement, nodeIndex);
                _replicaAsserter.AssertReplica(nodeIndex, node.ReplicaIndex);
            }

            private sealed class _ReplicaAsserter_266 : IReplicaAsserter
            {
                public void AssertReplica(int nodeIndex, int replica)
                {
                    Assert.AreEqual(RouteResultsetNode.DefaultReplicaIndex, replica);
                }
            }
        }

        private interface ISqlAsserter
        {
            /// <exception cref="System.Exception" />
            void AssertSql(string sql, int nodeIndex);
        }

        private class SimpleSqlAsserter : ISqlAsserter
        {
            private readonly IDictionary<int, ICollection<string>> map = new Dictionary<int, ICollection<string>>();

            /// <exception cref="System.Exception" />
            public virtual void AssertSql(string sql, int nodeIndex)
            {
                Assert.IsNotNull(map[nodeIndex]);
                Assert.IsTrue(map[nodeIndex].Contains(sql));
            }

            public virtual SimpleSqlAsserter AddExpectSql(int nodeIndex, string sql)
            {
                var set = map.GetValue(nodeIndex);
                if (set == null)
                {
                    set = new HashSet<string>();
                    map[nodeIndex] = set;
                }
                set.Add(sql);
                return this;
            }

            public virtual SimpleSqlAsserter AddExpectSql(int nodeIndex, params string[] sql)
            {
                foreach (var s in sql)
                {
                    AddExpectSql(nodeIndex, s);
                }
                return this;
            }

            public virtual SimpleSqlAsserter AddExpectSql(int nodeIndex,
                                                          string prefix,
                                                          PermutationUtil.PermutationGenerator pg,
                                                          string suffix)
            {
                var ss = pg.PermutateSql();
                foreach (var s in ss)
                {
                    AddExpectSql(nodeIndex, prefix + s + suffix);
                }
                return this;
            }
        }

        private abstract class ParseredSqlAsserter : ISqlAsserter
        {
            /// <exception cref="System.Exception" />
            public virtual void AssertSql(string sql, int nodeIndex)
            {
                var stmt = SqlParserDelegate.Parse(sql);
                AssertAst(stmt, nodeIndex);
            }

            protected internal abstract void AssertAst(ISqlStatement stmt, int nodeIndex);
        }

        private sealed class _ParseredSQLAsserter_351 : ParseredSqlAsserter
        {
            protected internal override void AssertAst(ISqlStatement stmt, int nodeIndex)
            {
                var insert = (DmlInsertStatement)stmt;
                var rows = insert.RowList;
                Assert.IsNotNull(rows);
                Assert.AreEqual(8, rows.Count);
                IList<int> vals = new List<int>(8);
                foreach (var row in rows)
                {
                    var val = (int)(Number)row.RowExprList[0].Evaluation(null);
                    vals.Add(val);
                }
                Assert.AreEqual(8, vals.Count);
                for (var i = 8*nodeIndex; i < 8*nodeIndex + 8; ++i)
                {
                    Assert.IsTrue(vals.Contains(i));
                }
            }
        }

        private sealed class _ReplicaAsserter_1468 : IReplicaAsserter
        {
            public void AssertReplica(int nodeIndex, int replica)
            {
                if (nodeIndex.Equals(2))
                {
                    Assert.AreEqual(2, replica);
                }
                else
                {
                    Assert.AreEqual(RouteResultsetNode.DefaultReplicaIndex, replica);
                }
            }
        }

        private sealed class _ReplicaAsserter_1492 : IReplicaAsserter
        {
            public void AssertReplica(int nodeIndex, int replica)
            {
                if (nodeIndex.Equals(2))
                {
                    Assert.AreEqual(2, replica);
                }
                else
                {
                    Assert.AreEqual(1, replica);
                }
            }
        }

        private sealed class _ReplicaAsserter_1521 : IReplicaAsserter
        {
            public void AssertReplica(int nodeIndex, int replica)
            {
                Assert.AreEqual(2, replica);
            }
        }

        private sealed class _ReplicaAsserter_1547 : IReplicaAsserter
        {
            public void AssertReplica(int nodeIndex, int replica)
            {
                Assert.AreEqual(2, replica);
            }
        }

        private sealed class _ReplicaAsserter_1565 : IReplicaAsserter
        {
            public void AssertReplica(int nodeIndex, int replica)
            {
                Assert.AreEqual(RouteResultsetNode.DefaultReplicaIndex, replica);
            }
        }

        private sealed class _ReplicaAsserter_1585 : IReplicaAsserter
        {
            public void AssertReplica(int nodeIndex, int replica)
            {
                Assert.AreEqual(2, replica);
            }
        }

        private sealed class _ReplicaAsserter_1687 : IReplicaAsserter
        {
            public void AssertReplica(int nodeIndex, int replica)
            {
                Assert.AreEqual(2, replica);
            }
        }
        #endregion

        /// <exception cref="System.Exception" />
        [Test]
        public virtual void TestBackquotedColumn()
        {
            var schema = schemaMap["cndb"];
            var sql = "select * from wp_image where `seLect`='pavarotti17' ";
            var rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(-1L, rrs.LimitSize);
            Assert.AreEqual(1, rrs.Nodes.Length);
            Assert.AreEqual(RouteResultsetNode.DefaultReplicaIndex, rrs.Nodes[0].ReplicaIndex);
            Assert.AreEqual("offer_dn[123]", rrs.Nodes[0].Name);
            Assert.AreEqual("select * from wp_image where `seLect`='pavarotti17' ", rrs.Nodes[0].Statement);
        }

        /// <exception cref="System.Exception" />
        [Test]
        public virtual void TestCobarHint()
        {
            var schema = schemaMap["cndb"];
            var sql = "  /*!cobar: $dataNodeId=2.1, $table='offer'*/ select * from `dual`";
            var rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(-1L, rrs.LimitSize);
            Assert.AreEqual(1, rrs.Nodes.Length);
            Assert.AreEqual(1, rrs.Nodes[0].ReplicaIndex);
            Assert.AreEqual("offer_dn[2]", rrs.Nodes[0].Name);
            Assert.AreEqual(" select * from `dual`", rrs.Nodes[0].Statement);
            sql = "/*!cobar: $dataNodeId=2.1, $table='offer', $replica =2*/ select * from `dual`";
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(-1L, rrs.LimitSize);
            Assert.AreEqual(1, rrs.Nodes.Length);
            Assert.AreEqual(1, rrs.Nodes[0].ReplicaIndex);
            Assert.AreEqual("offer_dn[2]", rrs.Nodes[0].Name);
            Assert.AreEqual(" select * from `dual`", rrs.Nodes[0].Statement);
            sql = "/*!cobar: $dataNodeId   = [ 1,2,5.2]  , $table =  'offer'   */ select * from `dual`";
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(-1L, rrs.LimitSize);
            var nodeMap = GetNodeMap(rrs, 3);
            var nameAsserter = new NodeNameAsserter("offer_dn[1]", "offer_dn[2]", "offer_dn[5]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            var sqlAsserter = new SimpleSqlAsserter();
            sqlAsserter.AddExpectSql(0, " select * from `dual`")
                       .AddExpectSql(1, " select * from `dual`")
                       .AddExpectSql(2, " select * from `dual`");
            var asserter = new RouteNodeAsserter(nameAsserter, sqlAsserter, new _ReplicaAsserter_1468());
            foreach (var node in nodeMap.Values)
            {
                asserter.AssertNode(node);
            }
            sql = "/*!cobar: $dataNodeId   = [ 1,2,5.2]  , $table =  'offer'  , $replica =1 */ select * from `dual`";
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(-1L, rrs.LimitSize);
            Assert.AreEqual(3, rrs.Nodes.Length);
            nodeMap = GetNodeMap(rrs, 3);
            nameAsserter = new NodeNameAsserter("offer_dn[1]", "offer_dn[2]",
                "offer_dn[5]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new SimpleSqlAsserter();
            sqlAsserter.AddExpectSql(0, " select * from `dual`")
                       .AddExpectSql(1, " select * from `dual`")
                       .AddExpectSql(2, " select * from `dual`");
            asserter = new RouteNodeAsserter(nameAsserter, sqlAsserter, new _ReplicaAsserter_1492());
            foreach (var node_1 in nodeMap.Values)
            {
                asserter.AssertNode(node_1);
            }
            sql = "/*!cobar: $partitionOperand=( 'member_id' = 'pavarotti17'), $table='offer'*/ select * from `dual`";
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(-1L, rrs.LimitSize);
            Assert.AreEqual(1, rrs.Nodes.Length);
            Assert.AreEqual(RouteResultsetNode.DefaultReplicaIndex, rrs.Nodes[0].ReplicaIndex);
            Assert.AreEqual("offer_dn[123]", rrs.Nodes[0].Name);
            Assert.AreEqual(" select * from `dual`", rrs.Nodes[0].Statement);
            sql =
                "/*!cobar:$partitionOperand =   ( 'member_id' = ['pavarotti17'  ,   'qaa' ]  ), $table='offer'  , $replica =  2*/  select * from `dual`";
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(-1L, rrs.LimitSize);
            nodeMap = GetNodeMap(rrs, 2);
            nameAsserter = new NodeNameAsserter("offer_dn[123]", "offer_dn[10]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new SimpleSqlAsserter();
            sqlAsserter.AddExpectSql(0, "  select * from `dual`").AddExpectSql(1, "  select * from `dual`");
            asserter = new RouteNodeAsserter(nameAsserter, sqlAsserter, new _ReplicaAsserter_1521());
            foreach (var node_2 in nodeMap.Values)
            {
                asserter.AssertNode(node_2);
            }
            sql =
                "/*!cobar:$partitionOperand = ( ['group_id','offer_id'] = [234,4]), $table='offer'*/ select * from `dual`";
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(-1L, rrs.LimitSize);
            Assert.AreEqual(1, rrs.Nodes.Length);
            Assert.AreEqual(RouteResultsetNode.DefaultReplicaIndex, rrs.Nodes[0].ReplicaIndex);
            Assert.AreEqual("offer_dn[29]", rrs.Nodes[0].Name);
            Assert.AreEqual(" select * from `dual`", rrs.Nodes[0].Statement);
            sql =
                "/*!cobar:$partitionOperand=(['offer_id','group_id']=[[123,3],[234,4]]), $table='offer'  , $replica =2*/ select * from `dual`";
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(-1L, rrs.LimitSize);
            nodeMap = GetNodeMap(rrs, 2);
            nameAsserter = new NodeNameAsserter("offer_dn[29]", "offer_dn[15]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new SimpleSqlAsserter();
            sqlAsserter.AddExpectSql(0, " select * from `dual`").AddExpectSql(1, " select * from `dual`");
            asserter = new RouteNodeAsserter(nameAsserter, sqlAsserter, new _ReplicaAsserter_1547());
            foreach (var node_3 in nodeMap.Values)
            {
                asserter.AssertNode(node_3);
            }
            sql =
                "/*!cobar:$partitionOperand=(['group_id','offer_id']=[[123,3], [ 234,4 ] ]), $table='offer'  */ select * from `dual`";
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(-1L, rrs.LimitSize);
            nodeMap = GetNodeMap(rrs, 2);
            nameAsserter = new NodeNameAsserter("offer_dn[29]", "offer_dn[15]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new SimpleSqlAsserter();
            sqlAsserter.AddExpectSql(0, " select * from `dual`").AddExpectSql(1, " select * from `dual`");
            asserter = new RouteNodeAsserter(nameAsserter, sqlAsserter, new _ReplicaAsserter_1565());
            foreach (var node_4 in nodeMap.Values)
            {
                asserter.AssertNode(node_4);
            }
            sql =
                "/*!cobar:$partitionOperand=(['offer_id','NON_EXistence']=[[123,3],[234,4]]), $table='offer'  , $replica =2*/ select * from `dual`";
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(-1L, rrs.LimitSize);
            nodeMap = GetNodeMap(rrs, 128);
            nameAsserter = new IndexedNodeNameAsserter("offer_dn", 0, 128);
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new SimpleSqlAsserter();
            for (var i = 0; i < 128; i++)
            {
                sqlAsserter.AddExpectSql(i, " select * from `dual`");
            }
            asserter = new RouteNodeAsserter(nameAsserter, sqlAsserter, new _ReplicaAsserter_1585());
            foreach (var node_5 in nodeMap.Values)
            {
                asserter.AssertNode(node_5);
            }
            sql = "/*!cobar:  $dataNodeId   = 1  ,$table =  'wp_image'*/ select * from `dual`";
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(-1L, rrs.LimitSize);
            nodeMap = GetNodeMap(rrs, 1);
            nameAsserter = new NodeNameAsserter("offer_dn[1]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new SimpleSqlAsserter();
            sqlAsserter.AddExpectSql(0, " select * from `dual`");
            asserter = new RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (var node_6 in nodeMap.Values)
            {
                asserter.AssertNode(node_6);
            }
            sql = "/*!cobar:  $dataNodeId   = [0,3]  ,$table =  'wp_image'*/ select * from `dual`";
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(-1L, rrs.LimitSize);
            nodeMap = GetNodeMap(rrs, 2);
            nameAsserter = new NodeNameAsserter("offer_dn[0]", "offer_dn[3]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new SimpleSqlAsserter();
            sqlAsserter.AddExpectSql(0, " select * from `dual`").AddExpectSql(1, " select * from `dual`");
            asserter = new RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (var node_7 in nodeMap.Values)
            {
                asserter.AssertNode(node_7);
            }
            sql = "/*!cobar:  $table =  'wp_image'*/ select * from `dual`";
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(-1L, rrs.LimitSize);
            nodeMap = GetNodeMap(rrs, 128);
            nameAsserter = new IndexedNodeNameAsserter("offer_dn", 0, 128);
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new SimpleSqlAsserter();
            for (var i_1 = 0; i_1 < 128; i_1++)
            {
                sqlAsserter.AddExpectSql(i_1, " select * from `dual`");
            }
            asserter = new RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (var node_8 in nodeMap.Values)
            {
                asserter.AssertNode(node_8);
            }
            sql = "/*!cobar:  $dataNodeId   = 0  ,$table =  'independent'*/ select * from `dual`";
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(-1L, rrs.LimitSize);
            nodeMap = GetNodeMap(rrs, 1);
            nameAsserter = new NodeNameAsserter("independent_dn[0]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new SimpleSqlAsserter();
            sqlAsserter.AddExpectSql(0, " select * from `dual`");
            asserter = new RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (var node_9 in nodeMap.Values)
            {
                asserter.AssertNode(node_9);
            }
            sql = "/*!cobar:  $dataNodeId   = [ 1,2,5]  ,$table =  'independent'*/ select * from `dual`";
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(-1L, rrs.LimitSize);
            nodeMap = GetNodeMap(rrs, 3);
            nameAsserter = new NodeNameAsserter("independent_dn[1]", "independent_dn[2]", "independent_dn[5]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new SimpleSqlAsserter();
            sqlAsserter.AddExpectSql(0, " select * from `dual`")
                       .AddExpectSql(1, " select * from `dual`")
                       .AddExpectSql(2, " select * from `dual`");
            asserter = new RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (var node_10 in nodeMap.Values)
            {
                asserter.AssertNode(node_10);
            }
            sql = "/*!cobar:  $table =  'independent'*/ select * from `dual`";
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(-1L, rrs.LimitSize);
            nodeMap = GetNodeMap(rrs, 128);
            nameAsserter = new IndexedNodeNameAsserter("independent_dn", 0, 128);
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new SimpleSqlAsserter();
            for (var i_2 = 0; i_2 < 128; i_2++)
            {
                sqlAsserter.AddExpectSql(i_2, " select * from `dual`");
            }
            asserter = new RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (var node_11 in nodeMap.Values)
            {
                asserter.AssertNode(node_11);
            }
            sql =
                "/*!cobar:$partitionOperand=(['member_id','NON_EXistence']=[['pavarotti17'],['qaa',4]]), $table='offer'  , $replica=2*/ select * from `dual`";
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(-1L, rrs.LimitSize);
            nodeMap = GetNodeMap(rrs, 2);
            nameAsserter = new NodeNameAsserter("offer_dn[123]", "offer_dn[10]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new SimpleSqlAsserter();
            sqlAsserter.AddExpectSql(0, " select * from `dual`").AddExpectSql(1, " select * from `dual`");
            asserter = new RouteNodeAsserter(nameAsserter, sqlAsserter, new _ReplicaAsserter_1687());
            foreach (var node_12 in nodeMap.Values)
            {
                asserter.AssertNode(node_12);
            }
            sql =
                "/*!cobar:$partitionOperand=(['offer_id','NON_EXistence']=[[123,3],[234,4]]), $table='non_existence'  , $replica=2*/ select * from `dual`";
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(-1L, rrs.LimitSize);
            Assert.AreEqual(1, rrs.Nodes.Length);
            Assert.AreEqual(2, rrs.Nodes[0].ReplicaIndex);
            Assert.AreEqual("cndb_dn", rrs.Nodes[0].Name);
            Assert.AreEqual(" select * from `dual`", rrs.Nodes[0].Statement);
            sql =
                "/*!cobar:$partitionOperand=(['offer_id','group_id']=[[123,3],[234,4]]), $table='non_existence'  , $replica=2*/ select * from `dual`";
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(-1L, rrs.LimitSize);
            Assert.AreEqual(1, rrs.Nodes.Length);
            Assert.AreEqual(2, rrs.Nodes[0].ReplicaIndex);
            Assert.AreEqual("cndb_dn", rrs.Nodes[0].Name);
            Assert.AreEqual(" select * from `dual`", rrs.Nodes[0].Statement);
            schema = schemaMap["dubbo"];
            sql = "/*!cobar: $replica=2*/ select * from `dual`";
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(-1L, rrs.LimitSize);
            Assert.AreEqual(1, rrs.Nodes.Length);
            Assert.AreEqual(2, rrs.Nodes[0].ReplicaIndex);
            Assert.AreEqual("dubbo_dn", rrs.Nodes[0].Name);
            Assert.AreEqual(" select * from `dual`", rrs.Nodes[0].Statement);
            schema = schemaMap["dubbo"];
            sql = "/*!cobar: $dataNodeId = [ 0.1],$replica=2*/ select * from `dual`";
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(1, rrs.Nodes.Length);
            Assert.AreEqual(1, rrs.Nodes[0].ReplicaIndex);
            Assert.AreEqual("dubbo_dn", rrs.Nodes[0].Name);
            Assert.AreEqual(" select * from `dual`", rrs.Nodes[0].Statement);
            schema = schemaMap["dubbo"];
            sql = "/*!cobar: */ select * from `dual`";
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(1, rrs.Nodes.Length);
            Assert.AreEqual(RouteResultsetNode.DefaultReplicaIndex, rrs.Nodes[0].ReplicaIndex);
            Assert.AreEqual("dubbo_dn", rrs.Nodes[0].Name);
            Assert.AreEqual(" select * from `dual`", rrs.Nodes[0].Statement);
        }

        /// <exception cref="System.Exception" />
        [Test]
        public virtual void TestConfigSchema()
        {
            try
            {
                var schema = schemaMap["config"];
                var sql = "select * from offer where offer_id=1";
                ServerRouter.Route(schema, sql, null, null);
                Assert.IsFalse(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            try
            {
                var schema = schemaMap["config"];
                var sql = "select * from offer where col11111=1";
                ServerRouter.Route(schema, sql, null, null);
                Assert.IsFalse(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            try
            {
                var schema = schemaMap["config"];
                var sql = "select * from offer ";
                ServerRouter.Route(schema, sql, null, null);
                Assert.IsFalse(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        /// <exception cref="System.Exception" />
        [Test]
        public virtual void TestDimension2Route()
        {
            var schema = schemaMap["cndb"];
            var sql = "select * from product_visit where member_id='pavarotti17' and product_id=2345";
            var rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(-1L, rrs.LimitSize);
            Assert.AreEqual(1, rrs.Nodes.Length);
            Assert.AreEqual(RouteResultsetNode.DefaultReplicaIndex, rrs.Nodes[0].ReplicaIndex);
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual("offer_dn[9]", rrs.Nodes[0].Name);
            Assert.AreEqual("select * from product_visit where member_id='pavarotti17' and product_id=2345", rrs.Nodes[0].Statement);
            sql = "select * from product_visit where member_id='pavarotti17' ";
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(-1L, rrs.LimitSize);
            var nodeMap = GetNodeMap(rrs, 8);
            var nameAsserter = new NodeNameAsserter("offer_dn[25]", "offer_dn[17]", "offer_dn[9]", "offer_dn[1]",
                "offer_dn[29]", "offer_dn[21]", "offer_dn[5]", "offer_dn[13]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            var sqlAsserter = new SimpleSqlAsserter();
            sqlAsserter.AddExpectSql(0, "SELECT * FROM product_visit WHERE member_id = 'pavarotti17'")
                       .AddExpectSql(1, "SELECT * FROM product_visit WHERE member_id = 'pavarotti17'")
                       .AddExpectSql(2, "SELECT * FROM product_visit WHERE member_id = 'pavarotti17'").
                        AddExpectSql(3, "SELECT * FROM product_visit WHERE member_id = 'pavarotti17'")
                       .AddExpectSql(4, "SELECT * FROM product_visit WHERE member_id = 'pavarotti17'")
                       .AddExpectSql(
                           5, "SELECT * FROM product_visit WHERE member_id = 'pavarotti17'").AddExpectSql(6, "SELECT * FROM product_visit WHERE member_id = 'pavarotti17'").AddExpectSql(7,
                                   "SELECT * FROM product_visit WHERE member_id = 'pavarotti17'");
            var asserter = new RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (var node in nodeMap.Values)
            {
                asserter.AssertNode(node);
            }
            sql = "select * from product_visit where member_id='abc' ";
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(-1L, rrs.LimitSize);
            nodeMap = GetNodeMap(rrs, 8);
            nameAsserter = new NodeNameAsserter("offer_dn[0]", "offer_dn[4]",
                "offer_dn[8]", "offer_dn[12]", "offer_dn[16]", "offer_dn[20]", "offer_dn[24]", "offer_dn[28]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new SimpleSqlAsserter();
            sqlAsserter.AddExpectSql(0, "SELECT * FROM product_visit WHERE member_id = 'abc'")
                       .AddExpectSql(1, "SELECT * FROM product_visit WHERE member_id = 'abc'")
                       .AddExpectSql(2, "SELECT * FROM product_visit WHERE member_id = 'abc'")
                       .AddExpectSql(3, "SELECT * FROM product_visit WHERE member_id = 'abc'")
                       .AddExpectSql(4, "SELECT * FROM product_visit WHERE member_id = 'abc'")
                       .AddExpectSql(5, "SELECT * FROM product_visit WHERE member_id = 'abc'")
                       .AddExpectSql(6, "SELECT * FROM product_visit WHERE member_id = 'abc'")
                       .AddExpectSql(7, "SELECT * FROM product_visit WHERE member_id = 'abc'");
            asserter = new RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (var node_1 in nodeMap.Values)
            {
                asserter.AssertNode(node_1);
            }
            sql = "delete from product_visit where member_id='pavarotti17' or Member_id between 'abc' and 'abc'";
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(-1L, rrs.LimitSize);
            nodeMap = GetNodeMap(rrs, 16);
            nameAsserter = new NodeNameAsserter("offer_dn[0]", "offer_dn[4]",
                "offer_dn[8]", "offer_dn[12]", "offer_dn[16]", "offer_dn[20]", "offer_dn[24]", "offer_dn[28]", "offer_dn[1]", "offer_dn[5]", "offer_dn[9]", "offer_dn[13]", "offer_dn[17]", "offer_dn[21]", "offer_dn[25]", "offer_dn[29]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new SimpleSqlAsserter();
            sqlAsserter.AddExpectSql(0, "DELETE FROM product_visit WHERE FALSE OR Member_id BETWEEN 'abc' AND 'abc'")
                       .AddExpectSql(1, "DELETE FROM product_visit WHERE FALSE OR Member_id BETWEEN 'abc' AND 'abc'")
                       .AddExpectSql(2, "DELETE FROM product_visit WHERE FALSE OR Member_id BETWEEN 'abc' AND 'abc'")
                       .AddExpectSql(3, "DELETE FROM product_visit WHERE FALSE OR Member_id BETWEEN 'abc' AND 'abc'")
                       .AddExpectSql(4, "DELETE FROM product_visit WHERE FALSE OR Member_id BETWEEN 'abc' AND 'abc'")
                       .AddExpectSql(5, "DELETE FROM product_visit WHERE FALSE OR Member_id BETWEEN 'abc' AND 'abc'")
                       .AddExpectSql(6, "DELETE FROM product_visit WHERE FALSE OR Member_id BETWEEN 'abc' AND 'abc'")
                       .AddExpectSql(7, "DELETE FROM product_visit WHERE FALSE OR Member_id BETWEEN 'abc' AND 'abc'")
                       .AddExpectSql(8, "DELETE FROM product_visit WHERE member_id = 'pavarotti17' OR FALSE")
                       .AddExpectSql(9, "DELETE FROM product_visit WHERE member_id = 'pavarotti17' OR FALSE")
                       .AddExpectSql(10, "DELETE FROM product_visit WHERE member_id = 'pavarotti17' OR FALSE")
                       .AddExpectSql(11, "DELETE FROM product_visit WHERE member_id = 'pavarotti17' OR FALSE")
                       .AddExpectSql(12, "DELETE FROM product_visit WHERE member_id = 'pavarotti17' OR FALSE")
                       .AddExpectSql(13, "DELETE FROM product_visit WHERE member_id = 'pavarotti17' OR FALSE")
                       .AddExpectSql(14, "DELETE FROM product_visit WHERE member_id = 'pavarotti17' OR FALSE")
                       .AddExpectSql(15, "DELETE FROM product_visit WHERE member_id = 'pavarotti17' OR FALSE");
            asserter = new RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (var node_2 in nodeMap.Values)
            {
                asserter.AssertNode(node_2);
            }
            sql = "select * from product_visit where  product_id=2345 ";
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(-1L, rrs.LimitSize);
            nodeMap = GetNodeMap(rrs, 4);
            nameAsserter = new NodeNameAsserter("offer_dn[8]", "offer_dn[9]",
                "offer_dn[10]", "offer_dn[11]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new SimpleSqlAsserter();
            sqlAsserter.AddExpectSql(0, "SELECT * FROM product_visit WHERE product_id = 2345")
                       .AddExpectSql(1, "SELECT * FROM product_visit WHERE product_id = 2345")
                       .AddExpectSql(2, "SELECT * FROM product_visit WHERE product_id = 2345")
                       .AddExpectSql(3, "SELECT * FROM product_visit WHERE product_id = 2345");
            asserter = new RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (var node_3 in nodeMap.Values)
            {
                asserter.AssertNode(node_3);
            }
            sql = "select * from product_visit where  product_id=1234 ";
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(-1L, rrs.LimitSize);
            nodeMap = GetNodeMap(rrs, 4);
            nameAsserter = new NodeNameAsserter("offer_dn[4]", "offer_dn[5]",
                "offer_dn[6]", "offer_dn[7]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new SimpleSqlAsserter();
            sqlAsserter.AddExpectSql(0, "SELECT * FROM product_visit WHERE product_id = 1234")
                       .AddExpectSql(1, "SELECT * FROM product_visit WHERE product_id = 1234")
                       .AddExpectSql(2, "SELECT * FROM product_visit WHERE product_id = 1234")
                       .AddExpectSql(3, "SELECT * FROM product_visit WHERE product_id = 1234");
            asserter = new RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (var node_4 in nodeMap.Values)
            {
                asserter.AssertNode(node_4);
            }
            sql = "select * from product_visit where  product_id=1234 or product_id=2345 ";
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(-1L, rrs.LimitSize);
            nodeMap = GetNodeMap(rrs, 8);
            nameAsserter = new NodeNameAsserter("offer_dn[4]", "offer_dn[5]",
                "offer_dn[6]", "offer_dn[7]", "offer_dn[8]", "offer_dn[9]", "offer_dn[10]", "offer_dn[11]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new SimpleSqlAsserter();
            sqlAsserter.AddExpectSql(0, "SELECT * FROM product_visit WHERE product_id = 1234 OR FALSE")
                       .AddExpectSql(1, "SELECT * FROM product_visit WHERE product_id = 1234 OR FALSE")
                       .AddExpectSql(2, "SELECT * FROM product_visit WHERE product_id = 1234 OR FALSE")
                       .AddExpectSql(3, "SELECT * FROM product_visit WHERE product_id = 1234 OR FALSE")
                       .AddExpectSql(4, "SELECT * FROM product_visit WHERE FALSE OR product_id = 2345")
                       .AddExpectSql(5, "SELECT * FROM product_visit WHERE FALSE OR product_id = 2345")
                       .AddExpectSql(6, "SELECT * FROM product_visit WHERE FALSE OR product_id = 2345")
                       .AddExpectSql(7, "SELECT * FROM product_visit WHERE FALSE OR product_id = 2345");
            asserter = new RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (var node_5 in nodeMap.Values)
            {
                asserter.AssertNode(node_5);
            }
            sql = "select * from product_visit where  product_id in (1234,2345) ";
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(-1L, rrs.LimitSize);
            nodeMap = GetNodeMap(rrs, 8);
            nameAsserter = new NodeNameAsserter("offer_dn[4]", "offer_dn[5]",
                "offer_dn[6]", "offer_dn[7]", "offer_dn[8]", "offer_dn[9]", "offer_dn[10]", "offer_dn[11]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new SimpleSqlAsserter();
            sqlAsserter.AddExpectSql(0, "SELECT * FROM product_visit WHERE product_id IN (1234)")
                       .AddExpectSql(1, "SELECT * FROM product_visit WHERE product_id IN (1234)")
                       .AddExpectSql(2, "SELECT * FROM product_visit WHERE product_id IN (1234)")
                       .AddExpectSql(3, "SELECT * FROM product_visit WHERE product_id IN (1234)")
                       .AddExpectSql(4, "SELECT * FROM product_visit WHERE product_id IN (2345)")
                       .AddExpectSql(5, "SELECT * FROM product_visit WHERE product_id IN (2345)")
                       .AddExpectSql(6, "SELECT * FROM product_visit WHERE product_id IN (2345)")
                       .AddExpectSql(7, "SELECT * FROM product_visit WHERE product_id IN (2345)");
            asserter = new RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (var node_6 in nodeMap.Values)
            {
                asserter.AssertNode(node_6);
            }
        }

        /// <exception cref="System.Exception" />
        [Test]
        public virtual void TestDuplicatePartitionKey()
        {
            var sql =
                "select * from offer.wp_image where member_id in ('pavarotti17', 'qaa') or offer.wp_image.member_id='1qq' or member_id='1qq'";
            var schema = schemaMap["cndb"];
            var rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(-1L, rrs.LimitSize);
            var nodeMap = GetNodeMap(rrs, 3);
            var nameAsserter = new NodeNameAsserter("offer_dn[123]", "offer_dn[10]", "offer_dn[66]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            var sqlAsserter = new SimpleSqlAsserter();
            sqlAsserter.AddExpectSql(0, "SELECT * FROM wp_image WHERE member_id IN ('pavarotti17') OR FALSE OR FALSE")
                       .AddExpectSql(1, "SELECT * FROM wp_image WHERE member_id IN ('qaa') OR FALSE OR FALSE")
                       .AddExpectSql(2,
                           "SELECT * FROM wp_image WHERE FALSE OR wp_image.member_id = '1qq' OR member_id = '1qq'");
            var asserter = new RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (var node in nodeMap.Values)
            {
                asserter.AssertNode(node);
            }
            sql =
                "insert into wp_image (id, member_id, gmt) values (1,'pavarotti17',now()),(2,'pavarotti17',now()),(3,'qaa',now())";
            schema = schemaMap["cndb"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(-1L, rrs.LimitSize);
            nodeMap = GetNodeMap(rrs, 2);
            nameAsserter = new NodeNameAsserter("offer_dn[123]", "offer_dn[10]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new SimpleSqlAsserter();
            sqlAsserter.AddExpectSql(0,
                "INSERT INTO wp_image (id, member_id, gmt) VALUES (2, 'pavarotti17', NOW()), (1, 'pavarotti17', NOW())"
                ,
                "INSERT INTO wp_image (id, member_id, gmt) VALUES (1, 'pavarotti17', NOW()), (2, 'pavarotti17', NOW())")
                       .AddExpectSql(1, "INSERT INTO wp_image (id, member_id, gmt) VALUES (3, 'qaa', NOW())");
            asserter = new RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (var node_1 in nodeMap.Values)
            {
                asserter.AssertNode(node_1);
            }
            sql =
                "select * from offer.wp_image where member_id in ('pavarotti17','pavarotti17', 'qaa') or offer.wp_image.member_id='pavarotti17'";
            schema = schemaMap["cndb"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(-1L, rrs.LimitSize);
            Assert.AreEqual(2, rrs.Nodes.Length);
            nodeMap = GetNodeMap(rrs, 2);
            nameAsserter = new NodeNameAsserter("offer_dn[123]", "offer_dn[10]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new SimpleSqlAsserter();
            sqlAsserter.AddExpectSql(0,
                "SELECT * FROM wp_image WHERE member_id IN ('pavarotti17', 'pavarotti17') OR wp_image.member_id = 'pavarotti17'")
                       .AddExpectSql(1, "SELECT * FROM wp_image WHERE member_id IN ('qaa') OR FALSE");
            asserter = new RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (var node_2 in nodeMap.Values)
            {
                asserter.AssertNode(node_2);
            }
            sql =
                "select * from offer.`wp_image` where `member_id` in ('pavarotti17','pavarotti17', 'qaa') or member_id in ('pavarotti17','1qq','pavarotti17') or offer.wp_image.member_id='pavarotti17'";
            schema = schemaMap["cndb"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(-1L, rrs.LimitSize);
            Assert.AreEqual(3, rrs.Nodes.Length);
            nodeMap = GetNodeMap(rrs, 3);
            nameAsserter = new NodeNameAsserter("offer_dn[123]", "offer_dn[10]", "offer_dn[66]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new SimpleSqlAsserter();
            sqlAsserter.AddExpectSql(0,
                "SELECT * FROM `wp_image` WHERE `member_id` IN ('pavarotti17', 'pavarotti17') OR member_id IN ('pavarotti17', 'pavarotti17') OR wp_image.member_id = 'pavarotti17'")
                       .AddExpectSql(1, "SELECT * FROM `wp_image` WHERE `member_id` IN ('qaa') OR FALSE OR FALSE")
                       .AddExpectSql(2, "SELECT * FROM `wp_image` WHERE FALSE OR member_id IN ('1qq') OR FALSE");
            asserter = new RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (var node_3 in nodeMap.Values)
            {
                asserter.AssertNode(node_3);
            }
            sql =
                "insert into offer_detail (offer_id, gmt) values (123,now()),(123,now()+1),(234,now()),(123,now()),(345,now()),(122+1,now()),(456,now())";
            schema = schemaMap["cndb"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(-1L, rrs.LimitSize);
            nodeMap = GetNodeMap(rrs, 4);
            nameAsserter = new NodeNameAsserter("detail_dn[29]", "detail_dn[43]", "detail_dn[57]", "detail_dn[15]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new SimpleSqlAsserter();
            sqlAsserter.AddExpectSql(0, "INSERT INTO offer_detail (offer_id, gmt) VALUES (234, NOW())")
                       .AddExpectSql(1, "INSERT INTO offer_detail (offer_id, gmt) VALUES (345, NOW())")
                       .AddExpectSql(2, "INSERT INTO offer_detail (offer_id, gmt) VALUES (456, NOW())")
                       .AddExpectSql(3, "INSERT INTO offer_detail (offer_id, gmt) VALUES ",
                           new PermutationUtil.PermutationGenerator("(123, NOW())", "(123, NOW() + 1)",
                               "(122 + 1, NOW())", "(123, NOW())").SetDelimiter(", "), string.Empty);
            asserter = new RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (var node_4 in nodeMap.Values)
            {
                asserter.AssertNode(node_4);
            }
            sql = "insert into offer (offer_id, group_id, gmt) values " +
                  "(123, 123, now()),(123, 234, now()),(123, 345, now()),(123, 456, now())"
                  + ",(234, 123, now()),(234, 234, now()),(234, 345, now()),(234, 456, now())" +
                  ",(345, 123, now()),(345, 234, now()),(345, 345, now()),(345, 456, now())" +
                  ",(456, 123, now()),(456, 234, now()),(456, 345, now()),(456, 456, now())";
            schema = schemaMap["cndb"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(-1L, rrs.LimitSize);
            nodeMap = GetNodeMap(rrs, 7);
            nameAsserter = new NodeNameAsserter("offer_dn[58]", "offer_dn[100]", "offer_dn[86]", "offer_dn[72]", "offer_dn[114]", "offer_dn[44]", "offer_dn[30]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new SimpleSqlAsserter();
            sqlAsserter.AddExpectSql(0, "INSERT INTO offer (offer_id, group_id, gmt) VALUES ", new PermutationUtil.PermutationGenerator("(345, 123, NOW())", "(123, 345, NOW())", "(234, 234, NOW())").SetDelimiter(", "), string.Empty)
                       .AddExpectSql(1, "INSERT INTO offer (offer_id, group_id, gmt) VALUES "
                           ,
                           new PermutationUtil.PermutationGenerator("(345, 456, NOW())", "(456, 345, NOW())")
                               .SetDelimiter(", "), string.Empty)
                       .AddExpectSql(2, "INSERT INTO offer (offer_id, group_id, gmt) VALUES ", new PermutationUtil.PermutationGenerator("(456, 234, NOW())", "(234, 456, NOW())", "(345, 345, NOW())").SetDelimiter(", "), string.Empty)
                       .AddExpectSql(3, "INSERT INTO offer (offer_id, group_id, gmt) VALUES ", new PermutationUtil.PermutationGenerator("(123, 456, NOW())", "(345, 234, NOW())", "(234, 345, NOW())", "(456, 123, NOW())").SetDelimiter(", "), string.Empty)
                       .AddExpectSql(4, "INSERT INTO offer (offer_id, group_id, gmt) VALUES ",
                           new PermutationUtil.PermutationGenerator("(456, 456, NOW())").SetDelimiter(", "),
                           string.Empty)
                       .AddExpectSql(5, "INSERT INTO offer (offer_id, group_id, gmt) VALUES "
                           ,
                           new PermutationUtil.PermutationGenerator("(234, 123, NOW())", "(123, 234, NOW())")
                               .SetDelimiter(", "), string.Empty)
                       .AddExpectSql(6, "INSERT INTO offer (offer_id, group_id, gmt) VALUES ", new PermutationUtil.PermutationGenerator("(123, 123, NOW())").SetDelimiter(", "),
                           string.Empty);
            asserter = new RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (var node_5 in nodeMap.Values)
            {
                asserter.AssertNode(node_5);
            }
            sql = "select * from offer where (offer_id, group_id ) = (123,234)";
            schema = schemaMap["cndb"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(-1L, rrs.LimitSize);
            Assert.AreEqual(128, rrs.Nodes.Length);
            for (var i = 0; i < 128; i++)
            {
                Assert.AreEqual(RouteResultsetNode.DefaultReplicaIndex, rrs.Nodes[i].ReplicaIndex);
                Assert.AreEqual("offer_dn[" + i + "]", rrs.Nodes[i].Name);
                Assert.AreEqual("select * from offer where (offer_id, group_id ) = (123,234)", rrs.Nodes[i].Statement);
            }
            sql = "select * from offer where offer_id=123 and group_id=234";
            schema = schemaMap["cndb"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(-1L, rrs.LimitSize);
            Assert.AreEqual(1, rrs.Nodes.Length);
            Assert.AreEqual(RouteResultsetNode.DefaultReplicaIndex, rrs.Nodes[0].ReplicaIndex);
            Assert.AreEqual("offer_dn[44]", rrs.Nodes[0].Name);
            Assert.AreEqual("select * from offer where offer_id=123 and group_id=234", rrs.Nodes[0].Statement);
            // WITHOUT Sql CHANGE unless schema is appeared
            sql = "select * from  cndb.offer where false" +
                  " or offer_id=123 and group_id=123 or offer_id=123 and group_id=234 or offer_id=123 and group_id=345 or offer_id=123 and group_id=456  "
                  +
                  " or offer_id=234 and group_id=123 or offer_id=234 and group_id=234 or offer_id=234 and group_id=345 or offer_id=234 and group_id=456  "
                  +
                  " or offer_id=345 and group_id=123 or offer_id=345 and group_id=234 or offer_id=345 and group_id=345 or offer_id=345 and group_id=456  "
                  +
                  " or offer_id=456 and group_id=123 or offer_id=456 and group_id=234 or offer_id=456 and group_id=345 or offer_id=456 and group_id=456  ";
            schema = schemaMap["cndb"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(-1L, rrs.LimitSize);
            var sqlTemp =
                "SELECT * FROM offer WHERE FALSE OR offer_id = 123 AND group_id = 123 OR offer_id = 123 AND group_id = 234 OR offer_id = 123 AND group_id = 345 OR offer_id = 123 AND group_id = 456 OR offer_id = 234 AND group_id = 123 OR offer_id = 234 AND group_id = 234 OR offer_id = 234 AND group_id = 345 OR offer_id = 234 AND group_id = 456 OR offer_id = 345 AND group_id = 123 OR offer_id = 345 AND group_id = 234 OR offer_id = 345 AND group_id = 345 OR offer_id = 345 AND group_id = 456 OR offer_id = 456 AND group_id = 123 OR offer_id = 456 AND group_id = 234 OR offer_id = 456 AND group_id = 345 OR offer_id = 456 AND group_id = 456";
            nodeMap = GetNodeMap(rrs, 7);
            nameAsserter = new NodeNameAsserter("offer_dn[58]", "offer_dn[100]", "offer_dn[86]", "offer_dn[72]", "offer_dn[114]", "offer_dn[44]", "offer_dn[30]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new SimpleSqlAsserter();
            sqlAsserter.AddExpectSql(0, sqlTemp)
                       .AddExpectSql(1, sqlTemp)
                       .AddExpectSql(2, sqlTemp)
                       .AddExpectSql(3, sqlTemp)
                       .AddExpectSql(4, sqlTemp)
                       .AddExpectSql(5, sqlTemp)
                       .AddExpectSql(6, sqlTemp);
            asserter = new RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (var node_6 in nodeMap.Values)
            {
                asserter.AssertNode(node_6);
            }
            sql = "select * from  offer where false" + " or offer_id=123 and group_id=123" +
                  " or group_id=123 and offer_id=234" + " or offer_id=123 and group_id=345" +
                  " or offer_id=123 and group_id=456  ";
            schema = schemaMap["cndb"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(-1L, rrs.LimitSize);
            sqlTemp =
                "select * from  offer where false or offer_id=123 and group_id=123 or group_id=123 and offer_id=234 or offer_id=123 and group_id=345 or offer_id=123 and group_id=456  ";
            nodeMap = GetNodeMap(rrs, 4);
            nameAsserter = new NodeNameAsserter("offer_dn[72]", "offer_dn[58]", "offer_dn[44]", "offer_dn[30]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new SimpleSqlAsserter();
            sqlAsserter.AddExpectSql(0, sqlTemp)
                       .AddExpectSql(1, sqlTemp)
                       .AddExpectSql(2, sqlTemp)
                       .AddExpectSql(3, sqlTemp);
            asserter = new RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (var node_7 in nodeMap.Values)
            {
                asserter.AssertNode(node_7);
            }
        }

        /// <exception cref="System.Exception" />
        [Test]
        public virtual void TestGroupLimit()
        {
            var schema = schemaMap["cndb"];
            var sql = "select Count(*) from wp_image where member_id = 'pavarotti17'";
            var rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(-1L, rrs.LimitSize);
            Assert.AreEqual(0, rrs.Flag);
            Assert.AreEqual(1, rrs.Nodes.Length);
            Assert.AreEqual(RouteResultsetNode.DefaultReplicaIndex, rrs.Nodes[0].ReplicaIndex);
            Assert.AreEqual("offer_dn[123]", rrs.Nodes[0].Name);
            Assert.AreEqual("select Count(*) from wp_image where member_id = 'pavarotti17'", rrs.Nodes[0].Statement);
            sql = "select Count(*) from wp_image where member_id in ('pavarotti17','qaa')";
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(-1L, rrs.LimitSize);
            if (rrs.Nodes.Length > 1)
            {
                Assert.AreEqual(RouteResultset.SumFlag, rrs.Flag);
            }
            var nodeMap = GetNodeMap(rrs, 2);
            var nameAsserter = new NodeNameAsserter("offer_dn[123]", "offer_dn[10]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            var sqlAsserter = new SimpleSqlAsserter();
            sqlAsserter.AddExpectSql(0, "SELECT COUNT(*) FROM wp_image WHERE member_id IN ('pavarotti17')")
                       .AddExpectSql(1, "SELECT COUNT(*) FROM wp_image WHERE member_id IN ('qaa')");
            var asserter = new RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (var node in nodeMap.Values)
            {
                asserter.AssertNode(node);
            }
            sql = "select min(id) from wp_image where member_id in ('pavarotti17','qaa') limit 99";
            rrs = ServerRouter.Route(schema, sql, null, null);
            if (rrs.Nodes.Length > 1)
            {
                Assert.AreEqual(RouteResultset.MinFlag, rrs.Flag);
            }
            if (rrs.Nodes.Length > 1)
            {
                Assert.AreEqual(99L, rrs.LimitSize);
            }
            nodeMap = GetNodeMap(rrs, 2);
            nameAsserter = new NodeNameAsserter("offer_dn[123]", "offer_dn[10]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new SimpleSqlAsserter();
            sqlAsserter.AddExpectSql(0, "SELECT MIN(id) FROM wp_image WHERE member_id IN ('pavarotti17') LIMIT 0, 99")
                       .AddExpectSql(1, "SELECT MIN(id) FROM wp_image WHERE member_id IN ('qaa') LIMIT 0, 99");
            asserter = new RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (var node_1 in nodeMap.Values)
            {
                asserter.AssertNode(node_1);
            }
            sql =
                "select max(offer_id) from offer.wp_image where member_id in ('pavarotti17','pavarotti17', 'qaa') or member_id in ('pavarotti17','1qq','pavarotti17') or offer.wp_image.member_id='pavarotti17' limit 99 offset 1";
            rrs = ServerRouter.Route(schema, sql, null, null);
            if (rrs.Nodes.Length > 1)
            {
                Assert.AreEqual(RouteResultset.MaxFlag, rrs.Flag);
            }
            if (rrs.Nodes.Length > 1)
            {
                Assert.AreEqual(99L, rrs.LimitSize);
            }
            nodeMap = GetNodeMap(rrs, 3);
            nameAsserter = new NodeNameAsserter("offer_dn[123]", "offer_dn[10]", "offer_dn[66]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new SimpleSqlAsserter();
            sqlAsserter.AddExpectSql(0,
                "SELECT MAX(offer_id) FROM wp_image WHERE member_id IN ('pavarotti17', 'pavarotti17') OR member_id IN ('pavarotti17', 'pavarotti17') OR wp_image.member_id = 'pavarotti17' LIMIT 1, 99")
                       .AddExpectSql(1,
                           "SELECT MAX(offer_id) FROM wp_image WHERE member_id IN ('qaa') OR FALSE OR FALSE LIMIT 1, 99")
                       .AddExpectSql(2,
                           "SELECT MAX(offer_id) FROM wp_image WHERE FALSE OR member_id IN ('1qq') OR FALSE LIMIT 1, 99");
            asserter = new RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (var node_2 in nodeMap.Values)
            {
                asserter.AssertNode(node_2);
            }
            sql = "select Count(*) from (select * from wp_image) w, (select * from offer) o "
                  + " where o.member_id=w.member_id and o.member_id='pavarotti17' limit 99";
            rrs = ServerRouter.Route(schema, sql, null, null);
            if (rrs.Nodes.Length > 1)
            {
                Assert.AreEqual(RouteResultset.SumFlag, rrs.Flag);
            }
            if (rrs.Nodes.Length > 1)
            {
                Assert.AreEqual(99L, rrs.LimitSize);
            }
            nodeMap = GetNodeMap(rrs, 128);
            nameAsserter = new IndexedNodeNameAsserter("offer_dn", 0, 128);
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new SimpleSqlAsserter();
            for (var i = 0; i < 128; ++i)
            {
                sqlAsserter.AddExpectSql(i,
                    "select Count(*) from (select * from wp_image) w, (select * from offer) o  where o.member_id=w.member_id and o.member_id='pavarotti17' limit 99");
            }
            asserter = new RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (var node_3 in nodeMap.Values)
            {
                asserter.AssertNode(node_3);
            }
            sql = "select Count(*) from (select * from wp_image) w, (select * from offer limit 99) o "
                  + " where o.member_id=w.member_id and o.member_id='pavarotti17' ";
            rrs = ServerRouter.Route(schema, sql, null, null);
            if (rrs.Nodes.Length > 1)
            {
                Assert.AreEqual(RouteResultset.SumFlag, rrs.Flag);
            }
            if (rrs.Nodes.Length > 1)
            {
                Assert.AreEqual(99L, rrs.LimitSize);
            }
            nodeMap = GetNodeMap(rrs, 128);
            nameAsserter = new IndexedNodeNameAsserter("offer_dn", 0, 128);
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new SimpleSqlAsserter();
            for (var i_1 = 0; i_1 < 128; ++i_1)
            {
                sqlAsserter.AddExpectSql(i_1,
                    "select Count(*) from (select * from wp_image) w, (select * from offer limit 99) o  where o.member_id=w.member_id and o.member_id='pavarotti17' ");
            }
            asserter = new RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (var node_4 in nodeMap.Values)
            {
                asserter.AssertNode(node_4);
            }
            sql = "select Count(*) from (select * from wp_image where member_id='abc' or member_id='pavarotti17' limit 100) w, (select * from offer_detail where offer_id='123') o "
                  + " where o.member_id=w.member_id and o.member_id='pavarotti17' limit 99";
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(RouteResultset.SumFlag, rrs.Flag);
            Assert.AreEqual(100L, rrs.LimitSize);
            nodeMap = GetNodeMap(rrs, 2);
            nameAsserter = new NodeNameAsserter("offer_dn[12]", "offer_dn[123]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new SimpleSqlAsserter();
            sqlAsserter.AddExpectSql(0,
                "SELECT COUNT(*) FROM (SELECT * FROM wp_image WHERE member_id = 'abc' OR FALSE LIMIT 0, 100) AS "
                + AliasConvert("w") + ", (SELECT * FROM offer_detail WHERE offer_id = '123') AS "
                + AliasConvert("o") + " WHERE o.member_id = w.member_id AND o.member_id = 'pavarotti17' LIMIT 0, 99")
                       .AddExpectSql(1,
                           "SELECT COUNT(*) FROM (SELECT * FROM wp_image WHERE FALSE OR member_id = 'pavarotti17' LIMIT 0, 100) AS "
                           + AliasConvert("w") + ", (SELECT * FROM offer_detail WHERE offer_id = '123') AS "
                           + AliasConvert("o") +
                           " WHERE o.member_id = w.member_id AND o.member_id = 'pavarotti17' LIMIT 0, 99");
            asserter = new RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (var node_5 in nodeMap.Values)
            {
                asserter.AssertNode(node_5);
            }
            sql = "select Count(*) from (select * from(select * from offer_detail where offer_id='123' or offer_id='234' limit 88)offer  where offer.member_id='abc' limit 60) w "
                  + " where w.member_id ='pavarotti17' limit 99";
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(88L, rrs.LimitSize);
            Assert.AreEqual(RouteResultset.SumFlag, rrs.Flag);
            nodeMap = GetNodeMap(rrs, 2);
            nameAsserter = new NodeNameAsserter("detail_dn[29]", "detail_dn[15]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new SimpleSqlAsserter();
            sqlAsserter.AddExpectSql(0,
                "SELECT COUNT(*) FROM (SELECT * FROM (SELECT * FROM offer_detail WHERE FALSE OR offer_id = '234' LIMIT 0, 88) AS "
                + AliasConvert("offer") + " WHERE offer.member_id = 'abc' LIMIT 0, 60) AS " + AliasConvert("w") +
                " WHERE w.member_id = 'pavarotti17' LIMIT 0, 99")
                       .AddExpectSql(1,
                           "SELECT COUNT(*) FROM (SELECT * FROM (SELECT * FROM offer_detail WHERE offer_id = '123' OR FALSE LIMIT 0, 88) AS "
                           + AliasConvert("offer") + " WHERE offer.member_id = 'abc' LIMIT 0, 60) AS " +
                           AliasConvert("w") + " WHERE w.member_id = 'pavarotti17' LIMIT 0, 99");
            asserter = new RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (var node_6 in nodeMap.Values)
            {
                asserter.AssertNode(node_6);
            }
            sql = "select Count(*) from (select * from(select max(id) from offer_detail where offer_id='123' or offer_id='234' limit 88)offer  where offer.member_id='abc' limit 60) w "
                  + " where w.member_id ='pavarotti17' limit 99";
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(88L, rrs.LimitSize);
            Assert.AreEqual(0, rrs.Flag);
            nodeMap = GetNodeMap(rrs, 2);
            nameAsserter = new NodeNameAsserter("detail_dn[29]", "detail_dn[15]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new SimpleSqlAsserter();
            sqlAsserter.AddExpectSql(0,
                "SELECT COUNT(*) FROM (SELECT * FROM (SELECT MAX(id) FROM offer_detail WHERE FALSE OR offer_id = '234' LIMIT 0, 88) AS "
                + AliasConvert("offer") + " WHERE offer.member_id = 'abc' LIMIT 0, 60) AS " + AliasConvert("w") +
                " WHERE w.member_id = 'pavarotti17' LIMIT 0, 99")
                       .AddExpectSql(1,
                           "SELECT COUNT(*) FROM (SELECT * FROM (SELECT MAX(id) FROM offer_detail WHERE offer_id = '123' OR FALSE LIMIT 0, 88) AS "
                           + AliasConvert("offer") + " WHERE offer.member_id = 'abc' LIMIT 0, 60) AS " +
                           AliasConvert("w") + " WHERE w.member_id = 'pavarotti17' LIMIT 0, 99");
            asserter = new RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (var node_7 in nodeMap.Values)
            {
                asserter.AssertNode(node_7);
            }
            sql = "select * from (select * from(select max(id) from offer_detail where offer_id='123' or offer_id='234' limit 88)offer  where offer.member_id='abc' limit 60) w "
                  + " where w.member_id ='pavarotti17' limit 99";
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(88L, rrs.LimitSize);
            Assert.AreEqual(RouteResultset.MaxFlag, rrs.Flag);
            nodeMap = GetNodeMap(rrs, 2);
            nameAsserter = new NodeNameAsserter("detail_dn[29]", "detail_dn[15]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new SimpleSqlAsserter();
            sqlAsserter.AddExpectSql(0,
                "SELECT * FROM (SELECT * FROM (SELECT MAX(id) FROM offer_detail WHERE FALSE OR offer_id = '234' LIMIT 0, 88) AS "
                + AliasConvert("offer") + " WHERE offer.member_id = 'abc' LIMIT 0, 60) AS " + AliasConvert("w") +
                " WHERE w.member_id = 'pavarotti17' LIMIT 0, 99")
                       .AddExpectSql(1,
                           "SELECT * FROM (SELECT * FROM (SELECT MAX(id) FROM offer_detail WHERE offer_id = '123' OR FALSE LIMIT 0, 88) AS "
                           + AliasConvert("offer") + " WHERE offer.member_id = 'abc' LIMIT 0, 60) AS " +
                           AliasConvert("w") + " WHERE w.member_id = 'pavarotti17' LIMIT 0, 99");
            asserter = new RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (var node_8 in nodeMap.Values)
            {
                asserter.AssertNode(node_8);
            }
            sql = "select * from (select Count(*) from(select * from offer_detail where offer_id='123' or offer_id='234' limit 88)offer  where offer.member_id='abc' limit 60) w "
                  + " where w.member_id ='pavarotti17' limit 99";
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(88L, rrs.LimitSize);
            Assert.AreEqual(RouteResultset.SumFlag, rrs.Flag);
            nodeMap = GetNodeMap(rrs, 2);
            nameAsserter = new NodeNameAsserter("detail_dn[29]", "detail_dn[15]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new SimpleSqlAsserter();
            sqlAsserter.AddExpectSql(0,
                "SELECT * FROM (SELECT COUNT(*) FROM (SELECT * FROM offer_detail WHERE FALSE OR offer_id = '234' LIMIT 0, 88) AS "
                + AliasConvert("offer") + " WHERE offer.member_id = 'abc' LIMIT 0, 60) AS " + AliasConvert("w") +
                " WHERE w.member_id = 'pavarotti17' LIMIT 0, 99")
                       .AddExpectSql(1,
                           "SELECT * FROM (SELECT COUNT(*) FROM (SELECT * FROM offer_detail WHERE offer_id = '123' OR FALSE LIMIT 0, 88) AS "
                           + AliasConvert("offer") + " WHERE offer.member_id = 'abc' LIMIT 0, 60) AS " +
                           AliasConvert("w") + " WHERE w.member_id = 'pavarotti17' LIMIT 0, 99");
            asserter = new RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (var node_9 in nodeMap.Values)
            {
                asserter.AssertNode(node_9);
            }
        }

        /// <exception cref="System.Exception" />
        [Test]
        public virtual void TestIgnoreSchema()
        {
            var schema = schemaMap["ignoreSchemaTest"];
            var sql = "select * from offer where offer_id=1";
            var rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual("cndb_dn", rrs.Nodes[0].Name);
            Assert.AreEqual(sql, rrs.Nodes[0].Statement);
            sql = "select * from ignoreSchemaTest.offer where ignoreSchemaTest.offer.offer_id=1";
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual("SELECT * FROM offer WHERE offer.offer_id = 1", rrs.Nodes[0].Statement);
            sql = "select * from ignoreSchemaTest2.offer where ignoreSchemaTest2.offer.offer_id=1";
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(sql, rrs.Nodes[0].Statement);
            sql =
                "select * from ignoreSchemaTest2.offer a,ignoreSchemaTest.offer b  where ignoreSchemaTest2.offer.offer_id=1";
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(
                "SELECT * FROM ignoreSchemaTest2.offer AS " + AliasConvert("a") + ", offer AS " + AliasConvert("b") +
                " WHERE ignoreSchemaTest2.offer.offer_id = 1", rrs.Nodes[0].Statement);
            schema = schemaMap["ignoreSchemaTest0"];
            sql = "select * from offer where offer_id=1";
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(sql, rrs.Nodes[0].Statement);
            sql = "select * from ignoreSchemaTest0.offer where ignoreSchemaTest.offer.offer_id=1";
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual("SELECT * FROM offer WHERE ignoreSchemaTest.offer.offer_id = 1", rrs.Nodes[0].Statement);
            sql = "insert into offer (group_id, offer_id, gmt) values (234,123,now())";
            schema = schemaMap["ignoreSchemaTest0"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(1, rrs.Nodes.Length);
            Assert.AreEqual(-1L, rrs.LimitSize);
            Assert.AreEqual(RouteResultsetNode.DefaultReplicaIndex, rrs.Nodes[0].ReplicaIndex);
            Assert.AreEqual("offer_dn[44]", rrs.Nodes[0].Name);
            Assert.AreEqual("insert into offer (group_id, offer_id, gmt) values (234,123,now())", rrs.Nodes[0].Statement);
            sql = "insert into ignoreSchemaTest0.offer (group_id, offer_id, gmt) values (234,123,now())";
            schema = schemaMap["ignoreSchemaTest0"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(1, rrs.Nodes.Length);
            Assert.AreEqual(-1L, rrs.LimitSize);
            Assert.AreEqual(RouteResultsetNode.DefaultReplicaIndex, rrs.Nodes[0].ReplicaIndex);
            Assert.AreEqual("offer_dn[44]", rrs.Nodes[0].Name);
            Assert.AreEqual("INSERT INTO offer (group_id, offer_id, gmt) VALUES (234, 123, NOW())", rrs.Nodes[0].Statement);
            sql = "insert into ignoreSchemaTest2.offer (group_id, offer_id, gmt) values (234,123,now())";
            schema = schemaMap["ignoreSchemaTest0"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual("cndb_dn", rrs.Nodes[0].Name);
            Assert.AreEqual(sql, rrs.Nodes[0].Statement);
            sql =
                "insert into ignoreSchemaTest2.offer (ignoreSchemaTest0.offer.group_id, offer_id, gmt) values (234,123,now())";
            schema = schemaMap["ignoreSchemaTest0"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual("cndb_dn", rrs.Nodes[0].Name);
            Assert.AreEqual(
                "INSERT INTO ignoreSchemaTest2.offer (offer.group_id, offer_id, gmt) VALUES (234, 123, NOW())", rrs.Nodes[0].Statement);
        }

        /// <exception cref="System.Exception" />
        [Test]
        public virtual void TestNonPartitionSql()
        {
            var schema = schemaMap["cndb"];
            var sql = "  select * from `dual`";
            var rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(1, rrs.Nodes.Length);
            Assert.AreEqual(RouteResultsetNode.DefaultReplicaIndex, rrs.Nodes[0].ReplicaIndex);
            Assert.AreEqual("cndb_dn", rrs.Nodes[0].Name);
            Assert.AreEqual("  select * from `dual`", rrs.Nodes[0].Statement);
            schema = schemaMap["dubbo"];
            sql = "  select * from `dual`";
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(1, rrs.Nodes.Length);
            Assert.AreEqual(RouteResultsetNode.DefaultReplicaIndex, rrs.Nodes[0].ReplicaIndex);
            Assert.AreEqual("dubbo_dn", rrs.Nodes[0].Name);
            Assert.AreEqual("  select * from `dual`", rrs.Nodes[0].Statement);
            schema = schemaMap["dubbo"];
            sql = "  select * from dubbo.`dual`";
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(1, rrs.Nodes.Length);
            Assert.AreEqual(RouteResultsetNode.DefaultReplicaIndex, rrs.Nodes[0].ReplicaIndex);
            Assert.AreEqual("dubbo_dn", rrs.Nodes[0].Name);
            Assert.AreEqual("  select * from dubbo.`dual`", rrs.Nodes[0].Statement);
            sql = "SHOW TABLES from db_name like 'solo'";
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(-1L, rrs.LimitSize);
            Assert.AreEqual(RouteResultsetNode.DefaultReplicaIndex, rrs.Nodes[0].ReplicaIndex);
            Assert.AreEqual(1, rrs.Nodes.Length);
            Assert.AreEqual("dubbo_dn", rrs.Nodes[0].Name);
            Assert.AreEqual("SHOW TABLES from db_name like 'solo'", rrs.Nodes[0].Statement);
            sql = "desc cndb.offer";
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(-1L, rrs.LimitSize);
            Assert.AreEqual(RouteResultsetNode.DefaultReplicaIndex, rrs.Nodes[0].ReplicaIndex);
            Assert.AreEqual(1, rrs.Nodes.Length);
            Assert.AreEqual("dubbo_dn", rrs.Nodes[0].Name);
            Assert.AreEqual("desc cndb.offer", rrs.Nodes[0].Statement);
            schema = schemaMap["cndb"];
            sql = "SHOW fulL TaBLES from db_name like 'solo'";
            rrs = ServerRouter.Route(schema, sql, null, null);
            var nodeMap = GetNodeMap(rrs, 4);
            var nameAsserter = new NodeNameAsserter("detail_dn[0]", "offer_dn[0]", "cndb_dn", "independent_dn[0]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            var sqlAsserter = new SimpleSqlAsserter();
            sqlAsserter.AddExpectSql(0, "SHOW FULL TABLES LIKE 'solo'")
                       .AddExpectSql(1, "SHOW FULL TABLES LIKE 'solo'")
                       .AddExpectSql(2, "SHOW FULL TABLES LIKE 'solo'")
                       .AddExpectSql(3, "SHOW FULL TABLES LIKE 'solo'");
            var asserter = new RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (var node in nodeMap.Values)
            {
                asserter.AssertNode(node);
            }
        }

        /// <exception cref="System.Exception" />
        [Test]
        public virtual void TestRoute()
        {
            var sql = "select * from offer.wp_image where member_id='pavarotti17' or member_id='1qq'";
            var schema = schemaMap["cndb"];
            var rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(-1L, rrs.LimitSize);
            Assert.AreEqual(RouteResultsetNode.DefaultReplicaIndex, rrs.Nodes[0].ReplicaIndex);
            var nodeMap = GetNodeMap(rrs, 2);
            var nameAsserter = new NodeNameAsserter("offer_dn[123]", "offer_dn[66]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            var sqlAsserter = new SimpleSqlAsserter();
            sqlAsserter.AddExpectSql(0, "SELECT * FROM wp_image WHERE member_id = 'pavarotti17' OR FALSE")
                       .AddExpectSql(1, "SELECT * FROM wp_image WHERE FALSE OR member_id = '1qq'");
            var asserter = new RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (var node in nodeMap.Values)
            {
                asserter.AssertNode(node);
            }
            sql = "select * from independent where member='abc'";
            schema = schemaMap["cndb"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            nodeMap = GetNodeMap(rrs, 128);
            nameAsserter = new IndexedNodeNameAsserter("independent_dn", 0, 128);
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new SimpleSqlAsserter();
            for (var i = 0; i < 128; ++i)
            {
                sqlAsserter.AddExpectSql(i, "select * from independent where member='abc'");
            }
            asserter = new RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (var node_1 in nodeMap.Values)
            {
                asserter.AssertNode(node_1);
            }
            sql = "select * from independent A where cndb.a.member='abc'";
            schema = schemaMap["cndb"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            nodeMap = GetNodeMap(rrs, 128);
            nameAsserter = new IndexedNodeNameAsserter("independent_dn", 0, 128);
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new SimpleSqlAsserter();
            for (var i_1 = 0; i_1 < 128; ++i_1)
            {
                sqlAsserter.AddExpectSql(i_1, "SELECT * FROM independent AS A WHERE a.member = 'abc'");
            }
            asserter = new RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (var node_2 in nodeMap.Values)
            {
                asserter.AssertNode(node_2);
            }
            sql = "select * from tb where member='abc'";
            schema = schemaMap["cndb"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(1, rrs.Nodes.Length);
            Assert.AreEqual(-1L, rrs.LimitSize);
            Assert.AreEqual(RouteResultsetNode.DefaultReplicaIndex, rrs.Nodes[0].ReplicaIndex);
            Assert.AreEqual("cndb_dn", rrs.Nodes[0].Name);
            Assert.AreEqual("select * from tb where member='abc'", rrs.Nodes[0].Statement);
            sql = "select * from offer.wp_image where member_id is null";
            schema = schemaMap["cndb"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(1, rrs.Nodes.Length);
            Assert.AreEqual(-1L, rrs.LimitSize);
            Assert.AreEqual(RouteResultsetNode.DefaultReplicaIndex, rrs.Nodes[0].ReplicaIndex);
            Assert.AreEqual("offer_dn[48]", rrs.Nodes[0].Name);
            Assert.AreEqual("SELECT * FROM wp_image WHERE member_id IS NULL",
                rrs.Nodes[0].Statement);
            sql = "select * from offer.wp_image where member_id between 'pavarotti17' and 'pavarotti17'";
            schema = schemaMap["cndb"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(1, rrs.Nodes.Length);
            Assert.AreEqual(-1L, rrs.LimitSize);
            Assert.AreEqual(RouteResultsetNode.DefaultReplicaIndex, rrs.Nodes[0].ReplicaIndex);
            Assert.AreEqual("offer_dn[123]", rrs.Nodes[0].Name);
            Assert.AreEqual("SELECT * FROM wp_image WHERE member_id BETWEEN 'pavarotti17' AND 'pavarotti17'", rrs.Nodes[0].Statement);
            sql =
                "select * from  offer A where a.member_id='abc' union select * from product_visit b where B.offer_id =123";
            schema = schemaMap["cndb"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(128, rrs.Nodes.Length);
            Assert.AreEqual(-1L, rrs.LimitSize);
            for (var i_2 = 0; i_2 < 128; i_2++)
            {
                Assert.AreEqual(RouteResultsetNode.DefaultReplicaIndex, rrs.Nodes[i_2].ReplicaIndex);
                Assert.AreEqual("offer_dn[" + i_2 + "]", rrs.Nodes[i_2].Name);
                Assert.AreEqual(
                    "select * from  offer A where a.member_id='abc' union select * from product_visit b where B.offer_id =123", rrs.Nodes[i_2].Statement);
            }
            sql =
                "update offer.offer a join offer_detail b set id=123 where a.offer_id=b.offer_id and a.offer_id=123 and group_id=234";
            schema = schemaMap["cndb"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(1, rrs.Nodes.Length);
            Assert.AreEqual(-1L, rrs.LimitSize);
            Assert.AreEqual(RouteResultsetNode.DefaultReplicaIndex, rrs.Nodes[0].ReplicaIndex);
            Assert.AreEqual("offer_dn[44]", rrs.Nodes[0].Name);
            Assert.AreEqual("UPDATE offer AS " + AliasConvert("a") + " INNER JOIN offer_detail AS "
                            + AliasConvert("b") +
                            " SET id = 123 WHERE a.offer_id = b.offer_id AND a.offer_id = 123 AND group_id = 234", rrs.Nodes[0].Statement);
            sql =
                "update    offer./*kjh*/offer a join offer_detail B set id:=123 where A.offer_id=b.offer_id and b.offer_id=123 and group_id=234";
            schema = schemaMap["cndb"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(1, rrs.Nodes.Length);
            Assert.AreEqual(-1L, rrs.LimitSize);
            Assert.AreEqual(RouteResultsetNode.DefaultReplicaIndex, rrs.Nodes[0].ReplicaIndex);
            Assert.AreEqual("detail_dn[15]", rrs.Nodes[0].Name);
            Assert.AreEqual("UPDATE offer AS " + AliasConvert("a") + " INNER JOIN offer_detail AS "
                            + AliasConvert("b") +
                            " SET id = 123 WHERE A.offer_id = b.offer_id AND b.offer_id = 123 AND group_id = 234", rrs.Nodes[0].Statement);
            sql =
                "select * from offer.wp_image where member_id in ('pavarotti17', 'qaa') or offer.wp_image.member_id='1qq'";
            schema = schemaMap["cndb"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(-1L, rrs.LimitSize);
            nodeMap = GetNodeMap(rrs, 3);
            nameAsserter = new NodeNameAsserter("offer_dn[123]", "offer_dn[10]", "offer_dn[66]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new SimpleSqlAsserter();
            sqlAsserter.AddExpectSql(0, "SELECT * FROM wp_image WHERE member_id IN ('pavarotti17') OR FALSE")
                       .AddExpectSql(1, "SELECT * FROM wp_image WHERE member_id IN ('qaa') OR FALSE")
                       .
                        AddExpectSql(2, "SELECT * FROM wp_image WHERE FALSE OR wp_image.member_id = '1qq'");
            asserter = new RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (var node_3 in nodeMap.Values)
            {
                asserter.AssertNode(node_3);
            }
            sql =
                "select * from offer.wp_image,tb2 as t2 where member_id in ('pavarotti17', 'qaa') or offer.wp_image.member_id='1qq'";
            schema = schemaMap["cndb"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(3, rrs.Nodes.Length);
            Assert.AreEqual(-1L, rrs.LimitSize);
            nodeMap = GetNodeMap(rrs, 3);
            nameAsserter = new NodeNameAsserter("offer_dn[123]", "offer_dn[10]", "offer_dn[66]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new SimpleSqlAsserter();
            sqlAsserter.AddExpectSql(0,
                "SELECT * FROM wp_image, tb2 AS " + AliasConvert("t2") + " WHERE member_id IN ('pavarotti17') OR FALSE")
                       .AddExpectSql(1, "SELECT * FROM wp_image, tb2 AS "
                                        + AliasConvert("t2") + " WHERE member_id IN ('qaa') OR FALSE").AddExpectSql(2,
                                            "SELECT * FROM wp_image, tb2 AS " + AliasConvert("t2") +
                                            " WHERE FALSE OR wp_image.member_id = '1qq'");
            asserter = new RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (var node_4 in nodeMap.Values)
            {
                asserter.AssertNode(node_4);
            }
            sql =
                "select * from offer.wp_image,tb2 as t2 where member_id in ('pavarotti17', 'sf', 's22f', 'sdddf', 'sd') ";
            schema = schemaMap["cndb"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(-1L, rrs.LimitSize);
            nodeMap = GetNodeMap(rrs, 4);
            nameAsserter = new NodeNameAsserter("offer_dn[123]", "offer_dn[126]", "offer_dn[74]", "offer_dn[26]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new SimpleSqlAsserter();
            sqlAsserter.AddExpectSql(0,
                "SELECT * FROM wp_image, tb2 AS " + AliasConvert("t2") + " WHERE member_id IN ('pavarotti17')")
                       .AddExpectSql(1, "SELECT * FROM wp_image, tb2 AS "
                                        + AliasConvert("t2") + " WHERE member_id IN ('sdddf')")
                       .AddExpectSql(2, "SELECT * FROM wp_image, tb2 AS "
                                        + AliasConvert("t2") + " WHERE member_id IN ('sf', 'sd')",
                           "SELECT * FROM wp_image, tb2 AS "
                           + AliasConvert("t2") + " WHERE member_id IN ('sd', 'sf')")
                       .AddExpectSql(3, "SELECT * FROM wp_image, tb2 AS "
                                        + AliasConvert("t2") + " WHERE member_id IN ('s22f')");
            asserter = new RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (var node_5 in nodeMap.Values)
            {
                asserter.AssertNode(node_5);
            }
            sql =
                "select * from tb2 as t2 ,offer.wp_image where member_id in ('pavarotti17', 'qaa') or offer.wp_image.member_id='1qq'";
            schema = schemaMap["cndb"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(-1L, rrs.LimitSize);
            nodeMap = GetNodeMap(rrs, 3);
            nameAsserter = new NodeNameAsserter("offer_dn[123]", "offer_dn[10]", "offer_dn[66]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new SimpleSqlAsserter();
            sqlAsserter.AddExpectSql(0,
                "SELECT * FROM tb2 AS " + AliasConvert("t2") + ", wp_image WHERE member_id IN ('pavarotti17') OR FALSE")
                       .AddExpectSql(1,
                           "SELECT * FROM tb2 AS " + AliasConvert("t2") +
                           ", wp_image WHERE member_id IN ('qaa') OR FALSE")
                       .AddExpectSql(2,
                           "SELECT * FROM tb2 AS " + AliasConvert("t2") +
                           ", wp_image WHERE FALSE OR wp_image.member_id = '1qq'");
            asserter = new RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (var node_6 in nodeMap.Values)
            {
                asserter.AssertNode(node_6);
            }
            sql =
                "select * from tb2 as t2 ,offer.wp_image where member_id in ('pavarotti17', 'qaa') or offer.wp_image.member_id='1qq' and t2.member_id='123'";
            schema = schemaMap["cndb"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(-1L, rrs.LimitSize);
            nodeMap = GetNodeMap(rrs, 3);
            nameAsserter = new NodeNameAsserter("offer_dn[123]", "offer_dn[10]", "offer_dn[66]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new SimpleSqlAsserter();
            sqlAsserter.AddExpectSql(0,
                "SELECT * FROM tb2 AS " + AliasConvert("t2") +
                ", wp_image WHERE member_id IN ('pavarotti17') OR FALSE AND t2.member_id = '123'")
                       .AddExpectSql(1,
                           "SELECT * FROM tb2 AS " + AliasConvert("t2") +
                           ", wp_image WHERE member_id IN ('qaa') OR FALSE AND t2.member_id = '123'")
                       .AddExpectSql(2,
                           "SELECT * FROM tb2 AS " + AliasConvert("t2") +
                           ", wp_image WHERE FALSE OR wp_image.member_id = '1qq' AND t2.member_id = '123'");
            asserter = new RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (var node_7 in nodeMap.Values)
            {
                asserter.AssertNode(node_7);
            }
            sql =
                "select * from wp_image wB inner join offer.offer o on wB.member_id=O.member_ID where wB.member_iD='pavarotti17' and o.id=3";
            schema = schemaMap["cndb"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(-1L, rrs.LimitSize);
            Assert.AreEqual(1, rrs.Nodes.Length);
            Assert.AreEqual(RouteResultsetNode.DefaultReplicaIndex, rrs.Nodes[0].ReplicaIndex);
            Assert.AreEqual("offer_dn[123]", rrs.Nodes[0].Name);
            Assert.AreEqual("SELECT * FROM wp_image AS " + AliasConvert("wB")
                            + " INNER JOIN offer AS " + AliasConvert("o") +
                            " ON wB.member_id = O.member_ID WHERE wB.member_iD = 'pavarotti17' AND o.id = 3", rrs.Nodes[0].Statement);
            sql =
                "select * from wp_image w inner join offer o on w.member_id=O.member_ID where w.member_iD in ('pavarotti17','13') and o.id=3";
            schema = schemaMap["cndb"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(-1L, rrs.LimitSize);
            nodeMap = GetNodeMap(rrs, 2);
            nameAsserter = new NodeNameAsserter("offer_dn[123]", "offer_dn[68]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new SimpleSqlAsserter();
            sqlAsserter.AddExpectSql(0, "SELECT * FROM wp_image AS " + AliasConvert("w") + " INNER JOIN offer AS "
                                        + AliasConvert("o") +
                                        " ON w.member_id = O.member_ID WHERE w.member_iD IN ('pavarotti17') AND o.id = 3")
                       .AddExpectSql(1, "SELECT * FROM wp_image AS " + AliasConvert("w") + " INNER JOIN offer AS "
                                        + AliasConvert("o") +
                                        " ON w.member_id = O.member_ID WHERE w.member_iD IN ('13') AND o.id = 3");
            asserter = new RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (var node_8 in nodeMap.Values)
            {
                asserter.AssertNode(node_8);
            }
            sql = "insert into wp_image (member_id,gmt) values ('pavarotti17',now()),('123',now())";
            schema = schemaMap["cndb"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(-1L, rrs.LimitSize);
            nodeMap = GetNodeMap(rrs, 2);
            nameAsserter = new NodeNameAsserter("offer_dn[123]", "offer_dn[70]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new SimpleSqlAsserter();
            sqlAsserter.AddExpectSql(0, "INSERT INTO wp_image (member_id, gmt) VALUES ('pavarotti17', NOW())")
                       .AddExpectSql(1, "INSERT INTO wp_image (member_id, gmt) VALUES ('123', NOW())");
            asserter = new RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (var node_9 in nodeMap.Values)
            {
                asserter.AssertNode(node_9);
            }
        }

        /// <exception cref="System.Exception" />
        [Test]
        public virtual void TestRouteInsertLong()
        {
            var sb = new StringBuilder("insert into offer_detail (offer_id, gmt) values ");
            for (var i = 0; i < 1024; ++i)
            {
                if (i > 0)
                {
                    sb.Append(", ");
                }
                sb.Append("(" + i + ", now())");
            }
            var schema = schemaMap["cndb"];
            var rrs = ServerRouter.Route(schema, sb.ToString(), null, null);
            Assert.AreEqual(-1L, rrs.LimitSize);
            var nodeMap = GetNodeMap(rrs, 128);
            var nameAsserter = new IndexedNodeNameAsserter("detail_dn", 0, 128);
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            var asserter = new RouteNodeAsserter(nameAsserter, new _ParseredSQLAsserter_351());
            foreach (var node in nodeMap.Values)
            {
                asserter.AssertNode(node);
            }
        }

        ///// <exception cref="System.Exception"/>
        //protected override void SetUp()
        //{
        //}

        // super.setUp();
        // schemaMap = CobarServer.getInstance().getConfig().getSchemas();
        /// <exception cref="System.Exception" />
        [Test]
        public virtual void TestRouteInsertShort()
        {
            var sql = "inSErt into offer_detail (`offer_id`, gmt) values (123,now())";
            var schema = schemaMap["cndb"];
            var rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(1, rrs.Nodes.Length);
            Assert.AreEqual(-1L, rrs.LimitSize);
            Assert.AreEqual(RouteResultsetNode.DefaultReplicaIndex, rrs.Nodes[0].ReplicaIndex);
            Assert.AreEqual("detail_dn[15]", rrs.Nodes[0].Name);
            Assert.AreEqual("inSErt into offer_detail (`offer_id`, gmt) values (123,now())", rrs.Nodes[0].Statement);
            sql = "inSErt into offer_detail ( gmt) values (now())";
            schema = schemaMap["cndb"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(128, rrs.Nodes.Length);
            sql = "inSErt into offer_detail (offer_id, gmt) values (123,now())";
            schema = schemaMap["cndb"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(1, rrs.Nodes.Length);
            Assert.AreEqual(-1L, rrs.LimitSize);
            Assert.AreEqual(RouteResultsetNode.DefaultReplicaIndex, rrs.Nodes[0].ReplicaIndex);
            Assert.AreEqual("detail_dn[15]", rrs.Nodes[0].Name);
            Assert.AreEqual("inSErt into offer_detail (offer_id, gmt) values (123,now())", rrs.Nodes[0].Statement);
            sql = "insert into offer(group_id,offer_id,member_id)values(234,123,'abc')";
            schema = schemaMap["cndb"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(1, rrs.Nodes.Length);
            Assert.AreEqual(-1L, rrs.LimitSize);
            Assert.AreEqual(RouteResultsetNode.DefaultReplicaIndex, rrs.Nodes[0].ReplicaIndex);
            Assert.AreEqual("offer_dn[12]", rrs.Nodes[0].Name);
            Assert.AreEqual("insert into offer(group_id,offer_id,member_id)values(234,123,'abc')", rrs.Nodes[0].Statement);
            sql = "insert into offer (group_id, offer_id, gmt) values (234,123,now())";
            schema = schemaMap["cndb"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(1, rrs.Nodes.Length);
            Assert.AreEqual(-1L, rrs.LimitSize);
            Assert.AreEqual(RouteResultsetNode.DefaultReplicaIndex, rrs.Nodes[0].ReplicaIndex);
            Assert.AreEqual("offer_dn[44]", rrs.Nodes[0].Name);
            Assert.AreEqual("insert into offer (group_id, offer_id, gmt) values (234,123,now())", rrs.Nodes[0].Statement);
            sql = "insert into offer (offer_id, group_id, gmt) values (123,234,now())";
            schema = schemaMap["cndb"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(1, rrs.Nodes.Length);
            Assert.AreEqual(-1L, rrs.LimitSize);
            Assert.AreEqual(RouteResultsetNode.DefaultReplicaIndex, rrs.Nodes[0].ReplicaIndex);
            Assert.AreEqual("offer_dn[44]", rrs.Nodes[0].Name);
            Assert.AreEqual("insert into offer (offer_id, group_id, gmt) values (123,234,now())", rrs.Nodes[0].Statement);
            sql = "insert into offer (offer_id, group_id, gmt) values (234,123,now())";
            schema = schemaMap["cndb"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(1, rrs.Nodes.Length);
            Assert.AreEqual(-1L, rrs.LimitSize);
            Assert.AreEqual(RouteResultsetNode.DefaultReplicaIndex, rrs.Nodes[0].ReplicaIndex);
            Assert.AreEqual("offer_dn[44]", rrs.Nodes[0].Name);
            Assert.AreEqual("insert into offer (offer_id, group_id, gmt) values (234,123,now())", rrs.Nodes[0].Statement);
            sql = "insert into wp_image (member_id,gmt) values ('pavarotti17',now())";
            schema = schemaMap["cndb"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(1, rrs.Nodes.Length);
            Assert.AreEqual(-1L, rrs.LimitSize);
            Assert.AreEqual(RouteResultsetNode.DefaultReplicaIndex, rrs.Nodes[0].ReplicaIndex);
            Assert.AreEqual("offer_dn[123]", rrs.Nodes[0].Name);
            Assert.AreEqual("insert into wp_image (member_id,gmt) values ('pavarotti17',now())", rrs.Nodes[0].Statement);
            sql =
                "insert low_priority into offer set offer_id=123,  group_id=234,gmt=now() on duplicate key update `dual`=1";
            schema = schemaMap["cndb"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(1, rrs.Nodes.Length);
            Assert.AreEqual(-1L, rrs.LimitSize);
            Assert.AreEqual(RouteResultsetNode.DefaultReplicaIndex, rrs.Nodes[0].ReplicaIndex);
            Assert.AreEqual("offer_dn[44]", rrs.Nodes[0].Name);
            Assert.AreEqual(
                "insert low_priority into offer set offer_id=123,  group_id=234,gmt=now() on duplicate key update `dual`=1", rrs.Nodes[0].Statement);
            sql = "update ignore wp_image set name='abc',gmt=now()where `select`='abc'";
            schema = schemaMap["cndb"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(1, rrs.Nodes.Length);
            Assert.AreEqual(-1L, rrs.LimitSize);
            Assert.AreEqual(RouteResultsetNode.DefaultReplicaIndex, rrs.Nodes[0].ReplicaIndex);
            Assert.AreEqual("offer_dn[12]", rrs.Nodes[0].Name);
            Assert.AreEqual("update ignore wp_image set name='abc',gmt=now()where `select`='abc'", rrs.Nodes[0].Statement);
            sql =
                "delete from offer.*,wp_image.* using offer a,wp_image b where a.member_id=b.member_id and a.member_id='abc' ";
            schema = schemaMap["cndb"];
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(1, rrs.Nodes.Length);
            Assert.AreEqual(-1L, rrs.LimitSize);
            Assert.AreEqual(RouteResultsetNode.DefaultReplicaIndex, rrs.Nodes[0].ReplicaIndex);
            Assert.AreEqual("offer_dn[12]", rrs.Nodes[0].Name);
            Assert.AreEqual(
                "delete from offer.*,wp_image.* using offer a,wp_image b where a.member_id=b.member_id and a.member_id='abc' ", rrs.Nodes[0].Statement);
        }

        /// <exception cref="System.Exception" />
        [Test]
        public virtual void TestTableMetaRead()
        {
            var schema = schemaMap["cndb"];
            var sql = "desc offer";
            var rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(-1L, rrs.LimitSize);
            Assert.AreEqual(1, rrs.Nodes.Length);
            Assert.AreEqual(RouteResultsetNode.DefaultReplicaIndex, rrs.Nodes[0].ReplicaIndex);
            Assert.AreEqual("offer_dn[0]", rrs.Nodes[0].Name);
            Assert.AreEqual("desc offer", rrs.Nodes[0].Statement);
            sql = "desc cndb.offer";
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(-1L, rrs.LimitSize);
            Assert.AreEqual(1, rrs.Nodes.Length);
            Assert.AreEqual(RouteResultsetNode.DefaultReplicaIndex, rrs.Nodes[0].ReplicaIndex);
            Assert.AreEqual("offer_dn[0]", rrs.Nodes[0].Name);
            Assert.AreEqual("DESC offer", rrs.Nodes[0].Statement);
            sql = "SHOW FULL COLUMNS FROM  offer  IN db_name WHERE true";
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(-1L, rrs.LimitSize);
            Assert.AreEqual(1, rrs.Nodes.Length);
            Assert.AreEqual(RouteResultsetNode.DefaultReplicaIndex, rrs.Nodes[0].ReplicaIndex);
            Assert.AreEqual("offer_dn[0]", rrs.Nodes[0].Name);
            Assert.AreEqual("SHOW FULL COLUMNS FROM offer WHERE TRUE", rrs.Nodes[0].Statement);
            sql = "SHOW FULL COLUMNS FROM  db.offer  IN db_name WHERE true";
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(-1L, rrs.LimitSize);
            Assert.AreEqual(1, rrs.Nodes.Length);
            Assert.AreEqual(RouteResultsetNode.DefaultReplicaIndex, rrs.Nodes[0].ReplicaIndex);
            Assert.AreEqual("offer_dn[0]", rrs.Nodes[0].Name);
            Assert.AreEqual("SHOW FULL COLUMNS FROM offer WHERE TRUE", rrs.Nodes[0].Statement);
            sql = "SHOW INDEX  IN offer FROM  db_name";
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(-1L, rrs.LimitSize);
            Assert.AreEqual(1, rrs.Nodes.Length);
            Assert.AreEqual(RouteResultsetNode.DefaultReplicaIndex, rrs.Nodes[0].ReplicaIndex);
            Assert.AreEqual("offer_dn[0]", rrs.Nodes[0].Name);
            Assert.AreEqual("SHOW INDEX IN offer", rrs.Nodes[0].Statement);
            sql = "SHOW TABLES from db_name like 'solo'";
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(-1L, rrs.LimitSize);
            var nodeMap = GetNodeMap(rrs, 4);
            var nameAsserter = new NodeNameAsserter("detail_dn[0]", "offer_dn[0]", "cndb_dn", "independent_dn[0]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            var sqlAsserter = new SimpleSqlAsserter();
            sqlAsserter.AddExpectSql(0, "SHOW TABLES LIKE 'solo'")
                       .AddExpectSql(1, "SHOW TABLES LIKE 'solo'")
                       .AddExpectSql(2, "SHOW TABLES LIKE 'solo'")
                       .AddExpectSql(3, "SHOW TABLES LIKE 'solo'");
            var asserter = new RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (var node in nodeMap.Values)
            {
                asserter.AssertNode(node);
            }
            sql = "SHOW TABLES in db_name ";
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(-1L, rrs.LimitSize);
            nodeMap = GetNodeMap(rrs, 4);
            nameAsserter = new NodeNameAsserter("detail_dn[0]", "offer_dn[0]", "cndb_dn", "independent_dn[0]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new SimpleSqlAsserter();
            sqlAsserter.AddExpectSql(0, "SHOW TABLES")
                       .AddExpectSql(1, "SHOW TABLES")
                       .AddExpectSql(2, "SHOW TABLES")
                       .AddExpectSql(3, "SHOW TABLES");
            asserter = new RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (var node_1 in nodeMap.Values)
            {
                asserter.AssertNode(node_1);
            }
            sql = "SHOW TABLeS ";
            rrs = ServerRouter.Route(schema, sql, null, null);
            Assert.AreEqual(-1L, rrs.LimitSize);
            nodeMap = GetNodeMap(rrs, 4);
            nameAsserter = new NodeNameAsserter("detail_dn[0]", "offer_dn[0]", "cndb_dn", "independent_dn[0]");
            nameAsserter.AssertRouteNodeNames(nodeMap.Keys);
            sqlAsserter = new SimpleSqlAsserter();
            sqlAsserter.AddExpectSql(0, "SHOW TABLeS ")
                       .AddExpectSql(1, "SHOW TABLeS ")
                       .AddExpectSql(2, "SHOW TABLeS ")
                       .AddExpectSql(3, "SHOW TABLeS ");
            asserter = new RouteNodeAsserter(nameAsserter, sqlAsserter);
            foreach (var node_2 in nodeMap.Values)
            {
                asserter.AssertNode(node_2);
            }
        }
    }
}