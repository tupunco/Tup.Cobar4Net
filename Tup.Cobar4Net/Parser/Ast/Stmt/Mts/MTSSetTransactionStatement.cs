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
using Tup.Cobar4Net.Parser.Ast.Fragment;
using Tup.Cobar4Net.Parser.Visitor;

namespace Tup.Cobar4Net.Parser.Ast.Stmt.Mts
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class MTSSetTransactionStatement : SQLStatement
    {
        public enum IsolationLevel
        {
            None = 0,
            ReadUncommitted,
            ReadCommitted,
            RepeatableRead,
            Serializable
        }

        private readonly VariableScope scope;

        private readonly IsolationLevel level;

        public MTSSetTransactionStatement(VariableScope scope, IsolationLevel level)
        {
            if (level == IsolationLevel.None)
            {
                throw new ArgumentException("isolation level is null");
            }

            this.level = level;
            this.scope = scope;
        }

        /// <retern>null means scope undefined</retern>
        public virtual VariableScope GetScope()
        {
            return scope;
        }

        public virtual IsolationLevel GetLevel()
        {
            return level;
        }

        public virtual void Accept(SQLASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}