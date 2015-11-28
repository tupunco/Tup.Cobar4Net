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

using Tup.Cobar4Net.Parser.Ast.Expression.Primary;
using Tup.Cobar4Net.Parser.Visitor;

namespace Tup.Cobar4Net.Parser.Ast.Stmt.Dal
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class ShowTableStatus : DALShowStatement
    {
        private Identifier database;

        private readonly string pattern;

        private readonly Tup.Cobar4Net.Parser.Ast.Expression.Expression where;

        public ShowTableStatus(Identifier database,
            Tup.Cobar4Net.Parser.Ast.Expression.Expression where)
        {
            this.database = database;
            this.pattern = null;
            this.where = where;
        }

        public ShowTableStatus(Identifier database, string pattern)
        {
            this.database = database;
            this.pattern = pattern;
            this.where = null;
        }

        public ShowTableStatus(Identifier database)
        {
            this.database = database;
            this.pattern = null;
            this.where = null;
        }

        public virtual void SetDatabase(Identifier database)
        {
            this.database = database;
        }

        public virtual Identifier GetDatabase()
        {
            return database;
        }

        public virtual string GetPattern()
        {
            return pattern;
        }

        public virtual Tup.Cobar4Net.Parser.Ast.Expression.Expression GetWhere()
        {
            return where;
        }

        public override void Accept(SQLASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}