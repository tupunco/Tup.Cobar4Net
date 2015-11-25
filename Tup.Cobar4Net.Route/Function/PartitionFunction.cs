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

using Tup.Cobar4Net.Parser.Ast.Expression.Primary.Function;
using Tup.Cobar4Net.Route.Util;
using Tup.Cobar4Net.Util;
using Expr = Tup.Cobar4Net.Parser.Ast.Expression.Expression;

namespace Tup.Cobar4Net.Route.Function
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public abstract class PartitionFunction : FunctionExpression
    {
        public PartitionFunction(string functionName, IList<Expr> arguments)
            : base(functionName, arguments)
        {
        }

        protected internal int[] count;

        protected internal int[] length;

        protected internal PartitionUtil partitionUtil;

        public string PartitionCount
        {
            get { return string.Join(",", count); }
            set { SetPartitionCount(value); }
        }
        public virtual void SetPartitionCount(string partitionCount)
        {
            this.count = ToIntArray(partitionCount);
        }

        public string PartitionLength
        {
            get { return string.Join(",", length); }
            set { SetPartitionLength(value); }
        }
        public virtual void SetPartitionLength(string partitionLength)
        {
            this.length = ToIntArray(partitionLength);
        }

        private static int[] ToIntArray(string @string)
        {
            string[] strs = SplitUtil.Split(@string, ',', true);
            int[] ints = new int[strs.Length];
            for (int i = 0; i < strs.Length; ++i)
            {
                ints[i] = System.Convert.ToInt32(strs[i]);
            }
            return ints;
        }

        public override void Init()
        {
            partitionUtil = new PartitionUtil(count, length);
        }

        protected internal virtual int PartitionIndex(long hash)
        {
            return partitionUtil.Partition(hash);
        }

        protected abstract override object EvaluationInternal(IDictionary<object, object> parameters);
    }
}