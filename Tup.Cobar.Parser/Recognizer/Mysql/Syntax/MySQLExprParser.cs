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

using Tup.Cobar.Parser.Ast.Expression.Arithmeic;
using Tup.Cobar.Parser.Ast.Expression.Bit;
using Tup.Cobar.Parser.Ast.Expression.Comparison;
using Tup.Cobar.Parser.Ast.Expression.Logical;
using Tup.Cobar.Parser.Ast.Expression.Misc;
using Tup.Cobar.Parser.Ast.Expression.Primary;
using Tup.Cobar.Parser.Ast.Expression.Primary.Function;
using Tup.Cobar.Parser.Ast.Expression.Primary.Function.Cast;
using Tup.Cobar.Parser.Ast.Expression.Primary.Function.Comparison;
using Tup.Cobar.Parser.Ast.Expression.Primary.Function.Datetime;
using Tup.Cobar.Parser.Ast.Expression.Primary.Function.Groupby;
using Tup.Cobar.Parser.Ast.Expression.Primary.Function.Info;
using Tup.Cobar.Parser.Ast.Expression.Primary.Function.String;
using Tup.Cobar.Parser.Ast.Expression.Primary.Literal;
using Tup.Cobar.Parser.Ast.Expression.String;
using Tup.Cobar.Parser.Ast.Expression.Type;
using Tup.Cobar.Parser.Recognizer.Mysql.Lexer;
using Tup.Cobar.Parser.Util;
using Expr = Tup.Cobar.Parser.Ast.Expression.Expression;
using IdentifierExpr = Tup.Cobar.Parser.Ast.Expression.Primary.Identifier;
using MatchExpr = Tup.Cobar.Parser.Ast.Expression.Primary.MatchExpression;

namespace Tup.Cobar.Parser.Recognizer.Mysql.Syntax
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class MySQLExprParser : MySQLParser
    {
        public MySQLExprParser(MySQLLexer lexer)
            : this(lexer, MySQLFunctionManager.InstanceMysqlDefault, true, DefaultCharset)
        {
        }

        public MySQLExprParser(MySQLLexer lexer, string charset)
            : this(lexer, MySQLFunctionManager.InstanceMysqlDefault, true, charset)
        {
        }

        public MySQLExprParser(MySQLLexer lexer, MySQLFunctionManager functionManager, bool cacheEvalRst, string charset)
            : base(lexer, cacheEvalRst)
        {
            this.functionManager = functionManager;
            this.charset = charset == null ? DefaultCharset : charset;
        }

        private string charset;

        private readonly MySQLFunctionManager functionManager;

        private MySQLDMLSelectParser selectParser;

        public virtual void SetSelectParser(MySQLDMLSelectParser selectParser)
        {
            this.selectParser = selectParser;
        }

        /// <summary>first token of this expression has been scanned, not yet consumed</summary>
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        public virtual Expr Expression()
        {
            MySQLToken token = lexer.Token();
            if (token == MySQLToken.None)
            {
                token = lexer.NextToken();
            }
            if (token == MySQLToken.Eof)
            {
                Err("unexpected EOF");
            }
            Expr left = LogicalOrExpression();
            if (lexer.Token() == MySQLToken.OpAssign)
            {
                lexer.NextToken();
                Expr right = Expression();
                return new AssignmentExpression(left, right).SetCacheEvalRst(cacheEvalRst);
            }
            return left;
        }

        /// <summary><code>higherPRJExpr ( ( '||' | 'OR' ) higherPRJExpr )*</code></summary>
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        private Expr LogicalOrExpression()
        {
            LogicalOrExpression or = null;
            for (Expr expr = LogicalXORExpression(); ;)
            {
                switch (lexer.Token())
                {
                    case MySQLToken.OpLogicalOr:
                    case MySQLToken.KwOr:
                        {
                            lexer.NextToken();
                            if (or == null)
                            {
                                or = new LogicalOrExpression();
                                or.SetCacheEvalRst(cacheEvalRst);
                                or.AppendOperand(expr);
                                expr = or;
                            }
                            Expr newExpr = LogicalXORExpression();
                            or.AppendOperand(newExpr);
                            break;
                        }

                    default:
                        {
                            return expr;
                        }
                }
            }
        }

        /// <summary><code>higherPRJExpr ( 'XOR' higherPRJExpr )*</code></summary>
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        private Expr LogicalXORExpression()
        {
            for (Expr expr = LogicalAndExpression(); ;)
            {
                switch (lexer.Token())
                {
                    case MySQLToken.KwXor:
                        {
                            lexer.NextToken();
                            Expr newExpr = LogicalAndExpression();
                            expr = new LogicalXORExpression(expr, newExpr).SetCacheEvalRst(cacheEvalRst);
                            break;
                        }

                    default:
                        {
                            return expr;
                        }
                }
            }
        }

        /// <summary><code>higherPRJExpr ( ('AND'|'&&') higherPRJExpr )*</code></summary>
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        private Expr LogicalAndExpression()
        {
            LogicalAndExpression and = null;
            for (Expr expr = LogicalNotExpression(); ;)
            {
                switch (lexer.Token())
                {
                    case MySQLToken.OpLogicalAnd:
                    case MySQLToken.KwAnd:
                        {
                            lexer.NextToken();
                            if (and == null)
                            {
                                and = new LogicalAndExpression();
                                and.SetCacheEvalRst(cacheEvalRst);
                                and.AppendOperand(expr);
                                expr = and;
                            }
                            Expr newExpr = LogicalNotExpression();
                            and.AppendOperand(newExpr);
                            break;
                        }

                    default:
                        {
                            return expr;
                        }
                }
            }
        }

        /// <summary><code>'NOT'* higherPRJExpr</code></summary>
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        private Expr LogicalNotExpression()
        {
            int not = 0;
            for (; lexer.Token() == MySQLToken.KwNot; ++not)
            {
                lexer.NextToken();
            }
            Expr expr = ComparisionExpression();
            for (; not > 0; --not)
            {
                expr = new LogicalNotExpression(expr).SetCacheEvalRst(cacheEvalRst);
            }
            return expr;
        }

        /// <summary><code>BETWEEN ...</summary>
        /// <remarks>
        /// <code>BETWEEN ... AND</code> has lower precedence than other comparison
        /// operator
        /// </remarks>
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        private Expr ComparisionExpression()
        {
            Expr temp;
            for (Expr fst = BitOrExpression(null, null); ;)
            {
                switch (lexer.Token())
                {
                    case MySQLToken.KwNot:
                        {
                            lexer.NextToken();
                            switch (lexer.Token())
                            {
                                case MySQLToken.KwBetween:
                                    {
                                        lexer.NextToken();
                                        Expr snd = ComparisionExpression();
                                        Match(MySQLToken.KwAnd);
                                        Expr trd = ComparisionExpression();
                                        return new BetweenAndExpression(true, fst, snd, trd).SetCacheEvalRst(cacheEvalRst);
                                    }

                                case MySQLToken.KwRlike:
                                case MySQLToken.KwRegexp:
                                    {
                                        lexer.NextToken();
                                        temp = BitOrExpression(null, null);
                                        fst = new RegexpExpression(true, fst, temp).SetCacheEvalRst(cacheEvalRst);
                                        continue;
                                    }

                                case MySQLToken.KwLike:
                                    {
                                        lexer.NextToken();
                                        temp = BitOrExpression(null, null);
                                        Expr escape = null;
                                        if (EqualsIdentifier("ESCAPE") >= 0)
                                        {
                                            lexer.NextToken();
                                            escape = BitOrExpression(null, null);
                                        }
                                        fst = new LikeExpression(true, fst, temp, escape).SetCacheEvalRst(cacheEvalRst);
                                        continue;
                                    }

                                case MySQLToken.KwIn:
                                    {
                                        if (lexer.NextToken() != MySQLToken.PuncLeftParen)
                                        {
                                            lexer.AddCacheToke(MySQLToken.KwIn);
                                            return fst;
                                        }
                                        Expr @in = RightOprandOfIn();
                                        fst = new InExpression(true, fst, @in).SetCacheEvalRst(cacheEvalRst);
                                        continue;
                                    }

                                default:
                                    {
                                        throw Err("unexpect token after NOT: " + lexer.Token());
                                    }
                            }
                            //goto case MySQLToken.KwBetween;
                        }

                    case MySQLToken.KwBetween:
                        {
                            lexer.NextToken();
                            Expr snd_1 = ComparisionExpression();
                            Match(MySQLToken.KwAnd);
                            Expr trd_1 = ComparisionExpression();
                            return new BetweenAndExpression(false, fst, snd_1, trd_1).SetCacheEvalRst(cacheEvalRst);
                        }

                    case MySQLToken.KwRlike:
                    case MySQLToken.KwRegexp:
                        {
                            lexer.NextToken();
                            temp = BitOrExpression(null, null);
                            fst = new RegexpExpression(false, fst, temp).SetCacheEvalRst(cacheEvalRst);
                            continue;
                        }

                    case MySQLToken.KwLike:
                        {
                            lexer.NextToken();
                            temp = BitOrExpression(null, null);
                            Expr escape_1 = null;
                            if (EqualsIdentifier("ESCAPE") >= 0)
                            {
                                lexer.NextToken();
                                escape_1 = BitOrExpression(null, null);
                            }
                            fst = new LikeExpression(false, fst, temp, escape_1).SetCacheEvalRst(cacheEvalRst);
                            continue;
                        }

                    case MySQLToken.KwIn:
                        {
                            if (lexer.NextToken() != MySQLToken.PuncLeftParen)
                            {
                                lexer.AddCacheToke(MySQLToken.KwIn);
                                return fst;
                            }
                            temp = RightOprandOfIn();
                            fst = new InExpression(false, fst, temp).SetCacheEvalRst(cacheEvalRst);
                            continue;
                        }

                    case MySQLToken.KwIs:
                        {
                            switch (lexer.NextToken())
                            {
                                case MySQLToken.KwNot:
                                    {
                                        switch (lexer.NextToken())
                                        {
                                            case MySQLToken.LiteralNull:
                                                {
                                                    lexer.NextToken();
                                                    fst = new ComparisionIsExpression(fst, ComparisionIsExpression.IsNotNull).SetCacheEvalRst(cacheEvalRst);
                                                    continue;
                                                }

                                            case MySQLToken.LiteralBoolFalse:
                                                {
                                                    lexer.NextToken();
                                                    fst = new ComparisionIsExpression(fst, ComparisionIsExpression.IsNotFalse).SetCacheEvalRst(cacheEvalRst);
                                                    continue;
                                                }

                                            case MySQLToken.LiteralBoolTrue:
                                                {
                                                    lexer.NextToken();
                                                    fst = new ComparisionIsExpression(fst, ComparisionIsExpression.IsNotTrue).SetCacheEvalRst(cacheEvalRst);
                                                    continue;
                                                }

                                            default:
                                                {
                                                    MatchIdentifier("UNKNOWN");
                                                    fst = new ComparisionIsExpression(fst, ComparisionIsExpression.IsNotUnknown).SetCacheEvalRst(cacheEvalRst);
                                                    continue;
                                                }
                                        }
                                        //goto case MySQLToken.LiteralNull;
                                    }

                                case MySQLToken.LiteralNull:
                                    {
                                        lexer.NextToken();
                                        fst = new ComparisionIsExpression(fst, ComparisionIsExpression.IsNull).SetCacheEvalRst(cacheEvalRst);
                                        continue;
                                    }

                                case MySQLToken.LiteralBoolFalse:
                                    {
                                        lexer.NextToken();
                                        fst = new ComparisionIsExpression(fst, ComparisionIsExpression.IsFalse).SetCacheEvalRst(cacheEvalRst);
                                        continue;
                                    }

                                case MySQLToken.LiteralBoolTrue:
                                    {
                                        lexer.NextToken();
                                        fst = new ComparisionIsExpression(fst, ComparisionIsExpression.IsTrue).SetCacheEvalRst(cacheEvalRst);
                                        continue;
                                    }

                                default:
                                    {
                                        MatchIdentifier("UNKNOWN");
                                        fst = new ComparisionIsExpression(fst, ComparisionIsExpression.IsUnknown).SetCacheEvalRst(cacheEvalRst);
                                        continue;
                                    }
                            }
                            //goto case MySQLToken.OpEquals;
                        }

                    case MySQLToken.OpEquals:
                        {
                            lexer.NextToken();
                            temp = AnyAllExpression();
                            fst = new ComparisionEqualsExpression(fst, temp).SetCacheEvalRst(cacheEvalRst);
                            continue;
                        }

                    case MySQLToken.OpNullSafeEquals:
                        {
                            lexer.NextToken();
                            temp = BitOrExpression(null, null);
                            fst = new ComparisionNullSafeEqualsExpression(fst, temp).SetCacheEvalRst(cacheEvalRst);
                            continue;
                        }

                    case MySQLToken.OpGreaterOrEquals:
                        {
                            lexer.NextToken();
                            temp = AnyAllExpression();
                            fst = new ComparisionGreaterThanOrEqualsExpression(fst, temp).SetCacheEvalRst(cacheEvalRst);
                            continue;
                        }

                    case MySQLToken.OpGreaterThan:
                        {
                            lexer.NextToken();
                            temp = AnyAllExpression();
                            fst = new ComparisionGreaterThanExpression(fst, temp).SetCacheEvalRst(cacheEvalRst);
                            continue;
                        }

                    case MySQLToken.OpLessOrEquals:
                        {
                            lexer.NextToken();
                            temp = AnyAllExpression();
                            fst = new ComparisionLessThanOrEqualsExpression(fst, temp).SetCacheEvalRst(cacheEvalRst);
                            continue;
                        }

                    case MySQLToken.OpLessThan:
                        {
                            lexer.NextToken();
                            temp = AnyAllExpression();
                            fst = new ComparisionLessThanExpression(fst, temp).SetCacheEvalRst(cacheEvalRst);
                            continue;
                        }

                    case MySQLToken.OpLessOrGreater:
                        {
                            lexer.NextToken();
                            temp = AnyAllExpression();
                            fst = new ComparisionLessOrGreaterThanExpression(fst, temp).SetCacheEvalRst(cacheEvalRst);
                            continue;
                        }

                    case MySQLToken.OpNotEquals:
                        {
                            lexer.NextToken();
                            temp = AnyAllExpression();
                            fst = new ComparisionNotEqualsExpression(fst, temp).SetCacheEvalRst(cacheEvalRst);
                            continue;
                        }

                    default:
                        {
                            if (EqualsIdentifier("SOUNDS") >= 0)
                            {
                                lexer.NextToken();
                                Match(MySQLToken.KwLike);
                                temp = BitOrExpression(null, null);
                                fst = new SoundsLikeExpression(fst, temp).SetCacheEvalRst(cacheEvalRst);
                                continue;
                            }
                            return fst;
                        }
                }
            }
        }

        /// <returns>
        ///
        /// <see cref="Tup.Cobar.Parser.Ast.Expression.Misc.QueryExpression"/>
        /// or
        /// <see cref="Tup.Cobar.Parser.Ast.Expression.Misc.InExpressionList"/>
        /// </returns>
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        private Expr RightOprandOfIn()
        {
            Match(MySQLToken.PuncLeftParen);
            if (MySQLToken.KwSelect == lexer.Token())
            {
                QueryExpression subq = SubQuery();
                Match(MySQLToken.PuncRightParen);
                return subq;
            }
            return new InExpressionList(ExpressionList(new List<Expr>())).SetCacheEvalRst(cacheEvalRst);
        }

        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        private Expr AnyAllExpression()
        {
            QueryExpression subquery = null;
            switch (lexer.Token())
            {
                case MySQLToken.KwAll:
                    {
                        lexer.NextToken();
                        Match(MySQLToken.PuncLeftParen);
                        subquery = SubQuery();
                        Match(MySQLToken.PuncRightParen);
                        return new SubqueryAllExpression(subquery).SetCacheEvalRst(cacheEvalRst);
                    }

                default:
                    {
                        int matchIndex = EqualsIdentifier("SOME", "ANY");
                        if (matchIndex < 0)
                        {
                            return BitOrExpression(null, null);
                        }
                        string consumed = lexer.StringValue();
                        string consumedUp = lexer.StringValueUppercase();
                        if (lexer.NextToken() == MySQLToken.PuncLeftParen)
                        {
                            lexer.NextToken();
                            subquery = SubQuery();
                            Match(MySQLToken.PuncRightParen);
                            return new SubqueryAnyExpression(subquery).SetCacheEvalRst(cacheEvalRst);
                        }
                        return BitOrExpression(consumed, consumedUp);
                    }
            }
        }

        /// <param name="consumed">
        /// not null means that a token that has been pre-consumed
        /// stands for next token
        /// </param>
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        private Expr BitOrExpression(string consumed, string consumedUp)
        {
            for (Expr expr = BitAndExpression(consumed, consumedUp); ;)
            {
                switch (lexer.Token())
                {
                    case MySQLToken.OpVerticalBar:
                        {
                            lexer.NextToken();
                            Expr newExpr = BitAndExpression(null, null);
                            expr = new BitOrExpression(expr, newExpr).SetCacheEvalRst(cacheEvalRst);
                            break;
                        }

                    default:
                        {
                            return expr;
                        }
                }
            }
        }

        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        private Expr BitAndExpression(string consumed, string consumedUp)
        {
            for (Expr expr = BitShiftExpression(consumed, consumedUp); ;)
            {
                switch (lexer.Token())
                {
                    case MySQLToken.OpAmpersand:
                        {
                            lexer.NextToken();
                            Expr newExpr = BitShiftExpression(null, null);
                            expr = new BitAndExpression(expr, newExpr).SetCacheEvalRst(cacheEvalRst);
                            break;
                        }

                    default:
                        {
                            return expr;
                        }
                }
            }
        }

        /// <summary><code>higherExpr ( ('&lt;&lt;'|'&gt;&gt;') higherExpr)+</code></summary>
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        private Expr BitShiftExpression(string consumed, string consumedUp)
        {
            Expr temp;
            for (Expr expr = ArithmeticTermOperatorExpression
                (consumed, consumedUp); ;)
            {
                switch (lexer.Token())
                {
                    case MySQLToken.OpLeftShift:
                        {
                            lexer.NextToken();
                            temp = ArithmeticTermOperatorExpression(null, null);
                            expr = new BitShiftExpression(false, expr, temp).SetCacheEvalRst(cacheEvalRst);
                            break;
                        }

                    case MySQLToken.OpRightShift:
                        {
                            lexer.NextToken();
                            temp = ArithmeticTermOperatorExpression(null, null);
                            expr = new BitShiftExpression(true, expr, temp).SetCacheEvalRst(cacheEvalRst);
                            break;
                        }

                    default:
                        {
                            return expr;
                        }
                }
            }
        }

        /// <summary><code>higherExpr ( ('+'|'-') higherExpr)+</code></summary>
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        private Expr ArithmeticTermOperatorExpression
            (string consumed, string consumedUp)
        {
            Expr temp;
            for (Expr expr = ArithmeticFactorOperatorExpression
                (consumed, consumedUp); ;)
            {
                switch (lexer.Token())
                {
                    case MySQLToken.OpPlus:
                        {
                            lexer.NextToken();
                            temp = ArithmeticFactorOperatorExpression(null, null);
                            expr = new ArithmeticAddExpression(expr, temp).SetCacheEvalRst(cacheEvalRst);
                            break;
                        }

                    case MySQLToken.OpMinus:
                        {
                            lexer.NextToken();
                            temp = ArithmeticFactorOperatorExpression(null, null);
                            expr = new ArithmeticSubtractExpression(expr, temp).SetCacheEvalRst(cacheEvalRst);
                            break;
                        }

                    default:
                        {
                            return expr;
                        }
                }
            }
        }

        /// <summary><code>higherExpr ( ('*'|'/'|'%'|'DIV'|'MOD') higherExpr)+</code></summary>
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        private Expr ArithmeticFactorOperatorExpression
            (string consumed, string consumedUp)
        {
            Expr temp;
            for (Expr expr = BitXORExpression(consumed,
                consumedUp); ;)
            {
                switch (lexer.Token())
                {
                    case MySQLToken.OpAsterisk:
                        {
                            lexer.NextToken();
                            temp = BitXORExpression(null, null);
                            expr = new ArithmeticMultiplyExpression(expr, temp).SetCacheEvalRst(cacheEvalRst);
                            break;
                        }

                    case MySQLToken.OpSlash:
                        {
                            lexer.NextToken();
                            temp = BitXORExpression(null, null);
                            expr = new ArithmeticDivideExpression(expr, temp).SetCacheEvalRst(cacheEvalRst);
                            break;
                        }

                    case MySQLToken.KwDiv:
                        {
                            lexer.NextToken();
                            temp = BitXORExpression(null, null);
                            expr = new ArithmeticIntegerDivideExpression(expr, temp).SetCacheEvalRst(cacheEvalRst);
                            break;
                        }

                    case MySQLToken.OpPercent:
                    case MySQLToken.KwMod:
                        {
                            lexer.NextToken();
                            temp = BitXORExpression(null, null);
                            expr = new ArithmeticModExpression(expr, temp).SetCacheEvalRst(cacheEvalRst);
                            break;
                        }

                    default:
                        {
                            return expr;
                        }
                }
            }
        }

        /// <summary><code>higherExpr ('^' higherExpr)+</code></summary>
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        private Expr BitXORExpression(string consumed, string consumedUp)
        {
            Expr temp;
            for (Expr expr = UnaryOpExpression(consumed, consumedUp); ;)
            {
                switch (lexer.Token())
                {
                    case MySQLToken.OpCaret:
                        {
                            lexer.NextToken();
                            temp = UnaryOpExpression(null, null);
                            expr = new BitXORExpression(expr, temp).SetCacheEvalRst(cacheEvalRst);
                            break;
                        }

                    default:
                        {
                            return expr;
                        }
                }
            }
        }

        /// <summary>
        /// <code>('+'|'-'|'~'|'!'|'BINARY')* higherExpr</code><br/>
        /// '!' has higher precedence
        /// </summary>
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        private Expr UnaryOpExpression(string consumed
            , string consumedUp)
        {
            if (consumed == null)
            {
                Expr expr;
                switch (lexer.Token())
                {
                    case MySQLToken.OpExclamation:
                        {
                            lexer.NextToken();
                            expr = UnaryOpExpression(null, null);
                            return new NegativeValueExpression(expr).SetCacheEvalRst(cacheEvalRst);
                        }

                    case MySQLToken.OpPlus:
                        {
                            lexer.NextToken();
                            return UnaryOpExpression(null, null);
                        }

                    case MySQLToken.OpMinus:
                        {
                            lexer.NextToken();
                            expr = UnaryOpExpression(null, null);
                            return new MinusExpression(expr).SetCacheEvalRst(cacheEvalRst);
                        }

                    case MySQLToken.OpTilde:
                        {
                            lexer.NextToken();
                            expr = UnaryOpExpression(null, null);
                            return new BitInvertExpression(expr).SetCacheEvalRst(cacheEvalRst);
                        }

                    case MySQLToken.KwBinary:
                        {
                            lexer.NextToken();
                            expr = UnaryOpExpression(null, null);
                            return new CastBinaryExpression(expr).SetCacheEvalRst(cacheEvalRst);
                        }
                }
            }
            return CollateExpression(consumed, consumedUp);
        }

        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        private Expr CollateExpression(string consumed
            , string consumedUp)
        {
            for (Expr expr = UserExpression(consumed, consumedUp
                ); ;)
            {
                if (lexer.Token() == MySQLToken.KwCollate)
                {
                    lexer.NextToken();
                    string collateName = lexer.StringValue();
                    Match(MySQLToken.Identifier);
                    expr = new CollateExpression(expr, collateName).SetCacheEvalRst(cacheEvalRst);
                    continue;
                }
                return expr;
            }
        }

        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        private Expr UserExpression(string consumed
            , string consumedUp)
        {
            Expr first = PrimaryExpression(consumed, consumedUp
                );
            if (lexer.Token() == MySQLToken.UsrVar)
            {
                if (first is LiteralString)
                {
                    StringBuilder str = new StringBuilder().Append('\'').Append(((LiteralString)first
                        ).GetString()).Append('\'').Append(lexer.StringValue());
                    lexer.NextToken();
                    return new UserExpression(str.ToString()).SetCacheEvalRst
                        (cacheEvalRst);
                }
                else
                {
                    if (first is Identifier)
                    {
                        StringBuilder str = new StringBuilder().Append(((Identifier)first).GetIdText()).Append
                            (lexer.StringValue());
                        lexer.NextToken();
                        return new UserExpression(str.ToString()).SetCacheEvalRst
                            (cacheEvalRst);
                    }
                }
            }
            return first;
        }

        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        private Expr PrimaryExpression(string consumed
            , string consumedUp)
        {
            if (consumed != null)
            {
                return StartedFromIdentifier(consumed, consumedUp);
            }
            string tempStr;
            string tempStrUp;
            StringBuilder tempSb;
            Number tempNum;
            Expr tempExpr;
            Expr tempExpr2;
            IList<Expr> tempExprList;
            switch (lexer.Token())
            {
                case MySQLToken.PlaceHolder:
                    {
                        tempStr = lexer.StringValue();
                        tempStrUp = lexer.StringValueUppercase();
                        lexer.NextToken();
                        return CreatePlaceHolder(tempStr, tempStrUp);
                    }

                case MySQLToken.LiteralBit:
                    {
                        tempStr = lexer.StringValue();
                        lexer.NextToken();
                        return new LiteralBitField(null, tempStr).SetCacheEvalRst(cacheEvalRst);
                    }

                case MySQLToken.LiteralHex:
                    {
                        LiteralHexadecimal hex = new LiteralHexadecimal(null, lexer.GetSQL(), lexer.GetOffsetCache
                            (), lexer.GetSizeCache(), charset);
                        lexer.NextToken();
                        return hex.SetCacheEvalRst(cacheEvalRst);
                    }

                case MySQLToken.LiteralBoolFalse:
                    {
                        lexer.NextToken();
                        return new LiteralBoolean(false).SetCacheEvalRst(cacheEvalRst);
                    }

                case MySQLToken.LiteralBoolTrue:
                    {
                        lexer.NextToken();
                        return new LiteralBoolean(true).SetCacheEvalRst(cacheEvalRst);
                    }

                case MySQLToken.LiteralNull:
                    {
                        lexer.NextToken();
                        return new LiteralNull().SetCacheEvalRst(cacheEvalRst);
                    }

                case MySQLToken.LiteralNchars:
                    {
                        tempSb = new StringBuilder();
                        do
                        {
                            lexer.AppendStringContent(tempSb);
                        }
                        while (lexer.NextToken() == MySQLToken.LiteralChars);
                        return new LiteralString(null, tempSb.ToString(), true).SetCacheEvalRst(cacheEvalRst
                            );
                    }

                case MySQLToken.LiteralChars:
                    {
                        tempSb = new StringBuilder();
                        do
                        {
                            lexer.AppendStringContent(tempSb);
                        }
                        while (lexer.NextToken() == MySQLToken.LiteralChars);
                        return new LiteralString(null, tempSb.ToString(), false).SetCacheEvalRst(cacheEvalRst
                            );
                    }

                case MySQLToken.LiteralNumPureDigit:
                    {
                        tempNum = lexer.IntegerValue();
                        lexer.NextToken();
                        return new LiteralNumber(tempNum).SetCacheEvalRst(cacheEvalRst);
                    }

                case MySQLToken.LiteralNumMixDigit:
                    {
                        tempNum = lexer.DecimalValue();
                        lexer.NextToken();
                        return new LiteralNumber(tempNum).SetCacheEvalRst(cacheEvalRst);
                    }

                case MySQLToken.QuestionMark:
                    {
                        int index = lexer.ParamIndex();
                        lexer.NextToken();
                        return CreateParam(index);
                    }

                case MySQLToken.KwCase:
                    {
                        lexer.NextToken();
                        return CaseWhenExpression();
                    }

                case MySQLToken.KwInterval:
                    {
                        lexer.NextToken();
                        return IntervalExpression();
                    }

                case MySQLToken.KwExists:
                    {
                        lexer.NextToken();
                        Match(MySQLToken.PuncLeftParen);
                        tempExpr = SubQuery();
                        Match(MySQLToken.PuncRightParen);
                        return new ExistsPrimary((QueryExpression)tempExpr).SetCacheEvalRst(cacheEvalRst);
                    }

                case MySQLToken.UsrVar:
                    {
                        tempStr = lexer.StringValue();
                        tempExpr = new UsrDefVarPrimary(tempStr).SetCacheEvalRst(cacheEvalRst);
                        if (lexer.NextToken() == MySQLToken.OpAssign)
                        {
                            lexer.NextToken();
                            tempExpr2 = Expression();
                            return new AssignmentExpression(tempExpr, tempExpr2);
                        }
                        return tempExpr;
                    }

                case MySQLToken.SysVar:
                    {
                        return SystemVariale();
                    }

                case MySQLToken.KwMatch:
                    {
                        lexer.NextToken();
                        return MatchExpression();
                    }

                case MySQLToken.PuncLeftParen:
                    {
                        lexer.NextToken();
                        if (lexer.Token() == MySQLToken.KwSelect)
                        {
                            tempExpr = SubQuery();
                            Match(MySQLToken.PuncRightParen);
                            return tempExpr;
                        }
                        tempExpr = Expression();
                        switch (lexer.Token())
                        {
                            case MySQLToken.PuncRightParen:
                                {
                                    lexer.NextToken();
                                    return tempExpr;
                                }

                            case MySQLToken.PuncComma:
                                {
                                    lexer.NextToken();
                                    tempExprList = new List<Expr>();
                                    tempExprList.Add(tempExpr);
                                    tempExprList = ExpressionList(tempExprList);
                                    return new RowExpression(tempExprList).SetCacheEvalRst(cacheEvalRst);
                                }

                            default:
                                {
                                    throw Err("unexpected token: " + lexer.Token());
                                }
                        }
                        //goto case MySQLToken.KwUtcDate;
                    }

                case MySQLToken.KwUtcDate:
                    {
                        lexer.NextToken();
                        if (lexer.Token() == MySQLToken.PuncLeftParen)
                        {
                            lexer.NextToken();
                            Match(MySQLToken.PuncRightParen);
                        }
                        return new UtcDate(null).SetCacheEvalRst(cacheEvalRst);
                    }

                case MySQLToken.KwUtcTime:
                    {
                        lexer.NextToken();
                        if (lexer.Token() == MySQLToken.PuncLeftParen)
                        {
                            lexer.NextToken();
                            Match(MySQLToken.PuncRightParen);
                        }
                        return new UtcTime(null).SetCacheEvalRst(cacheEvalRst);
                    }

                case MySQLToken.KwUtcTimestamp:
                    {
                        lexer.NextToken();
                        if (lexer.Token() == MySQLToken.PuncLeftParen)
                        {
                            lexer.NextToken();
                            Match(MySQLToken.PuncRightParen);
                        }
                        return new UtcTimestamp(null).SetCacheEvalRst(cacheEvalRst);
                    }

                case MySQLToken.KwCurrentDate:
                    {
                        lexer.NextToken();
                        if (lexer.Token() == MySQLToken.PuncLeftParen)
                        {
                            lexer.NextToken();
                            Match(MySQLToken.PuncRightParen);
                        }
                        return new Curdate().SetCacheEvalRst(cacheEvalRst);
                    }

                case MySQLToken.KwCurrentTime:
                    {
                        lexer.NextToken();
                        if (lexer.Token() == MySQLToken.PuncLeftParen)
                        {
                            lexer.NextToken();
                            Match(MySQLToken.PuncRightParen);
                        }
                        return new Curtime().SetCacheEvalRst(cacheEvalRst);
                    }

                case MySQLToken.KwCurrentTimestamp:
                case MySQLToken.KwLocaltime:
                case MySQLToken.KwLocaltimestamp:
                    {
                        lexer.NextToken();
                        if (lexer.Token() == MySQLToken.PuncLeftParen)
                        {
                            lexer.NextToken();
                            Match(MySQLToken.PuncRightParen);
                        }
                        return new Now().SetCacheEvalRst(cacheEvalRst);
                    }

                case MySQLToken.KwCurrentUser:
                    {
                        lexer.NextToken();
                        if (lexer.Token() == MySQLToken.PuncLeftParen)
                        {
                            lexer.NextToken();
                            Match(MySQLToken.PuncRightParen);
                        }
                        return new CurrentUser().SetCacheEvalRst(cacheEvalRst);
                    }

                case MySQLToken.KwDefault:
                    {
                        if (lexer.NextToken() == MySQLToken.PuncLeftParen)
                        {
                            return OrdinaryFunction(lexer.StringValue(), lexer.StringValueUppercase());
                        }
                        return new DefaultValue().SetCacheEvalRst(cacheEvalRst);
                    }

                case MySQLToken.KwDatabase:
                case MySQLToken.KwIf:
                case MySQLToken.KwInsert:
                case MySQLToken.KwLeft:
                case MySQLToken.KwRepeat:
                case MySQLToken.KwReplace:
                case MySQLToken.KwRight:
                case MySQLToken.KwSchema:
                case MySQLToken.KwValues:
                    {
                        tempStr = lexer.StringValue();
                        tempStrUp = lexer.StringValueUppercase();
                        string tempStrUp2 = MySQLTokenUtils.KeyWordToString(lexer.Token());
                        if (!tempStrUp2.Equals(tempStrUp))
                        {
                            tempStrUp = tempStr = tempStrUp2;
                        }
                        if (lexer.NextToken() == MySQLToken.PuncLeftParen)
                        {
                            return OrdinaryFunction(tempStr, tempStrUp);
                        }
                        throw Err("keyword not followed by '(' is not expression: " + tempStr);
                    }

                case MySQLToken.KwMod:
                    {
                        lexer.NextToken();
                        Match(MySQLToken.PuncLeftParen);
                        tempExpr = Expression();
                        Match(MySQLToken.PuncComma);
                        tempExpr2 = Expression();
                        Match(MySQLToken.PuncRightParen);
                        return new ArithmeticModExpression(tempExpr, tempExpr2).SetCacheEvalRst(cacheEvalRst);
                    }

                case MySQLToken.KwChar:
                    {
                        lexer.NextToken();
                        Match(MySQLToken.PuncLeftParen);
                        return FunctionChar();
                    }

                case MySQLToken.KwConvert:
                    {
                        lexer.NextToken();
                        Match(MySQLToken.PuncLeftParen);
                        return FunctionConvert();
                    }

                case MySQLToken.Identifier:
                    {
                        tempStr = lexer.StringValue();
                        tempStrUp = lexer.StringValueUppercase();
                        lexer.NextToken();
                        return StartedFromIdentifier(tempStr, tempStrUp);
                    }

                case MySQLToken.OpAsterisk:
                    {
                        lexer.NextToken();
                        return new Wildcard(null).SetCacheEvalRst(cacheEvalRst);
                    }

                default:
                    {
                        throw Err("unrecognized token as first token of primary: " + lexer.Token());
                    }
            }
        }

        /// <summary>first '(' has been consumed</summary>
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        private Timestampdiff Timestampdiff()
        {
            IntervalPrimary.Unit unit = IntervalPrimaryUnit();
            Match(MySQLToken.PuncComma);
            Expr interval = Expression();
            Match(MySQLToken.PuncComma);
            Expr expr = Expression();
            Match(MySQLToken.PuncRightParen);
            IList<Expr> argument = new List<Expr>(2);
            argument.Add(interval);
            argument.Add(expr);
            Timestampdiff func = new Timestampdiff(unit, argument);
            func.SetCacheEvalRst(cacheEvalRst);
            return func;
        }

        /// <summary>first '(' has been consumed</summary>
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        private Timestampadd Timestampadd()
        {
            IntervalPrimary.Unit unit = IntervalPrimaryUnit();
            Match(MySQLToken.PuncComma);
            Expr interval = Expression();
            Match(MySQLToken.PuncComma);
            Expr expr = Expression();
            Match(MySQLToken.PuncRightParen);
            IList<Expr> argument = new List<Expr>(2);
            argument.Add(interval);
            argument.Add(expr);
            Timestampadd func = new Timestampadd(unit, argument);
            func.SetCacheEvalRst(cacheEvalRst);
            return func;
        }

        /// <summary>first '(' has been consumed</summary>
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        private Extract Extract
            ()
        {
            IntervalPrimary.Unit unit = IntervalPrimaryUnit();
            Match(MySQLToken.KwFrom);
            Expr date = Expression();
            Match(MySQLToken.PuncRightParen);
            Extract extract = new Extract(unit, date);
            extract.SetCacheEvalRst(cacheEvalRst);
            return extract;
        }

        /// <summary>first '(' has been consumed</summary>
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        private Tup.Cobar.Parser.Ast.Expression.Primary.Function.Cast.Convert FunctionConvert()
        {
            Expr expr = Expression();
            Match(MySQLToken.KwUsing);
            string tempStr = lexer.StringValue();
            Match(MySQLToken.Identifier);
            Match(MySQLToken.PuncRightParen);
            var cvt = new Tup.Cobar.Parser.Ast.Expression.Primary.Function.Cast.Convert(expr, tempStr);
            cvt.SetCacheEvalRst(cacheEvalRst);
            return cvt;
        }

        /// <summary>first '(' has been consumed</summary>
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        private Ast.Expression.Primary.Function.String.Char FunctionChar()
        {
            Ast.Expression.Primary.Function.String.Char chr;
            for (IList<Expr> tempExprList = new List<Expr>(); ;)
            {
                Expr tempExpr = Expression();
                tempExprList.Add(tempExpr);
                switch (lexer.Token())
                {
                    case MySQLToken.PuncComma:
                        {
                            lexer.NextToken();
                            continue;
                        }

                    case MySQLToken.PuncRightParen:
                        {
                            lexer.NextToken();
                            chr = new Ast.Expression.Primary.Function.String.Char(tempExprList, null);
                            chr.SetCacheEvalRst(cacheEvalRst);
                            return chr;
                        }

                    case MySQLToken.KwUsing:
                        {
                            lexer.NextToken();
                            string tempStr = lexer.StringValue();
                            Match(MySQLToken.Identifier);
                            Match(MySQLToken.PuncRightParen);
                            chr = new Ast.Expression.Primary.Function.String.Char(tempExprList, tempStr);
                            chr.SetCacheEvalRst(cacheEvalRst);
                            return chr;
                        }

                    default:
                        {
                            throw Err("expect ',' or 'USING' or ')' but is " + lexer.Token());
                        }
                }
            }
        }

        /// <summary>
        /// last token consumed is
        /// <see cref="Tup.Cobar.Parser.Recognizer.Mysql.MySQLToken.Identifier"/>
        /// , MUST NOT be
        /// <code>null</code>
        /// </summary>
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        private Expr StartedFromIdentifier(string consumed, string consumedUp)
        {
            Expr tempExpr;
            Expr tempExpr2;
            IList<Expr> tempExprList;
            string tempStr;
            StringBuilder tempSb;
            bool tempGroupDistinct;
            switch (lexer.Token())
            {
                case MySQLToken.PuncDot:
                    {
                        for (tempExpr = new Identifier(null, consumed, consumedUp).SetCacheEvalRst(cacheEvalRst); lexer.Token() == MySQLToken.PuncDot;)
                        {
                            switch (lexer.NextToken())
                            {
                                case MySQLToken.Identifier:
                                    {
                                        tempExpr = new Identifier((Identifier)tempExpr, lexer.StringValue(), lexer.StringValueUppercase()).SetCacheEvalRst(cacheEvalRst);
                                        lexer.NextToken();
                                        break;
                                    }

                                case MySQLToken.OpAsterisk:
                                    {
                                        lexer.NextToken();
                                        return new Wildcard((Identifier)tempExpr).SetCacheEvalRst(cacheEvalRst);
                                    }

                                default:
                                    {
                                        throw Err("expect IDENTIFIER or '*' after '.', but is " + lexer.Token());
                                    }
                            }
                        }
                        return tempExpr;
                    }

                case MySQLToken.LiteralBit:
                    {
                        if (consumed[0] != '_')
                        {
                            return new Identifier(null, consumed, consumedUp).SetCacheEvalRst(cacheEvalRst);
                        }
                        tempStr = lexer.StringValue();
                        lexer.NextToken();
                        return new LiteralBitField(consumed, tempStr).SetCacheEvalRst(cacheEvalRst);
                    }

                case MySQLToken.LiteralHex:
                    {
                        if (consumed[0] != '_')
                        {
                            return new Identifier(null, consumed, consumedUp).SetCacheEvalRst(cacheEvalRst);
                        }
                        LiteralHexadecimal hex = new LiteralHexadecimal(consumed, lexer.GetSQL(), lexer.GetOffsetCache
                            (), lexer.GetSizeCache(), charset);
                        lexer.NextToken();
                        return hex.SetCacheEvalRst(cacheEvalRst);
                    }

                case MySQLToken.LiteralChars:
                    {
                        if (consumed[0] != '_')
                        {
                            return new Identifier(null, consumed, consumedUp).SetCacheEvalRst(cacheEvalRst);
                        }
                        tempSb = new StringBuilder();
                        do
                        {
                            lexer.AppendStringContent(tempSb);
                        }
                        while (lexer.NextToken() == MySQLToken.LiteralChars);
                        return new LiteralString(consumed, tempSb.ToString(), false).SetCacheEvalRst(cacheEvalRst);
                    }

                case MySQLToken.PuncLeftParen:
                    {
                        consumedUp = IdentifierExpr.UnescapeName(consumedUp);
                        switch (functionManager.GetParsingStrategy(consumedUp))
                        {
                            case MySQLFunctionManager.FunctionParsingStrategy.GetFormat:
                                {
                                    // GET_FORMAT({DATE|TIME|DATETIME},
                                    // {'EUR'|'USA'|'JIS'|'ISO'|'INTERNAL'})
                                    lexer.NextToken();
                                    int gfi = MatchIdentifier("DATE", "TIME", "DATETIME", "TIMESTAMP");
                                    Match(MySQLToken.PuncComma);
                                    Expr getFormatArg = Expression();
                                    Match(MySQLToken.PuncRightParen);
                                    switch (gfi)
                                    {
                                        case 0:
                                            {
                                                return new GetFormat(GetFormat.FormatType.Date, getFormatArg);
                                            }

                                        case 1:
                                            {
                                                return new GetFormat(GetFormat.FormatType.Time, getFormatArg);
                                            }

                                        case 2:
                                        case 3:
                                            {
                                                return new GetFormat(GetFormat.FormatType.Datetime, getFormatArg);
                                            }
                                    }
                                    throw Err("unexpected format type for GET_FORMAT()");
                                }

                            case MySQLFunctionManager.FunctionParsingStrategy.Cast:
                                {
                                    lexer.NextToken();
                                    tempExpr = Expression();
                                    Match(MySQLToken.KwAs);
                                    Pair<string, Pair<Expr, Expr>> type = Type4specialFunc();
                                    Match(MySQLToken.PuncRightParen);
                                    Pair<Expr, Expr> info = type.GetValue();
                                    if (info != null)
                                    {
                                        return new Cast(tempExpr, type.GetKey(), info.GetKey(), info.GetValue()).SetCacheEvalRst(cacheEvalRst);
                                    }
                                    else
                                    {
                                        return new Cast(tempExpr, type.GetKey(), null, null).SetCacheEvalRst(cacheEvalRst);
                                    }
                                    //goto case MySQLFunctionManager.FunctionParsingStrategy.Position;
                                }

                            case MySQLFunctionManager.FunctionParsingStrategy.Position:
                                {
                                    lexer.NextToken();
                                    tempExprList = new List<Expr>(2);
                                    tempExprList.Add(Expression());
                                    Match(MySQLToken.KwIn);
                                    tempExprList.Add(Expression());
                                    Match(MySQLToken.PuncRightParen);
                                    return new Locate(tempExprList).SetCacheEvalRst(cacheEvalRst);
                                }

                            case MySQLFunctionManager.FunctionParsingStrategy.Substring:
                                {
                                    lexer.NextToken();
                                    tempExprList = new List<Expr>(3);
                                    tempExprList.Add(Expression());
                                    Match(MySQLToken.PuncComma, MySQLToken.KwFrom);
                                    tempExprList.Add(Expression());
                                    switch (lexer.Token())
                                    {
                                        case MySQLToken.PuncComma:
                                        case MySQLToken.KwFor:
                                            {
                                                lexer.NextToken();
                                                tempExprList.Add(Expression());
                                                goto default;
                                            }

                                        default:
                                            {
                                                Match(MySQLToken.PuncRightParen);
                                                break;
                                            }
                                    }
                                    return new Substring(tempExprList).SetCacheEvalRst(cacheEvalRst);
                                }

                            case MySQLFunctionManager.FunctionParsingStrategy.Row:
                                {
                                    lexer.NextToken();
                                    tempExprList = ExpressionList(new List<Expr>());
                                    return new RowExpression(tempExprList).SetCacheEvalRst(cacheEvalRst);
                                }

                            case MySQLFunctionManager.FunctionParsingStrategy.Trim:
                                {
                                    Trim.Direction direction;
                                    switch (lexer.NextToken())
                                    {
                                        case MySQLToken.KwBoth:
                                            {
                                                lexer.NextToken();
                                                direction = Trim.Direction.Both;
                                                break;
                                            }

                                        case MySQLToken.KwLeading:
                                            {
                                                lexer.NextToken();
                                                direction = Trim.Direction.Leading;
                                                break;
                                            }

                                        case MySQLToken.KwTrailing:
                                            {
                                                lexer.NextToken();
                                                direction = Trim.Direction.Trailing;
                                                break;
                                            }

                                        default:
                                            {
                                                direction = Trim.Direction.Default;
                                                break;
                                            }
                                    }
                                    if (direction == Trim.Direction.Default)
                                    {
                                        tempExpr = Expression();
                                        if (lexer.Token() == MySQLToken.KwFrom)
                                        {
                                            lexer.NextToken();
                                            tempExpr2 = Expression();
                                            Match(MySQLToken.PuncRightParen);
                                            return new Trim(direction, tempExpr, tempExpr2).SetCacheEvalRst(cacheEvalRst);
                                        }
                                        Match(MySQLToken.PuncRightParen);
                                        return new Trim(direction, null, tempExpr).SetCacheEvalRst(cacheEvalRst);
                                    }
                                    if (lexer.Token() == MySQLToken.KwFrom)
                                    {
                                        lexer.NextToken();
                                        tempExpr = Expression();
                                        Match(MySQLToken.PuncRightParen);
                                        return new Trim(direction, null, tempExpr).SetCacheEvalRst(cacheEvalRst);
                                    }
                                    tempExpr = Expression();
                                    Match(MySQLToken.KwFrom);
                                    tempExpr2 = Expression();
                                    Match(MySQLToken.PuncRightParen);
                                    return new Trim(direction, tempExpr, tempExpr2).SetCacheEvalRst(cacheEvalRst);
                                }

                            case MySQLFunctionManager.FunctionParsingStrategy.Avg:
                                {
                                    if (lexer.NextToken() == MySQLToken.KwDistinct)
                                    {
                                        tempGroupDistinct = true;
                                        lexer.NextToken();
                                    }
                                    else
                                    {
                                        tempGroupDistinct = false;
                                    }
                                    tempExpr = Expression();
                                    Match(MySQLToken.PuncRightParen);
                                    return new Avg(tempExpr, tempGroupDistinct).SetCacheEvalRst(cacheEvalRst);
                                }

                            case MySQLFunctionManager.FunctionParsingStrategy.Max:
                                {
                                    if (lexer.NextToken() == MySQLToken.KwDistinct)
                                    {
                                        tempGroupDistinct = true;
                                        lexer.NextToken();
                                    }
                                    else
                                    {
                                        tempGroupDistinct = false;
                                    }
                                    tempExpr = Expression();
                                    Match(MySQLToken.PuncRightParen);
                                    return new Max(tempExpr, tempGroupDistinct).SetCacheEvalRst(cacheEvalRst);
                                }

                            case MySQLFunctionManager.FunctionParsingStrategy.Min:
                                {
                                    if (lexer.NextToken() == MySQLToken.KwDistinct)
                                    {
                                        tempGroupDistinct = true;
                                        lexer.NextToken();
                                    }
                                    else
                                    {
                                        tempGroupDistinct = false;
                                    }
                                    tempExpr = Expression();
                                    Match(MySQLToken.PuncRightParen);
                                    return new Min(tempExpr, tempGroupDistinct).SetCacheEvalRst(cacheEvalRst);
                                }

                            case MySQLFunctionManager.FunctionParsingStrategy.Sum:
                                {
                                    if (lexer.NextToken() == MySQLToken.KwDistinct)
                                    {
                                        tempGroupDistinct = true;
                                        lexer.NextToken();
                                    }
                                    else
                                    {
                                        tempGroupDistinct = false;
                                    }
                                    tempExpr = Expression();
                                    Match(MySQLToken.PuncRightParen);
                                    return new Sum(tempExpr, tempGroupDistinct).SetCacheEvalRst(cacheEvalRst);
                                }

                            case MySQLFunctionManager.FunctionParsingStrategy.Count:
                                {
                                    if (lexer.NextToken() == MySQLToken.KwDistinct)
                                    {
                                        lexer.NextToken();
                                        tempExprList = ExpressionList(new List<Expr>());
                                        return new Count(tempExprList).SetCacheEvalRst(cacheEvalRst);
                                    }
                                    tempExpr = Expression();
                                    Match(MySQLToken.PuncRightParen);
                                    return new Count(tempExpr).SetCacheEvalRst(cacheEvalRst);
                                }

                            case MySQLFunctionManager.FunctionParsingStrategy.GroupConcat:
                                {
                                    if (lexer.NextToken() == MySQLToken.KwDistinct)
                                    {
                                        lexer.NextToken();
                                        tempGroupDistinct = true;
                                    }
                                    else
                                    {
                                        tempGroupDistinct = false;
                                    }
                                    for (tempExprList = new List<Expr>(); ;)
                                    {
                                        tempExpr = Expression();
                                        tempExprList.Add(tempExpr);
                                        if (lexer.Token() == MySQLToken.PuncComma)
                                        {
                                            lexer.NextToken();
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                    bool isDesc = false;
                                    IList<Expr> appendedColumnNames = null;
                                    tempExpr = null;
                                    // order by
                                    tempStr = null;
                                    switch (lexer.Token())
                                    {
                                        case MySQLToken.KwOrder:
                                            {
                                                // literalChars
                                                lexer.NextToken();
                                                Match(MySQLToken.KwBy);
                                                tempExpr = Expression();
                                                if (lexer.Token() == MySQLToken.KwAsc)
                                                {
                                                    lexer.NextToken();
                                                }
                                                else
                                                {
                                                    if (lexer.Token() == MySQLToken.KwDesc)
                                                    {
                                                        isDesc = true;
                                                        lexer.NextToken();
                                                    }
                                                }
                                                for (appendedColumnNames = new List<Expr>()
                                                    ; lexer.Token() == MySQLToken.PuncComma;)
                                                {
                                                    lexer.NextToken();
                                                    appendedColumnNames.Add(Expression());
                                                }
                                                if (lexer.Token() != MySQLToken.KwSeparator)
                                                {
                                                    break;
                                                }
                                                goto case MySQLToken.KwSeparator;
                                            }

                                        case MySQLToken.KwSeparator:
                                            {
                                                lexer.NextToken();
                                                tempSb = new StringBuilder();
                                                lexer.AppendStringContent(tempSb);
                                                tempStr = LiteralString.GetUnescapedString(tempSb.ToString());
                                                Match(MySQLToken.LiteralChars);
                                                break;
                                            }
                                    }
                                    Match(MySQLToken.PuncRightParen);
                                    return new GroupConcat(tempGroupDistinct, tempExprList, tempExpr, isDesc, appendedColumnNames, tempStr).SetCacheEvalRst(cacheEvalRst);
                                }

                            case MySQLFunctionManager.FunctionParsingStrategy.Char:
                                {
                                    lexer.NextToken();
                                    return FunctionChar();
                                }

                            case MySQLFunctionManager.FunctionParsingStrategy.Convert:
                                {
                                    lexer.NextToken();
                                    return FunctionConvert();
                                }

                            case MySQLFunctionManager.FunctionParsingStrategy.Extract:
                                {
                                    lexer.NextToken();
                                    return Extract();
                                }

                            case MySQLFunctionManager.FunctionParsingStrategy.Timestampdiff:
                                {
                                    lexer.NextToken();
                                    return Timestampdiff();
                                }

                            case MySQLFunctionManager.FunctionParsingStrategy.Timestampadd:
                                {
                                    lexer.NextToken();
                                    return Timestampadd();
                                }

                            case MySQLFunctionManager.FunctionParsingStrategy.Ordinary:
                                {
                                    return OrdinaryFunction(consumed, consumedUp);
                                }

                            case MySQLFunctionManager.FunctionParsingStrategy.Default:
                                {
                                    return new Identifier(null, consumed, consumedUp).SetCacheEvalRst(cacheEvalRst);
                                }

                            default:
                                {
                                    throw Err("unexpected function parsing strategy for id of " + consumed);
                                }
                        }
                        //goto default;
                    }

                default:
                    {
                        return new Identifier(null, consumed, consumedUp).SetCacheEvalRst(cacheEvalRst);
                    }
            }
        }

        /// <returns>never null</returns>
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        private Pair<string, Pair<Expr, Expr
            >> Type4specialFunc()
        {
            Expr exp1 = null;
            Expr exp2 = null;
            // DATE
            // DATETIME
            // SIGNED [INTEGER]
            // TIME
            string typeName;
            switch (lexer.Token())
            {
                case MySQLToken.KwBinary:
                case MySQLToken.KwChar:
                    {
                        typeName = MySQLTokenUtils.KeyWordToString(lexer.Token());
                        if (lexer.NextToken() == MySQLToken.PuncLeftParen)
                        {
                            lexer.NextToken();
                            exp1 = Expression();
                            Match(MySQLToken.PuncRightParen);
                        }
                        return ConstructTypePair(typeName, exp1, exp2);
                    }

                case MySQLToken.KwDecimal:
                    {
                        typeName = MySQLTokenUtils.KeyWordToString(lexer.Token());
                        if (lexer.NextToken() == MySQLToken.PuncLeftParen)
                        {
                            lexer.NextToken();
                            exp1 = Expression();
                            if (lexer.Token() == MySQLToken.PuncComma)
                            {
                                lexer.NextToken();
                                exp2 = Expression();
                            }
                            Match(MySQLToken.PuncRightParen);
                        }
                        return ConstructTypePair(typeName, exp1, exp2);
                    }

                case MySQLToken.KwUnsigned:
                    {
                        typeName = MySQLTokenUtils.KeyWordToString(lexer.Token());
                        if (lexer.NextToken() == MySQLToken.KwInteger)
                        {
                            lexer.NextToken();
                        }
                        return ConstructTypePair(typeName, null, null);
                    }

                case MySQLToken.Identifier:
                    {
                        typeName = lexer.StringValueUppercase();
                        lexer.NextToken();
                        if ("SIGNED".Equals(typeName))
                        {
                            if (lexer.Token() == MySQLToken.KwInteger)
                            {
                                lexer.NextToken();
                            }
                        }
                        else
                        {
                            if (!"DATE".Equals(typeName) && !"DATETIME".Equals(typeName) && !"TIME".Equals(typeName))
                            {
                                throw Err("invalide type name: " + typeName);
                            }
                        }
                        return ConstructTypePair(typeName, null, null);
                    }

                default:
                    {
                        throw Err("invalide type name: " + lexer.StringValueUppercase());
                    }
            }
        }

        private static Pair<string, Pair<Expr, Expr>> ConstructTypePair(string typeName, Expr exp1, Expr exp2)
        {
            return new Pair<string, Pair<Expr, Expr>>(typeName, new Pair<Expr, Expr>(exp1, exp2));
        }

        /// <summary>id has been consumed.</summary>
        /// <remarks>
        /// id has been consumed. id must be a function name. current token must be
        /// <see cref="Tup.Cobar.Parser.Recognizer.Mysql.MySQLToken.PuncLeftParen"/>
        /// </remarks>
        /// <param name="idUpper">must be name of a function</param>
        /// <returns>never null</returns>
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        private FunctionExpression OrdinaryFunction(string id, string idUpper)
        {
            idUpper = IdentifierExpr.UnescapeName(idUpper);
            Match(MySQLToken.PuncLeftParen);
            FunctionExpression funcExpr;
            if (lexer.Token() == MySQLToken.PuncRightParen)
            {
                lexer.NextToken();
                funcExpr = functionManager.CreateFunctionExpression(idUpper, null);
            }
            else
            {
                IList<Expr> args = ExpressionList(new List<Expr>());
                funcExpr = functionManager.CreateFunctionExpression(idUpper, args);
            }
            if (funcExpr == null)
            {
                throw new SQLSyntaxErrorException(id + "() is not a function");
            }
            funcExpr.SetCacheEvalRst(cacheEvalRst);
            return funcExpr;
        }

        /// <summary>first <code>MATCH</code> has been consumed</summary>
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        private Expr MatchExpression()
        {
            Match(MySQLToken.PuncLeftParen);
            IList<Expr> colList = ExpressionList(new List<Expr>());
            MatchIdentifier("AGAINST");
            Match(MySQLToken.PuncLeftParen);
            Expr pattern = Expression();
            MatchExpr.Modifier modifier = MatchExpr.Modifier.Default;
            switch (lexer.Token())
            {
                case MySQLToken.KwWith:
                    {
                        lexer.NextToken();
                        Match(MySQLToken.Identifier);
                        Match(MySQLToken.Identifier);
                        modifier = MatchExpr.Modifier.WithQueryExpansion;
                        break;
                    }

                case MySQLToken.KwIn:
                    {
                        switch (lexer.NextToken())
                        {
                            case MySQLToken.KwNatural:
                                {
                                    lexer.NextToken();
                                    MatchIdentifier("LANGUAGE");
                                    MatchIdentifier("MODE");
                                    if (lexer.Token() == MySQLToken.KwWith)
                                    {
                                        lexer.NextToken();
                                        lexer.NextToken();
                                        lexer.NextToken();
                                        modifier = MatchExpr.Modifier.InNaturalLanguageModeWithQueryExpansion;
                                    }
                                    else
                                    {
                                        modifier = MatchExpr.Modifier.InNaturalLanguageMode;
                                    }
                                    break;
                                }

                            default:
                                {
                                    MatchIdentifier("BOOLEAN");
                                    MatchIdentifier("MODE");
                                    modifier = MatchExpr.Modifier.InBooleanMode;
                                    break;
                                }
                        }
                        break;
                    }
            }
            Match(MySQLToken.PuncRightParen);
            return new MatchExpr(colList, pattern, modifier).SetCacheEvalRst(cacheEvalRst);
        }

        /// <summary>first <code>INTERVAL</code> has been consumed</summary>
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        private Expr IntervalExpression()
        {
            Expr fstExpr;
            IList<Expr> argList = null;
            if (lexer.Token() == MySQLToken.PuncLeftParen)
            {
                if (lexer.NextToken() == MySQLToken.KwSelect)
                {
                    fstExpr = SubQuery();
                    Match(MySQLToken.PuncRightParen);
                }
                else
                {
                    fstExpr = Expression();
                    if (lexer.Token() == MySQLToken.PuncComma)
                    {
                        lexer.NextToken();
                        argList = new List<Expr>();
                        argList.Add(fstExpr);
                        argList = ExpressionList(argList);
                    }
                    else
                    {
                        Match(MySQLToken.PuncRightParen);
                    }
                }
            }
            else
            {
                fstExpr = Expression();
            }
            if (argList != null)
            {
                return new Interval(argList).SetCacheEvalRst(cacheEvalRst);
            }
            return new IntervalPrimary(fstExpr, IntervalPrimaryUnit()).SetCacheEvalRst(cacheEvalRst
                );
        }

        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        private IntervalPrimary.Unit IntervalPrimaryUnit()
        {
            switch (lexer.Token())
            {
                case MySQLToken.KwSecondMicrosecond:
                    {
                        lexer.NextToken();
                        return IntervalPrimary.Unit.SecondMicrosecond;
                    }

                case MySQLToken.KwMinuteMicrosecond:
                    {
                        lexer.NextToken();
                        return IntervalPrimary.Unit.MinuteMicrosecond;
                    }

                case MySQLToken.KwMinuteSecond:
                    {
                        lexer.NextToken();
                        return IntervalPrimary.Unit.MinuteSecond;
                    }

                case MySQLToken.KwHourMicrosecond:
                    {
                        lexer.NextToken();
                        return IntervalPrimary.Unit.HourMicrosecond;
                    }

                case MySQLToken.KwHourSecond:
                    {
                        lexer.NextToken();
                        return IntervalPrimary.Unit.HourSecond;
                    }

                case MySQLToken.KwHourMinute:
                    {
                        lexer.NextToken();
                        return IntervalPrimary.Unit.HourMinute;
                    }

                case MySQLToken.KwDayMicrosecond:
                    {
                        lexer.NextToken();
                        return IntervalPrimary.Unit.DayMicrosecond;
                    }

                case MySQLToken.KwDaySecond:
                    {
                        lexer.NextToken();
                        return IntervalPrimary.Unit.DaySecond;
                    }

                case MySQLToken.KwDayMinute:
                    {
                        lexer.NextToken();
                        return IntervalPrimary.Unit.DayMinute;
                    }

                case MySQLToken.KwDayHour:
                    {
                        lexer.NextToken();
                        return IntervalPrimary.Unit.DayHour;
                    }

                case MySQLToken.KwYearMonth:
                    {
                        lexer.NextToken();
                        return IntervalPrimary.Unit.YearMonth;
                    }

                case MySQLToken.Identifier:
                    {
                        string unitText = lexer.StringValueUppercase();
                        IntervalPrimary.Unit unit = IntervalPrimary.GetIntervalUnit(unitText);
                        if (unit != IntervalPrimary.Unit.None)
                        {
                            lexer.NextToken();
                            return unit;
                        }
                        goto default;
                    }

                default:
                    {
                        throw Err("literal INTERVAL should end with an UNIT");
                    }
            }
        }

        /// <summary>first <code>CASE</code> has been consumed</summary>
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        private Expr CaseWhenExpression()
        {
            Expr comparee = null;
            if (lexer.Token() != MySQLToken.KwWhen)
            {
                comparee = Expression();
            }
            IList<Pair<Expr, Expr>> list = new List<Pair<Expr, Expr>>();
            for (; lexer.Token() == MySQLToken.KwWhen;)
            {
                lexer.NextToken();
                Expr when = Expression();
                Match(MySQLToken.KwThen);
                Expr then = Expression();
                if (when == null || then == null)
                {
                    throw Err("when or then is null in CASE WHEN expression");
                }
                list.Add(new Pair<Expr, Expr>(when, then));
            }
            if (list.IsEmpty())
            {
                throw Err("at least one WHEN ... THEN branch for CASE ... WHEN syntax");
            }
            Expr elseValue = null;
            switch (lexer.Token())
            {
                case MySQLToken.KwElse:
                    {
                        lexer.NextToken();
                        elseValue = Expression();
                        goto default;
                    }

                default:
                    {
                        MatchIdentifier("END");
                        break;
                    }
            }
            return new CaseWhenOperatorExpression(comparee, list, elseValue).SetCacheEvalRst(
                cacheEvalRst);
        }

        /// <summary>first <code>'('</code> has been consumed.</summary>
        /// <remarks>
        /// first <code>'('</code> has been consumed. At least one element. Consume
        /// last ')' after invocation <br/>
        /// <code>'(' expr (',' expr)* ')'</code>
        /// </remarks>
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        private IList<Expr> ExpressionList(IList<Expr> exprList)
        {
            for (;;)
            {
                Expr expr = Expression();
                exprList.Add(expr);
                switch (lexer.Token())
                {
                    case MySQLToken.PuncComma:
                        {
                            lexer.NextToken();
                            break;
                        }

                    case MySQLToken.PuncRightParen:
                        {
                            lexer.NextToken();
                            return exprList;
                        }

                    default:
                        {
                            throw Err("unexpected token: " + lexer.Token());
                        }
                }
            }
        }

        /// <summary>
        /// first token is
        /// <see cref="Tup.Cobar.Parser.Recognizer.Mysql.MySQLToken.KwSelect"/>
        /// </summary>
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        private QueryExpression SubQuery()
        {
            if (selectParser == null)
            {
                selectParser = new MySQLDMLSelectParser(lexer, this);
            }
            return selectParser.Select();
        }
    }
}