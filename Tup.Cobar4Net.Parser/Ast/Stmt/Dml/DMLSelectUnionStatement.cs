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

using System.Collections.Generic;
using Tup.Cobar4Net.Parser.Ast.Fragment;
using Tup.Cobar4Net.Parser.Visitor;

namespace Tup.Cobar4Net.Parser.Ast.Stmt.Dml
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class DMLSelectUnionStatement : DMLQueryStatement
    {
        /// <summary>
        /// might be
        /// <see cref="System.Collections.ArrayList{E}"/>
        ///
        /// </summary>
        private readonly IList<DMLSelectStatement> selectStmtList;

        /// <summary>
        /// <code>Mixed UNION types are treated such that a DISTINCT union overrides any ALL union to its left</code>
        /// <br/>
        /// 0 means all relations of selects are union all<br/>
        /// last index of
        /// <see cref="selectStmtList"/>
        /// means all relations of selects are
        /// union distinct<br/>
        /// </summary>
        private int firstDistinctIndex = 0;

        private OrderBy orderBy;

        private Limit limit;

        public DMLSelectUnionStatement(DMLSelectStatement select)
            : base()
        {
            this.selectStmtList = new List<DMLSelectStatement>();
            this.selectStmtList.Add(select);
        }

        public virtual DMLSelectUnionStatement AddSelect(DMLSelectStatement select, bool unionAll)
        {
            selectStmtList.Add(select);
            if (!unionAll)
            {
                firstDistinctIndex = selectStmtList.Count - 1;
            }
            return this;
        }

        public virtual DMLSelectUnionStatement SetOrderBy(OrderBy orderBy)
        {
            this.orderBy = orderBy;
            return this;
        }

        public virtual DMLSelectUnionStatement SetLimit(Limit limit)
        {
            this.limit = limit;
            return this;
        }

        public virtual IList<DMLSelectStatement> GetSelectStmtList()
        {
            return selectStmtList;
        }

        public virtual int GetFirstDistinctIndex()
        {
            return firstDistinctIndex;
        }

        public virtual OrderBy GetOrderBy()
        {
            return orderBy;
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