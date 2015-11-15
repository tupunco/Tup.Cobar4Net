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
using Tup.Cobar.Parser.Visitor;

namespace Tup.Cobar.Parser.Ast.Fragment.Tableref
{
    /// <summary>used in <code>FROM</code> fragment</summary>
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class TableReferences : TableReference
    {
        protected static IList<TableReference> EnsureListType(IList<TableReference> list)
        {
            if (list is List<TableReference>)
            {
                return list;
            }
            return new List<TableReference>(list);
        }

        private readonly IList<TableReference> list;

        /// <returns>never null</returns>
        public virtual IList<TableReference> GetTableReferenceList()
        {
            return list;
        }

        /// <exception cref="System.Data.Sql.SQLSyntaxErrorException"/>
        public TableReferences(IList<TableReference> list)
        {
            if (list == null || list.IsEmpty())
            {
                throw new SQLSyntaxErrorException("at least one table reference");
            }
            this.list = EnsureListType(list);
        }

        public override object RemoveLastConditionElement()
        {
            if (list != null && !list.IsEmpty())
            {
                return list[list.Count - 1].RemoveLastConditionElement();
            }
            return null;
        }

        public override bool IsSingleTable()
        {
            if (list == null)
            {
                return false;
            }
            int count = 0;
            TableReference first = null;
            foreach (TableReference @ref in list)
            {
                if (@ref != null && 1 == ++count)
                {
                    first = @ref;
                }
            }
            return count == 1 && first.IsSingleTable();
        }

        public override int GetPrecedence()
        {
            return TableReference.PrecedenceRefs;
        }

        public override void Accept(SQLASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}