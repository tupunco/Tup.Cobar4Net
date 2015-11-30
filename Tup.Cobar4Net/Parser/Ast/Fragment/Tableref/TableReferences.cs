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
using Tup.Cobar4Net.Parser.Visitor;

namespace Tup.Cobar4Net.Parser.Ast.Fragment.Tableref
{
    /// <summary>used in <code>FROM</code> fragment</summary>
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public class TableReferences : TableReference
    {
        private readonly IList<TableReference> _list;

        /// <exception cref="System.SqlSyntaxErrorException" />
        public TableReferences(IList<TableReference> list)
        {
            if (list == null || list.IsEmpty())
            {
                throw new SqlSyntaxErrorException("at least one table reference");
            }
            _list = EnsureListType(list);
        }

        /// <value>never null</value>
        public virtual IList<TableReference> TableReferenceList
        {
            get { return _list; }
        }

        public override bool IsSingleTable
        {
            get
            {
                if (_list == null)
                {
                    return false;
                }
                var count = 0;
                TableReference first = null;
                foreach (var @ref in _list)
                {
                    if (@ref != null && 1 == ++count)
                    {
                        first = @ref;
                    }
                }
                return count == 1 && first.IsSingleTable;
            }
        }

        public override int Precedence
        {
            get { return PrecedenceRefs; }
        }

        protected static IList<TableReference> EnsureListType(IList<TableReference> list)
        {
            if (list is List<TableReference>)
            {
                return list;
            }
            return new List<TableReference>(list);
        }

        public override object RemoveLastConditionElement()
        {
            if (_list != null && !_list.IsEmpty())
            {
                return _list[_list.Count - 1].RemoveLastConditionElement();
            }
            return null;
        }

        public override void Accept(ISqlAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}