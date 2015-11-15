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

using Tup.Cobar.Parser.Ast.Expression.Primary;
using Tup.Cobar.Parser.Ast.Expression.Primary.Literal;
using Tup.Cobar.Parser.Visitor;

namespace Tup.Cobar.Parser.Ast.Fragment.Ddl.Index
{
    public enum IndexType
    {
        None = 0,
        Btree,
        Hash
    }

    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class IndexOption : ASTNode
    {
        private readonly Tup.Cobar.Parser.Ast.Expression.Expression keyBlockSize;

        private readonly IndexType indexType;

        private readonly Identifier parserName;

        private readonly LiteralString comment;

        public IndexOption(Tup.Cobar.Parser.Ast.Expression.Expression keyBlockSize)
        {
            this.keyBlockSize = keyBlockSize;
            this.indexType = IndexType.Btree;
            this.parserName = null;
            this.comment = null;
        }

        public IndexOption(IndexType indexType)
        {
            this.keyBlockSize = null;
            this.indexType = indexType;
            this.parserName = null;
            this.comment = null;
        }

        public IndexOption(Identifier parserName)
        {
            this.keyBlockSize = null;
            this.indexType = IndexType.Btree;
            this.parserName = parserName;
            this.comment = null;
        }

        public IndexOption(LiteralString comment)
        {
            this.keyBlockSize = null;
            this.indexType = IndexType.Btree;
            this.parserName = null;
            this.comment = comment;
        }

        public virtual Tup.Cobar.Parser.Ast.Expression.Expression GetKeyBlockSize()
        {
            return keyBlockSize;
        }

        public virtual IndexType GetIndexType()
        {
            return indexType;
        }

        public virtual Identifier GetParserName()
        {
            return parserName;
        }

        public virtual LiteralString GetComment()
        {
            return comment;
        }

        public virtual void Accept(SQLASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}