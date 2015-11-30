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
using Tup.Cobar4Net.Parser.Ast.Expression.Primary;
using Tup.Cobar4Net.Parser.Ast.Fragment;
using Tup.Cobar4Net.Parser.Recognizer.Mysql.Lexer;

namespace Tup.Cobar4Net.Parser.Recognizer.Mysql.Syntax
{
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public abstract class MySqlParser
    {
        /// <summary>
        /// MySqlParser SpecialIdentifier
        /// </summary>
        private enum SpecialIdentifier
        {
            None = 0,

            Global,
            Local,
            Session
        }

        public const string DefaultCharset = "utf-8";

        private static readonly IDictionary<string, SpecialIdentifier> specialIdentifiers
            = new Dictionary<string, SpecialIdentifier>();

        protected readonly bool cacheEvalRst;

        protected readonly MySqlLexer lexer;

        static MySqlParser()
        {
            specialIdentifiers["GLOBAL"] = SpecialIdentifier.Global;
            specialIdentifiers["SESSION"] = SpecialIdentifier.Session;
            specialIdentifiers["LOCAL"] = SpecialIdentifier.Local;
        }

        protected MySqlParser(MySqlLexer lexer)
            : this(lexer, true)
        {
        }

        protected MySqlParser(MySqlLexer lexer, bool cacheEvalRst)
        {
            this.lexer = lexer;
            this.cacheEvalRst = cacheEvalRst;
        }

        /// <returns>
        ///     type of
        ///     <see cref="Tup.Cobar4Net.Parser.Ast.Expression.Primary.Wildcard" />
        ///     is possible. never null
        /// </returns>
        /// <exception cref="System.SqlSyntaxErrorException">
        ///     if identifier dose not matched
        /// </exception>
        protected internal virtual Identifier Identifier()
        {
            if (lexer.Token() == MySqlToken.None)
            {
                lexer.NextToken();
            }
            Identifier id;
            switch (lexer.Token())
            {
                case MySqlToken.OpAsterisk:
                {
                    lexer.NextToken();
                    var wc = new Wildcard(null);
                    wc.SetCacheEvalRst(cacheEvalRst);
                    return wc;
                }

                case MySqlToken.Identifier:
                {
                    id = new Identifier(null, lexer.GetStringValue(), lexer.GetStringValueUppercase());
                    id.SetCacheEvalRst(cacheEvalRst);
                    lexer.NextToken();
                    break;
                }

                default:
                {
                    throw Err("expect id or * after '.'");
                }
            }
            for (; lexer.Token() == MySqlToken.PuncDot;)
            {
                switch (lexer.NextToken())
                {
                    case MySqlToken.OpAsterisk:
                    {
                        lexer.NextToken();
                        var wc_1 = new Wildcard(id);
                        wc_1.SetCacheEvalRst(cacheEvalRst);
                        return wc_1;
                    }

                    case MySqlToken.Identifier:
                    {
                        id = new Identifier(id, lexer.GetStringValue(), lexer.GetStringValueUppercase());
                        id.SetCacheEvalRst(cacheEvalRst);
                        lexer.NextToken();
                        break;
                    }

                    default:
                    {
                        throw Err("expect id or * after '.'");
                    }
                }
            }
            return id;
        }

        /// <summary>
        ///     first token must be
        ///     <see cref="MySqlToken.SysVar" />
        /// </summary>
        /// <exception cref="System.SqlSyntaxErrorException" />
        public virtual SysVarPrimary SystemVariale()
        {
            SysVarPrimary sys;
            var scope = VariableScope.Session;
            var str = lexer.GetStringValue();
            var strUp = lexer.GetStringValueUppercase();
            Match(MySqlToken.SysVar);
            var si = specialIdentifiers.GetValue(strUp);
            if (si != SpecialIdentifier.None)
            {
                switch (si)
                {
                    case SpecialIdentifier.Global:
                    {
                        scope = VariableScope.Global;
                        goto case SpecialIdentifier.Session;
                    }

                    case SpecialIdentifier.Session:
                    case SpecialIdentifier.Local:
                    {
                        Match(MySqlToken.PuncDot);
                        str = lexer.GetStringValue();
                        strUp = lexer.GetStringValueUppercase();
                        Match(MySqlToken.Identifier);
                        sys = new SysVarPrimary(scope, str, strUp);
                        sys.SetCacheEvalRst(cacheEvalRst);
                        return sys;
                    }
                }
            }
            sys = new SysVarPrimary(scope, str, strUp);
            sys.SetCacheEvalRst(cacheEvalRst);
            return sys;
        }

        protected virtual ParamMarker CreateParam(int index)
        {
            var param = new ParamMarker(index);
            param.SetCacheEvalRst(cacheEvalRst);
            return param;
        }

        protected virtual PlaceHolder CreatePlaceHolder(string str, string strUp)
        {
            var ph = new PlaceHolder(str, strUp);
            ph.SetCacheEvalRst(cacheEvalRst);
            return ph;
        }

        /// <summary>nothing has been pre-consumed</summary>
        /// <returns>null if there is no order limit</returns>
        /// <exception cref="System.SqlSyntaxErrorException" />
        public virtual Limit Limit()
        {
            if (lexer.Token() != MySqlToken.KwLimit)
            {
                return null;
            }
            int paramIndex1;
            int paramIndex2;
            Number num1;
            switch (lexer.NextToken())
            {
                case MySqlToken.LiteralNumPureDigit:
                {
                    num1 = lexer.GetIntegerValue();
                    switch (lexer.NextToken())
                    {
                        case MySqlToken.PuncComma:
                        {
                            switch (lexer.NextToken())
                            {
                                case MySqlToken.LiteralNumPureDigit:
                                {
                                    var num2 = lexer.GetIntegerValue();
                                    lexer.NextToken();
                                    return new Limit(num1, num2);
                                }

                                case MySqlToken.QuestionMark:
                                {
                                    paramIndex1 = lexer.ParamIndex;
                                    lexer.NextToken();
                                    return new Limit(num1, CreateParam(paramIndex1));
                                }

                                default:
                                {
                                    throw Err("expect digit or ? after , for limit");
                                }
                            }
                            //goto case MySqlToken.Identifier;
                        }

                        case MySqlToken.Identifier:
                        {
                            if ("OFFSET".Equals(lexer.GetStringValueUppercase()))
                            {
                                switch (lexer.NextToken())
                                {
                                    case MySqlToken.LiteralNumPureDigit:
                                    {
                                        var num2_1 = lexer.GetIntegerValue();
                                        lexer.NextToken();
                                        return new Limit(num2_1, num1);
                                    }

                                    case MySqlToken.QuestionMark:
                                    {
                                        paramIndex1 = lexer.ParamIndex;
                                        lexer.NextToken();
                                        return new Limit(CreateParam(paramIndex1), num1);
                                    }

                                    default:
                                    {
                                        throw Err("expect digit or ? after , for limit");
                                    }
                                }
                            }
                            break;
                        }
                    }
                    return new Limit(0, num1);
                }

                case MySqlToken.QuestionMark:
                {
                    paramIndex1 = lexer.ParamIndex;
                    switch (lexer.NextToken())
                    {
                        case MySqlToken.PuncComma:
                        {
                            switch (lexer.NextToken())
                            {
                                case MySqlToken.LiteralNumPureDigit:
                                {
                                    num1 = lexer.GetIntegerValue();
                                    lexer.NextToken();
                                    return new Limit(CreateParam(paramIndex1), num1);
                                }

                                case MySqlToken.QuestionMark:
                                {
                                    paramIndex2 = lexer.ParamIndex;
                                    lexer.NextToken();
                                    return new Limit(CreateParam(paramIndex1), CreateParam(paramIndex2));
                                }

                                default:
                                {
                                    throw Err("expect digit or ? after , for limit");
                                }
                            }
                            //goto case MySqlToken.Identifier;
                        }

                        case MySqlToken.Identifier:
                        {
                            if ("OFFSET".Equals(lexer.GetStringValueUppercase()))
                            {
                                switch (lexer.NextToken())
                                {
                                    case MySqlToken.LiteralNumPureDigit:
                                    {
                                        num1 = lexer.GetIntegerValue();
                                        lexer.NextToken();
                                        return new Limit(num1, CreateParam(paramIndex1));
                                    }

                                    case MySqlToken.QuestionMark:
                                    {
                                        paramIndex2 = lexer.ParamIndex;
                                        lexer.NextToken();
                                        return new Limit(CreateParam(paramIndex2), CreateParam(paramIndex1));
                                    }

                                    default:
                                    {
                                        throw Err("expect digit or ? after , for limit");
                                    }
                                }
                            }
                            break;
                        }
                    }
                    return new Limit(0, CreateParam(paramIndex1));
                }

                default:
                {
                    throw Err("expect digit or ? after limit");
                }
            }
        }

        /// <param name="expectTextUppercase">must be upper-case</param>
        /// <returns>
        ///     index (start from 0) of expected text which is first matched. -1
        ///     if none is matched.
        /// </returns>
        /// <exception cref="System.SqlSyntaxErrorException" />
        protected virtual int EqualsIdentifier(params string[] expectTextUppercases)
        {
            if (lexer.Token() == MySqlToken.Identifier)
            {
                var id = lexer.GetStringValueUppercase();
                for (var i = 0; i < expectTextUppercases.Length; ++i)
                {
                    if (expectTextUppercases[i].Equals(id))
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        /// <returns>index of expected token, start from 0</returns>
        /// <exception cref="System.SqlSyntaxErrorException">if no token is matched</exception>
        protected virtual int MatchIdentifier(params string[] expectTextUppercase)
        {
            if (expectTextUppercase == null || expectTextUppercase.Length <= 0)
            {
                throw new ArgumentException("at least one expect token");
            }
            if (lexer.Token() != MySqlToken.Identifier)
            {
                throw Err("expect an id");
            }
            var id = lexer.GetStringValueUppercase();
            for (var i = 0; i < expectTextUppercase.Length; ++i)
            {
                if (id == null ? expectTextUppercase[i] == null : id.Equals(expectTextUppercase[i]))
                {
                    lexer.NextToken();
                    return i;
                }
            }
            throw Err("expect " + expectTextUppercase);
        }

        /// <returns>index of expected token, start from 0</returns>
        /// <exception cref="System.SqlSyntaxErrorException">if no token is matched</exception>
        protected internal virtual int Match(params MySqlToken[] expectToken)
        {
            if (expectToken == null || expectToken.Length <= 0)
            {
                throw new ArgumentException("at least one expect token");
            }
            var token = lexer.Token();
            for (var i = 0; i < expectToken.Length; ++i)
            {
                if (token == expectToken[i])
                {
                    if (token != MySqlToken.Eof || i < expectToken.Length - 1)
                    {
                        lexer.NextToken();
                    }
                    return i;
                }
            }
            throw Err("expect " + expectToken);
        }

        /// <summary>side-effect is forbidden</summary>
        /// <exception cref="System.SqlSyntaxErrorException" />
        protected virtual SqlSyntaxErrorException Err(string msg)
        {
            var errmsg = new StringBuilder();
            errmsg.Append(msg).Append(". lexer state: ").Append(lexer);
            throw new SqlSyntaxErrorException(errmsg.ToString());
        }
    }
}