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
    /// <summary>
    ///     <code>higherExpr 'COLLATE' _collateName</code>
    /// </summary>
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public class CollateExpression : AbstractExpression
    {
        public CollateExpression(IExpression @string, string collateName)
        {
            if (collateName == null)
            {
                throw new ArgumentException("_collateName is null");
            }
            StringValue = @string;
            CollateName = collateName;
        }

        public virtual string CollateName { get; }

        public virtual IExpression StringValue { get; }

        public override int Precedence
        {
            get { return ExpressionConstants.PrecedenceCollate; }
        }

        protected override object EvaluationInternal(IDictionary<object, object> parameters)
        {
            return StringValue.Evaluation(parameters);
        }

        public override void Accept(ISqlAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}