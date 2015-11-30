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
using Sharpen;
using Tup.Cobar4Net.Parser.Recognizer.Mysql.Lexer;

namespace Tup.Cobar4Net.Parser.Recognizer.Mysql.Syntax
{
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    internal class SoloParser : MySqlParser
    {
        public SoloParser(MySqlLexer lexer)
            : base(lexer)
        {
        }

        /// <exception cref="System.SqlSyntaxErrorException" />
        public virtual Refs Refs()
        {
            var refs = new Refs();
            for (;;)
            {
                var @ref = Ref();
                refs.AddRef(@ref);
                if (lexer.Token() == MySqlToken.PuncComma)
                {
                    lexer.NextToken();
                }
                else
                {
                    return refs;
                }
            }
        }

        /// <exception cref="System.SqlSyntaxErrorException" />
        public virtual Ref BuildRef(Ref
                                        first)
        {
            for (; lexer.Token() == MySqlToken.KwJoin;)
            {
                lexer.NextToken();
                var temp = Factor();
                first = new Join(first, temp);
            }
            return first;
        }

        /// <exception cref="System.SqlSyntaxErrorException" />
        public virtual Ref Ref()
        {
            return BuildRef(Factor());
        }

        /// <exception cref="System.SqlSyntaxErrorException" />
        public virtual Ref Factor()
        {
            string alias;
            if (lexer.Token() == MySqlToken.PuncLeftParen)
            {
                lexer.NextToken();
                var queryRefs = RefsOrQuery();
                Match(MySqlToken.PuncRightParen);
                if (queryRefs is Query)
                {
                    Match(MySqlToken.KwAs);
                    alias = lexer.GetStringValue();
                    lexer.NextToken();
                    return new SubQuery((Query)queryRefs, alias);
                }
                return queryRefs;
            }
            var tableName = lexer.GetStringValue();
            lexer.NextToken();
            if (lexer.Token() == MySqlToken.KwAs)
            {
                lexer.NextToken();
                alias = lexer.GetStringValue();
                lexer.NextToken();
                return new Factor(tableName, alias);
            }
            return new Factor(tableName, null);
        }

        /// <summary>first <code>(</code> has been consumed</summary>
        /// <exception cref="System.SqlSyntaxErrorException" />
        public virtual Ref RefsOrQuery()
        {
            Ref temp;
            Refs rst;
            Union u;
            switch (lexer.Token())
            {
                case MySqlToken.KwSelect:
                {
                    u = new Union();
                    for (;;)
                    {
                        var s = SelectPrimary();
                        u.AddSelect(s);
                        if (lexer.Token() == MySqlToken.KwUnion)
                        {
                            lexer.NextToken();
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (u.selects.Count == 1)
                    {
                        return u.selects[0];
                    }
                    return u;
                }

                case MySqlToken.PuncLeftParen:
                {
                    lexer.NextToken();
                    temp = RefsOrQuery();
                    Match(MySqlToken.PuncRightParen);
                    if (temp is Query)
                    {
                        if (temp is Select)
                        {
                            if (lexer.Token() == MySqlToken.KwUnion)
                            {
                                u = new Union();
                                u.AddSelect((Select)temp);
                                while (lexer.Token() == MySqlToken.KwUnion)
                                {
                                    lexer.NextToken();
                                    temp = SelectPrimary();
                                    u.AddSelect((Select)temp);
                                }
                                return u;
                            }
                        }
                        if (lexer.Token() == MySqlToken.KwAs)
                        {
                            lexer.NextToken();
                            var alias = lexer.GetStringValue();
                            temp = new SubQuery((Query)temp, alias);
                            lexer.NextToken();
                        }
                        else
                        {
                            return temp;
                        }
                    }
                    // ---- build factor complete---------------
                    temp = BuildRef(temp);
                    // ---- build ref complete---------------
                    break;
                }

                default:
                {
                    temp = Ref();
                    break;
                }
            }
            if (lexer.Token() == MySqlToken.PuncComma)
            {
                rst = new Refs();
                rst.AddRef(temp);
                for (; lexer.Token() == MySqlToken.PuncComma;)
                {
                    lexer.NextToken();
                    temp = Ref();
                    rst.AddRef(temp);
                }
                return rst;
            }
            return temp;
        }

        /// <summary>first <code>SELECT</code> or <code>(</code> has not been consumed</summary>
        /// <exception cref="System.SqlSyntaxErrorException" />
        private Select SelectPrimary()
        {
            Select s = null;
            if (lexer.Token() == MySqlToken.PuncLeftParen)
            {
                lexer.NextToken();
                s = SelectPrimary();
                Match(MySqlToken.PuncRightParen);
                return s;
            }
            Match(MySqlToken.KwSelect);
            return new Select();
        }

        /// <exception cref="System.SqlSyntaxErrorException" />
        public static void Main(string[] args)
        {
            var sql =
                "   ( ( select union select union select)  as j join    (((select union (select)) as t    )   join t2 ) ,(select)as d), t3)";
            // String sql =
            // "((select) as s1, ((((   select  union select          ) as t2)) join (((t2),t4 as t))) ), t1 aS T1";
            // String sql =
            // "  (( select union select union select)  as j  ,(select)as d), t3";
            Console.Out.WriteLine(sql);
            var lexer = new MySqlLexer(sql);
            lexer.NextToken();
            var p = new SoloParser(lexer);
            var refs = p.Refs();
            Console.Out.WriteLine(refs);
        }
    }

    internal interface Ref
    {
    }

    internal class Factor : Ref
    {
        internal string alias;
        internal string tableName;

        public Factor(string tableName, string alias)
        {
            this.tableName = tableName;
            this.alias = alias;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(tableName);
            sb.Append(" AS ");
            sb.Append(alias);
            return sb.ToString();
        }
    }

    internal class SubQuery : Ref
    {
        internal string alias;
        internal Query u;

        public SubQuery(Query u, string alias)
        {
            this.u = u;
            this.alias = alias;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("(");
            sb.Append(u);
            sb.Append(") AS ");
            sb.Append(alias);
            return sb.ToString();
        }
    }

    internal class Join : Ref
    {
        internal Ref left;

        internal Ref right;

        public Join(Ref left, Ref right)
        {
            this.left = left;
            this.right = right;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("<");
            sb.Append(left);
            sb.Append(" JOIN ");
            sb.Append(right);
            sb.Append(">");
            return sb.ToString();
        }
    }

    internal class Refs : Ref
    {
        internal IList<Ref> refs = new List<Ref>();

        public virtual void AddRef(Ref @ref)
        {
            refs.Add(@ref);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("[");
            for (var i = 0; i < refs.Count; ++i)
            {
                if (i > 0)
                {
                    sb.Append(", ");
                }
                sb.Append(refs[i]);
            }
            sb.Append("]");
            return sb.ToString();
        }
    }

    internal interface Query
    {
    }

    internal class Union : Query, Ref
    {
        internal IList<Select> selects = new List<Select>();

        public virtual void AddSelect(Select select)
        {
            selects.Add(select);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var s in selects)
            {
                sb.Append(" UNION SELECT");
            }
            var rst = sb.ToString();
            var i = rst.IndexOf("UNION", StringComparison.Ordinal);
            if (i >= 0)
            {
                rst = Runtime.Substring(rst, i + "UNION".Length);
            }
            return rst;
        }
    }

    internal class Select : Query, Ref
    {
        public override string ToString()
        {
            return "SELECT";
        }
    }
}