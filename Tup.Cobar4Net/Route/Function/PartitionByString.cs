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
using System.Linq;
using Tup.Cobar4Net.Config.Model.Rule;
using Tup.Cobar4Net.Parser.Ast.Expression;
using Tup.Cobar4Net.Parser.Ast.Expression.Primary.Function;
using Tup.Cobar4Net.Parser.Util;
using Tup.Cobar4Net.Util;

namespace Tup.Cobar4Net.Route.Function
{
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public sealed class PartitionByString : PartitionFunction, IRuleAlgorithm
    {
        /// <summary>0 means str.length(), -1 means str.length()-1</summary>
        private int _hashSliceEnd = 8;

        private int _hashSliceStart;

        private int m_DualHashLength;

        private string m_DualHashSlice = string.Empty;

        public PartitionByString(string functionName)
            : this(functionName, null)
        {
        }

        public PartitionByString(string functionName, IList<IExpression> arguments)
            : base(functionName, arguments)
        {
        }

        public int HashLength
        {
            get { return m_DualHashLength; }
            set
            {
                m_DualHashLength = value;

                HashSlice = value.ToString();
            }
        }

        public string HashSlice
        {
            get { return m_DualHashSlice; }
            set
            {
                m_DualHashSlice = value;

                var p = PairUtil.SequenceSlicing(value);
                _hashSliceStart = p.Key;
                _hashSliceEnd = p.Value;
            }
        }

        public Number[] Calculate(IDictionary<object, object> parameters)
        {
            var rst = new int[1];
            var arg = arguments[0].Evaluation(parameters);
            if (arg == ExpressionConstants.Unevaluatable)
                throw new ArgumentException("argument is UNEVALUATABLE");

            //TODO string key = arg.ToString();
            var key = (arg ?? "null").ToString();
            var start = _hashSliceStart >= 0 ? _hashSliceStart : key.Length + _hashSliceStart;
            var end = _hashSliceEnd > 0 ? _hashSliceEnd : key.Length + _hashSliceEnd;
            var hash = StringUtil.Hash(key, start, end);
            rst[0] = PartitionIndex(hash);
            return Number.ValueOf(rst);
        }

        public IRuleAlgorithm ConstructMe(params object[] objects)
        {
            var args = objects.Select(x => (IExpression)x).ToList();

            var partitionFunc = new PartitionByString(functionName, args)
            {
                _hashSliceStart = _hashSliceStart,
                _hashSliceEnd = _hashSliceEnd,
                Count = Count,
                Length = Length
            };
            return partitionFunc;
        }

        public void Initialize()
        {
            Init();
        }

        protected override object EvaluationInternal(IDictionary<object, object> parameters)
        {
            return Calculate(parameters)[0];
        }

        public override FunctionExpression ConstructFunction(IList<IExpression> arguments)
        {
            if (arguments == null || arguments.Count != 1)
            {
                throw new ArgumentException("function " + FunctionName + " must have 1 argument but is "
                                            + arguments);
            }

            var args = new object[arguments.Count];
            var i = -1;
            foreach (var arg in arguments)
            {
                args[++i] = arg;
            }
            return (FunctionExpression)ConstructMe(args);
        }
    }
}