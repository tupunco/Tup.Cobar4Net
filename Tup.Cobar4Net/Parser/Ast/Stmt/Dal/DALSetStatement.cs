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
using Tup.Cobar4Net.Parser.Ast.Expression.Primary;
using Tup.Cobar4Net.Parser.Util;
using Tup.Cobar4Net.Parser.Visitor;

namespace Tup.Cobar4Net.Parser.Ast.Stmt.Dal
{
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public class DalSetStatement : ISqlStatement
    {
        public DalSetStatement(IList<Pair<VariableExpression, IExpression>> assignmentList)
        {
            if (assignmentList == null || assignmentList.IsEmpty())
            {
                AssignmentList = new List<Pair<VariableExpression, IExpression>>(0);
            }
            else if (assignmentList is List<Pair<VariableExpression, IExpression>>)
            {
                AssignmentList = assignmentList;
            }
            else
            {
                AssignmentList = new List<Pair<VariableExpression, IExpression>>(assignmentList);
            }
        }

        /// <value>never null</value>
        public virtual IList<Pair<VariableExpression, IExpression>> AssignmentList { get; }

        public virtual void Accept(ISqlAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}