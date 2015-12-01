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
using System.Threading;
using Deveel.Math;
using Tup.Cobar4Net.Parser.Util;

namespace Tup.Cobar4Net.Parser.Recognizer.Mysql.Lexer
{
    /// <summary>support MySql 5.5 token</summary>
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public class MySqlLexer
    {
        /// <summary>End of input character.</summary>
        /// <remarks>
        ///     End of input character. Used as a sentinel to denote the character one
        ///     beyond the last defined character in a source file.
        /// </remarks>
        private const byte Eoi = 0x1A;

        private static int CStyleCommentVersion = 50599;

        /// <summary>A character buffer for literals.</summary>
        protected internal static readonly ThreadLocal<char[]> sbufRef = new ThreadLocal<char[]>();

        /// <summary>
        ///     always be
        ///     <see cref="sql" />
        ///     .length - 1
        /// </summary>
        protected internal readonly int eofIndex;

        private readonly MySqlKeywords keywods = MySqlKeywords.DefaultKeywords;

        protected internal readonly char[] sql;

        /// <summary>
        ///     always be
        ///     <see cref="sql" />
        ///     [
        ///     <see cref="curIndex" />
        ///     ]
        /// </summary>
        protected internal char ch;

        /// <summary>
        ///     current index of
        ///     <see cref="sql" />
        /// </summary>
        protected internal int curIndex = -1;

        protected bool inCStyleComment;

        protected bool inCStyleCommentIgnore;

        protected int offsetCache;

        /// <summary>1 represents first parameter</summary>
        private int paramIndex;

        protected internal char[] sbuf;

        protected int sizeCache;

        private string stringValue;

        /// <summary>
        ///     make sense only for
        ///     <see cref="MySqlToken.Identifier" />
        /// </summary>
        private string stringValueUppercase;

        private MySqlToken token;

        /// <summary>keyword only</summary>
        private MySqlToken tokenCache;

        private MySqlToken tokenCache2;

        /// <exception cref="System.SqlSyntaxErrorException" />
        public MySqlLexer(char[] sql)
        {
            if ((sbuf = sbufRef.Value) == null)
            {
                sbuf = new char[1024];
                sbufRef.Value = sbuf;
            }
            if (CharTypes.IsWhitespace(sql[sql.Length - 1]))
            {
                this.sql = sql;
            }
            else
            {
                this.sql = new char[sql.Length + 1];
                Array.Copy(sql, 0, this.sql, 0, sql.Length);
            }
            eofIndex = this.sql.Length - 1;
            this.sql[eofIndex] = (char)Eoi;
            ScanChar();
            NextToken();
        }

        /// <exception cref="System.SqlSyntaxErrorException" />
        public MySqlLexer(string sql)
            : this(FromSQL2Chars(sql))
        {
        }

        public int CurrentIndex
        {
            get { return curIndex; }
        }

        public char[] Sql
        {
            get { return sql; }
        }

        public virtual int OffsetCache
        {
            get { return offsetCache; }
        }

        public virtual int SizeCache
        {
            get { return sizeCache; }
        }

        /// <value>start from 1. When there is no parameter yet, return 0.</value>
        public virtual int ParamIndex
        {
            get { return paramIndex; }
        }

        /// <returns>previous value</returns>
        public static int SetCStyleCommentVersion(int version)
        {
            var v = CStyleCommentVersion;
            CStyleCommentVersion = version;
            return v;
        }

        // /** current token, set by {@link #nextToken()} */
        // private int tokenPos = 0;
        /// <summary>
        ///     update
        ///     <see cref="stringValue" />
        ///     and
        ///     <see cref="stringValueUppercase" />
        ///     . It is possible that
        ///     <see cref="sbuf" />
        ///     be changed
        /// </summary>
        protected virtual void UpdateStringValue(char[] src, int srcOffset, int len)
        {
            // QS_TODO [performance enhance]: use String constant for special
            // identifier, so that parser can use '==' rather than 'equals'
            stringValue = new string(src, srcOffset, len);
            var end = srcOffset + len;
            var lowerCase = false;
            var srcIndex = srcOffset;
            var hash = 0;
            for (; srcIndex < end; ++srcIndex)
            {
                var c = src[srcIndex];
                if (c >= 'a' && c <= 'z')
                {
                    lowerCase = true;
                    if (srcIndex > srcOffset)
                    {
                        Array.Copy(src, srcOffset, sbuf, 0, srcIndex - srcOffset);
                    }
                    break;
                }
                hash = 31*hash + c;
            }
            if (lowerCase)
            {
                for (var destIndex = srcIndex - srcOffset; destIndex < len; ++destIndex)
                {
                    var c = src[srcIndex++];
                    hash = 31*hash + c;
                    if (c >= 'a' && c <= 'z')
                    {
                        sbuf[destIndex] = (char)(c - 32);
                        hash -= 32;
                    }
                    else
                    {
                        sbuf[destIndex] = c;
                    }
                }
                stringValueUppercase = new string(sbuf, 0, len);
            }
            else
            {
                stringValueUppercase = new string(src, srcOffset, len);
            }
        }

        private static char[] FromSQL2Chars(string sql)
        {
            if (CharTypes.IsWhitespace(sql[sql.Length - 1]))
            {
                return sql.ToCharArray();
            }
            var chars = new char[sql.Length + 1];
            //Sharpen.Runtime.GetCharsForString(sql, 0, sql.Length, chars, 0);
            Array.Copy(sql.ToCharArray(), 0, chars, 0, sql.Length);

            chars[chars.Length - 1] = ' ';
            return chars;
        }

        /// <param name="token">must be a keyword</param>
        public void AddCacheToke(MySqlToken token)
        {
            if (tokenCache != MySqlToken.None)
            {
                tokenCache2 = token;
            }
            else
            {
                tokenCache = token;
            }
        }

        public MySqlToken Token()
        {
            if (tokenCache2 != MySqlToken.None)
            {
                return tokenCache2;
            }
            if (tokenCache != MySqlToken.None)
            {
                return tokenCache;
            }
            return token;
        }

        protected char ScanChar()
        {
            return ch = sql[++curIndex];
        }

        /// <param name="skip">
        ///     if 1, then equals to
        ///     <see cref="ScanChar()" />
        /// </param>
        protected char ScanChar(int skip)
        {
            return ch = sql[curIndex += skip];
        }

        protected bool HasChars(int howMany)
        {
            return curIndex + howMany <= eofIndex;
        }

        protected internal bool Eof()
        {
            return curIndex >= eofIndex;
        }

        /// <exception cref="System.SqlSyntaxErrorException" />
        private MySqlToken NextTokenInternal()
        {
            switch (ch)
            {
                case '0':
                {
                    switch (sql[curIndex + 1])
                    {
                        case 'x':
                        {
                            ScanChar(2);
                            ScanHexaDecimal(false);
                            return token;
                        }

                        case 'b':
                        {
                            ScanChar(2);
                            ScanBitField(false);
                            return token;
                        }
                    }
                    goto case '1';
                }

                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                {
                    ScanNumber();
                    return token;
                }

                case '.':
                {
                    if (CharTypes.IsDigit(sql[curIndex + 1]))
                    {
                        ScanNumber();
                    }
                    else
                    {
                        ScanChar();
                        token = MySqlToken.PuncDot;
                    }
                    return token;
                }

                case '\'':
                case '"':
                {
                    ScanString();
                    return token;
                }

                case 'n':
                case 'N':
                {
                    if (sql[curIndex + 1] == '\'')
                    {
                        ScanChar();
                        ScanString();
                        token = MySqlToken.LiteralNchars;
                        return token;
                    }
                    ScanIdentifier();
                    return token;
                }

                case 'x':
                case 'X':
                {
                    if (sql[curIndex + 1] == '\'')
                    {
                        ScanChar(2);
                        ScanHexaDecimal(true);
                        return token;
                    }
                    ScanIdentifier();
                    return token;
                }

                case 'b':
                case 'B':
                {
                    if (sql[curIndex + 1] == '\'')
                    {
                        ScanChar(2);
                        ScanBitField(true);
                        return token;
                    }
                    ScanIdentifier();
                    return token;
                }

                case '@':
                {
                    if (sql[curIndex + 1] == '@')
                    {
                        ScanSystemVariable();
                        return token;
                    }
                    ScanUserVariable();
                    return token;
                }

                case '?':
                {
                    ScanChar();
                    token = MySqlToken.QuestionMark;
                    ++paramIndex;
                    return token;
                }

                case '(':
                {
                    ScanChar();
                    token = MySqlToken.PuncLeftParen;
                    return token;
                }

                case ')':
                {
                    ScanChar();
                    token = MySqlToken.PuncRightParen;
                    return token;
                }

                case '[':
                {
                    ScanChar();
                    token = MySqlToken.PuncLeftBracket;
                    return token;
                }

                case ']':
                {
                    ScanChar();
                    token = MySqlToken.PuncRightBracket;
                    return token;
                }

                case '{':
                {
                    ScanChar();
                    token = MySqlToken.PuncLeftBrace;
                    return token;
                }

                case '}':
                {
                    ScanChar();
                    token = MySqlToken.PuncRightBrace;
                    return token;
                }

                case ',':
                {
                    ScanChar();
                    token = MySqlToken.PuncComma;
                    return token;
                }

                case ';':
                {
                    ScanChar();
                    token = MySqlToken.PuncSemicolon;
                    return token;
                }

                case ':':
                {
                    if (sql[curIndex + 1] == '=')
                    {
                        ScanChar(2);
                        token = MySqlToken.OpAssign;
                        return token;
                    }
                    ScanChar();
                    token = MySqlToken.PuncColon;
                    return token;
                }

                case '=':
                {
                    ScanChar();
                    token = MySqlToken.OpEquals;
                    return token;
                }

                case '~':
                {
                    ScanChar();
                    token = MySqlToken.OpTilde;
                    return token;
                }

                case '*':
                {
                    if (inCStyleComment && sql[curIndex + 1] == '/')
                    {
                        inCStyleComment = false;
                        inCStyleCommentIgnore = false;
                        ScanChar(2);
                        token = MySqlToken.PuncCStyleCommentEnd;
                        return token;
                    }
                    ScanChar();
                    token = MySqlToken.OpAsterisk;
                    return token;
                }

                case '-':
                {
                    ScanChar();
                    token = MySqlToken.OpMinus;
                    return token;
                }

                case '+':
                {
                    ScanChar();
                    token = MySqlToken.OpPlus;
                    return token;
                }

                case '^':
                {
                    ScanChar();
                    token = MySqlToken.OpCaret;
                    return token;
                }

                case '/':
                {
                    ScanChar();
                    token = MySqlToken.OpSlash;
                    return token;
                }

                case '%':
                {
                    ScanChar();
                    token = MySqlToken.OpPercent;
                    return token;
                }

                case '&':
                {
                    if (sql[curIndex + 1] == '&')
                    {
                        ScanChar(2);
                        token = MySqlToken.OpLogicalAnd;
                        return token;
                    }
                    ScanChar();
                    token = MySqlToken.OpAmpersand;
                    return token;
                }

                case '|':
                {
                    if (sql[curIndex + 1] == '|')
                    {
                        ScanChar(2);
                        token = MySqlToken.OpLogicalOr;
                        return token;
                    }
                    ScanChar();
                    token = MySqlToken.OpVerticalBar;
                    return token;
                }

                case '!':
                {
                    if (sql[curIndex + 1] == '=')
                    {
                        ScanChar(2);
                        token = MySqlToken.OpNotEquals;
                        return token;
                    }
                    ScanChar();
                    token = MySqlToken.OpExclamation;
                    return token;
                }

                case '>':
                {
                    switch (sql[curIndex + 1])
                    {
                        case '=':
                        {
                            ScanChar(2);
                            token = MySqlToken.OpGreaterOrEquals;
                            return token;
                        }

                        case '>':
                        {
                            ScanChar(2);
                            token = MySqlToken.OpRightShift;
                            return token;
                        }

                        default:
                        {
                            ScanChar();
                            token = MySqlToken.OpGreaterThan;
                            return token;
                        }
                    }
                    //goto case '<';
                }

                case '<':
                {
                    switch (sql[curIndex + 1])
                    {
                        case '=':
                        {
                            if (sql[curIndex + 2] == '>')
                            {
                                ScanChar(3);
                                token = MySqlToken.OpNullSafeEquals;
                                return token;
                            }
                            ScanChar(2);
                            token = MySqlToken.OpLessOrEquals;
                            return token;
                        }

                        case '>':
                        {
                            ScanChar(2);
                            token = MySqlToken.OpLessOrGreater;
                            return token;
                        }

                        case '<':
                        {
                            ScanChar(2);
                            token = MySqlToken.OpLeftShift;
                            return token;
                        }

                        default:
                        {
                            ScanChar();
                            token = MySqlToken.OpLessThan;
                            return token;
                        }
                    }
                    //goto case '`';
                }

                case '`':
                {
                    ScanIdentifierWithAccent();
                    return token;
                }

                default:
                {
                    if (CharTypes.IsIdentifierChar(ch))
                    {
                        ScanIdentifier();
                    }
                    else
                    {
                        if (Eof())
                        {
                            token = MySqlToken.Eof;
                            curIndex = eofIndex;
                        }
                        else
                        {
                            // tokenPos = curIndex;
                            throw Err("unsupported character: " + ch);
                        }
                    }
                    return token;
                }
            }
        }

        /// <exception cref="System.SqlSyntaxErrorException" />
        public MySqlToken NextToken()
        {
            if (tokenCache2 != MySqlToken.None)
            {
                tokenCache2 = MySqlToken.None;
                return tokenCache;
            }
            if (tokenCache != MySqlToken.None)
            {
                tokenCache = MySqlToken.None;
                return token;
            }
            if (token == MySqlToken.Eof)
            {
                throw new SqlSyntaxErrorException("eof for sql is already reached, cannot get new token");
            }
            MySqlToken t;
            do
            {
                SkipSeparator();
                t = NextTokenInternal();
            } while (inCStyleComment && inCStyleCommentIgnore || MySqlToken.PuncCStyleCommentEnd
                     == t);
            return t;
        }

        /// <summary>first <code>@</code> is included</summary>
        /// <exception cref="System.SqlSyntaxErrorException" />
        protected virtual void ScanUserVariable()
        {
            if (ch != '@')
            {
                throw Err("first char must be @");
            }
            offsetCache = curIndex;
            sizeCache = 1;
            var dq = false;
            switch (ScanChar())
            {
                case '"':
                {
                    dq = true;
                    goto case '\'';
                }

                case '\'':
                {
                    for (++sizeCache;; ++sizeCache)
                    {
                        switch (ScanChar())
                        {
                            case '\\':
                            {
                                ++sizeCache;
                                ScanChar();
                                break;
                            }

                            case '"':
                            {
                                if (dq)
                                {
                                    ++sizeCache;
                                    if (ScanChar() == '"')
                                    {
                                        break;
                                    }
                                    goto loop1_break;
                                }
                                break;
                            }

                            case '\'':
                            {
                                if (!dq)
                                {
                                    ++sizeCache;
                                    if (ScanChar() == '\'')
                                    {
                                        break;
                                    }
                                    goto loop1_break;
                                }
                                break;
                            }
                        }
                        //loop1_continue:;
                    }
                    loop1_break:
                    ;
                    break;
                }

                case '`':
                {
                    for (++sizeCache;; ++sizeCache)
                    {
                        switch (ScanChar())
                        {
                            case '`':
                            {
                                ++sizeCache;
                                if (ScanChar() == '`')
                                {
                                    break;
                                }
                                goto loop1_break;
                            }
                        }
                        //loop1_continue:;
                    }
                    loop1_break:
                    ;
                    break;
                }

                default:
                {
                    for (; CharTypes.IsIdentifierChar(ch) || ch == '.'; ++sizeCache)
                    {
                        ScanChar();
                    }
                    break;
                }
            }
            stringValue = new string(sql, offsetCache, sizeCache);
            token = MySqlToken.UsrVar;
        }

        /// <summary>first <code>@@</code> is included</summary>
        /// <exception cref="System.SqlSyntaxErrorException" />
        protected virtual void ScanSystemVariable()
        {
            if (ch != '@' || sql[curIndex + 1] != '@')
            {
                throw Err("first char must be @@");
            }
            offsetCache = curIndex + 2;
            sizeCache = 0;
            ScanChar(2);
            if (ch == '`')
            {
                for (++sizeCache;; ++sizeCache)
                {
                    if (ScanChar() == '`')
                    {
                        ++sizeCache;
                        if (ScanChar() != '`')
                        {
                            break;
                        }
                    }
                }
            }
            else
            {
                for (; CharTypes.IsIdentifierChar(ch); ++sizeCache)
                {
                    ScanChar();
                }
            }
            UpdateStringValue(sql, offsetCache, sizeCache);
            token = MySqlToken.SysVar;
        }

        /// <exception cref="System.SqlSyntaxErrorException" />
        protected virtual void ScanString()
        {
            var dq = false;
            if (ch == '\'')
            {
            }
            else
            {
                if (ch == '"')
                {
                    dq = true;
                }
                else
                {
                    throw Err("first char must be \" or '");
                }
            }
            offsetCache = curIndex;
            var size = 1;
            sbuf[0] = '\'';
            if (dq)
            {
                while (true)
                {
                    switch (ScanChar())
                    {
                        case '\'':
                        {
                            PutChar('\\', size++);
                            PutChar('\'', size++);
                            break;
                        }

                        case '\\':
                        {
                            PutChar('\\', size++);
                            PutChar(ScanChar(), size++);
                            continue;
                        }

                        case '"':
                        {
                            if (sql[curIndex + 1] == '"')
                            {
                                PutChar('"', size++);
                                ScanChar();
                                continue;
                            }
                            PutChar('\'', size++);
                            ScanChar();
                            goto loop_break;
                        }

                        default:
                        {
                            if (Eof())
                            {
                                throw Err("unclosed string");
                            }
                            PutChar(ch, size++);
                            continue;
                        }
                    }
                    // loop_continue:;
                }
                loop_break:
                ;
            }
            else
            {
                while (true)
                {
                    switch (ScanChar())
                    {
                        case '\\':
                        {
                            PutChar('\\', size++);
                            PutChar(ScanChar(), size++);
                            continue;
                        }

                        case '\'':
                        {
                            if (sql[curIndex + 1] == '\'')
                            {
                                PutChar('\\', size++);
                                PutChar(ScanChar(), size++);
                                continue;
                            }
                            PutChar('\'', size++);
                            ScanChar();
                            goto loop_break;
                        }

                        default:
                        {
                            if (Eof())
                            {
                                throw Err("unclosed string");
                            }
                            PutChar(ch, size++);
                            continue;
                        }
                    }
                    //loop_continue:;
                }
                loop_break:
                ;
            }
            sizeCache = size;
            stringValue = new string(sbuf, 0, size);
            token = MySqlToken.LiteralChars;
        }

        /// <summary>Append a character to sbuf.</summary>
        protected void PutChar(char ch, int index)
        {
            if (index >= sbuf.Length)
            {
                var newsbuf = new char[sbuf.Length*2];
                Array.Copy(sbuf, 0, newsbuf, 0, sbuf.Length);
                sbuf = newsbuf;
            }
            sbuf[index] = ch;
        }

        /// <param name="quoteMode">
        ///     if false: first <code>0x</code> has been skipped; if
        ///     true: first <code>x'</code> has been skipped
        /// </param>
        /// <exception cref="System.SqlSyntaxErrorException" />
        protected virtual void ScanHexaDecimal(bool quoteMode)
        {
            offsetCache = curIndex;
            for (; CharTypes.IsHex(ch); ScanChar())
            {
            }
            sizeCache = curIndex - offsetCache;
            // if (sizeCache <= 0) {
            // throw err("expect at least one hexdigit");
            // }
            if (quoteMode)
            {
                if (ch != '\'')
                {
                    throw Err("invalid char for hex: " + ch);
                }
                ScanChar();
            }
            else
            {
                if (CharTypes.IsIdentifierChar(ch))
                {
                    ScanIdentifierFromNumber(offsetCache - 2, sizeCache + 2);
                    return;
                }
            }
            token = MySqlToken.LiteralHex;
        }

        /// <param name="quoteMode">
        ///     if false: first <code>0b</code> has been skipped; if
        ///     true: first <code>b'</code> has been skipped
        /// </param>
        /// <exception cref="System.SqlSyntaxErrorException" />
        protected virtual void ScanBitField(bool quoteMode)
        {
            offsetCache = curIndex;
            for (; ch == '0' || ch == '1'; ScanChar())
            {
            }
            sizeCache = curIndex - offsetCache;
            // if (sizeCache <= 0) {
            // throw err("expect at least one bit");
            // }
            if (quoteMode)
            {
                if (ch != '\'')
                {
                    throw Err("invalid char for bit: " + ch);
                }
                ScanChar();
            }
            else
            {
                if (CharTypes.IsIdentifierChar(ch))
                {
                    ScanIdentifierFromNumber(offsetCache - 2, sizeCache + 2);
                    return;
                }
            }
            token = MySqlToken.LiteralBit;
            stringValue = new string(sql, offsetCache, sizeCache);
        }

        /// <summary>
        ///     if first char is <code>.</code>, token may be
        ///     <see cref="MySqlToken.PuncDot" />
        ///     if invalid char is presented after <code>.</code>
        /// </summary>
        /// <exception cref="System.SqlSyntaxErrorException" />
        protected virtual void ScanNumber()
        {
            offsetCache = curIndex;
            sizeCache = 1;
            var fstDot = ch == '.';
            var dot = fstDot;
            var sign = false;
            var state = fstDot ? 1 : 0;
            for (; ScanChar() != Eoi; ++sizeCache)
            {
                switch (state)
                {
                    case 0:
                    {
                        if (CharTypes.IsDigit(ch))
                        {
                        }
                        else
                        {
                            if (ch == '.')
                            {
                                dot = true;
                                state = 1;
                            }
                            else
                            {
                                if (ch == 'e' || ch == 'E')
                                {
                                    state = 3;
                                }
                                else
                                {
                                    if (CharTypes.IsIdentifierChar(ch))
                                    {
                                        ScanIdentifierFromNumber(offsetCache, sizeCache);
                                        return;
                                    }
                                    token = MySqlToken.LiteralNumPureDigit;
                                    return;
                                }
                            }
                        }
                        break;
                    }

                    case 1:
                    {
                        if (CharTypes.IsDigit(ch))
                        {
                            state = 2;
                        }
                        else
                        {
                            if (ch == 'e' || ch == 'E')
                            {
                                state = 3;
                            }
                            else
                            {
                                if (CharTypes.IsIdentifierChar(ch) && fstDot)
                                {
                                    sizeCache = 1;
                                    ch = sql[curIndex = offsetCache + 1];
                                    token = MySqlToken.PuncDot;
                                    return;
                                }
                                token = MySqlToken.LiteralNumMixDigit;
                                return;
                            }
                        }
                        break;
                    }

                    case 2:
                    {
                        if (CharTypes.IsDigit(ch))
                        {
                        }
                        else
                        {
                            if (ch == 'e' || ch == 'E')
                            {
                                state = 3;
                            }
                            else
                            {
                                if (CharTypes.IsIdentifierChar(ch) && fstDot)
                                {
                                    sizeCache = 1;
                                    ch = sql[curIndex = offsetCache + 1];
                                    token = MySqlToken.PuncDot;
                                    return;
                                }
                                token = MySqlToken.LiteralNumMixDigit;
                                return;
                            }
                        }
                        break;
                    }

                    case 3:
                    {
                        if (CharTypes.IsDigit(ch))
                        {
                            state = 5;
                        }
                        else
                        {
                            if (ch == '+' || ch == '-')
                            {
                                sign = true;
                                state = 4;
                            }
                            else
                            {
                                if (fstDot)
                                {
                                    sizeCache = 1;
                                    ch = sql[curIndex = offsetCache + 1];
                                    token = MySqlToken.PuncDot;
                                    return;
                                }
                                if (!dot)
                                {
                                    if (CharTypes.IsIdentifierChar(ch))
                                    {
                                        ScanIdentifierFromNumber(offsetCache, sizeCache);
                                    }
                                    else
                                    {
                                        UpdateStringValue(sql, offsetCache, sizeCache);
                                        var tok = keywods.GetKeyword(stringValueUppercase);
                                        token = tok == MySqlToken.None ? MySqlToken.Identifier : tok;
                                    }
                                    return;
                                }
                                throw Err("invalid char after '.' and 'e' for as part of number: " + ch);
                            }
                        }
                        break;
                    }

                    case 4:
                    {
                        if (CharTypes.IsDigit(ch))
                        {
                            state = 5;
                            break;
                        }
                        if (fstDot)
                        {
                            sizeCache = 1;
                            ch = sql[curIndex = offsetCache + 1];
                            token = MySqlToken.PuncDot;
                        }
                        else
                        {
                            if (!dot)
                            {
                                ch = sql[--curIndex];
                                --sizeCache;
                                UpdateStringValue(sql, offsetCache, sizeCache);
                                var tok = keywods.GetKeyword(stringValueUppercase);
                                token = tok == MySqlToken.None ? MySqlToken.Identifier : tok;
                            }
                            else
                            {
                                throw Err("expect digit char after SIGN for 'e': " + ch);
                            }
                        }
                        return;
                    }

                    case 5:
                    {
                        if (CharTypes.IsDigit(ch))
                        {
                            break;
                        }
                        if (CharTypes.IsIdentifierChar(ch))
                        {
                            if (fstDot)
                            {
                                sizeCache = 1;
                                ch = sql[curIndex = offsetCache + 1];
                                token = MySqlToken.PuncDot;
                            }
                            else
                            {
                                if (!dot)
                                {
                                    if (sign)
                                    {
                                        ch = sql[curIndex = offsetCache];
                                        ScanIdentifierFromNumber(curIndex, 0);
                                    }
                                    else
                                    {
                                        ScanIdentifierFromNumber(offsetCache, sizeCache);
                                    }
                                }
                                else
                                {
                                    token = MySqlToken.LiteralNumMixDigit;
                                }
                            }
                        }
                        else
                        {
                            token = MySqlToken.LiteralNumMixDigit;
                        }
                        return;
                    }
                }
            }
            switch (state)
            {
                case 0:
                {
                    token = MySqlToken.LiteralNumPureDigit;
                    return;
                }

                case 1:
                {
                    if (fstDot)
                    {
                        token = MySqlToken.PuncDot;
                        return;
                    }
                    goto case 2;
                }

                case 2:
                case 5:
                {
                    token = MySqlToken.LiteralNumMixDigit;
                    return;
                }

                case 3:
                {
                    if (fstDot)
                    {
                        sizeCache = 1;
                        ch = sql[curIndex = offsetCache + 1];
                        token = MySqlToken.PuncDot;
                    }
                    else
                    {
                        if (!dot)
                        {
                            UpdateStringValue(sql, offsetCache, sizeCache);
                            var tok = keywods.GetKeyword(stringValueUppercase);
                            token = tok == MySqlToken.None ? MySqlToken.Identifier : tok;
                        }
                        else
                        {
                            throw Err("expect digit char after SIGN for 'e': " + ch);
                        }
                    }
                    return;
                }

                case 4:
                {
                    if (fstDot)
                    {
                        sizeCache = 1;
                        ch = sql[curIndex = offsetCache + 1];
                        token = MySqlToken.PuncDot;
                    }
                    else
                    {
                        if (!dot)
                        {
                            ch = sql[--curIndex];
                            --sizeCache;
                            UpdateStringValue(sql, offsetCache, sizeCache);
                            var tok = keywods.GetKeyword(stringValueUppercase);
                            token = tok == MySqlToken.None ? MySqlToken.Identifier : tok;
                        }
                        else
                        {
                            throw Err("expect digit char after SIGN for 'e': " + ch);
                        }
                    }
                    return;
                }
            }
        }

        /// <summary>
        ///     NOTE:
        ///     <see cref="MySqlToken.Identifier">id</see>
        ///     dosn't include <code>'.'</code>
        ///     for sake of performance issue (based on <i>shaojin.wensj</i>'s design).
        ///     However, it is not convenient for MySql compatibility. e.g.
        ///     <code>".123f"</code> will be regarded as <code>".123"</code> and
        ///     <code>"f"</code> in MySql, but in this
        ///     <see cref="MySqlLexer" />
        ///     , it will be
        ///     <code>"."</code> and <code>"123f"</code> because <code>".123f"</code> may
        ///     be part of <code>"db1.123f"</code> and <code>"123f"</code> is the table
        ///     name.
        /// </summary>
        /// <param name="initSize">how many char has already been consumed</param>
        /// <exception cref="System.SqlSyntaxErrorException" />
        private void ScanIdentifierFromNumber(int initOffset, int initSize)
        {
            offsetCache = initOffset;
            sizeCache = initSize;
            for (; CharTypes.IsIdentifierChar(ch); ++sizeCache)
            {
                ScanChar();
            }
            UpdateStringValue(sql, offsetCache, sizeCache);
            var tok = keywods.GetKeyword(stringValueUppercase);
            token = tok == MySqlToken.None ? MySqlToken.Identifier : tok;
        }

        /// <summary>id is NOT included in <code>`</code>.</summary>
        /// <exception cref="System.SqlSyntaxErrorException" />
        protected virtual void ScanIdentifier()
        {
            if (ch == '$')
            {
                if (ScanChar() == '{')
                {
                    ScanPlaceHolder();
                }
                else
                {
                    ScanIdentifierFromNumber(curIndex - 1, 1);
                }
            }
            else
            {
                ScanIdentifierFromNumber(curIndex, 0);
            }
        }

        /// <summary>not Sql syntax</summary>
        /// <exception cref="System.SqlSyntaxErrorException" />
        protected virtual void ScanPlaceHolder()
        {
            offsetCache = curIndex + 1;
            sizeCache = 0;
            for (ScanChar(); ch != '}' && !Eof(); ++sizeCache)
            {
                ScanChar();
            }
            if (ch == '}')
            {
                ScanChar();
            }
            UpdateStringValue(sql, offsetCache, sizeCache);
            token = MySqlToken.PlaceHolder;
        }

        /// <summary>id is included in <code>`</code>.</summary>
        /// <remarks>id is included in <code>`</code>. first <code>`</code> is included</remarks>
        /// <exception cref="System.SqlSyntaxErrorException" />
        protected virtual void ScanIdentifierWithAccent()
        {
            offsetCache = curIndex;
            for (; ScanChar() != Eoi;)
            {
                if (ch == '`' && ScanChar() != '`')
                {
                    break;
                }
            }
            UpdateStringValue(sql, offsetCache, sizeCache = curIndex - offsetCache);
            token = MySqlToken.Identifier;
        }

        /// <summary>skip whitespace and comment</summary>
        protected virtual void SkipSeparator()
        {
            for (; !Eof();)
            {
                for (; CharTypes.IsWhitespace(ch); ScanChar())
                {
                }
                switch (ch)
                {
                    case '#':
                    {
                        // MySql specified
                        for (; ScanChar() != '\n';)
                        {
                            if (Eof())
                            {
                                return;
                            }
                        }
                        ScanChar();
                        continue;
                    }

                    case '/':
                    {
                        if (HasChars(2) && '*' == sql[curIndex + 1])
                        {
                            bool commentSkip;
                            if ('!' == sql[curIndex + 2])
                            {
                                ScanChar(3);
                                inCStyleComment = true;
                                inCStyleCommentIgnore = false;
                                commentSkip = false;
                                // MySql use 5 digits to indicate version. 50508 means
                                // MySql 5.5.8
                                if (HasChars(5) && CharTypes.IsDigit(ch) && CharTypes.IsDigit(sql[curIndex + 1])
                                    && CharTypes.IsDigit(sql[curIndex + 2]) && CharTypes.IsDigit(sql[curIndex + 3])
                                    && CharTypes.IsDigit(sql[curIndex + 4]))
                                {
                                    var version = ch - '0';
                                    version *= 10;
                                    version += sql[curIndex + 1] - '0';
                                    version *= 10;
                                    version += sql[curIndex + 2] - '0';
                                    version *= 10;
                                    version += sql[curIndex + 3] - '0';
                                    version *= 10;
                                    version += sql[curIndex + 4] - '0';
                                    ScanChar(5);
                                    if (version > CStyleCommentVersion)
                                    {
                                        inCStyleCommentIgnore = true;
                                    }
                                }
                                SkipSeparator();
                            }
                            else
                            {
                                ScanChar(2);
                                commentSkip = true;
                            }
                            if (commentSkip)
                            {
                                for (var state = 0; !Eof(); ScanChar())
                                {
                                    if (state == 0)
                                    {
                                        if ('*' == ch)
                                        {
                                            state = 1;
                                        }
                                    }
                                    else
                                    {
                                        if ('/' == ch)
                                        {
                                            ScanChar();
                                            break;
                                        }
                                        if ('*' != ch)
                                        {
                                            state = 0;
                                        }
                                    }
                                }
                                continue;
                            }
                        }
                        return;
                    }

                    case '-':
                    {
                        if (HasChars(3) && '-' == sql[curIndex + 1] && CharTypes.IsWhitespace(sql[curIndex
                                                                                                  + 2]))
                        {
                            ScanChar(3);
                            for (; !Eof(); ScanChar())
                            {
                                if ('\n' == ch)
                                {
                                    ScanChar();
                                    break;
                                }
                            }
                            continue;
                        }
                        goto default;
                    }

                    default:
                    {
                        return;
                    }
                }
            }
        }

        /// <summary>always throw SqlSyntaxErrorException</summary>
        /// <exception cref="System.SqlSyntaxErrorException" />
        protected virtual SqlSyntaxErrorException Err(string msg)
        {
            var errMsg = msg + ". " + ToString();
            throw new SqlSyntaxErrorException(errMsg);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(GetType().Name).Append('@').Append(GetHashCode()).Append('{');
            var sqlLeft = new string(sql, curIndex, sql.Length - curIndex);
            sb.Append("curIndex=")
              .Append(curIndex)
              .Append(", ch=")
              .Append(ch)
              .Append(", token=")
              .Append(token)
              .Append(", sqlLeft=")
              .Append(sqlLeft)
              .Append(", sql=")
              .Append(sql);
            sb.Append('}');
            return sb.ToString();
        }

        /// <summary>
        ///     <see cref="token" />
        ///     must be
        ///     <see cref="MySqlToken.LiteralNumPureDigit" />
        /// </summary>
        public virtual Number GetIntegerValue()
        {
            // 2147483647
            // 9223372036854775807
            if (sizeCache < 10 ||
                sizeCache == 10 && (sql[offsetCache] < '2' || sql[offsetCache] == '2' && sql[offsetCache + 1] == '0'))
            {
                var rst = 0;
                var end = offsetCache + sizeCache;
                for (var i = offsetCache; i < end; ++i)
                {
                    rst = (rst << 3) + (rst << 1);
                    rst += sql[i] - '0';
                }
                return rst;
            }
            if (sizeCache < 19 || sizeCache == 19 && sql[offsetCache] < '9')
            {
                long rst = 0;
                var end = offsetCache + sizeCache;
                for (var i = offsetCache; i < end; ++i)
                {
                    rst = (rst << 3) + (rst << 1);
                    rst += sql[i] - '0';
                }
                return rst;
            }
            return BigInteger.Parse(new string(sql, offsetCache, sizeCache), 10);
        }

        public virtual BigDecimal GetDecimalValue()
        {
            // QS_TODO [performance enhance]: prevent BigDecimal's parser
            return BigDecimal.Parse(new string(sql, offsetCache, sizeCache));
        }

        /// <summary>
        ///     if
        ///     <see cref="GetStringValue" />
        ///     returns "'abc\\'d'", then "abc\\'d" is appended
        /// </summary>
        public virtual void AppendStringContent(StringBuilder sb)
        {
            sb.Append(sbuf, 1, sizeCache - 2);
        }

        /// <summary>
        ///     make sense for those types of token:<br />
        ///     <see cref="MySqlToken.UsrVar" />
        ///     : e.g. "@var1", "@'mary''s'";<br />
        ///     <see cref="MySqlToken.SysVar" />
        ///     : e.g. "var2";<br />
        ///     <see cref="MySqlToken.LiteralChars" />
        ///     ,
        ///     <see cref="MySqlToken.LiteralNchars" />
        ///     : e.g.
        ///     "'ab\\'c'";<br />
        ///     <see cref="MySqlToken.LiteralBit" />
        ///     : e.g. "0101" <br />
        ///     <see cref="MySqlToken.Identifier" />
        /// </summary>
        public string GetStringValue()
        {
            return stringValue;
        }

        /// <summary>
        ///     for
        ///     <see cref="MySqlToken.Identifier" />
        ///     ,
        ///     <see cref="MySqlToken.SysVar" />
        /// </summary>
        public string GetStringValueUppercase()
        {
            return stringValueUppercase;
        }
    }
}