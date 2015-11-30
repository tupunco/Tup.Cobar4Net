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

namespace Tup.Cobar4Net.Parser.Ast.Expression.Misc
{
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public class InExpressionList : AbstractExpression
    {
        private IList<IExpression> _exprList;

        private IList<IExpression> _replaceList;

        public InExpressionList(IList<IExpression> list)
        {
            if (list == null || list.Count == 0)
            {
                _exprList = new List<IExpression>(0);
            }
            else
            {
                if (list is List<IExpression>)
                {
                    _exprList = list;
                }
                else
                {
                    _exprList = new List<IExpression>(list);
                }
            }
        }

        /// <value>never null</value>
        public virtual IList<IExpression> ExprList
        {
            get { return _exprList; }
        }

        public override int Precedence
        {
            get { return ExpressionConstants.PrecedencePrimary; }
        }

        public virtual IList<IExpression> ReplaceExpr
        {
            set { _replaceList = value; }
        }

        protected override object EvaluationInternal(IDictionary<object, object> parameters)
        {
            return ExpressionConstants.Unevaluatable;
        }

        public virtual void ClearReplaceExpr()
        {
            _replaceList = null;
        }

        public override void Accept(ISqlAstVisitor visitor)
        {
            if (_replaceList == null)
            {
                visitor.Visit(this);
            }
            else
            {
                var temp = _exprList;
                _exprList = _replaceList;
                visitor.Visit(this);
                _exprList = temp;
            }
        }
    }
}