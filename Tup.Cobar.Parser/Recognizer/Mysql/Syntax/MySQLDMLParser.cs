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
using Tup.Cobar.Parser.Ast.Expression.Misc;
using Tup.Cobar.Parser.Ast.Expression.Primary;
using Tup.Cobar.Parser.Ast.Fragment;
using Tup.Cobar.Parser.Ast.Fragment.Tableref;
using Tup.Cobar.Parser.Ast.Stmt.Dml;
using Tup.Cobar.Parser.Recognizer.Mysql.Lexer;

namespace Tup.Cobar.Parser.Recognizer.Mysql.Syntax
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public abstract class MySQLDMLParser : MySQLParser
    {
        protected MySQLExprParser exprParser;

        public MySQLDMLParser(MySQLLexer lexer, MySQLExprParser exprParser)
            : base(lexer)
        {
            this.exprParser = exprParser;
        }

        /// <summary>nothing has been pre-consumed</summary>
        /// <returns>null if there is no order by</returns>
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        protected virtual Tup.Cobar.Parser.Ast.Fragment.GroupBy GroupBy()
        {
            if (lexer.Token() != MySQLToken.KwGroup)
            {
                return null;
            }
            lexer.NextToken();
            Match(MySQLToken.KwBy);
            Tup.Cobar.Parser.Ast.Expression.Expression expr = exprParser.Expression();
            SortOrder order = SortOrder.Asc;
            Tup.Cobar.Parser.Ast.Fragment.GroupBy groupBy;
            switch (lexer.Token())
            {
                case MySQLToken.KwDesc:
                    {
                        order = SortOrder.Desc;
                        goto case MySQLToken.KwAsc;
                    }

                case MySQLToken.KwAsc:
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
                case MySQLToken.KwWith:
                    {
                        lexer.NextToken();
                        MatchIdentifier("ROLLUP");
                        return new Tup.Cobar.Parser.Ast.Fragment.GroupBy(expr, order, true);
                    }

                case MySQLToken.PuncComma:
                    {
                        break;
                    }

                default:
                    {
                        return new Tup.Cobar.Parser.Ast.Fragment.GroupBy(expr, order, false);
                    }
            }
            for (groupBy = new Tup.Cobar.Parser.Ast.Fragment.GroupBy().AddOrderByItem(expr, order
                ); lexer.Token() == MySQLToken.PuncComma;)
            {
                lexer.NextToken();
                order = SortOrder.Asc;
                expr = exprParser.Expression();
                switch (lexer.Token())
                {
                    case MySQLToken.KwDesc:
                        {
                            order = SortOrder.Desc;
                            goto case MySQLToken.KwAsc;
                        }

                    case MySQLToken.KwAsc:
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
                if (lexer.Token() == MySQLToken.KwWith)
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
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        protected virtual Tup.Cobar.Parser.Ast.Fragment.OrderBy OrderBy()
        {
            if (lexer.Token() != MySQLToken.KwOrder)
            {
                return null;
            }
            lexer.NextToken();
            Match(MySQLToken.KwBy);
            Tup.Cobar.Parser.Ast.Expression.Expression expr = exprParser.Expression();
            SortOrder order = SortOrder.Asc;
            Tup.Cobar.Parser.Ast.Fragment.OrderBy orderBy;
            switch (lexer.Token())
            {
                case MySQLToken.KwDesc:
                    {
                        order = SortOrder.Desc;
                        goto case MySQLToken.KwAsc;
                    }

                case MySQLToken.KwAsc:
                    {
                        if (lexer.NextToken() != MySQLToken.PuncComma)
                        {
                            return new Tup.Cobar.Parser.Ast.Fragment.OrderBy(expr, order);
                        }
                        goto case MySQLToken.PuncComma;
                    }

                case MySQLToken.PuncComma:
                    {
                        orderBy = new Tup.Cobar.Parser.Ast.Fragment.OrderBy();
                        orderBy.AddOrderByItem(expr, order);
                        break;
                    }

                default:
                    {
                        return new Tup.Cobar.Parser.Ast.Fragment.OrderBy(expr, order);
                    }
            }
            for (; lexer.Token() == MySQLToken.PuncComma;)
            {
                lexer.NextToken();
                order = SortOrder.Asc;
                expr = exprParser.Expression();
                switch (lexer.Token())
                {
                    case MySQLToken.KwDesc:
                        {
                            order = SortOrder.Desc;
                            goto case MySQLToken.KwAsc;
                        }

                    case MySQLToken.KwAsc:
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
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        protected virtual IList<Identifier> BuildIdList(Identifier id)
        {
            if (lexer.Token() != MySQLToken.PuncComma)
            {
                IList<Identifier> list = new List<Identifier>(1);
                list.Add(id);
                return list;
            }
            IList<Identifier> list_1 = new List<Identifier>();
            list_1.Add(id);
            for (; lexer.Token() == MySQLToken.PuncComma;)
            {
                lexer.NextToken();
                id = Identifier();
                list_1.Add(id);
            }
            return list_1;
        }

        /// <summary><code>(id (',' id)*)?</code></summary>
        /// <returns>
        /// never null or empty.
        /// <see cref="System.Collections.ArrayList{E}"/>
        /// is possible
        /// </returns>
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        protected virtual IList<Identifier> IdList()
        {
            return BuildIdList(Identifier());
        }

        /// <summary><code>( idName (',' idName)*)? ')'</code></summary>
        /// <returns>empty list if emtpy id list</returns>
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        protected virtual IList<string> IdNameList()
        {
            if (lexer.Token() != MySQLToken.Identifier)
            {
                Match(MySQLToken.PuncRightParen);
                return new List<string>(0);
            }
            IList<string> list;
            string str = lexer.StringValue();
            if (lexer.NextToken() == MySQLToken.PuncComma)
            {
                list = new List<string>();
                list.Add(str);
                for (; lexer.Token() == MySQLToken.PuncComma;)
                {
                    lexer.NextToken();
                    list.Add(lexer.StringValue());
                    Match(MySQLToken.Identifier);
                }
            }
            else
            {
                list = new List<string>(1);
                list.Add(str);
            }
            Match(MySQLToken.PuncRightParen);
            return list;
        }

        /// <returns>never null</returns>
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        protected virtual TableReferences TableRefs()
        {
            TableReference @ref = TableReference();
            return BuildTableReferences(@ref);
        }

        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        private TableReferences BuildTableReferences(TableReference @ref)
        {
            IList<TableReference> list;
            if (lexer.Token() == MySQLToken.PuncComma)
            {
                list = new List<TableReference>();
                list.Add(@ref);
                for (; lexer.Token() == MySQLToken.PuncComma;)
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

        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        private TableReference TableReference()
        {
            TableReference @ref = TableFactor();
            return BuildTableReference(@ref);
        }

        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        private TableReference BuildTableReference(TableReference @ref)
        {
            for (;;)
            {
                Tup.Cobar.Parser.Ast.Expression.Expression on;
                IList<string> @using;
                TableReference temp;
                bool isOut = false;
                bool isLeft = true;
                switch (lexer.Token())
                {
                    case MySQLToken.KwInner:
                    case MySQLToken.KwCross:
                        {
                            lexer.NextToken();
                            goto case MySQLToken.KwJoin;
                        }

                    case MySQLToken.KwJoin:
                        {
                            lexer.NextToken();
                            temp = TableFactor();
                            switch (lexer.Token())
                            {
                                case MySQLToken.KwOn:
                                    {
                                        lexer.NextToken();
                                        on = exprParser.Expression();
                                        @ref = new InnerJoin(@ref, temp, on);
                                        break;
                                    }

                                case MySQLToken.KwUsing:
                                    {
                                        lexer.NextToken();
                                        Match(MySQLToken.PuncLeftParen);
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

                    case MySQLToken.KwStraightJoin:
                        {
                            lexer.NextToken();
                            temp = TableFactor();
                            switch (lexer.Token())
                            {
                                case MySQLToken.KwOn:
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

                    case MySQLToken.KwRight:
                        {
                            isLeft = false;
                            goto case MySQLToken.KwLeft;
                        }

                    case MySQLToken.KwLeft:
                        {
                            lexer.NextToken();
                            if (lexer.Token() == MySQLToken.KwOuter)
                            {
                                lexer.NextToken();
                            }
                            Match(MySQLToken.KwJoin);
                            temp = TableReference();
                            switch (lexer.Token())
                            {
                                case MySQLToken.KwOn:
                                    {
                                        lexer.NextToken();
                                        on = exprParser.Expression();
                                        @ref = new OuterJoin(isLeft, @ref, temp, on);
                                        break;
                                    }

                                case MySQLToken.KwUsing:
                                    {
                                        lexer.NextToken();
                                        Match(MySQLToken.PuncLeftParen);
                                        @using = IdNameList();
                                        @ref = new OuterJoin(isLeft, @ref, temp, @using);
                                        break;
                                    }

                                default:
                                    {
                                        object condition = temp.RemoveLastConditionElement();
                                        if (condition is Tup.Cobar.Parser.Ast.Expression.Expression)
                                        {
                                            @ref = new OuterJoin(isLeft, @ref, temp, (Tup.Cobar.Parser.Ast.Expression.Expression
                                                )condition);
                                        }
                                        else
                                        {
                                            if (condition is IList)
                                            {
                                                @ref = new OuterJoin(isLeft, @ref, temp, (IList<string>)condition);
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

                    case MySQLToken.KwNatural:
                        {
                            lexer.NextToken();
                            switch (lexer.Token())
                            {
                                case MySQLToken.KwRight:
                                    {
                                        isLeft = false;
                                        goto case MySQLToken.KwLeft;
                                    }

                                case MySQLToken.KwLeft:
                                    {
                                        lexer.NextToken();
                                        if (lexer.Token() == MySQLToken.KwOuter)
                                        {
                                            lexer.NextToken();
                                        }
                                        isOut = true;
                                        goto case MySQLToken.KwJoin;
                                    }

                                case MySQLToken.KwJoin:
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

        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        private TableReference TableFactor()
        {
            string alias = null;
            switch (lexer.Token())
            {
                case MySQLToken.PuncLeftParen:
                    {
                        lexer.NextToken();
                        object @ref = TrsOrQuery();
                        Match(MySQLToken.PuncRightParen);
                        if (@ref is QueryExpression)
                        {
                            alias = As();
                            return new SubqueryFactor((QueryExpression)@ref, alias);
                        }
                        return (TableReferences)@ref;
                    }

                case MySQLToken.Identifier:
                    {
                        Identifier table = Identifier();
                        alias = As();
                        IList<IndexHint> hintList = HintList();
                        return new TableRefFactor(table, alias, hintList);
                    }

                default:
                    {
                        throw Err("unexpected token for tableFactor: " + lexer.Token());
                    }
            }
        }

        /// <returns>
        /// never empty. upper-case if id format.
        /// <code>"alias1" |"`al`ias1`" | "'alias1'" | "_latin1'alias1'"</code>
        /// </returns>
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        protected virtual string As()
        {
            if (lexer.Token() == MySQLToken.KwAs)
            {
                lexer.NextToken();
            }
            StringBuilder alias = new StringBuilder();
            bool id = false;
            if (lexer.Token() == MySQLToken.Identifier)
            {
                alias.Append(lexer.StringValueUppercase());
                id = true;
                lexer.NextToken();
            }
            if (lexer.Token() == MySQLToken.LiteralChars)
            {
                if (!id || id && alias[0] == '_')
                {
                    alias.Append(lexer.StringValue());
                    lexer.NextToken();
                }
            }
            return alias.Length > 0 ? alias.ToString() : null;
        }

        /// <returns>
        /// type of
        /// <see cref="Tup.Cobar.Parser.Ast.Expression.Misc.QueryExpression"/>
        /// or
        /// <see cref="TableReferences"/>
        /// </returns>
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        private object TrsOrQuery()
        {
            object @ref;
            switch (lexer.Token())
            {
                case MySQLToken.KwSelect:
                    {
                        DMLSelectStatement select = SelectPrimary();
                        return BuildUnionSelect(select);
                    }

                case MySQLToken.PuncLeftParen:
                    {
                        lexer.NextToken();
                        @ref = TrsOrQuery();
                        Match(MySQLToken.PuncRightParen);
                        if (@ref is QueryExpression)
                        {
                            if (@ref is DMLSelectStatement)
                            {
                                QueryExpression rst = BuildUnionSelect((DMLSelectStatement)@ref);
                                if (rst != @ref)
                                {
                                    return rst;
                                }
                            }
                            string alias = As();
                            if (alias != null)
                            {
                                @ref = new SubqueryFactor((QueryExpression)@ref, alias);
                            }
                            else
                            {
                                return @ref;
                            }
                        }
                        // ---- build factor complete---------------
                        @ref = BuildTableReference((TableReference)@ref);
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
            if (lexer.Token() == MySQLToken.PuncComma)
            {
                list = new List<TableReference>();
                list.Add((TableReference)@ref);
                for (; lexer.Token() == MySQLToken.PuncComma;)
                {
                    lexer.NextToken();
                    @ref = TableReference();
                    list.Add((TableReference)@ref);
                }
                return new TableReferences(list);
            }
            list = new List<TableReference>(1);
            list.Add((TableReference)@ref);
            return new TableReferences(list);
        }

        /// <returns>null if there is no hint</returns>
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        private IList<IndexHint> HintList()
        {
            IndexHint hint = Hint();
            if (hint == null)
            {
                return null;
            }
            IList<IndexHint> list;
            IndexHint hint2 = Hint();
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
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        private IndexHint Hint()
        {
            IndexHint.IndexAction action;
            switch (lexer.Token())
            {
                case MySQLToken.KwUse:
                    {
                        action = IndexHint.IndexAction.Use;
                        break;
                    }

                case MySQLToken.KwIgnore:
                    {
                        action = IndexHint.IndexAction.Ignore;
                        break;
                    }

                case MySQLToken.KwForce:
                    {
                        action = IndexHint.IndexAction.Force;
                        break;
                    }

                default:
                    {
                        return null;
                    }
            }
            IndexHint.IndexType type;
            switch (lexer.NextToken())
            {
                case MySQLToken.KwIndex:
                    {
                        type = IndexHint.IndexType.Index;
                        break;
                    }

                case MySQLToken.KwKey:
                    {
                        type = IndexHint.IndexType.Key;
                        break;
                    }

                default:
                    {
                        throw Err("must be INDEX or KEY for hint type, not " + lexer.Token());
                    }
            }
            IndexHint.IndexScope scope = IndexHint.IndexScope.All;
            if (lexer.NextToken() == MySQLToken.KwFor)
            {
                switch (lexer.NextToken())
                {
                    case MySQLToken.KwJoin:
                        {
                            lexer.NextToken();
                            scope = IndexHint.IndexScope.Join;
                            break;
                        }

                    case MySQLToken.KwOrder:
                        {
                            lexer.NextToken();
                            Match(MySQLToken.KwBy);
                            scope = IndexHint.IndexScope.OrderBy;
                            break;
                        }

                    case MySQLToken.KwGroup:
                        {
                            lexer.NextToken();
                            Match(MySQLToken.KwBy);
                            scope = IndexHint.IndexScope.GroupBy;
                            break;
                        }

                    default:
                        {
                            throw Err("must be JOIN or ORDER or GROUP for hint scope, not " + lexer.Token());
                        }
                }
            }
            Match(MySQLToken.PuncLeftParen);
            IList<string> indexList = IdNameList();
            return new IndexHint(action, type, scope, indexList);
        }

        /// <returns>argument itself if there is no union</returns>
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        protected virtual DMLQueryStatement BuildUnionSelect(DMLSelectStatement select)
        {
            if (lexer.Token() != MySQLToken.KwUnion)
            {
                return select;
            }
            DMLSelectUnionStatement union = new DMLSelectUnionStatement(select);
            for (; lexer.Token() == MySQLToken.KwUnion;)
            {
                lexer.NextToken();
                bool isAll = false;
                switch (lexer.Token())
                {
                    case MySQLToken.KwAll:
                        {
                            isAll = true;
                            goto case MySQLToken.KwDistinct;
                        }

                    case MySQLToken.KwDistinct:
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

        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        protected virtual DMLSelectStatement SelectPrimary()
        {
            switch (lexer.Token())
            {
                case MySQLToken.KwSelect:
                    {
                        return Select();
                    }

                case MySQLToken.PuncLeftParen:
                    {
                        lexer.NextToken();
                        DMLSelectStatement select = SelectPrimary();
                        Match(MySQLToken.PuncRightParen);
                        return select;
                    }

                default:
                    {
                        throw Err("unexpected token: " + lexer.Token());
                    }
            }
        }

        /// <summary>
        /// first token is
        /// <see cref="Tup.Cobar.Parser.Recognizer.Mysql.MySQLToken.KwSelect">SELECT</see>
        /// which has been scanned
        /// but not yet consumed
        /// </summary>
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        public virtual DMLSelectStatement Select()
        {
            return new MySQLDMLSelectParser(lexer, exprParser).Select();
        }
    }
}