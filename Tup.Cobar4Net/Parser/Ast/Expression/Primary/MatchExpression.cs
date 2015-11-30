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
using Tup.Cobar4Net.Parser.Visitor;

namespace Tup.Cobar4Net.Parser.Ast.Expression.Primary
{
    /// <summary>
    ///     MatchExpression Modifier
    /// </summary>
    public enum MatchModifier
    {
        Default = 0,

        InBooleanMode,
        InNaturalLanguageMode,
        InNaturalLanguageModeWithQueryExpansion,
        WithQueryExpansion
    }

    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public class MatchExpression : PrimaryExpression
    {
        /// <param name="_matchModifier">never null</param>
        public MatchExpression(IList<IExpression> columns, IExpression pattern,
                               MatchModifier _matchModifier)
        {
            if (columns == null || columns.IsEmpty())
            {
                Columns = new List<IExpression>(0);
            }
            else
            {
                if (columns is List<IExpression>)
                {
                    Columns = columns;
                }
                else
                {
                    Columns = new List<IExpression>(columns);
                }
            }
            Pattern = pattern;
            Modifier = _matchModifier;
        }

        public virtual IList<IExpression> Columns { get; }

        public virtual IExpression Pattern { get; }

        public virtual MatchModifier Modifier { get; }

        public override void Accept(ISqlAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}