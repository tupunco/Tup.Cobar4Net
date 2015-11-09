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
using Tup.Cobar.Parser.Visitor;

namespace Tup.Cobar.Parser.Ast.Expression.Primary
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class MatchExpression : PrimaryExpression
    {
        public enum Modifier
        {
            Default,
            InBooleanMode,
            InNaturalLanguageMode,
            InNaturalLanguageModeWithQueryExpansion,
            WithQueryExpansion
        }

        private readonly IList<Expression> columns;

        private readonly Expression pattern;

        private readonly MatchExpression.Modifier modifier;

        /// <param name="modifier">never null</param>
        public MatchExpression(IList<Expression> columns, Expression pattern,
            MatchExpression.Modifier modifier)
        {
            if (columns == null || columns.IsEmpty())
            {
                this.columns = new List<Expression>(0);
            }
            else
            {
                if (columns is List<Expression>)
                {
                    this.columns = columns;
                }
                else
                {
                    this.columns = new List<Expression>(columns);
                }
            }
            this.pattern = pattern;
            this.modifier = modifier;
        }

        public virtual IList<Expression> GetColumns()
        {
            return columns;
        }

        public virtual Expression GetPattern()
        {
            return pattern;
        }

        public virtual MatchExpression.Modifier GetModifier()
        {
            return modifier;
        }

        public override void Accept(SQLASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}