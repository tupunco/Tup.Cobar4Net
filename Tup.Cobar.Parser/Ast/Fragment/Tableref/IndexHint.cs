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
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class IndexHint : ASTNode
    {
        public enum IndexAction
        {
            None = 0,
            Use,
            Ignore,
            Force
        }

        public enum IndexType
        {
            None = 0,
            Index,
            Key
        }

        public enum IndexScope
        {
            None = 0,
            All,
            Join,
            GroupBy,
            OrderBy
        }

        private readonly IndexHint.IndexAction action;

        private readonly IndexHint.IndexType type;

        private readonly IndexHint.IndexScope scope;

        private readonly IList<string> indexList;

        public IndexHint(IndexHint.IndexAction action,
            IndexHint.IndexType type,
            IndexHint.IndexScope scope,
            IList<string> indexList)
            : base()
        {
            if (action == IndexAction.None)
            {
                throw new ArgumentException("index hint action is null");
            }
            if (type == IndexType.None)
            {
                throw new ArgumentException("index hint type is null");
            }
            if (scope == IndexScope.None)
            {
                throw new ArgumentException("index hint scope is null");
            }
            this.action = action;
            this.type = type;
            this.scope = scope;
            if (indexList == null || indexList.IsEmpty())
            {
                this.indexList = new List<string>(0);
            }
            else
            {
                if (indexList is List<string>)
                {
                    this.indexList = indexList;
                }
                else
                {
                    this.indexList = new List<string>(indexList);
                }
            }
        }

        public virtual IndexHint.IndexAction GetAction()
        {
            return action;
        }

        public virtual IndexHint.IndexType GetIndexType()
        {
            return type;
        }

        public virtual IndexHint.IndexScope GetScope()
        {
            return scope;
        }

        public virtual IList<string> GetIndexList()
        {
            return indexList;
        }

        public virtual void Accept(SQLASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}