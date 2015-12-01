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
using Tup.Cobar4Net.Parser.Ast.Expression;
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
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    [TestFixture(Category = "MySqlDmlParserTest")]
    public class MySqlDmlParserTest : AbstractSyntaxTest
    {
        protected internal virtual MySqlDmlParser GetDmlParser(MySqlLexer lexer)
        {
            var exp = new MySqlExprParser(lexer);
            MySqlDmlParser parser = new MySqlDmlSelectParser(lexer, exp);
            return parser;
        }

        /// <exception cref="System.SqlSyntaxErrorException" />
        [Test]
        public virtual void TestGroupBy()
        {
            var sql = "group by c1 asc, c2 desc  , c3 with rollup";
            var lexer = new MySqlLexer(sql);
            var parser = GetDmlParser(lexer);
            var groupBy = parser.GroupBy();
            var output = Output2MySql(groupBy, sql);
            ListUtil.IsEquals(
                ListUtil.CreateList(new Pair<IExpression, SortOrder>(new Identifier(null, "c1"), SortOrder.Asc),
                    new Pair<IExpression, SortOrder>(new Identifier(null, "c2"), SortOrder.Desc),
                    new Pair<IExpression, SortOrder>(new Identifier(null, "c3"), SortOrder.Asc)), groupBy.OrderByList);
            Assert.AreEqual("GROUP BY c1, c2 DESC, c3 WITH ROLLUP", output);
            sql = "group by c1 asc, c2 desc  , c3 ";
            lexer = new MySqlLexer(sql);
            parser = GetDmlParser(lexer);
            groupBy = parser.GroupBy();
            output = Output2MySql(groupBy, sql);
            ListUtil.IsEquals(
                ListUtil.CreateList(new Pair<IExpression, SortOrder>(new Identifier(null, "c1"), SortOrder.Asc),
                    new Pair<IExpression, SortOrder>(new Identifier(null, "c2"), SortOrder.Desc),
                    new Pair<IExpression, SortOrder>(new Identifier(null, "c3"), SortOrder.Asc)), groupBy.OrderByList);
            Assert.AreEqual("GROUP BY c1, c2 DESC, c3", output);
            sql = "group by c1   ";
            lexer = new MySqlLexer(sql);
            parser = GetDmlParser(lexer);
            groupBy = parser.GroupBy();
            output = Output2MySql(groupBy, sql);
            ListUtil.IsEquals(
                ListUtil.CreateList(new Pair<IExpression, SortOrder>(new Identifier(null, "c1"), SortOrder.Asc)),
                groupBy.OrderByList);
            Assert.AreEqual("GROUP BY c1", output);
            sql = "group by c1 asc  ";
            lexer = new MySqlLexer(sql);
            parser = GetDmlParser(lexer);
            groupBy = parser.GroupBy();
            output = Output2MySql(groupBy, sql);
            ListUtil.IsEquals(
                ListUtil.CreateList(new Pair<IExpression, SortOrder>(new Identifier(null, "c1"), SortOrder.Asc)),
                groupBy.OrderByList);
            Assert.AreEqual("GROUP BY c1", output);
            sql = "group by c1 desc  ";
            lexer = new MySqlLexer(sql);
            parser = GetDmlParser(lexer);
            groupBy = parser.GroupBy();
            output = Output2MySql(groupBy, sql);
            ListUtil.IsEquals(
                ListUtil.CreateList(new Pair<IExpression, SortOrder>(new Identifier(null, "c1"), SortOrder.Desc)),
                groupBy.OrderByList);
            Assert.AreEqual("GROUP BY c1 DESC", output);
            sql = "group by c1 with rollup  ";
            lexer = new MySqlLexer(sql);
            parser = GetDmlParser(lexer);
            groupBy = parser.GroupBy();
            output = Output2MySql(groupBy, sql);
            ListUtil.IsEquals(
                ListUtil.CreateList(new Pair<IExpression, SortOrder>(new Identifier(null, "c1"), SortOrder.Asc)),
                groupBy.OrderByList);
            Assert.AreEqual("GROUP BY c1 WITH ROLLUP", output);
        }

        /// <exception cref="System.SqlSyntaxErrorException" />
        [Test]
        public virtual void TestLimit()
        {
            var sql = "limit 1,2";
            var lexer = new MySqlLexer(sql);
            var parser = GetDmlParser(lexer);
            var limit = parser.Limit();
            var output = Output2MySql(limit, sql);
            Assert.AreEqual(1, limit.Offset);
            Assert.AreEqual(2, limit.Size);
            Assert.AreEqual("LIMIT 1, 2", output);
            sql = "limit 1,?";
            lexer = new MySqlLexer(sql);
            parser = GetDmlParser(lexer);
            limit = parser.Limit();
            output = Output2MySql(limit, sql);
            Assert.AreEqual(1, limit.Offset);
            Assert.AreEqual(new ParamMarker(1), limit.Size);
            Assert.AreEqual("LIMIT 1, ?", output);
            sql = "limit ?,9";
            lexer = new MySqlLexer(sql);
            parser = GetDmlParser(lexer);
            limit = parser.Limit();
            output = Output2MySql(limit, sql);
            Assert.AreEqual(new ParamMarker(1), limit.Offset);
            Assert.AreEqual(9, limit.Size);
            Assert.AreEqual("LIMIT ?, 9", output);
            sql = "limit ?,?";
            lexer = new MySqlLexer(sql);
            parser = GetDmlParser(lexer);
            limit = parser.Limit();
            output = Output2MySql(limit, sql);
            Assert.AreEqual(new ParamMarker(1), limit.Offset);
            Assert.AreEqual(new ParamMarker(2), limit.Size);
            Assert.AreEqual("LIMIT ?, ?", output);
            sql = "limit ? d";
            lexer = new MySqlLexer(sql);
            parser = GetDmlParser(lexer);
            limit = parser.Limit();
            output = Output2MySql(limit, sql);
            Assert.AreEqual(0, limit.Offset);
            Assert.AreEqual(new ParamMarker(1), limit.Size);
            Assert.AreEqual("LIMIT 0, ?", output);
            sql = "limit 9 f";
            lexer = new MySqlLexer(sql);
            parser = GetDmlParser(lexer);
            limit = parser.Limit();
            output = Output2MySql(limit, sql);
            Assert.AreEqual(0, limit.Offset);
            Assert.AreEqual(9, limit.Size);
            Assert.AreEqual("LIMIT 0, 9", output);
            sql = "limit 9 ofFset 0";
            lexer = new MySqlLexer(sql);
            parser = GetDmlParser(lexer);
            limit = parser.Limit();
            output = Output2MySql(limit, sql);
            Assert.AreEqual(0, limit.Offset);
            Assert.AreEqual(9, limit.Size);
            Assert.AreEqual("LIMIT 0, 9", output);
            sql = "limit ? offset 0";
            lexer = new MySqlLexer(sql);
            parser = GetDmlParser(lexer);
            limit = parser.Limit();
            output = Output2MySql(limit, sql);
            Assert.AreEqual(0, limit.Offset);
            Assert.AreEqual(new ParamMarker(1), limit.Size);
            Assert.AreEqual("LIMIT 0, ?", output);
            sql = "limit ? offset ?";
            lexer = new MySqlLexer(sql);
            parser = GetDmlParser(lexer);
            limit = parser.Limit();
            output = Output2MySql(limit, sql);
            Assert.AreEqual(new ParamMarker(2), limit.Offset);
            Assert.AreEqual(new ParamMarker(1), limit.Size);
            Assert.AreEqual("LIMIT ?, ?", output);
            sql = "limit 9 offset ?";
            lexer = new MySqlLexer(sql);
            parser = GetDmlParser(lexer);
            limit = parser.Limit();
            output = Output2MySql(limit, sql);
            Assert.AreEqual(new ParamMarker(1), limit.Offset);
            Assert.AreEqual(9, limit.Size);
            Assert.AreEqual("LIMIT ?, 9", output);
        }

        /// <exception cref="System.SqlSyntaxErrorException" />
        [Test]
        public virtual void TestOrderBy()
        {
            var sql = "order by c1 asc, c2 desc  , c3 ";
            var lexer = new MySqlLexer(sql);
            var parser = GetDmlParser(lexer);
            var orderBy = parser.OrderBy();
            var output = Output2MySql(orderBy, sql);
            ListUtil.IsEquals(ListUtil.CreateList(
                new Pair<IExpression, SortOrder>(new Identifier(null, "c1"), SortOrder.Asc),
                new Pair<IExpression, SortOrder>(new Identifier(null, "c2"), SortOrder.Desc),
                new Pair<IExpression, SortOrder>(new Identifier(null, "c3"), SortOrder.Asc)),
                orderBy.OrderByList);
            Assert.AreEqual("ORDER BY c1, c2 DESC, c3", output);
            sql = "order by c1   ";
            lexer = new MySqlLexer(sql);
            parser = GetDmlParser(lexer);
            orderBy = parser.OrderBy();
            output = Output2MySql(orderBy, sql);
            ListUtil.IsEquals(
                ListUtil.CreateList(new Pair<IExpression, SortOrder>(new Identifier(null, "c1"), SortOrder.Asc)),
                orderBy.OrderByList);
            Assert.AreEqual("ORDER BY c1", output);
            sql = "order by c1 asc  ";
            lexer = new MySqlLexer(sql);
            parser = GetDmlParser(lexer);
            orderBy = parser.OrderBy();
            output = Output2MySql(orderBy, sql);
            ListUtil.IsEquals(
                ListUtil.CreateList(new Pair<IExpression, SortOrder>(new Identifier(null, "c1"), SortOrder.Asc)),
                orderBy.OrderByList);
            Assert.AreEqual("ORDER BY c1", output);
            sql = "order by c1 desc  ";
            lexer = new MySqlLexer(sql);
            parser = GetDmlParser(lexer);
            orderBy = parser.OrderBy();
            output = Output2MySql(orderBy, sql);
            ListUtil.IsEquals(
                ListUtil.CreateList(new Pair<IExpression, SortOrder>(new Identifier(null, "c1"), SortOrder.Desc)),
                orderBy.OrderByList);
            Assert.AreEqual("ORDER BY c1 DESC", output);
        }

        /// <exception cref="System.SqlSyntaxErrorException" />
        [Test]
        public virtual void TestTR1()
        {
            var sql = "(select * from `select`) as `select`";
            var lexer = new MySqlLexer(sql);
            var parser = GetDmlParser(lexer);
            var trs = parser.TableRefs();
            var output = Output2MySql(trs, sql);
            var list = trs.TableReferenceList;
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(typeof (SubqueryFactor), list[0].GetType());
            Assert.AreEqual("(SELECT * FROM `select`) AS `SELECT`", output);
            sql = "(((selecT * from any)union select `select` from `from` order by dd) as 'a1', (((t2)))), t3";
            lexer = new MySqlLexer(sql);
            parser = GetDmlParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySql(trs, sql);
            list = trs.TableReferenceList;
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(typeof (TableReferences), list[0].GetType());
            Assert.AreEqual(typeof (TableRefFactor), list[1].GetType());
            list = ((TableReferences)list[0]).TableReferenceList;
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(typeof (SubqueryFactor), list[0].GetType());
            Assert.AreEqual(typeof (TableReferences), list[1].GetType());
            Assert.AreEqual("((SELECT * FROM any) UNION (SELECT `select` FROM `from` ORDER BY dd)) AS 'a1', t2, t3",
                output);
            sql = "(t1)";
            lexer = new MySqlLexer(sql);
            parser = GetDmlParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySql(trs, sql);
            list = trs.TableReferenceList;
            Assert.AreEqual(1, list.Count);
            var tr = list[0];
            Assert.AreEqual(typeof (TableReferences), tr.GetType());
            list = ((TableReferences)tr).TableReferenceList;
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(typeof (TableRefFactor), list[0].GetType());
            Assert.AreEqual("t1", output);
            sql = "(t1,t2,(t3))";
            lexer = new MySqlLexer(sql);
            parser = GetDmlParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySql(trs, sql);
            list = trs.TableReferenceList;
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(typeof (TableReferences), list[0].GetType());
            tr = (TableReferences)list[0];
            Assert.AreEqual(typeof (TableReferences), tr.GetType());
            list = ((TableReferences)tr).TableReferenceList;
            Assert.AreEqual(3, list.Count);
            Assert.AreEqual(typeof (TableRefFactor), list[0].GetType());
            Assert.AreEqual(typeof (TableRefFactor), list[1].GetType());
            Assert.AreEqual(typeof (TableRefFactor), list[1].GetType());
            Assert.AreEqual("t1, t2, t3", output);
            sql = "(tb1 as t1)inner join (tb2 as t2) on t1.name=t2.name";
            lexer = new MySqlLexer(sql);
            parser = GetDmlParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySql(trs, sql);
            list = trs.TableReferenceList;
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(typeof (InnerJoin), list[0].GetType());
            tr = ((InnerJoin)list[0]).LeftTableRef;
            Assert.AreEqual(typeof (TableReferences), tr.GetType());
            list = ((TableReferences)tr).TableReferenceList;
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(typeof (TableRefFactor), list[0].GetType());
            var ex = ((InnerJoin)trs.TableReferenceList[0]).OnCond;
            Assert.AreEqual(ex.GetType(), typeof (ComparisionEqualsExpression));
            Assert.AreEqual("(tb1 AS T1) INNER JOIN (tb2 AS T2) ON t1.name = t2.name", output);
            sql = "(tb1 as t1)inner join tb2 as t2 using (c1)";
            lexer = new MySqlLexer(sql);
            parser = GetDmlParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySql(trs, sql);
            var using_list = ((InnerJoin)trs.TableReferenceList[0]).Using;
            Assert.AreEqual(1, using_list.Count);
            Assert.AreEqual("(tb1 AS T1) INNER JOIN tb2 AS T2 USING (c1)", output);
            sql = "(tb1 as t1)inner join tb2 as t2 using (c1,c2)";
            lexer = new MySqlLexer(sql);
            parser = GetDmlParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySql(trs, sql);
            using_list = ((InnerJoin)trs.TableReferenceList[0]).Using;
            Assert.AreEqual(2, using_list.Count);
            Assert.AreEqual("(tb1 AS T1) INNER JOIN tb2 AS T2 USING (c1, c2)", output);
            sql = "tb1 as t1 use index (i1,i2,i3)";
            lexer = new MySqlLexer(sql);
            parser = GetDmlParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySql(trs, sql);
            list = trs.TableReferenceList;
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(typeof (TableRefFactor), list[0].GetType());
            var hintlist = ((TableRefFactor)trs.TableReferenceList[0]).HintList;
            Assert.AreEqual(1, hintlist.Count);
            var indexhint = hintlist[0];
            Assert.AreEqual(3, indexhint.IndexList.Count);
            Assert.AreEqual("USE", indexhint.HintAction.GetEnumName());
            Assert.AreEqual("INDEX", indexhint.IndexType.GetEnumName());
            Assert.AreEqual("tb1 AS T1 USE INDEX (i1, i2, i3)", output);
            sql = "tb1 as t1 use index (i1,i2,i3),tb2 as t2 use index (i1,i2,i3)";
            lexer = new MySqlLexer(sql);
            parser = GetDmlParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySql(trs, sql);
            list = trs.TableReferenceList;
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(typeof (TableRefFactor), list[0].GetType());
            hintlist = ((TableRefFactor)trs.TableReferenceList[0]).HintList;
            Assert.AreEqual(1, hintlist.Count);
            indexhint = hintlist[0];
            Assert.AreEqual(3, indexhint.IndexList.Count);
            Assert.AreEqual("USE", indexhint.HintAction.GetEnumName());
            Assert.AreEqual("INDEX", indexhint.IndexType.GetEnumName());
            hintlist = ((TableRefFactor)trs.TableReferenceList[1]).HintList;
            Assert.AreEqual(1, hintlist.Count);
            indexhint = hintlist[0];
            Assert.AreEqual(3, indexhint.IndexList.Count);
            Assert.AreEqual("USE", indexhint.HintAction.GetEnumName());
            Assert.AreEqual("INDEX", indexhint.IndexType.GetEnumName());
            Assert.AreEqual("tb1 AS T1 USE INDEX (i1, i2, i3), tb2 AS T2 USE INDEX (i1, i2, i3)", output);
            sql = "tb1 as t1";
            lexer = new MySqlLexer(sql);
            parser = GetDmlParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySql(trs, sql);
            list = trs.TableReferenceList;
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(typeof (TableRefFactor), list[0].GetType());
            Assert.AreEqual("tb1", ((TableRefFactor)trs.TableReferenceList[0]).Table.IdText);
            Assert.AreEqual("T1", ((TableRefFactor)trs.TableReferenceList[0]).Alias);
            Assert.AreEqual("tb1 AS T1", output);
            sql = "tb1 t1";
            lexer = new MySqlLexer(sql);
            parser = GetDmlParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySql(trs, sql);
            list = trs.TableReferenceList;
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(typeof (TableRefFactor), list[0].GetType());
            Assert.AreEqual("tb1", ((TableRefFactor)trs.TableReferenceList[0]).Table.IdText);
            Assert.AreEqual("T1", ((TableRefFactor)trs.TableReferenceList[0]).Alias);
            Assert.AreEqual("tb1 AS T1", output);
            sql = "tb1,tb2,tb3";
            lexer = new MySqlLexer(sql);
            parser = GetDmlParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySql(trs, sql);
            list = trs.TableReferenceList;
            Assert.AreEqual(3, list.Count);
            Assert.AreEqual(typeof (TableRefFactor), list[0].GetType());
            Assert.AreEqual("tb1", ((TableRefFactor)trs.TableReferenceList[0]).Table.IdText);
            Assert.AreEqual(null, ((TableRefFactor)trs.TableReferenceList[0]).Alias);
            Assert.AreEqual("tb2", ((TableRefFactor)trs.TableReferenceList[1]).Table.IdText);
            Assert.AreEqual("tb3", ((TableRefFactor)trs.TableReferenceList[2]).Table.IdText);
            Assert.AreEqual("tb1, tb2, tb3", output);
            sql = "tb1 use key for join (i1,i2)";
            lexer = new MySqlLexer(sql);
            parser = GetDmlParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySql(trs, sql);
            list = trs.TableReferenceList;
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(typeof (TableRefFactor), list[0].GetType());
            hintlist = ((TableRefFactor)trs.TableReferenceList[0]).HintList;
            Assert.AreEqual(1, hintlist.Count);
            indexhint = hintlist[0];
            Assert.AreEqual(2, indexhint.IndexList.Count);
            Assert.AreEqual("USE", indexhint.HintAction.GetEnumName());
            Assert.AreEqual("KEY", indexhint.IndexType.GetEnumName());
            Assert.AreEqual("JOIN", indexhint.HintScope.GetEnumName());
            Assert.AreEqual("tb1 USE KEY FOR JOIN (i1, i2)", output);
            sql = "tb1 use index for group by(i1,i2)";
            lexer = new MySqlLexer(sql);
            parser = GetDmlParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySql(trs, sql);
            list = trs.TableReferenceList;
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(typeof (TableRefFactor), list[0].GetType());
            hintlist = ((TableRefFactor)trs.TableReferenceList[0]).HintList;
            Assert.AreEqual(1, hintlist.Count);
            indexhint = hintlist[0];
            Assert.AreEqual(2, indexhint.IndexList.Count);
            Assert.AreEqual("USE", indexhint.HintAction.GetEnumName());
            Assert.AreEqual("INDEX", indexhint.IndexType.GetEnumName());
            Assert.AreEqual("GROUP_BY", indexhint.HintScope.GetEnumName());
            Assert.AreEqual("tb1 USE INDEX FOR GROUP BY (i1, i2)", output);
            sql = "tb1 use key for order by (i1,i2) use key for group by () " + "ignore index for group by (i1,i2)";
            lexer = new MySqlLexer(sql);
            parser = GetDmlParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySql(trs, sql);
            list = trs.TableReferenceList;
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(typeof (TableRefFactor), list[0].GetType());
            Assert.AreEqual("tb1", ((TableRefFactor)trs.TableReferenceList[0]).Table.IdText);
            Assert.AreEqual(null, ((TableRefFactor)trs.TableReferenceList[0]).Alias);
            hintlist = ((TableRefFactor)trs.TableReferenceList[0]).HintList;
            Assert.AreEqual(3, hintlist.Count);
            indexhint = hintlist[0];
            Assert.AreEqual(2, indexhint.IndexList.Count);
            Assert.AreEqual("USE", indexhint.HintAction.GetEnumName());
            Assert.AreEqual("KEY", indexhint.IndexType.GetEnumName());
            Assert.AreEqual("ORDER_BY", indexhint.HintScope.GetEnumName());
            indexhint = hintlist[1];
            Assert.AreEqual(0, indexhint.IndexList.Count);
            Assert.AreEqual("USE", indexhint.HintAction.GetEnumName());
            Assert.AreEqual("KEY", indexhint.IndexType.GetEnumName());
            Assert.AreEqual("GROUP_BY", indexhint.HintScope.GetEnumName());
            indexhint = hintlist[2];
            Assert.AreEqual(2, indexhint.IndexList.Count);
            Assert.AreEqual("IGNORE", indexhint.HintAction.GetEnumName());
            Assert.AreEqual("INDEX", indexhint.IndexType.GetEnumName());
            Assert.AreEqual("GROUP_BY", indexhint.HintScope.GetEnumName());
            Assert.AreEqual(
                "tb1 USE KEY FOR ORDER BY (i1, i2) " + "USE KEY FOR GROUP BY () IGNORE INDEX FOR GROUP BY (i1, i2)",
                output);
            sql = "tb1 use index for order by (i1,i2) force index for group by (i1)";
            lexer = new MySqlLexer(sql);
            parser = GetDmlParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySql(trs, sql);
            list = trs.TableReferenceList;
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(typeof (TableRefFactor), list[0].GetType());
            Assert.AreEqual("tb1", ((TableRefFactor)trs.TableReferenceList[0]).Table.IdText);
            Assert.AreEqual(null, ((TableRefFactor)trs.TableReferenceList[0]).Alias);
            hintlist = ((TableRefFactor)trs.TableReferenceList[0]).HintList;
            Assert.AreEqual(2, hintlist.Count);
            indexhint = hintlist[0];
            Assert.AreEqual(2, indexhint.IndexList.Count);
            Assert.AreEqual("i1", indexhint.IndexList[0]);
            Assert.AreEqual("i2", indexhint.IndexList[1]);
            Assert.AreEqual("USE", indexhint.HintAction.GetEnumName());
            Assert.AreEqual("INDEX", indexhint.IndexType.GetEnumName());
            Assert.AreEqual("ORDER_BY", indexhint.HintScope.GetEnumName());
            indexhint = hintlist[1];
            Assert.AreEqual(1, indexhint.IndexList.Count);
            Assert.AreEqual("FORCE", indexhint.HintAction.GetEnumName());
            Assert.AreEqual("INDEX", indexhint.IndexType.GetEnumName());
            Assert.AreEqual("GROUP_BY", indexhint.HintScope.GetEnumName());
            Assert.AreEqual("tb1 USE INDEX FOR ORDER BY (i1, i2) FORCE INDEX FOR GROUP BY (i1)", output);
            sql = "tb1 ignore key for join (i1,i2)";
            lexer = new MySqlLexer(sql);
            parser = GetDmlParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySql(trs, sql);
            list = trs.TableReferenceList;
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(typeof (TableRefFactor), list[0].GetType());
            Assert.AreEqual("tb1", ((TableRefFactor)trs.TableReferenceList[0]).Table.IdText);
            Assert.AreEqual(null, ((TableRefFactor)trs.TableReferenceList[0]).Alias);
            hintlist = ((TableRefFactor)trs.TableReferenceList[0]).HintList;
            Assert.AreEqual(1, hintlist.Count);
            indexhint = hintlist[0];
            Assert.AreEqual(2, indexhint.IndexList.Count);
            Assert.AreEqual("i1", indexhint.IndexList[0]);
            Assert.AreEqual("i2", indexhint.IndexList[1]);
            Assert.AreEqual("IGNORE", indexhint.HintAction.GetEnumName());
            Assert.AreEqual("KEY", indexhint.IndexType.GetEnumName());
            Assert.AreEqual("JOIN", indexhint.HintScope.GetEnumName());
            Assert.AreEqual("tb1 IGNORE KEY FOR JOIN (i1, i2)", output);
            sql = "tb1 ignore index for group by (i1,i2)";
            lexer = new MySqlLexer(sql);
            parser = GetDmlParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySql(trs, sql);
            list = trs.TableReferenceList;
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(typeof (TableRefFactor), list[0].GetType());
            Assert.AreEqual("tb1", ((TableRefFactor)trs.TableReferenceList[0]).Table.IdText);
            Assert.AreEqual(null, ((TableRefFactor)trs.TableReferenceList[0]).Alias);
            hintlist = ((TableRefFactor)trs.TableReferenceList[0]).HintList;
            Assert.AreEqual(1, hintlist.Count);
            indexhint = hintlist[0];
            Assert.AreEqual(2, indexhint.IndexList.Count);
            Assert.AreEqual("i1", indexhint.IndexList[0]);
            Assert.AreEqual("i2", indexhint.IndexList[1]);
            Assert.AreEqual("IGNORE", indexhint.HintAction.GetEnumName());
            Assert.AreEqual("INDEX", indexhint.IndexType.GetEnumName());
            Assert.AreEqual("GROUP_BY", indexhint.HintScope.GetEnumName());
            Assert.AreEqual("tb1 IGNORE INDEX FOR GROUP BY (i1, i2)", output);
            sql =
                "(offer  a  straight_join wp_image b use key for join(t1,t2) on a.member_id=b.member_id inner join product_visit c )";
            lexer = new MySqlLexer(sql);
            parser = GetDmlParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySql(trs, sql);
            Assert.AreEqual(
                "offer AS A STRAIGHT_JOIN wp_image AS B USE KEY FOR JOIN (t1, t2) ON a.member_id = b.member_id INNER JOIN product_visit AS C",
                output);
            sql = "tb1 ignore index for order by(i1)";
            lexer = new MySqlLexer(sql);
            parser = GetDmlParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySql(trs, sql);
            list = trs.TableReferenceList;
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(typeof (TableRefFactor), list[0].GetType());
            Assert.AreEqual("tb1", ((TableRefFactor)trs.TableReferenceList[0]).Table.IdText);
            Assert.AreEqual(null, ((TableRefFactor)trs.TableReferenceList[0]).Alias);
            hintlist = ((TableRefFactor)trs.TableReferenceList[0]).HintList;
            Assert.AreEqual(1, hintlist.Count);
            indexhint = hintlist[0];
            Assert.AreEqual(1, indexhint.IndexList.Count);
            Assert.AreEqual("i1", indexhint.IndexList[0]);
            Assert.AreEqual("IGNORE", indexhint.HintAction.GetEnumName());
            Assert.AreEqual("INDEX", indexhint.IndexType.GetEnumName());
            Assert.AreEqual("ORDER_BY", indexhint.HintScope.GetEnumName());
            Assert.AreEqual("tb1 IGNORE INDEX FOR ORDER BY (i1)", output);
            sql = "tb1 force key for group by (i1,i2)";
            lexer = new MySqlLexer(sql);
            parser = GetDmlParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySql(trs, sql);
            list = trs.TableReferenceList;
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(typeof (TableRefFactor), list[0].GetType());
            Assert.AreEqual("tb1", ((TableRefFactor)trs.TableReferenceList[0]).Table.IdText);
            Assert.AreEqual(null, ((TableRefFactor)trs.TableReferenceList[0]).Alias);
            hintlist = ((TableRefFactor)trs.TableReferenceList[0]).HintList;
            Assert.AreEqual(1, hintlist.Count);
            indexhint = hintlist[0];
            Assert.AreEqual(2, indexhint.IndexList.Count);
            Assert.AreEqual("i1", indexhint.IndexList[0]);
            Assert.AreEqual("i2", indexhint.IndexList[1]);
            Assert.AreEqual("FORCE", indexhint.HintAction.GetEnumName());
            Assert.AreEqual("KEY", indexhint.IndexType.GetEnumName());
            Assert.AreEqual("GROUP_BY", indexhint.HintScope.GetEnumName());
            Assert.AreEqual("tb1 FORCE KEY FOR GROUP BY (i1, i2)", output);
            sql = "tb1 force index for group by (i1,i2)";
            lexer = new MySqlLexer(sql);
            parser = GetDmlParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySql(trs, sql);
            list = trs.TableReferenceList;
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(typeof (TableRefFactor), list[0].GetType());
            Assert.AreEqual("tb1", ((TableRefFactor)trs.TableReferenceList[0]).Table.IdText);
            Assert.AreEqual(null, ((TableRefFactor)trs.TableReferenceList[0]).Alias);
            hintlist = ((TableRefFactor)trs.TableReferenceList[0]).HintList;
            Assert.AreEqual(1, hintlist.Count);
            indexhint = hintlist[0];
            Assert.AreEqual(2, indexhint.IndexList.Count);
            Assert.AreEqual("i1", indexhint.IndexList[0]);
            Assert.AreEqual("i2", indexhint.IndexList[1]);
            Assert.AreEqual("FORCE", indexhint.HintAction.GetEnumName());
            Assert.AreEqual("INDEX", indexhint.IndexType.GetEnumName());
            Assert.AreEqual("GROUP_BY", indexhint.HintScope.GetEnumName());
            Assert.AreEqual("tb1 FORCE INDEX FOR GROUP BY (i1, i2)", output);
            sql = "tb1 force index for join (i1,i2)";
            lexer = new MySqlLexer(sql);
            parser = GetDmlParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySql(trs, sql);
            list = trs.TableReferenceList;
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(typeof (TableRefFactor), list[0].GetType());
            Assert.AreEqual("tb1", ((TableRefFactor)trs.TableReferenceList[0]).Table.IdText);
            Assert.AreEqual(null, ((TableRefFactor)trs.TableReferenceList[0]).Alias);
            hintlist = ((TableRefFactor)trs.TableReferenceList[0]).HintList;
            Assert.AreEqual(1, hintlist.Count);
            indexhint = hintlist[0];
            Assert.AreEqual(2, indexhint.IndexList.Count);
            Assert.AreEqual("i1", indexhint.IndexList[0]);
            Assert.AreEqual("i2", indexhint.IndexList[1]);
            Assert.AreEqual("FORCE", indexhint.HintAction.GetEnumName());
            Assert.AreEqual("INDEX", indexhint.IndexType.GetEnumName());
            Assert.AreEqual("JOIN", indexhint.HintScope.GetEnumName());
            Assert.AreEqual("tb1 FORCE INDEX FOR JOIN (i1, i2)", output);
            sql = "(tb1 force index for join (i1,i2) )left outer join tb2 as t2 " +
                  "use index (i1,i2,i3) on t1.id1=t2.id1";
            lexer = new MySqlLexer(sql);
            parser = GetDmlParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySql(trs, sql);
            list = trs.TableReferenceList;
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(typeof (OuterJoin), list[0].GetType());
            Assert.AreEqual(true, ((OuterJoin)list[0]).IsLeftJoin);
            var ltr = (TableReferences)((OuterJoin)list[0]).LeftTableRef;
            Assert.AreEqual(1, ltr.TableReferenceList.Count);
            Assert.AreEqual(typeof (TableRefFactor), ltr.TableReferenceList[0].GetType());
            Assert.AreEqual(null, ((TableRefFactor)ltr.TableReferenceList[0]).Alias);
            Assert.AreEqual("tb1", ((TableRefFactor)ltr.TableReferenceList[0]).Table.IdText);
            hintlist = ((TableRefFactor)ltr.TableReferenceList[0]).HintList;
            Assert.AreEqual(1, hintlist.Count);
            indexhint = hintlist[0];
            Assert.AreEqual(2, indexhint.IndexList.Count);
            Assert.AreEqual("i1", indexhint.IndexList[0]);
            Assert.AreEqual("i2", indexhint.IndexList[1]);
            Assert.AreEqual("FORCE", indexhint.HintAction.GetEnumName());
            Assert.AreEqual("INDEX", indexhint.IndexType.GetEnumName());
            Assert.AreEqual("JOIN", indexhint.HintScope.GetEnumName());
            var rtf = (TableRefFactor)((OuterJoin)list[0]).RightTableRef;
            Assert.AreEqual("T2", rtf.Alias);
            Assert.AreEqual("tb2", rtf.Table.IdText);
            hintlist = rtf.HintList;
            Assert.AreEqual(1, hintlist.Count);
            indexhint = hintlist[0];
            Assert.AreEqual(3, indexhint.IndexList.Count);
            Assert.AreEqual("i1", indexhint.IndexList[0]);
            Assert.AreEqual("i2", indexhint.IndexList[1]);
            Assert.AreEqual("USE", indexhint.HintAction.GetEnumName());
            Assert.AreEqual("INDEX", indexhint.IndexType.GetEnumName());
            Assert.AreEqual("ALL", indexhint.HintScope.GetEnumName());
            Assert.AreEqual(typeof (ComparisionEqualsExpression), ((OuterJoin)
                list[0]).OnCond.GetType());
            Assert.AreEqual(
                "(tb1 FORCE INDEX FOR JOIN (i1, i2)) " + "LEFT JOIN tb2 AS T2 USE INDEX (i1, i2, i3) ON t1.id1 = t2.id1",
                output);
            sql = " (((tb1 force index for join (i1,i2),tb3),tb4),tb5) " +
                  "left outer join (tb2 as t2 use index (i1,i2,i3)) using(id1)";
            lexer = new MySqlLexer(sql);
            parser = GetDmlParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySql(trs, sql);
            list = trs.TableReferenceList;
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(typeof (OuterJoin), list[0].GetType());
            Assert.AreEqual(true, ((OuterJoin)list[0]).IsLeftJoin);
            ltr = (TableReferences)((OuterJoin)list[0]).LeftTableRef;
            Assert.AreEqual(2, ltr.TableReferenceList.Count);
            Assert.AreEqual(typeof (TableReferences), ltr.TableReferenceList[0].GetType());
            var ltr1 = (TableReferences)ltr.TableReferenceList[0];
            Assert.AreEqual(2, ltr1.TableReferenceList.Count);
            Assert.AreEqual(typeof (TableReferences), ltr1.TableReferenceList[0].GetType());
            var ltr2 = (TableReferences)ltr1.TableReferenceList[0];
            Assert.AreEqual(2, ltr2.TableReferenceList.Count);
            Assert.AreEqual(typeof (TableRefFactor), ltr2.TableReferenceList[0].GetType());
            Assert.AreEqual(null, ((TableRefFactor)ltr2.TableReferenceList[0]).Alias);
            Assert.AreEqual("tb1", ((TableRefFactor)ltr2.TableReferenceList[0]).Table.IdText);
            hintlist = ((TableRefFactor)ltr2.TableReferenceList[0]).HintList;
            Assert.AreEqual(1, hintlist.Count);
            indexhint = hintlist[0];
            Assert.AreEqual(2, indexhint.IndexList.Count);
            Assert.AreEqual("i1", indexhint.IndexList[0]);
            Assert.AreEqual("i2", indexhint.IndexList[1]);
            Assert.AreEqual("FORCE", indexhint.HintAction.GetEnumName());
            Assert.AreEqual("INDEX", indexhint.IndexType.GetEnumName());
            Assert.AreEqual("JOIN", indexhint.HintScope.GetEnumName());
            Assert.AreEqual(typeof (TableRefFactor), ltr2.TableReferenceList[1].GetType());
            Assert.AreEqual("tb3", ((TableRefFactor)ltr2.TableReferenceList[1]).Table.IdText);
            Assert.AreEqual(typeof (TableRefFactor), ltr1.TableReferenceList[1].GetType());
            Assert.AreEqual("tb4", ((TableRefFactor)ltr1.TableReferenceList[1]).Table.IdText);
            Assert.AreEqual(typeof (TableRefFactor), ltr.TableReferenceList[1].GetType());
            Assert.AreEqual("tb5", ((TableRefFactor)ltr.TableReferenceList[1]).Table.IdText);
            var rtr = (TableReferences)((OuterJoin)list[0]).RightTableRef;
            Assert.AreEqual("T2", ((TableRefFactor)rtr.TableReferenceList[0]).Alias);
            Assert.AreEqual("tb2", ((TableRefFactor)rtr.TableReferenceList[0]).Table.IdText);
            hintlist = ((TableRefFactor)rtr.TableReferenceList[0]).HintList;
            Assert.AreEqual(1, hintlist.Count);
            indexhint = hintlist[0];
            Assert.AreEqual(3, indexhint.IndexList.Count);
            Assert.AreEqual("i1", indexhint.IndexList[0]);
            Assert.AreEqual("i2", indexhint.IndexList[1]);
            Assert.AreEqual("USE", indexhint.HintAction.GetEnumName());
            Assert.AreEqual("INDEX", indexhint.IndexType.GetEnumName());
            Assert.AreEqual("ALL", indexhint.HintScope.GetEnumName());
            using_list = ((OuterJoin)trs.TableReferenceList[0]).Using;
            Assert.AreEqual(1, using_list.Count);
            Assert.AreEqual("(tb1 FORCE INDEX FOR JOIN (i1, i2), tb3, tb4, tb5) "
                            + "LEFT JOIN (tb2 AS T2 USE INDEX (i1, i2, i3)) USING (id1)", output);
            sql = "(tb1 force index for join (i1,i2),tb3) " +
                  "left outer join tb2 as t2 use index (i1,i2,i3) using(id1,id2)";
            lexer = new MySqlLexer(sql);
            parser = GetDmlParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySql(trs, sql);
            Assert.AreEqual(
                "(tb1 FORCE INDEX FOR JOIN (i1, i2), tb3) " +
                "LEFT JOIN tb2 AS T2 USE INDEX (i1, i2, i3) USING (id1, id2)", output);
            sql =
                "(tb1 force index for join (i1,i2),tb3) left outer join (tb2 as t2 use index (i1,i2,i3)) using(id1,id2)";
            lexer = new MySqlLexer(sql);
            parser = GetDmlParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySql(trs, sql);
            Assert.AreEqual(
                "(tb1 FORCE INDEX FOR JOIN (i1, i2), tb3) " +
                "LEFT JOIN (tb2 AS T2 USE INDEX (i1, i2, i3)) USING (id1, id2)", output);
            sql = "tb1 as t1 cross join tb2 as t2 use index(i1)using(id1)";
            lexer = new MySqlLexer(sql);
            parser = GetDmlParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySql(trs, sql);
            Assert.AreEqual("tb1 AS T1 INNER JOIN tb2 AS T2 USE INDEX (i1) USING (id1)", output);
            sql = "(tb1 as t1) cross join tb2 as t2 use index(i1)using(id1)";
            lexer = new MySqlLexer(sql);
            parser = GetDmlParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySql(trs, sql);
            Assert.AreEqual("(tb1 AS T1) INNER JOIN tb2 AS T2 USE INDEX (i1) USING (id1)", output);
            sql = "tb1 as _latin't1' cross join tb2 as t2 use index(i1)";
            lexer = new MySqlLexer(sql);
            parser = GetDmlParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySql(trs, sql);
            Assert.AreEqual("tb1 AS _LATIN't1' INNER JOIN tb2 AS T2 USE INDEX (i1)", output);
            sql = "((select '  @  from' from `from`)) as t1 cross join tb2 as t2 use index()";
            lexer = new MySqlLexer(sql);
            parser = GetDmlParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySql(trs, sql);
            list = trs.TableReferenceList;
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(typeof (InnerJoin), list[0].GetType());
            var lsf = (SubqueryFactor)((InnerJoin)list[0]).LeftTableRef;
            Assert.AreEqual("T1", lsf.Alias);
            Assert.AreEqual(typeof (DmlSelectStatement), lsf.Subquery.GetType());
            rtf = (TableRefFactor)((InnerJoin)list[0]).RightTableRef;
            Assert.AreEqual("T2", rtf.Alias);
            hintlist = rtf.HintList;
            Assert.AreEqual(1, hintlist.Count);
            indexhint = hintlist[0];
            Assert.AreEqual("USE", indexhint.HintAction.GetEnumName());
            Assert.AreEqual("INDEX", indexhint.IndexType.GetEnumName());
            Assert.AreEqual("ALL", indexhint.HintScope.GetEnumName());
            Assert.AreEqual("tb2", rtf.Table.IdText);
            Assert.AreEqual("(SELECT '  @  from' FROM `from`) AS T1 " + "INNER JOIN tb2 AS T2 USE INDEX ()", output);
            sql = "(tb1 as t1) straight_join (tb2 as t2)";
            lexer = new MySqlLexer(sql);
            parser = GetDmlParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySql(trs, sql);
            Assert.AreEqual("(tb1 AS T1) STRAIGHT_JOIN (tb2 AS T2)", output);
            sql = "tb1 straight_join tb2 as t2 on tb1.id=tb2.id";
            lexer = new MySqlLexer(sql);
            parser = GetDmlParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySql(trs, sql);
            Assert.AreEqual("tb1 STRAIGHT_JOIN tb2 AS T2 ON tb1.id = tb2.id",
                output);
            sql = "tb1 left outer join tb2 on tb1.id=tb2.id";
            lexer = new MySqlLexer(sql);
            parser = GetDmlParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySql(trs, sql);
            Assert.AreEqual("tb1 LEFT JOIN tb2 ON tb1.id = tb2.id", output);
            sql = "tb1 left outer join tb2 using(id)";
            lexer = new MySqlLexer(sql);
            parser = GetDmlParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySql(trs, sql);
            Assert.AreEqual("tb1 LEFT JOIN tb2 USING (id)", output);
            sql = "(tb1 right outer join tb2 using()) join tb3 on tb1.id=tb2.id and tb2.id=tb3.id";
            lexer = new MySqlLexer(sql);
            parser = GetDmlParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySql(trs, sql);
            list = trs.TableReferenceList;
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(typeof (InnerJoin), list[0].GetType());
            ltr = (TableReferences)((InnerJoin)list[0]).LeftTableRef;
            Assert.AreEqual(1, ltr.TableReferenceList.Count);
            var lltrf = (TableRefFactor)((OuterJoin)ltr.TableReferenceList[0]).LeftTableRef;
            Assert.AreEqual(null, lltrf.Alias);
            Assert.AreEqual("tb1", lltrf.Table.IdText);
            using_list = ((OuterJoin)ltr.TableReferenceList[0]).Using;
            Assert.AreEqual(0, using_list.Count);
            rtf = (TableRefFactor)((InnerJoin)list[0]).RightTableRef;
            Assert.AreEqual(null, rtf.Alias);
            hintlist = rtf.HintList;
            Assert.AreEqual(0, hintlist.Count);
            Assert.AreEqual("tb3", rtf.Table.IdText);
            Assert.AreEqual(typeof (LogicalAndExpression), ((InnerJoin)list[0]).OnCond.GetType());
            Assert.AreEqual("(tb1 RIGHT JOIN tb2 USING ()) " + "INNER JOIN tb3 ON tb1.id = tb2.id AND tb2.id = tb3.id",
                output);
            sql = "tb1 right outer join tb2 using(id1,id2) " + "join (tb3,tb4) on tb1.id=tb2.id and tb2.id=tb3.id";
            lexer = new MySqlLexer(sql);
            parser = GetDmlParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySql(trs, sql);
            list = trs.TableReferenceList;
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(typeof (InnerJoin), list[0].GetType());
            var loj = (OuterJoin)((InnerJoin)list[0]).LeftTableRef;
            lltrf = (TableRefFactor)loj.LeftTableRef;
            Assert.AreEqual(null, lltrf.Alias);
            Assert.AreEqual("tb1", lltrf.Table.IdText);
            using_list = loj.Using;
            Assert.AreEqual(2, using_list.Count);
            rtr = (TableReferences)((InnerJoin)list[0]).RightTableRef;
            Assert.AreEqual(2, rtr.TableReferenceList.Count);
            Assert.AreEqual("tb3", ((TableRefFactor)rtr.TableReferenceList[0]).Table.IdText);
            Assert.AreEqual("tb4", ((TableRefFactor)rtr.TableReferenceList[1]).Table.IdText);
            Assert.AreEqual(typeof (LogicalAndExpression), ((InnerJoin)list[0]).OnCond.GetType());
            Assert.AreEqual(
                "tb1 RIGHT JOIN tb2 USING (id1, id2) " + "INNER JOIN (tb3, tb4) ON tb1.id = tb2.id AND tb2.id = tb3.id",
                output);
            sql = "tb1 left outer join tb2 join tb3 using(id)";
            lexer = new MySqlLexer(sql);
            parser = GetDmlParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySql(trs, sql);
            Assert.AreEqual("tb1 LEFT JOIN (tb2 INNER JOIN tb3) USING (id)",
                output);
            sql = "tb1 right join tb2 on tb1.id=tb2.id";
            lexer = new MySqlLexer(sql);
            parser = GetDmlParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySql(trs, sql);
            Assert.AreEqual("tb1 RIGHT JOIN tb2 ON tb1.id = tb2.id", output);
            sql = "tb1 natural right join tb2 ";
            lexer = new MySqlLexer(sql);
            parser = GetDmlParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySql(trs, sql);
            Assert.AreEqual("tb1 NATURAL RIGHT JOIN tb2", output);
            sql = "tb1 natural right outer join tb2 natural left outer join tb3";
            lexer = new MySqlLexer(sql);
            parser = GetDmlParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySql(trs, sql);
            list = trs.TableReferenceList;
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(typeof (NaturalJoin), list[0].GetType());
            var lnj = (NaturalJoin)((NaturalJoin)list[0]).LeftTableRef;
            lltrf = (TableRefFactor)lnj.LeftTableRef;
            Assert.AreEqual(null, lltrf.Alias);
            Assert.AreEqual("tb1", lltrf.Table.IdText);
            var rltrf = (TableRefFactor)lnj.RightTableRef;
            Assert.AreEqual(null, rltrf.Alias);
            Assert.AreEqual("tb2", rltrf.Table.IdText);
            rtf = (TableRefFactor)((NaturalJoin)list[0]).RightTableRef;
            Assert.AreEqual(null, rtf.Alias);
            Assert.AreEqual("tb3", rtf.Table.IdText);
            Assert.AreEqual("tb1 NATURAL RIGHT JOIN tb2 NATURAL LEFT JOIN tb3", output);
            sql = "tb1 natural left outer join tb2 ";
            lexer = new MySqlLexer(sql);
            parser = GetDmlParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySql(trs, sql);
            Assert.AreEqual("tb1 NATURAL LEFT JOIN tb2", output);
            sql = "(tb1  t1) natural  join (tb2 as t2) ";
            lexer = new MySqlLexer(sql);
            parser = GetDmlParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySql(trs, sql);
            Assert.AreEqual("(tb1 AS T1) NATURAL JOIN (tb2 AS T2)", output);
            sql = "(select (select * from tb1) from `select` " + "where `any`=any(select id2 from tb2))any  ";
            lexer = new MySqlLexer(sql);
            parser = GetDmlParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySql(trs, sql);
            list = trs.TableReferenceList;
            Assert.AreEqual(typeof (SubqueryFactor), list[0].GetType());
            Assert.AreEqual("ANY", ((SubqueryFactor)list[0]).Alias);
            Assert.AreEqual(
                "(SELECT SELECT * FROM tb1 FROM `select` " + "WHERE `any` = ANY (SELECT id2 FROM tb2)) AS ANY", output);
            sql = "((tb1),(tb3 as t3,`select`),tb2 use key for join (i1,i2))" + " left join tb4 join tb5 using ()";
            lexer = new MySqlLexer(sql);
            parser = GetDmlParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySql(trs, sql);
            list = trs.TableReferenceList;
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(typeof (OuterJoin), list[0].GetType());
            Assert.AreEqual(typeof (TableReferences), ((OuterJoin)list[0]).LeftTableRef.GetType());
            Assert.AreEqual(typeof (InnerJoin), ((OuterJoin)list[0]).RightTableRef.GetType());
            list = ((TableReferences)((OuterJoin)list[0]).LeftTableRef).TableReferenceList;
            list = ((TableReferences)list[1]).TableReferenceList;
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("(tb1, tb3 AS T3, `select`, tb2 USE KEY FOR JOIN (i1, i2))"
                            + " LEFT JOIN (tb4 INNER JOIN tb5) USING ()", output);
            sql = "((select `select` from `from` ) tb1),(tb3 as t3,`select`),tb2 use key for join (i1,i2) "
                  + "left join tb4 using (i1,i2)straight_join tb5";
            lexer = new MySqlLexer(sql);
            parser = GetDmlParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySql(trs, sql);
            list = trs.TableReferenceList;
            Assert.AreEqual(3, list.Count);
            Assert.AreEqual(typeof (TableReferences), list[0].GetType());
            Assert.AreEqual(typeof (TableReferences), list[1].GetType());
            Assert.AreEqual(typeof (StraightJoin), list[2].GetType());
            list = ((TableReferences)list[0]).TableReferenceList;
            Assert.AreEqual(typeof (SubqueryFactor), list[0].GetType());
            list = trs.TableReferenceList;
            list = ((TableReferences)list[1]).TableReferenceList;
            Assert.AreEqual(typeof (TableRefFactor), list[0].GetType());
            Assert.AreEqual(typeof (TableRefFactor), list[1].GetType());
            list = trs.TableReferenceList;
            var sj = (StraightJoin)list[2];
            Assert.AreEqual(typeof (OuterJoin), sj.LeftTableRef.GetType());
            Assert.AreEqual(typeof (TableRefFactor), sj.RightTableRef.GetType());
            var oj = (OuterJoin)sj.LeftTableRef;
            using_list = oj.Using;
            Assert.AreEqual(2, using_list.Count);
            Assert.AreEqual(
                "(SELECT `select` FROM `from`) AS TB1, tb3 AS T3, `select`, tb2 USE KEY FOR JOIN (i1, i2) LEFT JOIN tb4 USING (i1, i2) STRAIGHT_JOIN tb5",
                output);
            sql = "(`select`,(tb1 as t1 use index for join()ignore key for group by (i1)))" +
                  "join tb2 on cd1=any " + "right join " + "tb3 straight_join " +
                  "(tb4 use index() left outer join (tb6,tb7) on id3=all(select `all` from `all`)) "
                  + " on id2=any(select * from any) using  (i1)";
            lexer = new MySqlLexer(sql);
            parser = GetDmlParser(lexer);
            trs = parser.TableRefs();
            output = Output2MySql(trs, sql);
            list = trs.TableReferenceList;
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(typeof (OuterJoin), list[0].GetType());
            using_list = ((OuterJoin)list[0]).Using;
            Assert.AreEqual(1, using_list.Count);
            Assert.AreEqual(typeof (InnerJoin), ((OuterJoin)list[0]).LeftTableRef.GetType());
            Assert.AreEqual(typeof (StraightJoin), ((OuterJoin)list[0]).RightTableRef.GetType());
            var rsj = (StraightJoin)((OuterJoin)list[0]).RightTableRef;
            Assert.AreEqual(typeof (TableRefFactor), rsj.LeftTableRef.GetType());
            Assert.AreEqual(typeof (TableReferences), rsj.RightTableRef.GetType());
            list = ((TableReferences)rsj.RightTableRef).TableReferenceList;
            Assert.AreEqual(typeof (OuterJoin), list[0].GetType());
            Assert.AreEqual("(`select`, tb1 AS T1 USE INDEX FOR JOIN () IGNORE KEY FOR GROUP BY (i1)) "
                            + "INNER JOIN tb2 ON cd1 = any RIGHT JOIN (tb3 STRAIGHT_JOIN (tb4 USE INDEX () "
                            +
                            "LEFT JOIN (tb6, tb7) ON id3 = ALL (SELECT `all` FROM `all`)) ON id2 = ANY (SELECT * FROM any))"
                            + " USING (i1)", output);
        }
    }
}