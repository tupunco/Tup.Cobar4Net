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
using Expr = Tup.Cobar4Net.Parser.Ast.Expression.Expression;

namespace Tup.Cobar4Net.Route.Function
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class ExpressionAdapter : RuleAlgorithm
    {
        private readonly Expr expr;

        public ExpressionAdapter(Expr expr)
        {
            this.expr = expr;
        }

        public virtual RuleAlgorithm ConstructMe(params object[] objects)
        {
            throw new NotSupportedException();
        }

        public virtual void Initialize()
        {
        }

        public virtual int[] Calculate(IDictionary<object, object> parameters)
        {
            int[] rst;
            object eval = expr.Evaluation(parameters);
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
                rst[0] = (int)((Number)eval);
            }
            else if (eval is string)
            {
                rst = new int[1];
                rst[0] = System.Convert.ToInt32(((string)eval));
            }
            else if (eval is int[])
            {
                int[] ints = (int[])eval;
                rst = new int[ints.Length];
                for (int i = 0, len = ints.Length; i < len; ++i)
                {
                    rst[0] = ints[i];
                }
            }
            else if (eval is Number[])
            {
                Number[] longs = (Number[])eval;
                rst = new int[longs.Length];
                for (int i = 0, len = longs.Length; i < len; ++i)
                {
                    rst[0] = (int)longs[i];
                }
            }
            else if (eval is long[])
            {
                long[] longs = (long[])eval;
                rst = new int[longs.Length];
                for (int i = 0, len = longs.Length; i < len; ++i)
                {
                    rst[0] = (int)longs[i];
                }
            }
            else
            {
                throw new ArgumentException("rule calculate err: result of route function is wrong type or null: " + eval);
            }
            return rst;
        }
    }
}