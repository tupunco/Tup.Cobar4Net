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
using Tup.Cobar.Parser.Ast.Expression.Misc;
using Tup.Cobar.Parser.Ast.Expression.Primary;

namespace Tup.Cobar.Parser.Ast.Stmt.Dml
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public abstract class DMLInsertReplaceStatement : DMLStatement
    {
        protected readonly Identifier table;

        protected readonly IList<Identifier> columnNameList;

        protected IList<RowExpression> rowList;

        protected readonly QueryExpression select;

        public DMLInsertReplaceStatement(Identifier table,
            IList<Identifier> columnNameList, IList<RowExpression> rowList)
        {
            this.table = table;
            this.columnNameList = EnsureListType(columnNameList);
            this.rowList = EnsureListType(rowList);
            this.select = null;
        }

        public DMLInsertReplaceStatement(Identifier table, IList<Identifier> columnNameList
            , QueryExpression select)
        {
            if (select == null)
            {
                throw new ArgumentException("argument 'select' is empty");
            }
            this.select = select;
            this.table = table;
            this.columnNameList = EnsureListType(columnNameList);
            this.rowList = null;
        }

        public virtual Identifier GetTable()
        {
            return table;
        }

        /// <returns>
        ///
        /// <see cref="System.Collections.ArrayList{E}">ArrayList{E}</see>
        /// </returns>
        public virtual IList<Identifier> GetColumnNameList()
        {
            return columnNameList;
        }

        /// <returns>
        ///
        /// <see cref="System.Collections.ArrayList{E}">ArrayList{E}</see>
        /// or
        /// <see cref="Sharpen.Collections.EmptyList{T}()">EMPTY_LIST</see>
        /// </returns>
        public virtual IList<RowExpression> GetRowList()
        {
            return rowList;
        }

        public virtual QueryExpression GetSelect()
        {
            return select;
        }

        private IList<RowExpression> rowListBak;

        public virtual void SetReplaceRowList(IList<RowExpression> list)
        {
            rowListBak = rowList;
            rowList = list;
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