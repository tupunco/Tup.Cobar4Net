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

using Tup.Cobar4Net.Parser.Visitor;

namespace Tup.Cobar4Net.Parser.Ast.Fragment.Tableref
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public abstract class TableReference : ASTNode
    {
        public const int PrecedenceRefs = 0;

        public const int PrecedenceJoin = 1;

        public const int PrecedenceFactor = 2;

        /// <summary>remove last condition element is success</summary>
        /// <returns>
        ///
        /// <see cref="System.Collections.IList{E}">List&lt;String&gt;</see>
        /// or
        /// <see cref="Tup.Cobar4Net.Parser.Ast.Expression.Expression">Expression</see>
        /// . null if last condition element cannot be removed.
        /// </returns>
        public abstract object RemoveLastConditionElement();

        /// <returns>
        /// true if and only if there is one table (not subquery) in table
        /// reference
        /// </returns>
        public abstract bool IsSingleTable();

        /// <returns>
        /// precedences are defined in
        /// <see cref="TableReference"/>
        /// </returns>
        public abstract int GetPrecedence();

        public abstract void Accept(SQLASTVisitor visitor);
    }
}