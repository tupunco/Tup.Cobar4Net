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

namespace Tup.Cobar.Parser.Ast.Expression
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public interface Expression : ASTNode
    {
        /// <returns>
        /// precedences are defined in
        /// <see cref="Expression"/>
        /// </returns>
        int GetPrecedence();

        /// <returns>this</returns>
        Expression SetCacheEvalRst(bool cacheEvalRst);

        object Evaluation(IDictionary<object, Expression> parameters);
    }

    /// <summary>
    /// Expression Constants
    /// </summary>
    public static class ExpressionConstants
    {
        public readonly static int PrecedenceQuery = 0;

        public readonly static int PrecedenceAssignment = 1;

        public readonly static int PrecedenceLogicalOr = 2;

        public readonly static int PrecedenceLogicalXor = 3;

        public readonly static int PrecedenceLogicalAnd = 4;

        public readonly static int PrecedenceLogicalNot = 5;

        public readonly static int PrecedenceBetweenAnd = 6;

        public readonly static int PrecedenceComparision = 7;

        public readonly static int PrecedenceAnyAllSubquery = 8;

        public readonly static int PrecedenceBitOr = 8;

        public readonly static int PrecedenceBitAnd = 10;

        public readonly static int PrecedenceBitShift = 11;

        public readonly static int PrecedenceArithmeticTermOp = 12;

        public readonly static int PrecedenceArithmeticFactorOp = 13;

        public readonly static int PrecedenceBitXor = 14;

        public readonly static int PrecedenceUnaryOp = 15;

        public readonly static int PrecedenceBinary = 16;

        public readonly static int PrecedenceCollate = 17;

        public readonly static int PrecedencePrimary = 19;
    }
}