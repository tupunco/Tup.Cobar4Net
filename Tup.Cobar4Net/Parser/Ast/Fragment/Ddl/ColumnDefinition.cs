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
using Tup.Cobar4Net.Parser.Ast.Expression;
using Tup.Cobar4Net.Parser.Ast.Expression.Primary.Literal;
using Tup.Cobar4Net.Parser.Visitor;

namespace Tup.Cobar4Net.Parser.Ast.Fragment.Ddl
{
    /// <summary>
    ///     ColumnDefinition SpecialIndex
    /// </summary>
    public enum SpecialIndex
    {
        None = 0,

        Primary,
        Unique
    }

    /// <summary>
    ///     ColumnDefinition ColumnFormat
    /// </summary>
    public enum ColumnFormat
    {
        None = 0,

        Fixed,
        Dynamic,
        Default
    }

    /// <summary>NOT FULL AST</summary>
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public class ColumnDefinition : IAstNode
    {
        /// <param name="dataType" />
        /// <param name="notNull" />
        /// <param name="defaultVal">might be null</param>
        /// <param name="autoIncrement" />
        /// <param name="specialIndex">might be null</param>
        /// <param name="comment">might be null</param>
        /// <param name="columnFormat">might be null</param>
        public ColumnDefinition(DataType dataType,
                                bool notNull,
                                IExpression defaultVal,
                                bool autoIncrement,
                                SpecialIndex specialIndex,
                                LiteralString comment,
                                ColumnFormat columnFormat)
        {
            if (dataType == null)
            {
                throw new ArgumentException("data type is null");
            }
            DataType = dataType;
            IsNotNull = notNull;
            DefaultVal = defaultVal;
            IsAutoIncrement = autoIncrement;
            SpecialIndex = specialIndex;
            Comment = comment;
            ColumnFormat = columnFormat;
        }

        public virtual DataType DataType { get; }

        public virtual bool IsNotNull { get; }

        public virtual IExpression DefaultVal { get; }

        public virtual bool IsAutoIncrement { get; }

        public virtual SpecialIndex SpecialIndex { get; }

        public virtual LiteralString Comment { get; }

        public virtual ColumnFormat ColumnFormat { get; }

        public virtual void Accept(ISqlAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}