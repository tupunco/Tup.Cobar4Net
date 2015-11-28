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
using Tup.Cobar4Net.Parser.Ast.Stmt.Ddl;
using Tup.Cobar4Net.Parser.Util;
using Tup.Cobar4Net.Parser.Visitor;
using Expr = Tup.Cobar4Net.Parser.Ast.Expression.Expression;

namespace Tup.Cobar4Net.Parser.Ast.Stmt.Extension
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class ExtDDLCreatePolicy : DDLStatement
    {
        private readonly Identifier name;

        private readonly IList<Pair<int, Expr>> proportion;

        public ExtDDLCreatePolicy(Identifier name)
        {
            this.name = name;
            this.proportion = new List<Pair<int, Expr>>(1);
        }

        public virtual Identifier GetName()
        {
            return name;
        }

        public virtual IList<Pair<int, Expr>> GetProportion()
        {
            return proportion;
        }

        public virtual ExtDDLCreatePolicy AddProportion(int id, Expr val)
        {
            proportion.Add(new Pair<int, Expr>(id, val));
            return this;
        }

        public virtual void Accept(SQLASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}