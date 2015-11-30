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
using Tup.Cobar4Net.Parser.Ast.Expression.Primary;
using Tup.Cobar4Net.Parser.Ast.Expression.Primary.Literal;
using Tup.Cobar4Net.Parser.Visitor;

namespace Tup.Cobar4Net.Parser.Ast.Fragment.Ddl
{
    /// <summary>
    ///     TableOptions InsertMethod
    /// </summary>
    public enum InsertMethod
    {
        No,
        First,
        Last
    }

    /// <summary>
    ///     TableOptions PackKeys
    /// </summary>
    public enum PackKeys
    {
        False,
        True,
        Default
    }

    /// <summary>
    ///     TableOptions RowFormat
    /// </summary>
    public enum RowFormat
    {
        Default,
        Dynamic,
        Fixed,
        Compressed,
        Redundant,
        Compact
    }

    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public class TableOptions : IAstNode
    {
        // table_option:
        // ENGINE [=] engine_name
        // | AUTO_INCREMENT [=] value
        // | AVG_ROW_LENGTH [=] value
        // | [DEFAULT] CHARACTER SET [=] charset_name
        // | CHECKSUM [=] {0 | 1}
        // | [DEFAULT] COLLATE [=] collation_name
        // | COMMENT [=] 'string'
        // | CONNECTION [=] 'connect_string'
        // | DATA DIRECTORY [=] 'absolute path to directory'
        // | DELAY_KEY_WRITE [=] {0 | 1}
        // | INDEX DIRECTORY [=] 'absolute path to directory'
        // | INSERT_METHOD [=] { NO | FIRST | LAST }
        // | KEY_BLOCK_SIZE [=] value
        // | MAX_ROWS [=] value
        // | MIN_ROWS [=] value
        // | PACK_KEYS [=] {0 | 1 | DEFAULT}
        // | PASSWORD [=] 'string'
        // | ROW_FORMAT [=] {DEFAULT|DYNAMIC|FIXED|COMPRESSED|REDUNDANT|COMPACT}
        // | UNION [=] (tbl_name[,tbl_name]...)

        public virtual Identifier Engine { get; set; }

        public virtual IExpression AutoIncrement { get; set; }

        public virtual IExpression AvgRowLength { get; set; }

        public virtual Identifier CharSet { get; set; }

        public virtual Identifier Collation { get; set; }

        public virtual bool CheckSum { get; set; }

        public virtual LiteralString Comment { get; set; }

        public virtual LiteralString Connection { get; set; }

        public virtual LiteralString DataDir { get; set; }

        public virtual LiteralString IndexDir { get; set; }

        public virtual bool DelayKeyWrite { set; get; }

        public virtual InsertMethod InsertMethod { get; set; }

        public virtual IExpression KeyBlockSize { get; set; }

        public virtual IExpression MaxRows { get; set; }

        public virtual IExpression MinRows { get; set; }

        public virtual PackKeys PackKeys { get; set; }

        public virtual LiteralString Password { get; set; }

        public virtual RowFormat RowFormat { get; set; }

        public virtual IList<Identifier> Union { get; set; }

        public virtual void Accept(ISqlAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}