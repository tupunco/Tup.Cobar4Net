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
using Tup.Cobar.Parser.Ast.Fragment;
using Tup.Cobar.Parser.Ast.Fragment.Tableref;
using Tup.Cobar.Parser.Util;
using Tup.Cobar.Parser.Visitor;
using Expr = Tup.Cobar.Parser.Ast.Expression.Expression;

namespace Tup.Cobar.Parser.Ast.Stmt.Dml
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class DMLSelectStatement : DMLQueryStatement
    {
        public enum SelectDuplicationStrategy
        {
            All,
            Distinct,
            Distinctrow
        }

        public enum QueryCacheStrategy
        {
            Undef,
            SqlCache,
            SqlNoCache
        }

        public enum SmallOrBigResult
        {
            Undef,
            SqlSmallResult,
            SqlBigResult
        }

        public enum LockMode
        {
            Undef,
            ForUpdate,
            LockInShareMode
        }

        public sealed class SelectOption
        {
            public DMLSelectStatement.SelectDuplicationStrategy resultDup = DMLSelectStatement.SelectDuplicationStrategy
                .All;

            public bool highPriority = false;

            public bool straightJoin = false;

            public DMLSelectStatement.SmallOrBigResult resultSize = DMLSelectStatement.SmallOrBigResult
                .Undef;

            public bool sqlBufferResult = false;

            public DMLSelectStatement.QueryCacheStrategy queryCache = DMLSelectStatement.QueryCacheStrategy
                .Undef;

            public bool sqlCalcFoundRows = false;

            public DMLSelectStatement.LockMode lockMode = DMLSelectStatement.LockMode.Undef;

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(GetType().Name).Append('{');
                sb.Append("resultDup").Append('=').Append(resultDup.ToString());
                sb.Append(", ").Append("highPriority").Append('=').Append(highPriority);
                sb.Append(", ").Append("straightJoin").Append('=').Append(straightJoin);
                sb.Append(", ").Append("resultSize").Append('=').Append(resultSize.ToString());
                sb.Append(", ").Append("sqlBufferResult").Append('=').Append(sqlBufferResult);
                sb.Append(", ").Append("queryCache").Append('=').Append(queryCache.ToString());
                sb.Append(", ").Append("sqlCalcFoundRows").Append('=').Append(sqlCalcFoundRows);
                sb.Append(", ").Append("lockMode").Append('=').Append(lockMode.ToString());
                sb.Append('}');
                return sb.ToString();
            }
        }

        private readonly DMLSelectStatement.SelectOption option;

        /// <summary>string: id | `id` | 'id'</summary>
        private readonly IList<Pair<Expr, string>> selectExprList;

        private readonly TableReferences tables;

        private readonly Expr where;

        private readonly GroupBy group;

        private readonly Expr having;

        private readonly OrderBy order;

        private readonly Limit limit;

        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        public DMLSelectStatement(DMLSelectStatement.SelectOption option, IList<Pair<Expr
            , string>> selectExprList, TableReferences tables, Expr
             where, GroupBy group, Expr having, OrderBy
             order, Limit limit)
        {
            if (option == null)
            {
                throw new ArgumentException("argument 'option' is null");
            }
            this.option = option;
            if (selectExprList == null || selectExprList.IsEmpty())
            {
                this.selectExprList = new List<Pair<Expr, string>>(0);
            }
            else
            {
                this.selectExprList = EnsureListType(selectExprList);
            }
            this.tables = tables;
            this.where = where;
            this.group = group;
            this.having = having;
            this.order = order;
            this.limit = limit;
        }

        public virtual DMLSelectStatement.SelectOption GetOption()
        {
            return option;
        }

        /// <returns>never null</returns>
        public virtual IList<Pair<Expr, string>> GetSelectExprList()
        {
            return selectExprList;
        }

        /// <performance>slow</performance>
        public virtual IList<Expr> GetSelectExprListWithoutAlias()
        {
            if (selectExprList == null || selectExprList.IsEmpty())
            {
                return new List<Expr>(0);
            }
            IList<Expr> list = new List<Expr>(selectExprList.Count);
            foreach (Pair<Expr, string> p in selectExprList)
            {
                if (p != null && p.GetKey() != null)
                {
                    list.Add(p.GetKey());
                }
            }
            return list;
        }

        public virtual TableReferences GetTables()
        {
            return tables;
        }

        public virtual Expr GetWhere()
        {
            return where;
        }

        public virtual GroupBy GetGroup()
        {
            return group;
        }

        public virtual Expr GetHaving()
        {
            return having;
        }

        public virtual OrderBy GetOrder()
        {
            return order;
        }

        public virtual Limit GetLimit()
        {
            return limit;
        }

        public override void Accept(SQLASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}