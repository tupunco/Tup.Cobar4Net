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
using System.Text;
using Sharpen;
using Tup.Cobar4Net.Config.Model;
using Tup.Cobar4Net.Parser.Ast.Stmt;
using Tup.Cobar4Net.Parser.Ast.Stmt.Dml;
using Tup.Cobar4Net.Parser.Recognizer;
using Tup.Cobar4Net.Parser.Recognizer.Mysql.Lexer;
using Tup.Cobar4Net.Parser.Recognizer.Mysql.Syntax;
using Tup.Cobar4Net.Parser.Visitor;
using Tup.Cobar4Net.Route.Visitor;

namespace Tup.Cobar4Net.Route.Perf
{
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public class ServerRoutePerformance
    {
        private readonly SchemaConfig schema = null;

        /// <exception cref="System.Exception" />
        public virtual void Perf()
        {
            TestProvider provider;
            provider = new InsertLongSQLGen();
            provider = new InsertLongSQLGenShort();
            provider = new SelectShort();
            provider = new InsertLong();
            provider = new SelectLongIn();
            provider = new ShardingMultiTableSpace();
            provider = new ShardingDefaultSpace();
            provider = new ShardingTableSpace();
            var schema = GetSchema();
            var sql = provider.GetSql();
            Console.Out.WriteLine(ServerRouter.Route(schema, sql, null, null));
            var start = Runtime.CurrentTimeMillis();
            provider.Route(schema, 1, sql);
            long end;
            var loop = 200*10000;
            start = Runtime.CurrentTimeMillis();
            provider.Route(schema, loop, sql);
            end = Runtime.CurrentTimeMillis();
            Console.Out.WriteLine((end - start)*1000.0d/loop + " us");
        }

        // CobarConfig conf = CobarServer.getInstance().getConfig();
        // schema = conf.getSchemas().get("cndb");
        /// <param name="args" />
        /// <exception cref="System.Exception" />
        public static void Main(string[] args)
        {
            var perf = new ServerRoutePerformance();
            perf.Perf();
        }

        protected internal virtual SchemaConfig GetSchema()
        {
            return schema;
        }

        private abstract class TestProvider
        {
            /// <exception cref="System.Exception" />
            public abstract string GetSql();

            /// <exception cref="System.Exception" />
            public abstract void Route(SchemaConfig schema, int loop, string sql);
        }

        private class ShardingDefaultSpace : TestProvider
        {
            private ISqlStatement stmt;

            /// <exception cref="System.Exception" />
            public override void Route(SchemaConfig schema, int loop, string sql)
            {
                for (var i = 0; i < loop; ++i)
                {
                    // SQLLexer lexer = new SQLLexer(sql);
                    // DmlSelectStatement select = new DmlSelectParser(lexer, new
                    // SQLExprParser(lexer)).select();
                    // PartitionKeyVisitor visitor = new
                    // PartitionKeyVisitor(schema.getTablesSpace());
                    // select.accept(visitor);
                    // visitor.getColumnValue();
                    ServerRouter.Route(schema, sql, null, null);
                }
            }

            // StringBuilder s = new StringBuilder();
            // stmt.accept(new MySqlOutputAstVisitor(s));
            // s.toString();
            /// <exception cref="System.Exception" />
            public override string GetSql()
            {
                var sql = "insert into xoffer (member_id, gmt_create) values ('1','2001-09-13 20:20:33')";
                stmt = SqlParserDelegate.Parse(sql);
                return "insert into xoffer (member_id, gmt_create) values ('1','2001-09-13 20:20:33')";
            }
        }

        private class ShardingTableSpace : TestProvider
        {
            private ISqlStatement stmt;

            /// <exception cref="System.Exception" />
            public override void Route(SchemaConfig schema, int loop, string sql)
            {
                for (var i = 0; i < loop; ++i)
                {
                    // SQLLexer lexer = new SQLLexer(sql);
                    // DmlSelectStatement select = new DmlSelectParser(lexer, new
                    // SQLExprParser(lexer)).select();
                    // PartitionKeyVisitor visitor = new
                    // PartitionKeyVisitor(schema.getTablesSpace());
                    // select.accept(visitor);
                    // visitor.getColumnValue();
                    ServerRouter.Route(schema, sql, null, null);
                }
            }

            // StringBuilder s = new StringBuilder();
            // stmt.accept(new MySqlOutputAstVisitor(s));
            // s.toString();
            /// <exception cref="System.Exception" />
            public override string GetSql()
            {
                var sql = "insert into offer (member_id, gmt_create) values ('1','2001-09-13 20:20:33')";
                stmt = SqlParserDelegate.Parse(sql);
                return
                    "insert into offer (member_id, gmt_create) values ('1','2001-09-13 20:20:33'),('1','2001-09-13 20:20:34')";
            }
        }

        private class ShardingMultiTableSpace : TestProvider
        {
            private ISqlStatement stmt;

            /// <exception cref="System.Exception" />
            public override void Route(SchemaConfig schema, int loop, string sql)
            {
                for (var i = 0; i < loop*5; ++i)
                {
                    // SQLLexer lexer = new SQLLexer(sql);
                    // DmlSelectStatement select = new DmlSelectParser(lexer, new
                    // SQLExprParser(lexer)).select();
                    // PartitionKeyVisitor visitor = new
                    // PartitionKeyVisitor(schema.getTablesSpace());
                    // select.accept(visitor);
                    // visitor.getColumnValue();
                    ServerRouter.Route(schema, sql, null, null);
                }
            }

            // StringBuilder s = new StringBuilder();
            // stmt.accept(new MySqlOutputAstVisitor(s));
            // s.toString();
            /// <exception cref="System.Exception" />
            public override string GetSql()
            {
                var sql = "select id,member_id,gmt_create from offer where member_id in ('22')";
                stmt = SqlParserDelegate.Parse(sql);
                return "select id,member_id,gmt_create from offer where member_id in ('1','22','333','1124','4525')";
            }
        }

        private class SelectShort : TestProvider
        {
            /// <exception cref="System.Exception" />
            public override void Route(SchemaConfig schema, int loop, string sql)
            {
                for (var i = 0; i < loop; ++i)
                {
                    var lexer = new MySqlLexer(sql);
                    var select = new MySqlDmlSelectParser(lexer, new MySqlExprParser(lexer)).Select();
                    var visitor = new PartitionKeyVisitor(schema.Tables);
                    select.Accept(visitor);
                }
            }

            // visitor.getColumnValue();
            // ServerRoute.route(schema, sql);
            /// <exception cref="System.Exception" />
            public override string GetSql()
            {
                return
                    " seLEcT id, member_id , image_path  \t , image_size , STATUS,   gmt_modified from    offer_detail wheRe \t\t\n offer_id =  123 AND member_id\t=\t-123.456";
            }
        }

        private class SelectLongIn : TestProvider
        {
            /// <exception cref="System.Exception" />
            public override void Route(SchemaConfig schema, int loop, string sql)
            {
                for (var i = 0; i < loop; ++i)
                {
                    var lexer = new MySqlLexer(sql);
                    var select = new MySqlDmlSelectParser(lexer, new MySqlExprParser(lexer)).Select();
                    var visitor = new PartitionKeyVisitor(schema.Tables);
                    select.Accept(visitor);
                }
            }

            // visitor.getColumnValue();
            // ServerRoute.route(schema, sql);
            /// <exception cref="System.Exception" />
            public override string GetSql()
            {
                var sb = new StringBuilder();
                sb.Append(" seLEcT id, member_id , image_path  \t , image_size , STATUS,   gmt_modified from")
                  .Append("    offer_detail wheRe \t\t\n offer_id in (");
                for (var i = 0; i < 1024; ++i)
                {
                    if (i > 0)
                    {
                        sb.Append(", ");
                    }
                    sb.Append(i);
                }
                sb.Append(") AND member_id\t=\t-123.456");
                // System.out.println(sb.length());
                return sb.ToString();
            }
        }

        private class InsertLong : TestProvider
        {
            /// <exception cref="System.Exception" />
            public override string GetSql()
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
                // System.out.println(sb.length());
                return sb.ToString();
            }

            /// <exception cref="System.Exception" />
            public override void Route(SchemaConfig schema, int loop, string sql)
            {
                for (var i = 0; i < loop; ++i)
                {
                    var lexer = new MySqlLexer(sql);
                    var insert = new MySqlDmlInsertParser(lexer, new MySqlExprParser(lexer)).Insert();
                }
            }

            // PartitionKeyVisitor visitor = new

            // PartitionKeyVisitor(schema.getTablesSpace());
            // insert.accept(visitor);
            // visitor.getColumnValue();
            // SQLLexer lexer = new SQLLexer(sql);
            // new DmlInsertParser(lexer, new
            // SQLExprParser(lexer)).insert();
            // RouteResultset rrs = ServerRoute.route(schema, sql);
            // System.out.println(rrs);
        }

        private class InsertLongSQLGen : TestProvider
        {
            private readonly int sqlSize = 0;
            private DmlInsertStatement insert;

            /// <exception cref="System.Exception" />
            public override string GetSql()
            {
                var sql = new InsertLong().GetSql();
                var lexer = new MySqlLexer(sql);
                insert = new MySqlDmlInsertParser(lexer, new MySqlExprParser(lexer)).Insert();
                return sql;
            }

            /// <exception cref="System.Exception" />
            public override void Route(SchemaConfig schema, int loop, string sql)
            {
                for (var i = 0; i < loop; ++i)
                {
                    var sb = new StringBuilder(sqlSize);
                    insert.Accept(new MySqlOutputAstVisitor(sb));
                    sb.ToString();
                }
            }
        }

        private class InsertLongSQLGenShort : TestProvider
        {
            private DmlInsertStatement insert;

            private int sqlSize;

            /// <exception cref="System.Exception" />
            public override string GetSql()
            {
                var sb = new StringBuilder("insert into offer_detail (offer_id, gmt) values ");
                for (var i = 0; i < 8; ++i)
                {
                    if (i > 0)
                    {
                        sb.Append(", ");
                    }
                    sb.Append("(" + (i + 100) + ", now())");
                }
                var sql = sb.ToString();
                var lexer = new MySqlLexer(sql);
                insert = new MySqlDmlInsertParser(lexer, new MySqlExprParser(lexer)).Insert();
                sqlSize = new InsertLong().GetSql().Length;
                return sql;
            }

            /// <exception cref="System.Exception" />
            public override void Route(SchemaConfig schema, int loop, string sql)
            {
                for (var i = 0; i < loop; ++i)
                {
                    for (var j = 0; j < 128; ++j)
                    {
                        var sb = new StringBuilder();
                        insert.Accept(new MySqlOutputAstVisitor(sb));
                        sb.ToString();
                    }
                }
            }
        }
    }
}