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

using Deveel.Math;
using System;
using System.Text;
using System.Threading;

using Tup.Cobar.Parser.Util;

namespace Tup.Cobar.Parser.Recognizer.Mysql.Lexer
{
    /// <summary>support MySQL 5.5 token</summary>
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class MySQLLexer
    {
        private static int CStyleCommentVersion = 50599;

        /// <returns>previous value</returns>
        public static int SetCStyleCommentVersion(int version)
        {
            int v = CStyleCommentVersion;
            CStyleCommentVersion = version;
            return v;
        }

        /// <summary>End of input character.</summary>
        /// <remarks>
        /// End of input character. Used as a sentinel to denote the character one
        /// beyond the last defined character in a source file.
        /// </remarks>
        private const byte Eoi = unchecked((int)(0x1A));

        protected readonly char[] sql;
        /// <summary>
        /// Sql
        /// </summary>
        public char[] Sql
        {
            get { return sql; }
        }

        /// <summary>
        /// always be
        /// <see cref="sql"/>
        /// .length - 1
        /// </summary>
        protected readonly int eofIndex;
        /// <summary>
        /// always be
        /// <see cref="sql"/>
        /// .length - 1
        /// </summary>
        public int EofIndex
        {
            get { return eofIndex; }
        }

        /// <summary>
        /// current index of
        /// <see cref="sql"/>
        ///
        /// </summary>
        protected int curIndex = -1;
        /// <summary>
        /// current index of
        /// <see cref="sql"/>
        ///
        /// </summary>
        public int CurIndex
        {
            get { return curIndex; }
        }

        /// <summary>
        /// always be
        /// <see cref="sql"/>
        /// [
        /// <see cref="curIndex"/>
        /// ]
        /// </summary>
        protected char ch;
        /// <summary>
        /// always be
        /// <see cref="sql"/>
        /// [
        /// <see cref="curIndex"/>
        /// ]
        /// </summary>
        public char Ch
        {
            get { return ch; }
        }

        private MySQLToken token;

        /// <summary>keyword only</summary>
        private MySQLToken tokenCache;

        private MySQLToken tokenCache2;

        /// <summary>1 represents first parameter</summary>
        private int paramIndex = 0;

        /// <summary>A character buffer for literals.</summary>
        protected static readonly ThreadLocal<char[]> sbufRef = new ThreadLocal<char[]>();

        protected char[] sbuf;

        private string stringValue;

        /// <summary>
        /// make sense only for
        /// <see cref="Tup.Cobar.Parser.Recognizer.Mysql.MySQLToken.Identifier"/>
        ///
        /// </summary>
        private string stringValueUppercase;

        // /** current token, set by {@link #nextToken()} */
        // private int tokenPos = 0;
        /// <summary>
        /// update
        /// <see cref="stringValue"/>
        /// and
        /// <see cref="stringValueUppercase"/>
        /// . It is possible that
        /// <see cref="sbuf"/>
        /// be changed
        /// </summary>
        protected virtual void UpdateStringValue(char[] src, int srcOffset, int
            len)
        {
            // QS_TODO [performance enhance]: use String constant for special
            // identifier, so that parser can use '==' rather than 'equals'
            stringValue = new string(src, srcOffset, len);
            int end = srcOffset + len;
            bool lowerCase = false;
            int srcIndex = srcOffset;
            int hash = 0;
            for (; srcIndex < end; ++srcIndex)
            {
                char c = src[srcIndex];
                if (c >= 'a' && c <= 'z')
                {
                    lowerCase = true;
                    if (srcIndex > srcOffset)
                    {
                        System.Array.Copy(src, srcOffset, sbuf, 0, srcIndex - srcOffset);
                    }
                    break;
                }
                hash = 31 * hash + c;
            }
            if (lowerCase)
            {
                for (int destIndex = srcIndex - srcOffset; destIndex < len; ++destIndex)
                {
                    char c = src[srcIndex++];
                    hash = 31 * hash + c;
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

        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        public MySQLLexer(char[] sql)
        {
            if ((this.sbuf = sbufRef.Value) == null)
            {
                this.sbuf = new char[1024];
                sbufRef.Value = this.sbuf;
            }
            if (CharTypes.IsWhitespace(sql[sql.Length - 1]))
            {
                this.sql = sql;
            }
            else
            {
                this.sql = new char[sql.Length + 1];
                System.Array.Copy(sql, 0, this.sql, 0, sql.Length);
            }
            this.eofIndex = this.sql.Length - 1;
            this.sql[this.eofIndex] = (char)MySQLLexer.Eoi;
            ScanChar();
            NextToken();
        }

        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        public MySQLLexer(string sql)
            : this(FromSQL2Chars(sql))
        {
        }

        private static char[] FromSQL2Chars(string sql)
        {
            if (CharTypes.IsWhitespace(sql[sql.Length - 1]))
            {
                return sql.ToCharArray();
            }
            char[] chars = new char[sql.Length + 1];
            //Sharpen.Runtime.GetCharsForString(sql, 0, sql.Length, chars, 0);
            SystemUtils.arraycopy(sql.ToCharArray(), 0, chars, 0, sql.Length);

            chars[chars.Length - 1] = ' ';
            return chars;
        }

        private MySQLKeywords keywods = MySQLKeywords.DefaultKeywords;

        /// <param name="token">must be a keyword</param>
        public void AddCacheToke(MySQLToken token)
        {
            if (tokenCache != MySQLToken.None)
            {
                tokenCache2 = token;
            }
            else
            {
                tokenCache = token;
            }
        }

        public MySQLToken Token()
        {
            if (tokenCache2 != MySQLToken.None)
            {
                return tokenCache2;
            }
            if (tokenCache != MySQLToken.None)
            {
                return tokenCache;
            }
            return token;
        }

        public int GetCurrentIndex()
        {
            return this.curIndex;
        }

        public char[] GetSQL()
        {
            return sql;
        }

        public virtual int GetOffsetCache()
        {
            return offsetCache;
        }

        public virtual int GetSizeCache()
        {
            return sizeCache;
        }

        /// <returns>start from 1. When there is no parameter yet, return 0.</returns>
        public virtual int ParamIndex()
        {
            return paramIndex;
        }

        protected char ScanChar()
        {
            return ch = sql[++curIndex];
        }

        /// <param name="skip">
        /// if 1, then equals to
        /// <see cref="ScanChar()"/>
        /// </param>
        protected char ScanChar(int skip)
        {
            return ch = sql[curIndex += skip];
        }

        protected bool HasChars(int howMany)
        {
            return curIndex + howMany <= eofIndex;
        }

        public bool Eof()
        {
            return curIndex >= eofIndex;
        }

        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        private MySQLToken NextTokenInternal()
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
                            token = MySQLToken.PuncDot;
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
                            token = MySQLToken.LiteralNchars;
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
                        token = MySQLToken.QuestionMark;
                        ++paramIndex;
                        return token;
                    }

                case '(':
                    {
                        ScanChar();
                        token = MySQLToken.PuncLeftParen;
                        return token;
                    }

                case ')':
                    {
                        ScanChar();
                        token = MySQLToken.PuncRightParen;
                        return token;
                    }

                case '[':
                    {
                        ScanChar();
                        token = MySQLToken.PuncLeftBracket;
                        return token;
                    }

                case ']':
                    {
                        ScanChar();
                        token = MySQLToken.PuncRightBracket;
                        return token;
                    }

                case '{':
                    {
                        ScanChar();
                        token = MySQLToken.PuncLeftBrace;
                        return token;
                    }

                case '}':
                    {
                        ScanChar();
                        token = MySQLToken.PuncRightBrace;
                        return token;
                    }

                case ',':
                    {
                        ScanChar();
                        token = MySQLToken.PuncComma;
                        return token;
                    }

                case ';':
                    {
                        ScanChar();
                        token = MySQLToken.PuncSemicolon;
                        return token;
                    }

                case ':':
                    {
                        if (sql[curIndex + 1] == '=')
                        {
                            ScanChar(2);
                            token = MySQLToken.OpAssign;
                            return token;
                        }
                        ScanChar();
                        token = MySQLToken.PuncColon;
                        return token;
                    }

                case '=':
                    {
                        ScanChar();
                        token = MySQLToken.OpEquals;
                        return token;
                    }

                case '~':
                    {
                        ScanChar();
                        token = MySQLToken.OpTilde;
                        return token;
                    }

                case '*':
                    {
                        if (inCStyleComment && sql[curIndex + 1] == '/')
                        {
                            inCStyleComment = false;
                            inCStyleCommentIgnore = false;
                            ScanChar(2);
                            token = MySQLToken.PuncCStyleCommentEnd;
                            return token;
                        }
                        ScanChar();
                        token = MySQLToken.OpAsterisk;
                        return token;
                    }

                case '-':
                    {
                        ScanChar();
                        token = MySQLToken.OpMinus;
                        return token;
                    }

                case '+':
                    {
                        ScanChar();
                        token = MySQLToken.OpPlus;
                        return token;
                    }

                case '^':
                    {
                        ScanChar();
                        token = MySQLToken.OpCaret;
                        return token;
                    }

                case '/':
                    {
                        ScanChar();
                        token = MySQLToken.OpSlash;
                        return token;
                    }

                case '%':
                    {
                        ScanChar();
                        token = MySQLToken.OpPercent;
                        return token;
                    }

                case '&':
                    {
                        if (sql[curIndex + 1] == '&')
                        {
                            ScanChar(2);
                            token = MySQLToken.OpLogicalAnd;
                            return token;
                        }
                        ScanChar();
                        token = MySQLToken.OpAmpersand;
                        return token;
                    }

                case '|':
                    {
                        if (sql[curIndex + 1] == '|')
                        {
                            ScanChar(2);
                            token = MySQLToken.OpLogicalOr;
                            return token;
                        }
                        ScanChar();
                        token = MySQLToken.OpVerticalBar;
                        return token;
                    }

                case '!':
                    {
                        if (sql[curIndex + 1] == '=')
                        {
                            ScanChar(2);
                            token = MySQLToken.OpNotEquals;
                            return token;
                        }
                        ScanChar();
                        token = MySQLToken.OpExclamation;
                        return token;
                    }

                case '>':
                    {
                        switch (sql[curIndex + 1])
                        {
                            case '=':
                                {
                                    ScanChar(2);
                                    token = MySQLToken.OpGreaterOrEquals;
                                    return token;
                                }

                            case '>':
                                {
                                    ScanChar(2);
                                    token = MySQLToken.OpRightShift;
                                    return token;
                                }

                            default:
                                {
                                    ScanChar();
                                    token = MySQLToken.OpGreaterThan;
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
                                        token = MySQLToken.OpNullSafeEquals;
                                        return token;
                                    }
                                    ScanChar(2);
                                    token = MySQLToken.OpLessOrEquals;
                                    return token;
                                }

                            case '>':
                                {
                                    ScanChar(2);
                                    token = MySQLToken.OpLessOrGreater;
                                    return token;
                                }

                            case '<':
                                {
                                    ScanChar(2);
                                    token = MySQLToken.OpLeftShift;
                                    return token;
                                }

                            default:
                                {
                                    ScanChar();
                                    token = MySQLToken.OpLessThan;
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
                                token = MySQLToken.Eof;
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

        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        public MySQLToken NextToken()
        {
            if (tokenCache2 != MySQLToken.None)
            {
                tokenCache2 = MySQLToken.None;
                return tokenCache;
            }
            if (tokenCache != MySQLToken.None)
            {
                tokenCache = MySQLToken.None;
                return token;
            }
            if (token == MySQLToken.Eof)
            {
                throw new SQLSyntaxErrorException("eof for sql is already reached, cannot get new token"
                    );
            }
            MySQLToken t;
            do
            {
                SkipSeparator();
                t = NextTokenInternal();
            }
            while (inCStyleComment && inCStyleCommentIgnore || MySQLToken.PuncCStyleCommentEnd
                 == t);
            return t;
        }

        protected bool inCStyleComment;

        protected bool inCStyleCommentIgnore;

        protected int offsetCache;

        protected int sizeCache;

        /// <summary>first <code>@</code> is included</summary>
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        protected virtual void ScanUserVariable()
        {
            if (ch != '@')
            {
                throw Err("first char must be @");
            }
            offsetCache = curIndex;
            sizeCache = 1;
            bool dq = false;
            switch (ScanChar())
            {
                case '"':
                    {
                        dq = true;
                        goto case '\'';
                    }

                case '\'':
                    {
                        for (++sizeCache; ; ++sizeCache)
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
                    loop1_break:;
                        break;
                    }

                case '`':
                    {
                        for (++sizeCache; ; ++sizeCache)
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
                    loop1_break:;
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
            token = MySQLToken.UsrVar;
        }

        /// <summary>first <code>@@</code> is included</summary>
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
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
                for (++sizeCache; ; ++sizeCache)
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
            token = MySQLToken.SysVar;
        }

        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        protected virtual void ScanString()
        {
            bool dq = false;
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
            int size = 1;
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
            loop_break:;
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
            loop_break:;
            }
            sizeCache = size;
            stringValue = new string(sbuf, 0, size);
            token = MySQLToken.LiteralChars;
        }

        /// <summary>Append a character to sbuf.</summary>
        protected void PutChar(char ch, int index)
        {
            if (index >= sbuf.Length)
            {
                char[] newsbuf = new char[sbuf.Length * 2];
                System.Array.Copy(sbuf, 0, newsbuf, 0, sbuf.Length);
                sbuf = newsbuf;
            }
            sbuf[index] = ch;
        }

        /// <param name="quoteMode">
        /// if false: first <code>0x</code> has been skipped; if
        /// true: first <code>x'</code> has been skipped
        /// </param>
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
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
            token = MySQLToken.LiteralHex;
        }

        /// <param name="quoteMode">
        /// if false: first <code>0b</code> has been skipped; if
        /// true: first <code>b'</code> has been skipped
        /// </param>
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
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
            token = MySQLToken.LiteralBit;
            stringValue = new string(sql, offsetCache, sizeCache);
        }

        /// <summary>
        /// if first char is <code>.</code>, token may be
        /// <see cref="Tup.Cobar.Parser.Recognizer.Mysql.MySQLToken.PuncDot"/>
        /// if invalid char is presented after <code>.</code>
        /// </summary>
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        protected virtual void ScanNumber()
        {
            offsetCache = curIndex;
            sizeCache = 1;
            bool fstDot = ch == '.';
            bool dot = fstDot;
            bool sign = false;
            int state = fstDot ? 1 : 0;
            for (; ScanChar() != Tup.Cobar.Parser.Recognizer.Mysql.Lexer.MySQLLexer.Eoi; ++sizeCache)
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
                                        else
                                        {
                                            token = MySQLToken.LiteralNumPureDigit;
                                            return;
                                        }
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
                                        token = MySQLToken.PuncDot;
                                        return;
                                    }
                                    else
                                    {
                                        token = MySQLToken.LiteralNumMixDigit;
                                        return;
                                    }
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
                                        token = MySQLToken.PuncDot;
                                        return;
                                    }
                                    else
                                    {
                                        token = MySQLToken.LiteralNumMixDigit;
                                        return;
                                    }
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
                                        token = MySQLToken.PuncDot;
                                        return;
                                    }
                                    else
                                    {
                                        if (!dot)
                                        {
                                            if (CharTypes.IsIdentifierChar(ch))
                                            {
                                                ScanIdentifierFromNumber(offsetCache, sizeCache);
                                            }
                                            else
                                            {
                                                UpdateStringValue(sql, offsetCache, sizeCache);
                                                MySQLToken tok = keywods.GetKeyword(stringValueUppercase);
                                                token = tok == MySQLToken.None ? MySQLToken.Identifier : tok;
                                            }
                                            return;
                                        }
                                        else
                                        {
                                            throw Err("invalid char after '.' and 'e' for as part of number: " + ch);
                                        }
                                    }
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
                            else
                            {
                                if (fstDot)
                                {
                                    sizeCache = 1;
                                    ch = sql[curIndex = offsetCache + 1];
                                    token = MySQLToken.PuncDot;
                                }
                                else
                                {
                                    if (!dot)
                                    {
                                        ch = sql[--curIndex];
                                        --sizeCache;
                                        UpdateStringValue(sql, offsetCache, sizeCache);
                                        MySQLToken tok = keywods.GetKeyword(stringValueUppercase);
                                        token = tok == MySQLToken.None ? MySQLToken.Identifier : tok;
                                    }
                                    else
                                    {
                                        throw Err("expect digit char after SIGN for 'e': " + ch);
                                    }
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
                            else
                            {
                                if (CharTypes.IsIdentifierChar(ch))
                                {
                                    if (fstDot)
                                    {
                                        sizeCache = 1;
                                        ch = sql[curIndex = offsetCache + 1];
                                        token = MySQLToken.PuncDot;
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
                                            token = MySQLToken.LiteralNumMixDigit;
                                        }
                                    }
                                }
                                else
                                {
                                    token = MySQLToken.LiteralNumMixDigit;
                                }
                            }
                            return;
                        }
                }
            }
            switch (state)
            {
                case 0:
                    {
                        token = MySQLToken.LiteralNumPureDigit;
                        return;
                    }

                case 1:
                    {
                        if (fstDot)
                        {
                            token = MySQLToken.PuncDot;
                            return;
                        }
                        goto case 2;
                    }

                case 2:
                case 5:
                    {
                        token = MySQLToken.LiteralNumMixDigit;
                        return;
                    }

                case 3:
                    {
                        if (fstDot)
                        {
                            sizeCache = 1;
                            ch = sql[curIndex = offsetCache + 1];
                            token = MySQLToken.PuncDot;
                        }
                        else
                        {
                            if (!dot)
                            {
                                UpdateStringValue(sql, offsetCache, sizeCache);
                                MySQLToken tok = keywods.GetKeyword(stringValueUppercase);
                                token = tok == MySQLToken.None ? MySQLToken.Identifier : tok;
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
                            token = MySQLToken.PuncDot;
                        }
                        else
                        {
                            if (!dot)
                            {
                                ch = sql[--curIndex];
                                --sizeCache;
                                UpdateStringValue(sql, offsetCache, sizeCache);
                                MySQLToken tok = keywods.GetKeyword(stringValueUppercase);
                                token = tok == MySQLToken.None ? MySQLToken.Identifier : tok;
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
        /// NOTE:
        /// <see cref="Tup.Cobar.Parser.Recognizer.Mysql.MySQLToken.Identifier">id</see>
        /// dosn't include <code>'.'</code>
        /// for sake of performance issue (based on <i>shaojin.wensj</i>'s design).
        /// However, it is not convenient for MySQL compatibility. e.g.
        /// <code>".123f"</code> will be regarded as <code>".123"</code> and
        /// <code>"f"</code> in MySQL, but in this
        /// <see cref="MySQLLexer"/>
        /// , it will be
        /// <code>"."</code> and <code>"123f"</code> because <code>".123f"</code> may
        /// be part of <code>"db1.123f"</code> and <code>"123f"</code> is the table
        /// name.
        /// </summary>
        /// <param name="initSize">how many char has already been consumed</param>
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        private void ScanIdentifierFromNumber(int initOffset, int initSize)
        {
            offsetCache = initOffset;
            sizeCache = initSize;
            for (; CharTypes.IsIdentifierChar(ch); ++sizeCache)
            {
                ScanChar();
            }
            UpdateStringValue(sql, offsetCache, sizeCache);
            MySQLToken tok = keywods.GetKeyword(stringValueUppercase);
            token = tok == MySQLToken.None ? MySQLToken.Identifier : tok;
        }

        /// <summary>id is NOT included in <code>`</code>.</summary>
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
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

        /// <summary>not SQL syntax</summary>
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
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
            token = MySQLToken.PlaceHolder;
        }

        /// <summary>id is included in <code>`</code>.</summary>
        /// <remarks>id is included in <code>`</code>. first <code>`</code> is included</remarks>
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        protected virtual void ScanIdentifierWithAccent()
        {
            offsetCache = curIndex;
            for (; ScanChar() != Tup.Cobar.Parser.Recognizer.Mysql.Lexer.MySQLLexer.Eoi;)
            {
                if (ch == '`' && ScanChar() != '`')
                {
                    break;
                }
            }
            UpdateStringValue(sql, offsetCache, sizeCache = curIndex - offsetCache);
            token = MySQLToken.Identifier;
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
                            // MySQL specified
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
                                    // MySQL use 5 digits to indicate version. 50508 means
                                    // MySQL 5.5.8
                                    if (HasChars(5) && CharTypes.IsDigit(ch) && CharTypes.IsDigit(sql[curIndex + 1])
                                        && CharTypes.IsDigit(sql[curIndex + 2]) && CharTypes.IsDigit(sql[curIndex + 3])
                                        && CharTypes.IsDigit(sql[curIndex + 4]))
                                    {
                                        int version = ch - '0';
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
                                    for (int state = 0; !Eof(); ScanChar())
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
                                            else
                                            {
                                                if ('*' != ch)
                                                {
                                                    state = 0;
                                                }
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

        /// <summary>always throw SQLSyntaxErrorException</summary>
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        protected virtual SQLSyntaxErrorException Err(string msg)
        {
            string errMsg = msg + ". " + ToString();
            throw new SQLSyntaxErrorException(errMsg);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(GetType().Name).Append('@').Append(GetHashCode()).Append('{');
            string sqlLeft = new string(sql, curIndex, sql.Length - curIndex);
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
        /// <see cref="token"/>
        /// must be
        /// <see cref="Tup.Cobar.Parser.Recognizer.Mysql.MySQLToken.LiteralNumPureDigit"/>
        /// </summary>
        public virtual Number IntegerValue()
        {
            // 2147483647
            // 9223372036854775807
            if (sizeCache < 10 || sizeCache == 10 && (sql[offsetCache] < '2' || sql[offsetCache
                ] == '2' && sql[offsetCache + 1] == '0'))
            {
                int rst = 0;
                int end = offsetCache + sizeCache;
                for (int i = offsetCache; i < end; ++i)
                {
                    rst = (rst << 3) + (rst << 1);
                    rst += sql[i] - '0';
                }
                return rst;
            }
            else
            {
                if (sizeCache < 19 || sizeCache == 19 && sql[offsetCache] < '9')
                {
                    long rst = 0;
                    int end = offsetCache + sizeCache;
                    for (int i = offsetCache; i < end; ++i)
                    {
                        rst = (rst << 3) + (rst << 1);
                        rst += sql[i] - '0';
                    }
                    return rst;
                }
                else
                {
                    return BigInteger.Parse(new string(sql, offsetCache, sizeCache), 10);
                }
            }
        }

        public virtual BigDecimal DecimalValue()
        {
            // QS_TODO [performance enhance]: prevent BigDecimal's parser
            return BigDecimal.Parse(new string(sql, offsetCache, sizeCache));
        }

        /// <summary>
        /// if
        /// <see cref="StringValue()"/>
        /// returns "'abc\\'d'", then "abc\\'d" is appended
        /// </summary>
        public virtual void AppendStringContent(StringBuilder sb)
        {
            sb.Append(sbuf, 1, sizeCache - 2);
        }

        /// <summary>
        /// make sense for those types of token:<br/>
        /// <see cref="Tup.Cobar.Parser.Recognizer.Mysql.MySQLToken.UsrVar"/>
        /// : e.g. "@var1", "@'mary''s'";<br/>
        /// <see cref="Tup.Cobar.Parser.Recognizer.Mysql.MySQLToken.SysVar"/>
        /// : e.g. "var2";<br/>
        /// <see cref="Tup.Cobar.Parser.Recognizer.Mysql.MySQLToken.LiteralChars"/>
        /// ,
        /// <see cref="Tup.Cobar.Parser.Recognizer.Mysql.MySQLToken.LiteralNchars"/>
        /// : e.g.
        /// "'ab\\'c'";<br/>
        /// <see cref="Tup.Cobar.Parser.Recognizer.Mysql.MySQLToken.LiteralBit"/>
        /// : e.g. "0101" <br/>
        /// <see cref="Tup.Cobar.Parser.Recognizer.Mysql.MySQLToken.Identifier"/>
        /// </summary>
        public string StringValue()
        {
            return stringValue;
        }

        /// <summary>
        /// for
        /// <see cref="Tup.Cobar.Parser.Recognizer.Mysql.MySQLToken.Identifier"/>
        /// ,
        /// <see cref="Tup.Cobar.Parser.Recognizer.Mysql.MySQLToken.SysVar"/>
        /// </summary>
        public string StringValueUppercase()
        {
            return stringValueUppercase;
        }
    }
}