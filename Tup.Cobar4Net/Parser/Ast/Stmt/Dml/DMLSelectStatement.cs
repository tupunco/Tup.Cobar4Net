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
using Tup.Cobar4Net.Parser.Ast.Fragment;
using Tup.Cobar4Net.Parser.Ast.Fragment.Tableref;
using Tup.Cobar4Net.Parser.Util;
using Tup.Cobar4Net.Parser.Visitor;

namespace Tup.Cobar4Net.Parser.Ast.Stmt.Dml
{
    /// <summary>
    ///     DmlSelectStatement LockMode
    /// </summary>
    public enum LockMode
    {
        Undef,
        ForUpdate,
        LockInShareMode
    }

    /// <summary>
    ///     DmlSelectStatement SelectQueryCacheStrategy
    /// </summary>
    public enum SelectQueryCacheStrategy
    {
        Undef,
        SqlCache,
        SqlNoCache
    }

    /// <summary>
    ///     DmlSelectStatement SelectDuplicationStrategy
    /// </summary>
    public enum SelectDuplicationStrategy
    {
        All,
        Distinct,
        Distinctrow
    }

    /// <summary>
    ///     DmlSelectStatement SelectSmallOrBigResult
    /// </summary>
    public enum SelectSmallOrBigResult
    {
        Undef,
        SqlSmallResult,
        SqlBigResult
    }

    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public class DmlSelectStatement : DmlQueryStatement
    {
        /// <summary>string: id | `id` | 'id'</summary>
        private readonly IList<Pair<IExpression, string>> selectExprList;

        /// <exception cref="System.SqlSyntaxErrorException" />
        public DmlSelectStatement(SelectOption option,
            IList<Pair<IExpression, string>> selectExprList,
            TableReferences tables,
            IExpression where,
            GroupBy group,
            IExpression having,
            OrderBy order,
            Limit limit)
        {
            if (option == null)
            {
                throw new ArgumentException("argument 'option' is null");
            }
            Option = option;
            if (selectExprList == null || selectExprList.IsEmpty())
            {
                this.selectExprList = new List<Pair<IExpression, string>>(0);
            }
            else
            {
                this.selectExprList = EnsureListType(selectExprList);
            }
            Tables = tables;
            Where = where;
            Group = group;
            Having = having;
            Order = order;
            Limit = limit;
        }

        public virtual SelectOption Option { get; }

        /// <value>never null</value>
        public virtual IList<Pair<IExpression, string>> SelectExprList
        {
            get { return selectExprList; }
        }

        public virtual TableReferences Tables { get; }

        public virtual IExpression Where { get; }

        public virtual GroupBy Group { get; }

        public virtual IExpression Having { get; }

        public virtual OrderBy Order { get; }

        public virtual Limit Limit { get; }

        /// <performance>slow</performance>
        public virtual IList<IExpression> GetSelectExprListWithoutAlias()
        {
            if (selectExprList == null || selectExprList.IsEmpty())
            {
                return new List<IExpression>(0);
            }
            IList<IExpression> list = new List<IExpression>(selectExprList.Count);
            foreach (var p in selectExprList)
            {
                if (p != null && p.Key != null)
                {
                    list.Add(p.Key);
                }
            }
            return list;
        }

        public override void Accept(ISqlAstVisitor visitor)
        {
            visitor.Visit(this);
        }

        public sealed class SelectOption
        {
            public bool highPriority = false;

            public LockMode lockMode = LockMode.Undef;
            public SelectDuplicationStrategy resultDup = SelectDuplicationStrategy.All;

            public SelectSmallOrBigResult resultSize = SelectSmallOrBigResult.Undef;

            public SelectQueryCacheStrategy SelectQueryCache = SelectQueryCacheStrategy.Undef;

            public bool sqlBufferResult = false;

            public bool sqlCalcFoundRows = false;

            public bool straightJoin = false;

            public override string ToString()
            {
                var sb = new StringBuilder();
                sb.Append(GetType().Name).Append('{');
                sb.Append("resultDup").Append('=').Append(resultDup);
                sb.Append(", ").Append("highPriority").Append('=').Append(highPriority);
                sb.Append(", ").Append("straightJoin").Append('=').Append(straightJoin);
                sb.Append(", ").Append("resultSize").Append('=').Append(resultSize);
                sb.Append(", ").Append("sqlBufferResult").Append('=').Append(sqlBufferResult);
                sb.Append(", ").Append("SelectQueryCache").Append('=').Append(SelectQueryCache);
                sb.Append(", ").Append("sqlCalcFoundRows").Append('=').Append(sqlCalcFoundRows);
                sb.Append(", ").Append("lockMode").Append('=').Append(lockMode);
                sb.Append('}');
                return sb.ToString();
            }
        }
    }
}