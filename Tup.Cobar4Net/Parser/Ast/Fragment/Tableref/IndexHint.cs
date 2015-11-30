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
    /// <summary>
    ///     IndexHint Action
    /// </summary>
    public enum IndexHintAction
    {
        None = 0,

        Use,
        Ignore,
        Force
    }

    /// <summary>
    ///     IndexHint Scope
    /// </summary>
    public enum IndexHintScope
    {
        None = 0,

        All,
        Join,
        GroupBy,
        OrderBy
    }

    /// <summary>
    ///     IndexHint ProfileType
    /// </summary>
    public enum IndexHintType
    {
        None = 0,

        Index,
        Key
    }

    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public class IndexHint : IAstNode
    {
        public IndexHint(IndexHintAction hintAction,
                         IndexHintType hintType,
                         IndexHintScope hintScope,
                         IList<string> indexList)
        {
            if (hintAction == IndexHintAction.None)
            {
                throw new ArgumentException("index hint hintAction is null");
            }
            if (hintType == IndexHintType.None)
            {
                throw new ArgumentException("index hint hintType is null");
            }
            if (hintScope == IndexHintScope.None)
            {
                throw new ArgumentException("index hint hintScope is null");
            }
            HintAction = hintAction;
            IndexType = hintType;
            HintScope = hintScope;
            if (indexList == null || indexList.IsEmpty())
            {
                IndexList = new List<string>(0);
            }
            else if (indexList is List<string>)
            {
                IndexList = indexList;
            }
            else
            {
                IndexList = new List<string>(indexList);
            }
        }

        public virtual IndexHintAction HintAction { get; }

        public virtual IndexHintType IndexType { get; }

        public virtual IndexHintScope HintScope { get; }

        public virtual IList<string> IndexList { get; }

        public virtual void Accept(ISqlAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}