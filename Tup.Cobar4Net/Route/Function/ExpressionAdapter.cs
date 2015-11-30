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

namespace Tup.Cobar4Net.Route.Function
{
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public class ExpressionAdapter : IRuleAlgorithm
    {
        private readonly IExpression _expr;

        public ExpressionAdapter(IExpression expr)
        {
            _expr = expr;
        }

        public virtual IRuleAlgorithm ConstructMe(params object[] objects)
        {
            throw new NotSupportedException();
        }

        public virtual void Initialize()
        {
        }

        public virtual Number[] Calculate(IDictionary<object, object> parameters)
        {
            int[] rst;
            var eval = _expr.Evaluation(parameters);
            if (eval is int)
            {
                rst = new int[1];
                rst[0] = (int)eval;
            }
            else if (eval is int[])
            {
                rst = (int[])eval;
            }
            else if (eval is Number)
            {
                rst = new int[1];
                rst[0] = (int)(Number)eval;
            }
            else if (eval is string)
            {
                rst = new int[1];
                rst[0] = Convert.ToInt32((string)eval);
            }
            else if (eval is Number[])
            {
                var longs = (Number[])eval;
                rst = new int[longs.Length];
                for (int i = 0, len = longs.Length; i < len; ++i)
                {
                    rst[0] = (int)longs[i];
                }
            }
            else if (eval is long[])
            {
                var longs = (long[])eval;
                rst = new int[longs.Length];
                for (int i = 0, len = longs.Length; i < len; ++i)
                {
                    rst[0] = (int)longs[i];
                }
            }
            else
            {
                throw new ArgumentException("rule calculate err: result of route function is wrong type or null: " +
                                            eval);
            }
            return Number.ValueOf(rst);
        }
    }
}