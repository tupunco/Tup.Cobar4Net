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

using Tup.Cobar4Net.Parser.Ast.Expression;
using Tup.Cobar4Net.Parser.Ast.Expression.Primary;
using Tup.Cobar4Net.Parser.Ast.Expression.Primary.Literal;
using Tup.Cobar4Net.Parser.Visitor;

namespace Tup.Cobar4Net.Parser.Ast.Fragment.Ddl
{
    /// <summary>
    ///     IndexOption IndexHintType
    /// </summary>
    public enum IndexType
    {
        None = 0,

        Btree,
        Hash
    }

    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public class IndexOption : IAstNode
    {
        public IndexOption(IExpression keyBlockSize)
        {
            KeyBlockSize = keyBlockSize;
            IndexType = IndexType.Btree;
            ParserName = null;
            Comment = null;
        }

        public IndexOption(IndexType indexType)
        {
            KeyBlockSize = null;
            IndexType = indexType;
            ParserName = null;
            Comment = null;
        }

        public IndexOption(Identifier parserName)
        {
            KeyBlockSize = null;
            IndexType = IndexType.Btree;
            ParserName = parserName;
            Comment = null;
        }

        public IndexOption(LiteralString comment)
        {
            KeyBlockSize = null;
            IndexType = IndexType.Btree;
            ParserName = null;
            Comment = comment;
        }

        public virtual IExpression KeyBlockSize { get; }

        public virtual IndexType IndexType { get; }

        public virtual Identifier ParserName { get; }

        public virtual LiteralString Comment { get; }

        public virtual void Accept(ISqlAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}