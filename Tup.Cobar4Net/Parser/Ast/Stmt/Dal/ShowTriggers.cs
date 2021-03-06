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

using Tup.Cobar4Net.Parser.Ast.Expression;
using Tup.Cobar4Net.Parser.Ast.Expression.Primary;
using Tup.Cobar4Net.Parser.Visitor;

namespace Tup.Cobar4Net.Parser.Ast.Stmt.Dal
{
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public class ShowTriggers : DalShowStatement
    {
        public ShowTriggers(Identifier schema, string pattern)
        {
            Schema = schema;
            Pattern = pattern;
            Where = null;
        }

        public ShowTriggers(Identifier schema, IExpression where)
        {
            Schema = schema;
            Pattern = null;
            Where = where;
        }

        public ShowTriggers(Identifier schema)
        {
            Schema = schema;
            Pattern = null;
            Where = null;
        }

        public virtual Identifier Schema { set; get; }

        public virtual string Pattern { get; }

        public virtual IExpression Where { get; }

        public override void Accept(ISqlAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}