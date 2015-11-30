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

namespace Tup.Cobar4Net.Parser.Ast.Fragment.Ddl
{
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public class IndexDefinition : IAstNode
    {
        public IndexDefinition(IndexType indexType,
                               IList<IndexColumnName> columns,
                               IList<IndexOption> options)
        {
            IndexType = indexType;
            if (columns == null || columns.IsEmpty())
            {
                throw new ArgumentException("columns is null or empty");
            }

            Columns = columns;
            Options = options == null || options.IsEmpty() ? new List<IndexOption>(0) : options;
        }

        public virtual IndexType IndexType { get; }

        /// <value>never null</value>
        public virtual IList<IndexColumnName> Columns { get; }

        /// <value>never null</value>
        public virtual IList<IndexOption> Options { get; }

        public virtual void Accept(ISqlAstVisitor visitor)
        {
        }

        // QS_TODO
    }
}