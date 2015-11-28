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
using NUnit.Framework;

using System.Collections.Generic;
using System.Text;
using Tup.Cobar4Net.Parser.Recognizer.Mysql.Lexer;

namespace Tup.Cobar4Net.Parser.Recognizer.Mysql.Syntax
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    internal class SoloParser : MySQLParser
    {
        public SoloParser(MySQLLexer lexer)
            : base(lexer)
        {
        }

        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        public virtual Tup.Cobar4Net.Parser.Recognizer.Mysql.Syntax.Refs Refs()
        {
            Tup.Cobar4Net.Parser.Recognizer.Mysql.Syntax.Refs refs = new Tup.Cobar4Net.Parser.Recognizer.Mysql.Syntax.Refs
                ();
            for (;;)
            {
                Tup.Cobar4Net.Parser.Recognizer.Mysql.Syntax.Ref @ref = Ref();
                refs.AddRef(@ref);
                if (lexer.Token() == MySQLToken.PuncComma)
                {
                    lexer.NextToken();
                }
                else
                {
                    return refs;
                }
            }
        }

        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        public virtual Tup.Cobar4Net.Parser.Recognizer.Mysql.Syntax.Ref BuildRef(Tup.Cobar4Net.Parser.Recognizer.Mysql.Syntax.Ref
             first)
        {
            for (; lexer.Token() == MySQLToken.KwJoin;)
            {
                lexer.NextToken();
                Tup.Cobar4Net.Parser.Recognizer.Mysql.Syntax.Ref temp = Factor();
                first = new Join(first, temp);
            }
            return first;
        }

        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        public virtual Tup.Cobar4Net.Parser.Recognizer.Mysql.Syntax.Ref Ref()
        {
            return BuildRef(Factor());
        }

        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        public virtual Tup.Cobar4Net.Parser.Recognizer.Mysql.Syntax.Ref Factor()
        {
            string alias;
            if (lexer.Token() == MySQLToken.PuncLeftParen)
            {
                lexer.NextToken();
                Tup.Cobar4Net.Parser.Recognizer.Mysql.Syntax.Ref queryRefs = RefsOrQuery();
                Match(MySQLToken.PuncRightParen);
                if (queryRefs is Query)
                {
                    Match(MySQLToken.KwAs);
                    alias = lexer.StringValue();
                    lexer.NextToken();
                    return new SubQuery((Query)queryRefs, alias);
                }
                return queryRefs;
            }
            string tableName = lexer.StringValue();
            lexer.NextToken();
            if (lexer.Token() == MySQLToken.KwAs)
            {
                lexer.NextToken();
                alias = lexer.StringValue();
                lexer.NextToken();
                return new Tup.Cobar4Net.Parser.Recognizer.Mysql.Syntax.Factor(tableName, alias);
            }
            return new Tup.Cobar4Net.Parser.Recognizer.Mysql.Syntax.Factor(tableName, null);
        }

        /// <summary>first <code>(</code> has been consumed</summary>
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        public virtual Tup.Cobar4Net.Parser.Recognizer.Mysql.Syntax.Ref RefsOrQuery()
        {
            Tup.Cobar4Net.Parser.Recognizer.Mysql.Syntax.Ref temp;
            Tup.Cobar4Net.Parser.Recognizer.Mysql.Syntax.Refs rst;
            Union u;
            switch (lexer.Token())
            {
                case MySQLToken.KwSelect:
                    {
                        u = new Union();
                        for (;;)
                        {
                            Select s = SelectPrimary();
                            u.AddSelect(s);
                            if (lexer.Token() == MySQLToken.KwUnion)
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

                case MySQLToken.PuncLeftParen:
                    {
                        lexer.NextToken();
                        temp = RefsOrQuery();
                        Match(MySQLToken.PuncRightParen);
                        if (temp is Query)
                        {
                            if (temp is Select)
                            {
                                if (lexer.Token() == MySQLToken.KwUnion)
                                {
                                    u = new Union();
                                    u.AddSelect((Select)temp);
                                    while (lexer.Token() == MySQLToken.KwUnion)
                                    {
                                        lexer.NextToken();
                                        temp = SelectPrimary();
                                        u.AddSelect((Select)temp);
                                    }
                                    return u;
                                }
                            }
                            if (lexer.Token() == MySQLToken.KwAs)
                            {
                                lexer.NextToken();
                                string alias = lexer.StringValue();
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
            if (lexer.Token() == MySQLToken.PuncComma)
            {
                rst = new Tup.Cobar4Net.Parser.Recognizer.Mysql.Syntax.Refs();
                rst.AddRef(temp);
                for (; lexer.Token() == MySQLToken.PuncComma;)
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
        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        private Select SelectPrimary()
        {
            Select s = null;
            if (lexer.Token() == MySQLToken.PuncLeftParen)
            {
                lexer.NextToken();
                s = SelectPrimary();
                Match(MySQLToken.PuncRightParen);
                return s;
            }
            Match(MySQLToken.KwSelect);
            return new Select();
        }

        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        public static void Main(string[] args)
        {
            string sql = "   ( ( select union select union select)  as j join    (((select union (select)) as t    )   join t2 ) ,(select)as d), t3)";
            // String sql =
            // "((select) as s1, ((((   select  union select          ) as t2)) join (((t2),t4 as t))) ), t1 aS T1";
            // String sql =
            // "  (( select union select union select)  as j  ,(select)as d), t3";
            System.Console.Out.WriteLine(sql);
            MySQLLexer lexer = new MySQLLexer(sql);
            lexer.NextToken();
            Tup.Cobar4Net.Parser.Recognizer.Mysql.Syntax.SoloParser p = new Tup.Cobar4Net.Parser.Recognizer.Mysql.Syntax.SoloParser
                (lexer);
            Tup.Cobar4Net.Parser.Recognizer.Mysql.Syntax.Refs refs = p.Refs();
            System.Console.Out.WriteLine(refs);
        }
    }

    internal interface Ref
    {
    }

    internal class Factor : Ref
    {
        internal string tableName;

        internal string alias;

        public Factor(string tableName, string alias)
        {
            this.tableName = tableName;
            this.alias = alias;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(tableName);
            sb.Append(" AS ");
            sb.Append(alias);
            return sb.ToString();
        }
    }

    internal class SubQuery : Ref
    {
        internal Query u;

        internal string alias;

        public SubQuery(Query u, string alias)
        {
            this.u = u;
            this.alias = alias;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
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
            StringBuilder sb = new StringBuilder();
            sb.Append("<");
            sb.Append(left.ToString());
            sb.Append(" JOIN ");
            sb.Append(right.ToString());
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
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            for (int i = 0; i < refs.Count; ++i)
            {
                if (i > 0)
                {
                    sb.Append(", ");
                }
                sb.Append(refs[i].ToString());
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
            StringBuilder sb = new StringBuilder();
            foreach (Select s in selects)
            {
                sb.Append(" UNION SELECT");
            }
            string rst = sb.ToString();
            int i = rst.IndexOf("UNION", System.StringComparison.Ordinal);
            if (i >= 0)
            {
                rst = Sharpen.Runtime.Substring(rst, i + "UNION".Length);
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
