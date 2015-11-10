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
using Tup.Cobar.Parser.Ast.Expression.Misc;
using Tup.Cobar.Parser.Ast.Expression.Primary;
using Tup.Cobar.Parser.Util;
using Tup.Cobar.Parser.Visitor;
using Expr = Tup.Cobar.Parser.Ast.Expression.Expression;

namespace Tup.Cobar.Parser.Ast.Stmt.Dml
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class DMLInsertStatement : DMLInsertReplaceStatement
    {
        public enum InsertMode
        {
            Undef,
            Low,
            Delay,
            High
        }

        private readonly DMLInsertStatement.InsertMode mode;

        private readonly bool ignore;

        private readonly IList<Pair<Identifier, Expr
            >> duplicateUpdate;

        /// <summary>(insert ...</summary>
        /// <remarks>(insert ... values | insert ... set) format</remarks>
        /// <param name="columnNameList">can be null</param>
        public DMLInsertStatement(DMLInsertStatement.InsertMode mode,
            bool ignore,
            Identifier table,
            IList<Identifier> columnNameList,
            IList<RowExpression> rowList,
            IList<Pair<Identifier, Expr>> duplicateUpdate)
            : base(table, columnNameList, rowList)
        {
            this.mode = mode;
            this.ignore = ignore;
            this.duplicateUpdate = EnsureListType(duplicateUpdate);
        }

        /// <summary>insert ...</summary>
        /// <remarks>insert ... select format</remarks>
        /// <param name="columnNameList">can be null</param>
        public DMLInsertStatement(DMLInsertStatement.InsertMode mode,
            bool ignore,
            Identifier table, IList<Identifier> columnNameList,
            QueryExpression select,
            IList<Pair<Identifier, Expr>> duplicateUpdate)
            : base(table, columnNameList, select)
        {
            this.mode = mode;
            this.ignore = ignore;
            this.duplicateUpdate = EnsureListType(duplicateUpdate);
        }

        public virtual DMLInsertStatement.InsertMode GetMode()
        {
            return mode;
        }

        public virtual bool IsIgnore()
        {
            return ignore;
        }

        public virtual IList<Pair<Identifier, Expr>> GetDuplicateUpdate()
        {
            return duplicateUpdate;
        }

        public override void Accept(SQLASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}