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
using Tup.Cobar4Net.Parser.Ast.Expression.Misc;
using Tup.Cobar4Net.Parser.Ast.Expression.Primary;

namespace Tup.Cobar4Net.Parser.Ast.Stmt.Dml
{
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public abstract class DmlInsertReplaceStatement : DmlStatement
    {
        protected readonly IList<Identifier> columnNameList;

        protected readonly IQueryExpression select;
        protected readonly Identifier table;

        protected IList<RowExpression> rowList;

        private IList<RowExpression> rowListBak;

        protected DmlInsertReplaceStatement(Identifier table,
            IList<Identifier> columnNameList,
            IList<RowExpression> rowList)
        {
            this.table = table;
            this.columnNameList = EnsureListType(columnNameList);
            this.rowList = EnsureListType(rowList);
            select = null;
        }

        protected DmlInsertReplaceStatement(Identifier table,
            IList<Identifier> columnNameList,
            IQueryExpression select)
        {
            if (select == null)
            {
                throw new ArgumentException("argument 'select' is empty");
            }
            this.select = select;
            this.table = table;
            this.columnNameList = EnsureListType(columnNameList);
            rowList = null;
        }

        public virtual Identifier Table
        {
            get { return table; }
        }

        /// <value>
        ///     <see cref="System.Collections.Generic.List{E}">ArrayList{E}</see>
        /// </value>
        public virtual IList<Identifier> ColumnNameList
        {
            get { return columnNameList; }
        }

        /// <value>
        ///     <see cref="System.Collections.Generic.List{E}">ArrayList{E}</see>
        /// </value>
        public virtual IList<RowExpression> RowList
        {
            get { return rowList; }
        }

        public virtual IQueryExpression Select
        {
            get { return @select; }
        }

        public virtual IList<RowExpression> ReplaceRowList
        {
            set
            {
                rowListBak = rowList;
                rowList = value;
            }
        }

        public virtual void ClearReplaceRowList()
        {
            if (rowListBak != null)
            {
                rowList = rowListBak;
                rowListBak = null;
            }
        }
    }
}