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
using Tup.Cobar4Net.Parser.Ast.Expression;
using Tup.Cobar4Net.Parser.Ast.Expression.Primary.Function;
using Tup.Cobar4Net.Route.Util;
using Tup.Cobar4Net.Util;

namespace Tup.Cobar4Net.Route.Function
{
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public abstract class PartitionFunction : FunctionExpression
    {
        protected internal int[] Count;

        protected internal int[] Length;

        protected internal PartitionUtil PartitionUtil;

        protected PartitionFunction(string functionName, IList<IExpression> arguments)
            : base(functionName, arguments)
        {
        }

        public virtual string PartitionCount
        {
            get { return string.Join(",", Count); }
            set { Count = ToIntArray(value); }
        }

        public virtual string PartitionLength
        {
            get { return string.Join(",", Length); }
            set { Length = ToIntArray(value); }
        }

        private static int[] ToIntArray(string @string)
        {
            var strs = SplitUtil.Split(@string, ',', true);
            var ints = new int[strs.Length];
            for (var i = 0; i < strs.Length; ++i)
            {
                ints[i] = Convert.ToInt32(strs[i]);
            }
            return ints;
        }

        public override void Init()
        {
            PartitionUtil = new PartitionUtil(Count, Length);
        }

        protected internal virtual int PartitionIndex(long hash)
        {
            return PartitionUtil.Partition(hash);
        }

        protected abstract override object EvaluationInternal(IDictionary<object, object> parameters);
    }
}