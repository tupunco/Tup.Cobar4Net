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
using System.Collections.Generic;
using System.Text;

using Tup.Cobar.Parser.Ast.Expression.Primary;
using Tup.Cobar.Parser.Ast.Fragment;
using Tup.Cobar.Parser.Recognizer.Mysql.Lexer;

namespace Tup.Cobar.Parser.Recognizer.Mysql.Syntax
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public abstract class MySQLParser
    {
        public const string DefaultCharset = "utf-8";

        protected internal readonly MySQLLexer lexer;

        public MySQLParser(MySQLLexer lexer)
            : this(lexer, true)
        {
        }

        public MySQLParser(MySQLLexer lexer, bool cacheEvalRst)
        {
            this.lexer = lexer;
            this.cacheEvalRst = cacheEvalRst;
        }

        private enum SpecialIdentifier
        {
            None = 0,
            Global,
            Local,
            Session
        }

        private static readonly IDictionary<string, SpecialIdentifier> specialIdentifiers
             = new Dictionary<string, SpecialIdentifier>();

        static MySQLParser()
        {
            specialIdentifiers["GLOBAL"] = SpecialIdentifier.Global;
            specialIdentifiers["SESSION"] = SpecialIdentifier.Session;
            specialIdentifiers["LOCAL"] = SpecialIdentifier.Local;
        }

        protected internal readonly bool cacheEvalRst;

        /// <returns>
        /// type of
        /// <see cref="Tup.Cobar.Parser.Ast.Expression.Primary.Wildcard"/>
        /// is possible. never null
        /// </returns>
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException">if identifier dose not matched
        /// 	</exception>
        public virtual Identifier Identifier()
        {
            if (lexer.Token() == MySQLToken.None)
            {
                lexer.NextToken();
            }
            Identifier id;
            switch (lexer.Token())
            {
                case MySQLToken.OpAsterisk:
                    {
                        lexer.NextToken();
                        Wildcard wc = new Wildcard(null);
                        wc.SetCacheEvalRst(cacheEvalRst);
                        return wc;
                    }

                case MySQLToken.Identifier:
                    {
                        id = new Identifier(null, lexer.StringValue(), lexer.StringValueUppercase());
                        id.SetCacheEvalRst(cacheEvalRst);
                        lexer.NextToken();
                        break;
                    }

                default:
                    {
                        throw Err("expect id or * after '.'");
                    }
            }
            for (; lexer.Token() == MySQLToken.PuncDot;)
            {
                switch (lexer.NextToken())
                {
                    case MySQLToken.OpAsterisk:
                        {
                            lexer.NextToken();
                            Wildcard wc_1 = new Wildcard(id);
                            wc_1.SetCacheEvalRst(cacheEvalRst);
                            return wc_1;
                        }

                    case MySQLToken.Identifier:
                        {
                            id = new Identifier(id, lexer.StringValue(), lexer.StringValueUppercase());
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
        /// first token must be
        /// <see cref="Tup.Cobar.Parser.Recognizer.Mysql.MySQLToken.SysVar"/>
        /// </summary>
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        public virtual SysVarPrimary SystemVariale()
        {
            SysVarPrimary sys;
            VariableScope scope = VariableScope.Session;
            string str = lexer.StringValue();
            string strUp = lexer.StringValueUppercase();
            Match(MySQLToken.SysVar);
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
                            Match(MySQLToken.PuncDot);
                            str = lexer.StringValue();
                            strUp = lexer.StringValueUppercase();
                            Match(MySQLToken.Identifier);
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
            ParamMarker param = new ParamMarker(index);
            param.SetCacheEvalRst(cacheEvalRst);
            return param;
        }

        protected virtual PlaceHolder CreatePlaceHolder(string str, string strUp)
        {
            PlaceHolder ph = new PlaceHolder(str, strUp);
            ph.SetCacheEvalRst(cacheEvalRst);
            return ph;
        }

        /// <summary>nothing has been pre-consumed</summary>
        /// <returns>null if there is no order limit</returns>
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        protected virtual Limit Limit()
        {
            if (lexer.Token() != MySQLToken.KwLimit)
            {
                return null;
            }
            int paramIndex1;
            int paramIndex2;
            Number num1;
            switch (lexer.NextToken())
            {
                case MySQLToken.LiteralNumPureDigit:
                    {
                        num1 = lexer.IntegerValue();
                        switch (lexer.NextToken())
                        {
                            case MySQLToken.PuncComma:
                                {
                                    switch (lexer.NextToken())
                                    {
                                        case MySQLToken.LiteralNumPureDigit:
                                            {
                                                Number num2 = lexer.IntegerValue();
                                                lexer.NextToken();
                                                return new Limit(num1, num2);
                                            }

                                        case MySQLToken.QuestionMark:
                                            {
                                                paramIndex1 = lexer.ParamIndex();
                                                lexer.NextToken();
                                                return new Limit(num1, CreateParam(paramIndex1));
                                            }

                                        default:
                                            {
                                                throw Err("expect digit or ? after , for limit");
                                            }
                                    }
                                    //goto case MySQLToken.Identifier;
                                }

                            case MySQLToken.Identifier:
                                {
                                    if ("OFFSET".Equals(lexer.StringValueUppercase()))
                                    {
                                        switch (lexer.NextToken())
                                        {
                                            case MySQLToken.LiteralNumPureDigit:
                                                {
                                                    Number num2_1 = lexer.IntegerValue();
                                                    lexer.NextToken();
                                                    return new Limit(num2_1, num1);
                                                }

                                            case MySQLToken.QuestionMark:
                                                {
                                                    paramIndex1 = lexer.ParamIndex();
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

                case MySQLToken.QuestionMark:
                    {
                        paramIndex1 = lexer.ParamIndex();
                        switch (lexer.NextToken())
                        {
                            case MySQLToken.PuncComma:
                                {
                                    switch (lexer.NextToken())
                                    {
                                        case MySQLToken.LiteralNumPureDigit:
                                            {
                                                num1 = lexer.IntegerValue();
                                                lexer.NextToken();
                                                return new Limit(CreateParam(paramIndex1), num1);
                                            }

                                        case MySQLToken.QuestionMark:
                                            {
                                                paramIndex2 = lexer.ParamIndex();
                                                lexer.NextToken();
                                                return new Limit(CreateParam(paramIndex1), CreateParam
                                                    (paramIndex2));
                                            }

                                        default:
                                            {
                                                throw Err("expect digit or ? after , for limit");
                                            }
                                    }
                                    //goto case MySQLToken.Identifier;
                                }

                            case MySQLToken.Identifier:
                                {
                                    if ("OFFSET".Equals(lexer.StringValueUppercase()))
                                    {
                                        switch (lexer.NextToken())
                                        {
                                            case MySQLToken.LiteralNumPureDigit:
                                                {
                                                    num1 = lexer.IntegerValue();
                                                    lexer.NextToken();
                                                    return new Limit(num1, CreateParam(paramIndex1));
                                                }

                                            case MySQLToken.QuestionMark:
                                                {
                                                    paramIndex2 = lexer.ParamIndex();
                                                    lexer.NextToken();
                                                    return new Limit(CreateParam(paramIndex2), CreateParam
                                                        (paramIndex1));
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
        /// index (start from 0) of expected text which is first matched. -1
        /// if none is matched.
        /// </returns>
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        protected internal virtual int EqualsIdentifier(params string[] expectTextUppercases)
        {
            if (lexer.Token() == MySQLToken.Identifier)
            {
                string id = lexer.StringValueUppercase();
                for (int i = 0; i < expectTextUppercases.Length; ++i)
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
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException">if no token is matched</exception>
        protected virtual int MatchIdentifier(params string[] expectTextUppercase)
        {
            if (expectTextUppercase == null || expectTextUppercase.Length <= 0)
            {
                throw new ArgumentException("at least one expect token");
            }
            if (lexer.Token() != MySQLToken.Identifier)
            {
                throw Err("expect an id");
            }
            string id = lexer.StringValueUppercase();
            for (int i = 0; i < expectTextUppercase.Length; ++i)
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
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException">if no token is matched</exception>
        protected virtual int Match(params MySQLToken[] expectToken)
        {
            if (expectToken == null || expectToken.Length <= 0)
            {
                throw new ArgumentException("at least one expect token");
            }
            MySQLToken token = lexer.Token();
            for (int i = 0; i < expectToken.Length; ++i)
            {
                if (token == expectToken[i])
                {
                    if (token != MySQLToken.Eof || i < expectToken.Length - 1)
                    {
                        lexer.NextToken();
                    }
                    return i;
                }
            }
            throw Err("expect " + expectToken);
        }

        /// <summary>side-effect is forbidden</summary>
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        protected virtual SQLSyntaxErrorException Err(string msg)
        {
            StringBuilder errmsg = new StringBuilder();
            errmsg.Append(msg).Append(". lexer state: ").Append(lexer.ToString());
            throw new SQLSyntaxErrorException(errmsg.ToString());
        }
    }
}