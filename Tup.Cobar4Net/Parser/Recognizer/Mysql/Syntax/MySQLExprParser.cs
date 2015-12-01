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
using Tup.Cobar4Net.Parser.Ast.Expression;
using Tup.Cobar4Net.Parser.Ast.Expression.Arithmeic;
using Tup.Cobar4Net.Parser.Ast.Expression.Bit;
using Tup.Cobar4Net.Parser.Ast.Expression.Comparison;
using Tup.Cobar4Net.Parser.Ast.Expression.Logical;
using Tup.Cobar4Net.Parser.Ast.Expression.Misc;
using Tup.Cobar4Net.Parser.Ast.Expression.Primary;
using Tup.Cobar4Net.Parser.Ast.Expression.Primary.Function;
using Tup.Cobar4Net.Parser.Ast.Expression.Primary.Function.Cast;
using Tup.Cobar4Net.Parser.Ast.Expression.Primary.Function.Comparison;
using Tup.Cobar4Net.Parser.Ast.Expression.Primary.Function.Datetime;
using Tup.Cobar4Net.Parser.Ast.Expression.Primary.Function.Groupby;
using Tup.Cobar4Net.Parser.Ast.Expression.Primary.Function.Info;
using Tup.Cobar4Net.Parser.Ast.Expression.Primary.Function.String;
using Tup.Cobar4Net.Parser.Ast.Expression.Primary.Literal;
using Tup.Cobar4Net.Parser.Ast.Expression.String;
using Tup.Cobar4Net.Parser.Ast.Expression.Type;
using Tup.Cobar4Net.Parser.Recognizer.Mysql.Lexer;
using Tup.Cobar4Net.Parser.Util;
using Char = Tup.Cobar4Net.Parser.Ast.Expression.Primary.Function.String.Char;
using Convert = Tup.Cobar4Net.Parser.Ast.Expression.Primary.Function.Cast.Convert;
using IdentifierExpr = Tup.Cobar4Net.Parser.Ast.Expression.Primary.Identifier;
using MatchExpr = Tup.Cobar4Net.Parser.Ast.Expression.Primary.MatchExpression;

namespace Tup.Cobar4Net.Parser.Recognizer.Mysql.Syntax
{
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public class MySqlExprParser : MySqlParser
    {
        private readonly string charset;
        private readonly MySqlFunctionManager functionManager;

        private MySqlDmlSelectParser selectParser;

        public MySqlExprParser(MySqlLexer lexer)
            : this(lexer, MySqlFunctionManager.InstanceMysqlDefault, true, DefaultCharset)
        {
        }

        public MySqlExprParser(MySqlLexer lexer, string charset)
            : this(lexer, MySqlFunctionManager.InstanceMysqlDefault, true, charset)
        {
        }

        public MySqlExprParser(MySqlLexer lexer, MySqlFunctionManager functionManager, bool cacheEvalRst, string charset)
            : base(lexer, cacheEvalRst)
        {
            this.functionManager = functionManager;
            this.charset = charset ?? DefaultCharset;
        }

        public virtual void SetSelectParser(MySqlDmlSelectParser selectParser)
        {
            this.selectParser = selectParser;
        }

        /// <summary>first token of this expression has been scanned, not yet consumed</summary>
        /// <exception cref="System.SqlSyntaxErrorException" />
        public virtual IExpression Expression()
        {
            var token = lexer.Token();
            if (token == MySqlToken.None)
            {
                token = lexer.NextToken();
            }
            if (token == MySqlToken.Eof)
            {
                Err("unexpected EOF");
            }
            var left = LogicalOrExpression();
            if (lexer.Token() == MySqlToken.OpAssign)
            {
                lexer.NextToken();
                var right = Expression();
                return new AssignmentExpression(left, right).SetCacheEvalRst(cacheEvalRst);
            }
            return left;
        }

        /// <summary>
        ///     <code>higherPRJExpr ( ( '||' | 'OR' ) higherPRJExpr )*</code>
        /// </summary>
        /// <exception cref="System.SqlSyntaxErrorException" />
        private IExpression LogicalOrExpression()
        {
            LogicalOrExpression or = null;
            for (var expr = LogicalXORExpression();;)
            {
                switch (lexer.Token())
                {
                    case MySqlToken.OpLogicalOr:
                    case MySqlToken.KwOr:
                    {
                        lexer.NextToken();
                        if (or == null)
                        {
                            or = new LogicalOrExpression();
                            or.SetCacheEvalRst(cacheEvalRst);
                            or.AppendOperand(expr);
                            expr = or;
                        }
                        var newExpr = LogicalXORExpression();
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

        /// <summary>
        ///     <code>higherPRJExpr ( 'XOR' higherPRJExpr )*</code>
        /// </summary>
        /// <exception cref="System.SqlSyntaxErrorException" />
        private IExpression LogicalXORExpression()
        {
            for (var expr = LogicalAndExpression();;)
            {
                switch (lexer.Token())
                {
                    case MySqlToken.KwXor:
                    {
                        lexer.NextToken();
                        var newExpr = LogicalAndExpression();
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

        /// <summary>
        ///     <code>higherPRJExpr ( ('AND'|'&&') higherPRJExpr )*</code>
        /// </summary>
        /// <exception cref="System.SqlSyntaxErrorException" />
        private IExpression LogicalAndExpression()
        {
            LogicalAndExpression and = null;
            for (var expr = LogicalNotExpression();;)
            {
                switch (lexer.Token())
                {
                    case MySqlToken.OpLogicalAnd:
                    case MySqlToken.KwAnd:
                    {
                        lexer.NextToken();
                        if (and == null)
                        {
                            and = new LogicalAndExpression();
                            and.SetCacheEvalRst(cacheEvalRst);
                            and.AppendOperand(expr);
                            expr = and;
                        }
                        var newExpr = LogicalNotExpression();
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

        /// <summary>
        ///     <code>'NOT'* higherPRJExpr</code>
        /// </summary>
        /// <exception cref="System.SqlSyntaxErrorException" />
        private IExpression LogicalNotExpression()
        {
            var not = 0;
            for (; lexer.Token() == MySqlToken.KwNot; ++not)
            {
                lexer.NextToken();
            }
            var expr = ComparisionExpression();
            for (; not > 0; --not)
            {
                expr = new LogicalNotExpression(expr).SetCacheEvalRst(cacheEvalRst);
            }
            return expr;
        }

        /// <summary>
        ///     <code>BETWEEN ...
        /// </summary>
        /// <remarks>
        ///     <code>BETWEEN ... AND</code> has lower precedence than other comparison
        ///     operator
        /// </remarks>
        /// <exception cref="System.SqlSyntaxErrorException" />
        private IExpression ComparisionExpression()
        {
            IExpression temp;
            for (var fst = BitOrExpression(null, null);;)
            {
                switch (lexer.Token())
                {
                    case MySqlToken.KwNot:
                    {
                        lexer.NextToken();
                        switch (lexer.Token())
                        {
                            case MySqlToken.KwBetween:
                            {
                                lexer.NextToken();
                                var snd = ComparisionExpression();
                                Match(MySqlToken.KwAnd);
                                var trd = ComparisionExpression();
                                return new BetweenAndExpression(true, fst, snd, trd).SetCacheEvalRst(cacheEvalRst);
                            }

                            case MySqlToken.KwRlike:
                            case MySqlToken.KwRegexp:
                            {
                                lexer.NextToken();
                                temp = BitOrExpression(null, null);
                                fst = new RegexpExpression(true, fst, temp).SetCacheEvalRst(cacheEvalRst);
                                continue;
                            }

                            case MySqlToken.KwLike:
                            {
                                lexer.NextToken();
                                temp = BitOrExpression(null, null);
                                IExpression escape = null;
                                if (EqualsIdentifier("ESCAPE") >= 0)
                                {
                                    lexer.NextToken();
                                    escape = BitOrExpression(null, null);
                                }
                                fst = new LikeExpression(true, fst, temp, escape).SetCacheEvalRst(cacheEvalRst);
                                continue;
                            }

                            case MySqlToken.KwIn:
                            {
                                if (lexer.NextToken() != MySqlToken.PuncLeftParen)
                                {
                                    lexer.AddCacheToke(MySqlToken.KwIn);
                                    return fst;
                                }
                                var @in = RightOprandOfIn();
                                fst = new InExpression(true, fst, @in).SetCacheEvalRst(cacheEvalRst);
                                continue;
                            }

                            default:
                            {
                                throw Err("unexpect token after NOT: " + lexer.Token());
                            }
                        }
                        //goto case MySqlToken.KwBetween;
                    }

                    case MySqlToken.KwBetween:
                    {
                        lexer.NextToken();
                        var snd_1 = ComparisionExpression();
                        Match(MySqlToken.KwAnd);
                        var trd_1 = ComparisionExpression();
                        return new BetweenAndExpression(false, fst, snd_1, trd_1).SetCacheEvalRst(cacheEvalRst);
                    }

                    case MySqlToken.KwRlike:
                    case MySqlToken.KwRegexp:
                    {
                        lexer.NextToken();
                        temp = BitOrExpression(null, null);
                        fst = new RegexpExpression(false, fst, temp).SetCacheEvalRst(cacheEvalRst);
                        continue;
                    }

                    case MySqlToken.KwLike:
                    {
                        lexer.NextToken();
                        temp = BitOrExpression(null, null);
                        IExpression escape_1 = null;
                        if (EqualsIdentifier("ESCAPE") >= 0)
                        {
                            lexer.NextToken();
                            escape_1 = BitOrExpression(null, null);
                        }
                        fst = new LikeExpression(false, fst, temp, escape_1).SetCacheEvalRst(cacheEvalRst);
                        continue;
                    }

                    case MySqlToken.KwIn:
                    {
                        if (lexer.NextToken() != MySqlToken.PuncLeftParen)
                        {
                            lexer.AddCacheToke(MySqlToken.KwIn);
                            return fst;
                        }
                        temp = RightOprandOfIn();
                        fst = new InExpression(false, fst, temp).SetCacheEvalRst(cacheEvalRst);
                        continue;
                    }

                    case MySqlToken.KwIs:
                    {
                        switch (lexer.NextToken())
                        {
                            case MySqlToken.KwNot:
                            {
                                switch (lexer.NextToken())
                                {
                                    case MySqlToken.LiteralNull:
                                    {
                                        lexer.NextToken();
                                        fst =
                                            new ComparisionIsExpression(fst, ComparisionIsExpression.IsNotNull)
                                                .SetCacheEvalRst(cacheEvalRst);
                                        continue;
                                    }

                                    case MySqlToken.LiteralBoolFalse:
                                    {
                                        lexer.NextToken();
                                        fst =
                                            new ComparisionIsExpression(fst, ComparisionIsExpression.IsNotFalse)
                                                .SetCacheEvalRst(cacheEvalRst);
                                        continue;
                                    }

                                    case MySqlToken.LiteralBoolTrue:
                                    {
                                        lexer.NextToken();
                                        fst =
                                            new ComparisionIsExpression(fst, ComparisionIsExpression.IsNotTrue)
                                                .SetCacheEvalRst(cacheEvalRst);
                                        continue;
                                    }

                                    default:
                                    {
                                        MatchIdentifier("UNKNOWN");
                                        fst =
                                            new ComparisionIsExpression(fst, ComparisionIsExpression.IsNotUnknown)
                                                .SetCacheEvalRst(cacheEvalRst);
                                        continue;
                                    }
                                }
                                //goto case MySqlToken.LiteralNull;
                            }

                            case MySqlToken.LiteralNull:
                            {
                                lexer.NextToken();
                                fst =
                                    new ComparisionIsExpression(fst, ComparisionIsExpression.IsNull).SetCacheEvalRst(
                                        cacheEvalRst);
                                continue;
                            }

                            case MySqlToken.LiteralBoolFalse:
                            {
                                lexer.NextToken();
                                fst =
                                    new ComparisionIsExpression(fst, ComparisionIsExpression.IsFalse).SetCacheEvalRst(
                                        cacheEvalRst);
                                continue;
                            }

                            case MySqlToken.LiteralBoolTrue:
                            {
                                lexer.NextToken();
                                fst =
                                    new ComparisionIsExpression(fst, ComparisionIsExpression.IsTrue).SetCacheEvalRst(
                                        cacheEvalRst);
                                continue;
                            }

                            default:
                            {
                                MatchIdentifier("UNKNOWN");
                                fst =
                                    new ComparisionIsExpression(fst, ComparisionIsExpression.IsUnknown).SetCacheEvalRst(
                                        cacheEvalRst);
                                continue;
                            }
                        }
                        //goto case MySqlToken.OpEquals;
                    }

                    case MySqlToken.OpEquals:
                    {
                        lexer.NextToken();
                        temp = AnyAllExpression();
                        fst = new ComparisionEqualsExpression(fst, temp).SetCacheEvalRst(cacheEvalRst);
                        continue;
                    }

                    case MySqlToken.OpNullSafeEquals:
                    {
                        lexer.NextToken();
                        temp = BitOrExpression(null, null);
                        fst = new ComparisionNullSafeEqualsExpression(fst, temp).SetCacheEvalRst(cacheEvalRst);
                        continue;
                    }

                    case MySqlToken.OpGreaterOrEquals:
                    {
                        lexer.NextToken();
                        temp = AnyAllExpression();
                        fst = new ComparisionGreaterThanOrEqualsExpression(fst, temp).SetCacheEvalRst(cacheEvalRst);
                        continue;
                    }

                    case MySqlToken.OpGreaterThan:
                    {
                        lexer.NextToken();
                        temp = AnyAllExpression();
                        fst = new ComparisionGreaterThanExpression(fst, temp).SetCacheEvalRst(cacheEvalRst);
                        continue;
                    }

                    case MySqlToken.OpLessOrEquals:
                    {
                        lexer.NextToken();
                        temp = AnyAllExpression();
                        fst = new ComparisionLessThanOrEqualsExpression(fst, temp).SetCacheEvalRst(cacheEvalRst);
                        continue;
                    }

                    case MySqlToken.OpLessThan:
                    {
                        lexer.NextToken();
                        temp = AnyAllExpression();
                        fst = new ComparisionLessThanExpression(fst, temp).SetCacheEvalRst(cacheEvalRst);
                        continue;
                    }

                    case MySqlToken.OpLessOrGreater:
                    {
                        lexer.NextToken();
                        temp = AnyAllExpression();
                        fst = new ComparisionLessOrGreaterThanExpression(fst, temp).SetCacheEvalRst(cacheEvalRst);
                        continue;
                    }

                    case MySqlToken.OpNotEquals:
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
                            Match(MySqlToken.KwLike);
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
        ///     <see cref="Tup.Cobar4Net.Parser.Ast.Expression.Misc.IQueryExpression" />
        ///     or
        ///     <see cref="Tup.Cobar4Net.Parser.Ast.Expression.Misc.InExpressionList" />
        /// </returns>
        /// <exception cref="System.SqlSyntaxErrorException" />
        private IExpression RightOprandOfIn()
        {
            Match(MySqlToken.PuncLeftParen);
            if (MySqlToken.KwSelect == lexer.Token())
            {
                var subq = SubQuery();
                Match(MySqlToken.PuncRightParen);
                return subq;
            }
            return new InExpressionList(ExpressionList(new List<IExpression>())).SetCacheEvalRst(cacheEvalRst);
        }

        /// <exception cref="System.SqlSyntaxErrorException" />
        private IExpression AnyAllExpression()
        {
            IQueryExpression subquery = null;
            switch (lexer.Token())
            {
                case MySqlToken.KwAll:
                {
                    lexer.NextToken();
                    Match(MySqlToken.PuncLeftParen);
                    subquery = SubQuery();
                    Match(MySqlToken.PuncRightParen);
                    return new SubqueryAllExpression(subquery).SetCacheEvalRst(cacheEvalRst);
                }

                default:
                {
                    var matchIndex = EqualsIdentifier("SOME", "ANY");
                    if (matchIndex < 0)
                    {
                        return BitOrExpression(null, null);
                    }
                    var consumed = lexer.GetStringValue();
                    var consumedUp = lexer.GetStringValueUppercase();
                    if (lexer.NextToken() == MySqlToken.PuncLeftParen)
                    {
                        lexer.NextToken();
                        subquery = SubQuery();
                        Match(MySqlToken.PuncRightParen);
                        return new SubqueryAnyExpression(subquery).SetCacheEvalRst(cacheEvalRst);
                    }
                    return BitOrExpression(consumed, consumedUp);
                }
            }
        }

        /// <param name="consumed">
        ///     not null means that a token that has been pre-consumed
        ///     stands for next token
        /// </param>
        /// <exception cref="System.SqlSyntaxErrorException" />
        private IExpression BitOrExpression(string consumed, string consumedUp)
        {
            for (var expr = BitAndExpression(consumed, consumedUp);;)
            {
                switch (lexer.Token())
                {
                    case MySqlToken.OpVerticalBar:
                    {
                        lexer.NextToken();
                        var newExpr = BitAndExpression(null, null);
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

        /// <exception cref="System.SqlSyntaxErrorException" />
        private IExpression BitAndExpression(string consumed, string consumedUp)
        {
            for (var expr = BitShiftExpression(consumed, consumedUp);;)
            {
                switch (lexer.Token())
                {
                    case MySqlToken.OpAmpersand:
                    {
                        lexer.NextToken();
                        var newExpr = BitShiftExpression(null, null);
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

        /// <summary>
        ///     <code>higherExpr ( ('&lt;&lt;'|'&gt;&gt;') higherExpr)+</code>
        /// </summary>
        /// <exception cref="System.SqlSyntaxErrorException" />
        private IExpression BitShiftExpression(string consumed, string consumedUp)
        {
            IExpression temp;
            for (var expr = ArithmeticTermOperatorExpression(consumed, consumedUp);;)
            {
                switch (lexer.Token())
                {
                    case MySqlToken.OpLeftShift:
                    {
                        lexer.NextToken();
                        temp = ArithmeticTermOperatorExpression(null, null);
                        expr = new BitShiftExpression(false, expr, temp).SetCacheEvalRst(cacheEvalRst);
                        break;
                    }

                    case MySqlToken.OpRightShift:
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

        /// <summary>
        ///     <code>higherExpr ( ('+'|'-') higherExpr)+</code>
        /// </summary>
        /// <exception cref="System.SqlSyntaxErrorException" />
        private IExpression ArithmeticTermOperatorExpression(string consumed, string consumedUp)
        {
            IExpression temp;
            for (var expr = ArithmeticFactorOperatorExpression(consumed, consumedUp);;)
            {
                switch (lexer.Token())
                {
                    case MySqlToken.OpPlus:
                    {
                        lexer.NextToken();
                        temp = ArithmeticFactorOperatorExpression(null, null);
                        expr = new ArithmeticAddExpression(expr, temp).SetCacheEvalRst(cacheEvalRst);
                        break;
                    }

                    case MySqlToken.OpMinus:
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

        /// <summary>
        ///     <code>higherExpr ( ('*'|'/'|'%'|'DIV'|'MOD') higherExpr)+</code>
        /// </summary>
        /// <exception cref="System.SqlSyntaxErrorException" />
        private IExpression ArithmeticFactorOperatorExpression(string consumed, string consumedUp)
        {
            IExpression temp;
            for (var expr = BitXORExpression(consumed,
                consumedUp);;)
            {
                switch (lexer.Token())
                {
                    case MySqlToken.OpAsterisk:
                    {
                        lexer.NextToken();
                        temp = BitXORExpression(null, null);
                        expr = new ArithmeticMultiplyExpression(expr, temp).SetCacheEvalRst(cacheEvalRst);
                        break;
                    }

                    case MySqlToken.OpSlash:
                    {
                        lexer.NextToken();
                        temp = BitXORExpression(null, null);
                        expr = new ArithmeticDivideExpression(expr, temp).SetCacheEvalRst(cacheEvalRst);
                        break;
                    }

                    case MySqlToken.KwDiv:
                    {
                        lexer.NextToken();
                        temp = BitXORExpression(null, null);
                        expr = new ArithmeticIntegerDivideExpression(expr, temp).SetCacheEvalRst(cacheEvalRst);
                        break;
                    }

                    case MySqlToken.OpPercent:
                    case MySqlToken.KwMod:
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

        /// <summary>
        ///     <code>higherExpr ('^' higherExpr)+</code>
        /// </summary>
        /// <exception cref="System.SqlSyntaxErrorException" />
        private IExpression BitXORExpression(string consumed, string consumedUp)
        {
            IExpression temp;
            for (var expr = UnaryOpExpression(consumed, consumedUp);;)
            {
                switch (lexer.Token())
                {
                    case MySqlToken.OpCaret:
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
        ///     <code>('+'|'-'|'~'|'!'|'BINARY')* higherExpr</code><br />
        ///     '!' has higher precedence
        /// </summary>
        /// <exception cref="System.SqlSyntaxErrorException" />
        private IExpression UnaryOpExpression(string consumed
                                              , string consumedUp)
        {
            if (consumed == null)
            {
                IExpression expr;
                switch (lexer.Token())
                {
                    case MySqlToken.OpExclamation:
                    {
                        lexer.NextToken();
                        expr = UnaryOpExpression(null, null);
                        return new NegativeValueExpression(expr).SetCacheEvalRst(cacheEvalRst);
                    }

                    case MySqlToken.OpPlus:
                    {
                        lexer.NextToken();
                        return UnaryOpExpression(null, null);
                    }

                    case MySqlToken.OpMinus:
                    {
                        lexer.NextToken();
                        expr = UnaryOpExpression(null, null);
                        return new MinusExpression(expr).SetCacheEvalRst(cacheEvalRst);
                    }

                    case MySqlToken.OpTilde:
                    {
                        lexer.NextToken();
                        expr = UnaryOpExpression(null, null);
                        return new BitInvertExpression(expr).SetCacheEvalRst(cacheEvalRst);
                    }

                    case MySqlToken.KwBinary:
                    {
                        lexer.NextToken();
                        expr = UnaryOpExpression(null, null);
                        return new CastBinaryExpression(expr).SetCacheEvalRst(cacheEvalRst);
                    }
                }
            }
            return CollateExpression(consumed, consumedUp);
        }

        /// <exception cref="System.SqlSyntaxErrorException" />
        private IExpression CollateExpression(string consumed
                                              , string consumedUp)
        {
            for (var expr = UserExpression(consumed, consumedUp);;)
            {
                if (lexer.Token() == MySqlToken.KwCollate)
                {
                    lexer.NextToken();
                    var collateName = lexer.GetStringValue();
                    Match(MySqlToken.Identifier);
                    expr = new CollateExpression(expr, collateName).SetCacheEvalRst(cacheEvalRst);
                    continue;
                }
                return expr;
            }
        }

        /// <exception cref="System.SqlSyntaxErrorException" />
        private IExpression UserExpression(string consumed
                                           , string consumedUp)
        {
            var first = PrimaryExpression(consumed, consumedUp);
            if (lexer.Token() == MySqlToken.UsrVar)
            {
                if (first is LiteralString)
                {
                    var str = new StringBuilder().Append('\'').Append(((LiteralString)first
                        ).StringValue).Append('\'').Append(lexer.GetStringValue());
                    lexer.NextToken();
                    return new UserExpression(str.ToString()).SetCacheEvalRst(cacheEvalRst);
                }
                if (first is IdentifierExpr)
                {
                    var str = new StringBuilder().Append(((IdentifierExpr)first).IdText).Append(lexer.GetStringValue());
                    lexer.NextToken();
                    return new UserExpression(str.ToString()).SetCacheEvalRst(cacheEvalRst);
                }
            }
            return first;
        }

        /// <exception cref="System.SqlSyntaxErrorException" />
        private IExpression PrimaryExpression(string consumed
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
            IExpression tempExpr;
            IExpression tempExpr2;
            IList<IExpression> tempExprList;
            switch (lexer.Token())
            {
                case MySqlToken.PlaceHolder:
                {
                    tempStr = lexer.GetStringValue();
                    tempStrUp = lexer.GetStringValueUppercase();
                    lexer.NextToken();
                    return CreatePlaceHolder(tempStr, tempStrUp);
                }

                case MySqlToken.LiteralBit:
                {
                    tempStr = lexer.GetStringValue();
                    lexer.NextToken();
                    return new LiteralBitField(null, tempStr).SetCacheEvalRst(cacheEvalRst);
                }

                case MySqlToken.LiteralHex:
                {
                    var hex = new LiteralHexadecimal(null, lexer.Sql, lexer.OffsetCache, lexer.SizeCache, charset);
                    lexer.NextToken();
                    return hex.SetCacheEvalRst(cacheEvalRst);
                }

                case MySqlToken.LiteralBoolFalse:
                {
                    lexer.NextToken();
                    return new LiteralBoolean(false).SetCacheEvalRst(cacheEvalRst);
                }

                case MySqlToken.LiteralBoolTrue:
                {
                    lexer.NextToken();
                    return new LiteralBoolean(true).SetCacheEvalRst(cacheEvalRst);
                }

                case MySqlToken.LiteralNull:
                {
                    lexer.NextToken();
                    return new LiteralNull().SetCacheEvalRst(cacheEvalRst);
                }

                case MySqlToken.LiteralNchars:
                {
                    tempSb = new StringBuilder();
                    do
                    {
                        lexer.AppendStringContent(tempSb);
                    } while (lexer.NextToken() == MySqlToken.LiteralChars);
                    return new LiteralString(null, tempSb.ToString(), true).SetCacheEvalRst(cacheEvalRst);
                }

                case MySqlToken.LiteralChars:
                {
                    tempSb = new StringBuilder();
                    do
                    {
                        lexer.AppendStringContent(tempSb);
                    } while (lexer.NextToken() == MySqlToken.LiteralChars);
                    return new LiteralString(null, tempSb.ToString(), false).SetCacheEvalRst(cacheEvalRst);
                }

                case MySqlToken.LiteralNumPureDigit:
                {
                    tempNum = lexer.GetIntegerValue();
                    lexer.NextToken();
                    return new LiteralNumber(tempNum).SetCacheEvalRst(cacheEvalRst);
                }

                case MySqlToken.LiteralNumMixDigit:
                {
                    tempNum = lexer.GetDecimalValue();
                    lexer.NextToken();
                    return new LiteralNumber(tempNum).SetCacheEvalRst(cacheEvalRst);
                }

                case MySqlToken.QuestionMark:
                {
                    var index = lexer.ParamIndex;
                    lexer.NextToken();
                    return CreateParam(index);
                }

                case MySqlToken.KwCase:
                {
                    lexer.NextToken();
                    return CaseWhenExpression();
                }

                case MySqlToken.KwInterval:
                {
                    lexer.NextToken();
                    return IntervalExpression();
                }

                case MySqlToken.KwExists:
                {
                    lexer.NextToken();
                    Match(MySqlToken.PuncLeftParen);
                    tempExpr = SubQuery();
                    Match(MySqlToken.PuncRightParen);
                    return new ExistsPrimary((IQueryExpression)tempExpr).SetCacheEvalRst(cacheEvalRst);
                }

                case MySqlToken.UsrVar:
                {
                    tempStr = lexer.GetStringValue();
                    tempExpr = new UsrDefVarPrimary(tempStr).SetCacheEvalRst(cacheEvalRst);
                    if (lexer.NextToken() == MySqlToken.OpAssign)
                    {
                        lexer.NextToken();
                        tempExpr2 = Expression();
                        return new AssignmentExpression(tempExpr, tempExpr2);
                    }
                    return tempExpr;
                }

                case MySqlToken.SysVar:
                {
                    return SystemVariale();
                }

                case MySqlToken.KwMatch:
                {
                    lexer.NextToken();
                    return MatchExpression();
                }

                case MySqlToken.PuncLeftParen:
                {
                    lexer.NextToken();
                    if (lexer.Token() == MySqlToken.KwSelect)
                    {
                        tempExpr = SubQuery();
                        Match(MySqlToken.PuncRightParen);
                        return tempExpr;
                    }
                    tempExpr = Expression();
                    switch (lexer.Token())
                    {
                        case MySqlToken.PuncRightParen:
                        {
                            lexer.NextToken();
                            return tempExpr;
                        }

                        case MySqlToken.PuncComma:
                        {
                            lexer.NextToken();
                            tempExprList = new List<IExpression>();
                            tempExprList.Add(tempExpr);
                            tempExprList = ExpressionList(tempExprList);
                            return new RowExpression(tempExprList).SetCacheEvalRst(cacheEvalRst);
                        }

                        default:
                        {
                            throw Err("unexpected token: " + lexer.Token());
                        }
                    }
                    //goto case MySqlToken.KwUtcDate;
                }

                case MySqlToken.KwUtcDate:
                {
                    lexer.NextToken();
                    if (lexer.Token() == MySqlToken.PuncLeftParen)
                    {
                        lexer.NextToken();
                        Match(MySqlToken.PuncRightParen);
                    }
                    return new UtcDate(null).SetCacheEvalRst(cacheEvalRst);
                }

                case MySqlToken.KwUtcTime:
                {
                    lexer.NextToken();
                    if (lexer.Token() == MySqlToken.PuncLeftParen)
                    {
                        lexer.NextToken();
                        Match(MySqlToken.PuncRightParen);
                    }
                    return new UtcTime(null).SetCacheEvalRst(cacheEvalRst);
                }

                case MySqlToken.KwUtcTimestamp:
                {
                    lexer.NextToken();
                    if (lexer.Token() == MySqlToken.PuncLeftParen)
                    {
                        lexer.NextToken();
                        Match(MySqlToken.PuncRightParen);
                    }
                    return new UtcTimestamp(null).SetCacheEvalRst(cacheEvalRst);
                }

                case MySqlToken.KwCurrentDate:
                {
                    lexer.NextToken();
                    if (lexer.Token() == MySqlToken.PuncLeftParen)
                    {
                        lexer.NextToken();
                        Match(MySqlToken.PuncRightParen);
                    }
                    return new Curdate().SetCacheEvalRst(cacheEvalRst);
                }

                case MySqlToken.KwCurrentTime:
                {
                    lexer.NextToken();
                    if (lexer.Token() == MySqlToken.PuncLeftParen)
                    {
                        lexer.NextToken();
                        Match(MySqlToken.PuncRightParen);
                    }
                    return new Curtime().SetCacheEvalRst(cacheEvalRst);
                }

                case MySqlToken.KwCurrentTimestamp:
                case MySqlToken.KwLocaltime:
                case MySqlToken.KwLocaltimestamp:
                {
                    lexer.NextToken();
                    if (lexer.Token() == MySqlToken.PuncLeftParen)
                    {
                        lexer.NextToken();
                        Match(MySqlToken.PuncRightParen);
                    }
                    return new Now().SetCacheEvalRst(cacheEvalRst);
                }

                case MySqlToken.KwCurrentUser:
                {
                    lexer.NextToken();
                    if (lexer.Token() == MySqlToken.PuncLeftParen)
                    {
                        lexer.NextToken();
                        Match(MySqlToken.PuncRightParen);
                    }
                    return new CurrentUser().SetCacheEvalRst(cacheEvalRst);
                }

                case MySqlToken.KwDefault:
                {
                    if (lexer.NextToken() == MySqlToken.PuncLeftParen)
                    {
                        return OrdinaryFunction(lexer.GetStringValue(), lexer.GetStringValueUppercase());
                    }
                    return new DefaultValue().SetCacheEvalRst(cacheEvalRst);
                }

                case MySqlToken.KwDatabase:
                case MySqlToken.KwIf:
                case MySqlToken.KwInsert:
                case MySqlToken.KwLeft:
                case MySqlToken.KwRepeat:
                case MySqlToken.KwReplace:
                case MySqlToken.KwRight:
                case MySqlToken.KwSchema:
                case MySqlToken.KwValues:
                {
                    tempStr = lexer.GetStringValue();
                    tempStrUp = lexer.GetStringValueUppercase();
                    var tempStrUp2 = lexer.Token().KeyWordToString();
                    if (!tempStrUp2.Equals(tempStrUp))
                    {
                        tempStrUp = tempStr = tempStrUp2;
                    }
                    if (lexer.NextToken() == MySqlToken.PuncLeftParen)
                    {
                        return OrdinaryFunction(tempStr, tempStrUp);
                    }
                    throw Err("keyword not followed by '(' is not expression: " + tempStr);
                }

                case MySqlToken.KwMod:
                {
                    lexer.NextToken();
                    Match(MySqlToken.PuncLeftParen);
                    tempExpr = Expression();
                    Match(MySqlToken.PuncComma);
                    tempExpr2 = Expression();
                    Match(MySqlToken.PuncRightParen);
                    return new ArithmeticModExpression(tempExpr, tempExpr2).SetCacheEvalRst(cacheEvalRst);
                }

                case MySqlToken.KwChar:
                {
                    lexer.NextToken();
                    Match(MySqlToken.PuncLeftParen);
                    return FunctionChar();
                }

                case MySqlToken.KwConvert:
                {
                    lexer.NextToken();
                    Match(MySqlToken.PuncLeftParen);
                    return FunctionConvert();
                }

                case MySqlToken.Identifier:
                {
                    tempStr = lexer.GetStringValue();
                    tempStrUp = lexer.GetStringValueUppercase();
                    lexer.NextToken();
                    return StartedFromIdentifier(tempStr, tempStrUp);
                }

                case MySqlToken.OpAsterisk:
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
        /// <exception cref="System.SqlSyntaxErrorException" />
        private Timestampdiff Timestampdiff()
        {
            var unit = IntervalPrimaryUnit();
            Match(MySqlToken.PuncComma);
            var interval = Expression();
            Match(MySqlToken.PuncComma);
            var expr = Expression();
            Match(MySqlToken.PuncRightParen);
            IList<IExpression> argument = new List<IExpression>(2);
            argument.Add(interval);
            argument.Add(expr);
            var func = new Timestampdiff(unit, argument);
            func.SetCacheEvalRst(cacheEvalRst);
            return func;
        }

        /// <summary>first '(' has been consumed</summary>
        /// <exception cref="System.SqlSyntaxErrorException" />
        private Timestampadd Timestampadd()
        {
            var unit = IntervalPrimaryUnit();
            Match(MySqlToken.PuncComma);
            var interval = Expression();
            Match(MySqlToken.PuncComma);
            var expr = Expression();
            Match(MySqlToken.PuncRightParen);
            IList<IExpression> argument = new List<IExpression>(2);
            argument.Add(interval);
            argument.Add(expr);
            var func = new Timestampadd(unit, argument);
            func.SetCacheEvalRst(cacheEvalRst);
            return func;
        }

        /// <summary>first '(' has been consumed</summary>
        /// <exception cref="System.SqlSyntaxErrorException" />
        private Extract Extract()
        {
            var unit = IntervalPrimaryUnit();
            Match(MySqlToken.KwFrom);
            var date = Expression();
            Match(MySqlToken.PuncRightParen);
            var extract = new Extract(unit, date);
            extract.SetCacheEvalRst(cacheEvalRst);
            return extract;
        }

        /// <summary>first '(' has been consumed</summary>
        /// <exception cref="System.SqlSyntaxErrorException" />
        private Convert FunctionConvert()
        {
            var expr = Expression();
            Match(MySqlToken.KwUsing);
            var tempStr = lexer.GetStringValue();
            Match(MySqlToken.Identifier);
            Match(MySqlToken.PuncRightParen);
            var cvt = new Convert(expr, tempStr);
            cvt.SetCacheEvalRst(cacheEvalRst);
            return cvt;
        }

        /// <summary>first '(' has been consumed</summary>
        /// <exception cref="System.SqlSyntaxErrorException" />
        private Char FunctionChar()
        {
            Char chr;
            for (IList<IExpression> tempExprList = new List<IExpression>();;)
            {
                var tempExpr = Expression();
                tempExprList.Add(tempExpr);
                switch (lexer.Token())
                {
                    case MySqlToken.PuncComma:
                    {
                        lexer.NextToken();
                        continue;
                    }

                    case MySqlToken.PuncRightParen:
                    {
                        lexer.NextToken();
                        chr = new Char(tempExprList, null);
                        chr.SetCacheEvalRst(cacheEvalRst);
                        return chr;
                    }

                    case MySqlToken.KwUsing:
                    {
                        lexer.NextToken();
                        var tempStr = lexer.GetStringValue();
                        Match(MySqlToken.Identifier);
                        Match(MySqlToken.PuncRightParen);
                        chr = new Char(tempExprList, tempStr);
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
        ///     last token consumed is
        ///     <see cref="MySqlToken.Identifier" />
        ///     , MUST NOT be
        ///     <code>null</code>
        /// </summary>
        /// <exception cref="System.SqlSyntaxErrorException" />
        private IExpression StartedFromIdentifier(string consumed, string consumedUp)
        {
            IExpression tempExpr;
            IExpression tempExpr2;
            IList<IExpression> tempExprList;
            string tempStr;
            StringBuilder tempSb;
            bool tempGroupDistinct;
            switch (lexer.Token())
            {
                case MySqlToken.PuncDot:
                {
                    for (tempExpr = new Identifier(null, consumed, consumedUp).SetCacheEvalRst(cacheEvalRst);
                         lexer.Token() == MySqlToken.PuncDot;)
                    {
                        switch (lexer.NextToken())
                        {
                            case MySqlToken.Identifier:
                            {
                                tempExpr =
                                    new Identifier((Identifier)tempExpr, lexer.GetStringValue(),
                                        lexer.GetStringValueUppercase()).SetCacheEvalRst(cacheEvalRst);
                                lexer.NextToken();
                                break;
                            }

                            case MySqlToken.OpAsterisk:
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

                case MySqlToken.LiteralBit:
                {
                    if (consumed[0] != '_')
                    {
                        return new Identifier(null, consumed, consumedUp).SetCacheEvalRst(cacheEvalRst);
                    }
                    tempStr = lexer.GetStringValue();
                    lexer.NextToken();
                    return new LiteralBitField(consumed, tempStr).SetCacheEvalRst(cacheEvalRst);
                }

                case MySqlToken.LiteralHex:
                {
                    if (consumed[0] != '_')
                    {
                        return new Identifier(null, consumed, consumedUp).SetCacheEvalRst(cacheEvalRst);
                    }
                    var hex = new LiteralHexadecimal(consumed, lexer.Sql, lexer.OffsetCache, lexer.SizeCache, charset);
                    lexer.NextToken();
                    return hex.SetCacheEvalRst(cacheEvalRst);
                }

                case MySqlToken.LiteralChars:
                {
                    if (consumed[0] != '_')
                    {
                        return new Identifier(null, consumed, consumedUp).SetCacheEvalRst(cacheEvalRst);
                    }
                    tempSb = new StringBuilder();
                    do
                    {
                        lexer.AppendStringContent(tempSb);
                    } while (lexer.NextToken() == MySqlToken.LiteralChars);
                    return new LiteralString(consumed, tempSb.ToString(), false).SetCacheEvalRst(cacheEvalRst);
                }

                case MySqlToken.PuncLeftParen:
                {
                    consumedUp = IdentifierExpr.UnescapeName(consumedUp);
                    switch (functionManager.GetParsingStrategy(consumedUp))
                    {
                        case FunctionParsingStrategy.GetFormat:
                        {
                            // GET_FORMAT({DATE|TIME|DATETIME},
                            // {'EUR'|'USA'|'JIS'|'ISO'|'INTERNAL'})
                            lexer.NextToken();
                            var gfi = MatchIdentifier("DATE", "TIME", "DATETIME", "TIMESTAMP");
                            Match(MySqlToken.PuncComma);
                            var getFormatArg = Expression();
                            Match(MySqlToken.PuncRightParen);
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

                        case FunctionParsingStrategy.Cast:
                        {
                            lexer.NextToken();
                            tempExpr = Expression();
                            Match(MySqlToken.KwAs);
                            var type = Type4specialFunc();
                            Match(MySqlToken.PuncRightParen);
                            var info = type.Value;
                            if (info != null)
                            {
                                return
                                    new Cast(tempExpr, type.Key, info.Key, info.Value).SetCacheEvalRst(
                                        cacheEvalRst);
                            }
                            return new Cast(tempExpr, type.Key, null, null).SetCacheEvalRst(cacheEvalRst);
                            //goto case FunctionParsingStrategy.Position;
                        }

                        case FunctionParsingStrategy.Position:
                        {
                            lexer.NextToken();
                            tempExprList = new List<IExpression>(2);
                            tempExprList.Add(Expression());
                            Match(MySqlToken.KwIn);
                            tempExprList.Add(Expression());
                            Match(MySqlToken.PuncRightParen);
                            return new Locate(tempExprList).SetCacheEvalRst(cacheEvalRst);
                        }

                        case FunctionParsingStrategy.Substring:
                        {
                            lexer.NextToken();
                            tempExprList = new List<IExpression>(3);
                            tempExprList.Add(Expression());
                            Match(MySqlToken.PuncComma, MySqlToken.KwFrom);
                            tempExprList.Add(Expression());
                            switch (lexer.Token())
                            {
                                case MySqlToken.PuncComma:
                                case MySqlToken.KwFor:
                                {
                                    lexer.NextToken();
                                    tempExprList.Add(Expression());
                                    goto default;
                                }

                                default:
                                {
                                    Match(MySqlToken.PuncRightParen);
                                    break;
                                }
                            }
                            return new Substring(tempExprList).SetCacheEvalRst(cacheEvalRst);
                        }

                        case FunctionParsingStrategy.Row:
                        {
                            lexer.NextToken();
                            tempExprList = ExpressionList(new List<IExpression>());
                            return new RowExpression(tempExprList).SetCacheEvalRst(cacheEvalRst);
                        }

                        case FunctionParsingStrategy.Trim:
                        {
                            TrimDirection _trimDirection;
                            switch (lexer.NextToken())
                            {
                                case MySqlToken.KwBoth:
                                {
                                    lexer.NextToken();
                                    _trimDirection = TrimDirection.Both;
                                    break;
                                }

                                case MySqlToken.KwLeading:
                                {
                                    lexer.NextToken();
                                    _trimDirection = TrimDirection.Leading;
                                    break;
                                }

                                case MySqlToken.KwTrailing:
                                {
                                    lexer.NextToken();
                                    _trimDirection = TrimDirection.Trailing;
                                    break;
                                }

                                default:
                                {
                                    _trimDirection = TrimDirection.Default;
                                    break;
                                }
                            }
                            if (_trimDirection == TrimDirection.Default)
                            {
                                tempExpr = Expression();
                                if (lexer.Token() == MySqlToken.KwFrom)
                                {
                                    lexer.NextToken();
                                    tempExpr2 = Expression();
                                    Match(MySqlToken.PuncRightParen);
                                    return new Trim(_trimDirection, tempExpr, tempExpr2).SetCacheEvalRst(cacheEvalRst);
                                }
                                Match(MySqlToken.PuncRightParen);
                                return new Trim(_trimDirection, null, tempExpr).SetCacheEvalRst(cacheEvalRst);
                            }
                            if (lexer.Token() == MySqlToken.KwFrom)
                            {
                                lexer.NextToken();
                                tempExpr = Expression();
                                Match(MySqlToken.PuncRightParen);
                                return new Trim(_trimDirection, null, tempExpr).SetCacheEvalRst(cacheEvalRst);
                            }
                            tempExpr = Expression();
                            Match(MySqlToken.KwFrom);
                            tempExpr2 = Expression();
                            Match(MySqlToken.PuncRightParen);
                            return new Trim(_trimDirection, tempExpr, tempExpr2).SetCacheEvalRst(cacheEvalRst);
                        }

                        case FunctionParsingStrategy.Avg:
                        {
                            if (lexer.NextToken() == MySqlToken.KwDistinct)
                            {
                                tempGroupDistinct = true;
                                lexer.NextToken();
                            }
                            else
                            {
                                tempGroupDistinct = false;
                            }
                            tempExpr = Expression();
                            Match(MySqlToken.PuncRightParen);
                            return new Avg(tempExpr, tempGroupDistinct).SetCacheEvalRst(cacheEvalRst);
                        }

                        case FunctionParsingStrategy.Max:
                        {
                            if (lexer.NextToken() == MySqlToken.KwDistinct)
                            {
                                tempGroupDistinct = true;
                                lexer.NextToken();
                            }
                            else
                            {
                                tempGroupDistinct = false;
                            }
                            tempExpr = Expression();
                            Match(MySqlToken.PuncRightParen);
                            return new Max(tempExpr, tempGroupDistinct).SetCacheEvalRst(cacheEvalRst);
                        }

                        case FunctionParsingStrategy.Min:
                        {
                            if (lexer.NextToken() == MySqlToken.KwDistinct)
                            {
                                tempGroupDistinct = true;
                                lexer.NextToken();
                            }
                            else
                            {
                                tempGroupDistinct = false;
                            }
                            tempExpr = Expression();
                            Match(MySqlToken.PuncRightParen);
                            return new Min(tempExpr, tempGroupDistinct).SetCacheEvalRst(cacheEvalRst);
                        }

                        case FunctionParsingStrategy.Sum:
                        {
                            if (lexer.NextToken() == MySqlToken.KwDistinct)
                            {
                                tempGroupDistinct = true;
                                lexer.NextToken();
                            }
                            else
                            {
                                tempGroupDistinct = false;
                            }
                            tempExpr = Expression();
                            Match(MySqlToken.PuncRightParen);
                            return new Sum(tempExpr, tempGroupDistinct).SetCacheEvalRst(cacheEvalRst);
                        }

                        case FunctionParsingStrategy.Count:
                        {
                            if (lexer.NextToken() == MySqlToken.KwDistinct)
                            {
                                lexer.NextToken();
                                tempExprList = ExpressionList(new List<IExpression>());
                                return new Count(tempExprList).SetCacheEvalRst(cacheEvalRst);
                            }
                            tempExpr = Expression();
                            Match(MySqlToken.PuncRightParen);
                            return new Count(tempExpr).SetCacheEvalRst(cacheEvalRst);
                        }

                        case FunctionParsingStrategy.GroupConcat:
                        {
                            if (lexer.NextToken() == MySqlToken.KwDistinct)
                            {
                                lexer.NextToken();
                                tempGroupDistinct = true;
                            }
                            else
                            {
                                tempGroupDistinct = false;
                            }
                            for (tempExprList = new List<IExpression>();;)
                            {
                                tempExpr = Expression();
                                tempExprList.Add(tempExpr);
                                if (lexer.Token() == MySqlToken.PuncComma)
                                {
                                    lexer.NextToken();
                                }
                                else
                                {
                                    break;
                                }
                            }
                            var isDesc = false;
                            IList<IExpression> appendedColumnNames = null;
                            tempExpr = null;
                            // order by
                            tempStr = null;
                            switch (lexer.Token())
                            {
                                case MySqlToken.KwOrder:
                                {
                                    // literalChars
                                    lexer.NextToken();
                                    Match(MySqlToken.KwBy);
                                    tempExpr = Expression();
                                    if (lexer.Token() == MySqlToken.KwAsc)
                                    {
                                        lexer.NextToken();
                                    }
                                    else
                                    {
                                        if (lexer.Token() == MySqlToken.KwDesc)
                                        {
                                            isDesc = true;
                                            lexer.NextToken();
                                        }
                                    }
                                    for (appendedColumnNames = new List<IExpression>()
                                         ;
                                         lexer.Token() == MySqlToken.PuncComma;)
                                    {
                                        lexer.NextToken();
                                        appendedColumnNames.Add(Expression());
                                    }
                                    if (lexer.Token() != MySqlToken.KwSeparator)
                                    {
                                        break;
                                    }
                                    goto case MySqlToken.KwSeparator;
                                }

                                case MySqlToken.KwSeparator:
                                {
                                    lexer.NextToken();
                                    tempSb = new StringBuilder();
                                    lexer.AppendStringContent(tempSb);
                                    tempStr = LiteralString.GetUnescapedString(tempSb.ToString());
                                    Match(MySqlToken.LiteralChars);
                                    break;
                                }
                            }
                            Match(MySqlToken.PuncRightParen);
                            return
                                new GroupConcat(tempGroupDistinct, tempExprList, tempExpr, isDesc, appendedColumnNames,
                                    tempStr).SetCacheEvalRst(cacheEvalRst);
                        }

                        case FunctionParsingStrategy.Char:
                        {
                            lexer.NextToken();
                            return FunctionChar();
                        }

                        case FunctionParsingStrategy.Convert:
                        {
                            lexer.NextToken();
                            return FunctionConvert();
                        }

                        case FunctionParsingStrategy.Extract:
                        {
                            lexer.NextToken();
                            return Extract();
                        }

                        case FunctionParsingStrategy.Timestampdiff:
                        {
                            lexer.NextToken();
                            return Timestampdiff();
                        }

                        case FunctionParsingStrategy.Timestampadd:
                        {
                            lexer.NextToken();
                            return Timestampadd();
                        }

                        case FunctionParsingStrategy.Ordinary:
                        {
                            return OrdinaryFunction(consumed, consumedUp);
                        }

                        case FunctionParsingStrategy.Default:
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
        /// <exception cref="System.SqlSyntaxErrorException" />
        private Pair<string, Pair<IExpression, IExpression>> Type4specialFunc()
        {
            IExpression exp1 = null;
            IExpression exp2 = null;
            // DATE
            // DATETIME
            // SIGNED [INTEGER]
            // TIME
            string typeName;
            switch (lexer.Token())
            {
                case MySqlToken.KwBinary:
                case MySqlToken.KwChar:
                {
                    typeName = lexer.Token().KeyWordToString();
                    if (lexer.NextToken() == MySqlToken.PuncLeftParen)
                    {
                        lexer.NextToken();
                        exp1 = Expression();
                        Match(MySqlToken.PuncRightParen);
                    }
                    return ConstructTypePair(typeName, exp1, exp2);
                }

                case MySqlToken.KwDecimal:
                {
                    typeName = lexer.Token().KeyWordToString();
                    if (lexer.NextToken() == MySqlToken.PuncLeftParen)
                    {
                        lexer.NextToken();
                        exp1 = Expression();
                        if (lexer.Token() == MySqlToken.PuncComma)
                        {
                            lexer.NextToken();
                            exp2 = Expression();
                        }
                        Match(MySqlToken.PuncRightParen);
                    }
                    return ConstructTypePair(typeName, exp1, exp2);
                }

                case MySqlToken.KwUnsigned:
                {
                    typeName = lexer.Token().KeyWordToString();
                    if (lexer.NextToken() == MySqlToken.KwInteger)
                    {
                        lexer.NextToken();
                    }
                    return ConstructTypePair(typeName, null, null);
                }

                case MySqlToken.Identifier:
                {
                    typeName = lexer.GetStringValueUppercase();
                    lexer.NextToken();
                    if ("SIGNED".Equals(typeName))
                    {
                        if (lexer.Token() == MySqlToken.KwInteger)
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
                    throw Err("invalide type name: " + lexer.GetStringValueUppercase());
                }
            }
        }

        private static Pair<string, Pair<IExpression, IExpression>> ConstructTypePair(string typeName, IExpression exp1,
                                                                                      IExpression exp2)
        {
            return new Pair<string, Pair<IExpression, IExpression>>(typeName,
                new Pair<IExpression, IExpression>(exp1, exp2));
        }

        /// <summary>id has been consumed.</summary>
        /// <remarks>
        ///     id has been consumed. id must be a function name. current token must be
        ///     <see cref="MySqlToken.PuncLeftParen" />
        /// </remarks>
        /// <param name="idUpper">must be name of a function</param>
        /// <returns>never null</returns>
        /// <exception cref="System.SqlSyntaxErrorException" />
        private FunctionExpression OrdinaryFunction(string id, string idUpper)
        {
            idUpper = IdentifierExpr.UnescapeName(idUpper);
            Match(MySqlToken.PuncLeftParen);
            FunctionExpression funcExpr;
            if (lexer.Token() == MySqlToken.PuncRightParen)
            {
                lexer.NextToken();
                funcExpr = functionManager.CreateFunctionExpression(idUpper, null);
            }
            else
            {
                var args = ExpressionList(new List<IExpression>());
                funcExpr = functionManager.CreateFunctionExpression(idUpper, args);
            }
            if (funcExpr == null)
            {
                throw new SqlSyntaxErrorException(id + "() is not a function");
            }
            funcExpr.SetCacheEvalRst(cacheEvalRst);
            return funcExpr;
        }

        /// <summary>first <code>MATCH</code> has been consumed</summary>
        /// <exception cref="System.SqlSyntaxErrorException" />
        private IExpression MatchExpression()
        {
            Match(MySqlToken.PuncLeftParen);
            var colList = ExpressionList(new List<IExpression>());
            MatchIdentifier("AGAINST");
            Match(MySqlToken.PuncLeftParen);
            var pattern = Expression();
            var modifier = MatchModifier.Default;
            switch (lexer.Token())
            {
                case MySqlToken.KwWith:
                {
                    lexer.NextToken();
                    Match(MySqlToken.Identifier);
                    Match(MySqlToken.Identifier);
                    modifier = MatchModifier.WithQueryExpansion;
                    break;
                }

                case MySqlToken.KwIn:
                {
                    switch (lexer.NextToken())
                    {
                        case MySqlToken.KwNatural:
                        {
                            lexer.NextToken();
                            MatchIdentifier("LANGUAGE");
                            MatchIdentifier("MODE");
                            if (lexer.Token() == MySqlToken.KwWith)
                            {
                                lexer.NextToken();
                                lexer.NextToken();
                                lexer.NextToken();
                                modifier = MatchModifier.InNaturalLanguageModeWithQueryExpansion;
                            }
                            else
                            {
                                modifier = MatchModifier.InNaturalLanguageMode;
                            }
                            break;
                        }

                        default:
                        {
                            MatchIdentifier("BOOLEAN");
                            MatchIdentifier("MODE");
                            modifier = MatchModifier.InBooleanMode;
                            break;
                        }
                    }
                    break;
                }
            }
            Match(MySqlToken.PuncRightParen);
            return new MatchExpr(colList, pattern, modifier).SetCacheEvalRst(cacheEvalRst);
        }

        /// <summary>first <code>INTERVAL</code> has been consumed</summary>
        /// <exception cref="System.SqlSyntaxErrorException" />
        private IExpression IntervalExpression()
        {
            IExpression fstExpr;
            IList<IExpression> argList = null;
            if (lexer.Token() == MySqlToken.PuncLeftParen)
            {
                if (lexer.NextToken() == MySqlToken.KwSelect)
                {
                    fstExpr = SubQuery();
                    Match(MySqlToken.PuncRightParen);
                }
                else
                {
                    fstExpr = Expression();
                    if (lexer.Token() == MySqlToken.PuncComma)
                    {
                        lexer.NextToken();
                        argList = new List<IExpression>();
                        argList.Add(fstExpr);
                        argList = ExpressionList(argList);
                    }
                    else
                    {
                        Match(MySqlToken.PuncRightParen);
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
            return new IntervalPrimary(fstExpr, IntervalPrimaryUnit()).SetCacheEvalRst(cacheEvalRst);
        }

        /// <exception cref="System.SqlSyntaxErrorException" />
        private Unit IntervalPrimaryUnit()
        {
            switch (lexer.Token())
            {
                case MySqlToken.KwSecondMicrosecond:
                {
                    lexer.NextToken();
                    return Unit.SecondMicrosecond;
                }

                case MySqlToken.KwMinuteMicrosecond:
                {
                    lexer.NextToken();
                    return Unit.MinuteMicrosecond;
                }

                case MySqlToken.KwMinuteSecond:
                {
                    lexer.NextToken();
                    return Unit.MinuteSecond;
                }

                case MySqlToken.KwHourMicrosecond:
                {
                    lexer.NextToken();
                    return Unit.HourMicrosecond;
                }

                case MySqlToken.KwHourSecond:
                {
                    lexer.NextToken();
                    return Unit.HourSecond;
                }

                case MySqlToken.KwHourMinute:
                {
                    lexer.NextToken();
                    return Unit.HourMinute;
                }

                case MySqlToken.KwDayMicrosecond:
                {
                    lexer.NextToken();
                    return Unit.DayMicrosecond;
                }

                case MySqlToken.KwDaySecond:
                {
                    lexer.NextToken();
                    return Unit.DaySecond;
                }

                case MySqlToken.KwDayMinute:
                {
                    lexer.NextToken();
                    return Unit.DayMinute;
                }

                case MySqlToken.KwDayHour:
                {
                    lexer.NextToken();
                    return Unit.DayHour;
                }

                case MySqlToken.KwYearMonth:
                {
                    lexer.NextToken();
                    return Unit.YearMonth;
                }

                case MySqlToken.Identifier:
                {
                    var unitText = lexer.GetStringValueUppercase();
                    var unit = IntervalPrimary.GetIntervalUnit(unitText);
                    if (unit != Unit.None)
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
        /// <exception cref="System.SqlSyntaxErrorException" />
        private IExpression CaseWhenExpression()
        {
            IExpression comparee = null;
            if (lexer.Token() != MySqlToken.KwWhen)
            {
                comparee = Expression();
            }
            IList<Pair<IExpression, IExpression>> list = new List<Pair<IExpression, IExpression>>();
            for (; lexer.Token() == MySqlToken.KwWhen;)
            {
                lexer.NextToken();
                var when = Expression();
                Match(MySqlToken.KwThen);
                var then = Expression();
                if (when == null || then == null)
                {
                    throw Err("when or then is null in CASE WHEN expression");
                }
                list.Add(new Pair<IExpression, IExpression>(when, then));
            }
            if (list.IsEmpty())
            {
                throw Err("at least one WHEN ... THEN branch for CASE ... WHEN syntax");
            }
            IExpression elseValue = null;
            switch (lexer.Token())
            {
                case MySqlToken.KwElse:
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
        ///     first <code>'('</code> has been consumed. At least one element. Consume
        ///     last ')' after invocation <br />
        ///     <code>'(' expr (',' expr)* ')'</code>
        /// </remarks>
        /// <exception cref="System.SqlSyntaxErrorException" />
        private IList<IExpression> ExpressionList(IList<IExpression> exprList)
        {
            for (;;)
            {
                var expr = Expression();
                exprList.Add(expr);
                switch (lexer.Token())
                {
                    case MySqlToken.PuncComma:
                    {
                        lexer.NextToken();
                        break;
                    }

                    case MySqlToken.PuncRightParen:
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
        ///     first token is
        ///     <see cref="MySqlToken.KwSelect" />
        /// </summary>
        /// <exception cref="System.SqlSyntaxErrorException" />
        private IQueryExpression SubQuery()
        {
            if (selectParser == null)
            {
                selectParser = new MySqlDmlSelectParser(lexer, this);
            }
            return selectParser.Select();
        }
    }
}