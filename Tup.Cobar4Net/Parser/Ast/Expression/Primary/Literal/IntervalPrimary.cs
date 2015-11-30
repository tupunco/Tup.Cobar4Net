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

namespace Tup.Cobar4Net.Parser.Ast.Expression.Primary.Literal
{
    /// <summary>
    ///     IntervalPrimary Unit
    /// </summary>
    public enum Unit
    {
        None = 0,

        Microsecond,
        Second,
        Minute,
        Hour,
        Day,
        Week,
        Month,
        Quarter,
        Year,
        SecondMicrosecond,
        MinuteMicrosecond,
        MinuteSecond,
        HourMicrosecond,
        HourSecond,
        HourMinute,
        DayMicrosecond,
        DaySecond,
        DayMinute,
        DayHour,
        YearMonth
    }

    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public class IntervalPrimary : Literal
    {
        private static readonly IDictionary<string, Unit> unitMap = InitUnitMap();

        public IntervalPrimary(IExpression quantity, Unit unit)
        {
            if (quantity == null)
            {
                throw new ArgumentException("quantity expression is null");
            }
            if (unit == Unit.None)
            {
                throw new ArgumentException("unit of time is null");
            }
            Quantity = quantity;
            Unit = unit;
        }

        /// <value>never null</value>
        public virtual Unit Unit { get; }

        /// <value>never null</value>
        public virtual IExpression Quantity { get; }

        private static IDictionary<string, Unit> InitUnitMap()
        {
            return SystemUtils.GetEnumNameMapping<Unit>();
        }

        /// <param name="unitString">must be upper case, null is forbidden</param>
        public static Unit GetIntervalUnit(string unitString)
        {
            return unitMap.GetValue(unitString);
        }

        public override void Accept(ISqlAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}