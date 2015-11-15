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

namespace Tup.Cobar.Parser.Ast.Fragment.Ddl.Index
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class IndexDefinition : ASTNode
    {
        private readonly IndexType indexType;

        private readonly IList<IndexColumnName> columns;

        private readonly IList<IndexOption> options;

        public IndexDefinition(IndexType indexType,
            IList<IndexColumnName> columns,
            IList<IndexOption> options)
        {
            this.indexType = indexType;
            if (columns == null || columns.IsEmpty())
            {
                throw new ArgumentException("columns is null or empty");
            }

            this.columns = columns;
            this.options = (IList<IndexOption>)(options == null || options.IsEmpty() ? new List<IndexOption>(0) : options);
        }

        public virtual IndexType GetIndexType()
        {
            return indexType;
        }

        /// <returns>never null</returns>
        public virtual IList<IndexColumnName> GetColumns()
        {
            return columns;
        }

        /// <returns>never null</returns>
        public virtual IList<IndexOption> GetOptions()
        {
            return options;
        }

        public virtual void Accept(SQLASTVisitor visitor)
        {
        }

        // QS_TODO
    }
}