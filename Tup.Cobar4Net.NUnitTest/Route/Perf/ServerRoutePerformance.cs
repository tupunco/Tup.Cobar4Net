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
using System.Text;
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
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class ServerRoutePerformance
    {
        private abstract class TestProvider
        {
            /// <exception cref="System.Exception"/>
            public abstract string GetSql();

            /// <exception cref="System.Exception"/>
            public abstract void Route(SchemaConfig schema, int loop, string sql);
        }

        private class ShardingDefaultSpace : ServerRoutePerformance.TestProvider
        {
            private SQLStatement stmt;

            /// <exception cref="System.Exception"/>
            public override void Route(SchemaConfig schema, int loop, string sql)
            {
                for (int i = 0; i < loop; ++i)
                {
                    // SQLLexer lexer = new SQLLexer(sql);
                    // DMLSelectStatement select = new DMLSelectParser(lexer, new
                    // SQLExprParser(lexer)).select();
                    // PartitionKeyVisitor visitor = new
                    // PartitionKeyVisitor(schema.getTablesSpace());
                    // select.accept(visitor);
                    // visitor.getColumnValue();
                    ServerRouter.Route(schema, sql, null, null);
                }
            }

            // StringBuilder s = new StringBuilder();
            // stmt.accept(new MySQLOutputASTVisitor(s));
            // s.toString();
            /// <exception cref="System.Exception"/>
            public override string GetSql()
            {
                string sql = "insert into xoffer (member_id, gmt_create) values ('1','2001-09-13 20:20:33')";
                stmt = SQLParserDelegate.Parse(sql);
                return "insert into xoffer (member_id, gmt_create) values ('1','2001-09-13 20:20:33')";
            }
        }

        private class ShardingTableSpace : ServerRoutePerformance.TestProvider
        {
            private SQLStatement stmt;

            /// <exception cref="System.Exception"/>
            public override void Route(SchemaConfig schema, int loop, string sql)
            {
                for (int i = 0; i < loop; ++i)
                {
                    // SQLLexer lexer = new SQLLexer(sql);
                    // DMLSelectStatement select = new DMLSelectParser(lexer, new
                    // SQLExprParser(lexer)).select();
                    // PartitionKeyVisitor visitor = new
                    // PartitionKeyVisitor(schema.getTablesSpace());
                    // select.accept(visitor);
                    // visitor.getColumnValue();
                    ServerRouter.Route(schema, sql, null, null);
                }
            }

            // StringBuilder s = new StringBuilder();
            // stmt.accept(new MySQLOutputASTVisitor(s));
            // s.toString();
            /// <exception cref="System.Exception"/>
            public override string GetSql()
            {
                string sql = "insert into offer (member_id, gmt_create) values ('1','2001-09-13 20:20:33')";
                stmt = SQLParserDelegate.Parse(sql);
                return "insert into offer (member_id, gmt_create) values ('1','2001-09-13 20:20:33'),('1','2001-09-13 20:20:34')";
            }
        }

        private class ShardingMultiTableSpace : ServerRoutePerformance.TestProvider
        {
            private SQLStatement stmt;

            /// <exception cref="System.Exception"/>
            public override void Route(SchemaConfig schema, int loop, string sql)
            {
                for (int i = 0; i < loop * 5; ++i)
                {
                    // SQLLexer lexer = new SQLLexer(sql);
                    // DMLSelectStatement select = new DMLSelectParser(lexer, new
                    // SQLExprParser(lexer)).select();
                    // PartitionKeyVisitor visitor = new
                    // PartitionKeyVisitor(schema.getTablesSpace());
                    // select.accept(visitor);
                    // visitor.getColumnValue();
                    ServerRouter.Route(schema, sql, null, null);
                }
            }

            // StringBuilder s = new StringBuilder();
            // stmt.accept(new MySQLOutputASTVisitor(s));
            // s.toString();
            /// <exception cref="System.Exception"/>
            public override string GetSql()
            {
                string sql = "select id,member_id,gmt_create from offer where member_id in ('22')";
                stmt = SQLParserDelegate.Parse(sql);
                return "select id,member_id,gmt_create from offer where member_id in ('1','22','333','1124','4525')";
            }
        }

        private class SelectShort : ServerRoutePerformance.TestProvider
        {
            /// <exception cref="System.Exception"/>
            public override void Route(SchemaConfig schema, int loop, string sql)
            {
                for (int i = 0; i < loop; ++i)
                {
                    MySQLLexer lexer = new MySQLLexer(sql);
                    DMLSelectStatement select = new MySQLDMLSelectParser(lexer, new MySQLExprParser(lexer
                        )).Select();
                    PartitionKeyVisitor visitor = new PartitionKeyVisitor(schema.GetTables());
                    select.Accept(visitor);
                }
            }

            // visitor.getColumnValue();
            // ServerRoute.route(schema, sql);
            /// <exception cref="System.Exception"/>
            public override string GetSql()
            {
                return " seLEcT id, member_id , image_path  \t , image_size , STATUS,   gmt_modified from    offer_detail wheRe \t\t\n offer_id =  123 AND member_id\t=\t-123.456";
            }
        }

        private class SelectLongIn : ServerRoutePerformance.TestProvider
        {
            /// <exception cref="System.Exception"/>
            public override void Route(SchemaConfig schema, int loop, string sql)
            {
                for (int i = 0; i < loop; ++i)
                {
                    MySQLLexer lexer = new MySQLLexer(sql);
                    DMLSelectStatement select = new MySQLDMLSelectParser(lexer, new MySQLExprParser(lexer
                        )).Select();
                    PartitionKeyVisitor visitor = new PartitionKeyVisitor(schema.GetTables());
                    select.Accept(visitor);
                }
            }

            // visitor.getColumnValue();
            // ServerRoute.route(schema, sql);
            /// <exception cref="System.Exception"/>
            public override string GetSql()
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(" seLEcT id, member_id , image_path  \t , image_size , STATUS,   gmt_modified from"
                    ).Append("    offer_detail wheRe \t\t\n offer_id in (");
                for (int i = 0; i < 1024; ++i)
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

        private class InsertLong : ServerRoutePerformance.TestProvider
        {
            /// <exception cref="System.Exception"/>
            public override string GetSql()
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
                // System.out.println(sb.length());
                return sb.ToString();
            }

            /// <exception cref="System.Exception"/>
            public override void Route(SchemaConfig schema, int loop, string sql)
            {
                for (int i = 0; i < loop; ++i)
                {
                    MySQLLexer lexer = new MySQLLexer(sql);
                    DMLInsertStatement insert = new MySQLDMLInsertParser(lexer, new MySQLExprParser(lexer
                        )).Insert();
                }
            }

            // PartitionKeyVisitor visitor = new
            // PartitionKeyVisitor(schema.getTablesSpace());
            // insert.accept(visitor);
            // visitor.getColumnValue();
            // SQLLexer lexer = new SQLLexer(sql);
            // new DMLInsertParser(lexer, new
            // SQLExprParser(lexer)).insert();
            // RouteResultset rrs = ServerRoute.route(schema, sql);
            // System.out.println(rrs);
        }

        private class InsertLongSQLGen : ServerRoutePerformance.TestProvider
        {
            private DMLInsertStatement insert;

            private int sqlSize = 0;

            /// <exception cref="System.Exception"/>
            public override string GetSql()
            {
                string sql = new ServerRoutePerformance.InsertLong().GetSql();
                MySQLLexer lexer = new MySQLLexer(sql);
                insert = new MySQLDMLInsertParser(lexer, new MySQLExprParser(lexer)).Insert();
                return sql;
            }

            /// <exception cref="System.Exception"/>
            public override void Route(SchemaConfig schema, int loop, string sql)
            {
                for (int i = 0; i < loop; ++i)
                {
                    StringBuilder sb = new StringBuilder(sqlSize);
                    insert.Accept(new MySQLOutputASTVisitor(sb));
                    sb.ToString();
                }
            }
        }

        private class InsertLongSQLGenShort : ServerRoutePerformance.TestProvider
        {
            private DMLInsertStatement insert;

            private int sqlSize;

            /// <exception cref="System.Exception"/>
            public override string GetSql()
            {
                StringBuilder sb = new StringBuilder("insert into offer_detail (offer_id, gmt) values "
                    );
                for (int i = 0; i < 8; ++i)
                {
                    if (i > 0)
                    {
                        sb.Append(", ");
                    }
                    sb.Append("(" + (i + 100) + ", now())");
                }
                string sql = sb.ToString();
                MySQLLexer lexer = new MySQLLexer(sql);
                insert = new MySQLDMLInsertParser(lexer, new MySQLExprParser(lexer)).Insert();
                sqlSize = new ServerRoutePerformance.InsertLong().GetSql().Length;
                return sql;
            }

            /// <exception cref="System.Exception"/>
            public override void Route(SchemaConfig schema, int loop, string sql)
            {
                for (int i = 0; i < loop; ++i)
                {
                    for (int j = 0; j < 128; ++j)
                    {
                        StringBuilder sb = new StringBuilder();
                        insert.Accept(new MySQLOutputASTVisitor(sb));
                        sb.ToString();
                    }
                }
            }
        }

        /// <exception cref="System.Exception"/>
        public virtual void Perf()
        {
            ServerRoutePerformance.TestProvider provider;
            provider = new ServerRoutePerformance.InsertLongSQLGen();
            provider = new ServerRoutePerformance.InsertLongSQLGenShort();
            provider = new ServerRoutePerformance.SelectShort();
            provider = new ServerRoutePerformance.InsertLong();
            provider = new ServerRoutePerformance.SelectLongIn();
            provider = new ServerRoutePerformance.ShardingMultiTableSpace();
            provider = new ServerRoutePerformance.ShardingDefaultSpace();
            provider = new ServerRoutePerformance.ShardingTableSpace();
            SchemaConfig schema = GetSchema();
            string sql = provider.GetSql();
            System.Console.Out.WriteLine(ServerRouter.Route(schema, sql, null, null));
            long start = Runtime.CurrentTimeMillis();
            provider.Route(schema, 1, sql);
            long end;
            int loop = 200 * 10000;
            start = Runtime.CurrentTimeMillis();
            provider.Route(schema, loop, sql);
            end = Runtime.CurrentTimeMillis();
            System.Console.Out.WriteLine((end - start) * 1000.0d / loop + " us");
        }

        private SchemaConfig schema = null;

        public ServerRoutePerformance()
        {
        }

        // CobarConfig conf = CobarServer.getInstance().getConfig();
        // schema = conf.getSchemas().get("cndb");
        /// <param name="args"/>
        /// <exception cref="System.Exception"/>
        public static void Main(string[] args)
        {
            ServerRoutePerformance perf = new ServerRoutePerformance();
            perf.Perf();
        }

        protected internal virtual SchemaConfig GetSchema()
        {
            return schema;
        }
    }
}
