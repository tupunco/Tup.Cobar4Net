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
using Tup.Cobar4Net.Config.Model.Rule;
using Tup.Cobar4Net.Parser.Ast.Expression;
using Tup.Cobar4Net.Parser.Ast.Expression.Primary.Function;
using Tup.Cobar4Net.Parser.Util;
using Tup.Cobar4Net.Util;
using Expr = Tup.Cobar4Net.Parser.Ast.Expression.Expression;

namespace Tup.Cobar4Net.Route.Function
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public sealed class PartitionByString : PartitionFunction, RuleAlgorithm
    {
        public PartitionByString(string functionName)
            : this(functionName, null)
        {
        }

        public PartitionByString(string functionName, IList<Expr> arguments)
            : base(functionName, arguments)
        {
        }

        private int hashSliceStart = 0;

        /// <summary>0 means str.length(), -1 means str.length()-1</summary>
        private int hashSliceEnd = 8;

        public void SetHashLength(int hashLength)
        {
            SetHashSlice(hashLength.ToString());
        }

        public void SetHashSlice(string hashSlice)
        {
            Pair<int, int> p = PairUtil.SequenceSlicing(hashSlice);
            hashSliceStart = p.GetKey();
            hashSliceEnd = p.GetValue();
        }

        protected override object EvaluationInternal(IDictionary<object, object> parameters)
        {
            return Calculate(parameters)[0];
        }

        public int[] Calculate(IDictionary<object, object> parameters)
        {
            int[] rst = new int[1];
            object arg = arguments[0].Evaluation(parameters);
            if (arg == ExpressionConstants.Unevaluatable)
            {
                throw new ArgumentException("argument is UNEVALUATABLE");
            }
            string key = arg.ToString();
            int start = hashSliceStart >= 0 ? hashSliceStart : key.Length + hashSliceStart;
            int end = hashSliceEnd > 0 ? hashSliceEnd : key.Length + hashSliceEnd;
            long hash = StringUtil.Hash(key, start, end);
            rst[0] = PartitionIndex(hash);
            return rst;
        }

        public override FunctionExpression ConstructFunction(IList<Expr> arguments)
        {
            if (arguments == null || arguments.Count != 1)
            {
                throw new ArgumentException("function " + GetFunctionName() + " must have 1 argument but is "
                     + arguments);
            }
            object[] args = new object[arguments.Count];
            int i = -1;
            foreach (Expr arg in arguments)
            {
                args[++i] = arg;
            }
            return (FunctionExpression)ConstructMe(args);
        }

        public RuleAlgorithm ConstructMe(params object[] objects)
        {
            IList<Expr> args = new List<Expr>(objects.Length);
            foreach (object obj in objects)
            {
                args.Add((Expr)obj);
            }
            var partitionFunc = new PartitionByString(functionName, args);
            partitionFunc.hashSliceStart = hashSliceStart;
            partitionFunc.hashSliceEnd = hashSliceEnd;
            partitionFunc.count = count;
            partitionFunc.length = length;
            return partitionFunc;
        }

        public void Initialize()
        {
            Init();
        }
    }
}