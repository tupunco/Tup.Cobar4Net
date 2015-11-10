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
using Sharpen;
using Tup.Cobar.Parser.Ast;
using Tup.Cobar.Parser.Ast.Expression.Primary;
using Tup.Cobar.Parser.Visitor;

namespace Tup.Cobar.Parser.Ast.Fragment.Ddl.Index
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class IndexColumnName : ASTNode
    {
        private readonly Identifier columnName;

        /// <summary>null is possible</summary>
        private readonly Tup.Cobar.Parser.Ast.Expression.Expression length;

        private readonly bool asc;

        public IndexColumnName(Identifier columnName,
            Tup.Cobar.Parser.Ast.Expression.Expression length, 
            bool asc)
        {
            this.columnName = columnName;
            this.length = length;
            this.asc = asc;
        }

        public virtual void Accept(SQLASTVisitor visitor)
        {
            visitor.Visit(this);
        }

        public virtual Identifier GetColumnName()
        {
            return columnName;
        }

        public virtual Tup.Cobar.Parser.Ast.Expression.Expression GetLength()
        {
            return length;
        }

        public virtual bool IsAsc()
        {
            return asc;
        }
    }
}
