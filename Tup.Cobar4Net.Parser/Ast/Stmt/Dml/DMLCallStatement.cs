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
using Expr = Tup.Cobar4Net.Parser.Ast.Expression.Expression;

namespace Tup.Cobar4Net.Parser.Ast.Stmt.Dml
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class DMLCallStatement : DMLStatement
    {
        private readonly Identifier procedure;

        private readonly IList<Expr> arguments;

        public DMLCallStatement(Identifier procedure, IList<Expr> arguments)
        {
            this.procedure = procedure;
            if (arguments == null || arguments.IsEmpty())
            {
                this.arguments = new List<Expr>(0);
            }
            else
            {
                if (arguments is List<Expr>)
                {
                    this.arguments = arguments;
                }
                else
                {
                    this.arguments = new List<Expr>(arguments);
                }
            }
        }

        public DMLCallStatement(Identifier procedure)
        {
            this.procedure = procedure;
            this.arguments = new List<Expr>(0);
        }

        public virtual Identifier GetProcedure()
        {
            return procedure;
        }

        /// <returns>never null</returns>
        public virtual IList<Expr> GetArguments()
        {
            return arguments;
        }

        public override void Accept(SQLASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}