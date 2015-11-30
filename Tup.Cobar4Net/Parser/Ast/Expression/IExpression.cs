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

namespace Tup.Cobar4Net.Parser.Ast.Expression
{
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public interface IExpression : IAstNode
    {
        /// <value>
        ///     precedences are defined in
        ///     <see cref="IExpression" />
        /// </value>
        int Precedence { get; }

        /// <returns>this</returns>
        IExpression SetCacheEvalRst(bool cacheEvalRst);

        object Evaluation(IDictionary<object, object> parameters);
    }

    /// <summary>
    ///     Expression Constants
    /// </summary>
    public static class ExpressionConstants
    {
        public static readonly int PrecedenceQuery = 0;

        public static readonly int PrecedenceAssignment = 1;

        public static readonly int PrecedenceLogicalOr = 2;

        public static readonly int PrecedenceLogicalXor = 3;

        public static readonly int PrecedenceLogicalAnd = 4;

        public static readonly int PrecedenceLogicalNot = 5;

        public static readonly int PrecedenceBetweenAnd = 6;

        public static readonly int PrecedenceComparision = 7;

        public static readonly int PrecedenceAnyAllSubquery = 8;

        public static readonly int PrecedenceBitOr = 8;

        public static readonly int PrecedenceBitAnd = 10;

        public static readonly int PrecedenceBitShift = 11;

        public static readonly int PrecedenceArithmeticTermOp = 12;

        public static readonly int PrecedenceArithmeticFactorOp = 13;

        public static readonly int PrecedenceBitXor = 14;

        public static readonly int PrecedenceUnaryOp = 15;

        public static readonly int PrecedenceBinary = 16;

        public static readonly int PrecedenceCollate = 17;

        public static readonly int PrecedencePrimary = 19;

        public static readonly object Unevaluatable = new UnevaluatableExpression();

        #region ExpressionConstants.Unevaluatable

        /// <summary>
        ///     ExpressionConstants.Unevaluatable Expression
        /// </summary>
        private class UnevaluatableExpression : AbstractExpression
        {
            public override int Precedence
            {
                get { throw new NotSupportedException(); }
            }

            public override void Accept(ISqlAstVisitor visitor)
            {
                throw new NotSupportedException();
            }

            protected override object EvaluationInternal(IDictionary<object, object> parameters)
            {
                throw new NotSupportedException();
            }
        }

        #endregion ExpressionConstants.Unevaluatable
    }
}