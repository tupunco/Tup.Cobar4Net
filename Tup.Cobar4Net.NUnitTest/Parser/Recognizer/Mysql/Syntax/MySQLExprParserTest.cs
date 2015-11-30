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
using Tup.Cobar4Net.Parser.Ast.Expression.Arithmeic;
using Tup.Cobar4Net.Parser.Ast.Expression.Bit;
using Tup.Cobar4Net.Parser.Ast.Expression.Comparison;
using Tup.Cobar4Net.Parser.Ast.Expression.Logical;
using Tup.Cobar4Net.Parser.Ast.Expression.Misc;
using Tup.Cobar4Net.Parser.Ast.Expression.Primary;
using Tup.Cobar4Net.Parser.Ast.Expression.Primary.Literal;
using Tup.Cobar4Net.Parser.Ast.Expression.String;
using Tup.Cobar4Net.Parser.Ast.Expression.Type;
using Tup.Cobar4Net.Parser.Ast.Fragment;
using Tup.Cobar4Net.Parser.Ast.Stmt.Dml;
using Tup.Cobar4Net.Parser.Recognizer.Mysql.Lexer;

namespace Tup.Cobar4Net.Parser.Recognizer.Mysql.Syntax
{
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    [TestFixture(Category = "MySqlExprParserTest")]
    public class MySqlExprParserTest : AbstractSyntaxTest
    {
        /// <exception cref="System.Exception" />
        [Test]
        public virtual void TestAnyAll()
        {
            var sql = "1 >= any (select id from t1 limit 1)";
            var parser = new MySqlExprParser(new MySqlLexer(sql));
            var expr = parser.Expression();
            var output = Output2MySql(expr, sql);
            Assert.AreEqual("1 >= ANY (SELECT id FROM t1 LIMIT 0, 1)", output);
            Assert.AreEqual(typeof (ComparisionGreaterThanOrEqualsExpression),
                expr.GetType());
            sql =
                "1 >= any (select id from t1 limit 1) > aLl(select tb1.id from tb1 t1,tb2 as t2 where t1.id=t2.id limit 1)";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual(
                "1 >= ANY (SELECT id FROM t1 LIMIT 0, 1) > ALL (SELECT tb1.id FROM tb1 AS T1, tb2 AS T2 WHERE t1.id = t2.id LIMIT 0, 1)", output);
            var gt = (ComparisionGreaterThanExpression)expr;
            var ge = (ComparisionGreaterThanOrEqualsExpression)gt.LeftOprand;
            Assert.AreEqual(typeof (LiteralNumber), ge.LeftOprand.GetType());
            sql = "1 >= any + any";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("1 >= any + any", output);
        }

        /// <exception cref="System.Exception" />
        [Test]
        public virtual void TestArithmetic()
        {
            var sql = "? + @usrVar1 * c/@@version- e % -f diV g";
            var parser = new MySqlExprParser(new MySqlLexer(sql));
            var expr = parser.Expression();
            var output = Output2MySql(expr, sql);
            Assert.AreEqual("? + @usrVar1 * c / @@version - e % - f DIV g", output);
            var sub = (ArithmeticSubtractExpression)expr;
            var add = (ArithmeticAddExpression)sub.LeftOprand;
            var idiv = (ArithmeticIntegerDivideExpression)sub.RightOprand;
            var mod = (ArithmeticModExpression)idiv.LeftOprand;
            var div = (ArithmeticDivideExpression)add.RightOprand;
            var mt = (ArithmeticMultiplyExpression)div.LeftOprand;
            var mi = (MinusExpression)mod.RightOprand;
            Assert.AreEqual("c", ((Identifier)mt.RightOprand).IdText);
            Assert.AreEqual("f", ((Identifier)mi.Operand).IdText);
            sql = "a+-b";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("a + - b", output);
            sql = "a+--b";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("a + - - b", output);
            sql = "a++b";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("a + b", output);
            sql = "a+++b";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("a + b", output);
            sql = "a++-b";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("a + - b", output);
            sql = "a + b mod (-((select id from t1 limit 1)- e) ) ";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("a + b % - ((SELECT id FROM t1 LIMIT 0, 1) - e)",
                output);
            add = (ArithmeticAddExpression)expr;
            mod = (ArithmeticModExpression)add.RightOprand;
            mi = (MinusExpression)mod.RightOprand;
            sub = (ArithmeticSubtractExpression)mi.Operand;
            Assert.IsTrue(typeof (IQueryExpression).IsAssignableFrom(sub.LeftOprand.GetType()));
            Assert.AreEqual("e", ((Identifier)sub.RightOprand).IdText);
        }

        /// <exception cref="System.Exception" />
        [Test]
        public virtual void TestAssignment()
        {
            var sql = "a /*dd*/:=b:=c";
            var parser = new MySqlExprParser(new MySqlLexer(sql));
            var expr = parser.Expression();
            var output = Output2MySql(expr, sql);
            Assert.AreEqual("a := b := c", output);
            Assert.AreEqual(typeof (AssignmentExpression), expr.GetType());
            var ass = (AssignmentExpression)expr;
            Assert.AreEqual(typeof (AssignmentExpression), ass.RightOprand.GetType());
            ass = (AssignmentExpression)ass.RightOprand;
            Assert.AreEqual("b", ((Identifier)ass.LeftOprand).IdText);
            sql = "c=@var:=1";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("c = (@var := 1)", output);
            Assert.AreEqual(typeof (ComparisionEqualsExpression), expr.GetType());
            ass = (AssignmentExpression)((BinaryOperatorExpression)expr).RightOprand;
            var usr = (UsrDefVarPrimary)ass.LeftOprand;
            Assert.AreEqual("@var", usr.VarText);
            sql = "a:=b or c &&d :=0b1101 or b'01'&0xabc";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("a := b OR c AND d := b'1101' OR b'01' & x'abc'",
                output);
            Assert.AreEqual(typeof (AssignmentExpression), expr.GetType());
            ass = (AssignmentExpression)expr;
            Assert.AreEqual(typeof (AssignmentExpression), ass.RightOprand.GetType());
            ass = (AssignmentExpression)ass.RightOprand;
            Assert.AreEqual(typeof (LogicalOrExpression), ass.LeftOprand.
                                                              GetType());
            Assert.AreEqual(typeof (LogicalOrExpression), ass.RightOprand
                                                             .GetType());
            var lor = (LogicalOrExpression)ass.LeftOprand;
            Assert.AreEqual(typeof (LogicalAndExpression), lor.GetOperand(1).GetType());
            lor = (LogicalOrExpression)ass.RightOprand;
            Assert.AreEqual(typeof (LiteralBitField), lor.GetOperand(0).GetType());
            Assert.AreEqual(typeof (BitAndExpression), lor.GetOperand(1).GetType());
            sql = "a:=((b or (c &&d)) :=((0b1101 or (b'01'&0xabc))))";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("a := b OR c AND d := b'1101' OR b'01' & x'abc'",
                output);
            Assert.AreEqual(typeof (AssignmentExpression), expr.GetType());
            ass = (AssignmentExpression)expr;
            Assert.AreEqual(typeof (AssignmentExpression), ass.RightOprand.GetType());
            ass = (AssignmentExpression)ass.RightOprand;
            Assert.AreEqual(typeof (LogicalOrExpression), ass.LeftOprand.
                                                              GetType());
            Assert.AreEqual(typeof (LogicalOrExpression), ass.RightOprand
                                                             .GetType());
            lor = (LogicalOrExpression)ass.LeftOprand;
            Assert.AreEqual(typeof (LogicalAndExpression), lor.GetOperand(1).GetType());
            lor = (LogicalOrExpression)ass.RightOprand;
            Assert.AreEqual(typeof (LiteralBitField), lor.GetOperand(0).GetType());
            Assert.AreEqual(typeof (BitAndExpression), lor.GetOperand(1).GetType());
            sql = "(a:=b) or c &&(d :=0b1101 or b'01')&0xabc ^null";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("(a := b) OR c AND (d := b'1101' OR b'01') & x'abc' ^ NULL", output);
            Assert.AreEqual(typeof (LogicalOrExpression), expr.GetType());
            lor = (LogicalOrExpression)expr;
            Assert.AreEqual(typeof (AssignmentExpression), lor.GetOperand(0).GetType());
            Assert.AreEqual(typeof (LogicalAndExpression), lor.GetOperand(1).GetType());
            var land = (LogicalAndExpression)lor.GetOperand(1);
            Assert.AreEqual(typeof (Identifier), land.GetOperand(0).GetType());
            Assert.AreEqual(typeof (BitAndExpression), land.GetOperand(1).GetType());
            var band = (BitAndExpression)land.GetOperand(1);
            Assert.AreEqual(typeof (AssignmentExpression), band.LeftOprand.GetType());
            Assert.AreEqual(typeof (BitXORExpression), band.RightOprand.GetType());
            ass = (AssignmentExpression)band.LeftOprand;
            Assert.AreEqual(typeof (LogicalOrExpression), ass.RightOprand
                                                             .GetType());
            var bxor = (BitXORExpression)band.RightOprand;
            Assert.AreEqual(typeof (LiteralHexadecimal), bxor.LeftOprand.
                                                              GetType());
            Assert.AreEqual(typeof (LiteralNull), bxor.RightOprand.GetType());
        }

        /// <exception cref="System.Exception" />
        [Test]
        public virtual void TestBit()
        {
            var sql = "0b01001001 | 3 & 1.2 <<d >> 0x0f";
            var parser = new MySqlExprParser(new MySqlLexer(sql));
            var expr = parser.Expression();
            var output = Output2MySql(expr, sql);
            Assert.AreEqual("b'01001001' | 3 & 1.2 << d >> x'0f'", output);
            var or = (BitOrExpression)expr;
            var and = (BitAndExpression)or.RightOprand;
            var rs = (BitShiftExpression)and.RightOprand;
            var ls = (BitShiftExpression)rs.LeftOprand;
            Assert.AreEqual("d", ((Identifier)ls.RightOprand).IdText);
            Assert.IsTrue(rs.IsRightShift());
            Assert.IsFalse(ls.IsRightShift());
            sql = "true + b & false ^ d - null ";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("TRUE + b & FALSE ^ d - NULL", output);
            and = (BitAndExpression)expr;
            var add = (ArithmeticAddExpression)and.LeftOprand;
            var sub = (ArithmeticSubtractExpression)and.RightOprand;
            var xor = (BitXORExpression)sub.LeftOprand;
            Assert.AreEqual("d", ((Identifier)xor.RightOprand).IdText);
            Assert.AreEqual("b", ((Identifier)add.RightOprand).IdText);
        }

        /// <exception cref="System.Exception" />
        [Test]
        public virtual void TestBitHex()
        {
            var sql = "x'89af' ";
            var parser = new MySqlExprParser(new MySqlLexer(sql));
            var expr = parser.Expression();
            var output = Output2MySql(expr, sql);
            Assert.AreEqual("x'89af'", output);
            Assert.AreEqual("89af", ((LiteralHexadecimal)expr).Text);
            sql = "_latin1 b'1011' ";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("_latin1 b'1011'", output);
            Assert.AreEqual("1011", ((LiteralBitField)expr).Text);
            Assert.AreEqual("_latin1", ((LiteralBitField)expr).Introducer);
            sql = "abc 0b1011 ";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("abc", output);
            sql = "_latin1 0xabc ";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("_latin1 x'abc'", output);
            sql = "jkl 0xabc ";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("jkl", output);
        }

        /// <exception cref="System.Exception" />
        [Test]
        public virtual void TestComparision()
        {
            var sql = "a  betwEen b and c Not between d and e";
            var parser = new MySqlExprParser(new MySqlLexer(sql));
            var expr = parser.Expression();
            var output = Output2MySql(expr, sql);
            Assert.AreEqual("a BETWEEN b AND c NOT BETWEEN d AND e", output);
            var ba = (BetweenAndExpression)expr;
            Assert.AreEqual("a", ((Identifier)ba.First).IdText);
            Assert.AreEqual("b", ((Identifier)ba.Second).IdText);
            Assert.AreEqual(false, ba.IsNot);
            ba = (BetweenAndExpression)ba.Third;
            Assert.AreEqual("c", ((Identifier)ba.First).IdText);
            Assert.AreEqual("d", ((Identifier)ba.Second).IdText);
            Assert.AreEqual("e", ((Identifier)ba.Third).IdText);
            Assert.AreEqual(true, ba.IsNot);
            sql = "a between b between c and d and e between f and g";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("a BETWEEN b BETWEEN c AND d AND e BETWEEN f AND g", output);
            ba = (BetweenAndExpression)expr;
            var ba2 = (BetweenAndExpression)ba.Second;
            var ba3 = (BetweenAndExpression)ba.Third;
            Assert.AreEqual("a", ((Identifier)ba.First).IdText);
            Assert.AreEqual("b", ((Identifier)ba2.First).IdText);
            Assert.AreEqual("c", ((Identifier)ba2.Second).IdText);
            Assert.AreEqual("d", ((Identifier)ba2.Third).IdText);
            Assert.AreEqual("e", ((Identifier)ba3.First).IdText);
            Assert.AreEqual("f", ((Identifier)ba3.Second).IdText);
            Assert.AreEqual("g", ((Identifier)ba3.Third).IdText);
            sql = "((select a)) between (select b)   and (select d) ";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("(SELECT a) BETWEEN (SELECT b) AND (SELECT d)", output);
            sql = "a  rliKe b not REGEXP c";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("a REGEXP b NOT REGEXP c", output);
            var re = (RegexpExpression)expr;
            var re2 = (RegexpExpression)re.LeftOprand;
            Assert.AreEqual("a", ((Identifier)re2.LeftOprand).IdText);
            Assert.AreEqual("b", ((Identifier)re2.RightOprand).IdText);
            Assert.AreEqual("c", ((Identifier)re.RightOprand).IdText);
            Assert.AreEqual(true, re.IsNot);
            Assert.AreEqual(false, re2.IsNot);
            sql = "((a)) like (((b)))escape (((d)))";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("a LIKE b ESCAPE d", output);
            sql = "((select a)) like (((select b)))escape (((select d)))";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("(SELECT a) LIKE (SELECT b) ESCAPE (SELECT d)", output);
            sql = "a  like b NOT LIKE c escape d";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("a LIKE b NOT LIKE c ESCAPE d", output);
            var le = (LikeExpression)expr;
            var le2 = (LikeExpression)le.First;
            Assert.AreEqual("a", ((Identifier)le2.First).IdText);
            Assert.AreEqual("b", ((Identifier)le2.Second).IdText);
            Assert.AreEqual("c", ((Identifier)le.Second).IdText);
            Assert.AreEqual("d", ((Identifier)le.Third).IdText);
            Assert.AreEqual(true, le.IsNot);
            Assert.AreEqual(false, le2.IsNot);
            sql = "b NOT LIKE c ";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("b NOT LIKE c", output);
            sql = "a in (b) not in (select id from t1)";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("a IN (b) NOT IN (SELECT id FROM t1)", output);
            var @in = (InExpression)expr;
            var in2 = (InExpression)@in.LeftOprand;
            Assert.AreEqual("a", ((Identifier)in2.LeftOprand).IdText);
            Assert.IsTrue(typeof (IQueryExpression).IsAssignableFrom(@in.RightOprand.GetType()));
            Assert.AreEqual(true, @in.IsNot);
            Assert.AreEqual(false, in2.IsNot);
            sql = "(select a)is not null";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("(SELECT a) IS NOT NULL", output);
            sql = "a is not null is not false is not true is not UNKNOWn is null is false is true is unknown";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("a IS NOT NULL IS NOT FALSE IS NOT TRUE IS NOT UNKNOWN IS NULL IS FALSE IS TRUE IS UNKNOWN", output);
            var @is = (ComparisionIsExpression)expr;
            var is2 = (ComparisionIsExpression)@is.Operand;
            var is3 = (ComparisionIsExpression)is2.Operand;
            var is4 = (ComparisionIsExpression)is3.Operand;
            var is5 = (ComparisionIsExpression)is4.Operand;
            var is6 = (ComparisionIsExpression)is5.Operand;
            var is7 = (ComparisionIsExpression)is6.Operand;
            var is8 = (ComparisionIsExpression)is7.Operand;
            Assert.AreEqual(ComparisionIsExpression.IsUnknown, @is.Mode);
            Assert.AreEqual(ComparisionIsExpression.IsTrue, is2.Mode);
            Assert.AreEqual(ComparisionIsExpression.IsFalse, is3.Mode);
            Assert.AreEqual(ComparisionIsExpression.IsNull, is4.Mode);
            Assert.AreEqual(ComparisionIsExpression.IsNotUnknown, is5.Mode);
            Assert.AreEqual(ComparisionIsExpression.IsNotTrue, is6.Mode);
            Assert.AreEqual(ComparisionIsExpression.IsNotFalse, is7.Mode);
            Assert.AreEqual(ComparisionIsExpression.IsNotNull, is8.Mode);
            Assert.AreEqual("a", ((Identifier)is8.Operand).IdText);
            sql = "a = b <=> c >= d > e <= f < g <> h != i";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("a = b <=> c >= d > e <= f < g <> h != i", output);
            var neq = (ComparisionNotEqualsExpression)expr;
            var lg = (ComparisionLessOrGreaterThanExpression)neq.LeftOprand;
            var l = (ComparisionLessThanExpression)lg.LeftOprand;
            var leq = (ComparisionLessThanOrEqualsExpression)l.LeftOprand;
            var g = (ComparisionGreaterThanExpression)leq.LeftOprand;
            var geq = (ComparisionGreaterThanOrEqualsExpression)g.LeftOprand;
            var nseq = (ComparisionNullSafeEqualsExpression)geq.LeftOprand;
            var eq = (ComparisionEqualsExpression)nseq.LeftOprand;
            Assert.AreEqual("i", ((Identifier)neq.RightOprand).IdText);
            Assert.AreEqual("h", ((Identifier)lg.RightOprand).IdText);
            Assert.AreEqual("g", ((Identifier)l.RightOprand).IdText);
            Assert.AreEqual("f", ((Identifier)leq.RightOprand).IdText);
            Assert.AreEqual("e", ((Identifier)g.RightOprand).IdText);
            Assert.AreEqual("d", ((Identifier)geq.RightOprand).IdText);
            Assert.AreEqual("c", ((Identifier)nseq.RightOprand).IdText);
            Assert.AreEqual("b", ((Identifier)eq.RightOprand).IdText);
            Assert.AreEqual("a", ((Identifier)eq.LeftOprand).IdText);
            sql = "a sounds like b sounds like c";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("a SOUNDS LIKE b SOUNDS LIKE c", output);
            var sl = (SoundsLikeExpression)expr;
            var sl2 = (SoundsLikeExpression)sl.LeftOprand;
            Assert.AreEqual("a", ((Identifier)sl2.LeftOprand).IdText);
            Assert.AreEqual("b", ((Identifier)sl2.RightOprand).IdText);
            Assert.AreEqual("c", ((Identifier)sl.RightOprand).IdText);
            sql = "a like b escape c";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("a LIKE b ESCAPE c", output);
            sql = "(select a) collate z";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("(SELECT a) COLLATE z", output);
            sql = "val1 IN (1,2,'a')";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("val1 IN (1, 2, 'a')", output);
        }

        /// <exception cref="System.Exception" />
        [Test]
        public virtual void TestExpr1()
        {
            var sql = "\"abc\" /* */  '\\'s' + id2/ id3, 123-456*(ii moD d)";
            var lexer = new MySqlLexer(sql);
            var parser = new MySqlExprParser(lexer);
            var expr = parser.Expression();
            var output = Output2MySql(expr, sql);
            Assert.AreEqual("'abc\\'s' + id2 / id3", output);
            Assert.AreEqual(typeof (ArithmeticAddExpression), expr.GetType());
            var bex = (BinaryOperatorExpression)((ArithmeticAddExpression)expr).RightOprand;
            Assert.AreEqual(typeof (ArithmeticDivideExpression), bex.GetType());
            Assert.AreEqual(typeof (Identifier), bex.RightOprand.GetType());
            lexer.NextToken();
            parser = new MySqlExprParser(lexer);
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("123 - 456 * (ii % d)", output);
            Assert.AreEqual(typeof (ArithmeticSubtractExpression), expr.GetType());
            sql = "(n'\"abc\"' \"abc\" /* */  '\\'s' + 1.123e1/ id3)*(.1e3-a||b)mod x'abc'&&(select 0b1001^b'0000')";
            lexer = new MySqlLexer(sql);
            parser = new MySqlExprParser(lexer);
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("(N'\"abc\"abc\\'s' + 11.23 / id3) * (1E+2 - a OR b) % x'abc' AND (SELECT b'1001' ^ b'0000')", output);
            Assert.AreEqual(typeof (LogicalAndExpression), expr.GetType());
            bex = (BinaryOperatorExpression)((LogicalAndExpression)expr).GetOperand(0);
            Assert.AreEqual(typeof (ArithmeticModExpression), bex.GetType());
            bex = (BinaryOperatorExpression)((ArithmeticModExpression)bex).LeftOprand;
            Assert.AreEqual(typeof (ArithmeticMultiplyExpression), bex.GetType());
            bex = (BinaryOperatorExpression)((ArithmeticMultiplyExpression)bex).LeftOprand;
            Assert.AreEqual(typeof (ArithmeticAddExpression), bex.GetType());
            Assert.AreEqual(typeof (LiteralString), ((ArithmeticAddExpression)bex).LeftOprand.GetType());
            bex = (BinaryOperatorExpression)((ArithmeticAddExpression)bex).RightOprand;
            Assert.AreEqual(typeof (ArithmeticDivideExpression), bex.GetType());
            Assert.AreEqual(typeof (DmlSelectStatement), ((LogicalAndExpression)expr).GetOperand(1).GetType());
            sql = "not! ~`select` in (1,current_date,`current_date`)like `all` div a between (c&&d) and (d|e)";
            lexer = new MySqlLexer(sql);
            parser = new MySqlExprParser(lexer);
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("NOT ! ~ `select` IN (1, CURDATE(), `current_date`) LIKE `all` DIV a BETWEEN (c AND d) AND d | e", output);
            Assert.AreEqual(typeof (LogicalNotExpression), expr.GetType());
            var tex = (TernaryOperatorExpression)((LogicalNotExpression)expr).Operand;
            Assert.AreEqual(typeof (BetweenAndExpression), tex.GetType());
            Assert.AreEqual(typeof (LikeExpression), tex.First.GetType());
            Assert.AreEqual(typeof (LogicalAndExpression), tex.Second.GetType());
            Assert.AreEqual(typeof (BitOrExpression), tex.Third.GetType());
            tex = (TernaryOperatorExpression)((BetweenAndExpression)tex).First;
            Assert.AreEqual(typeof (InExpression), tex.First.GetType());
            Assert.AreEqual(typeof (ArithmeticIntegerDivideExpression), tex.Second.GetType());
            bex = (InExpression)tex.First;
            Assert.AreEqual(typeof (NegativeValueExpression), bex.LeftOprand.GetType());
            Assert.AreEqual(typeof (InExpressionList), bex.RightOprand.GetType());
            UnaryOperatorExpression uex = (NegativeValueExpression)bex.LeftOprand;
            Assert.AreEqual(typeof (BitInvertExpression), uex.Operand.GetType());
            sql = " binary case ~a||b&&c^d xor e when 2>any(select a ) then 3 else 4 end is not null =a";
            lexer = new MySqlLexer(sql);
            parser = new MySqlExprParser(lexer);
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("BINARY CASE ~ a OR b AND c ^ d XOR e WHEN 2 > ANY (SELECT a) THEN 3 ELSE 4 END IS NOT NULL = a", output);
            Assert.AreEqual(typeof (ComparisionEqualsExpression), expr.GetType());
            bex = (ComparisionEqualsExpression)expr;
            Assert.AreEqual(typeof (ComparisionIsExpression), bex.LeftOprand.GetType());
            Assert.AreEqual(typeof (Identifier), bex.RightOprand.GetType());
            var cex = (ComparisionIsExpression)bex.LeftOprand;
            Assert.AreEqual(typeof (CastBinaryExpression), cex.Operand.GetType());
            uex = (UnaryOperatorExpression)cex.Operand;
            Assert.AreEqual(typeof (CaseWhenOperatorExpression), uex.Operand.GetType());
            var cwex = (CaseWhenOperatorExpression)uex.Operand;
            Assert.AreEqual(typeof (LogicalOrExpression), cwex.Comparee.GetType());
            PolyadicOperatorExpression pex = (LogicalOrExpression)cwex.Comparee;
            Assert.AreEqual(typeof (BitInvertExpression), pex.GetOperand(0).GetType());
            Assert.AreEqual(typeof (LogicalXORExpression), pex.GetOperand(1).GetType());
            bex = (LogicalXORExpression)pex.GetOperand(1);
            Assert.AreEqual(typeof (LogicalAndExpression), bex.LeftOprand
                                                              .GetType());
            Assert.AreEqual(typeof (Identifier), bex.RightOprand.GetType());
            pex = (LogicalAndExpression)bex.LeftOprand;
            Assert.AreEqual(typeof (Identifier), pex.GetOperand(0).GetType());
            Assert.AreEqual(typeof (BitXORExpression), pex.GetOperand(1).GetType());
            sql = " !interval(a,b)<=>a>>b collate x /?+a!=@@1 or @var sounds like -(a-b) mod -(d or e)";
            lexer = new MySqlLexer(sql);
            parser = new MySqlExprParser(lexer);
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual(
                "! INTERVAL(a, b) <=> a >> b COLLATE x / ? + a != @@1 OR @var SOUNDS LIKE - (a - b) % - (d OR e)", output);
            Assert.AreEqual(typeof (LogicalOrExpression), expr.GetType());
            pex = (LogicalOrExpression)expr;
            Assert.AreEqual(typeof (ComparisionNotEqualsExpression), pex.GetOperand(0).GetType());
            Assert.AreEqual(typeof (SoundsLikeExpression), pex.GetOperand(1).GetType());
            bex = (BinaryOperatorExpression)pex.GetOperand(0);
            Assert.AreEqual(typeof (ComparisionNullSafeEqualsExpression), bex.LeftOprand.GetType());
            Assert.AreEqual(typeof (SysVarPrimary), bex.RightOprand.GetType());
            bex = (BinaryOperatorExpression)bex.LeftOprand;
            Assert.AreEqual(typeof (NegativeValueExpression), bex.LeftOprand.GetType());
            Assert.AreEqual(typeof (BitShiftExpression), bex.RightOprand.
                                                             GetType());
            bex = (BinaryOperatorExpression)bex.RightOprand;
            Assert.AreEqual(typeof (Identifier), bex.LeftOprand.GetType());
            Assert.AreEqual(typeof (ArithmeticAddExpression), bex.RightOprand.GetType());
            bex = (BinaryOperatorExpression)bex.RightOprand;
            Assert.AreEqual(typeof (ArithmeticDivideExpression), bex.LeftOprand.GetType());
            Assert.AreEqual(typeof (Identifier), bex.RightOprand.GetType());
            bex = (BinaryOperatorExpression)bex.LeftOprand;
            Assert.AreEqual(typeof (CollateExpression), bex.LeftOprand.GetType());
            Assert.AreEqual(typeof (ParamMarker), bex.RightOprand.GetType());
            bex = (BinaryOperatorExpression)((LogicalOrExpression)expr).GetOperand(1);
            Assert.AreEqual(typeof (UsrDefVarPrimary), bex.LeftOprand.GetType());
            Assert.AreEqual(typeof (ArithmeticModExpression), bex.RightOprand.GetType());
            bex = (BinaryOperatorExpression)bex.RightOprand;
            Assert.AreEqual(typeof (MinusExpression), bex.LeftOprand.GetType());
            Assert.AreEqual(typeof (MinusExpression), bex.RightOprand.GetType());
            uex = (UnaryOperatorExpression)bex.LeftOprand;
            Assert.AreEqual(typeof (ArithmeticSubtractExpression), uex.Operand.GetType());
            uex = (UnaryOperatorExpression)bex.RightOprand;
            Assert.AreEqual(typeof (LogicalOrExpression), uex.Operand.GetType());
        }

        /// <exception cref="System.Exception" />
        [Test]
        public virtual void TestInterval()
        {
            // QS_TODO
            var sql = "DATE_ADD('2009-01-01', INTERVAL (6/4) HOUR_MINUTE)";
            var parser = new MySqlExprParser(new MySqlLexer(sql));
            var expr = parser.Expression();
            var output = Output2MySql(expr, sql);
            Assert.AreEqual("DATE_ADD('2009-01-01', INTERVAL (6 / 4) HOUR_MINUTE)", output);
            sql = "'2008-12-31 23:59:59' + INTERVAL 1 SECOND";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("'2008-12-31 23:59:59' + INTERVAL 1 SECOND", output);
            sql = " INTERVAL 1 DAY + '2008-12-31'";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("INTERVAL 1 DAY + '2008-12-31'", output);
            sql = "DATE_ADD('2100-12-31 23:59:59',INTERVAL '1:1' MINUTE_SECOND)";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("DATE_ADD('2100-12-31 23:59:59', INTERVAL '1:1' MINUTE_SECOND)", output);
            sql = "DATE_SUB('2005-01-01 00:00:00',INTERVAL '1 1:1:1' DAY_SECOND)";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("DATE_SUB('2005-01-01 00:00:00', INTERVAL '1 1:1:1' DAY_SECOND)", output);
            sql = "DATE_ADD('1900-01-01 00:00:00',INTERVAL '-1 10' DAY_HOUR)";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("DATE_ADD('1900-01-01 00:00:00', INTERVAL '-1 10' DAY_HOUR)", output);
            sql = "DATE_SUB('1998-01-02', INTERVAL 31 DAY)";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("DATE_SUB('1998-01-02', INTERVAL 31 DAY)", output);
            sql = "DATE_ADD('1992-12-31 23:59:59.000002',INTERVAL '1.999999' SECOND_MICROSECOND)";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("DATE_ADD('1992-12-31 23:59:59.000002', INTERVAL '1.999999' SECOND_MICROSECOND)", output);
            sql = "DATE_ADD('2013-01-01', INTERVAL 1 HOUR)";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("DATE_ADD('2013-01-01', INTERVAL 1 HOUR)", output);
            sql = "DATE_ADD('2009-01-30', INTERVAL 1 MONTH)";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("DATE_ADD('2009-01-30', INTERVAL 1 MONTH)", output);
            sql = "DATE_ADD('1992-12-31 23:59:59.000002',INTERVAL '1:1.999999' minute_MICROSECOND)";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("DATE_ADD('1992-12-31 23:59:59.000002', INTERVAL '1:1.999999' MINUTE_MICROSECOND)", output);
            sql = "DATE_ADD('1992-12-31 23:59:59.000002',INTERVAL '1:1:1.999999' hour_MICROSECOND)";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("DATE_ADD('1992-12-31 23:59:59.000002', INTERVAL '1:1:1.999999' HOUR_MICROSECOND)", output);
            sql = "DATE_ADD('2100-12-31 23:59:59',INTERVAL '1:1:1' hour_SECOND)";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("DATE_ADD('2100-12-31 23:59:59', INTERVAL '1:1:1' HOUR_SECOND)", output);
            sql = "DATE_ADD('1992-12-31 23:59:59.000002',INTERVAL '1 1:1:1.999999' day_MICROSECOND)";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("DATE_ADD('1992-12-31 23:59:59.000002', INTERVAL '1 1:1:1.999999' DAY_MICROSECOND)", output);
            sql = "DATE_ADD('2100-12-31 23:59:59',INTERVAL '1 1:1' day_minute)";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("DATE_ADD('2100-12-31 23:59:59', INTERVAL '1 1:1' DAY_MINUTE)", output);
            sql = "DATE_ADD('2100-12-31',INTERVAL '1-1' year_month)";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("DATE_ADD('2100-12-31', INTERVAL '1-1' YEAR_MONTH)", output);
            sql = "INTERVAL(n1,n2,n3)";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("INTERVAL(n1, n2, n3)", output);
            sql = "INTERVAL a+b day";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("INTERVAL (a + b) DAY", output);
            sql = "INTERVAL(select id from t1) day";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("INTERVAL (SELECT id FROM t1) DAY", output);
            sql = "INTERVAL(('jklj'+a))day";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("INTERVAL ('jklj' + a) DAY", output);
        }

        /// <exception cref="System.Exception" />
        [Test]
        public virtual void TestLogical()
        {
            var sql = "a || b Or c";
            var parser = new MySqlExprParser(new MySqlLexer(sql));
            var expr = parser.Expression();
            var output = Output2MySql(expr, sql);
            Assert.AreEqual("a OR b OR c", output);
            Assert.AreEqual(typeof (LogicalOrExpression), expr.GetType());
            var or = (LogicalOrExpression)expr;
            Assert.AreEqual(3, or.Arity);
            Assert.AreEqual("b", ((Identifier)or.GetOperand(1)).IdText);
            sql = "a XOR b xOr c";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("a XOR b XOR c", output);
            Assert.AreEqual(typeof (LogicalXORExpression), expr.GetType());
            var xor = (LogicalXORExpression)expr;
            Assert.AreEqual(typeof (LogicalXORExpression), xor.LeftOprand
                                                              .GetType());
            xor = (LogicalXORExpression)xor.LeftOprand;
            Assert.AreEqual("b", ((Identifier)xor.RightOprand).IdText);
            sql = "a XOR( b xOr c)";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("a XOR (b XOR c)", output);
            xor = (LogicalXORExpression)expr;
            var xor2 = (LogicalXORExpression)xor.RightOprand;
            Assert.AreEqual("a", ((Identifier)xor.LeftOprand).IdText);
            Assert.AreEqual("b", ((Identifier)xor2.LeftOprand).IdText);
            Assert.AreEqual("c", ((Identifier)xor2.RightOprand).IdText);
            sql = "a and     b && c";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("a AND b AND c", output);
            Assert.AreEqual(typeof (LogicalAndExpression), expr.GetType());
            var and = (LogicalAndExpression)expr;
            Assert.AreEqual(3, or.Arity);
            Assert.AreEqual("b", ((Identifier)and.GetOperand(1)).IdText);
            sql = "not NOT Not a";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("NOT NOT NOT a", output);
            Assert.AreEqual(typeof (LogicalNotExpression), expr.GetType());
            var not = (LogicalNotExpression)((LogicalNotExpression)expr).Operand;
            Assert.AreEqual(typeof (LogicalNotExpression), not.GetType());
            not = (LogicalNotExpression)not.Operand;
            Assert.AreEqual(typeof (LogicalNotExpression), not.GetType());
            Assert.AreEqual("a", ((Identifier)not.Operand).IdText);
        }

        /// <exception cref="System.Exception" />
        [Test]
        public virtual void TestMatchExpression()
        {
            // QS_TODO
            var sql = "MATCH (title,body) AGAINST ('database' WITH QUERY EXPANSION)";
            var parser = new MySqlExprParser(new MySqlLexer(sql));
            var expr = parser.Expression();
            var output = Output2MySql(expr, sql);
            Assert.AreEqual("MATCH (title, body) AGAINST ('database' WITH QUERY EXPANSION)", output);
            Assert.AreEqual("MATCH (title, body) AGAINST ('database' WITH QUERY EXPANSION)", output);
            sql = "MATCH (title,body) AGAINST ( (abc in (d)) IN boolean MODE)";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("MATCH (title, body) AGAINST ((abc IN (d)) IN BOOLEAN MODE)", output);
            sql = "MATCH (title,body) AGAINST ('database')";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("MATCH (title, body) AGAINST ('database')", output);
            sql = "MATCH (col1,col2,col3) AGAINST ((a:=b:=c) IN boolean MODE)";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("MATCH (col1, col2, col3) AGAINST (a := b := c IN BOOLEAN MODE)", output);
            sql = "MATCH (title,body) AGAINST ((a and (b ||c)) IN boolean MODE)";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("MATCH (title, body) AGAINST (a AND (b OR c) IN BOOLEAN MODE)", output);
            sql = "MATCH (title,body) AGAINST ((a between b and c) IN boolean MODE)";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("MATCH (title, body) AGAINST (a BETWEEN b AND c IN BOOLEAN MODE)", output);
            sql = "MATCH (title,body) AGAINST ((a between b and (abc in (d))) IN boolean MODE)";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("MATCH (title, body) AGAINST ((a BETWEEN b AND abc IN (d)) IN BOOLEAN MODE)", output);
            sql = "MATCH (title,body) AGAINST ((not not a) IN boolean MODE)";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("MATCH (title, body) AGAINST (NOT NOT a IN BOOLEAN MODE)", output);
            sql = "MATCH (title,body) AGAINST ((a is true) IN boolean MODE)";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("MATCH (title, body) AGAINST (a IS TRUE IN BOOLEAN MODE)", output);
            sql = "MATCH (title,body) AGAINST ((select a) IN boolean MODE)";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("MATCH (title, body) AGAINST (SELECT a IN BOOLEAN MODE)", output);
            sql = "MATCH (title,body) AGAINST ('database' IN NATURAL LANGUAGE MODE)";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("MATCH (title, body) AGAINST ('database' IN NATURAL LANGUAGE MODE)", output);
            sql = "MATCH (title,body) AGAINST ('database' IN NATURAL LANGUAGE MODE WITH QUERY EXPANSION)";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("MATCH (title, body) AGAINST ('database' IN NATURAL LANGUAGE MODE WITH QUERY EXPANSION)", output);
        }

        /// <exception cref="System.Exception" />
        [Test]
        public virtual void TestPrimary()
        {
            var sql = "(1,2,existS (select id.* from t1))";
            var parser = new MySqlExprParser(new MySqlLexer(sql));
            var expr = parser.Expression();
            var output = Output2MySql(expr, sql);
            Assert.AreEqual("ROW(1, 2, EXISTS (SELECT id.* FROM t1))", output);
            var row = (RowExpression)expr;
            Assert.AreEqual(3, row.RowExprList.Count);
            sql = "*";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("*", output);
            Assert.IsTrue(typeof (Wildcard).IsAssignableFrom(expr.GetType()));
            sql = "case v1 when `index` then a when 2 then b else c end";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("CASE v1 WHEN `index` THEN a WHEN 2 THEN b ELSE c END", output);
            var cw = (CaseWhenOperatorExpression)expr;
            Assert.AreEqual("v1", ((Identifier)cw.Comparee).IdText);
            Assert.AreEqual(2, cw.WhenList.Count);
            Assert.AreEqual("c", ((Identifier)cw.ElseResult).IdText);
            sql = "case  when 1=value then a  end";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("CASE WHEN 1 = value THEN a END", output);
            cw = (CaseWhenOperatorExpression)expr;
            Assert.IsNull(cw.Comparee);
            Assert.AreEqual(1, cw.WhenList.Count);
            Assert.IsNull(cw.ElseResult);
            sql = "case  when 1=`in` then a  end";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("CASE WHEN 1 = `in` THEN a END", output);
            sql = " ${INSENSITIVE}. ";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("${INSENSITIVE}", output);
            sql = "current_date, ";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("CURDATE()", output);
            sql = "CurRent_Date  (  ) ";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("CURDATE()", output);
            sql = "CurRent_TiMe   ";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("CURTIME()", output);
            sql = "CurRent_TiMe  () ";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("CURTIME()", output);
            sql = "CurRent_TimesTamp ";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("NOW()", output);
            sql = "CurRent_TimesTamp  ()";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("NOW()", output);
            sql = "localTimE";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("NOW()", output);
            sql = "localTimE  () ";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("NOW()", output);
            sql = "localTimesTamP  ";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("NOW()", output);
            sql = "localTimesTamP  () ";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("NOW()", output);
            sql = "CurRent_user ";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("CURRENT_USER()", output);
            sql = "CurRent_user  () ";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("CURRENT_USER()", output);
            sql = "default  () ";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("DEFAULT()", output);
            sql = "vaLueS(1,col1*2)";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("VALUES(1, col1 * 2)", output);
            sql = "(1,2,mod(m,n))";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("ROW(1, 2, m % n)", output);
            sql = "chaR (77,121,'77.3')";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("CHAR(77, 121, '77.3')", output);
            sql = "CHARSET(CHAR(0x65 USING utf8))";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("CHARSET(CHAR(x'65' USING utf8))", output);
            sql = "CONVERT(_latin1'Müller' USING utf8)";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("CONVERT(_latin1'Müller' USING utf8)", output);
        }

        /// <exception cref="System.Exception" />
        [Test]
        public virtual void TestPrimarySystemVar()
        {
            var sql = "@@gloBal . /*dd*/ `all`";
            var lexer = new MySqlLexer(sql);
            var parser = new MySqlExprParser(lexer);
            var expr = parser.Expression();
            var output = Output2MySql(expr, sql);
            Assert.AreEqual("@@global.`all`", output);
            Assert.AreEqual(typeof (SysVarPrimary), expr.GetType());
            var sysvar = (SysVarPrimary)expr;
            Assert.AreEqual(VariableScope.Global, sysvar.Scope);
            Assert.AreEqual("`all`", sysvar.VarText);
            sql = "@@Session . /*dd*/ any";
            lexer = new MySqlLexer(sql);
            parser = new MySqlExprParser(lexer);
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("@@any", output);
            Assert.AreEqual(typeof (SysVarPrimary), expr.GetType());
            sysvar = (SysVarPrimary)expr;
            Assert.AreEqual(VariableScope.Session, sysvar.Scope);
            Assert.AreEqual("any", sysvar.VarText);
            sql = "@@LOCAl . /*dd*/ `usage`";
            lexer = new MySqlLexer(sql);
            parser = new MySqlExprParser(lexer);
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("@@`usage`", output);
            Assert.AreEqual(typeof (SysVarPrimary), expr.GetType());
            sysvar = (SysVarPrimary)expr;
            Assert.AreEqual(VariableScope.Session, sysvar.Scope);
            Assert.AreEqual("`usage`", sysvar.VarText);
            sql = "@@LOCAl . /*dd*/ `var1`";
            lexer = new MySqlLexer(sql);
            parser = new MySqlExprParser(lexer);
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("@@`var1`", output);
            Assert.AreEqual(typeof (SysVarPrimary), expr.GetType());
            sysvar = (SysVarPrimary)expr;
            Assert.AreEqual(VariableScope.Session, sysvar.Scope);
            Assert.AreEqual("`var1`", sysvar.VarText);
            sql = "@@var1   ,";
            lexer = new MySqlLexer(sql);
            parser = new MySqlExprParser(lexer);
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("@@var1", output);
            Assert.AreEqual(typeof (SysVarPrimary), expr.GetType());
            sysvar = (SysVarPrimary)expr;
            Assert.AreEqual(VariableScope.Session, sysvar.Scope);
            Assert.AreEqual("var1", sysvar.VarText);
            sql = "@@`case``1`   ,@@_";
            lexer = new MySqlLexer(sql);
            parser = new MySqlExprParser(lexer);
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("@@`case``1`", output);
            Assert.AreEqual(typeof (SysVarPrimary), expr.GetType());
            sysvar = (SysVarPrimary)expr;
            Assert.AreEqual(VariableScope.Session, sysvar.Scope);
            Assert.AreEqual("`case``1`", sysvar.VarText);
            lexer.NextToken();
            parser = new MySqlExprParser(lexer);
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("@@_", output);
            Assert.AreEqual(typeof (SysVarPrimary), expr.GetType());
            sysvar = (SysVarPrimary)expr;
            Assert.AreEqual(VariableScope.Session, sysvar.Scope);
            Assert.AreEqual("_", sysvar.VarText);
        }

        // QS_TODO
        /// <exception cref="System.Exception" />
        [Test]
        public virtual void TestStartedFromIdentifier()
        {
            // QS_TODO
            var sql = "cast(CAST(1-2 AS UNSIGNED) AS SIGNED)";
            var parser = new MySqlExprParser(new MySqlLexer(sql));
            var expr = parser.Expression();
            var output = Output2MySql(expr, sql);
            Assert.AreEqual("CAST(CAST(1 - 2 AS UNSIGNED) AS SIGNED)", output);
            sql = "position('a' in \"abc\")";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("LOCATE('a', 'abc')", output);
            sql = "cast(CAST(1-2 AS UNSIGNED integer) AS SIGNED integer)";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("CAST(CAST(1 - 2 AS UNSIGNED) AS SIGNED)", output);
            sql = "CAST(expr as char)";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("CAST(expr AS CHAR)", output);
            sql = "CAST(6/4 AS DECIMAL(3,1))";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("CAST(6 / 4 AS DECIMAL(3, 1))", output);
            sql = "CAST(6/4 AS DECIMAL(3))";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("CAST(6 / 4 AS DECIMAL(3))", output);
            sql = "CAST(6/4 AS DECIMAL)";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("CAST(6 / 4 AS DECIMAL)", output);
            sql = "CAST(now() as date)";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("CAST(NOW() AS DATE)", output);
            sql = "CAST(expr as char(5))";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("CAST(expr AS CHAR(5))", output);
            sql = "SUBSTRING('abc',pos,len)";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("SUBSTRING('abc', pos, len)", output);
            sql = "SUBSTRING('abc' FROM pos FOR len)";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("SUBSTRING('abc', pos, len)", output);
            sql = "SUBSTRING(str,pos)";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("SUBSTRING(str, pos)", output);
            sql = "SUBSTRING('abc',1,2)";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("SUBSTRING('abc', 1, 2)", output);
            sql = "row(1,2,str)";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("ROW(1, 2, str)", output);
            sql = "position(\"abc\" in '/*abc*/')";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("LOCATE('abc', '/*abc*/')", output);
            sql = "locate(localtime,b)";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("LOCATE(NOW(), b)", output);
            sql = "locate(locate(a,b),`match`)";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("LOCATE(LOCATE(a, b), `match`)", output);
            sql = "TRIM(LEADING 'x' FROM 'xxxbarxxx')";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("TRIM(LEADING 'x' FROM 'xxxbarxxx')", output);
            sql = "TRIM(BOTH 'x' FROM 'xxxbarxxx')";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("TRIM(BOTH 'x' FROM 'xxxbarxxx')", output);
            sql = "TRIM(TRAILING 'xyz' FROM 'barxxyz')";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("TRIM(TRAILING 'xyz' FROM 'barxxyz')", output);
            sql = "TRIM('  if   ')";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("TRIM('  if   ')", output);
            sql = "TRIM( 'x' FROM 'xxxbarxxx')";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("TRIM('x' FROM 'xxxbarxxx')", output);
            sql = "TRIM(both  FROM 'barxxyz')";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("TRIM(BOTH  FROM 'barxxyz')", output);
            sql = "TRIM(leading  FROM 'barxxyz')";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("TRIM(LEADING  FROM 'barxxyz')", output);
            sql = "TRIM(TRAILING  FROM 'barxxyz')";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("TRIM(TRAILING  FROM 'barxxyz')", output);
            sql = "avg(DISTINCT results)";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("AVG(DISTINCT results)", output);
            sql = "avg(results)";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("AVG(results)", output);
            sql = "max(DISTINCT results)";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("MAX(DISTINCT results)", output);
            sql = "max(results)";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("MAX(results)", output);
            sql = "min(DISTINCT results)";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("MIN(DISTINCT results)", output);
            sql = "min(results)";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("MIN(results)", output);
            sql = "sum(DISTINCT results)";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("SUM(DISTINCT results)", output);
            sql = "sum(results)";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("SUM(results)", output);
            sql = "Count(DISTINCT expr1,expr2,expr3)";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("COUNT(DISTINCT expr1, expr2, expr3)", output);
            sql = "Count(*)";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("COUNT(*)", output);
            sql = "GROUP_CONCAT(DISTINCT expr1,expr2,expr3 ORDER BY col_name1 DESC,col_name2 SEPARATOR ' ')";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("GROUP_CONCAT(DISTINCT expr1, expr2, expr3 ORDER BY col_name1 DESC, col_name2 SEPARATOR  )", output);
            sql = "GROUP_CONCAT(a||b,expr2,expr3 ORDER BY col_name1 asc,col_name2 SEPARATOR '@ ')";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("GROUP_CONCAT(a OR b, expr2, expr3 ORDER BY col_name1 ASC, col_name2 SEPARATOR @ )", output);
            sql = "GROUP_CONCAT(expr1 ORDER BY col_name1 asc,col_name2 SEPARATOR 'str_val ')";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("GROUP_CONCAT(expr1 ORDER BY col_name1 ASC, col_name2 SEPARATOR str_val )", output);
            sql = "GROUP_CONCAT(DISTINCT test_score ORDER BY test_score DESC )";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("GROUP_CONCAT(DISTINCT test_score ORDER BY test_score DESC SEPARATOR ,)", output);
            sql = "GROUP_CONCAT(DISTINCT test_score ORDER BY test_score asc )";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("GROUP_CONCAT(DISTINCT test_score ORDER BY test_score ASC SEPARATOR ,)", output);
            sql = "GROUP_CONCAT(c1)";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("GROUP_CONCAT(c1 SEPARATOR ,)", output);
            sql = "GROUP_CONCAT(c1 separator '')";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("GROUP_CONCAT(c1 SEPARATOR )", output);
            sql = "default";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("DEFAULT", output);
            sql = "default(col)";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("DEFAULT(col)", output);
            sql = "database()";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("DATABASE()", output);
            sql = "if(1>2,a+b,a:=1)";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("IF(1 > 2, a + b, a := 1)", output);
            sql = "insert('abc',1,2,'')";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("INSERT('abc', 1, 2, '')", output);
            sql = "left(\"hjkafag\",4)";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("LEFT('hjkafag', 4)", output);
            sql = "repeat('ag',2.1e1)";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("REPEAT('ag', 21)", output);
            sql = "replace('anjd',\"df\",'af')";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("REPLACE('anjd', 'df', 'af')", output);
            sql = "right(\"hjkafag\",4)";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("RIGHT('hjkafag', 4)", output);
            sql = "schema()";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("DATABASE()", output);
            sql = "utc_date()";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("UTC_DATE()", output);
            sql = "Utc_time()";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("UTC_TIME()", output);
            sql = "Utc_timestamp()";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("UTC_TIMESTAMP()", output);
        }

        /// <exception cref="System.Exception" />
        [Test]
        public virtual void TestString()
        {
            var sql = "_latin1'abc\\'d' 'ef\"'";
            var lexer = new MySqlLexer(sql);
            var parser = new MySqlExprParser(lexer);
            var expr = parser.Expression();
            var output = Output2MySql(expr, sql);
            Assert.AreEqual("_latin1'abc\\'def\"'", output);
            sql = "n'abc\\'d' \"ef'\"\"\"";
            lexer = new MySqlLexer(sql);
            parser = new MySqlExprParser(lexer);
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("N'abc\\'def\\'\"'", output);
            sql = "`char`'an'";
            lexer = new MySqlLexer(sql);
            parser = new MySqlExprParser(lexer);
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("`char`", output);
            sql = "_latin1 n'abc' ";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("_latin1", output);
        }

        /// <exception cref="System.Exception" />
        [Test]
        public virtual void TestUnary()
        {
            var sql = "!-~ binary a collate latin1_danish_ci";
            var parser = new MySqlExprParser(new MySqlLexer(sql));
            var expr = parser.Expression();
            var output = Output2MySql(expr, sql);
            Assert.AreEqual("! - ~ BINARY a COLLATE latin1_danish_ci", output);
            var neg = (NegativeValueExpression)expr;
            var mi = (MinusExpression)neg.Operand;
            var bi = (BitInvertExpression)mi.Operand;
            var bin = (CastBinaryExpression)bi.Operand;
            var col = (CollateExpression)bin.Operand;
            Assert.AreEqual("a", ((Identifier)col.StringValue).IdText);
        }

        /// <exception cref="System.Exception" />
        [Test]
        public virtual void TestUser()
        {
            var sql = "'root'@'localhost'";
            var parser = new MySqlExprParser(new MySqlLexer(sql));
            var expr = parser.Expression();
            var output = Output2MySql(expr, sql);
            Assert.AreEqual("'root'@'localhost'", output);
            var usr = (UserExpression)expr;
            Assert.AreEqual("'root'@'localhost'", usr.UserAtHost);
            sql = "root@localhost";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("root@localhost", output);
            usr = (UserExpression)expr;
            Assert.AreEqual("root@localhost", usr.UserAtHost);
            sql = "var@'localhost'";
            parser = new MySqlExprParser(new MySqlLexer(sql));
            expr = parser.Expression();
            output = Output2MySql(expr, sql);
            Assert.AreEqual("var@'localhost'", output);
            usr = (UserExpression)expr;
            Assert.AreEqual("var@'localhost'", usr.UserAtHost);
        }
    }
}