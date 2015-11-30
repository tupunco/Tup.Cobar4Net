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
using Tup.Cobar4Net.Parser.Ast.Expression;
using Tup.Cobar4Net.Parser.Ast.Fragment;
using Tup.Cobar4Net.Parser.Visitor;

namespace Tup.Cobar4Net.Parser.Ast.Stmt.Dal
{
    /// <summary>enum name must equals to real sql while ' ' is replaced with '_'</summary>
    public enum ProfileType
    {
        None = 0,
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

    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public class ShowProfile : DalShowStatement
    {
        public ShowProfile(IList<ProfileType> types, IExpression forQuery, Limit limit)
        {
            if (types == null || types.IsEmpty())
            {
                ProfileTypes = new List<ProfileType>(0);
            }
            else if (types is List<ProfileType>)
            {
                ProfileTypes = types;
            }
            else
            {
                ProfileTypes = new List<ProfileType>(types);
            }

            ForQuery = forQuery;
            Limit = limit;
        }

        /// <value>never null</value>
        public virtual IList<ProfileType> ProfileTypes { get; }

        public virtual IExpression ForQuery { get; }

        public virtual Limit Limit { get; }

        public override void Accept(ISqlAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}