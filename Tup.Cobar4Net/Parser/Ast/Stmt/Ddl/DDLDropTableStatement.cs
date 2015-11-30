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
using Tup.Cobar4Net.Parser.Ast.Expression.Primary;
using Tup.Cobar4Net.Parser.Visitor;

namespace Tup.Cobar4Net.Parser.Ast.Stmt.Ddl
{
    /// <summary>
    ///     DdlDropTableStatement Mode
    /// </summary>
    public enum DropTableMode
    {
        Undef,
        Restrict,
        Cascade
    }

    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public class DdlDropTableStatement : IDdlStatement
    {
        public DdlDropTableStatement(IList<Identifier> tableNames, bool temp, bool ifExists)
            : this(tableNames, temp, ifExists, DropTableMode.Undef)
        {
        }

        public DdlDropTableStatement(IList<Identifier> tableNames,
            bool temp,
            bool ifExists,
            DropTableMode dropTableMode)
        {
            if (tableNames == null || tableNames.IsEmpty())
            {
                TableNames = new List<Identifier>(0);
            }
            else
            {
                TableNames = tableNames;
            }
            IsTemp = temp;
            IsIfExists = ifExists;
            Mode = dropTableMode;
        }

        public virtual IList<Identifier> TableNames { get; }

        public virtual bool IsTemp { get; }

        public virtual bool IsIfExists { get; }

        public virtual DropTableMode Mode { get; }

        public virtual void Accept(ISqlAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}