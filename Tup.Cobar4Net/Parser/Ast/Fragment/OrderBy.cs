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
using Tup.Cobar4Net.Parser.Ast.Expression;
using Tup.Cobar4Net.Parser.Util;
using Tup.Cobar4Net.Parser.Visitor;

namespace Tup.Cobar4Net.Parser.Ast.Fragment
{
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public class OrderBy : IAstNode
    {
        /// <summary>
        ///     might be
        /// </summary>
        private readonly IList<Pair<IExpression, SortOrder>> orderByList;

        /// <summary>performance tip: linked list is used</summary>
        public OrderBy()
        {
            orderByList = new List<Pair<IExpression, SortOrder>>();
        }

        /// <summary>performance tip: expect to have only 1 order item</summary>
        public OrderBy(IExpression expr, SortOrder order)
        {
            orderByList = new List<Pair<IExpression, SortOrder>>(1)
            {
                new Pair<IExpression, SortOrder>(expr, order)
            };
        }

        public OrderBy(IList<Pair<IExpression, SortOrder>> orderByList)
        {
            if (orderByList == null)
            {
                throw new ArgumentException("order list is null");
            }
            this.orderByList = orderByList;
        }

        public virtual IList<Pair<IExpression, SortOrder>> OrderByList
        {
            get { return orderByList; }
        }

        public virtual void Accept(ISqlAstVisitor visitor)
        {
            visitor.Visit(this);
        }

        public virtual OrderBy AddOrderByItem(IExpression expr, SortOrder order)
        {
            orderByList.Add(new Pair<IExpression, SortOrder>(expr, order));
            return this;
        }
    }
}