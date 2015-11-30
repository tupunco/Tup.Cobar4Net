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

namespace Tup.Cobar4Net.Route.Function
{
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public sealed class PartitionByLong : PartitionFunction, IRuleAlgorithm
    {
        public PartitionByLong(string functionName)
            : this(functionName, null)
        {
        }

        public PartitionByLong(string functionName, IList<IExpression> arguments)
            : base(functionName, arguments)
        {
        }

        public Number[] Calculate(IDictionary<object, object> parameters)
        {
            var rst = new int[1];
            var arg = arguments[0].Evaluation(parameters) ?? 0L;

            //if (arg == null)
            //{
            //    throw new ArgumentException("partition key is null ");
            //}
            //else
            //{
            if (arg == ExpressionConstants.Unevaluatable)
            {
                throw new ArgumentException("argument is UNEVALUATABLE");
            }
            //}
            Number key = null;
            if (arg is Number)
                key = (Number)arg;
            else if (arg is long)
                key = (long)arg;
            else if (arg is string)
                key = long.Parse((string)arg);
            else
                throw new ArgumentException("unsupported data type for partition key: " + arg.GetType());

            rst[0] = PartitionIndex((long)key);
            return Number.ValueOf(rst);
        }

        public IRuleAlgorithm ConstructMe(params object[] objects)
        {
            var args = objects.Select(x => (IExpression)x).ToList();

            var partitionFunc = new PartitionByLong(functionName, args)
            {
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