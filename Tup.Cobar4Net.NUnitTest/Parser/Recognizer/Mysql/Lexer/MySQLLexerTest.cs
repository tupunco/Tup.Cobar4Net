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

using Sharpen;
using System;

namespace Tup.Cobar4Net.Parser.Recognizer.Mysql.Lexer
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    [NUnit.Framework.TestFixture(Category = "MySQLLexerTest")]
    public class MySQLLexerTest
    {
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        public static void Main(string[] args)
        {
            string sql = Performance.SqlBenchmarkSelect;
            char[] chars = sql.ToCharArray();
            MySQLLexer sut = new MySQLLexer(sql);
            long start = Runtime.CurrentTimeMillis();
            long end = Runtime.CurrentTimeMillis();
            for (int i = 0; i < 1; ++i)
            {
                for (; !sut.Eof();)
                {
                    sut.NextToken();
                    switch (sut.Token())
                    {
                        case MySQLToken.LiteralNumMixDigit:
                            {
                                sut.DecimalValue();
                                break;
                            }

                        case MySQLToken.LiteralNumPureDigit:
                            {
                                sut.IntegerValue();
                                break;
                            }

                        default:
                            {
                                sut.StringValue();
                                break;
                            }
                    }
                }
            }
            int loop = 5000000;
            sut = new MySQLLexer(sql);
            start = Runtime.CurrentTimeMillis();
            for (int i_1 = 0; i_1 < loop; ++i_1)
            {
                sut = new MySQLLexer(chars);
                for (; !sut.Eof();)
                {
                    sut.NextToken();
                    switch (sut.Token())
                    {
                        case MySQLToken.LiteralNumMixDigit:
                            {
                                sut.DecimalValue();
                                break;
                            }

                        case MySQLToken.LiteralNumPureDigit:
                            {
                                sut.IntegerValue();
                                break;
                            }

                        default:
                            {
                                sut.StringValue();
                                break;
                            }
                    }
                }
            }
            end = Runtime.CurrentTimeMillis();
            System.Console.Out.WriteLine((end - start) * 1.0d / (loop / 1000) + " us.");
        }
        //[NUnit.Framework.Test]
        //public virtual void TestMain()
        //{
        //    Main(null);
        //    NUnit.Framework.Assert.IsTrue(true);
        //}
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        [NUnit.Framework.Test]
        public virtual void TestParameter()
        {
            MySQLLexer sut = new MySQLLexer("?,?,?");
            NUnit.Framework.Assert.AreEqual(MySQLToken.QuestionMark, sut.Token());
            NUnit.Framework.Assert.AreEqual(1, sut.ParamIndex());
            sut.NextToken();
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.QuestionMark, sut.Token());
            NUnit.Framework.Assert.AreEqual(2, sut.ParamIndex());
            sut.NextToken();
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.QuestionMark, sut.Token());
            NUnit.Framework.Assert.AreEqual(3, sut.ParamIndex());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
        }

        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        [NUnit.Framework.Test]
        public virtual void TestUserDefVar()
        {
            MySQLLexer sut = new MySQLLexer("@abc  ");
            NUnit.Framework.Assert.AreEqual(MySQLToken.UsrVar, sut.Token());
            NUnit.Framework.Assert.AreEqual("@abc", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("@abc.d  ");
            NUnit.Framework.Assert.AreEqual(MySQLToken.UsrVar, sut.Token());
            NUnit.Framework.Assert.AreEqual("@abc.d", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("@abc_$.d");
            NUnit.Framework.Assert.AreEqual(MySQLToken.UsrVar, sut.Token());
            NUnit.Framework.Assert.AreEqual("@abc_$.d", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("@abc_$_.");
            NUnit.Framework.Assert.AreEqual(MySQLToken.UsrVar, sut.Token());
            NUnit.Framework.Assert.AreEqual("@abc_$_.", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("@''");
            NUnit.Framework.Assert.AreEqual(MySQLToken.UsrVar, sut.Token());
            NUnit.Framework.Assert.AreEqual("@''", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("@'\\''");
            NUnit.Framework.Assert.AreEqual(MySQLToken.UsrVar, sut.Token());
            NUnit.Framework.Assert.AreEqual("@'\\''", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("@''''");
            NUnit.Framework.Assert.AreEqual(MySQLToken.UsrVar, sut.Token());
            NUnit.Framework.Assert.AreEqual("@''''", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("@'\"'");
            NUnit.Framework.Assert.AreEqual(MySQLToken.UsrVar, sut.Token());
            NUnit.Framework.Assert.AreEqual("@'\"'", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("@'\"\"'");
            NUnit.Framework.Assert.AreEqual(MySQLToken.UsrVar, sut.Token());
            NUnit.Framework.Assert.AreEqual("@'\"\"'", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("@'\\\"'");
            NUnit.Framework.Assert.AreEqual(MySQLToken.UsrVar, sut.Token());
            NUnit.Framework.Assert.AreEqual("@'\\\"'", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("@'ac\\''  ");
            NUnit.Framework.Assert.AreEqual(MySQLToken.UsrVar, sut.Token());
            NUnit.Framework.Assert.AreEqual("@'ac\\''", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("@'''ac\\''  ");
            NUnit.Framework.Assert.AreEqual(MySQLToken.UsrVar, sut.Token());
            NUnit.Framework.Assert.AreEqual("@'''ac\\''", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("@'abc'''ac\\''  ");
            NUnit.Framework.Assert.AreEqual(MySQLToken.UsrVar, sut.Token());
            NUnit.Framework.Assert.AreEqual("@'abc'''", sut.StringValue());
            sut = new MySQLLexer("@''abc''");
            NUnit.Framework.Assert.AreEqual(MySQLToken.UsrVar, sut.Token());
            NUnit.Framework.Assert.AreEqual("@''", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("abc", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralChars, sut.Token());
            NUnit.Framework.Assert.AreEqual("''", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("@\"\"  ");
            NUnit.Framework.Assert.AreEqual(MySQLToken.UsrVar, sut.Token());
            NUnit.Framework.Assert.AreEqual("@\"\"", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("@\"\"\"abc\"  ");
            NUnit.Framework.Assert.AreEqual(MySQLToken.UsrVar, sut.Token());
            NUnit.Framework.Assert.AreEqual("@\"\"\"abc\"", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("@\"\\\"\"\"abc\"  ");
            NUnit.Framework.Assert.AreEqual(MySQLToken.UsrVar, sut.Token());
            NUnit.Framework.Assert.AreEqual("@\"\\\"\"\"abc\"", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("@\"\\\"\"  ");
            NUnit.Framework.Assert.AreEqual(MySQLToken.UsrVar, sut.Token());
            NUnit.Framework.Assert.AreEqual("@\"\\\"\"", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("@\"\"\"\\\"d\"  ");
            NUnit.Framework.Assert.AreEqual(MySQLToken.UsrVar, sut.Token());
            NUnit.Framework.Assert.AreEqual("@\"\"\"\\\"d\"", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("@\"'\"  ");
            NUnit.Framework.Assert.AreEqual(MySQLToken.UsrVar, sut.Token());
            NUnit.Framework.Assert.AreEqual("@\"'\"", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("@`` ");
            NUnit.Framework.Assert.AreEqual(MySQLToken.UsrVar, sut.Token());
            NUnit.Framework.Assert.AreEqual("@``", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("@````");
            NUnit.Framework.Assert.AreEqual(MySQLToken.UsrVar, sut.Token());
            NUnit.Framework.Assert.AreEqual("@````", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("@` `");
            NUnit.Framework.Assert.AreEqual(MySQLToken.UsrVar, sut.Token());
            NUnit.Framework.Assert.AreEqual("@` `", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("@`abv```");
            NUnit.Framework.Assert.AreEqual(MySQLToken.UsrVar, sut.Token());
            NUnit.Framework.Assert.AreEqual("@`abv```", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("@`````abc`");
            NUnit.Framework.Assert.AreEqual(MySQLToken.UsrVar, sut.Token());
            NUnit.Framework.Assert.AreEqual("@`````abc`", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("@`````abc```");
            NUnit.Framework.Assert.AreEqual(MySQLToken.UsrVar, sut.Token());
            NUnit.Framework.Assert.AreEqual("@`````abc```", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("@``abc");
            NUnit.Framework.Assert.AreEqual(MySQLToken.UsrVar, sut.Token());
            NUnit.Framework.Assert.AreEqual("@``", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("abc", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("@`abc`````abc```");
            NUnit.Framework.Assert.AreEqual(MySQLToken.UsrVar, sut.Token());
            NUnit.Framework.Assert.AreEqual("@`abc`````", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("abc", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("```", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer(" -- \n  @  #abc\n\r\t\"\"@\"abc\\\\''-- abc\n'''\\\"\"\"\""
                );
            NUnit.Framework.Assert.AreEqual(MySQLToken.UsrVar, sut.Token());
            NUnit.Framework.Assert.AreEqual("@", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralChars, sut.Token());
            NUnit.Framework.Assert.AreEqual("''", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.UsrVar, sut.Token());
            NUnit.Framework.Assert.AreEqual("@\"abc\\\\''-- abc\n'''\\\"\"\"\"", sut.StringValue
                ());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("/**/@a #@abc\n@.\r\t");
            NUnit.Framework.Assert.AreEqual(MySQLToken.UsrVar, sut.Token());
            NUnit.Framework.Assert.AreEqual("@a", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.UsrVar, sut.Token());
            NUnit.Framework.Assert.AreEqual("@.", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("  #@abc\n@\"1a_-@#!''`=\\a\"-- @\r\n@'-_1a/**/\\\"\\''/*@abc*/@`_1@\\''\"`"
                );
            NUnit.Framework.Assert.AreEqual(MySQLToken.UsrVar, sut.Token());
            NUnit.Framework.Assert.AreEqual("@\"1a_-@#!''`=\\a\"", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.UsrVar, sut.Token());
            NUnit.Framework.Assert.AreEqual("@'-_1a/**/\\\"\\''", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.UsrVar, sut.Token());
            NUnit.Framework.Assert.AreEqual("@`_1@\\''\"`", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("  /*! */@._a$ @_a.b$c.\r@1_a.$#\n@A.a_/@-- \n@_--@.[]'\"@#abc'@a,@;@~#@abc"
                );
            NUnit.Framework.Assert.AreEqual(MySQLToken.UsrVar, sut.Token());
            NUnit.Framework.Assert.AreEqual("@._a$", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.UsrVar, sut.Token());
            NUnit.Framework.Assert.AreEqual("@_a.b$c.", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.UsrVar, sut.Token());
            NUnit.Framework.Assert.AreEqual("@1_a.$", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.UsrVar, sut.Token());
            NUnit.Framework.Assert.AreEqual("@A.a_", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.OpSlash, sut.Token());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.UsrVar, sut.Token());
            NUnit.Framework.Assert.AreEqual("@", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.UsrVar, sut.Token());
            NUnit.Framework.Assert.AreEqual("@_", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.OpMinus, sut.Token());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.OpMinus, sut.Token());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.UsrVar, sut.Token());
            NUnit.Framework.Assert.AreEqual("@.", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.PuncLeftBracket, sut.Token());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.PuncRightBracket, sut.Token());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralChars, sut.Token());
            NUnit.Framework.Assert.AreEqual("'\"@#abc'", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.UsrVar, sut.Token());
            NUnit.Framework.Assert.AreEqual("@a", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.PuncComma, sut.Token());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.UsrVar, sut.Token());
            NUnit.Framework.Assert.AreEqual("@", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.PuncSemicolon, sut.Token());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.UsrVar, sut.Token());
            NUnit.Framework.Assert.AreEqual("@", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.OpTilde, sut.Token());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
        }

        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        [NUnit.Framework.Test]
        public virtual void TestSystemVar()
        {
            MySQLLexer sut = new MySQLLexer("@@abc  ");
            NUnit.Framework.Assert.AreEqual(MySQLToken.SysVar, sut.Token());
            NUnit.Framework.Assert.AreEqual("abc", sut.StringValue());
            NUnit.Framework.Assert.AreEqual("ABC", sut.StringValueUppercase());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("@@`abc`  ");
            NUnit.Framework.Assert.AreEqual(MySQLToken.SysVar, sut.Token());
            NUnit.Framework.Assert.AreEqual("`abc`", sut.StringValue());
            NUnit.Framework.Assert.AreEqual("`ABC`", sut.StringValueUppercase());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("@@```abc`  ");
            NUnit.Framework.Assert.AreEqual(MySQLToken.SysVar, sut.Token());
            NUnit.Framework.Assert.AreEqual("```abc`", sut.StringValue());
            NUnit.Framework.Assert.AreEqual("```ABC`", sut.StringValueUppercase());
            sut = new MySQLLexer("@@``  ");
            NUnit.Framework.Assert.AreEqual(MySQLToken.SysVar, sut.Token());
            NUnit.Framework.Assert.AreEqual("``", sut.StringValue());
            NUnit.Framework.Assert.AreEqual("``", sut.StringValueUppercase());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("@@`a```  ");
            NUnit.Framework.Assert.AreEqual(MySQLToken.SysVar, sut.Token());
            NUnit.Framework.Assert.AreEqual("`a```", sut.StringValue());
            NUnit.Framework.Assert.AreEqual("`A```", sut.StringValueUppercase());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("@@````  ");
            NUnit.Framework.Assert.AreEqual(MySQLToken.SysVar, sut.Token());
            NUnit.Framework.Assert.AreEqual("````", sut.StringValue());
            NUnit.Framework.Assert.AreEqual("````", sut.StringValueUppercase());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("@@`~!````@#$%^&*()``_+=-1{}[]\";:'<>,./?|\\`  ");
            NUnit.Framework.Assert.AreEqual(MySQLToken.SysVar, sut.Token());
            NUnit.Framework.Assert.AreEqual("`~!````@#$%^&*()``_+=-1{}[]\";:'<>,./?|\\`", sut
                .StringValue());
            NUnit.Framework.Assert.AreEqual("`~!````@#$%^&*()``_+=-1{}[]\";:'<>,./?|\\`", sut
                .StringValueUppercase());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("@@global.var1  ");
            NUnit.Framework.Assert.AreEqual(MySQLToken.SysVar, sut.Token());
            NUnit.Framework.Assert.AreEqual("global", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.PuncDot, sut.Token());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("var1", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("@@'abc'  ");
            NUnit.Framework.Assert.AreEqual(MySQLToken.SysVar, sut.Token());
            NUnit.Framework.Assert.AreEqual(string.Empty, sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralChars, sut.Token());
            NUnit.Framework.Assert.AreEqual("'abc'", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("@@\"abc\"  ");
            NUnit.Framework.Assert.AreEqual(MySQLToken.SysVar, sut.Token());
            NUnit.Framework.Assert.AreEqual(string.Empty, sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralChars, sut.Token());
            NUnit.Framework.Assert.AreEqual("'abc'", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("@@.  /*@@abc*/@@`abc''\"\\@@!%*&+_abcQ`//@@_1.  @@$#\n@@$var.-- @@a\t\n@@system_var:@@a`b`?"
                );
            NUnit.Framework.Assert.AreEqual(MySQLToken.SysVar, sut.Token());
            NUnit.Framework.Assert.AreEqual(string.Empty, sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.PuncDot, sut.Token());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.SysVar, sut.Token());
            NUnit.Framework.Assert.AreEqual("`abc''\"\\@@!%*&+_abcQ`", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.OpSlash, sut.Token());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.OpSlash, sut.Token());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.SysVar, sut.Token());
            NUnit.Framework.Assert.AreEqual("_1", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.PuncDot, sut.Token());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.SysVar, sut.Token());
            NUnit.Framework.Assert.AreEqual("$", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.SysVar, sut.Token());
            NUnit.Framework.Assert.AreEqual("$var", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.PuncDot, sut.Token());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.SysVar, sut.Token());
            NUnit.Framework.Assert.AreEqual("system_var", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.PuncColon, sut.Token());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.SysVar, sut.Token());
            NUnit.Framework.Assert.AreEqual("a", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("`b`", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.QuestionMark, sut.Token());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
        }

        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        [NUnit.Framework.Test]
        public virtual void TestPlaceHolder()
        {
            MySQLLexer sut = new MySQLLexer(" ${abc}. ");
            NUnit.Framework.Assert.AreEqual(MySQLToken.PlaceHolder, sut.Token());
            NUnit.Framework.Assert.AreEqual("abc", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.PuncDot, sut.Token());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer(" ${abc");
            NUnit.Framework.Assert.AreEqual(MySQLToken.PlaceHolder, sut.Token());
            NUnit.Framework.Assert.AreEqual("abc", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer(" ${abc}");
            NUnit.Framework.Assert.AreEqual(MySQLToken.PlaceHolder, sut.Token());
            NUnit.Framework.Assert.AreEqual("abc", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer(" ${abc}abn");
            NUnit.Framework.Assert.AreEqual(MySQLToken.PlaceHolder, sut.Token());
            NUnit.Framework.Assert.AreEqual("abc", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("abn", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer(" ${abc12@,,.~`*-_$}}}}");
            NUnit.Framework.Assert.AreEqual(MySQLToken.PlaceHolder, sut.Token());
            NUnit.Framework.Assert.AreEqual("abc12@,,.~`*-_$", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.PuncRightBrace, sut.Token());
            sut.NextToken();
            sut.NextToken();
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer(" #${abc\n,${abc12@,,.~`*-_$}");
            NUnit.Framework.Assert.AreEqual(MySQLToken.PuncComma, sut.Token());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.PlaceHolder, sut.Token());
            NUnit.Framework.Assert.AreEqual("abc12@,,.~`*-_$", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("${abc(123,345)} ,");
            NUnit.Framework.Assert.AreEqual(MySQLToken.PlaceHolder, sut.Token());
            NUnit.Framework.Assert.AreEqual("abc(123,345)", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.PuncComma, sut.Token());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
        }

        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        [NUnit.Framework.Test]
        public virtual void TestId1()
        {
            MySQLLexer sut = new MySQLLexer("id . 12e3f /***/`12\\3```-- d\n \r#\r  ##\n\t123d"
                );
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("id", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.PuncDot, sut.Token());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("12e3f", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("`12\\3```", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("123d", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("`ab``c`");
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("`ab``c`", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("`,\"'\\//*$#\nab``c  -`");
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("`,\"'\\//*$#\nab``c  -`", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("`ab````c```");
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("`ab````c```", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("`ab`````c``````");
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("`ab`````", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("c", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("``````", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("n123 \t b123 x123");
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("n123", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("b123", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("x123", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("n邱 硕");
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("n邱", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("硕", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("n邱硕");
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("n邱硕", sut.StringValue());
            sut.NextToken();
            sut = new MySQLLexer(" $abc");
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("$abc", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("$abc  ");
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("$abc", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer(" $abc  ");
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("$abc", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer(" 123d +=_&*_1a^abc-- $123");
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("123d", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.OpPlus, sut.Token());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.OpEquals, sut.Token());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("_", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.OpAmpersand, sut.Token());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.OpAsterisk, sut.Token());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("_1a", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.OpCaret, sut.Token());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("abc", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer(" $abc  ,#$abc\n{`_``12`(123a)_abcnd; //x123");
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("$abc", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.PuncComma, sut.Token());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.PuncLeftBrace, sut.Token());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("`_``12`", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.PuncLeftParen, sut.Token());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("123a", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.PuncRightParen, sut.Token());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("_abcnd", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.PuncSemicolon, sut.Token());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.OpSlash, sut.Token());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.OpSlash, sut.Token());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("x123", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
        }

        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        [NUnit.Framework.Test]
        public virtual void TestString()
        {
            MySQLLexer sut = new MySQLLexer("''");
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralChars, sut.Token());
            NUnit.Framework.Assert.AreEqual("''", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("'''\\''");
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralChars, sut.Token());
            NUnit.Framework.Assert.AreEqual("'\\'\\''", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("\'\'\'\'\'\\''");
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralChars, sut.Token());
            NUnit.Framework.Assert.AreEqual("'\\'\\'\\''", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("''''''/'abc\\''");
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralChars, sut.Token());
            NUnit.Framework.Assert.AreEqual("'\\'\\''", sut.StringValue());
            sut.NextToken();
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralChars, sut.Token());
            NUnit.Framework.Assert.AreEqual("'abc\\''", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("\'abc\\\'\'");
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralChars, sut.Token());
            NUnit.Framework.Assert.AreEqual("'abc\\''", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("'\\\\\\\"\"\"'");
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralChars, sut.Token());
            NUnit.Framework.Assert.AreEqual("'\\\\\\\"\"\"'", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("'\'\''");
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralChars, sut.Token());
            NUnit.Framework.Assert.AreEqual("'\\''", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("''''");
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralChars, sut.Token());
            NUnit.Framework.Assert.AreEqual("'\\''", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("'he\"\"\"llo'");
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralChars, sut.Token());
            NUnit.Framework.Assert.AreEqual("'he\"\"\"llo'", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("'he'\''\'llo'");
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralChars, sut.Token());
            NUnit.Framework.Assert.AreEqual("'he\\'\\'llo'", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("'\''hello'");
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralChars, sut.Token());
            NUnit.Framework.Assert.AreEqual("'\\'hello'", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("\"abc'\\d\"\"ef\"");
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralChars, sut.Token());
            NUnit.Framework.Assert.AreEqual("'abc\\'\\d\"ef'", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("\"abc'\\\\\"\"\"");
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralChars, sut.Token());
            NUnit.Framework.Assert.AreEqual("'abc\\'\\\\\"'", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("\"\"");
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralChars, sut.Token());
            NUnit.Framework.Assert.AreEqual("''", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("\"\\'\"");
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralChars, sut.Token());
            NUnit.Framework.Assert.AreEqual("'\\''", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("\"\"\"\"");
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralChars, sut.Token());
            NUnit.Framework.Assert.AreEqual("'\"'", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("\"abc\" '\\'s'");
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralChars, sut.Token());
            NUnit.Framework.Assert.AreEqual("'abc'", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralChars, sut.Token());
            NUnit.Framework.Assert.AreEqual("'\\'s'", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("\"\"\"'\"");
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralChars, sut.Token());
            NUnit.Framework.Assert.AreEqual("'\"\\''", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("\"\\\"\"");
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralChars, sut.Token());
            NUnit.Framework.Assert.AreEqual("'\\\"'", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("\"\\\\\"");
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralChars, sut.Token());
            NUnit.Framework.Assert.AreEqual("'\\\\'", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("\"   hello '''/**/#\n-- \n~=+\"\"\"\"\"");
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralChars, sut.Token());
            NUnit.Framework.Assert.AreEqual("'   hello \\'\\'\\'/**/#\n-- \n~=+\"\"'", sut.StringValue
                ());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("\r--\t\n\"abc\"");
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralChars, sut.Token());
            NUnit.Framework.Assert.AreEqual("'abc'", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("N'ab\\'c'");
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralNchars, sut.Token());
            NUnit.Framework.Assert.AreEqual("'ab\\'c'", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("  \'abc\\\\\\'\' 'abc\\a\\'\''\"\"'/\"abc\\\"\".\"\"\"abc\"\"\"\"'\''\"n'ab\\'c'"
                );
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralChars, sut.Token());
            NUnit.Framework.Assert.AreEqual("'abc\\\\\\''", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralChars, sut.Token());
            NUnit.Framework.Assert.AreEqual("'abc\\a\\'\\'\"\"'", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.OpSlash, sut.Token());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralChars, sut.Token());
            NUnit.Framework.Assert.AreEqual("'abc\\\"'", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.PuncDot, sut.Token());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralChars, sut.Token());
            NUnit.Framework.Assert.AreEqual("'\"abc\"\"\\'\\'\\''", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralNchars, sut.Token());
            NUnit.Framework.Assert.AreEqual("'ab\\'c'", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
        }

        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        [NUnit.Framework.Test]
        public virtual void TestHexBit()
        {
            MySQLLexer sut = new MySQLLexer("0x123  ");
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralHex, sut.Token());
            NUnit.Framework.Assert.AreEqual("123", new string(sut.GetSQL(), sut.GetOffsetCache
                (), sut.GetSizeCache()));
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("0x123");
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralHex, sut.Token());
            NUnit.Framework.Assert.AreEqual("123", new string(sut.GetSQL(), sut.GetOffsetCache
                (), sut.GetSizeCache()));
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("0x123aDef");
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralHex, sut.Token());
            NUnit.Framework.Assert.AreEqual("123aDef", new string(sut.GetSQL(), sut.GetOffsetCache
                (), sut.GetSizeCache()));
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("0x0");
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralHex, sut.Token());
            NUnit.Framework.Assert.AreEqual("0", new string(sut.GetSQL(), sut.GetOffsetCache(
                ), sut.GetSizeCache()));
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("0xABC");
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralHex, sut.Token());
            NUnit.Framework.Assert.AreEqual("ABC", new string(sut.GetSQL(), sut.GetOffsetCache
                (), sut.GetSizeCache()));
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("0xA01aBC");
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralHex, sut.Token());
            NUnit.Framework.Assert.AreEqual("A01aBC", new string(sut.GetSQL(), sut.GetOffsetCache
                (), sut.GetSizeCache()));
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("0x123re2  ");
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("0x123re2", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("x'123'e  ");
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralHex, sut.Token());
            NUnit.Framework.Assert.AreEqual("123", new string(sut.GetSQL(), sut.GetOffsetCache
                (), sut.GetSizeCache()));
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("e", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("x'123'");
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralHex, sut.Token());
            NUnit.Framework.Assert.AreEqual("123", new string(sut.GetSQL(), sut.GetOffsetCache
                (), sut.GetSizeCache()));
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("x'102AaeF3'");
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralHex, sut.Token());
            NUnit.Framework.Assert.AreEqual("102AaeF3", new string(sut.GetSQL(), sut.GetOffsetCache
                (), sut.GetSizeCache()));
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("0b10");
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralBit, sut.Token());
            NUnit.Framework.Assert.AreEqual("10", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("0b101101");
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralBit, sut.Token());
            NUnit.Framework.Assert.AreEqual("101101", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("0b103  ");
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("0b103", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("b'10'b  ");
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralBit, sut.Token());
            NUnit.Framework.Assert.AreEqual("10", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("b", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("\r 0xabc.123;x'e'a0x1.3x'a2w'--\t0b11\n0b12*b '123' b'101'"
                );
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralHex, sut.Token());
            NUnit.Framework.Assert.AreEqual("abc", new string(sut.GetSQL(), sut.GetOffsetCache
                (), sut.GetSizeCache()));
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralNumMixDigit, sut.Token());
            NUnit.Framework.Assert.AreEqual("0.123", sut.DecimalValue().ToPlainString());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.PuncSemicolon, sut.Token());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralHex, sut.Token());
            NUnit.Framework.Assert.AreEqual("e", new string(sut.GetSQL(), sut.GetOffsetCache(
                ), sut.GetSizeCache()));
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("a0x1", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.PuncDot, sut.Token());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("3x", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralChars, sut.Token());
            NUnit.Framework.Assert.AreEqual("'a2w'", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("0b12", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.OpAsterisk, sut.Token());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("b", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralChars, sut.Token());
            NUnit.Framework.Assert.AreEqual("'123'", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralBit, sut.Token());
            NUnit.Framework.Assert.AreEqual("101", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
        }

        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        [NUnit.Framework.Test]
        public virtual void TestNumber()
        {
            MySQLLexer sut = new MySQLLexer(" . 12e3/***/.12e3#/**\n.123ee123.1--  \r\t\n.12e/*a*//* !*/.12e_a/12e-- \r\t.12e-1"
                );
            NUnit.Framework.Assert.AreEqual(MySQLToken.PuncDot, sut.Token());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralNumMixDigit, sut.Token());
            NUnit.Framework.Assert.AreEqual("12000", sut.DecimalValue().ToPlainString());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralNumMixDigit, sut.Token());
            NUnit.Framework.Assert.AreEqual("120", sut.DecimalValue().ToPlainString());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.PuncDot, sut.Token());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("123ee123", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralNumMixDigit, sut.Token());
            NUnit.Framework.Assert.AreEqual("0.1", sut.DecimalValue().ToPlainString());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.PuncDot, sut.Token());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("12e", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.PuncDot, sut.Token());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("12e_a", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.OpSlash, sut.Token());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("12e", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer(".12e-1  ");
            NUnit.Framework.Assert.AreEqual("0.012", sut.DecimalValue().ToPlainString());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("12e000000000000000  ");
            NUnit.Framework.Assert.AreEqual("12", sut.DecimalValue().ToPlainString());
            sut = new MySQLLexer(".12e-  ");
            NUnit.Framework.Assert.AreEqual(MySQLToken.PuncDot, sut.Token());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("12e", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.OpMinus, sut.Token());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer(".12e-1d  ");
            NUnit.Framework.Assert.AreEqual(MySQLToken.PuncDot, sut.Token());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("12e", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.OpMinus, sut.Token());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("1d", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("12.e+1d  ");
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralNumMixDigit, sut.Token());
            NUnit.Framework.Assert.AreEqual("120", sut.DecimalValue().ToPlainString());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("d", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("12.f ");
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralNumMixDigit, sut.Token());
            NUnit.Framework.Assert.AreEqual("12", sut.DecimalValue().ToPlainString());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("f", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer(".12f ");
            NUnit.Framework.Assert.AreEqual(MySQLToken.PuncDot, sut.Token());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("12f", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("1.2f ");
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralNumMixDigit, sut.Token());
            NUnit.Framework.Assert.AreEqual("1.2", sut.DecimalValue().ToPlainString());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("f", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            try
            {
                sut = new MySQLLexer("12.e ");
                NUnit.Framework.Assert.IsFalse(true, "should not reach here");
            }
            catch (SQLSyntaxErrorException)
            {
            }
            sut = new MySQLLexer("0e  ");
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("0e", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("12. e  ");
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralNumMixDigit, sut.Token());
            NUnit.Framework.Assert.AreEqual("12", sut.DecimalValue().ToPlainString());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("e", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("12. e+1  ");
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralNumMixDigit, sut.Token());
            NUnit.Framework.Assert.AreEqual("12", sut.DecimalValue().ToPlainString());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("e", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.OpPlus, sut.Token());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralNumPureDigit, sut.Token());
            NUnit.Framework.Assert.AreEqual("1", sut.IntegerValue().ToString());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("12.e+1  ");
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralNumMixDigit, sut.Token());
            NUnit.Framework.Assert.AreEqual("120", sut.DecimalValue().ToPlainString());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("12.");
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralNumMixDigit, sut.Token());
            NUnit.Framework.Assert.AreEqual("12", sut.DecimalValue().ToPlainString());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer(".12");
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralNumMixDigit, sut.Token());
            NUnit.Framework.Assert.AreEqual("0.12", sut.DecimalValue().ToPlainString());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("12e");
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("12e", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("12ef");
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("12ef", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer(".12e");
            NUnit.Framework.Assert.AreEqual(MySQLToken.PuncDot, sut.Token());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("12e", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("1.0e0");
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralNumMixDigit, sut.Token());
            NUnit.Framework.Assert.AreEqual("1.0", sut.DecimalValue().ToPlainString());
            sut = new MySQLLexer("1.01e0,");
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralNumMixDigit, sut.Token());
            NUnit.Framework.Assert.AreEqual("1.01", sut.DecimalValue().ToPlainString());
            sut = new MySQLLexer(".12e-");
            NUnit.Framework.Assert.AreEqual(MySQLToken.PuncDot, sut.Token());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("12e", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.OpMinus, sut.Token());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer(".12e-d");
            NUnit.Framework.Assert.AreEqual(MySQLToken.PuncDot, sut.Token());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("12e", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.OpMinus, sut.Token());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("d", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("123E2.*");
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralNumMixDigit, sut.Token());
            NUnit.Framework.Assert.AreEqual("12300", sut.DecimalValue().ToPlainString());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.PuncDot, sut.Token());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.OpAsterisk, sut.Token());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("1e-1  ");
            NUnit.Framework.Assert.AreEqual("0.1", sut.DecimalValue().ToPlainString());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer(".E5");
            NUnit.Framework.Assert.AreEqual(MySQLToken.PuncDot, sut.Token());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("E5", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("0E5d");
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("0E5d", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("0E10");
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralNumMixDigit, sut.Token());
            NUnit.Framework.Assert.AreEqual("0", sut.DecimalValue().ToPlainString());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer(".   ");
            NUnit.Framework.Assert.AreEqual(MySQLToken.PuncDot, sut.Token());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("12345678901234567890123 1234567890 1234567890123456789");
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralNumPureDigit, sut.Token());
            NUnit.Framework.Assert.AreEqual("12345678901234567890123", sut.IntegerValue().ToString());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralNumPureDigit, sut.Token());
            NUnit.Framework.Assert.AreEqual("1234567890", sut.IntegerValue().ToString());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralNumPureDigit, sut.Token());
            NUnit.Framework.Assert.AreEqual("1234567890123456789", sut.IntegerValue().ToString());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer(".");
            NUnit.Framework.Assert.AreEqual(MySQLToken.PuncDot, sut.Token());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
        }

        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        [NUnit.Framework.Test]
        public virtual void TestSkipSeparator()
        {
            MySQLLexer sut = new MySQLLexer("  /**//***/ \t\n\r\n -- \n#\n/*/*-- \n*/");
            NUnit.Framework.Assert.AreEqual(sut.eofIndex, sut.curIndex);
            NUnit.Framework.Assert.AreEqual(sut.sql[sut.eofIndex], sut.ch);
        }

        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        [NUnit.Framework.Test]
        public virtual void TestCStyleComment()
        {
            MySQLLexer sut = new MySQLLexer("id1 /*!id2 */ id3");
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("id1", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("id2", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("id3", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("id1 /*! id2 */ id3");
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("id1", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("id2", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("id3", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            sut = new MySQLLexer("id1 /*!*/ id3");
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("id1", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("id3", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            int version = MySQLLexer.SetCStyleCommentVersion(40000);
            sut = new MySQLLexer("id1 /*!40001id2*/ id3");
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("id1", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("id3", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            MySQLLexer.SetCStyleCommentVersion(version);
            version = MySQLLexer.SetCStyleCommentVersion(40000);
            sut = new MySQLLexer("id1 /*!40000id2*/ id3");
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("id1", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("id2", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("id3", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            MySQLLexer.SetCStyleCommentVersion(version);
            version = MySQLLexer.SetCStyleCommentVersion(40000);
            sut = new MySQLLexer("id1 /*!4000id2*/ id3");
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("id1", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("4000id2", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("id3", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            MySQLLexer.SetCStyleCommentVersion(version);
            version = MySQLLexer.SetCStyleCommentVersion(40000);
            sut = new MySQLLexer("id1 /*!400001id2*/ id3");
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("id1", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("1id2", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("id3", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            MySQLLexer.SetCStyleCommentVersion(version);
            version = MySQLLexer.SetCStyleCommentVersion(40000);
            sut = new MySQLLexer("id1 /*!400011id2*/ id3");
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("id1", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("id3", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            MySQLLexer.SetCStyleCommentVersion(version);
            version = MySQLLexer.SetCStyleCommentVersion(40000);
            sut = new MySQLLexer("id1 /*!4000*/ id3");
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("id1", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralNumPureDigit, sut.Token());
            NUnit.Framework.Assert.AreEqual("4000", sut.IntegerValue().ToString());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("id3", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            MySQLLexer.SetCStyleCommentVersion(version);
            version = MySQLLexer.SetCStyleCommentVersion(40000);
            sut = new MySQLLexer("id1 /*!40000*/ id3");
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("id1", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("id3", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            MySQLLexer.SetCStyleCommentVersion(version);
            version = MySQLLexer.SetCStyleCommentVersion(40000);
            sut = new MySQLLexer("id1 /*!40001*/ id3");
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("id1", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("id3", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            MySQLLexer.SetCStyleCommentVersion(version);
            version = MySQLLexer.SetCStyleCommentVersion(40000);
            sut = new MySQLLexer("id1 /*!400001*/ id3");
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("id1", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralNumPureDigit, sut.Token());
            NUnit.Framework.Assert.AreEqual("1", sut.IntegerValue().ToString());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("id3", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            MySQLLexer.SetCStyleCommentVersion(version);
            version = MySQLLexer.SetCStyleCommentVersion(40000);
            sut = new MySQLLexer("id1 /*!40000 -- id2\n*/ id3");
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("id1", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("id3", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            MySQLLexer.SetCStyleCommentVersion(version);
            version = MySQLLexer.SetCStyleCommentVersion(40000);
            sut = new MySQLLexer("id1 /*!40000 /* id2*/*/ id3");
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("id1", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("id3", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            MySQLLexer.SetCStyleCommentVersion(version);
            version = MySQLLexer.SetCStyleCommentVersion(40000);
            sut = new MySQLLexer("id1 /*!40000/* id2*/*/ id3");
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("id1", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("id3", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            MySQLLexer.SetCStyleCommentVersion(version);
            version = MySQLLexer.SetCStyleCommentVersion(40000);
            sut = new MySQLLexer("id1 /*!40000id2*/* id3");
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("id1", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("id2", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.OpAsterisk, sut.Token());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("id3", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            MySQLLexer.SetCStyleCommentVersion(version);
            version = MySQLLexer.SetCStyleCommentVersion(40000);
            sut = new MySQLLexer("id1 /*!40000/*/*/id2*/ id3");
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("id1", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("id2", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("id3", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            MySQLLexer.SetCStyleCommentVersion(version);
            version = MySQLLexer.SetCStyleCommentVersion(40000);
            sut = new MySQLLexer("id1 /*!40001/*/*/id2*/ id3");
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("id1", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("id3", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            MySQLLexer.SetCStyleCommentVersion(version);
            version = MySQLLexer.SetCStyleCommentVersion(40000);
            sut = new MySQLLexer("id1 /*!40000/*/*/id2*/ id3");
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("id1", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("id2", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("id3", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            MySQLLexer.SetCStyleCommentVersion(version);
            version = MySQLLexer.SetCStyleCommentVersion(40000);
            sut = new MySQLLexer("id1 /*!40000/*/*/id2*/ /*!40000 id4*/ id3");
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("id1", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("id2", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("id4", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("id3", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            MySQLLexer.SetCStyleCommentVersion(version);
            version = MySQLLexer.SetCStyleCommentVersion(40000);
            sut = new MySQLLexer("id1 /*!40001/*/*/id2*/ /*!40000 id4*/ id3");
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("id1", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("id4", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("id3", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            MySQLLexer.SetCStyleCommentVersion(version);
            version = MySQLLexer.SetCStyleCommentVersion(40000);
            sut = new MySQLLexer("id1 /*!40000/*/*/id2*/ /*!40001 id4*/ id3");
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("id1", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("id2", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("id3", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            MySQLLexer.SetCStyleCommentVersion(version);
            version = MySQLLexer.SetCStyleCommentVersion(40000);
            sut = new MySQLLexer("id1 /*!40000/*/*/id2*//*!40001 id4*//*!40001 id5*/ id3");
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("id1", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("id2", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("id3", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            MySQLLexer.SetCStyleCommentVersion(version);
            version = MySQLLexer.SetCStyleCommentVersion(40000);
            sut = new MySQLLexer("id1 /*!40000/*/*/id2*//*!40001 id4*//*!40000id5*/ id3");
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("id1", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("id2", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("id5", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("id3", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
            MySQLLexer.SetCStyleCommentVersion(version);
        }

        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        [NUnit.Framework.Test]
        public virtual void TestLexer()
        {
            MySQLLexer sut = new MySQLLexer(" @a.1_$ .1e+1a%x'a1e'*0b11a \r#\"\"\n@@`123`@@'abc'1.e-1d`/`1.1e1.1e1"
                );
            NUnit.Framework.Assert.AreEqual(MySQLToken.UsrVar, sut.Token());
            NUnit.Framework.Assert.AreEqual("@a.1_$", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.PuncDot, sut.Token());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("1e", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.OpPlus, sut.Token());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("1a", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.OpPercent, sut.Token());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralHex, sut.Token());
            NUnit.Framework.Assert.AreEqual("a1e", new string(sut.GetSQL(), sut.GetOffsetCache
                (), sut.GetSizeCache()));
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.OpAsterisk, sut.Token());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("0b11a", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.SysVar, sut.Token());
            NUnit.Framework.Assert.AreEqual("`123`", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.SysVar, sut.Token());
            NUnit.Framework.Assert.AreEqual(string.Empty, sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralChars, sut.Token());
            NUnit.Framework.Assert.AreEqual("'abc'", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralNumMixDigit, sut.Token());
            NUnit.Framework.Assert.AreEqual("0.1", sut.DecimalValue().ToPlainString());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("d", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Identifier, sut.Token());
            NUnit.Framework.Assert.AreEqual("`/`", sut.StringValue());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralNumMixDigit, sut.Token());
            NUnit.Framework.Assert.AreEqual("11", sut.DecimalValue().ToPlainString());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.LiteralNumMixDigit, sut.Token());
            NUnit.Framework.Assert.AreEqual("1", sut.DecimalValue().ToPlainString());
            sut.NextToken();
            NUnit.Framework.Assert.AreEqual(MySQLToken.Eof, sut.Token());
        }
    }
}