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

using System;
using System.Collections.Generic;
using Tup.Cobar4Net.Parser.Visitor;

namespace Tup.Cobar4Net.Parser.Ast.Expression.Type
{
    /// <summary><code>higherExpr 'COLLATE' collateName</code></summary>
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class CollateExpression : AbstractExpression
    {
        private readonly string collateName;

        private readonly Expression @string;

        public CollateExpression(Expression @string, string collateName)
        {
            if (collateName == null)
            {
                throw new ArgumentException("collateName is null");
            }
            this.@string = @string;
            this.collateName = collateName;
        }

        public virtual string GetCollateName()
        {
            return collateName;
        }

        public virtual Expression GetString()
        {
            return @string;
        }

        public override int GetPrecedence()
        {
            return ExpressionConstants.PrecedenceCollate;
        }

        protected override object EvaluationInternal(IDictionary<object, object> parameters)
        {
            return @string.Evaluation(parameters);
        }

        public override void Accept(SQLASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}