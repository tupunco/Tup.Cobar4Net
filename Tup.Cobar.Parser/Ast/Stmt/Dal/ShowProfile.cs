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
using Tup.Cobar.Parser.Ast.Fragment;
using Tup.Cobar.Parser.Visitor;
using Expr = Tup.Cobar.Parser.Ast.Expression.Expression;

namespace Tup.Cobar.Parser.Ast.Stmt.Dal
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class ShowProfile : DALShowStatement
    {
        /// <summary>enum name must equals to real sql while ' ' is replaced with '_'</summary>
        public enum Type
        {
            All,
            BlockIo,
            ContextSwitches,
            Cpu,
            Ipc,
            Memory,
            PageFaults,
            Source,
            Swaps
        }

        private readonly IList<ShowProfile.Type> types;

        private readonly Expr forQuery;

        private readonly Limit limit;

        public ShowProfile(IList<ShowProfile.Type> types, Expr
             forQuery, Limit limit)
        {
            if (types == null || types.IsEmpty())
            {
                this.types = new List<ShowProfile.Type>(0);
            }
            else
            {
                if (types is List<ShowProfile.Type>)
                {
                    this.types = types;
                }
                else
                {
                    this.types = new List<ShowProfile.Type>(types);
                }
            }
            this.forQuery = forQuery;
            this.limit = limit;
        }

        /// <returns>never null</returns>
        public virtual IList<ShowProfile.Type> GetTypes()
        {
            return types;
        }

        public virtual Expr GetForQuery()
        {
            return forQuery;
        }

        public virtual Limit GetLimit()
        {
            return limit;
        }

        public override void Accept(SQLASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}