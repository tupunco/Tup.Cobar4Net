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
/**
 * (created at 2011-3-14)
 */

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Tup.Cobar.SqlParser.Recognizer.MySql.Lexer;
using Tup.Cobar.SqlParser.Recognizer.MySql;

namespace Tup.Cobar.SqlParser.UnitTest.Recognizer.MySql.Lexer
{
    /**
     * @author <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
     */
    [TestClass]
    public class MySQLLexerTest
    {
        //public static void Main(string[] args) //// throws SQLSyntaxErrorException
        //{
        //    var sql = Performance.SQL_BENCHMARK_SELECT;
        //    char[] chars = sql.ToCharArray();
        //    MySQLLexer sut = new MySQLLexer(sql);
        //    long start = System.currentTimeMillis();
        //    long end = System.currentTimeMillis();
        //    for (int i = 0; i < 1; ++i)
        //    {
        //        for (; !sut.eof();)
        //        {
        //            sut.nextToken();
        //            switch (sut.GetToken())
        //            {
        //                case LITERAL_NUM_MIX_DIGIT:
        //                    sut.decimalValue();
        //                    break;
        //                case LITERAL_NUM_PURE_DIGIT:
        //                    sut.GetIntegerValue();
        //                    break;
        //                default:
        //                    sut.GetStringValue();
        //                    break;
        //            }
        //        }
        //    }

        //    int loop = 5000000;
        //    sut = new MySQLLexer(sql);
        //    start = System.currentTimeMillis();
        //    for (int i = 0; i < loop; ++i)
        //    {
        //        sut = new MySQLLexer(chars);
        //        for (; !sut.eof();)
        //        {
        //            sut.nextToken();
        //            switch (sut.GetToken())
        //            {
        //                case LITERAL_NUM_MIX_DIGIT:
        //                    sut.decimalValue();
        //                    break;
        //                case LITERAL_NUM_PURE_DIGIT:
        //                    sut.GetIntegerValue();
        //                    break;
        //                default:
        //                    sut.GetStringValue();
        //                    break;
        //            }
        //        }
        //    }
        //    end = System.currentTimeMillis();
        //    System.out.println((end - start) * 1.0d / (loop / 1000) + " us.");
        //}

        [TestMethod]
        public void testParameter() // throws SQLSyntaxErrorException
        {
            MySQLLexer sut = new MySQLLexer("?,?,?");
            Assert.AreEqual(MySQLToken.QUESTION_MARK, sut.GetToken());
            Assert.AreEqual(1, sut.GetParamIndex());
            sut.nextToken();
            sut.nextToken();
            Assert.AreEqual(MySQLToken.QUESTION_MARK, sut.GetToken());
            Assert.AreEqual(2, sut.GetParamIndex());
            sut.nextToken();
            sut.nextToken();
            Assert.AreEqual(MySQLToken.QUESTION_MARK, sut.GetToken());
            Assert.AreEqual(3, sut.GetParamIndex());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());
        }

        [TestMethod]
        public void testUserDefVar() // throws SQLSyntaxErrorException
        {
            MySQLLexer sut = new MySQLLexer("@abc  ");
            Assert.AreEqual(MySQLToken.USR_VAR, sut.GetToken());
            Assert.AreEqual("@abc", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("@abc.d  ");
            Assert.AreEqual(MySQLToken.USR_VAR, sut.GetToken());
            Assert.AreEqual("@abc.d", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("@abc_$.d");
            Assert.AreEqual(MySQLToken.USR_VAR, sut.GetToken());
            Assert.AreEqual("@abc_$.d", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("@abc_$_.");
            Assert.AreEqual(MySQLToken.USR_VAR, sut.GetToken());
            Assert.AreEqual("@abc_$_.", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("@''");
            Assert.AreEqual(MySQLToken.USR_VAR, sut.GetToken());
            Assert.AreEqual("@''", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("@'\\''");
            Assert.AreEqual(MySQLToken.USR_VAR, sut.GetToken());
            Assert.AreEqual("@'\\''", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("@''''");
            Assert.AreEqual(MySQLToken.USR_VAR, sut.GetToken());
            Assert.AreEqual("@''''", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("@'\"'");
            Assert.AreEqual(MySQLToken.USR_VAR, sut.GetToken());
            Assert.AreEqual("@'\"'", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("@'\"\"'");
            Assert.AreEqual(MySQLToken.USR_VAR, sut.GetToken());
            Assert.AreEqual("@'\"\"'", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("@'\\\"'");
            Assert.AreEqual(MySQLToken.USR_VAR, sut.GetToken());
            Assert.AreEqual("@'\\\"'", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("@'ac\\''  ");
            Assert.AreEqual(MySQLToken.USR_VAR, sut.GetToken());
            Assert.AreEqual("@'ac\\''", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("@'''ac\\''  ");
            Assert.AreEqual(MySQLToken.USR_VAR, sut.GetToken());
            Assert.AreEqual("@'''ac\\''", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("@'abc'''ac\\''  ");
            Assert.AreEqual(MySQLToken.USR_VAR, sut.GetToken());
            Assert.AreEqual("@'abc'''", sut.GetStringValue());

            sut = new MySQLLexer("@''abc''");
            Assert.AreEqual(MySQLToken.USR_VAR, sut.GetToken());
            Assert.AreEqual("@''", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("abc", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.LITERAL_CHARS, sut.GetToken());
            Assert.AreEqual("''", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("@\"\"  ");
            Assert.AreEqual(MySQLToken.USR_VAR, sut.GetToken());
            Assert.AreEqual("@\"\"", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("@\"\"\"abc\"  ");
            Assert.AreEqual(MySQLToken.USR_VAR, sut.GetToken());
            Assert.AreEqual("@\"\"\"abc\"", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("@\"\\\"\"\"abc\"  ");
            Assert.AreEqual(MySQLToken.USR_VAR, sut.GetToken());
            Assert.AreEqual("@\"\\\"\"\"abc\"", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("@\"\\\"\"  ");
            Assert.AreEqual(MySQLToken.USR_VAR, sut.GetToken());
            Assert.AreEqual("@\"\\\"\"", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("@\"\"\"\\\"d\"  ");
            Assert.AreEqual(MySQLToken.USR_VAR, sut.GetToken());
            Assert.AreEqual("@\"\"\"\\\"d\"", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("@\"'\"  ");
            Assert.AreEqual(MySQLToken.USR_VAR, sut.GetToken());
            Assert.AreEqual("@\"'\"", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("@`` ");
            Assert.AreEqual(MySQLToken.USR_VAR, sut.GetToken());
            Assert.AreEqual("@``", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("@````");
            Assert.AreEqual(MySQLToken.USR_VAR, sut.GetToken());
            Assert.AreEqual("@````", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("@` `");
            Assert.AreEqual(MySQLToken.USR_VAR, sut.GetToken());
            Assert.AreEqual("@` `", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("@`abv```");
            Assert.AreEqual(MySQLToken.USR_VAR, sut.GetToken());
            Assert.AreEqual("@`abv```", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("@`````abc`");
            Assert.AreEqual(MySQLToken.USR_VAR, sut.GetToken());
            Assert.AreEqual("@`````abc`", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("@`````abc```");
            Assert.AreEqual(MySQLToken.USR_VAR, sut.GetToken());
            Assert.AreEqual("@`````abc```", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("@``abc");
            Assert.AreEqual(MySQLToken.USR_VAR, sut.GetToken());
            Assert.AreEqual("@``", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("abc", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("@`abc`````abc```");
            Assert.AreEqual(MySQLToken.USR_VAR, sut.GetToken());
            Assert.AreEqual("@`abc`````", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("abc", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("```", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer(" -- \n  @  #abc\n\r\t\"\"@\"abc\\\\''-- abc\n'''\\\"\"\"\"");
            Assert.AreEqual(MySQLToken.USR_VAR, sut.GetToken());
            Assert.AreEqual("@", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.LITERAL_CHARS, sut.GetToken());
            Assert.AreEqual("''", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.USR_VAR, sut.GetToken());
            Assert.AreEqual("@\"abc\\\\''-- abc\n'''\\\"\"\"\"", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("/**/@a #@abc\n@.\r\t");
            Assert.AreEqual(MySQLToken.USR_VAR, sut.GetToken());
            Assert.AreEqual("@a", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.USR_VAR, sut.GetToken());
            Assert.AreEqual("@.", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("  #@abc\n@\"1a_-@#!''`=\\a\"-- @\r\n@'-_1a/**/\\\"\\''/*@abc*/@`_1@\\''\"`");
            Assert.AreEqual(MySQLToken.USR_VAR, sut.GetToken());
            Assert.AreEqual("@\"1a_-@#!''`=\\a\"", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.USR_VAR, sut.GetToken());
            Assert.AreEqual("@'-_1a/**/\\\"\\''", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.USR_VAR, sut.GetToken());
            Assert.AreEqual("@`_1@\\''\"`", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("  /*! */@._a$ @_a.b$c.\r@1_a.$#\n@A.a_/@-- \n@_--@.[]'\"@#abc'@a,@;@~#@abc");
            Assert.AreEqual(MySQLToken.USR_VAR, sut.GetToken());
            Assert.AreEqual("@._a$", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.USR_VAR, sut.GetToken());
            Assert.AreEqual("@_a.b$c.", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.USR_VAR, sut.GetToken());
            Assert.AreEqual("@1_a.$", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.USR_VAR, sut.GetToken());
            Assert.AreEqual("@A.a_", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.OP_SLASH, sut.GetToken());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.USR_VAR, sut.GetToken());
            Assert.AreEqual("@", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.USR_VAR, sut.GetToken());
            Assert.AreEqual("@_", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.OP_MINUS, sut.GetToken());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.OP_MINUS, sut.GetToken());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.USR_VAR, sut.GetToken());
            Assert.AreEqual("@.", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.PUNC_LEFT_BRACKET, sut.GetToken());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.PUNC_RIGHT_BRACKET, sut.GetToken());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.LITERAL_CHARS, sut.GetToken());
            Assert.AreEqual("'\"@#abc'", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.USR_VAR, sut.GetToken());
            Assert.AreEqual("@a", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.PUNC_COMMA, sut.GetToken());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.USR_VAR, sut.GetToken());
            Assert.AreEqual("@", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.PUNC_SEMICOLON, sut.GetToken());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.USR_VAR, sut.GetToken());
            Assert.AreEqual("@", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.OP_TILDE, sut.GetToken());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());
        }

        [TestMethod]
        public void testSystemVar() // throws SQLSyntaxErrorException
        {
            MySQLLexer sut = new MySQLLexer("@@abc  ");
            Assert.AreEqual(MySQLToken.SYS_VAR, sut.GetToken());
            Assert.AreEqual("abc", sut.GetStringValue());
            Assert.AreEqual("ABC", sut.GetStringValueUppercase());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("@@`abc`  ");
            Assert.AreEqual(MySQLToken.SYS_VAR, sut.GetToken());
            Assert.AreEqual("`abc`", sut.GetStringValue());
            Assert.AreEqual("`ABC`", sut.GetStringValueUppercase());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("@@```abc`  ");
            Assert.AreEqual(MySQLToken.SYS_VAR, sut.GetToken());
            Assert.AreEqual("```abc`", sut.GetStringValue());
            Assert.AreEqual("```ABC`", sut.GetStringValueUppercase());

            sut = new MySQLLexer("@@``  ");
            Assert.AreEqual(MySQLToken.SYS_VAR, sut.GetToken());
            Assert.AreEqual("``", sut.GetStringValue());
            Assert.AreEqual("``", sut.GetStringValueUppercase());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("@@`a```  ");
            Assert.AreEqual(MySQLToken.SYS_VAR, sut.GetToken());
            Assert.AreEqual("`a```", sut.GetStringValue());
            Assert.AreEqual("`A```", sut.GetStringValueUppercase());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("@@````  ");
            Assert.AreEqual(MySQLToken.SYS_VAR, sut.GetToken());
            Assert.AreEqual("````", sut.GetStringValue());
            Assert.AreEqual("````", sut.GetStringValueUppercase());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("@@`~!````@#$%^&*()``_+=-1{}[]\";:'<>,./?|\\`  ");
            Assert.AreEqual(MySQLToken.SYS_VAR, sut.GetToken());
            Assert.AreEqual("`~!````@#$%^&*()``_+=-1{}[]\";:'<>,./?|\\`", sut.GetStringValue());
            Assert.AreEqual("`~!````@#$%^&*()``_+=-1{}[]\";:'<>,./?|\\`", sut.GetStringValueUppercase());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("@@global.var1  ");
            Assert.AreEqual(MySQLToken.SYS_VAR, sut.GetToken());
            Assert.AreEqual("global", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.PUNC_DOT, sut.GetToken());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("var1", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("@@'abc'  ");
            Assert.AreEqual(MySQLToken.SYS_VAR, sut.GetToken());
            Assert.AreEqual("", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.LITERAL_CHARS, sut.GetToken());
            Assert.AreEqual("'abc'", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("@@\"abc\"  ");
            Assert.AreEqual(MySQLToken.SYS_VAR, sut.GetToken());
            Assert.AreEqual("", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.LITERAL_CHARS, sut.GetToken());
            Assert.AreEqual("'abc'", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer(
                    "@@.  /*@@abc*/@@`abc''\"\\@@!%*&+_abcQ`//@@_1.  @@$#\n@@$var.-- @@a\t\n@@system_var:@@a`b`?");
            Assert.AreEqual(MySQLToken.SYS_VAR, sut.GetToken());
            Assert.AreEqual("", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.PUNC_DOT, sut.GetToken());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.SYS_VAR, sut.GetToken());
            Assert.AreEqual("`abc''\"\\@@!%*&+_abcQ`", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.OP_SLASH, sut.GetToken());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.OP_SLASH, sut.GetToken());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.SYS_VAR, sut.GetToken());
            Assert.AreEqual("_1", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.PUNC_DOT, sut.GetToken());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.SYS_VAR, sut.GetToken());
            Assert.AreEqual("$", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.SYS_VAR, sut.GetToken());
            Assert.AreEqual("$var", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.PUNC_DOT, sut.GetToken());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.SYS_VAR, sut.GetToken());
            Assert.AreEqual("system_var", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.PUNC_COLON, sut.GetToken());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.SYS_VAR, sut.GetToken());
            Assert.AreEqual("a", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("`b`", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.QUESTION_MARK, sut.GetToken());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());
        }

        [TestMethod]
        public void testPlaceHolder() // throws SQLSyntaxErrorException
        {
            MySQLLexer sut = new MySQLLexer(" ${abc}. ");
            Assert.AreEqual(MySQLToken.PLACE_HOLDER, sut.GetToken());
            Assert.AreEqual("abc", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.PUNC_DOT, sut.GetToken());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer(" ${abc");
            Assert.AreEqual(MySQLToken.PLACE_HOLDER, sut.GetToken());
            Assert.AreEqual("abc", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer(" ${abc}");
            Assert.AreEqual(MySQLToken.PLACE_HOLDER, sut.GetToken());
            Assert.AreEqual("abc", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer(" ${abc}abn");
            Assert.AreEqual(MySQLToken.PLACE_HOLDER, sut.GetToken());
            Assert.AreEqual("abc", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("abn", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer(" ${abc12@,,.~`*-_$}}}}");
            Assert.AreEqual(MySQLToken.PLACE_HOLDER, sut.GetToken());
            Assert.AreEqual("abc12@,,.~`*-_$", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.PUNC_RIGHT_BRACE, sut.GetToken());
            sut.nextToken();
            sut.nextToken();
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer(" #${abc\n,${abc12@,,.~`*-_$}");
            Assert.AreEqual(MySQLToken.PUNC_COMMA, sut.GetToken());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.PLACE_HOLDER, sut.GetToken());
            Assert.AreEqual("abc12@,,.~`*-_$", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("${abc(123,345)} ,");
            Assert.AreEqual(MySQLToken.PLACE_HOLDER, sut.GetToken());
            Assert.AreEqual("abc(123,345)", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.PUNC_COMMA, sut.GetToken());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());
        }

        [TestMethod]
        public void testId1() // throws SQLSyntaxErrorException
        {
            MySQLLexer sut = new MySQLLexer("id . 12e3f /***/`12\\3```-- d\n \r#\r  ##\n\t123d");
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("id", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.PUNC_DOT, sut.GetToken());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("12e3f", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("`12\\3```", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("123d", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("`ab``c`");
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("`ab``c`", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("`,\"'\\//*$#\nab``c  -`");
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("`,\"'\\//*$#\nab``c  -`", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("`ab````c```");
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("`ab````c```", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("`ab`````c``````");
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("`ab`````", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("c", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("``````", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("n123 \t b123 x123");
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("n123", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("b123", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("x123", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("n邱 硕");
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("n邱", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("硕", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("n邱硕");
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("n邱硕", sut.GetStringValue());
            sut.nextToken();

            sut = new MySQLLexer(" $abc");
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("$abc", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("$abc  ");
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("$abc", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer(" $abc  ");
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("$abc", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer(" 123d +=_&*_1a^abc-- $123");
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("123d", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.OP_PLUS, sut.GetToken());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.OP_EQUALS, sut.GetToken());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("_", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.OP_AMPERSAND, sut.GetToken());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.OP_ASTERISK, sut.GetToken());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("_1a", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.OP_CARET, sut.GetToken());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("abc", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer(" $abc  ,#$abc\n{`_``12`(123a)_abcnd; //x123");
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("$abc", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.PUNC_COMMA, sut.GetToken());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.PUNC_LEFT_BRACE, sut.GetToken());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("`_``12`", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.PUNC_LEFT_PAREN, sut.GetToken());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("123a", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.PUNC_RIGHT_PAREN, sut.GetToken());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("_abcnd", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.PUNC_SEMICOLON, sut.GetToken());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.OP_SLASH, sut.GetToken());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.OP_SLASH, sut.GetToken());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("x123", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

        }

        [TestMethod]
        public void testString() // throws SQLSyntaxErrorException
        {
            MySQLLexer sut = new MySQLLexer("''");
            Assert.AreEqual(MySQLToken.LITERAL_CHARS, sut.GetToken());
            Assert.AreEqual("''", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("'''\\''");
            Assert.AreEqual(MySQLToken.LITERAL_CHARS, sut.GetToken());
            Assert.AreEqual("'\\'\\''", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("\'\'\'\'\'\\''");
            Assert.AreEqual(MySQLToken.LITERAL_CHARS, sut.GetToken());
            Assert.AreEqual("'\\'\\'\\''", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("''''''/'abc\\''");
            Assert.AreEqual(MySQLToken.LITERAL_CHARS, sut.GetToken());
            Assert.AreEqual("'\\'\\''", sut.GetStringValue());
            sut.nextToken();
            sut.nextToken();
            Assert.AreEqual(MySQLToken.LITERAL_CHARS, sut.GetToken());
            Assert.AreEqual("'abc\\''", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("\'abc\\\'\'");
            Assert.AreEqual(MySQLToken.LITERAL_CHARS, sut.GetToken());
            Assert.AreEqual("'abc\\''", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("'\\\\\\\"\"\"'");
            Assert.AreEqual(MySQLToken.LITERAL_CHARS, sut.GetToken());
            Assert.AreEqual("'\\\\\\\"\"\"'", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("'\'\''");
            Assert.AreEqual(MySQLToken.LITERAL_CHARS, sut.GetToken());
            Assert.AreEqual("'\\''", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("''''");
            Assert.AreEqual(MySQLToken.LITERAL_CHARS, sut.GetToken());
            Assert.AreEqual("'\\''", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("'he\"\"\"llo'");
            Assert.AreEqual(MySQLToken.LITERAL_CHARS, sut.GetToken());
            Assert.AreEqual("'he\"\"\"llo'", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("'he'\''\'llo'");
            Assert.AreEqual(MySQLToken.LITERAL_CHARS, sut.GetToken());
            Assert.AreEqual("'he\\'\\'llo'", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("'\''hello'");
            Assert.AreEqual(MySQLToken.LITERAL_CHARS, sut.GetToken());
            Assert.AreEqual("'\\'hello'", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("\"abc'\\d\"\"ef\"");
            Assert.AreEqual(MySQLToken.LITERAL_CHARS, sut.GetToken());
            Assert.AreEqual("'abc\\'\\d\"ef'", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("\"abc'\\\\\"\"\"");
            Assert.AreEqual(MySQLToken.LITERAL_CHARS, sut.GetToken());
            Assert.AreEqual("'abc\\'\\\\\"'", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("\"\"");
            Assert.AreEqual(MySQLToken.LITERAL_CHARS, sut.GetToken());
            Assert.AreEqual("''", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("\"\\'\"");
            Assert.AreEqual(MySQLToken.LITERAL_CHARS, sut.GetToken());
            Assert.AreEqual("'\\''", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("\"\"\"\"");
            Assert.AreEqual(MySQLToken.LITERAL_CHARS, sut.GetToken());
            Assert.AreEqual("'\"'", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("\"abc\" '\\'s'");
            Assert.AreEqual(MySQLToken.LITERAL_CHARS, sut.GetToken());
            Assert.AreEqual("'abc'", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.LITERAL_CHARS, sut.GetToken());
            Assert.AreEqual("'\\'s'", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("\"\"\"'\"");
            Assert.AreEqual(MySQLToken.LITERAL_CHARS, sut.GetToken());
            Assert.AreEqual("'\"\\''", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("\"\\\"\"");
            Assert.AreEqual(MySQLToken.LITERAL_CHARS, sut.GetToken());
            Assert.AreEqual("'\\\"'", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("\"\\\\\"");
            Assert.AreEqual(MySQLToken.LITERAL_CHARS, sut.GetToken());
            Assert.AreEqual("'\\\\'", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("\"   hello '''/**/#\n-- \n~=+\"\"\"\"\"");
            Assert.AreEqual(MySQLToken.LITERAL_CHARS, sut.GetToken());
            Assert.AreEqual("'   hello \\'\\'\\'/**/#\n-- \n~=+\"\"'", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("\r--\t\n\"abc\"");
            Assert.AreEqual(MySQLToken.LITERAL_CHARS, sut.GetToken());
            Assert.AreEqual("'abc'", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("N'ab\\'c'");
            Assert.AreEqual(MySQLToken.LITERAL_NCHARS, sut.GetToken());
            Assert.AreEqual("'ab\\'c'", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("  \'abc\\\\\\'\' 'abc\\a\\'\''\"\"'/\"abc\\\"\".\"\"\"abc\"\"\"\"'\''\"n'ab\\'c'");
            Assert.AreEqual(MySQLToken.LITERAL_CHARS, sut.GetToken());
            Assert.AreEqual("'abc\\\\\\''", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.LITERAL_CHARS, sut.GetToken());
            Assert.AreEqual("'abc\\a\\'\\'\"\"'", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.OP_SLASH, sut.GetToken());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.LITERAL_CHARS, sut.GetToken());
            Assert.AreEqual("'abc\\\"'", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.PUNC_DOT, sut.GetToken());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.LITERAL_CHARS, sut.GetToken());
            Assert.AreEqual("'\"abc\"\"\\'\\'\\''", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.LITERAL_NCHARS, sut.GetToken());
            Assert.AreEqual("'ab\\'c'", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

        }

        [TestMethod]
        public void testHexBit() // throws SQLSyntaxErrorException
        {
            MySQLLexer sut = new MySQLLexer("0x123  ");
            Assert.AreEqual(MySQLToken.LITERAL_HEX, sut.GetToken());
            Assert.AreEqual("123", new String(sut.getSQL(), sut.getOffsetCache(), sut.getSizeCache()));
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("0x123");
            Assert.AreEqual(MySQLToken.LITERAL_HEX, sut.GetToken());
            Assert.AreEqual("123", new String(sut.getSQL(), sut.getOffsetCache(), sut.getSizeCache()));
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("0x123aDef");
            Assert.AreEqual(MySQLToken.LITERAL_HEX, sut.GetToken());
            Assert.AreEqual("123aDef", new String(sut.getSQL(), sut.getOffsetCache(), sut.getSizeCache()));
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("0x0");
            Assert.AreEqual(MySQLToken.LITERAL_HEX, sut.GetToken());
            Assert.AreEqual("0", new String(sut.getSQL(), sut.getOffsetCache(), sut.getSizeCache()));
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("0xABC");
            Assert.AreEqual(MySQLToken.LITERAL_HEX, sut.GetToken());
            Assert.AreEqual("ABC", new String(sut.getSQL(), sut.getOffsetCache(), sut.getSizeCache()));
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("0xA01aBC");
            Assert.AreEqual(MySQLToken.LITERAL_HEX, sut.GetToken());
            Assert.AreEqual("A01aBC", new String(sut.getSQL(), sut.getOffsetCache(), sut.getSizeCache()));
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("0x123re2  ");
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("0x123re2", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("x'123'e  ");
            Assert.AreEqual(MySQLToken.LITERAL_HEX, sut.GetToken());
            Assert.AreEqual("123", new String(sut.getSQL(), sut.getOffsetCache(), sut.getSizeCache()));
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("e", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("x'123'");
            Assert.AreEqual(MySQLToken.LITERAL_HEX, sut.GetToken());
            Assert.AreEqual("123", new String(sut.getSQL(), sut.getOffsetCache(), sut.getSizeCache()));
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("x'102AaeF3'");
            Assert.AreEqual(MySQLToken.LITERAL_HEX, sut.GetToken());
            Assert.AreEqual("102AaeF3", new String(sut.getSQL(), sut.getOffsetCache(), sut.getSizeCache()));
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("0b10");
            Assert.AreEqual(MySQLToken.LITERAL_BIT, sut.GetToken());
            Assert.AreEqual("10", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("0b101101");
            Assert.AreEqual(MySQLToken.LITERAL_BIT, sut.GetToken());
            Assert.AreEqual("101101", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("0b103  ");
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("0b103", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("b'10'b  ");
            Assert.AreEqual(MySQLToken.LITERAL_BIT, sut.GetToken());
            Assert.AreEqual("10", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("b", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("\r 0xabc.123;x'e'a0x1.3x'a2w'--\t0b11\n0b12*b '123' b'101'");
            Assert.AreEqual(MySQLToken.LITERAL_HEX, sut.GetToken());
            Assert.AreEqual("abc", new String(sut.getSQL(), sut.getOffsetCache(), sut.getSizeCache()));
            sut.nextToken();
            Assert.AreEqual(MySQLToken.LITERAL_NUM_MIX_DIGIT, sut.GetToken());
            Assert.AreEqual("0.123", ((decimal)sut.decimalValue()).ToString());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.PUNC_SEMICOLON, sut.GetToken());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.LITERAL_HEX, sut.GetToken());
            Assert.AreEqual("e", new String(sut.getSQL(), sut.getOffsetCache(), sut.getSizeCache()));
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("a0x1", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.PUNC_DOT, sut.GetToken());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("3x", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.LITERAL_CHARS, sut.GetToken());
            Assert.AreEqual("'a2w'", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("0b12", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.OP_ASTERISK, sut.GetToken());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("b", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.LITERAL_CHARS, sut.GetToken());
            Assert.AreEqual("'123'", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.LITERAL_BIT, sut.GetToken());
            Assert.AreEqual("101", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());
        }

        [TestMethod]
        public void testNumber() // throws SQLSyntaxErrorException
        {
            MySQLLexer sut = new MySQLLexer(
                    " . 12e3/***/.12e3#/**\n.123ee123.1--  \r\t\n.12e/*a*//* !*/.12e_a/12e-- \r\t.12e-1");
            Assert.AreEqual(MySQLToken.PUNC_DOT, sut.GetToken());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.LITERAL_NUM_MIX_DIGIT, sut.GetToken());
            Assert.AreEqual("12000", ((decimal)sut.decimalValue()).ToString());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.LITERAL_NUM_MIX_DIGIT, sut.GetToken());
            Assert.AreEqual("120", ((decimal)sut.decimalValue()).ToString());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.PUNC_DOT, sut.GetToken());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("123ee123", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.LITERAL_NUM_MIX_DIGIT, sut.GetToken());
            Assert.AreEqual("0.1", ((decimal)sut.decimalValue()).ToString());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.PUNC_DOT, sut.GetToken());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("12e", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.PUNC_DOT, sut.GetToken());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("12e_a", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.OP_SLASH, sut.GetToken());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("12e", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer(".12e-1  ");
            Assert.AreEqual("0.012", ((decimal)sut.decimalValue()).ToString());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("12e000000000000000  ");
            Assert.AreEqual("12", ((decimal)sut.decimalValue()).ToString());

            sut = new MySQLLexer(".12e-  ");
            Assert.AreEqual(MySQLToken.PUNC_DOT, sut.GetToken());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("12e", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.OP_MINUS, sut.GetToken());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer(".12e-1d  ");
            Assert.AreEqual(MySQLToken.PUNC_DOT, sut.GetToken());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("12e", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.OP_MINUS, sut.GetToken());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("1d", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("12.e+1d  ");
            Assert.AreEqual(MySQLToken.LITERAL_NUM_MIX_DIGIT, sut.GetToken());
            Assert.AreEqual("120", ((decimal)sut.decimalValue()).ToString());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("d", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("12.f ");
            Assert.AreEqual(MySQLToken.LITERAL_NUM_MIX_DIGIT, sut.GetToken());
            Assert.AreEqual("12", ((decimal)sut.decimalValue()).ToString());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("f", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer(".12f ");
            Assert.AreEqual(MySQLToken.PUNC_DOT, sut.GetToken());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("12f", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("1.2f ");
            Assert.AreEqual(MySQLToken.LITERAL_NUM_MIX_DIGIT, sut.GetToken());
            Assert.AreEqual("1.2", ((decimal)sut.decimalValue()).ToString());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("f", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            try
            {
                sut = new MySQLLexer("12.e ");
                Assert.IsFalse(true, "should not reach here");
            }
            catch (SQLSyntaxErrorException e)
            {
            }

            sut = new MySQLLexer("0e  ");
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("0e", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("12. e  ");
            Assert.AreEqual(MySQLToken.LITERAL_NUM_MIX_DIGIT, sut.GetToken());
            Assert.AreEqual("12", ((decimal)sut.decimalValue()).ToString());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("e", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("12. e+1  ");
            Assert.AreEqual(MySQLToken.LITERAL_NUM_MIX_DIGIT, sut.GetToken());
            Assert.AreEqual("12", ((decimal)sut.decimalValue()).ToString());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("e", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.OP_PLUS, sut.GetToken());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.LITERAL_NUM_PURE_DIGIT, sut.GetToken());
            Assert.AreEqual("1", sut.integerValue().ToString());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("12.e+1  ");
            Assert.AreEqual(MySQLToken.LITERAL_NUM_MIX_DIGIT, sut.GetToken());
            Assert.AreEqual("120", ((decimal)sut.decimalValue()).ToString());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("12.");
            Assert.AreEqual(MySQLToken.LITERAL_NUM_MIX_DIGIT, sut.GetToken());
            Assert.AreEqual("12", ((decimal)sut.decimalValue()).ToString());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer(".12");
            Assert.AreEqual(MySQLToken.LITERAL_NUM_MIX_DIGIT, sut.GetToken());
            Assert.AreEqual("0.12", ((decimal)sut.decimalValue()).ToString());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("12e");
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("12e", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("12ef");
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("12ef", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer(".12e");
            Assert.AreEqual(MySQLToken.PUNC_DOT, sut.GetToken());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("12e", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("1.0e0");
            Assert.AreEqual(MySQLToken.LITERAL_NUM_MIX_DIGIT, sut.GetToken());
            //INFO Assert.AreEqual("1.0", ((decimal)sut.decimalValue()).ToString());
            Assert.AreEqual("1", ((decimal)sut.decimalValue()).ToString());

            sut = new MySQLLexer("1.01e0,");
            Assert.AreEqual(MySQLToken.LITERAL_NUM_MIX_DIGIT, sut.GetToken());
            Assert.AreEqual("1.01", ((decimal)sut.decimalValue()).ToString());

            sut = new MySQLLexer(".12e-");
            Assert.AreEqual(MySQLToken.PUNC_DOT, sut.GetToken());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("12e", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.OP_MINUS, sut.GetToken());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer(".12e-d");
            Assert.AreEqual(MySQLToken.PUNC_DOT, sut.GetToken());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("12e", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.OP_MINUS, sut.GetToken());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("d", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("123E2.*");
            Assert.AreEqual(MySQLToken.LITERAL_NUM_MIX_DIGIT, sut.GetToken());
            Assert.AreEqual("12300", ((decimal)sut.decimalValue()).ToString());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.PUNC_DOT, sut.GetToken());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.OP_ASTERISK, sut.GetToken());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("1e-1  ");
            Assert.AreEqual("0.1", ((decimal)sut.decimalValue()).ToString());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer(".E5");
            Assert.AreEqual(MySQLToken.PUNC_DOT, sut.GetToken());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("E5", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("0E5d");
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("0E5d", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("0E10");
            Assert.AreEqual(MySQLToken.LITERAL_NUM_MIX_DIGIT, sut.GetToken());
            Assert.AreEqual("0", ((decimal)sut.decimalValue()).ToString());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer(".   ");
            Assert.AreEqual(MySQLToken.PUNC_DOT, sut.GetToken());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("12345678901234567890123 1234567890 1234567890123456789");
            Assert.AreEqual(MySQLToken.LITERAL_NUM_PURE_DIGIT, sut.GetToken());
            Assert.AreEqual("12345678901234567890123", sut.integerValue().ToString());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.LITERAL_NUM_PURE_DIGIT, sut.GetToken());
            Assert.AreEqual("1234567890", sut.integerValue().ToString());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.LITERAL_NUM_PURE_DIGIT, sut.GetToken());
            Assert.AreEqual("1234567890123456789", sut.integerValue().ToString());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer(".");
            Assert.AreEqual(MySQLToken.PUNC_DOT, sut.GetToken());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());
        }

        [TestMethod]
        public void testSkipSeparator() // throws SQLSyntaxErrorException
        {
            MySQLLexer sut = new MySQLLexer("  /**//***/ \t\n\r\n -- \n#\n/*/*-- \n*/");
            Assert.AreEqual(sut.EofIndex, sut.CurIndex);
            Assert.AreEqual(sut.Sql[sut.EofIndex], sut.Ch);

        }

        [TestMethod]
        public void testCStyleComment() // throws SQLSyntaxErrorException
        {
            MySQLLexer sut = new MySQLLexer("id1 /*!id2 */ id3");
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("id1", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("id2", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("id3", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("id1 /*! id2 */ id3");
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("id1", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("id2", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("id3", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            sut = new MySQLLexer("id1 /*!*/ id3");
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("id1", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("id3", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

            int version = MySQLLexer.setCStyleCommentVersion(40000);
            sut = new MySQLLexer("id1 /*!40001id2*/ id3");
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("id1", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("id3", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());
            MySQLLexer.setCStyleCommentVersion(version);

            version = MySQLLexer.setCStyleCommentVersion(40000);
            sut = new MySQLLexer("id1 /*!40000id2*/ id3");
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("id1", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("id2", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("id3", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());
            MySQLLexer.setCStyleCommentVersion(version);

            version = MySQLLexer.setCStyleCommentVersion(40000);
            sut = new MySQLLexer("id1 /*!4000id2*/ id3");
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("id1", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("4000id2", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("id3", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());
            MySQLLexer.setCStyleCommentVersion(version);

            version = MySQLLexer.setCStyleCommentVersion(40000);
            sut = new MySQLLexer("id1 /*!400001id2*/ id3");
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("id1", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("1id2", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("id3", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());
            MySQLLexer.setCStyleCommentVersion(version);

            version = MySQLLexer.setCStyleCommentVersion(40000);
            sut = new MySQLLexer("id1 /*!400011id2*/ id3");
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("id1", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("id3", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());
            MySQLLexer.setCStyleCommentVersion(version);

            version = MySQLLexer.setCStyleCommentVersion(40000);
            sut = new MySQLLexer("id1 /*!4000*/ id3");
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("id1", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.LITERAL_NUM_PURE_DIGIT, sut.GetToken());
            Assert.AreEqual("4000", sut.integerValue().ToString());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("id3", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());
            MySQLLexer.setCStyleCommentVersion(version);

            version = MySQLLexer.setCStyleCommentVersion(40000);
            sut = new MySQLLexer("id1 /*!40000*/ id3");
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("id1", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("id3", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());
            MySQLLexer.setCStyleCommentVersion(version);

            version = MySQLLexer.setCStyleCommentVersion(40000);
            sut = new MySQLLexer("id1 /*!40001*/ id3");
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("id1", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("id3", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());
            MySQLLexer.setCStyleCommentVersion(version);

            version = MySQLLexer.setCStyleCommentVersion(40000);
            sut = new MySQLLexer("id1 /*!400001*/ id3");
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("id1", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.LITERAL_NUM_PURE_DIGIT, sut.GetToken());
            Assert.AreEqual("1", sut.integerValue().ToString());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("id3", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());
            MySQLLexer.setCStyleCommentVersion(version);

            version = MySQLLexer.setCStyleCommentVersion(40000);
            sut = new MySQLLexer("id1 /*!40000 -- id2\n*/ id3");
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("id1", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("id3", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());
            MySQLLexer.setCStyleCommentVersion(version);

            version = MySQLLexer.setCStyleCommentVersion(40000);
            sut = new MySQLLexer("id1 /*!40000 /* id2*/*/ id3");
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("id1", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("id3", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());
            MySQLLexer.setCStyleCommentVersion(version);

            version = MySQLLexer.setCStyleCommentVersion(40000);
            sut = new MySQLLexer("id1 /*!40000/* id2*/*/ id3");
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("id1", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("id3", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());
            MySQLLexer.setCStyleCommentVersion(version);

            version = MySQLLexer.setCStyleCommentVersion(40000);
            sut = new MySQLLexer("id1 /*!40000id2*/* id3");
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("id1", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("id2", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.OP_ASTERISK, sut.GetToken());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("id3", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());
            MySQLLexer.setCStyleCommentVersion(version);

            version = MySQLLexer.setCStyleCommentVersion(40000);
            sut = new MySQLLexer("id1 /*!40000/*/*/id2*/ id3");
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("id1", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("id2", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("id3", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());
            MySQLLexer.setCStyleCommentVersion(version);

            version = MySQLLexer.setCStyleCommentVersion(40000);
            sut = new MySQLLexer("id1 /*!40001/*/*/id2*/ id3");
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("id1", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("id3", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());
            MySQLLexer.setCStyleCommentVersion(version);

            version = MySQLLexer.setCStyleCommentVersion(40000);
            sut = new MySQLLexer("id1 /*!40000/*/*/id2*/ id3");
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("id1", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("id2", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("id3", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());
            MySQLLexer.setCStyleCommentVersion(version);

            version = MySQLLexer.setCStyleCommentVersion(40000);
            sut = new MySQLLexer("id1 /*!40000/*/*/id2*/ /*!40000 id4*/ id3");
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("id1", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("id2", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("id4", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("id3", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());
            MySQLLexer.setCStyleCommentVersion(version);

            version = MySQLLexer.setCStyleCommentVersion(40000);
            sut = new MySQLLexer("id1 /*!40001/*/*/id2*/ /*!40000 id4*/ id3");
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("id1", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("id4", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("id3", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());
            MySQLLexer.setCStyleCommentVersion(version);

            version = MySQLLexer.setCStyleCommentVersion(40000);
            sut = new MySQLLexer("id1 /*!40000/*/*/id2*/ /*!40001 id4*/ id3");
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("id1", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("id2", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("id3", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());
            MySQLLexer.setCStyleCommentVersion(version);

            version = MySQLLexer.setCStyleCommentVersion(40000);
            sut = new MySQLLexer("id1 /*!40000/*/*/id2*//*!40001 id4*//*!40001 id5*/ id3");
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("id1", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("id2", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("id3", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());
            MySQLLexer.setCStyleCommentVersion(version);

            version = MySQLLexer.setCStyleCommentVersion(40000);
            sut = new MySQLLexer("id1 /*!40000/*/*/id2*//*!40001 id4*//*!40000id5*/ id3");
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("id1", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("id2", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("id5", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("id3", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());
            MySQLLexer.setCStyleCommentVersion(version);

        }

        [TestMethod]
        public void testLexer() // throws SQLSyntaxErrorException
        {
            MySQLLexer sut = new MySQLLexer(" @a.1_$ .1e+1a%x'a1e'*0b11a \r#\"\"\n@@`123`@@'abc'1.e-1d`/`1.1e1.1e1");
            Assert.AreEqual(MySQLToken.USR_VAR, sut.GetToken());
            Assert.AreEqual("@a.1_$", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.PUNC_DOT, sut.GetToken());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("1e", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.OP_PLUS, sut.GetToken());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("1a", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.OP_PERCENT, sut.GetToken());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.LITERAL_HEX, sut.GetToken());
            Assert.AreEqual("a1e", new String(sut.getSQL(), sut.getOffsetCache(), sut.getSizeCache()));
            sut.nextToken();
            Assert.AreEqual(MySQLToken.OP_ASTERISK, sut.GetToken());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("0b11a", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.SYS_VAR, sut.GetToken());
            Assert.AreEqual("`123`", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.SYS_VAR, sut.GetToken());
            Assert.AreEqual("", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.LITERAL_CHARS, sut.GetToken());
            Assert.AreEqual("'abc'", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.LITERAL_NUM_MIX_DIGIT, sut.GetToken());
            Assert.AreEqual("0.1", ((decimal)sut.decimalValue()).ToString());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("d", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.IDENTIFIER, sut.GetToken());
            Assert.AreEqual("`/`", sut.GetStringValue());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.LITERAL_NUM_MIX_DIGIT, sut.GetToken());
            Assert.AreEqual("11", ((decimal)sut.decimalValue()).ToString());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.LITERAL_NUM_MIX_DIGIT, sut.GetToken());
            Assert.AreEqual("1", ((decimal)sut.decimalValue()).ToString());
            sut.nextToken();
            Assert.AreEqual(MySQLToken.EOF, sut.GetToken());

        }
    }
}