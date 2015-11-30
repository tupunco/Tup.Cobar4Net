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
using Tup.Cobar4Net.Parser.Ast.Expression.Primary;
using Tup.Cobar4Net.Parser.Visitor;

namespace Tup.Cobar4Net.Parser.Ast.Stmt.Mts
{
    /// <summary>
    ///     MTSRollbackStatement CompleteType
    /// </summary>
    public enum CompleteType
    {
        None = 0,
        UnDef,
        Chain,
        NoChain,
        Release,
        NoRelease
    }

    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public class MTSRollbackStatement : ISqlStatement
    {
        public MTSRollbackStatement(CompleteType completeType)
        {
            if (completeType == CompleteType.None)
            {
                throw new ArgumentException("complete type is null!");
            }
            CompleteType = completeType;
            Savepoint = null;
        }

        public MTSRollbackStatement(Identifier savepoint)
        {
            CompleteType = CompleteType.None;

            if (savepoint == null)
            {
                throw new ArgumentException("savepoint is null!");
            }
            Savepoint = savepoint;
        }

        /// <value>null if roll back to SAVEPOINT</value>
        public virtual CompleteType CompleteType { get; }

        /// <value>null for roll back the whole transaction</value>
        public virtual Identifier Savepoint { get; }

        public virtual void Accept(ISqlAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}