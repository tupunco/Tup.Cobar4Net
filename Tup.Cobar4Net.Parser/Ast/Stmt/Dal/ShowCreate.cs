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
    public class ShowCreate : DALShowStatement
    {
        /// <summary>enum name must equals to real sql string</summary>
        public enum CreateType
        {
            Database,
            Event,
            Function,
            Procedure,
            Table,
            Trigger,
            View
        }

        private readonly CreateType type;

        private readonly Identifier id;

        public ShowCreate(CreateType type, Identifier id)
        {
            this.type = type;
            this.id = id;
        }

        public virtual CreateType GetCreateType()
        {
            return type;
        }

        public virtual Identifier GetId()
        {
            return id;
        }

        public override void Accept(SQLASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}