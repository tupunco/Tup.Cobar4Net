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
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    [NUnit.Framework.TestFixture(Category = "MySQLExprParserTest")]
    public class MySQLExprParserTest : AbstractSyntaxTest
    {
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestExpr1()
        {
            string sql = "\"abc\" /* */  '\\'s' + id2/ id3, 123-456*(ii moD d)";
            MySQLLexer lexer = new MySQLLexer(sql);
            MySQLExprParser parser = new MySQLExprParser(lexer);
            Tup.Cobar4Net.Parser.Ast.Expression.Expression expr = parser.Expression();
            string output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("'abc\\'s' + id2 / id3", output);
            NUnit.Framework.Assert.AreEqual(typeof(ArithmeticAddExpression), expr.GetType());
            BinaryOperatorExpression bex = (BinaryOperatorExpression)((ArithmeticAddExpression
                )expr).GetRightOprand();
            NUnit.Framework.Assert.AreEqual(typeof(ArithmeticDivideExpression), bex.GetType()
                );
            NUnit.Framework.Assert.AreEqual(typeof(Identifier), bex.GetRightOprand().GetType(
                ));
            lexer.NextToken();
            parser = new MySQLExprParser(lexer);
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("123 - 456 * (ii % d)", output);
            NUnit.Framework.Assert.AreEqual(typeof(ArithmeticSubtractExpression), expr.GetType
                ());
            sql = "(n'\"abc\"' \"abc\" /* */  '\\'s' + 1.123e1/ id3)*(.1e3-a||b)mod x'abc'&&(select 0b1001^b'0000')";
            lexer = new MySQLLexer(sql);
            parser = new MySQLExprParser(lexer);
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("(N'\"abc\"abc\\'s' + 11.23 / id3) * (1E+2 - a OR b) % x'abc' AND (SELECT b'1001' ^ b'0000')"
                , output);
            NUnit.Framework.Assert.AreEqual(typeof(LogicalAndExpression), expr.GetType());
            bex = (BinaryOperatorExpression)((LogicalAndExpression)expr).GetOperand(0);
            NUnit.Framework.Assert.AreEqual(typeof(ArithmeticModExpression), bex.GetType());
            bex = (BinaryOperatorExpression)((ArithmeticModExpression)bex).GetLeftOprand();
            NUnit.Framework.Assert.AreEqual(typeof(ArithmeticMultiplyExpression), bex.GetType
                ());
            bex = (BinaryOperatorExpression)((ArithmeticMultiplyExpression)bex).GetLeftOprand
                ();
            NUnit.Framework.Assert.AreEqual(typeof(ArithmeticAddExpression), bex.GetType());
            NUnit.Framework.Assert.AreEqual(typeof(LiteralString), ((ArithmeticAddExpression)
                bex).GetLeftOprand().GetType());
            bex = (BinaryOperatorExpression)((ArithmeticAddExpression)bex).GetRightOprand();
            NUnit.Framework.Assert.AreEqual(typeof(ArithmeticDivideExpression), bex.GetType()
                );
            NUnit.Framework.Assert.AreEqual(typeof(DMLSelectStatement), ((LogicalAndExpression
                )expr).GetOperand(1).GetType());
            sql = "not! ~`select` in (1,current_date,`current_date`)like `all` div a between (c&&d) and (d|e)";
            lexer = new MySQLLexer(sql);
            parser = new MySQLExprParser(lexer);
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("NOT ! ~ `select` IN (1, CURDATE(), `current_date`) LIKE `all` DIV a BETWEEN (c AND d) AND d | e"
                , output);
            NUnit.Framework.Assert.AreEqual(typeof(LogicalNotExpression), expr.GetType());
            TernaryOperatorExpression tex = (TernaryOperatorExpression)((LogicalNotExpression
                )expr).GetOperand();
            NUnit.Framework.Assert.AreEqual(typeof(BetweenAndExpression), tex.GetType());
            NUnit.Framework.Assert.AreEqual(typeof(LikeExpression), tex.GetFirst().GetType());
            NUnit.Framework.Assert.AreEqual(typeof(LogicalAndExpression), tex.GetSecond().GetType
                ());
            NUnit.Framework.Assert.AreEqual(typeof(BitOrExpression), tex.GetThird().GetType()
                );
            tex = (TernaryOperatorExpression)((BetweenAndExpression)tex).GetFirst();
            NUnit.Framework.Assert.AreEqual(typeof(InExpression), tex.GetFirst().GetType());
            NUnit.Framework.Assert.AreEqual(typeof(ArithmeticIntegerDivideExpression), tex.GetSecond
                ().GetType());
            bex = (BinaryOperatorExpression)(InExpression)tex.GetFirst();
            NUnit.Framework.Assert.AreEqual(typeof(NegativeValueExpression), bex.GetLeftOprand
                ().GetType());
            NUnit.Framework.Assert.AreEqual(typeof(InExpressionList), bex.GetRightOprand().GetType
                ());
            UnaryOperatorExpression uex = (UnaryOperatorExpression)((NegativeValueExpression)
                bex.GetLeftOprand());
            NUnit.Framework.Assert.AreEqual(typeof(BitInvertExpression), uex.GetOperand().GetType
                ());
            sql = " binary case ~a||b&&c^d xor e when 2>any(select a ) then 3 else 4 end is not null =a";
            lexer = new MySQLLexer(sql);
            parser = new MySQLExprParser(lexer);
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("BINARY CASE ~ a OR b AND c ^ d XOR e WHEN 2 > ANY (SELECT a) THEN 3 ELSE 4 END IS NOT NULL = a"
                , output);
            NUnit.Framework.Assert.AreEqual(typeof(ComparisionEqualsExpression), expr.GetType
                ());
            bex = (BinaryOperatorExpression)((ComparisionEqualsExpression)expr);
            NUnit.Framework.Assert.AreEqual(typeof(ComparisionIsExpression), bex.GetLeftOprand
                ().GetType());
            NUnit.Framework.Assert.AreEqual(typeof(Identifier), bex.GetRightOprand().GetType(
                ));
            ComparisionIsExpression cex = (ComparisionIsExpression)bex.GetLeftOprand();
            NUnit.Framework.Assert.AreEqual(typeof(CastBinaryExpression), cex.GetOperand().GetType
                ());
            uex = (UnaryOperatorExpression)cex.GetOperand();
            NUnit.Framework.Assert.AreEqual(typeof(CaseWhenOperatorExpression), uex.GetOperand
                ().GetType());
            CaseWhenOperatorExpression cwex = (CaseWhenOperatorExpression)uex.GetOperand();
            NUnit.Framework.Assert.AreEqual(typeof(LogicalOrExpression), cwex.GetComparee().GetType
                ());
            PolyadicOperatorExpression pex = (LogicalOrExpression)cwex.GetComparee();
            NUnit.Framework.Assert.AreEqual(typeof(BitInvertExpression), pex.GetOperand(0).GetType
                ());
            NUnit.Framework.Assert.AreEqual(typeof(LogicalXORExpression), pex.GetOperand(1).GetType
                ());
            bex = (LogicalXORExpression)pex.GetOperand(1);
            NUnit.Framework.Assert.AreEqual(typeof(LogicalAndExpression), bex.GetLeftOprand()
                .GetType());
            NUnit.Framework.Assert.AreEqual(typeof(Identifier), bex.GetRightOprand().GetType(
                ));
            pex = (LogicalAndExpression)bex.GetLeftOprand();
            NUnit.Framework.Assert.AreEqual(typeof(Identifier), pex.GetOperand(0).GetType());
            NUnit.Framework.Assert.AreEqual(typeof(BitXORExpression), pex.GetOperand(1).GetType
                ());
            sql = " !interval(a,b)<=>a>>b collate x /?+a!=@@1 or @var sounds like -(a-b) mod -(d or e)";
            lexer = new MySQLLexer(sql);
            parser = new MySQLExprParser(lexer);
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("! INTERVAL(a, b) <=> a >> b COLLATE x / ? + a != @@1 OR @var SOUNDS LIKE - (a - b) % - (d OR e)"
                , output);
            NUnit.Framework.Assert.AreEqual(typeof(LogicalOrExpression), expr.GetType());
            pex = (LogicalOrExpression)expr;
            NUnit.Framework.Assert.AreEqual(typeof(ComparisionNotEqualsExpression), pex.GetOperand
                (0).GetType());
            NUnit.Framework.Assert.AreEqual(typeof(SoundsLikeExpression), pex.GetOperand(1).GetType
                ());
            bex = (BinaryOperatorExpression)pex.GetOperand(0);
            NUnit.Framework.Assert.AreEqual(typeof(ComparisionNullSafeEqualsExpression), bex.
                GetLeftOprand().GetType());
            NUnit.Framework.Assert.AreEqual(typeof(SysVarPrimary), bex.GetRightOprand().GetType
                ());
            bex = (BinaryOperatorExpression)bex.GetLeftOprand();
            NUnit.Framework.Assert.AreEqual(typeof(NegativeValueExpression), bex.GetLeftOprand
                ().GetType());
            NUnit.Framework.Assert.AreEqual(typeof(BitShiftExpression), bex.GetRightOprand().
                GetType());
            bex = (BinaryOperatorExpression)bex.GetRightOprand();
            NUnit.Framework.Assert.AreEqual(typeof(Identifier), bex.GetLeftOprand().GetType()
                );
            NUnit.Framework.Assert.AreEqual(typeof(ArithmeticAddExpression), bex.GetRightOprand
                ().GetType());
            bex = (BinaryOperatorExpression)bex.GetRightOprand();
            NUnit.Framework.Assert.AreEqual(typeof(ArithmeticDivideExpression), bex.GetLeftOprand
                ().GetType());
            NUnit.Framework.Assert.AreEqual(typeof(Identifier), bex.GetRightOprand().GetType(
                ));
            bex = (BinaryOperatorExpression)bex.GetLeftOprand();
            NUnit.Framework.Assert.AreEqual(typeof(CollateExpression), bex.GetLeftOprand().GetType
                ());
            NUnit.Framework.Assert.AreEqual(typeof(ParamMarker), bex.GetRightOprand().GetType
                ());
            bex = (BinaryOperatorExpression)((LogicalOrExpression)expr).GetOperand(1);
            NUnit.Framework.Assert.AreEqual(typeof(UsrDefVarPrimary), bex.GetLeftOprand().GetType
                ());
            NUnit.Framework.Assert.AreEqual(typeof(ArithmeticModExpression), bex.GetRightOprand
                ().GetType());
            bex = (BinaryOperatorExpression)bex.GetRightOprand();
            NUnit.Framework.Assert.AreEqual(typeof(MinusExpression), bex.GetLeftOprand().GetType
                ());
            NUnit.Framework.Assert.AreEqual(typeof(MinusExpression), bex.GetRightOprand().GetType
                ());
            uex = (UnaryOperatorExpression)bex.GetLeftOprand();
            NUnit.Framework.Assert.AreEqual(typeof(ArithmeticSubtractExpression), uex.GetOperand
                ().GetType());
            uex = (UnaryOperatorExpression)bex.GetRightOprand();
            NUnit.Framework.Assert.AreEqual(typeof(LogicalOrExpression), uex.GetOperand().GetType
                ());
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestAssignment()
        {
            string sql = "a /*dd*/:=b:=c";
            MySQLExprParser parser = new MySQLExprParser(new MySQLLexer(sql));
            Tup.Cobar4Net.Parser.Ast.Expression.Expression expr = parser.Expression();
            string output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("a := b := c", output);
            NUnit.Framework.Assert.AreEqual(typeof(AssignmentExpression), expr.GetType());
            AssignmentExpression ass = (AssignmentExpression)expr;
            NUnit.Framework.Assert.AreEqual(typeof(AssignmentExpression), ass.GetRightOprand(
                ).GetType());
            ass = (AssignmentExpression)ass.GetRightOprand();
            NUnit.Framework.Assert.AreEqual("b", ((Identifier)ass.GetLeftOprand()).GetIdText(
                ));
            sql = "c=@var:=1";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("c = (@var := 1)", output);
            NUnit.Framework.Assert.AreEqual(typeof(ComparisionEqualsExpression), expr.GetType
                ());
            ass = (AssignmentExpression)((BinaryOperatorExpression)expr).GetRightOprand();
            UsrDefVarPrimary usr = (UsrDefVarPrimary)ass.GetLeftOprand();
            NUnit.Framework.Assert.AreEqual("@var", usr.GetVarText());
            sql = "a:=b or c &&d :=0b1101 or b'01'&0xabc";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("a := b OR c AND d := b'1101' OR b'01' & x'abc'",
                output);
            NUnit.Framework.Assert.AreEqual(typeof(AssignmentExpression), expr.GetType());
            ass = (AssignmentExpression)expr;
            NUnit.Framework.Assert.AreEqual(typeof(AssignmentExpression), ass.GetRightOprand(
                ).GetType());
            ass = (AssignmentExpression)ass.GetRightOprand();
            NUnit.Framework.Assert.AreEqual(typeof(LogicalOrExpression), ass.GetLeftOprand().
                GetType());
            NUnit.Framework.Assert.AreEqual(typeof(LogicalOrExpression), ass.GetRightOprand()
                .GetType());
            LogicalOrExpression lor = (LogicalOrExpression)ass.GetLeftOprand();
            NUnit.Framework.Assert.AreEqual(typeof(LogicalAndExpression), lor.GetOperand(1).GetType
                ());
            lor = (LogicalOrExpression)ass.GetRightOprand();
            NUnit.Framework.Assert.AreEqual(typeof(LiteralBitField), lor.GetOperand(0).GetType
                ());
            NUnit.Framework.Assert.AreEqual(typeof(BitAndExpression), lor.GetOperand(1).GetType
                ());
            sql = "a:=((b or (c &&d)) :=((0b1101 or (b'01'&0xabc))))";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("a := b OR c AND d := b'1101' OR b'01' & x'abc'",
                output);
            NUnit.Framework.Assert.AreEqual(typeof(AssignmentExpression), expr.GetType());
            ass = (AssignmentExpression)expr;
            NUnit.Framework.Assert.AreEqual(typeof(AssignmentExpression), ass.GetRightOprand(
                ).GetType());
            ass = (AssignmentExpression)ass.GetRightOprand();
            NUnit.Framework.Assert.AreEqual(typeof(LogicalOrExpression), ass.GetLeftOprand().
                GetType());
            NUnit.Framework.Assert.AreEqual(typeof(LogicalOrExpression), ass.GetRightOprand()
                .GetType());
            lor = (LogicalOrExpression)ass.GetLeftOprand();
            NUnit.Framework.Assert.AreEqual(typeof(LogicalAndExpression), lor.GetOperand(1).GetType
                ());
            lor = (LogicalOrExpression)ass.GetRightOprand();
            NUnit.Framework.Assert.AreEqual(typeof(LiteralBitField), lor.GetOperand(0).GetType
                ());
            NUnit.Framework.Assert.AreEqual(typeof(BitAndExpression), lor.GetOperand(1).GetType
                ());
            sql = "(a:=b) or c &&(d :=0b1101 or b'01')&0xabc ^null";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("(a := b) OR c AND (d := b'1101' OR b'01') & x'abc' ^ NULL"
                , output);
            NUnit.Framework.Assert.AreEqual(typeof(LogicalOrExpression), expr.GetType());
            lor = (LogicalOrExpression)expr;
            NUnit.Framework.Assert.AreEqual(typeof(AssignmentExpression), lor.GetOperand(0).GetType
                ());
            NUnit.Framework.Assert.AreEqual(typeof(LogicalAndExpression), lor.GetOperand(1).GetType
                ());
            LogicalAndExpression land = (LogicalAndExpression)lor.GetOperand(1);
            NUnit.Framework.Assert.AreEqual(typeof(Identifier), land.GetOperand(0).GetType());
            NUnit.Framework.Assert.AreEqual(typeof(BitAndExpression), land.GetOperand(1).GetType
                ());
            BitAndExpression band = (BitAndExpression)land.GetOperand(1);
            NUnit.Framework.Assert.AreEqual(typeof(AssignmentExpression), band.GetLeftOprand(
                ).GetType());
            NUnit.Framework.Assert.AreEqual(typeof(BitXORExpression), band.GetRightOprand().GetType
                ());
            ass = (AssignmentExpression)band.GetLeftOprand();
            NUnit.Framework.Assert.AreEqual(typeof(LogicalOrExpression), ass.GetRightOprand()
                .GetType());
            BitXORExpression bxor = (BitXORExpression)band.GetRightOprand();
            NUnit.Framework.Assert.AreEqual(typeof(LiteralHexadecimal), bxor.GetLeftOprand().
                GetType());
            NUnit.Framework.Assert.AreEqual(typeof(LiteralNull), bxor.GetRightOprand().GetType
                ());
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestLogical()
        {
            string sql = "a || b Or c";
            MySQLExprParser parser = new MySQLExprParser(new MySQLLexer(sql));
            Tup.Cobar4Net.Parser.Ast.Expression.Expression expr = parser.Expression();
            string output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("a OR b OR c", output);
            NUnit.Framework.Assert.AreEqual(typeof(LogicalOrExpression), expr.GetType());
            LogicalOrExpression or = (LogicalOrExpression)expr;
            NUnit.Framework.Assert.AreEqual(3, or.GetArity());
            NUnit.Framework.Assert.AreEqual("b", ((Identifier)or.GetOperand(1)).GetIdText());
            sql = "a XOR b xOr c";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("a XOR b XOR c", output);
            NUnit.Framework.Assert.AreEqual(typeof(LogicalXORExpression), expr.GetType());
            LogicalXORExpression xor = (LogicalXORExpression)expr;
            NUnit.Framework.Assert.AreEqual(typeof(LogicalXORExpression), xor.GetLeftOprand()
                .GetType());
            xor = (LogicalXORExpression)xor.GetLeftOprand();
            NUnit.Framework.Assert.AreEqual("b", ((Identifier)xor.GetRightOprand()).GetIdText
                ());
            sql = "a XOR( b xOr c)";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("a XOR (b XOR c)", output);
            xor = (LogicalXORExpression)expr;
            LogicalXORExpression xor2 = (LogicalXORExpression)xor.GetRightOprand();
            NUnit.Framework.Assert.AreEqual("a", ((Identifier)xor.GetLeftOprand()).GetIdText(
                ));
            NUnit.Framework.Assert.AreEqual("b", ((Identifier)xor2.GetLeftOprand()).GetIdText
                ());
            NUnit.Framework.Assert.AreEqual("c", ((Identifier)xor2.GetRightOprand()).GetIdText
                ());
            sql = "a and     b && c";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("a AND b AND c", output);
            NUnit.Framework.Assert.AreEqual(typeof(LogicalAndExpression), expr.GetType());
            LogicalAndExpression and = (LogicalAndExpression)expr;
            NUnit.Framework.Assert.AreEqual(3, or.GetArity());
            NUnit.Framework.Assert.AreEqual("b", ((Identifier)and.GetOperand(1)).GetIdText());
            sql = "not NOT Not a";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("NOT NOT NOT a", output);
            NUnit.Framework.Assert.AreEqual(typeof(LogicalNotExpression), expr.GetType());
            LogicalNotExpression not = (LogicalNotExpression)((LogicalNotExpression)expr).GetOperand
                ();
            NUnit.Framework.Assert.AreEqual(typeof(LogicalNotExpression), not.GetType());
            not = (LogicalNotExpression)not.GetOperand();
            NUnit.Framework.Assert.AreEqual(typeof(LogicalNotExpression), not.GetType());
            NUnit.Framework.Assert.AreEqual("a", ((Identifier)not.GetOperand()).GetIdText());
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestComparision()
        {
            string sql = "a  betwEen b and c Not between d and e";
            MySQLExprParser parser = new MySQLExprParser(new MySQLLexer(sql));
            Tup.Cobar4Net.Parser.Ast.Expression.Expression expr = parser.Expression();
            string output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("a BETWEEN b AND c NOT BETWEEN d AND e", output);
            BetweenAndExpression ba = (BetweenAndExpression)expr;
            NUnit.Framework.Assert.AreEqual("a", ((Identifier)ba.GetFirst()).GetIdText());
            NUnit.Framework.Assert.AreEqual("b", ((Identifier)ba.GetSecond()).GetIdText());
            NUnit.Framework.Assert.AreEqual(false, ba.IsNot());
            ba = (BetweenAndExpression)ba.GetThird();
            NUnit.Framework.Assert.AreEqual("c", ((Identifier)ba.GetFirst()).GetIdText());
            NUnit.Framework.Assert.AreEqual("d", ((Identifier)ba.GetSecond()).GetIdText());
            NUnit.Framework.Assert.AreEqual("e", ((Identifier)ba.GetThird()).GetIdText());
            NUnit.Framework.Assert.AreEqual(true, ba.IsNot());
            sql = "a between b between c and d and e between f and g";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("a BETWEEN b BETWEEN c AND d AND e BETWEEN f AND g"
                , output);
            ba = (BetweenAndExpression)expr;
            BetweenAndExpression ba2 = (BetweenAndExpression)ba.GetSecond();
            BetweenAndExpression ba3 = (BetweenAndExpression)ba.GetThird();
            NUnit.Framework.Assert.AreEqual("a", ((Identifier)ba.GetFirst()).GetIdText());
            NUnit.Framework.Assert.AreEqual("b", ((Identifier)ba2.GetFirst()).GetIdText());
            NUnit.Framework.Assert.AreEqual("c", ((Identifier)ba2.GetSecond()).GetIdText());
            NUnit.Framework.Assert.AreEqual("d", ((Identifier)ba2.GetThird()).GetIdText());
            NUnit.Framework.Assert.AreEqual("e", ((Identifier)ba3.GetFirst()).GetIdText());
            NUnit.Framework.Assert.AreEqual("f", ((Identifier)ba3.GetSecond()).GetIdText());
            NUnit.Framework.Assert.AreEqual("g", ((Identifier)ba3.GetThird()).GetIdText());
            sql = "((select a)) between (select b)   and (select d) ";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("(SELECT a) BETWEEN (SELECT b) AND (SELECT d)", output
                );
            sql = "a  rliKe b not REGEXP c";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("a REGEXP b NOT REGEXP c", output);
            RegexpExpression re = (RegexpExpression)expr;
            RegexpExpression re2 = (RegexpExpression)re.GetLeftOprand();
            NUnit.Framework.Assert.AreEqual("a", ((Identifier)re2.GetLeftOprand()).GetIdText(
                ));
            NUnit.Framework.Assert.AreEqual("b", ((Identifier)re2.GetRightOprand()).GetIdText
                ());
            NUnit.Framework.Assert.AreEqual("c", ((Identifier)re.GetRightOprand()).GetIdText(
                ));
            NUnit.Framework.Assert.AreEqual(true, re.IsNot());
            NUnit.Framework.Assert.AreEqual(false, re2.IsNot());
            sql = "((a)) like (((b)))escape (((d)))";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("a LIKE b ESCAPE d", output);
            sql = "((select a)) like (((select b)))escape (((select d)))";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("(SELECT a) LIKE (SELECT b) ESCAPE (SELECT d)", output
                );
            sql = "a  like b NOT LIKE c escape d";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("a LIKE b NOT LIKE c ESCAPE d", output);
            LikeExpression le = (LikeExpression)expr;
            LikeExpression le2 = (LikeExpression)le.GetFirst();
            NUnit.Framework.Assert.AreEqual("a", ((Identifier)le2.GetFirst()).GetIdText());
            NUnit.Framework.Assert.AreEqual("b", ((Identifier)le2.GetSecond()).GetIdText());
            NUnit.Framework.Assert.AreEqual("c", ((Identifier)le.GetSecond()).GetIdText());
            NUnit.Framework.Assert.AreEqual("d", ((Identifier)le.GetThird()).GetIdText());
            NUnit.Framework.Assert.AreEqual(true, le.IsNot());
            NUnit.Framework.Assert.AreEqual(false, le2.IsNot());
            sql = "b NOT LIKE c ";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("b NOT LIKE c", output);
            sql = "a in (b) not in (select id from t1)";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("a IN (b) NOT IN (SELECT id FROM t1)", output);
            InExpression @in = (InExpression)expr;
            InExpression in2 = (InExpression)@in.GetLeftOprand();
            NUnit.Framework.Assert.AreEqual("a", ((Identifier)in2.GetLeftOprand()).GetIdText(
                ));
            NUnit.Framework.Assert.IsTrue(typeof(QueryExpression).IsAssignableFrom(@in.GetRightOprand
                ().GetType()));
            NUnit.Framework.Assert.AreEqual(true, @in.IsNot());
            NUnit.Framework.Assert.AreEqual(false, in2.IsNot());
            sql = "(select a)is not null";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("(SELECT a) IS NOT NULL", output);
            sql = "a is not null is not false is not true is not UNKNOWn is null is false is true is unknown";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("a IS NOT NULL IS NOT FALSE IS NOT TRUE IS NOT UNKNOWN IS NULL IS FALSE IS TRUE IS UNKNOWN"
                , output);
            ComparisionIsExpression @is = (ComparisionIsExpression)expr;
            ComparisionIsExpression is2 = (ComparisionIsExpression)@is.GetOperand();
            ComparisionIsExpression is3 = (ComparisionIsExpression)is2.GetOperand();
            ComparisionIsExpression is4 = (ComparisionIsExpression)is3.GetOperand();
            ComparisionIsExpression is5 = (ComparisionIsExpression)is4.GetOperand();
            ComparisionIsExpression is6 = (ComparisionIsExpression)is5.GetOperand();
            ComparisionIsExpression is7 = (ComparisionIsExpression)is6.GetOperand();
            ComparisionIsExpression is8 = (ComparisionIsExpression)is7.GetOperand();
            NUnit.Framework.Assert.AreEqual(ComparisionIsExpression.IsUnknown, @is.GetMode());
            NUnit.Framework.Assert.AreEqual(ComparisionIsExpression.IsTrue, is2.GetMode());
            NUnit.Framework.Assert.AreEqual(ComparisionIsExpression.IsFalse, is3.GetMode());
            NUnit.Framework.Assert.AreEqual(ComparisionIsExpression.IsNull, is4.GetMode());
            NUnit.Framework.Assert.AreEqual(ComparisionIsExpression.IsNotUnknown, is5.GetMode
                ());
            NUnit.Framework.Assert.AreEqual(ComparisionIsExpression.IsNotTrue, is6.GetMode());
            NUnit.Framework.Assert.AreEqual(ComparisionIsExpression.IsNotFalse, is7.GetMode()
                );
            NUnit.Framework.Assert.AreEqual(ComparisionIsExpression.IsNotNull, is8.GetMode());
            NUnit.Framework.Assert.AreEqual("a", ((Identifier)is8.GetOperand()).GetIdText());
            sql = "a = b <=> c >= d > e <= f < g <> h != i";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("a = b <=> c >= d > e <= f < g <> h != i", output
                );
            ComparisionNotEqualsExpression neq = (ComparisionNotEqualsExpression)expr;
            ComparisionLessOrGreaterThanExpression lg = (ComparisionLessOrGreaterThanExpression
                )neq.GetLeftOprand();
            ComparisionLessThanExpression l = (ComparisionLessThanExpression)lg.GetLeftOprand
                ();
            ComparisionLessThanOrEqualsExpression leq = (ComparisionLessThanOrEqualsExpression
                )l.GetLeftOprand();
            ComparisionGreaterThanExpression g = (ComparisionGreaterThanExpression)leq.GetLeftOprand
                ();
            ComparisionGreaterThanOrEqualsExpression geq = (ComparisionGreaterThanOrEqualsExpression
                )g.GetLeftOprand();
            ComparisionNullSafeEqualsExpression nseq = (ComparisionNullSafeEqualsExpression)geq
                .GetLeftOprand();
            ComparisionEqualsExpression eq = (ComparisionEqualsExpression)nseq.GetLeftOprand(
                );
            NUnit.Framework.Assert.AreEqual("i", ((Identifier)neq.GetRightOprand()).GetIdText
                ());
            NUnit.Framework.Assert.AreEqual("h", ((Identifier)lg.GetRightOprand()).GetIdText(
                ));
            NUnit.Framework.Assert.AreEqual("g", ((Identifier)l.GetRightOprand()).GetIdText()
                );
            NUnit.Framework.Assert.AreEqual("f", ((Identifier)leq.GetRightOprand()).GetIdText
                ());
            NUnit.Framework.Assert.AreEqual("e", ((Identifier)g.GetRightOprand()).GetIdText()
                );
            NUnit.Framework.Assert.AreEqual("d", ((Identifier)geq.GetRightOprand()).GetIdText
                ());
            NUnit.Framework.Assert.AreEqual("c", ((Identifier)nseq.GetRightOprand()).GetIdText
                ());
            NUnit.Framework.Assert.AreEqual("b", ((Identifier)eq.GetRightOprand()).GetIdText(
                ));
            NUnit.Framework.Assert.AreEqual("a", ((Identifier)eq.GetLeftOprand()).GetIdText()
                );
            sql = "a sounds like b sounds like c";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("a SOUNDS LIKE b SOUNDS LIKE c", output);
            SoundsLikeExpression sl = (SoundsLikeExpression)expr;
            SoundsLikeExpression sl2 = (SoundsLikeExpression)sl.GetLeftOprand();
            NUnit.Framework.Assert.AreEqual("a", ((Identifier)sl2.GetLeftOprand()).GetIdText(
                ));
            NUnit.Framework.Assert.AreEqual("b", ((Identifier)sl2.GetRightOprand()).GetIdText
                ());
            NUnit.Framework.Assert.AreEqual("c", ((Identifier)sl.GetRightOprand()).GetIdText(
                ));
            sql = "a like b escape c";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("a LIKE b ESCAPE c", output);
            sql = "(select a) collate z";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("(SELECT a) COLLATE z", output);
            sql = "val1 IN (1,2,'a')";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("val1 IN (1, 2, 'a')", output);
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestBit()
        {
            string sql = "0b01001001 | 3 & 1.2 <<d >> 0x0f";
            MySQLExprParser parser = new MySQLExprParser(new MySQLLexer(sql));
            Tup.Cobar4Net.Parser.Ast.Expression.Expression expr = parser.Expression();
            string output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("b'01001001' | 3 & 1.2 << d >> x'0f'", output);
            BitOrExpression or = (BitOrExpression)expr;
            BitAndExpression and = (BitAndExpression)or.GetRightOprand();
            BitShiftExpression rs = (BitShiftExpression)and.GetRightOprand();
            BitShiftExpression ls = (BitShiftExpression)rs.GetLeftOprand();
            NUnit.Framework.Assert.AreEqual("d", ((Identifier)ls.GetRightOprand()).GetIdText(
                ));
            NUnit.Framework.Assert.IsTrue(rs.IsRightShift());
            NUnit.Framework.Assert.IsFalse(ls.IsRightShift());
            sql = "true + b & false ^ d - null ";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("TRUE + b & FALSE ^ d - NULL", output);
            and = (BitAndExpression)expr;
            ArithmeticAddExpression add = (ArithmeticAddExpression)and.GetLeftOprand();
            ArithmeticSubtractExpression sub = (ArithmeticSubtractExpression)and.GetRightOprand
                ();
            BitXORExpression xor = (BitXORExpression)sub.GetLeftOprand();
            NUnit.Framework.Assert.AreEqual("d", ((Identifier)xor.GetRightOprand()).GetIdText
                ());
            NUnit.Framework.Assert.AreEqual("b", ((Identifier)add.GetRightOprand()).GetIdText
                ());
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestArithmetic()
        {
            string sql = "? + @usrVar1 * c/@@version- e % -f diV g";
            MySQLExprParser parser = new MySQLExprParser(new MySQLLexer(sql));
            Tup.Cobar4Net.Parser.Ast.Expression.Expression expr = parser.Expression();
            string output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("? + @usrVar1 * c / @@version - e % - f DIV g", output
                );
            ArithmeticSubtractExpression sub = (ArithmeticSubtractExpression)expr;
            ArithmeticAddExpression add = (ArithmeticAddExpression)sub.GetLeftOprand();
            ArithmeticIntegerDivideExpression idiv = (ArithmeticIntegerDivideExpression)sub.GetRightOprand
                ();
            ArithmeticModExpression mod = (ArithmeticModExpression)idiv.GetLeftOprand();
            ArithmeticDivideExpression div = (ArithmeticDivideExpression)add.GetRightOprand();
            ArithmeticMultiplyExpression mt = (ArithmeticMultiplyExpression)div.GetLeftOprand
                ();
            MinusExpression mi = (MinusExpression)mod.GetRightOprand();
            NUnit.Framework.Assert.AreEqual("c", ((Identifier)mt.GetRightOprand()).GetIdText(
                ));
            NUnit.Framework.Assert.AreEqual("f", ((Identifier)mi.GetOperand()).GetIdText());
            sql = "a+-b";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("a + - b", output);
            sql = "a+--b";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("a + - - b", output);
            sql = "a++b";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("a + b", output);
            sql = "a+++b";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("a + b", output);
            sql = "a++-b";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("a + - b", output);
            sql = "a + b mod (-((select id from t1 limit 1)- e) ) ";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("a + b % - ((SELECT id FROM t1 LIMIT 0, 1) - e)",
                output);
            add = (ArithmeticAddExpression)expr;
            mod = (ArithmeticModExpression)add.GetRightOprand();
            mi = (MinusExpression)mod.GetRightOprand();
            sub = (ArithmeticSubtractExpression)mi.GetOperand();
            NUnit.Framework.Assert.IsTrue(typeof(QueryExpression).IsAssignableFrom(sub.GetLeftOprand
                ().GetType()));
            NUnit.Framework.Assert.AreEqual("e", ((Identifier)sub.GetRightOprand()).GetIdText
                ());
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestBitHex()
        {
            string sql = "x'89af' ";
            MySQLExprParser parser = new MySQLExprParser(new MySQLLexer(sql));
            Tup.Cobar4Net.Parser.Ast.Expression.Expression expr = parser.Expression();
            string output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("x'89af'", output);
            NUnit.Framework.Assert.AreEqual("89af", ((LiteralHexadecimal)expr).GetText());
            sql = "_latin1 b'1011' ";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("_latin1 b'1011'", output);
            NUnit.Framework.Assert.AreEqual("1011", ((LiteralBitField)expr).GetText());
            NUnit.Framework.Assert.AreEqual("_latin1", ((LiteralBitField)expr).GetIntroducer(
                ));
            sql = "abc 0b1011 ";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("abc", output);
            sql = "_latin1 0xabc ";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("_latin1 x'abc'", output);
            sql = "jkl 0xabc ";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("jkl", output);
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestString()
        {
            string sql = "_latin1'abc\\'d' 'ef\"'";
            MySQLLexer lexer = new MySQLLexer(sql);
            MySQLExprParser parser = new MySQLExprParser(lexer);
            Tup.Cobar4Net.Parser.Ast.Expression.Expression expr = parser.Expression();
            string output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("_latin1'abc\\'def\"'", output);
            sql = "n'abc\\'d' \"ef'\"\"\"";
            lexer = new MySQLLexer(sql);
            parser = new MySQLExprParser(lexer);
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("N'abc\\'def\\'\"'", output);
            sql = "`char`'an'";
            lexer = new MySQLLexer(sql);
            parser = new MySQLExprParser(lexer);
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("`char`", output);
            sql = "_latin1 n'abc' ";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("_latin1", output);
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestAnyAll()
        {
            string sql = "1 >= any (select id from t1 limit 1)";
            MySQLExprParser parser = new MySQLExprParser(new MySQLLexer(sql));
            Tup.Cobar4Net.Parser.Ast.Expression.Expression expr = parser.Expression();
            string output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("1 >= ANY (SELECT id FROM t1 LIMIT 0, 1)", output
                );
            NUnit.Framework.Assert.AreEqual(typeof(ComparisionGreaterThanOrEqualsExpression),
                expr.GetType());
            sql = "1 >= any (select id from t1 limit 1) > aLl(select tb1.id from tb1 t1,tb2 as t2 where t1.id=t2.id limit 1)";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("1 >= ANY (SELECT id FROM t1 LIMIT 0, 1) > ALL (SELECT tb1.id FROM tb1 AS T1, tb2 AS T2 WHERE t1.id = t2.id LIMIT 0, 1)"
                , output);
            ComparisionGreaterThanExpression gt = (ComparisionGreaterThanExpression)expr;
            ComparisionGreaterThanOrEqualsExpression ge = (ComparisionGreaterThanOrEqualsExpression
                )gt.GetLeftOprand();
            NUnit.Framework.Assert.AreEqual(typeof(LiteralNumber), ge.GetLeftOprand().GetType
                ());
            sql = "1 >= any + any";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("1 >= any + any", output);
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestUnary()
        {
            string sql = "!-~ binary a collate latin1_danish_ci";
            MySQLExprParser parser = new MySQLExprParser(new MySQLLexer(sql));
            Tup.Cobar4Net.Parser.Ast.Expression.Expression expr = parser.Expression();
            string output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("! - ~ BINARY a COLLATE latin1_danish_ci", output
                );
            NegativeValueExpression neg = (NegativeValueExpression)expr;
            MinusExpression mi = (MinusExpression)neg.GetOperand();
            BitInvertExpression bi = (BitInvertExpression)mi.GetOperand();
            CastBinaryExpression bin = (CastBinaryExpression)bi.GetOperand();
            CollateExpression col = (CollateExpression)bin.GetOperand();
            NUnit.Framework.Assert.AreEqual("a", ((Identifier)col.GetString()).GetIdText());
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestUser()
        {
            string sql = "'root'@'localhost'";
            MySQLExprParser parser = new MySQLExprParser(new MySQLLexer(sql));
            Tup.Cobar4Net.Parser.Ast.Expression.Expression expr = parser.Expression();
            string output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("'root'@'localhost'", output);
            UserExpression usr = (UserExpression)expr;
            NUnit.Framework.Assert.AreEqual("'root'@'localhost'", usr.GetUserAtHost());
            sql = "root@localhost";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("root@localhost", output);
            usr = (UserExpression)expr;
            NUnit.Framework.Assert.AreEqual("root@localhost", usr.GetUserAtHost());
            sql = "var@'localhost'";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("var@'localhost'", output);
            usr = (UserExpression)expr;
            NUnit.Framework.Assert.AreEqual("var@'localhost'", usr.GetUserAtHost());
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestPrimarySystemVar()
        {
            string sql = "@@gloBal . /*dd*/ `all`";
            MySQLLexer lexer = new MySQLLexer(sql);
            MySQLExprParser parser = new MySQLExprParser(lexer);
            Tup.Cobar4Net.Parser.Ast.Expression.Expression expr = parser.Expression();
            string output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("@@global.`all`", output);
            NUnit.Framework.Assert.AreEqual(typeof(SysVarPrimary), expr.GetType());
            SysVarPrimary sysvar = (SysVarPrimary)expr;
            NUnit.Framework.Assert.AreEqual(VariableScope.Global, sysvar.GetScope());
            NUnit.Framework.Assert.AreEqual("`all`", sysvar.GetVarText());
            sql = "@@Session . /*dd*/ any";
            lexer = new MySQLLexer(sql);
            parser = new MySQLExprParser(lexer);
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("@@any", output);
            NUnit.Framework.Assert.AreEqual(typeof(SysVarPrimary), expr.GetType());
            sysvar = (SysVarPrimary)expr;
            NUnit.Framework.Assert.AreEqual(VariableScope.Session, sysvar.GetScope());
            NUnit.Framework.Assert.AreEqual("any", sysvar.GetVarText());
            sql = "@@LOCAl . /*dd*/ `usage`";
            lexer = new MySQLLexer(sql);
            parser = new MySQLExprParser(lexer);
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("@@`usage`", output);
            NUnit.Framework.Assert.AreEqual(typeof(SysVarPrimary), expr.GetType());
            sysvar = (SysVarPrimary)expr;
            NUnit.Framework.Assert.AreEqual(VariableScope.Session, sysvar.GetScope());
            NUnit.Framework.Assert.AreEqual("`usage`", sysvar.GetVarText());
            sql = "@@LOCAl . /*dd*/ `var1`";
            lexer = new MySQLLexer(sql);
            parser = new MySQLExprParser(lexer);
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("@@`var1`", output);
            NUnit.Framework.Assert.AreEqual(typeof(SysVarPrimary), expr.GetType());
            sysvar = (SysVarPrimary)expr;
            NUnit.Framework.Assert.AreEqual(VariableScope.Session, sysvar.GetScope());
            NUnit.Framework.Assert.AreEqual("`var1`", sysvar.GetVarText());
            sql = "@@var1   ,";
            lexer = new MySQLLexer(sql);
            parser = new MySQLExprParser(lexer);
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("@@var1", output);
            NUnit.Framework.Assert.AreEqual(typeof(SysVarPrimary), expr.GetType());
            sysvar = (SysVarPrimary)expr;
            NUnit.Framework.Assert.AreEqual(VariableScope.Session, sysvar.GetScope());
            NUnit.Framework.Assert.AreEqual("var1", sysvar.GetVarText());
            sql = "@@`case``1`   ,@@_";
            lexer = new MySQLLexer(sql);
            parser = new MySQLExprParser(lexer);
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("@@`case``1`", output);
            NUnit.Framework.Assert.AreEqual(typeof(SysVarPrimary), expr.GetType());
            sysvar = (SysVarPrimary)expr;
            NUnit.Framework.Assert.AreEqual(VariableScope.Session, sysvar.GetScope());
            NUnit.Framework.Assert.AreEqual("`case``1`", sysvar.GetVarText());
            lexer.NextToken();
            parser = new MySQLExprParser(lexer);
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("@@_", output);
            NUnit.Framework.Assert.AreEqual(typeof(SysVarPrimary), expr.GetType());
            sysvar = (SysVarPrimary)expr;
            NUnit.Framework.Assert.AreEqual(VariableScope.Session, sysvar.GetScope());
            NUnit.Framework.Assert.AreEqual("_", sysvar.GetVarText());
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestPrimary()
        {
            string sql = "(1,2,existS (select id.* from t1))";
            MySQLExprParser parser = new MySQLExprParser(new MySQLLexer(sql));
            Tup.Cobar4Net.Parser.Ast.Expression.Expression expr = parser.Expression();
            string output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("ROW(1, 2, EXISTS (SELECT id.* FROM t1))", output
                );
            RowExpression row = (RowExpression)expr;
            NUnit.Framework.Assert.AreEqual(3, row.GetRowExprList().Count);
            sql = "*";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("*", output);
            NUnit.Framework.Assert.IsTrue(typeof(Wildcard).IsAssignableFrom(expr.GetType()));
            sql = "case v1 when `index` then a when 2 then b else c end";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("CASE v1 WHEN `index` THEN a WHEN 2 THEN b ELSE c END"
                , output);
            CaseWhenOperatorExpression cw = (CaseWhenOperatorExpression)expr;
            NUnit.Framework.Assert.AreEqual("v1", ((Identifier)cw.GetComparee()).GetIdText());
            NUnit.Framework.Assert.AreEqual(2, cw.GetWhenList().Count);
            NUnit.Framework.Assert.AreEqual("c", ((Identifier)cw.GetElseResult()).GetIdText()
                );
            sql = "case  when 1=value then a  end";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("CASE WHEN 1 = value THEN a END", output);
            cw = (CaseWhenOperatorExpression)expr;
            NUnit.Framework.Assert.IsNull(cw.GetComparee());
            NUnit.Framework.Assert.AreEqual(1, cw.GetWhenList().Count);
            NUnit.Framework.Assert.IsNull(cw.GetElseResult());
            sql = "case  when 1=`in` then a  end";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("CASE WHEN 1 = `in` THEN a END", output);
            sql = " ${INSENSITIVE}. ";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("${INSENSITIVE}", output);
            sql = "current_date, ";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("CURDATE()", output);
            sql = "CurRent_Date  (  ) ";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("CURDATE()", output);
            sql = "CurRent_TiMe   ";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("CURTIME()", output);
            sql = "CurRent_TiMe  () ";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("CURTIME()", output);
            sql = "CurRent_TimesTamp ";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("NOW()", output);
            sql = "CurRent_TimesTamp  ()";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("NOW()", output);
            sql = "localTimE";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("NOW()", output);
            sql = "localTimE  () ";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("NOW()", output);
            sql = "localTimesTamP  ";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("NOW()", output);
            sql = "localTimesTamP  () ";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("NOW()", output);
            sql = "CurRent_user ";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("CURRENT_USER()", output);
            sql = "CurRent_user  () ";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("CURRENT_USER()", output);
            sql = "default  () ";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("DEFAULT()", output);
            sql = "vaLueS(1,col1*2)";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("VALUES(1, col1 * 2)", output);
            sql = "(1,2,mod(m,n))";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("ROW(1, 2, m % n)", output);
            sql = "chaR (77,121,'77.3')";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("CHAR(77, 121, '77.3')", output);
            sql = "CHARSET(CHAR(0x65 USING utf8))";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("CHARSET(CHAR(x'65' USING utf8))", output);
            sql = "CONVERT(_latin1'Müller' USING utf8)";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("CONVERT(_latin1'Müller' USING utf8)", output);
        }

        // QS_TODO
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestStartedFromIdentifier()
        {
            // QS_TODO
            string sql = "cast(CAST(1-2 AS UNSIGNED) AS SIGNED)";
            MySQLExprParser parser = new MySQLExprParser(new MySQLLexer(sql));
            Tup.Cobar4Net.Parser.Ast.Expression.Expression expr = parser.Expression();
            string output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("CAST(CAST(1 - 2 AS UNSIGNED) AS SIGNED)", output
                );
            sql = "position('a' in \"abc\")";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("LOCATE('a', 'abc')", output);
            sql = "cast(CAST(1-2 AS UNSIGNED integer) AS SIGNED integer)";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("CAST(CAST(1 - 2 AS UNSIGNED) AS SIGNED)", output
                );
            sql = "CAST(expr as char)";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("CAST(expr AS CHAR)", output);
            sql = "CAST(6/4 AS DECIMAL(3,1))";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("CAST(6 / 4 AS DECIMAL(3, 1))", output);
            sql = "CAST(6/4 AS DECIMAL(3))";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("CAST(6 / 4 AS DECIMAL(3))", output);
            sql = "CAST(6/4 AS DECIMAL)";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("CAST(6 / 4 AS DECIMAL)", output);
            sql = "CAST(now() as date)";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("CAST(NOW() AS DATE)", output);
            sql = "CAST(expr as char(5))";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("CAST(expr AS CHAR(5))", output);
            sql = "SUBSTRING('abc',pos,len)";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("SUBSTRING('abc', pos, len)", output);
            sql = "SUBSTRING('abc' FROM pos FOR len)";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("SUBSTRING('abc', pos, len)", output);
            sql = "SUBSTRING(str,pos)";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("SUBSTRING(str, pos)", output);
            sql = "SUBSTRING('abc',1,2)";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("SUBSTRING('abc', 1, 2)", output);
            sql = "row(1,2,str)";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("ROW(1, 2, str)", output);
            sql = "position(\"abc\" in '/*abc*/')";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("LOCATE('abc', '/*abc*/')", output);
            sql = "locate(localtime,b)";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("LOCATE(NOW(), b)", output);
            sql = "locate(locate(a,b),`match`)";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("LOCATE(LOCATE(a, b), `match`)", output);
            sql = "TRIM(LEADING 'x' FROM 'xxxbarxxx')";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("TRIM(LEADING 'x' FROM 'xxxbarxxx')", output);
            sql = "TRIM(BOTH 'x' FROM 'xxxbarxxx')";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("TRIM(BOTH 'x' FROM 'xxxbarxxx')", output);
            sql = "TRIM(TRAILING 'xyz' FROM 'barxxyz')";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("TRIM(TRAILING 'xyz' FROM 'barxxyz')", output);
            sql = "TRIM('  if   ')";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("TRIM('  if   ')", output);
            sql = "TRIM( 'x' FROM 'xxxbarxxx')";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("TRIM('x' FROM 'xxxbarxxx')", output);
            sql = "TRIM(both  FROM 'barxxyz')";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("TRIM(BOTH  FROM 'barxxyz')", output);
            sql = "TRIM(leading  FROM 'barxxyz')";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("TRIM(LEADING  FROM 'barxxyz')", output);
            sql = "TRIM(TRAILING  FROM 'barxxyz')";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("TRIM(TRAILING  FROM 'barxxyz')", output);
            sql = "avg(DISTINCT results)";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("AVG(DISTINCT results)", output);
            sql = "avg(results)";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("AVG(results)", output);
            sql = "max(DISTINCT results)";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("MAX(DISTINCT results)", output);
            sql = "max(results)";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("MAX(results)", output);
            sql = "min(DISTINCT results)";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("MIN(DISTINCT results)", output);
            sql = "min(results)";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("MIN(results)", output);
            sql = "sum(DISTINCT results)";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("SUM(DISTINCT results)", output);
            sql = "sum(results)";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("SUM(results)", output);
            sql = "count(DISTINCT expr1,expr2,expr3)";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("COUNT(DISTINCT expr1, expr2, expr3)", output);
            sql = "count(*)";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("COUNT(*)", output);
            sql = "GROUP_CONCAT(DISTINCT expr1,expr2,expr3 ORDER BY col_name1 DESC,col_name2 SEPARATOR ' ')";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("GROUP_CONCAT(DISTINCT expr1, expr2, expr3 ORDER BY col_name1 DESC, col_name2 SEPARATOR  )"
                , output);
            sql = "GROUP_CONCAT(a||b,expr2,expr3 ORDER BY col_name1 asc,col_name2 SEPARATOR '@ ')";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("GROUP_CONCAT(a OR b, expr2, expr3 ORDER BY col_name1 ASC, col_name2 SEPARATOR @ )"
                , output);
            sql = "GROUP_CONCAT(expr1 ORDER BY col_name1 asc,col_name2 SEPARATOR 'str_val ')";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("GROUP_CONCAT(expr1 ORDER BY col_name1 ASC, col_name2 SEPARATOR str_val )"
                , output);
            sql = "GROUP_CONCAT(DISTINCT test_score ORDER BY test_score DESC )";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("GROUP_CONCAT(DISTINCT test_score ORDER BY test_score DESC SEPARATOR ,)"
                , output);
            sql = "GROUP_CONCAT(DISTINCT test_score ORDER BY test_score asc )";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("GROUP_CONCAT(DISTINCT test_score ORDER BY test_score ASC SEPARATOR ,)"
                , output);
            sql = "GROUP_CONCAT(c1)";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("GROUP_CONCAT(c1 SEPARATOR ,)", output);
            sql = "GROUP_CONCAT(c1 separator '')";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("GROUP_CONCAT(c1 SEPARATOR )", output);
            sql = "default";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("DEFAULT", output);
            sql = "default(col)";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("DEFAULT(col)", output);
            sql = "database()";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("DATABASE()", output);
            sql = "if(1>2,a+b,a:=1)";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("IF(1 > 2, a + b, a := 1)", output);
            sql = "insert('abc',1,2,'')";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("INSERT('abc', 1, 2, '')", output);
            sql = "left(\"hjkafag\",4)";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("LEFT('hjkafag', 4)", output);
            sql = "repeat('ag',2.1e1)";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("REPEAT('ag', 21)", output);
            sql = "replace('anjd',\"df\",'af')";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("REPLACE('anjd', 'df', 'af')", output);
            sql = "right(\"hjkafag\",4)";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("RIGHT('hjkafag', 4)", output);
            sql = "schema()";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("DATABASE()", output);
            sql = "utc_date()";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("UTC_DATE()", output);
            sql = "Utc_time()";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("UTC_TIME()", output);
            sql = "Utc_timestamp()";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("UTC_TIMESTAMP()", output);
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestInterval()
        {
            // QS_TODO
            string sql = "DATE_ADD('2009-01-01', INTERVAL (6/4) HOUR_MINUTE)";
            MySQLExprParser parser = new MySQLExprParser(new MySQLLexer(sql));
            Tup.Cobar4Net.Parser.Ast.Expression.Expression expr = parser.Expression();
            string output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("DATE_ADD('2009-01-01', INTERVAL (6 / 4) HOUR_MINUTE)"
                , output);
            sql = "'2008-12-31 23:59:59' + INTERVAL 1 SECOND";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("'2008-12-31 23:59:59' + INTERVAL 1 SECOND", output
                );
            sql = " INTERVAL 1 DAY + '2008-12-31'";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("INTERVAL 1 DAY + '2008-12-31'", output);
            sql = "DATE_ADD('2100-12-31 23:59:59',INTERVAL '1:1' MINUTE_SECOND)";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("DATE_ADD('2100-12-31 23:59:59', INTERVAL '1:1' MINUTE_SECOND)"
                , output);
            sql = "DATE_SUB('2005-01-01 00:00:00',INTERVAL '1 1:1:1' DAY_SECOND)";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("DATE_SUB('2005-01-01 00:00:00', INTERVAL '1 1:1:1' DAY_SECOND)"
                , output);
            sql = "DATE_ADD('1900-01-01 00:00:00',INTERVAL '-1 10' DAY_HOUR)";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("DATE_ADD('1900-01-01 00:00:00', INTERVAL '-1 10' DAY_HOUR)"
                , output);
            sql = "DATE_SUB('1998-01-02', INTERVAL 31 DAY)";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("DATE_SUB('1998-01-02', INTERVAL 31 DAY)", output
                );
            sql = "DATE_ADD('1992-12-31 23:59:59.000002',INTERVAL '1.999999' SECOND_MICROSECOND)";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("DATE_ADD('1992-12-31 23:59:59.000002', INTERVAL '1.999999' SECOND_MICROSECOND)"
                , output);
            sql = "DATE_ADD('2013-01-01', INTERVAL 1 HOUR)";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("DATE_ADD('2013-01-01', INTERVAL 1 HOUR)", output
                );
            sql = "DATE_ADD('2009-01-30', INTERVAL 1 MONTH)";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("DATE_ADD('2009-01-30', INTERVAL 1 MONTH)", output
                );
            sql = "DATE_ADD('1992-12-31 23:59:59.000002',INTERVAL '1:1.999999' minute_MICROSECOND)";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("DATE_ADD('1992-12-31 23:59:59.000002', INTERVAL '1:1.999999' MINUTE_MICROSECOND)"
                , output);
            sql = "DATE_ADD('1992-12-31 23:59:59.000002',INTERVAL '1:1:1.999999' hour_MICROSECOND)";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("DATE_ADD('1992-12-31 23:59:59.000002', INTERVAL '1:1:1.999999' HOUR_MICROSECOND)"
                , output);
            sql = "DATE_ADD('2100-12-31 23:59:59',INTERVAL '1:1:1' hour_SECOND)";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("DATE_ADD('2100-12-31 23:59:59', INTERVAL '1:1:1' HOUR_SECOND)"
                , output);
            sql = "DATE_ADD('1992-12-31 23:59:59.000002',INTERVAL '1 1:1:1.999999' day_MICROSECOND)";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("DATE_ADD('1992-12-31 23:59:59.000002', INTERVAL '1 1:1:1.999999' DAY_MICROSECOND)"
                , output);
            sql = "DATE_ADD('2100-12-31 23:59:59',INTERVAL '1 1:1' day_minute)";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("DATE_ADD('2100-12-31 23:59:59', INTERVAL '1 1:1' DAY_MINUTE)"
                , output);
            sql = "DATE_ADD('2100-12-31',INTERVAL '1-1' year_month)";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("DATE_ADD('2100-12-31', INTERVAL '1-1' YEAR_MONTH)"
                , output);
            sql = "INTERVAL(n1,n2,n3)";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("INTERVAL(n1, n2, n3)", output);
            sql = "INTERVAL a+b day";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("INTERVAL (a + b) DAY", output);
            sql = "INTERVAL(select id from t1) day";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("INTERVAL (SELECT id FROM t1) DAY", output);
            sql = "INTERVAL(('jklj'+a))day";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("INTERVAL ('jklj' + a) DAY", output);
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestMatchExpression()
        {
            // QS_TODO
            string sql = "MATCH (title,body) AGAINST ('database' WITH QUERY EXPANSION)";
            MySQLExprParser parser = new MySQLExprParser(new MySQLLexer(sql));
            Tup.Cobar4Net.Parser.Ast.Expression.Expression expr = parser.Expression();
            string output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("MATCH (title, body) AGAINST ('database' WITH QUERY EXPANSION)"
                , output);
            NUnit.Framework.Assert.AreEqual("MATCH (title, body) AGAINST ('database' WITH QUERY EXPANSION)"
                , output);
            sql = "MATCH (title,body) AGAINST ( (abc in (d)) IN boolean MODE)";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("MATCH (title, body) AGAINST ((abc IN (d)) IN BOOLEAN MODE)"
                , output);
            sql = "MATCH (title,body) AGAINST ('database')";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("MATCH (title, body) AGAINST ('database')", output
                );
            sql = "MATCH (col1,col2,col3) AGAINST ((a:=b:=c) IN boolean MODE)";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("MATCH (col1, col2, col3) AGAINST (a := b := c IN BOOLEAN MODE)"
                , output);
            sql = "MATCH (title,body) AGAINST ((a and (b ||c)) IN boolean MODE)";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("MATCH (title, body) AGAINST (a AND (b OR c) IN BOOLEAN MODE)"
                , output);
            sql = "MATCH (title,body) AGAINST ((a between b and c) IN boolean MODE)";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("MATCH (title, body) AGAINST (a BETWEEN b AND c IN BOOLEAN MODE)"
                , output);
            sql = "MATCH (title,body) AGAINST ((a between b and (abc in (d))) IN boolean MODE)";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("MATCH (title, body) AGAINST ((a BETWEEN b AND abc IN (d)) IN BOOLEAN MODE)"
                , output);
            sql = "MATCH (title,body) AGAINST ((not not a) IN boolean MODE)";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("MATCH (title, body) AGAINST (NOT NOT a IN BOOLEAN MODE)"
                , output);
            sql = "MATCH (title,body) AGAINST ((a is true) IN boolean MODE)";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("MATCH (title, body) AGAINST (a IS TRUE IN BOOLEAN MODE)"
                , output);
            sql = "MATCH (title,body) AGAINST ((select a) IN boolean MODE)";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("MATCH (title, body) AGAINST (SELECT a IN BOOLEAN MODE)"
                , output);
            sql = "MATCH (title,body) AGAINST ('database' IN NATURAL LANGUAGE MODE)";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("MATCH (title, body) AGAINST ('database' IN NATURAL LANGUAGE MODE)"
                , output);
            sql = "MATCH (title,body) AGAINST ('database' IN NATURAL LANGUAGE MODE WITH QUERY EXPANSION)";
            parser = new MySQLExprParser(new MySQLLexer(sql));
            expr = parser.Expression();
            output = Output2MySQL(expr, sql);
            NUnit.Framework.Assert.AreEqual("MATCH (title, body) AGAINST ('database' IN NATURAL LANGUAGE MODE WITH QUERY EXPANSION)"
                , output);
        }
    }
}