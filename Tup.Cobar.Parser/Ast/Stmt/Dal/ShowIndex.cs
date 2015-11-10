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
using Tup.Cobar.Parser.Visitor;

namespace Tup.Cobar.Parser.Ast.Stmt.Dal
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class ShowIndex : DALShowStatement
    {
        public enum Type
        {
            Index,
            Indexes,
            Keys
        }

        private readonly ShowIndex.Type type;

        private readonly Identifier table;

        public ShowIndex(ShowIndex.Type type, Identifier table, Identifier database)
        {
            this.table = table;
            this.table.SetParent(database);
            this.type = type;
        }

        public ShowIndex(ShowIndex.Type type, Identifier table)
        {
            this.table = table;
            this.type = type;
        }

        public virtual ShowIndex.Type GetType()
        {
            return type;
        }

        public virtual Identifier GetTable()
        {
            return table;
        }

        public override void Accept(SQLASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}