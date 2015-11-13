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
using Tup.Cobar.Parser.Visitor;

namespace Tup.Cobar.Parser.Ast.Expression.Primary.Function.String
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class Trim : FunctionExpression
    {
        public enum Direction
        {
            Default,
            Both,
            Leading,
            Trailing
        }

        private readonly Trim.Direction direction;

        private static IList<Expression> WrapList(Expression str, Expression remstr)
        {
            if (str == null)
            {
                throw new ArgumentException("str is null");
            }
            IList<Expression> list = remstr != null ? new List
                <Tup.Cobar.Parser.Ast.Expression.Expression>(2) : new List<Expression >(1);
            list.Add(str);
            if (remstr != null)
            {
                list.Add(remstr);
            }
            return list;
        }

        public Trim(Trim.Direction direction, Expression remstr, Expression str)
            : base("TRIM", WrapList(str, remstr))
        {
            this.direction = direction;
        }

        /// <returns>never null</returns>
        public virtual Expression GetString()
        {
            return GetArguments()[0];
        }

        public virtual Expression GetRemainString()
        {
            IList<Expression> args = GetArguments();
            if (args.Count < 2)
            {
                return null;
            }
            return GetArguments()[1];
        }

        public virtual Trim.Direction GetDirection()
        {
            return direction;
        }

        public override FunctionExpression ConstructFunction(IList<Expression> arguments)
        {
            throw new NotSupportedException("function of trim has special arguments");
        }

        public override void Accept(SQLASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}