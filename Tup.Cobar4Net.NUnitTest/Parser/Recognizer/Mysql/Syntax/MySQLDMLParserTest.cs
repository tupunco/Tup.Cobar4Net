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

using System.Collections.Generic;
using Tup.Cobar4Net.Parser.Ast.Expression.Comparison;
using Tup.Cobar4Net.Parser.Ast.Expression.Logical;
using Tup.Cobar4Net.Parser.Ast.Expression.Primary;
using Tup.Cobar4Net.Parser.Ast.Fragment;
using Tup.Cobar4Net.Parser.Ast.Fragment.Tableref;
using Tup.Cobar4Net.Parser.Ast.Stmt.Dml;
using Tup.Cobar4Net.Parser.Recognizer.Mysql.Lexer;
using Tup.Cobar4Net.Parser.Util;

namespace Tup.Cobar4Net.Parser.Recognizer.Mysql.Syntax
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    [TestFixture(Category = "MySQLDMLParserTest")]
    public class MySQLDMLParserTest : AbstractSyntaxTest
    {
        protected internal virtual MySQLDMLParser GetDMLParser(MySQLLexer lexer)
        {
            MySQLExprParser exp = new MySQLExprParser(lexer);
            MySQLDMLParser parser = new MySQLDMLSelectParser(lexer, exp);
            return parser;
        }

        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        [Test]
        public virtual void TestOrderBy()
        {
            string sql = "order by c1 asc, c2 desc  , c3 ";
            MySQLLexer lexer = new MySQLLexer(sql);
            MySQLDMLParser parser = GetDMLParser(lexer);
            OrderBy orderBy = parser.OrderBy();
            string output = Output2MySQL(orderBy, sql);
            ListUtil.IsEquals(ListUtil.CreateList(new Pair<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                , SortOrder>(new Identifier(null, "c1"), SortOrder.Asc), new Pair<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                , SortOrder>(new Identifier(null, "c2"), SortOrder.Desc), new Pair<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                , SortOrder>(new Identifier(null, "c3"), SortOrder.Asc)), orderBy.GetOrderByList
                ());
            Assert.AreEqual("ORDER BY c1, c2 DESC, c3", output);
            sql = "order by c1   ";
            lexer = new MySQLLexer(sql);
            parser = GetDMLParser(lexer);
            orderBy = parser.OrderBy();
            output = Output2MySQL(orderBy, sql);
            ListUtil.IsEquals(ListUtil.CreateList(new Pair<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                , SortOrder>(new Identifier(null, "c1"), SortOrder.Asc)), orderBy.GetOrderByList
                ());
            Assert.AreEqual("ORDER BY c1", output);
            sql = "order by c1 asc  ";
            lexer = new MySQLLexer(sql);
            parser = GetDMLParser(lexer);
            orderBy = parser.OrderBy();
            output = Output2MySQL(orderBy, sql);
            ListUtil.IsEquals(ListUtil.CreateList(new Pair<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                , SortOrder>(new Identifier(null, "c1"), SortOrder.Asc)), orderBy.GetOrderByList
                ());
            Assert.AreEqual("ORDER BY c1", output);
            sql = "order by c1 desc  ";
            lexer = new MySQLLexer(sql);
            parser = GetDMLParser(lexer);
            orderBy = parser.OrderBy();
            output = Output2MySQL(orderBy, sql);
            ListUtil.IsEquals(ListUtil.CreateList(new Pair<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                , SortOrder>(new Identifier(null, "c1"), SortOrder.Desc)), orderBy.GetOrderByList
                ());
            Assert.AreEqual("ORDER BY c1 DESC", output);
        }

        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        [Test]
        public virtual void TestLimit()
        {
            string sql = "limit 1,2";
            MySQLLexer lexer = new MySQLLexer(sql);
            MySQLDMLParser parser = GetDMLParser(lexer);
            Limit limit = parser.Limit();
            string output = Output2MySQL(limit, sql);
            Assert.AreEqual(1, limit.GetOffset());
            Assert.AreEqual(2, limit.GetSize());
            Assert.AreEqual("LIMIT 1, 2", output);
            sql = "limit 1,?";
            lexer = new MySQLLexer(sql);
            parser = GetDMLParser(lexer);
            limit = parser.Limit();
            output = Output2MySQL(limit, sql);
            Assert.AreEqual(1, limit.GetOffset());
            Assert.AreEqual(new ParamMarker(1), limit.GetSize());
            Assert.AreEqual("LIMIT 1, ?", output);
            sql = "limit ?,9";
            lexer = new MySQLLexer(sql);
            parser = GetDMLParser(lexer);
            limit = parser.Limit();
            output = Output2MySQL(limit, sql);
            Assert.AreEqual(new ParamMarker(1), limit.GetOffset());
            Assert.AreEqual(9, limit.GetSize());
            Assert.AreEqual("LIMIT ?, 9", output);
            sql = "limit ?,?";
            lexer = new MySQLLexer(sql);
            parser = GetDMLParser(lexer);
            limit = parser.Limit();
            output = Output2MySQL(limit, sql);
            Assert.AreEqual(new ParamMarker(1), limit.GetOffset());
            Assert.AreEqual(new ParamMarker(2), limit.GetSize());
            Assert.AreEqual("LIMIT ?, ?", output);
            sql = "limit ? d";
            lexer = new MySQLLexer(sql);
            parser = GetDMLParser(lexer);
            limit = parser.Limit();
            output = Output2MySQL(limit, sql);
            Assert.AreEqual(0, limit.GetOffset());
            Assert.AreEqual(new ParamMarker(1), limit.GetSize());
            Assert.AreEqual("LIMIT 0, ?", output);
            sql = "limit 9 f";
            lexer = new MySQLLexer(sql);
            parser = GetDMLParser(lexer);
            limit = parser.Limit();
            output = Output2MySQL(limit, sql);
            Assert.AreEqual(0, limit.GetOffset());
            Assert.AreEqual(9, limit.GetSize());
            Assert.AreEqual("LIMIT 0, 9", output);
            sql = "limit 9 ofFset 0";
            lexer = new MySQLLexer(sql);
            parser = GetDMLParser(lexer);
            limit = parser.Limit();
            output = Output2MySQL(limit, sql);
            Assert.AreEqual(0, limit.GetOffset());
            Assert.AreEqual(9, limit.GetSize());
            Assert.AreEqual("LIMIT 0, 9", output);
            sql = "limit ? offset 0";
            lexer = new MySQLLexer(sql);
            parser = GetDMLParser(lexer);
            limit = parser.Limit();
            output = Output2MySQL(limit, sql);
            Assert.AreEqual(0, limit.GetOffset());
            Assert.AreEqual(new ParamMarker(1), limit.GetSize());
            Assert.AreEqual("LIMIT 0, ?", output);
            sql = "limit ? offset ?";
            lexer = new MySQLLexer(sql);
            parser = GetDMLParser(lexer);
            limit = parser.Limit();
            output = Output2MySQL(limit, sql);
            Assert.AreEqual(new ParamMarker(2), limit.GetOffset());
            Assert.AreEqual(new ParamMarker(1), limit.GetSize());
            Assert.AreEqual("LIMIT ?, ?", output);
            sql = "limit 9 offset ?";
            lexer = new MySQLLexer(sql);
            parser = GetDMLParser(lexer);
            limit = parser.Limit();
            output = Output2MySQL(limit, sql);
            Assert.AreEqual(new ParamMarker(1), limit.GetOffset());
            Assert.AreEqual(9, limit.GetSize());
            Assert.AreEqual("LIMIT ?, 9", output);
        }

        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        [Test]
        public virtual void TestGroupBy()
        {
            string sql = "group by c1 asc, c2 desc  , c3 with rollup";
            MySQLLexer lexer = new MySQLLexer(sql);
            MySQLDMLParser parser = GetDMLParser(lexer);
            GroupBy groupBy = parser.GroupBy();
            string output = Output2MySQL(groupBy, sql);
            ListUtil.IsEquals(ListUtil.CreateList(new Pair<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                , SortOrder>(new Identifier(null, "c1"), SortOrder.Asc), new Pair<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                , SortOrder>(new Identifier(null, "c2"), SortOrder.Desc), new Pair<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                , SortOrder>(new Identifier(null, "c3"), SortOrder.Asc)), groupBy.GetOrderByList
                ());
            Assert.AreEqual("GROUP BY c1, c2 DESC, c3 WITH ROLLUP", output);
            sql = "group by c1 asc, c2 desc  , c3 ";
            lexer = new MySQLLexer(sql);
            parser = GetDMLParser(lexer);
            groupBy = parser.GroupBy();
            output = Output2MySQL(groupBy, sql);
            ListUtil.IsEquals(ListUtil.CreateList(new Pair<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                , SortOrder>(new Identifier(null, "c1"), SortOrder.Asc), new Pair<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                , SortOrder>(new Identifier(null, "c2"), SortOrder.Desc), new Pair<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                , SortOrder>(new Identifier(null, "c3"), SortOrder.Asc)), groupBy.GetOrderByList
                ());
            Assert.AreEqual("GROUP BY c1, c2 DESC, c3", output);
            sql = "group by c1   ";
            lexer = new MySQLLexer(sql);
            parser = GetDMLParser(lexer);
            groupBy = parser.GroupBy();
            output = Output2MySQL(groupBy, sql);
            ListUtil.IsEquals(ListUtil.CreateList(new Pair<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                , SortOrder>(new Identifier(null, "c1"), SortOrder.Asc)), groupBy.GetOrderByList
                ());
            Assert.AreEqual("GROUP BY c1", output);
            sql = "group by c1 asc  ";
            lexer = new MySQLLexer(sql);
            parser = GetDMLParser(lexer);
            groupBy = parser.GroupBy();
            output = Output2MySQL(groupBy, sql);
            ListUtil.IsEquals(ListUtil.CreateList(new Pair<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                , SortOrder>(new Identifier(null, "c1"), SortOrder.Asc)), groupBy.GetOrderByList
                ());
            Assert.AreEqual("GROUP BY c1", output);
            sql = "group by c1 desc  ";
            lexer = new MySQLLexer(sql);
            parser = GetDMLParser(lexer);
            groupBy = parser.GroupBy();
            output = Output2MySQL(groupBy, sql);
            ListUtil.IsEquals(ListUtil.CreateList(new Pair<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                , SortOrder>(new Identifier(null, "c1"), SortOrder.Desc)), groupBy.GetOrderByList
                ());
            Assert.AreEqual("GROUP BY c1 DESC", output);
            sql = "group by c1 with rollup  ";
            lexer = new MySQLLexer(sql);
            parser = GetDMLParser(lexer);
            groupBy = parser.GroupBy();
            output = Output2MySQL(groupBy, sql);
            ListUtil.IsEquals(ListUtil.CreateList(new Pair<Tup.Cobar4Net.Parser.Ast.Expression.Expression
                , SortOrder>(new Identifier(null, "c1"), SortOrder.Asc)), groupBy.GetOrderByList
                ());
            Assert.AreEqual("GROUP BY c1 WITH ROLLUP", output);
        }

        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        [Test]
        public virtual void TestTR1()
        {
            string sql = "(select * from `select`) as `select`";
            MySQLLexer lexer = new MySQLLexer(sql);
            MySQLDMLParser parser = GetDMLParser(lexer);
            TableReferences trs = parser.TableRefs();
            string output = Output2MySQL(trs, sql);
            IList<TableReference> list = trs.GetTableReferenceList();
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(typeof(SubqueryFactor), list[0].GetType());
            Assert.AreEqual("(SELECT * FROM `select`) AS `SELECT`", output);
            sql = "(((selecT * from any)union select `select` from `from` order by dd) as 'a1', (((t2)))), t3";
            lexer = new MySQLLexer(sql);
            parser = GetDMLParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySQL(trs, sql);
            list = trs.GetTableReferenceList();
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(typeof(TableReferences), list[0].GetType());
            Assert.AreEqual(typeof(TableRefFactor), list[1].GetType());
            list = ((TableReferences)list[0]).GetTableReferenceList();
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(typeof(SubqueryFactor), list[0].GetType());
            Assert.AreEqual(typeof(TableReferences), list[1].GetType());
            Assert.AreEqual("((SELECT * FROM any) UNION (SELECT `select` FROM `from` ORDER BY dd)) AS 'a1', t2, t3"
                , output);
            sql = "(t1)";
            lexer = new MySQLLexer(sql);
            parser = GetDMLParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySQL(trs, sql);
            list = trs.GetTableReferenceList();
            Assert.AreEqual(1, list.Count);
            TableReference tr = list[0];
            Assert.AreEqual(typeof(TableReferences), tr.GetType());
            list = ((TableReferences)tr).GetTableReferenceList();
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(typeof(TableRefFactor), list[0].GetType());
            Assert.AreEqual("t1", output);
            sql = "(t1,t2,(t3))";
            lexer = new MySQLLexer(sql);
            parser = GetDMLParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySQL(trs, sql);
            list = trs.GetTableReferenceList();
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(typeof(TableReferences), list[0].GetType());
            tr = (TableReferences)list[0];
            Assert.AreEqual(typeof(TableReferences), tr.GetType());
            list = ((TableReferences)tr).GetTableReferenceList();
            Assert.AreEqual(3, list.Count);
            Assert.AreEqual(typeof(TableRefFactor), list[0].GetType());
            Assert.AreEqual(typeof(TableRefFactor), list[1].GetType());
            Assert.AreEqual(typeof(TableRefFactor), list[1].GetType());
            Assert.AreEqual("t1, t2, t3", output);
            sql = "(tb1 as t1)inner join (tb2 as t2) on t1.name=t2.name";
            lexer = new MySQLLexer(sql);
            parser = GetDMLParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySQL(trs, sql);
            list = trs.GetTableReferenceList();
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(typeof(InnerJoin), list[0].GetType());
            tr = ((InnerJoin)list[0]).GetLeftTableRef();
            Assert.AreEqual(typeof(TableReferences), tr.GetType());
            list = ((TableReferences)tr).GetTableReferenceList();
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(typeof(TableRefFactor), list[0].GetType());
            Tup.Cobar4Net.Parser.Ast.Expression.Expression ex = ((InnerJoin)(trs.GetTableReferenceList
                ())[0]).GetOnCond();
            Assert.AreEqual(ex.GetType(), typeof(ComparisionEqualsExpression)
                );
            Assert.AreEqual("(tb1 AS T1) INNER JOIN (tb2 AS T2) ON t1.name = t2.name"
                , output);
            sql = "(tb1 as t1)inner join tb2 as t2 using (c1)";
            lexer = new MySQLLexer(sql);
            parser = GetDMLParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySQL(trs, sql);
            IList<string> using_list = ((InnerJoin)(trs.GetTableReferenceList())[0]).GetUsing
                ();
            Assert.AreEqual(1, using_list.Count);
            Assert.AreEqual("(tb1 AS T1) INNER JOIN tb2 AS T2 USING (c1)", output
                );
            sql = "(tb1 as t1)inner join tb2 as t2 using (c1,c2)";
            lexer = new MySQLLexer(sql);
            parser = GetDMLParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySQL(trs, sql);
            using_list = ((InnerJoin)(trs.GetTableReferenceList())[0]).GetUsing();
            Assert.AreEqual(2, using_list.Count);
            Assert.AreEqual("(tb1 AS T1) INNER JOIN tb2 AS T2 USING (c1, c2)"
                , output);
            sql = "tb1 as t1 use index (i1,i2,i3)";
            lexer = new MySQLLexer(sql);
            parser = GetDMLParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySQL(trs, sql);
            list = trs.GetTableReferenceList();
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(typeof(TableRefFactor), list[0].GetType());
            IList<IndexHint> hintlist = ((TableRefFactor)(trs.GetTableReferenceList())[0]).GetHintList
                ();
            Assert.AreEqual(1, hintlist.Count);
            IndexHint indexhint = hintlist[0];
            Assert.AreEqual(3, indexhint.GetIndexList().Count);
            Assert.AreEqual("USE", indexhint.GetAction().GetEnumName());
            Assert.AreEqual("INDEX", indexhint.GetIndexType().GetEnumName());
            Assert.AreEqual("tb1 AS T1 USE INDEX (i1, i2, i3)", output);
            sql = "tb1 as t1 use index (i1,i2,i3),tb2 as t2 use index (i1,i2,i3)";
            lexer = new MySQLLexer(sql);
            parser = GetDMLParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySQL(trs, sql);
            list = trs.GetTableReferenceList();
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(typeof(TableRefFactor), list[0].GetType());
            hintlist = ((TableRefFactor)(trs.GetTableReferenceList())[0]).GetHintList();
            Assert.AreEqual(1, hintlist.Count);
            indexhint = hintlist[0];
            Assert.AreEqual(3, indexhint.GetIndexList().Count);
            Assert.AreEqual("USE", indexhint.GetAction().GetEnumName());
            Assert.AreEqual("INDEX", indexhint.GetIndexType().GetEnumName());
            hintlist = ((TableRefFactor)(trs.GetTableReferenceList())[1]).GetHintList();
            Assert.AreEqual(1, hintlist.Count);
            indexhint = hintlist[0];
            Assert.AreEqual(3, indexhint.GetIndexList().Count);
            Assert.AreEqual("USE", indexhint.GetAction().GetEnumName());
            Assert.AreEqual("INDEX", indexhint.GetIndexType().GetEnumName());
            Assert.AreEqual("tb1 AS T1 USE INDEX (i1, i2, i3), tb2 AS T2 USE INDEX (i1, i2, i3)"
                , output);
            sql = "tb1 as t1";
            lexer = new MySQLLexer(sql);
            parser = GetDMLParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySQL(trs, sql);
            list = trs.GetTableReferenceList();
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(typeof(TableRefFactor), list[0].GetType());
            Assert.AreEqual("tb1", ((TableRefFactor)(trs.GetTableReferenceList
                ())[0]).GetTable().GetIdText());
            Assert.AreEqual("T1", ((TableRefFactor)(trs.GetTableReferenceList
                ())[0]).GetAlias());
            Assert.AreEqual("tb1 AS T1", output);
            sql = "tb1 t1";
            lexer = new MySQLLexer(sql);
            parser = GetDMLParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySQL(trs, sql);
            list = trs.GetTableReferenceList();
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(typeof(TableRefFactor), list[0].GetType());
            Assert.AreEqual("tb1", ((TableRefFactor)(trs.GetTableReferenceList
                ())[0]).GetTable().GetIdText());
            Assert.AreEqual("T1", ((TableRefFactor)(trs.GetTableReferenceList
                ())[0]).GetAlias());
            Assert.AreEqual("tb1 AS T1", output);
            sql = "tb1,tb2,tb3";
            lexer = new MySQLLexer(sql);
            parser = GetDMLParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySQL(trs, sql);
            list = trs.GetTableReferenceList();
            Assert.AreEqual(3, list.Count);
            Assert.AreEqual(typeof(TableRefFactor), list[0].GetType());
            Assert.AreEqual("tb1", ((TableRefFactor)(trs.GetTableReferenceList
                ())[0]).GetTable().GetIdText());
            Assert.AreEqual(null, ((TableRefFactor)(trs.GetTableReferenceList
                ())[0]).GetAlias());
            Assert.AreEqual("tb2", ((TableRefFactor)(trs.GetTableReferenceList
                ())[1]).GetTable().GetIdText());
            Assert.AreEqual("tb3", ((TableRefFactor)(trs.GetTableReferenceList
                ())[2]).GetTable().GetIdText());
            Assert.AreEqual("tb1, tb2, tb3", output);
            sql = "tb1 use key for join (i1,i2)";
            lexer = new MySQLLexer(sql);
            parser = GetDMLParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySQL(trs, sql);
            list = trs.GetTableReferenceList();
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(typeof(TableRefFactor), list[0].GetType());
            hintlist = ((TableRefFactor)(trs.GetTableReferenceList())[0]).GetHintList();
            Assert.AreEqual(1, hintlist.Count);
            indexhint = hintlist[0];
            Assert.AreEqual(2, indexhint.GetIndexList().Count);
            Assert.AreEqual("USE", indexhint.GetAction().GetEnumName());
            Assert.AreEqual("KEY", indexhint.GetIndexType().GetEnumName());
            Assert.AreEqual("JOIN", indexhint.GetScope().GetEnumName());
            Assert.AreEqual("tb1 USE KEY FOR JOIN (i1, i2)", output);
            sql = "tb1 use index for group by(i1,i2)";
            lexer = new MySQLLexer(sql);
            parser = GetDMLParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySQL(trs, sql);
            list = trs.GetTableReferenceList();
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(typeof(TableRefFactor), list[0].GetType());
            hintlist = ((TableRefFactor)(trs.GetTableReferenceList())[0]).GetHintList();
            Assert.AreEqual(1, hintlist.Count);
            indexhint = hintlist[0];
            Assert.AreEqual(2, indexhint.GetIndexList().Count);
            Assert.AreEqual("USE", indexhint.GetAction().GetEnumName());
            Assert.AreEqual("INDEX", indexhint.GetIndexType().GetEnumName());
            Assert.AreEqual("GROUP_BY", indexhint.GetScope().GetEnumName());
            Assert.AreEqual("tb1 USE INDEX FOR GROUP BY (i1, i2)", output);
            sql = "tb1 use key for order by (i1,i2) use key for group by () " + "ignore index for group by (i1,i2)";
            lexer = new MySQLLexer(sql);
            parser = GetDMLParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySQL(trs, sql);
            list = trs.GetTableReferenceList();
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(typeof(TableRefFactor), list[0].GetType());
            Assert.AreEqual("tb1", ((TableRefFactor)(trs.GetTableReferenceList
                ())[0]).GetTable().GetIdText());
            Assert.AreEqual(null, ((TableRefFactor)(trs.GetTableReferenceList
                ())[0]).GetAlias());
            hintlist = ((TableRefFactor)(trs.GetTableReferenceList())[0]).GetHintList();
            Assert.AreEqual(3, hintlist.Count);
            indexhint = hintlist[0];
            Assert.AreEqual(2, indexhint.GetIndexList().Count);
            Assert.AreEqual("USE", indexhint.GetAction().GetEnumName());
            Assert.AreEqual("KEY", indexhint.GetIndexType().GetEnumName());
            Assert.AreEqual("ORDER_BY", indexhint.GetScope().GetEnumName());
            indexhint = hintlist[1];
            Assert.AreEqual(0, indexhint.GetIndexList().Count);
            Assert.AreEqual("USE", indexhint.GetAction().GetEnumName());
            Assert.AreEqual("KEY", indexhint.GetIndexType().GetEnumName());
            Assert.AreEqual("GROUP_BY", indexhint.GetScope().GetEnumName());
            indexhint = hintlist[2];
            Assert.AreEqual(2, indexhint.GetIndexList().Count);
            Assert.AreEqual("IGNORE", indexhint.GetAction().GetEnumName());
            Assert.AreEqual("INDEX", indexhint.GetIndexType().GetEnumName());
            Assert.AreEqual("GROUP_BY", indexhint.GetScope().GetEnumName());
            Assert.AreEqual("tb1 USE KEY FOR ORDER BY (i1, i2) " + "USE KEY FOR GROUP BY () IGNORE INDEX FOR GROUP BY (i1, i2)"
                , output);
            sql = "tb1 use index for order by (i1,i2) force index for group by (i1)";
            lexer = new MySQLLexer(sql);
            parser = GetDMLParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySQL(trs, sql);
            list = trs.GetTableReferenceList();
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(typeof(TableRefFactor), list[0].GetType());
            Assert.AreEqual("tb1", ((TableRefFactor)(trs.GetTableReferenceList
                ())[0]).GetTable().GetIdText());
            Assert.AreEqual(null, ((TableRefFactor)(trs.GetTableReferenceList
                ())[0]).GetAlias());
            hintlist = ((TableRefFactor)(trs.GetTableReferenceList())[0]).GetHintList();
            Assert.AreEqual(2, hintlist.Count);
            indexhint = hintlist[0];
            Assert.AreEqual(2, indexhint.GetIndexList().Count);
            Assert.AreEqual("i1", indexhint.GetIndexList()[0]);
            Assert.AreEqual("i2", indexhint.GetIndexList()[1]);
            Assert.AreEqual("USE", indexhint.GetAction().GetEnumName());
            Assert.AreEqual("INDEX", indexhint.GetIndexType().GetEnumName());
            Assert.AreEqual("ORDER_BY", indexhint.GetScope().GetEnumName());
            indexhint = hintlist[1];
            Assert.AreEqual(1, indexhint.GetIndexList().Count);
            Assert.AreEqual("FORCE", indexhint.GetAction().GetEnumName());
            Assert.AreEqual("INDEX", indexhint.GetIndexType().GetEnumName());
            Assert.AreEqual("GROUP_BY", indexhint.GetScope().GetEnumName());
            Assert.AreEqual("tb1 USE INDEX FOR ORDER BY (i1, i2) FORCE INDEX FOR GROUP BY (i1)"
                , output);
            sql = "tb1 ignore key for join (i1,i2)";
            lexer = new MySQLLexer(sql);
            parser = GetDMLParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySQL(trs, sql);
            list = trs.GetTableReferenceList();
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(typeof(TableRefFactor), list[0].GetType());
            Assert.AreEqual("tb1", ((TableRefFactor)(trs.GetTableReferenceList
                ())[0]).GetTable().GetIdText());
            Assert.AreEqual(null, ((TableRefFactor)(trs.GetTableReferenceList
                ())[0]).GetAlias());
            hintlist = ((TableRefFactor)(trs.GetTableReferenceList())[0]).GetHintList();
            Assert.AreEqual(1, hintlist.Count);
            indexhint = hintlist[0];
            Assert.AreEqual(2, indexhint.GetIndexList().Count);
            Assert.AreEqual("i1", indexhint.GetIndexList()[0]);
            Assert.AreEqual("i2", indexhint.GetIndexList()[1]);
            Assert.AreEqual("IGNORE", indexhint.GetAction().GetEnumName());
            Assert.AreEqual("KEY", indexhint.GetIndexType().GetEnumName());
            Assert.AreEqual("JOIN", indexhint.GetScope().GetEnumName());
            Assert.AreEqual("tb1 IGNORE KEY FOR JOIN (i1, i2)", output);
            sql = "tb1 ignore index for group by (i1,i2)";
            lexer = new MySQLLexer(sql);
            parser = GetDMLParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySQL(trs, sql);
            list = trs.GetTableReferenceList();
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(typeof(TableRefFactor), list[0].GetType());
            Assert.AreEqual("tb1", ((TableRefFactor)(trs.GetTableReferenceList
                ())[0]).GetTable().GetIdText());
            Assert.AreEqual(null, ((TableRefFactor)(trs.GetTableReferenceList
                ())[0]).GetAlias());
            hintlist = ((TableRefFactor)(trs.GetTableReferenceList())[0]).GetHintList();
            Assert.AreEqual(1, hintlist.Count);
            indexhint = hintlist[0];
            Assert.AreEqual(2, indexhint.GetIndexList().Count);
            Assert.AreEqual("i1", indexhint.GetIndexList()[0]);
            Assert.AreEqual("i2", indexhint.GetIndexList()[1]);
            Assert.AreEqual("IGNORE", indexhint.GetAction().GetEnumName());
            Assert.AreEqual("INDEX", indexhint.GetIndexType().GetEnumName());
            Assert.AreEqual("GROUP_BY", indexhint.GetScope().GetEnumName());
            Assert.AreEqual("tb1 IGNORE INDEX FOR GROUP BY (i1, i2)", output);
            sql = "(offer  a  straight_join wp_image b use key for join(t1,t2) on a.member_id=b.member_id inner join product_visit c )";
            lexer = new MySQLLexer(sql);
            parser = GetDMLParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySQL(trs, sql);
            Assert.AreEqual("offer AS A STRAIGHT_JOIN wp_image AS B USE KEY FOR JOIN (t1, t2) ON a.member_id = b.member_id INNER JOIN product_visit AS C"
                , output);
            sql = "tb1 ignore index for order by(i1)";
            lexer = new MySQLLexer(sql);
            parser = GetDMLParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySQL(trs, sql);
            list = trs.GetTableReferenceList();
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(typeof(TableRefFactor), list[0].GetType());
            Assert.AreEqual("tb1", ((TableRefFactor)(trs.GetTableReferenceList
                ())[0]).GetTable().GetIdText());
            Assert.AreEqual(null, ((TableRefFactor)(trs.GetTableReferenceList
                ())[0]).GetAlias());
            hintlist = ((TableRefFactor)(trs.GetTableReferenceList())[0]).GetHintList();
            Assert.AreEqual(1, hintlist.Count);
            indexhint = hintlist[0];
            Assert.AreEqual(1, indexhint.GetIndexList().Count);
            Assert.AreEqual("i1", indexhint.GetIndexList()[0]);
            Assert.AreEqual("IGNORE", indexhint.GetAction().GetEnumName());
            Assert.AreEqual("INDEX", indexhint.GetIndexType().GetEnumName());
            Assert.AreEqual("ORDER_BY", indexhint.GetScope().GetEnumName());
            Assert.AreEqual("tb1 IGNORE INDEX FOR ORDER BY (i1)", output);
            sql = "tb1 force key for group by (i1,i2)";
            lexer = new MySQLLexer(sql);
            parser = GetDMLParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySQL(trs, sql);
            list = trs.GetTableReferenceList();
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(typeof(TableRefFactor), list[0].GetType());
            Assert.AreEqual("tb1", ((TableRefFactor)(trs.GetTableReferenceList
                ())[0]).GetTable().GetIdText());
            Assert.AreEqual(null, ((TableRefFactor)(trs.GetTableReferenceList
                ())[0]).GetAlias());
            hintlist = ((TableRefFactor)(trs.GetTableReferenceList())[0]).GetHintList();
            Assert.AreEqual(1, hintlist.Count);
            indexhint = hintlist[0];
            Assert.AreEqual(2, indexhint.GetIndexList().Count);
            Assert.AreEqual("i1", indexhint.GetIndexList()[0]);
            Assert.AreEqual("i2", indexhint.GetIndexList()[1]);
            Assert.AreEqual("FORCE", indexhint.GetAction().GetEnumName());
            Assert.AreEqual("KEY", indexhint.GetIndexType().GetEnumName());
            Assert.AreEqual("GROUP_BY", indexhint.GetScope().GetEnumName());
            Assert.AreEqual("tb1 FORCE KEY FOR GROUP BY (i1, i2)", output);
            sql = "tb1 force index for group by (i1,i2)";
            lexer = new MySQLLexer(sql);
            parser = GetDMLParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySQL(trs, sql);
            list = trs.GetTableReferenceList();
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(typeof(TableRefFactor), list[0].GetType());
            Assert.AreEqual("tb1", ((TableRefFactor)(trs.GetTableReferenceList
                ())[0]).GetTable().GetIdText());
            Assert.AreEqual(null, ((TableRefFactor)(trs.GetTableReferenceList
                ())[0]).GetAlias());
            hintlist = ((TableRefFactor)(trs.GetTableReferenceList())[0]).GetHintList();
            Assert.AreEqual(1, hintlist.Count);
            indexhint = hintlist[0];
            Assert.AreEqual(2, indexhint.GetIndexList().Count);
            Assert.AreEqual("i1", indexhint.GetIndexList()[0]);
            Assert.AreEqual("i2", indexhint.GetIndexList()[1]);
            Assert.AreEqual("FORCE", indexhint.GetAction().GetEnumName());
            Assert.AreEqual("INDEX", indexhint.GetIndexType().GetEnumName());
            Assert.AreEqual("GROUP_BY", indexhint.GetScope().GetEnumName());
            Assert.AreEqual("tb1 FORCE INDEX FOR GROUP BY (i1, i2)", output);
            sql = "tb1 force index for join (i1,i2)";
            lexer = new MySQLLexer(sql);
            parser = GetDMLParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySQL(trs, sql);
            list = trs.GetTableReferenceList();
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(typeof(TableRefFactor), list[0].GetType());
            Assert.AreEqual("tb1", ((TableRefFactor)(trs.GetTableReferenceList
                ())[0]).GetTable().GetIdText());
            Assert.AreEqual(null, ((TableRefFactor)(trs.GetTableReferenceList
                ())[0]).GetAlias());
            hintlist = ((TableRefFactor)(trs.GetTableReferenceList())[0]).GetHintList();
            Assert.AreEqual(1, hintlist.Count);
            indexhint = hintlist[0];
            Assert.AreEqual(2, indexhint.GetIndexList().Count);
            Assert.AreEqual("i1", indexhint.GetIndexList()[0]);
            Assert.AreEqual("i2", indexhint.GetIndexList()[1]);
            Assert.AreEqual("FORCE", indexhint.GetAction().GetEnumName());
            Assert.AreEqual("INDEX", indexhint.GetIndexType().GetEnumName());
            Assert.AreEqual("JOIN", indexhint.GetScope().GetEnumName());
            Assert.AreEqual("tb1 FORCE INDEX FOR JOIN (i1, i2)", output);
            sql = "(tb1 force index for join (i1,i2) )left outer join tb2 as t2 " + "use index (i1,i2,i3) on t1.id1=t2.id1";
            lexer = new MySQLLexer(sql);
            parser = GetDMLParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySQL(trs, sql);
            list = trs.GetTableReferenceList();
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(typeof(OuterJoin), list[0].GetType());
            Assert.AreEqual(true, (((OuterJoin)list[0]).IsLeftJoin()));
            TableReferences ltr = (TableReferences)((OuterJoin)list[0]).GetLeftTableRef();
            Assert.AreEqual(1, ltr.GetTableReferenceList().Count);
            Assert.AreEqual(typeof(TableRefFactor), ltr.GetTableReferenceList
                ()[0].GetType());
            Assert.AreEqual(null, ((TableRefFactor)(ltr.GetTableReferenceList
                ()[0])).GetAlias());
            Assert.AreEqual("tb1", ((TableRefFactor)(ltr.GetTableReferenceList
                ()[0])).GetTable().GetIdText());
            hintlist = ((TableRefFactor)(ltr.GetTableReferenceList())[0]).GetHintList();
            Assert.AreEqual(1, hintlist.Count);
            indexhint = hintlist[0];
            Assert.AreEqual(2, indexhint.GetIndexList().Count);
            Assert.AreEqual("i1", indexhint.GetIndexList()[0]);
            Assert.AreEqual("i2", indexhint.GetIndexList()[1]);
            Assert.AreEqual("FORCE", indexhint.GetAction().GetEnumName());
            Assert.AreEqual("INDEX", indexhint.GetIndexType().GetEnumName());
            Assert.AreEqual("JOIN", indexhint.GetScope().GetEnumName());
            TableRefFactor rtf = (TableRefFactor)((OuterJoin)list[0]).GetRightTableRef();
            Assert.AreEqual("T2", rtf.GetAlias());
            Assert.AreEqual("tb2", rtf.GetTable().GetIdText());
            hintlist = rtf.GetHintList();
            Assert.AreEqual(1, hintlist.Count);
            indexhint = hintlist[0];
            Assert.AreEqual(3, indexhint.GetIndexList().Count);
            Assert.AreEqual("i1", indexhint.GetIndexList()[0]);
            Assert.AreEqual("i2", indexhint.GetIndexList()[1]);
            Assert.AreEqual("USE", indexhint.GetAction().GetEnumName());
            Assert.AreEqual("INDEX", indexhint.GetIndexType().GetEnumName());
            Assert.AreEqual("ALL", indexhint.GetScope().GetEnumName());
            Assert.AreEqual(typeof(ComparisionEqualsExpression), ((OuterJoin)
                list[0]).GetOnCond().GetType());
            Assert.AreEqual("(tb1 FORCE INDEX FOR JOIN (i1, i2)) " + "LEFT JOIN tb2 AS T2 USE INDEX (i1, i2, i3) ON t1.id1 = t2.id1"
                , output);
            sql = " (((tb1 force index for join (i1,i2),tb3),tb4),tb5) " + "left outer join (tb2 as t2 use index (i1,i2,i3)) using(id1)";
            lexer = new MySQLLexer(sql);
            parser = GetDMLParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySQL(trs, sql);
            list = trs.GetTableReferenceList();
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(typeof(OuterJoin), list[0].GetType());
            Assert.AreEqual(true, (((OuterJoin)list[0]).IsLeftJoin()));
            ltr = (TableReferences)((OuterJoin)list[0]).GetLeftTableRef();
            Assert.AreEqual(2, ltr.GetTableReferenceList().Count);
            Assert.AreEqual(typeof(TableReferences), ltr.GetTableReferenceList
                ()[0].GetType());
            TableReferences ltr1 = (TableReferences)(ltr.GetTableReferenceList())[0];
            Assert.AreEqual(2, ltr1.GetTableReferenceList().Count);
            Assert.AreEqual(typeof(TableReferences), ltr1.GetTableReferenceList
                ()[0].GetType());
            TableReferences ltr2 = (TableReferences)(ltr1.GetTableReferenceList())[0];
            Assert.AreEqual(2, ltr2.GetTableReferenceList().Count);
            Assert.AreEqual(typeof(TableRefFactor), ltr2.GetTableReferenceList
                ()[0].GetType());
            Assert.AreEqual(null, ((TableRefFactor)(ltr2.GetTableReferenceList
                ()[0])).GetAlias());
            Assert.AreEqual("tb1", ((TableRefFactor)(ltr2.GetTableReferenceList
                ()[0])).GetTable().GetIdText());
            hintlist = ((TableRefFactor)(ltr2.GetTableReferenceList())[0]).GetHintList();
            Assert.AreEqual(1, hintlist.Count);
            indexhint = hintlist[0];
            Assert.AreEqual(2, indexhint.GetIndexList().Count);
            Assert.AreEqual("i1", indexhint.GetIndexList()[0]);
            Assert.AreEqual("i2", indexhint.GetIndexList()[1]);
            Assert.AreEqual("FORCE", indexhint.GetAction().GetEnumName());
            Assert.AreEqual("INDEX", indexhint.GetIndexType().GetEnumName());
            Assert.AreEqual("JOIN", indexhint.GetScope().GetEnumName());
            Assert.AreEqual(typeof(TableRefFactor), ltr2.GetTableReferenceList
                ()[1].GetType());
            Assert.AreEqual("tb3", ((TableRefFactor)(ltr2.GetTableReferenceList
                ()[1])).GetTable().GetIdText());
            Assert.AreEqual(typeof(TableRefFactor), ltr1.GetTableReferenceList
                ()[1].GetType());
            Assert.AreEqual("tb4", ((TableRefFactor)(ltr1.GetTableReferenceList
                ()[1])).GetTable().GetIdText());
            Assert.AreEqual(typeof(TableRefFactor), ltr.GetTableReferenceList
                ()[1].GetType());
            Assert.AreEqual("tb5", ((TableRefFactor)(ltr.GetTableReferenceList
                ()[1])).GetTable().GetIdText());
            TableReferences rtr = (TableReferences)((OuterJoin)list[0]).GetRightTableRef();
            Assert.AreEqual("T2", ((TableRefFactor)rtr.GetTableReferenceList(
                )[0]).GetAlias());
            Assert.AreEqual("tb2", ((TableRefFactor)rtr.GetTableReferenceList
                ()[0]).GetTable().GetIdText());
            hintlist = ((TableRefFactor)rtr.GetTableReferenceList()[0]).GetHintList();
            Assert.AreEqual(1, hintlist.Count);
            indexhint = hintlist[0];
            Assert.AreEqual(3, indexhint.GetIndexList().Count);
            Assert.AreEqual("i1", indexhint.GetIndexList()[0]);
            Assert.AreEqual("i2", indexhint.GetIndexList()[1]);
            Assert.AreEqual("USE", indexhint.GetAction().GetEnumName());
            Assert.AreEqual("INDEX", indexhint.GetIndexType().GetEnumName());
            Assert.AreEqual("ALL", indexhint.GetScope().GetEnumName());
            using_list = ((OuterJoin)(trs.GetTableReferenceList())[0]).GetUsing();
            Assert.AreEqual(1, using_list.Count);
            Assert.AreEqual("(tb1 FORCE INDEX FOR JOIN (i1, i2), tb3, tb4, tb5) "
                 + "LEFT JOIN (tb2 AS T2 USE INDEX (i1, i2, i3)) USING (id1)", output);
            sql = "(tb1 force index for join (i1,i2),tb3) " + "left outer join tb2 as t2 use index (i1,i2,i3) using(id1,id2)";
            lexer = new MySQLLexer(sql);
            parser = GetDMLParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySQL(trs, sql);
            Assert.AreEqual("(tb1 FORCE INDEX FOR JOIN (i1, i2), tb3) " + "LEFT JOIN tb2 AS T2 USE INDEX (i1, i2, i3) USING (id1, id2)"
                , output);
            sql = "(tb1 force index for join (i1,i2),tb3) left outer join (tb2 as t2 use index (i1,i2,i3)) using(id1,id2)";
            lexer = new MySQLLexer(sql);
            parser = GetDMLParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySQL(trs, sql);
            Assert.AreEqual("(tb1 FORCE INDEX FOR JOIN (i1, i2), tb3) " + "LEFT JOIN (tb2 AS T2 USE INDEX (i1, i2, i3)) USING (id1, id2)"
                , output);
            sql = "tb1 as t1 cross join tb2 as t2 use index(i1)using(id1)";
            lexer = new MySQLLexer(sql);
            parser = GetDMLParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySQL(trs, sql);
            Assert.AreEqual("tb1 AS T1 INNER JOIN tb2 AS T2 USE INDEX (i1) USING (id1)"
                , output);
            sql = "(tb1 as t1) cross join tb2 as t2 use index(i1)using(id1)";
            lexer = new MySQLLexer(sql);
            parser = GetDMLParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySQL(trs, sql);
            Assert.AreEqual("(tb1 AS T1) INNER JOIN tb2 AS T2 USE INDEX (i1) USING (id1)"
                , output);
            sql = "tb1 as _latin't1' cross join tb2 as t2 use index(i1)";
            lexer = new MySQLLexer(sql);
            parser = GetDMLParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySQL(trs, sql);
            Assert.AreEqual("tb1 AS _LATIN't1' INNER JOIN tb2 AS T2 USE INDEX (i1)"
                , output);
            sql = "((select '  @  from' from `from`)) as t1 cross join tb2 as t2 use index()";
            lexer = new MySQLLexer(sql);
            parser = GetDMLParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySQL(trs, sql);
            list = trs.GetTableReferenceList();
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(typeof(InnerJoin), list[0].GetType());
            SubqueryFactor lsf = (SubqueryFactor)((InnerJoin)list[0]).GetLeftTableRef();
            Assert.AreEqual("T1", lsf.GetAlias());
            Assert.AreEqual(typeof(DMLSelectStatement), lsf.GetSubquery().GetType
                ());
            rtf = (TableRefFactor)((InnerJoin)list[0]).GetRightTableRef();
            Assert.AreEqual("T2", rtf.GetAlias());
            hintlist = rtf.GetHintList();
            Assert.AreEqual(1, hintlist.Count);
            indexhint = hintlist[0];
            Assert.AreEqual("USE", indexhint.GetAction().GetEnumName());
            Assert.AreEqual("INDEX", indexhint.GetIndexType().GetEnumName());
            Assert.AreEqual("ALL", indexhint.GetScope().GetEnumName());
            Assert.AreEqual("tb2", rtf.GetTable().GetIdText());
            Assert.AreEqual("(SELECT '  @  from' FROM `from`) AS T1 " + "INNER JOIN tb2 AS T2 USE INDEX ()"
                , output);
            sql = "(tb1 as t1) straight_join (tb2 as t2)";
            lexer = new MySQLLexer(sql);
            parser = GetDMLParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySQL(trs, sql);
            Assert.AreEqual("(tb1 AS T1) STRAIGHT_JOIN (tb2 AS T2)", output);
            sql = "tb1 straight_join tb2 as t2 on tb1.id=tb2.id";
            lexer = new MySQLLexer(sql);
            parser = GetDMLParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySQL(trs, sql);
            Assert.AreEqual("tb1 STRAIGHT_JOIN tb2 AS T2 ON tb1.id = tb2.id",
                output);
            sql = "tb1 left outer join tb2 on tb1.id=tb2.id";
            lexer = new MySQLLexer(sql);
            parser = GetDMLParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySQL(trs, sql);
            Assert.AreEqual("tb1 LEFT JOIN tb2 ON tb1.id = tb2.id", output);
            sql = "tb1 left outer join tb2 using(id)";
            lexer = new MySQLLexer(sql);
            parser = GetDMLParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySQL(trs, sql);
            Assert.AreEqual("tb1 LEFT JOIN tb2 USING (id)", output);
            sql = "(tb1 right outer join tb2 using()) join tb3 on tb1.id=tb2.id and tb2.id=tb3.id";
            lexer = new MySQLLexer(sql);
            parser = GetDMLParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySQL(trs, sql);
            list = trs.GetTableReferenceList();
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(typeof(InnerJoin), list[0].GetType());
            ltr = (TableReferences)((InnerJoin)list[0]).GetLeftTableRef();
            Assert.AreEqual(1, ltr.GetTableReferenceList().Count);
            TableRefFactor lltrf = (TableRefFactor)((OuterJoin)ltr.GetTableReferenceList()[0]
                ).GetLeftTableRef();
            Assert.AreEqual(null, lltrf.GetAlias());
            Assert.AreEqual("tb1", lltrf.GetTable().GetIdText());
            using_list = ((OuterJoin)ltr.GetTableReferenceList()[0]).GetUsing();
            Assert.AreEqual(0, using_list.Count);
            rtf = (TableRefFactor)((InnerJoin)list[0]).GetRightTableRef();
            Assert.AreEqual(null, rtf.GetAlias());
            hintlist = rtf.GetHintList();
            Assert.AreEqual(0, hintlist.Count);
            Assert.AreEqual("tb3", rtf.GetTable().GetIdText());
            Assert.AreEqual(typeof(LogicalAndExpression), ((InnerJoin)list[0]
                ).GetOnCond().GetType());
            Assert.AreEqual("(tb1 RIGHT JOIN tb2 USING ()) " + "INNER JOIN tb3 ON tb1.id = tb2.id AND tb2.id = tb3.id"
                , output);
            sql = "tb1 right outer join tb2 using(id1,id2) " + "join (tb3,tb4) on tb1.id=tb2.id and tb2.id=tb3.id";
            lexer = new MySQLLexer(sql);
            parser = GetDMLParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySQL(trs, sql);
            list = trs.GetTableReferenceList();
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(typeof(InnerJoin), list[0].GetType());
            OuterJoin loj = (OuterJoin)((InnerJoin)list[0]).GetLeftTableRef();
            lltrf = (TableRefFactor)loj.GetLeftTableRef();
            Assert.AreEqual(null, lltrf.GetAlias());
            Assert.AreEqual("tb1", lltrf.GetTable().GetIdText());
            using_list = loj.GetUsing();
            Assert.AreEqual(2, using_list.Count);
            rtr = (TableReferences)((InnerJoin)list[0]).GetRightTableRef();
            Assert.AreEqual(2, rtr.GetTableReferenceList().Count);
            Assert.AreEqual("tb3", ((TableRefFactor)(rtr.GetTableReferenceList
                ()[0])).GetTable().GetIdText());
            Assert.AreEqual("tb4", ((TableRefFactor)(rtr.GetTableReferenceList
                ()[1])).GetTable().GetIdText());
            Assert.AreEqual(typeof(LogicalAndExpression), ((InnerJoin)list[0]
                ).GetOnCond().GetType());
            Assert.AreEqual("tb1 RIGHT JOIN tb2 USING (id1, id2) " + "INNER JOIN (tb3, tb4) ON tb1.id = tb2.id AND tb2.id = tb3.id"
                , output);
            sql = "tb1 left outer join tb2 join tb3 using(id)";
            lexer = new MySQLLexer(sql);
            parser = GetDMLParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySQL(trs, sql);
            Assert.AreEqual("tb1 LEFT JOIN (tb2 INNER JOIN tb3) USING (id)",
                output);
            sql = "tb1 right join tb2 on tb1.id=tb2.id";
            lexer = new MySQLLexer(sql);
            parser = GetDMLParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySQL(trs, sql);
            Assert.AreEqual("tb1 RIGHT JOIN tb2 ON tb1.id = tb2.id", output);
            sql = "tb1 natural right join tb2 ";
            lexer = new MySQLLexer(sql);
            parser = GetDMLParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySQL(trs, sql);
            Assert.AreEqual("tb1 NATURAL RIGHT JOIN tb2", output);
            sql = "tb1 natural right outer join tb2 natural left outer join tb3";
            lexer = new MySQLLexer(sql);
            parser = GetDMLParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySQL(trs, sql);
            list = trs.GetTableReferenceList();
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(typeof(NaturalJoin), list[0].GetType());
            NaturalJoin lnj = (NaturalJoin)((NaturalJoin)list[0]).GetLeftTableRef();
            lltrf = (TableRefFactor)lnj.GetLeftTableRef();
            Assert.AreEqual(null, lltrf.GetAlias());
            Assert.AreEqual("tb1", lltrf.GetTable().GetIdText());
            TableRefFactor rltrf = (TableRefFactor)lnj.GetRightTableRef();
            Assert.AreEqual(null, rltrf.GetAlias());
            Assert.AreEqual("tb2", rltrf.GetTable().GetIdText());
            rtf = (TableRefFactor)((NaturalJoin)list[0]).GetRightTableRef();
            Assert.AreEqual(null, rtf.GetAlias());
            Assert.AreEqual("tb3", rtf.GetTable().GetIdText());
            Assert.AreEqual("tb1 NATURAL RIGHT JOIN tb2 NATURAL LEFT JOIN tb3"
                , output);
            sql = "tb1 natural left outer join tb2 ";
            lexer = new MySQLLexer(sql);
            parser = GetDMLParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySQL(trs, sql);
            Assert.AreEqual("tb1 NATURAL LEFT JOIN tb2", output);
            sql = "(tb1  t1) natural  join (tb2 as t2) ";
            lexer = new MySQLLexer(sql);
            parser = GetDMLParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySQL(trs, sql);
            Assert.AreEqual("(tb1 AS T1) NATURAL JOIN (tb2 AS T2)", output);
            sql = "(select (select * from tb1) from `select` " + "where `any`=any(select id2 from tb2))any  ";
            lexer = new MySQLLexer(sql);
            parser = GetDMLParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySQL(trs, sql);
            list = trs.GetTableReferenceList();
            Assert.AreEqual(typeof(SubqueryFactor), list[0].GetType());
            Assert.AreEqual("ANY", ((SubqueryFactor)list[0]).GetAlias());
            Assert.AreEqual("(SELECT SELECT * FROM tb1 FROM `select` " + "WHERE `any` = ANY (SELECT id2 FROM tb2)) AS ANY"
                , output);
            sql = "((tb1),(tb3 as t3,`select`),tb2 use key for join (i1,i2))" + " left join tb4 join tb5 using ()";
            lexer = new MySQLLexer(sql);
            parser = GetDMLParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySQL(trs, sql);
            list = trs.GetTableReferenceList();
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(typeof(OuterJoin), list[0].GetType());
            Assert.AreEqual(typeof(TableReferences), ((OuterJoin)list[0]).GetLeftTableRef
                ().GetType());
            Assert.AreEqual(typeof(InnerJoin), ((OuterJoin)list[0]).GetRightTableRef
                ().GetType());
            list = ((TableReferences)((OuterJoin)list[0]).GetLeftTableRef()).GetTableReferenceList
                ();
            list = ((TableReferences)list[1]).GetTableReferenceList();
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("(tb1, tb3 AS T3, `select`, tb2 USE KEY FOR JOIN (i1, i2))"
                 + " LEFT JOIN (tb4 INNER JOIN tb5) USING ()", output);
            sql = "((select `select` from `from` ) tb1),(tb3 as t3,`select`),tb2 use key for join (i1,i2) "
                 + "left join tb4 using (i1,i2)straight_join tb5";
            lexer = new MySQLLexer(sql);
            parser = GetDMLParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySQL(trs, sql);
            list = trs.GetTableReferenceList();
            Assert.AreEqual(3, list.Count);
            Assert.AreEqual(typeof(TableReferences), list[0].GetType());
            Assert.AreEqual(typeof(TableReferences), list[1].GetType());
            Assert.AreEqual(typeof(StraightJoin), list[2].GetType());
            list = ((TableReferences)list[0]).GetTableReferenceList();
            Assert.AreEqual(typeof(SubqueryFactor), list[0].GetType());
            list = trs.GetTableReferenceList();
            list = ((TableReferences)list[1]).GetTableReferenceList();
            Assert.AreEqual(typeof(TableRefFactor), list[0].GetType());
            Assert.AreEqual(typeof(TableRefFactor), list[1].GetType());
            list = trs.GetTableReferenceList();
            StraightJoin sj = (StraightJoin)list[2];
            Assert.AreEqual(typeof(OuterJoin), sj.GetLeftTableRef().GetType()
                );
            Assert.AreEqual(typeof(TableRefFactor), sj.GetRightTableRef().GetType
                ());
            OuterJoin oj = (OuterJoin)sj.GetLeftTableRef();
            using_list = oj.GetUsing();
            Assert.AreEqual(2, using_list.Count);
            Assert.AreEqual("(SELECT `select` FROM `from`) AS TB1, tb3 AS T3, `select`, tb2 USE KEY FOR JOIN (i1, i2) LEFT JOIN tb4 USING (i1, i2) STRAIGHT_JOIN tb5"
                , output);
            sql = "(`select`,(tb1 as t1 use index for join()ignore key for group by (i1)))" +
                 "join tb2 on cd1=any " + "right join " + "tb3 straight_join " + "(tb4 use index() left outer join (tb6,tb7) on id3=all(select `all` from `all`)) "
                 + " on id2=any(select * from any) using  (i1)";
            lexer = new MySQLLexer(sql);
            parser = GetDMLParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySQL(trs, sql);
            list = trs.GetTableReferenceList();
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(typeof(OuterJoin), list[0].GetType());
            using_list = ((OuterJoin)list[0]).GetUsing();
            Assert.AreEqual(1, using_list.Count);
            Assert.AreEqual(typeof(InnerJoin), ((OuterJoin)(list[0])).GetLeftTableRef
                ().GetType());
            Assert.AreEqual(typeof(StraightJoin), ((OuterJoin)(list[0])).GetRightTableRef
                ().GetType());
            StraightJoin rsj = (StraightJoin)((OuterJoin)(list[0])).GetRightTableRef();
            Assert.AreEqual(typeof(TableRefFactor), rsj.GetLeftTableRef().GetType
                ());
            Assert.AreEqual(typeof(TableReferences), rsj.GetRightTableRef().GetType
                ());
            list = ((TableReferences)rsj.GetRightTableRef()).GetTableReferenceList();
            Assert.AreEqual(typeof(OuterJoin), list[0].GetType());
            Assert.AreEqual("(`select`, tb1 AS T1 USE INDEX FOR JOIN () IGNORE KEY FOR GROUP BY (i1)) "
                 + "INNER JOIN tb2 ON cd1 = any RIGHT JOIN (tb3 STRAIGHT_JOIN (tb4 USE INDEX () "
                 + "LEFT JOIN (tb6, tb7) ON id3 = ALL (SELECT `all` FROM `all`)) ON id2 = ANY (SELECT * FROM any))"
                 + " USING (i1)", output);
        }
    }
}
