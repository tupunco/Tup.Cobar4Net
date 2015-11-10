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
using Tup.Cobar.Parser.Util;
using Tup.Cobar.Parser.Visitor;

namespace Tup.Cobar.Parser.Ast.Stmt.Ddl
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class DDLRenameTableStatement : DDLStatement
    {
        private readonly IList<Pair<Identifier, Identifier>> list;

        public DDLRenameTableStatement()
        {
            this.list = new List<Pair<Identifier, Identifier>>();
        }

        public DDLRenameTableStatement(IList<Pair<Identifier, Identifier>> list)
        {
            if (list == null)
            {
                this.list = new List<Pair<Identifier, Identifier>>(0);
            }
            else
            {
                this.list = list;
            }
        }

        public virtual DDLRenameTableStatement AddRenamePair(Identifier from, Identifier to)
        {
            list.Add(new Pair<Identifier, Identifier>(from, to));
            return this;
        }

        public virtual IList<Pair<Identifier, Identifier>> GetList()
        {
            return list;
        }

        public virtual void Accept(SQLASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}