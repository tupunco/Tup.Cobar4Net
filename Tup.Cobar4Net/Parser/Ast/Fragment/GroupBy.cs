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
using Tup.Cobar4Net.Parser.Ast.Expression;
using Tup.Cobar4Net.Parser.Util;
using Tup.Cobar4Net.Parser.Visitor;

namespace Tup.Cobar4Net.Parser.Ast.Fragment
{
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public class GroupBy : IAstNode
    {
        /// <summary>
        ///     might be
        ///     <see cref="System.Collections.Generic.IList{E}" />
        /// </summary>
        private readonly IList<Pair<IExpression, SortOrder>> _orderByList;

        private bool _withRollup;

        /// <summary>performance tip: expect to have only 1 order item</summary>
        public GroupBy(IExpression expr, SortOrder order, bool withRollup)
        {
            _orderByList = new List<Pair<IExpression, SortOrder>>(1)
                           {
                               new Pair<IExpression, SortOrder>(expr, order)
                           };
            _withRollup = withRollup;
        }

        /// <summary>performance tip: linked list is used</summary>
        public GroupBy()
        {
            _orderByList = new List<Pair<IExpression, SortOrder>>();
        }

        public virtual bool IsWithRollup
        {
            get { return _withRollup; }
        }

        /// <value>never null</value>
        public virtual IList<Pair<IExpression, SortOrder>> OrderByList
        {
            get { return _orderByList; }
        }

        public virtual void Accept(ISqlAstVisitor visitor)
        {
            visitor.Visit(this);
        }

        public virtual GroupBy SetWithRollup()
        {
            _withRollup = true;
            return this;
        }

        public virtual GroupBy AddOrderByItem(IExpression expr, SortOrder order)
        {
            _orderByList.Add(new Pair<IExpression, SortOrder>(expr, order));
            return this;
        }
    }
}