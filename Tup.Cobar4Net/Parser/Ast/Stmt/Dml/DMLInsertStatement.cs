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
using Tup.Cobar4Net.Parser.Ast.Expression.Misc;
using Tup.Cobar4Net.Parser.Ast.Expression.Primary;
using Tup.Cobar4Net.Parser.Util;
using Tup.Cobar4Net.Parser.Visitor;

namespace Tup.Cobar4Net.Parser.Ast.Stmt.Dml
{
    /// <summary>
    ///     DmlInsertStatement Mode
    /// </summary>
    public enum InsertMode
    {
        Undef,
        Low,
        Delay,
        High
    }

    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public class DmlInsertStatement : DmlInsertReplaceStatement
    {
        /// <summary>
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="ignore"></param>
        /// <param name="table"></param>
        /// <param name="columnNameList"></param>
        /// <param name="rowList"></param>
        /// <param name="duplicateUpdate"></param>
        public DmlInsertStatement(InsertMode mode,
                                  bool ignore,
                                  Identifier table,
                                  IList<Identifier> columnNameList,
                                  IList<RowExpression> rowList,
                                  IList<Pair<Identifier, IExpression>> duplicateUpdate)
            : base(table, columnNameList, rowList)
        {
            Mode = mode;
            IsIgnore = ignore;
            DuplicateUpdate = EnsureListType(duplicateUpdate);
        }

        /// <summary>
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="ignore"></param>
        /// <param name="table"></param>
        /// <param name="columnNameList"></param>
        /// <param name="select"></param>
        /// <param name="duplicateUpdate"></param>
        public DmlInsertStatement(InsertMode mode,
                                  bool ignore,
                                  Identifier table,
                                  IList<Identifier> columnNameList,
                                  IQueryExpression select,
                                  IList<Pair<Identifier, IExpression>> duplicateUpdate)
            : base(table, columnNameList, select)
        {
            Mode = mode;
            IsIgnore = ignore;
            DuplicateUpdate = EnsureListType(duplicateUpdate);
        }

        public virtual InsertMode Mode { get; }

        public virtual bool IsIgnore { get; }

        public virtual IList<Pair<Identifier, IExpression>> DuplicateUpdate { get; }

        public override void Accept(ISqlAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}