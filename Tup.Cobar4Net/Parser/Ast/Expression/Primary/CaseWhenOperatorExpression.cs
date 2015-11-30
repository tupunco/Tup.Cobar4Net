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
using Tup.Cobar4Net.Parser.Util;
using Tup.Cobar4Net.Parser.Visitor;

namespace Tup.Cobar4Net.Parser.Ast.Expression.Primary
{
    /// <summary>
    ///     <code>'CASE' value? ('WHEN' condition 'THEN' result)+ ('ELSE' result)? 'END' </code>
    /// </summary>
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public class CaseWhenOperatorExpression : PrimaryExpression
    {
        /// <param name="whenList">never null or empry; no pair contains null key or value</param>
        /// <param name="comparee">
        ///     null for format of <code>CASE WHEN ...</code>, otherwise,
        ///     <code>CASE comparee WHEN ...</code>
        /// </param>
        public CaseWhenOperatorExpression(IExpression comparee,
                                          IList<Pair<IExpression, IExpression>> whenList,
                                          IExpression elseResult)
        {
            Comparee = comparee;
            if (whenList is List<Pair<IExpression, IExpression>>)
            {
                WhenList = whenList;
            }
            else
            {
                WhenList = new List<Pair<IExpression, IExpression>>(whenList);
            }
            ElseResult = elseResult;
        }

        public virtual IExpression Comparee { get; }

        /// <value>never null or empty; no pair contains null key or value</value>
        public virtual IList<Pair<IExpression, IExpression>> WhenList { get; }

        public virtual IExpression ElseResult { get; }

        public override void Accept(ISqlAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}