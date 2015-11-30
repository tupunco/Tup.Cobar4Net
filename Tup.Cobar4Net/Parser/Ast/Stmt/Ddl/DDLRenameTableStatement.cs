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
using Tup.Cobar4Net.Parser.Util;
using Tup.Cobar4Net.Parser.Visitor;

namespace Tup.Cobar4Net.Parser.Ast.Stmt.Ddl
{
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public class DdlRenameTableStatement : IDdlStatement
    {
        private readonly IList<Pair<Identifier, Identifier>> _list;

        public DdlRenameTableStatement()
        {
            _list = new List<Pair<Identifier, Identifier>>();
        }

        public DdlRenameTableStatement(IList<Pair<Identifier, Identifier>> list)
        {
            _list = list ?? new List<Pair<Identifier, Identifier>>(0);
        }

        public virtual IList<Pair<Identifier, Identifier>> PairList
        {
            get { return _list; }
        }

        public virtual void Accept(ISqlAstVisitor visitor)
        {
            visitor.Visit(this);
        }

        public virtual DdlRenameTableStatement AddRenamePair(Identifier from, Identifier to)
        {
            _list.Add(new Pair<Identifier, Identifier>(from, to));
            return this;
        }
    }
}