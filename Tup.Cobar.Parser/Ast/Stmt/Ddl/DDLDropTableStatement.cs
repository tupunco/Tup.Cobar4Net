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
using Tup.Cobar.Parser.Ast.Expression.Primary;
using Tup.Cobar.Parser.Visitor;

namespace Tup.Cobar.Parser.Ast.Stmt.Ddl
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class DDLDropTableStatement : DDLStatement
    {
        public enum Mode
        {
            Undef,
            Restrict,
            Cascade
        }

        private readonly IList<Identifier> tableNames;

        private readonly bool temp;

        private readonly bool ifExists;

        private readonly DDLDropTableStatement.Mode mode;

        public DDLDropTableStatement(IList<Identifier> tableNames, bool temp, bool ifExists)
            : this(tableNames, temp, ifExists, DDLDropTableStatement.Mode.Undef)
        {
        }

        public DDLDropTableStatement(IList<Identifier> tableNames,
            bool temp,
            bool ifExists,
            DDLDropTableStatement.Mode mode)
        {
            if (tableNames == null || tableNames.IsEmpty())
            {
                this.tableNames = new List<Identifier>(0);
            }
            else
            {
                this.tableNames = tableNames;
            }
            this.temp = temp;
            this.ifExists = ifExists;
            this.mode = mode;
        }

        public virtual IList<Identifier> GetTableNames()
        {
            return tableNames;
        }

        public virtual bool IsTemp()
        {
            return temp;
        }

        public virtual bool IsIfExists()
        {
            return ifExists;
        }

        public virtual DDLDropTableStatement.Mode GetMode()
        {
            return mode;
        }

        public virtual void Accept(SQLASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}