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
 * (created at 2011-3-11)
 */

//using java.sql.SQLSyntaxErrorException;
using System;
using System.Numerics;
using System.Text;
using System.Threading;
using Tup.Cobar.SqlParser.Util;

namespace Tup.Cobar.SqlParser.Recognizer.MySql.Lexer
{
    using keywods = Recognizer.MySql.Lexer.MySQLKeywords;

    /**
     * support MySQL 5.5 token
     *
     * @author <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
     */

    public class MySQLLexer
    {
        private static int C_STYLE_COMMENT_VERSION = 50599;

        /**
         * @return previous value
         */

        public static int setCStyleCommentVersion(int version)
        {
            int v = C_STYLE_COMMENT_VERSION;
            C_STYLE_COMMENT_VERSION = version;
            return v;
        }

        /**
         * End of input character. Used as a sentinel to denote the character one
         * beyond the last defined character in a source file.
         */
        private readonly static byte EOI = 0x1A;

        private readonly char[] sql;

        public char[] Sql
        {
            get { return sql; }
        }

        /** always be {@link #sql}.Length - 1 */
        private readonly int eofIndex;

        public int EofIndex
        {
            get { return eofIndex; }
        }

        /** current index of {@link #sql} */
        protected int curIndex = -1;

        public int CurIndex
        {
            get { return curIndex; }
        }

        /** always be {@link #sql}[{@link #curIndex}] */
        protected char ch;

        public char Ch
        {
            get { return ch; }
        }

        // /** current token, set by {@link #nextToken()} */
        // private int tokenPos = 0;
        private MySQLToken token;

        /** keyword only */
        private MySQLToken tokenCache;
        private MySQLToken tokenCache2;
        /** 1 represents first parameter */
        private int paramIndex = 0;

        /** A character buffer for literals. */
        protected readonly static ThreadLocal<char[]> sbufRef = new ThreadLocal<char[]>();
        protected char[] sbuf;

        private string stringValue = null;
        /** make sense only for {@link MySQLToken#IDENTIFIER} */
        private string stringValueUppercase = null;

        /**
         * update {@link MySQLLexer#stringValue} and
         * {@link MySQLLexer#stringValueUppercase}. It is possible that
         * {@link #sbuf} be changed
         */

        protected void updateStringValue(char[] src, int srcOffset, int len)
        {
            // QS_TODO [performance enhance]: use string constant for special
            // identifier, so that parser can use '==' rather than 'equals'
            stringValue = new String(src, srcOffset, len);
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
                        SystemUtils.arraycopy(src, srcOffset, sbuf, 0, srcIndex - srcOffset);
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
                stringValueUppercase = new String(sbuf, 0, len);
            }
            else
            {
                stringValueUppercase = new String(src, srcOffset, len);
            }
        }

        public MySQLLexer(char[] sql) //throws SQLSyntaxErrorException
        {
            if ((sbuf = sbufRef.Value) == null)
            {
                sbuf = new char[1024];
                sbufRef.Value = sbuf;
            }
            if (CharTypes.isWhitespace(sql[sql.Length - 1]))
            {
                this.sql = sql;
            }
            else
            {
                this.sql = new char[sql.Length + 1];
                SystemUtils.arraycopy(sql, 0, this.sql, 0, sql.Length);
            }
            eofIndex = this.sql.Length - 1;
            this.sql[eofIndex] = (char)MySQLLexer.EOI;
            scanChar();
            nextToken();
        }

        public MySQLLexer(string sql) : this(fromSQL2Chars(sql)) //throws SQLSyntaxErrorException
        {
            //this(fromSQL2Chars(sql));
        }

        private static char[] fromSQL2Chars(string sql)
        {
            if (CharTypes.isWhitespace(sql[sql.Length - 1]))
            {
                return sql.ToCharArray();
            }
            char[] chars = new char[sql.Length + 1];
            SystemUtils.arraycopy(sql.ToCharArray(), 0, chars, 0, sql.Length);
            chars[chars.Length - 1] = ' ';
            return chars;
        }

        //protected MySQLKeywords keywods = MySQLKeywords.DEFAULT_KEYWORDS;

        /**
         * @param token must be a keyword
         */

        public void addCacheToke(MySQLToken token)
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

        public MySQLToken GetToken()
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

        public int getCurrentIndex()
        {
            return curIndex;
        }

        public char[] getSQL()
        {
            return sql;
        }

        public int getOffsetCache()
        {
            return offsetCache;
        }

        public int getSizeCache()
        {
            return sizeCache;
        }

        /**
         * @return start from 1. When there is no parameter yet, return 0.
         */

        public int GetParamIndex()
        {
            return paramIndex;
        }

        protected char scanChar()
        {
            return ch = sql[++curIndex];
        }

        /**
         * @param skip if 1, then equals to {@link #scanChar()}
         */

        protected char scanChar(int skip)
        {
            return ch = sql[curIndex += skip];
        }

        protected bool hasChars(int howMany)
        {
            return curIndex + howMany <= eofIndex;
        }

        protected bool eof()
        {
            return curIndex >= eofIndex;
        }

        private MySQLToken nextTokenInternal() //throws SQLSyntaxErrorException
        {
            switch (ch)
            {
                case '0':
                    switch (sql[curIndex + 1])
                    {
                        case 'x':
                            scanChar(2);
                            scanHexaDecimal(false);
                            return token;

                        case 'b':
                            scanChar(2);
                            scanBitField(false);
                            return token;
                    }
                    scanNumber();
                    return token;

                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    scanNumber();
                    return token;

                case '.':
                    if (CharTypes.isDigit(sql[curIndex + 1]))
                    {
                        scanNumber();
                    }
                    else
                    {
                        scanChar();
                        token = MySQLToken.PUNC_DOT;
                    }
                    return token;

                case '\'':
                case '"':
                    scanString();
                    return token;

                case 'n':
                case 'N':
                    if (sql[curIndex + 1] == '\'')
                    {
                        scanChar();
                        scanString();
                        token = MySQLToken.LITERAL_NCHARS;
                        return token;
                    }
                    scanIdentifier();
                    return token;

                case 'x':
                case 'X':
                    if (sql[curIndex + 1] == '\'')
                    {
                        scanChar(2);
                        scanHexaDecimal(true);
                        return token;
                    }
                    scanIdentifier();
                    return token;

                case 'b':
                case 'B':
                    if (sql[curIndex + 1] == '\'')
                    {
                        scanChar(2);
                        scanBitField(true);
                        return token;
                    }
                    scanIdentifier();
                    return token;

                case '@':
                    if (sql[curIndex + 1] == '@')
                    {
                        scanSystemUtilsVariable();
                        return token;
                    }
                    scanUserVariable();
                    return token;

                case '?':
                    scanChar();
                    token = MySQLToken.QUESTION_MARK;
                    ++paramIndex;
                    return token;

                case '(':
                    scanChar();
                    token = MySQLToken.PUNC_LEFT_PAREN;
                    return token;

                case ')':
                    scanChar();
                    token = MySQLToken.PUNC_RIGHT_PAREN;
                    return token;

                case '[':
                    scanChar();
                    token = MySQLToken.PUNC_LEFT_BRACKET;
                    return token;

                case ']':
                    scanChar();
                    token = MySQLToken.PUNC_RIGHT_BRACKET;
                    return token;

                case '{':
                    scanChar();
                    token = MySQLToken.PUNC_LEFT_BRACE;
                    return token;

                case '}':
                    scanChar();
                    token = MySQLToken.PUNC_RIGHT_BRACE;
                    return token;

                case ',':
                    scanChar();
                    token = MySQLToken.PUNC_COMMA;
                    return token;

                case ';':
                    scanChar();
                    token = MySQLToken.PUNC_SEMICOLON;
                    return token;

                case ':':
                    if (sql[curIndex + 1] == '=')
                    {
                        scanChar(2);
                        token = MySQLToken.OP_ASSIGN;
                        return token;
                    }
                    scanChar();
                    token = MySQLToken.PUNC_COLON;
                    return token;

                case '=':
                    scanChar();
                    token = MySQLToken.OP_EQUALS;
                    return token;

                case '~':
                    scanChar();
                    token = MySQLToken.OP_TILDE;
                    return token;

                case '*':
                    if (inCStyleComment && sql[curIndex + 1] == '/')
                    {
                        inCStyleComment = false;
                        inCStyleCommentIgnore = false;
                        scanChar(2);
                        token = MySQLToken.PUNC_C_STYLE_COMMENT_END;
                        return token;
                    }
                    scanChar();
                    token = MySQLToken.OP_ASTERISK;
                    return token;

                case '-':
                    scanChar();
                    token = MySQLToken.OP_MINUS;
                    return token;

                case '+':
                    scanChar();
                    token = MySQLToken.OP_PLUS;
                    return token;

                case '^':
                    scanChar();
                    token = MySQLToken.OP_CARET;
                    return token;

                case '/':
                    scanChar();
                    token = MySQLToken.OP_SLASH;
                    return token;

                case '%':
                    scanChar();
                    token = MySQLToken.OP_PERCENT;
                    return token;

                case '&':
                    if (sql[curIndex + 1] == '&')
                    {
                        scanChar(2);
                        token = MySQLToken.OP_LOGICAL_AND;
                        return token;
                    }
                    scanChar();
                    token = MySQLToken.OP_AMPERSAND;
                    return token;

                case '|':
                    if (sql[curIndex + 1] == '|')
                    {
                        scanChar(2);
                        token = MySQLToken.OP_LOGICAL_OR;
                        return token;
                    }
                    scanChar();
                    token = MySQLToken.OP_VERTICAL_BAR;
                    return token;

                case '!':
                    if (sql[curIndex + 1] == '=')
                    {
                        scanChar(2);
                        token = MySQLToken.OP_NOT_EQUALS;
                        return token;
                    }
                    scanChar();
                    token = MySQLToken.OP_EXCLAMATION;
                    return token;

                case '>':
                    switch (sql[curIndex + 1])
                    {
                        case '=':
                            scanChar(2);
                            token = MySQLToken.OP_GREATER_OR_EQUALS;
                            return token;

                        case '>':
                            scanChar(2);
                            token = MySQLToken.OP_RIGHT_SHIFT;
                            return token;

                        default:
                            scanChar();
                            token = MySQLToken.OP_GREATER_THAN;
                            return token;
                    }
                case '<':
                    switch (sql[curIndex + 1])
                    {
                        case '=':
                            if (sql[curIndex + 2] == '>')
                            {
                                scanChar(3);
                                token = MySQLToken.OP_NULL_SAFE_EQUALS;
                                return token;
                            }
                            scanChar(2);
                            token = MySQLToken.OP_LESS_OR_EQUALS;
                            return token;

                        case '>':
                            scanChar(2);
                            token = MySQLToken.OP_LESS_OR_GREATER;
                            return token;

                        case '<':
                            scanChar(2);
                            token = MySQLToken.OP_LEFT_SHIFT;
                            return token;

                        default:
                            scanChar();
                            token = MySQLToken.OP_LESS_THAN;
                            return token;
                    }
                case '`':
                    scanIdentifierWithAccent();
                    return token;

                default:
                    if (CharTypes.isIdentifierChar(ch))
                    {
                        scanIdentifier();
                    }
                    else if (eof())
                    {
                        token = MySQLToken.EOF;
                        curIndex = eofIndex;
                        // tokenPos = curIndex;
                    }
                    else
                    {
                        throw err("unsupported character: " + ch);
                    }
                    return token;
            }
        }

        public MySQLToken nextToken()
        //throws SQLSyntaxErrorException
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
            if (token == MySQLToken.EOF)
            {
                throw new SQLSyntaxErrorException("eof for sql is already reached, cannot get new token");
            }
            MySQLToken t;
            do
            {
                skipSeparator();
                t = nextTokenInternal();
            } while (inCStyleComment && inCStyleCommentIgnore || MySQLToken.PUNC_C_STYLE_COMMENT_END == t);
            return t;
        }

        protected bool inCStyleComment;
        protected bool inCStyleCommentIgnore;

        protected int offsetCache;
        protected int sizeCache;

        /**
         * first <code>@</code> is included
         */

        protected void scanUserVariable()
        //throws SQLSyntaxErrorException
        {
            if (ch != '@')
                throw err("first char must be @");
            offsetCache = curIndex;
            sizeCache = 1;

            bool dq = false;
            switch (scanChar())
            {
                case '"':
                    dq = true;
                    goto loop0;
                case '\'':
                loop0:
                    for (++sizeCache; ; ++sizeCache)
                    {
                        switch (scanChar())
                        {
                            case '\\':
                                ++sizeCache;
                                scanChar();
                                break;

                            case '"':
                                if (dq)
                                {
                                    ++sizeCache;
                                    if (scanChar() == '"')
                                    {
                                        break;
                                    }
                                    goto loop1;
                                }
                                break;

                            case '\'':
                                if (!dq)
                                {
                                    ++sizeCache;
                                    if (scanChar() == '\'')
                                    {
                                        break;
                                    }
                                    goto loop1;
                                }
                                break;
                        }
                    }
                loop1:
                    break;

                case '`':
                    for (++sizeCache; ; ++sizeCache)
                    {
                        switch (scanChar())
                        {
                            case '`':
                                ++sizeCache;
                                if (scanChar() == '`')
                                {
                                    break;
                                }
                                goto loop2;
                        }
                    }
                loop2:
                    break;

                default:
                    for (; CharTypes.isIdentifierChar(ch) || ch == '.'; ++sizeCache)
                    {
                        scanChar();
                    }
                    break;
            }

            stringValue = new String(sql, offsetCache, sizeCache);
            token = MySQLToken.USR_VAR;
        }

        /**
         * first <code>@@</code> is included
         */

        protected void scanSystemUtilsVariable() //throws SQLSyntaxErrorException
        {
            if (ch != '@' || sql[curIndex + 1] != '@')
                throw err("first char must be @@");

            offsetCache = curIndex + 2;
            sizeCache = 0;
            scanChar(2);
            if (ch == '`')
            {
                for (++sizeCache; ; ++sizeCache)
                {
                    if (scanChar() == '`')
                    {
                        ++sizeCache;
                        if (scanChar() != '`')
                        {
                            break;
                        }
                    }
                }
            }
            else
            {
                for (; CharTypes.isIdentifierChar(ch); ++sizeCache)
                {
                    scanChar();
                }
            }
            updateStringValue(sql, offsetCache, sizeCache);
            token = MySQLToken.SYS_VAR;
        }

        protected void scanString() //throws SQLSyntaxErrorException
        {
            bool dq = false;
            if (ch == '\'')
            {
            }
            else if (ch == '"')
            {
                dq = true;
            }
            else
            {
                throw err("first char must be \" or '");
            }

            offsetCache = curIndex;
            int size = 1;
            sbuf[0] = '\'';
            if (dq)
            {
                while (true)
                {
                    switch (scanChar())
                    {
                        case '\'':
                            putChar('\\', size++);
                            putChar('\'', size++);
                            break;

                        case '\\':
                            putChar('\\', size++);
                            putChar(scanChar(), size++);
                            continue;
                        case '"':
                            if (sql[curIndex + 1] == '"')
                            {
                                putChar('"', size++);
                                scanChar();
                                continue;
                            }
                            putChar('\'', size++);
                            scanChar();
                            goto loop;
                        default:
                            if (eof())
                            {
                                throw err("unclosed string");
                            }
                            putChar(ch, size++);
                            continue;
                    }
                }
            }
            else
            {
                while (true)
                {
                    switch (scanChar())
                    {
                        case '\\':
                            putChar('\\', size++);
                            putChar(scanChar(), size++);
                            continue;
                        case '\'':
                            if (sql[curIndex + 1] == '\'')
                            {
                                putChar('\\', size++);
                                putChar(scanChar(), size++);
                                continue;
                            }
                            putChar('\'', size++);
                            scanChar();
                            goto loop;
                        default:
                            if (eof())
                            {
                                throw err("unclosed string");
                            }
                            putChar(ch, size++);
                            continue;
                    }
                }
            }
        loop:
            sizeCache = size;
            stringValue = new String(sbuf, 0, size);
            token = MySQLToken.LITERAL_CHARS;
        }

        /**
         * Append a character to sbuf.
         */

        protected void putChar(char ch, int index)
        {
            if (index >= sbuf.Length)
            {
                char[] newsbuf = new char[sbuf.Length * 2];
                SystemUtils.arraycopy(sbuf, 0, newsbuf, 0, sbuf.Length);
                sbuf = newsbuf;
            }
            sbuf[index] = ch;
        }

        /**
         * @param quoteMode if false: first <code>0x</code> has been skipped; if
         *            true: first <code>x'</code> has been skipped
         */

        protected void scanHexaDecimal(bool quoteMode) //throws SQLSyntaxErrorException
        {
            offsetCache = curIndex;
            for (; CharTypes.isHex(ch); scanChar()) ;

            sizeCache = curIndex - offsetCache;
            // if (sizeCache <= 0) {
            // throw err("expect at least one hexdigit");
            // }
            if (quoteMode)
            {
                if (ch != '\'')
                {
                    throw err("invalid char for hex: " + ch);
                }
                scanChar();
            }
            else if (CharTypes.isIdentifierChar(ch))
            {
                scanIdentifierFromNumber(offsetCache - 2, sizeCache + 2);
                return;
            }

            token = MySQLToken.LITERAL_HEX;
        }

        /**
         * @param quoteMode if false: first <code>0b</code> has been skipped; if
         *            true: first <code>b'</code> has been skipped
         */

        protected void scanBitField(bool quoteMode) //throws SQLSyntaxErrorException
        {
            offsetCache = curIndex;
            for (; ch == '0' || ch == '1'; scanChar()) ;
            sizeCache = curIndex - offsetCache;
            // if (sizeCache <= 0) {
            // throw err("expect at least one bit");
            // }
            if (quoteMode)
            {
                if (ch != '\'')
                {
                    throw err("invalid char for bit: " + ch);
                }
                scanChar();
            }
            else if (CharTypes.isIdentifierChar(ch))
            {
                scanIdentifierFromNumber(offsetCache - 2, sizeCache + 2);
                return;
            }

            token = MySQLToken.LITERAL_BIT;
            stringValue = new String(sql, offsetCache, sizeCache);
        }

        /**
         * if first char is <code>.</code>, token may be {@link MySQLToken#PUNC_DOT}
         * if invalid char is presented after <code>.</code>
         */

        protected void scanNumber() //throws SQLSyntaxErrorException
        {
            offsetCache = curIndex;
            sizeCache = 1;
            bool fstDot = ch == '.';
            bool dot = fstDot;
            bool sign = false;
            int state = fstDot ? 1 : 0;

            for (; scanChar() != MySQLLexer.EOI; ++sizeCache)
            {
                switch (state)
                {
                    case 0:
                        if (CharTypes.isDigit(ch))
                        {
                            ;
                        }
                        else if (ch == '.')
                        {
                            dot = true;
                            state = 1;
                        }
                        else if (ch == 'e' || ch == 'E')
                        {
                            state = 3;
                        }
                        else if (CharTypes.isIdentifierChar(ch))
                        {
                            scanIdentifierFromNumber(offsetCache, sizeCache);
                            return;
                        }
                        else
                        {
                            token = MySQLToken.LITERAL_NUM_PURE_DIGIT;
                            return;
                        }
                        break;

                    case 1:
                        if (CharTypes.isDigit(ch))
                        {
                            state = 2;
                        }
                        else if (ch == 'e' || ch == 'E')
                        {
                            state = 3;
                        }
                        else if (CharTypes.isIdentifierChar(ch) && fstDot)
                        {
                            sizeCache = 1;
                            ch = sql[curIndex = offsetCache + 1];
                            token = MySQLToken.PUNC_DOT;
                            return;
                        }
                        else
                        {
                            token = MySQLToken.LITERAL_NUM_MIX_DIGIT;
                            return;
                        }
                        break;

                    case 2:
                        if (CharTypes.isDigit(ch))
                        {
                            ;
                        }
                        else if (ch == 'e' || ch == 'E')
                        {
                            state = 3;
                        }
                        else if (CharTypes.isIdentifierChar(ch) && fstDot)
                        {
                            sizeCache = 1;
                            ch = sql[curIndex = offsetCache + 1];
                            token = MySQLToken.PUNC_DOT;
                            return;
                        }
                        else
                        {
                            token = MySQLToken.LITERAL_NUM_MIX_DIGIT;
                            return;
                        }
                        break;

                    case 3:
                        if (CharTypes.isDigit(ch))
                        {
                            state = 5;
                        }
                        else if (ch == '+' || ch == '-')
                        {
                            sign = true;
                            state = 4;
                        }
                        else if (fstDot)
                        {
                            sizeCache = 1;
                            ch = sql[curIndex = offsetCache + 1];
                            token = MySQLToken.PUNC_DOT;
                            return;
                        }
                        else if (!dot)
                        {
                            if (CharTypes.isIdentifierChar(ch))
                            {
                                scanIdentifierFromNumber(offsetCache, sizeCache);
                            }
                            else
                            {
                                updateStringValue(sql, offsetCache, sizeCache);
                                MySQLToken tok = keywods.getKeyword(stringValueUppercase);
                                token = tok == MySQLToken.None ? MySQLToken.IDENTIFIER : tok;
                            }
                            return;
                        }
                        else
                        {
                            throw err("invalid char after '.' and 'e' for as part of number: " + ch);
                        }
                        break;

                    case 4:
                        if (CharTypes.isDigit(ch))
                        {
                            state = 5;
                            break;
                        }
                        else if (fstDot)
                        {
                            sizeCache = 1;
                            ch = sql[curIndex = offsetCache + 1];
                            token = MySQLToken.PUNC_DOT;
                        }
                        else if (!dot)
                        {
                            ch = sql[--curIndex];
                            --sizeCache;
                            updateStringValue(sql, offsetCache, sizeCache);
                            MySQLToken tok = keywods.getKeyword(stringValueUppercase);
                            token = tok == MySQLToken.None ? MySQLToken.IDENTIFIER : tok;
                        }
                        else
                        {
                            throw err("expect digit char after SIGN for 'e': " + ch);
                        }
                        return;

                    case 5:
                        if (CharTypes.isDigit(ch))
                        {
                            break;
                        }
                        else if (CharTypes.isIdentifierChar(ch))
                        {
                            if (fstDot)
                            {
                                sizeCache = 1;
                                ch = sql[curIndex = offsetCache + 1];
                                token = MySQLToken.PUNC_DOT;
                            }
                            else if (!dot)
                            {
                                if (sign)
                                {
                                    ch = sql[curIndex = offsetCache];
                                    scanIdentifierFromNumber(curIndex, 0);
                                }
                                else
                                {
                                    scanIdentifierFromNumber(offsetCache, sizeCache);
                                }
                            }
                            else
                            {
                                token = MySQLToken.LITERAL_NUM_MIX_DIGIT;
                            }
                        }
                        else
                        {
                            token = MySQLToken.LITERAL_NUM_MIX_DIGIT;
                        }
                        return;
                }
            }

            switch (state)
            {
                case 0:
                    token = MySQLToken.LITERAL_NUM_PURE_DIGIT;
                    return;

                case 1:
                    if (fstDot)
                        token = MySQLToken.PUNC_DOT;
                    else
                        token = MySQLToken.LITERAL_NUM_MIX_DIGIT;
                    return;

                case 2:
                case 5:
                    token = MySQLToken.LITERAL_NUM_MIX_DIGIT;
                    return;

                case 3:
                    if (fstDot)
                    {
                        sizeCache = 1;
                        ch = sql[curIndex = offsetCache + 1];
                        token = MySQLToken.PUNC_DOT;
                    }
                    else if (!dot)
                    {
                        updateStringValue(sql, offsetCache, sizeCache);
                        MySQLToken tok = keywods.getKeyword(stringValueUppercase);
                        token = tok == MySQLToken.None ? MySQLToken.IDENTIFIER : tok;
                    }
                    else
                    {
                        throw err("expect digit char after SIGN for 'e': " + ch);
                    }
                    return;

                case 4:
                    if (fstDot)
                    {
                        sizeCache = 1;
                        ch = sql[curIndex = offsetCache + 1];
                        token = MySQLToken.PUNC_DOT;
                    }
                    else if (!dot)
                    {
                        ch = sql[--curIndex];
                        --sizeCache;
                        updateStringValue(sql, offsetCache, sizeCache);
                        MySQLToken tok = keywods.getKeyword(stringValueUppercase);
                        token = tok == MySQLToken.None ? MySQLToken.IDENTIFIER : tok;
                    }
                    else
                    {
                        throw err("expect digit char after SIGN for 'e': " + ch);
                    }
                    return;
            }
        }

        /**
         * NOTE: {@link MySQLToken#IDENTIFIER id} dosn't include <code>'.'</code>
         * for sake of performance issue (based on <i>shaojin.wensj</i>'s design).
         * However, it is not convenient for MySQL compatibility. e.g.
         * <code>".123f"</code> will be regarded as <code>".123"</code> and
         * <code>"f"</code> in MySQL, but in this {@link MySQLLexer}, it will be
         * <code>"."</code> and <code>"123f"</code> because <code>".123f"</code> may
         * be part of <code>"db1.123f"</code> and <code>"123f"</code> is the table
         * name.
         *
         * @param initSize how many char has already been consumed
         */

        private void scanIdentifierFromNumber(int initOffset, int initSize) //throws SQLSyntaxErrorException
        {
            offsetCache = initOffset;
            sizeCache = initSize;
            for (; CharTypes.isIdentifierChar(ch); ++sizeCache)
            {
                scanChar();
            }
            updateStringValue(sql, offsetCache, sizeCache);
            MySQLToken tok = keywods.getKeyword(stringValueUppercase);
            token = tok == MySQLToken.None ? MySQLToken.IDENTIFIER : tok;
        }

        /**
         * id is NOT included in <code>`</code>.
         */

        protected void scanIdentifier() //throws SQLSyntaxErrorException
        {
            if (ch == '$')
            {
                if (scanChar() == '{')
                {
                    scanPlaceHolder();
                }
                else
                {
                    scanIdentifierFromNumber(curIndex - 1, 1);
                }
            }
            else
            {
                scanIdentifierFromNumber(curIndex, 0);
            }
        }

        /**
         * not SQL syntax
         */

        protected void scanPlaceHolder() //throws SQLSyntaxErrorException
        {
            offsetCache = curIndex + 1;
            sizeCache = 0;
            for (scanChar(); ch != '}' && !eof(); ++sizeCache)
            {
                scanChar();
            }
            if (ch == '}')
                scanChar();

            updateStringValue(sql, offsetCache, sizeCache);
            token = MySQLToken.PLACE_HOLDER;
        }

        /**
         * id is included in <code>`</code>. first <code>`</code> is included
         */

        protected void scanIdentifierWithAccent() //throws SQLSyntaxErrorException
        {
            offsetCache = curIndex;
            for (; scanChar() != MySQLLexer.EOI;)
            {
                if (ch == '`' && scanChar() != '`')
                {
                    break;
                }
            }
            updateStringValue(sql, offsetCache, sizeCache = curIndex - offsetCache);
            token = MySQLToken.IDENTIFIER;
        }

        /**
         * skip whitespace and comment
         */

        protected void skipSeparator()
        {
            for (; !eof();)
            {
                for (; CharTypes.isWhitespace(ch); scanChar()) ;

                switch (ch)
                {
                    case '#': // MySQL specified
                        for (; scanChar() != '\n';)
                        {
                            if (eof())
                            {
                                return;
                            }
                        }
                        scanChar();
                        continue;
                    case '/':
                        if (hasChars(2) && '*' == sql[curIndex + 1])
                        {
                            bool commentSkip;
                            if ('!' == sql[curIndex + 2])
                            {
                                scanChar(3);
                                inCStyleComment = true;
                                inCStyleCommentIgnore = false;
                                commentSkip = false;
                                // MySQL use 5 digits to indicate version. 50508 means
                                // MySQL 5.5.8
                                if (hasChars(5) && CharTypes.isDigit(ch) && CharTypes.isDigit(sql[curIndex + 1])
                                        && CharTypes.isDigit(sql[curIndex + 2]) && CharTypes.isDigit(sql[curIndex + 3])
                                        && CharTypes.isDigit(sql[curIndex + 4]))
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
                                    scanChar(5);
                                    if (version > C_STYLE_COMMENT_VERSION)
                                    {
                                        inCStyleCommentIgnore = true;
                                    }
                                }
                                skipSeparator();
                            }
                            else
                            {
                                scanChar(2);
                                commentSkip = true;
                            }

                            if (commentSkip)
                            {
                                for (int state = 0; !eof(); scanChar())
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
                                            scanChar();
                                            break;
                                        }
                                        else if ('*' != ch)
                                        {
                                            state = 0;
                                        }
                                    }
                                }
                                continue;
                            }
                        }
                        return;

                    case '-':
                        if (hasChars(3) && '-' == sql[curIndex + 1] && CharTypes.isWhitespace(sql[curIndex + 2]))
                        {
                            scanChar(3);
                            for (; !eof(); scanChar())
                            {
                                if ('\n' == ch)
                                {
                                    scanChar();
                                    break;
                                }
                            }
                            continue;
                        }
                        return;

                    default:
                        return;
                }
            }
        }

        /**
         * always throw SQLSyntaxErrorException
         */

        protected SQLSyntaxErrorException err(string msg) //throws SQLSyntaxErrorException
        {
            string errMsg = msg + ". " + ToString();
            throw new SQLSyntaxErrorException(errMsg);
        }

        //@Override
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("{{MySQLLexer {0}", GetHashCode());
            string sqlLeft = new String(sql, curIndex, sql.Length - curIndex);
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

        /**
         * {@link #token} must be {@link MySQLToken#LITERAL_NUM_PURE_DIGIT}
         */

        public BigInteger integerValue()
        {
            // 2147483647
            // 9223372036854775807
            if (sizeCache < 10 || sizeCache == 10
                    && (sql[offsetCache] < '2' || sql[offsetCache] == '2' && sql[offsetCache + 1] == '0'))
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
            else if (sizeCache < 19 || sizeCache == 19 && sql[offsetCache] < '9')
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
                return BigInteger.Parse(new string(sql, offsetCache, sizeCache));
            }
        }

        public BigDecimal decimalValue()
        {
            // QS_TODO [performance enhance]: prevent BigDecimal's parser
            //TODO return new BigDecimal(sql, offsetCache, sizeCache);
            return double.Parse(new string(sql, offsetCache, sizeCache));
        }

        /**
         * if {@link #stringValue()} returns "'abc\\'d'", then "abc\\'d" is appended
         */

        public void appendStringContent(StringBuilder sb)
        {
            sb.Append(sbuf, 1, sizeCache - 2);
        }

        /**
         * make sense for those types of token:<br/>
         * {@link MySQLToken#USR_VAR}: e.g. "@var1", "@'mary''s'";<br/>
         * {@link MySQLToken#SYS_VAR}: e.g. "var2";<br/>
         * {@link MySQLToken#LITERAL_CHARS}, {@link MySQLToken#LITERAL_NCHARS}: e.g.
         * "'ab\\'c'";<br/>
         * {@link MySQLToken#LITERAL_BIT}: e.g. "0101" <br/>
         * {@link MySQLToken#IDENTIFIER}
         */

        public string GetStringValue()
        {
            return stringValue;
        }

        /**
         * for {@link MySQLToken#IDENTIFIER}, {@link MySQLToken#SYS_VAR}
         */

        public string GetStringValueUppercase()
        {
            return stringValueUppercase;
        }
    }
}