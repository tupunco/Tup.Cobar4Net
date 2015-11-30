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

using System.Collections;
using System.Collections.Generic;
using System.Text;
using Tup.Cobar4Net.Parser.Ast.Expression;
using Tup.Cobar4Net.Parser.Ast.Expression.Misc;
using Tup.Cobar4Net.Parser.Ast.Expression.Primary;
using Tup.Cobar4Net.Parser.Ast.Fragment;
using Tup.Cobar4Net.Parser.Ast.Fragment.Tableref;
using Tup.Cobar4Net.Parser.Ast.Stmt.Dml;
using Tup.Cobar4Net.Parser.Recognizer.Mysql.Lexer;

namespace Tup.Cobar4Net.Parser.Recognizer.Mysql.Syntax
{
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public abstract class MySqlDmlParser : MySqlParser
    {
        protected MySqlExprParser exprParser;

        public MySqlDmlParser(MySqlLexer lexer, MySqlExprParser exprParser)
            : base(lexer)
        {
            this.exprParser = exprParser;
        }

        /// <summary>nothing has been pre-consumed</summary>
        /// <returns>null if there is no order by</returns>
        /// <exception cref="System.SqlSyntaxErrorException" />
        protected internal virtual GroupBy GroupBy()
        {
            if (lexer.Token() != MySqlToken.KwGroup)
            {
                return null;
            }
            lexer.NextToken();
            Match(MySqlToken.KwBy);
            var expr = exprParser.Expression();
            var order = SortOrder.Asc;
            GroupBy groupBy;
            switch (lexer.Token())
            {
                case MySqlToken.KwDesc:
                {
                    order = SortOrder.Desc;
                    goto case MySqlToken.KwAsc;
                }

                case MySqlToken.KwAsc:
                {
                    lexer.NextToken();
                    goto default;
                }

                default:
                {
                    break;
                }
            }
            switch (lexer.Token())
            {
                case MySqlToken.KwWith:
                {
                    lexer.NextToken();
                    MatchIdentifier("ROLLUP");
                    return new GroupBy(expr, order, true);
                }

                case MySqlToken.PuncComma:
                {
                    break;
                }

                default:
                {
                    return new GroupBy(expr, order, false);
                }
            }
            for (groupBy = new GroupBy().AddOrderByItem(expr, order);
                lexer.Token() == MySqlToken.PuncComma;)
            {
                lexer.NextToken();
                order = SortOrder.Asc;
                expr = exprParser.Expression();
                switch (lexer.Token())
                {
                    case MySqlToken.KwDesc:
                    {
                        order = SortOrder.Desc;
                        goto case MySqlToken.KwAsc;
                    }

                    case MySqlToken.KwAsc:
                    {
                        lexer.NextToken();
                        goto default;
                    }

                    default:
                    {
                        break;
                    }
                }
                groupBy.AddOrderByItem(expr, order);
                if (lexer.Token() == MySqlToken.KwWith)
                {
                    lexer.NextToken();
                    MatchIdentifier("ROLLUP");
                    return groupBy.SetWithRollup();
                }
            }
            return groupBy;
        }

        /// <summary>nothing has been pre-consumed</summary>
        /// <returns>null if there is no order by</returns>
        /// <exception cref="System.SqlSyntaxErrorException" />
        protected internal virtual OrderBy OrderBy()
        {
            if (lexer.Token() != MySqlToken.KwOrder)
            {
                return null;
            }
            lexer.NextToken();
            Match(MySqlToken.KwBy);
            var expr = exprParser.Expression();
            var order = SortOrder.Asc;
            OrderBy orderBy;
            switch (lexer.Token())
            {
                case MySqlToken.KwDesc:
                {
                    order = SortOrder.Desc;
                    goto case MySqlToken.KwAsc;
                }

                case MySqlToken.KwAsc:
                {
                    if (lexer.NextToken() != MySqlToken.PuncComma)
                    {
                        return new OrderBy(expr, order);
                    }
                    goto case MySqlToken.PuncComma;
                }

                case MySqlToken.PuncComma:
                {
                    orderBy = new OrderBy();
                    orderBy.AddOrderByItem(expr, order);
                    break;
                }

                default:
                {
                    return new OrderBy(expr, order);
                }
            }
            for (; lexer.Token() == MySqlToken.PuncComma;)
            {
                lexer.NextToken();
                order = SortOrder.Asc;
                expr = exprParser.Expression();
                switch (lexer.Token())
                {
                    case MySqlToken.KwDesc:
                    {
                        order = SortOrder.Desc;
                        goto case MySqlToken.KwAsc;
                    }

                    case MySqlToken.KwAsc:
                    {
                        lexer.NextToken();
                        break;
                    }
                }
                orderBy.AddOrderByItem(expr, order);
            }
            return orderBy;
        }

        /// <param name="id">never null</param>
        /// <exception cref="System.SqlSyntaxErrorException" />
        protected virtual IList<Identifier> BuildIdList(Identifier id)
        {
            if (lexer.Token() != MySqlToken.PuncComma)
            {
                IList<Identifier> list = new List<Identifier>(1);
                list.Add(id);
                return list;
            }
            IList<Identifier> list_1 = new List<Identifier>();
            list_1.Add(id);
            for (; lexer.Token() == MySqlToken.PuncComma;)
            {
                lexer.NextToken();
                id = Identifier();
                list_1.Add(id);
            }
            return list_1;
        }

        /// <summary>
        ///     <code>(id (',' id)*)?</code>
        /// </summary>
        /// <returns>
        ///     never null or empty.
        ///     <see cref="System.Collections.ArrayList{E}" />
        ///     is possible
        /// </returns>
        /// <exception cref="System.SqlSyntaxErrorException" />
        protected virtual IList<Identifier> IdList()
        {
            return BuildIdList(Identifier());
        }

        /// <summary>
        ///     <code>( idName (',' idName)*)? ')'</code>
        /// </summary>
        /// <returns>empty list if emtpy id list</returns>
        /// <exception cref="System.SqlSyntaxErrorException" />
        protected virtual IList<string> IdNameList()
        {
            if (lexer.Token() != MySqlToken.Identifier)
            {
                Match(MySqlToken.PuncRightParen);
                return new List<string>(0);
            }
            IList<string> list;
            var str = lexer.GetStringValue();
            if (lexer.NextToken() == MySqlToken.PuncComma)
            {
                list = new List<string>();
                list.Add(str);
                for (; lexer.Token() == MySqlToken.PuncComma;)
                {
                    lexer.NextToken();
                    list.Add(lexer.GetStringValue());
                    Match(MySqlToken.Identifier);
                }
            }
            else
            {
                list = new List<string>(1);
                list.Add(str);
            }
            Match(MySqlToken.PuncRightParen);
            return list;
        }

        /// <returns>never null</returns>
        /// <exception cref="System.SqlSyntaxErrorException" />
        protected internal virtual TableReferences TableRefs()
        {
            var @ref = TableReference();
            return BuildTableReferences(@ref);
        }

        /// <exception cref="System.SqlSyntaxErrorException" />
        private TableReferences BuildTableReferences(TableReference @ref)
        {
            IList<TableReference> list;
            if (lexer.Token() == MySqlToken.PuncComma)
            {
                list = new List<TableReference>();
                list.Add(@ref);
                for (; lexer.Token() == MySqlToken.PuncComma;)
                {
                    lexer.NextToken();
                    @ref = TableReference();
                    list.Add(@ref);
                }
            }
            else
            {
                list = new List<TableReference>(1);
                list.Add(@ref);
            }
            return new TableReferences(list);
        }

        /// <exception cref="System.SqlSyntaxErrorException" />
        private TableReference TableReference()
        {
            var @ref = TableFactor();
            return BuildTableReference(@ref);
        }

        /// <exception cref="System.SqlSyntaxErrorException" />
        private TableReference BuildTableReference(TableReference @ref)
        {
            for (;;)
            {
                IExpression on;
                IList<string> @using;
                TableReference temp;
                var isOut = false;
                var isLeft = true;
                switch (lexer.Token())
                {
                    case MySqlToken.KwInner:
                    case MySqlToken.KwCross:
                    {
                        lexer.NextToken();
                        goto case MySqlToken.KwJoin;
                    }

                    case MySqlToken.KwJoin:
                    {
                        lexer.NextToken();
                        temp = TableFactor();
                        switch (lexer.Token())
                        {
                            case MySqlToken.KwOn:
                            {
                                lexer.NextToken();
                                on = exprParser.Expression();
                                @ref = new InnerJoin(@ref, temp, on);
                                break;
                            }

                            case MySqlToken.KwUsing:
                            {
                                lexer.NextToken();
                                Match(MySqlToken.PuncLeftParen);
                                @using = IdNameList();
                                @ref = new InnerJoin(@ref, temp, @using);
                                break;
                            }

                            default:
                            {
                                @ref = new InnerJoin(@ref, temp);
                                break;
                            }
                        }
                        break;
                    }

                    case MySqlToken.KwStraightJoin:
                    {
                        lexer.NextToken();
                        temp = TableFactor();
                        switch (lexer.Token())
                        {
                            case MySqlToken.KwOn:
                            {
                                lexer.NextToken();
                                on = exprParser.Expression();
                                @ref = new StraightJoin(@ref, temp, on);
                                break;
                            }

                            default:
                            {
                                @ref = new StraightJoin(@ref, temp);
                                break;
                            }
                        }
                        break;
                    }

                    case MySqlToken.KwRight:
                    {
                        isLeft = false;
                        goto case MySqlToken.KwLeft;
                    }

                    case MySqlToken.KwLeft:
                    {
                        lexer.NextToken();
                        if (lexer.Token() == MySqlToken.KwOuter)
                        {
                            lexer.NextToken();
                        }
                        Match(MySqlToken.KwJoin);
                        temp = TableReference();
                        switch (lexer.Token())
                        {
                            case MySqlToken.KwOn:
                            {
                                lexer.NextToken();
                                on = exprParser.Expression();
                                @ref = new OuterJoin(isLeft, @ref, temp, on);
                                break;
                            }

                            case MySqlToken.KwUsing:
                            {
                                lexer.NextToken();
                                Match(MySqlToken.PuncLeftParen);
                                @using = IdNameList();
                                @ref = new OuterJoin(isLeft, @ref, temp, @using);
                                break;
                            }

                            default:
                            {
                                var condition = temp.RemoveLastConditionElement();
                                if (condition is IExpression)
                                {
                                    @ref = new OuterJoin(isLeft, @ref, temp, (IExpression) condition);
                                }
                                else
                                {
                                    if (condition is IList)
                                    {
                                        @ref = new OuterJoin(isLeft, @ref, temp, (IList<string>) condition);
                                    }
                                    else
                                    {
                                        throw Err("conditionExpr cannot be null for outer join");
                                    }
                                }
                                break;
                            }
                        }
                        break;
                    }

                    case MySqlToken.KwNatural:
                    {
                        lexer.NextToken();
                        switch (lexer.Token())
                        {
                            case MySqlToken.KwRight:
                            {
                                isLeft = false;
                                goto case MySqlToken.KwLeft;
                            }

                            case MySqlToken.KwLeft:
                            {
                                lexer.NextToken();
                                if (lexer.Token() == MySqlToken.KwOuter)
                                {
                                    lexer.NextToken();
                                }
                                isOut = true;
                                goto case MySqlToken.KwJoin;
                            }

                            case MySqlToken.KwJoin:
                            {
                                lexer.NextToken();
                                temp = TableFactor();
                                @ref = new NaturalJoin(isOut, isLeft, @ref, temp);
                                break;
                            }

                            default:
                            {
                                throw Err("unexpected token after NATURAL for natural join:" + lexer.Token());
                            }
                        }
                        break;
                    }

                    default:
                    {
                        return @ref;
                    }
                }
            }
        }

        /// <exception cref="System.SqlSyntaxErrorException" />
        private TableReference TableFactor()
        {
            string alias = null;
            switch (lexer.Token())
            {
                case MySqlToken.PuncLeftParen:
                {
                    lexer.NextToken();
                    var @ref = TrsOrQuery();
                    Match(MySqlToken.PuncRightParen);
                    if (@ref is IQueryExpression)
                    {
                        alias = As();
                        return new SubqueryFactor((IQueryExpression) @ref, alias);
                    }
                    return (TableReferences) @ref;
                }

                case MySqlToken.Identifier:
                {
                    var table = Identifier();
                    alias = As();
                    var hintList = HintList();
                    return new TableRefFactor(table, alias, hintList);
                }

                default:
                {
                    throw Err("unexpected token for tableFactor: " + lexer.Token());
                }
            }
        }

        /// <returns>
        ///     never empty. upper-case if id format.
        ///     <code>"alias1" |"`al`ias1`" | "'alias1'" | "_latin1'alias1'"</code>
        /// </returns>
        /// <exception cref="System.SqlSyntaxErrorException" />
        protected virtual string As()
        {
            if (lexer.Token() == MySqlToken.KwAs)
            {
                lexer.NextToken();
            }
            var alias = new StringBuilder();
            var id = false;
            if (lexer.Token() == MySqlToken.Identifier)
            {
                alias.Append(lexer.GetStringValueUppercase());
                id = true;
                lexer.NextToken();
            }
            if (lexer.Token() == MySqlToken.LiteralChars)
            {
                if (!id || id && alias[0] == '_')
                {
                    alias.Append(lexer.GetStringValue());
                    lexer.NextToken();
                }
            }
            return alias.Length > 0 ? alias.ToString() : null;
        }

        /// <returns>
        ///     type of
        ///     <see cref="Tup.Cobar4Net.Parser.Ast.Expression.Misc.IQueryExpression" />
        ///     or
        ///     <see cref="TableReferences" />
        /// </returns>
        /// <exception cref="System.SqlSyntaxErrorException" />
        private object TrsOrQuery()
        {
            object @ref;
            switch (lexer.Token())
            {
                case MySqlToken.KwSelect:
                {
                    var select = SelectPrimary();
                    return BuildUnionSelect(select);
                }

                case MySqlToken.PuncLeftParen:
                {
                    lexer.NextToken();
                    @ref = TrsOrQuery();
                    Match(MySqlToken.PuncRightParen);
                    if (@ref is IQueryExpression)
                    {
                        if (@ref is DmlSelectStatement)
                        {
                            IQueryExpression rst = BuildUnionSelect((DmlSelectStatement) @ref);
                            if (rst != @ref)
                            {
                                return rst;
                            }
                        }
                        var alias = As();
                        if (alias != null)
                        {
                            @ref = new SubqueryFactor((IQueryExpression) @ref, alias);
                        }
                        else
                        {
                            return @ref;
                        }
                    }
                    // ---- build factor complete---------------
                    @ref = BuildTableReference((TableReference) @ref);
                    // ---- build ref complete---------------
                    break;
                }

                default:
                {
                    @ref = TableReference();
                    break;
                }
            }
            IList<TableReference> list;
            if (lexer.Token() == MySqlToken.PuncComma)
            {
                list = new List<TableReference>();
                list.Add((TableReference) @ref);
                for (; lexer.Token() == MySqlToken.PuncComma;)
                {
                    lexer.NextToken();
                    @ref = TableReference();
                    list.Add((TableReference) @ref);
                }
                return new TableReferences(list);
            }
            list = new List<TableReference>(1);
            list.Add((TableReference) @ref);
            return new TableReferences(list);
        }

        /// <returns>null if there is no hint</returns>
        /// <exception cref="System.SqlSyntaxErrorException" />
        private IList<IndexHint> HintList()
        {
            var hint = Hint();
            if (hint == null)
            {
                return null;
            }
            IList<IndexHint> list;
            var hint2 = Hint();
            if (hint2 == null)
            {
                list = new List<IndexHint>(1);
                list.Add(hint);
                return list;
            }
            list = new List<IndexHint>();
            list.Add(hint);
            list.Add(hint2);
            for (; (hint2 = Hint()) != null; list.Add(hint2))
            {
            }
            return list;
        }

        /// <returns>null if there is no hint</returns>
        /// <exception cref="System.SqlSyntaxErrorException" />
        private IndexHint Hint()
        {
            IndexHintAction _hintAction;
            switch (lexer.Token())
            {
                case MySqlToken.KwUse:
                {
                    _hintAction = IndexHintAction.Use;
                    break;
                }

                case MySqlToken.KwIgnore:
                {
                    _hintAction = IndexHintAction.Ignore;
                    break;
                }

                case MySqlToken.KwForce:
                {
                    _hintAction = IndexHintAction.Force;
                    break;
                }

                default:
                {
                    return null;
                }
            }
            IndexHintType _hintType;
            switch (lexer.NextToken())
            {
                case MySqlToken.KwIndex:
                {
                    _hintType = IndexHintType.Index;
                    break;
                }

                case MySqlToken.KwKey:
                {
                    _hintType = IndexHintType.Key;
                    break;
                }

                default:
                {
                    throw Err("must be INDEX or KEY for hint _hintType, not " + lexer.Token());
                }
            }
            var scope = IndexHintScope.All;
            if (lexer.NextToken() == MySqlToken.KwFor)
            {
                switch (lexer.NextToken())
                {
                    case MySqlToken.KwJoin:
                    {
                        lexer.NextToken();
                        scope = IndexHintScope.Join;
                        break;
                    }

                    case MySqlToken.KwOrder:
                    {
                        lexer.NextToken();
                        Match(MySqlToken.KwBy);
                        scope = IndexHintScope.OrderBy;
                        break;
                    }

                    case MySqlToken.KwGroup:
                    {
                        lexer.NextToken();
                        Match(MySqlToken.KwBy);
                        scope = IndexHintScope.GroupBy;
                        break;
                    }

                    default:
                    {
                        throw Err("must be JOIN or ORDER or GROUP for hint _hintScope, not " + lexer.Token());
                    }
                }
            }
            Match(MySqlToken.PuncLeftParen);
            var indexList = IdNameList();
            return new IndexHint(_hintAction, _hintType, scope, indexList);
        }

        /// <returns>argument itself if there is no union</returns>
        /// <exception cref="System.SqlSyntaxErrorException" />
        protected virtual DmlQueryStatement BuildUnionSelect(DmlSelectStatement select)
        {
            if (lexer.Token() != MySqlToken.KwUnion)
            {
                return select;
            }
            var union = new DmlSelectUnionStatement(select);
            for (; lexer.Token() == MySqlToken.KwUnion;)
            {
                lexer.NextToken();
                var isAll = false;
                switch (lexer.Token())
                {
                    case MySqlToken.KwAll:
                    {
                        isAll = true;
                        goto case MySqlToken.KwDistinct;
                    }

                    case MySqlToken.KwDistinct:
                    {
                        lexer.NextToken();
                        break;
                    }
                }
                select = SelectPrimary();
                union.AddSelect(select, isAll);
            }
            union.SetOrderBy(OrderBy()).SetLimit(Limit());
            return union;
        }

        /// <exception cref="System.SqlSyntaxErrorException" />
        protected virtual DmlSelectStatement SelectPrimary()
        {
            switch (lexer.Token())
            {
                case MySqlToken.KwSelect:
                {
                    return Select();
                }

                case MySqlToken.PuncLeftParen:
                {
                    lexer.NextToken();
                    var select = SelectPrimary();
                    Match(MySqlToken.PuncRightParen);
                    return select;
                }

                default:
                {
                    throw Err("unexpected token: " + lexer.Token());
                }
            }
        }

        /// <summary>
        ///     first token is
        ///     <see cref="MySqlToken.KwSelect">SELECT</see>
        ///     which has been scanned
        ///     but not yet consumed
        /// </summary>
        /// <exception cref="System.SqlSyntaxErrorException" />
        public virtual DmlSelectStatement Select()
        {
            return new MySqlDmlSelectParser(lexer, exprParser).Select();
        }
    }
}