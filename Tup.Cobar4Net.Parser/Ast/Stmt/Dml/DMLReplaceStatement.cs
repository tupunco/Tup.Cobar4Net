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
using Tup.Cobar4Net.Parser.Ast.Expression.Misc;
using Tup.Cobar4Net.Parser.Ast.Expression.Primary;
using Tup.Cobar4Net.Parser.Visitor;

namespace Tup.Cobar4Net.Parser.Ast.Stmt.Dml
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class DMLReplaceStatement : DMLInsertReplaceStatement
    {
        public enum ReplaceMode
        {
            Undef,
            Low,
            Delay
        }

        private readonly DMLReplaceStatement.ReplaceMode mode;

        public DMLReplaceStatement(DMLReplaceStatement.ReplaceMode mode,
            Identifier table,
            IList<Identifier> columnNameList,
            IList<RowExpression> rowList)
            : base(table, columnNameList, rowList)
        {
            this.mode = mode;
        }

        public DMLReplaceStatement(DMLReplaceStatement.ReplaceMode mode,
            Identifier table,
            IList<Identifier> columnNameList,
            QueryExpression select)
            : base(table, columnNameList, select)
        {
            this.mode = mode;
        }

        public virtual DMLReplaceStatement.ReplaceMode GetMode()
        {
            return mode;
        }

        public override void Accept(SQLASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}