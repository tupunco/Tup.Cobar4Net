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

namespace Tup.Cobar4Net.Parser.Ast.Expression.Primary.Function.String
{
    /// <summary>
    ///     Trim Direction
    /// </summary>
    public enum TrimDirection
    {
        Default = 0,
        Both,
        Leading,
        Trailing
    }

    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public class Trim : FunctionExpression
    {
        public Trim(TrimDirection _trimDirection, IExpression remstr, IExpression str)
            : base("TRIM", WrapList(str, remstr))
        {
            Direction = _trimDirection;
        }

        /// <value>never null</value>
        public virtual IExpression StringValue
        {
            get { return Arguments[0]; }
        }

        public virtual IExpression RemainString
        {
            get
            {
                var args = Arguments;
                if (args.Count < 2)
                {
                    return null;
                }
                return Arguments[1];
            }
        }

        public virtual TrimDirection Direction { get; }

        private static IList<IExpression> WrapList(IExpression str, IExpression remstr)
        {
            if (str == null)
            {
                throw new ArgumentException("str is null");
            }
            var list = new List<IExpression>(remstr != null ? 2 : 1);
            list.Add(str);
            if (remstr != null)
            {
                list.Add(remstr);
            }
            return list;
        }

        public override FunctionExpression ConstructFunction(IList<IExpression> arguments)
        {
            throw new NotSupportedException("function of trim has special arguments");
        }

        public override void Accept(ISqlAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}