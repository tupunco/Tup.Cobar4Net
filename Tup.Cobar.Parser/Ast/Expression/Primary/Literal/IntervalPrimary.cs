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

namespace Tup.Cobar.Parser.Ast.Expression.Primary.Literal
{
    /// <author><a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a></author>
    public class IntervalPrimary : Literal
    {
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

        private static readonly IDictionary<string, IntervalPrimary.Unit> unitMap = InitUnitMap();

        private static IDictionary<string, IntervalPrimary.Unit> InitUnitMap()
        {
            //TODO---IntervalPrimary InitUnitMap
            IntervalPrimary.Unit[] units = null;// typeof(IntervalPrimary.Unit).GetEnumConstants();
            IDictionary<string, IntervalPrimary.Unit> map = new Dictionary<string, IntervalPrimary.Unit>(units.Length);
            foreach (IntervalPrimary.Unit unit in units)
            {
                map[unit.ToString()] = unit;
            }
            return map;
        }

        /// <param name="unitString">must be upper case, null is forbidden</param>
        public static IntervalPrimary.Unit GetIntervalUnit(string unitString)
        {
            return unitMap[unitString];
        }

        private readonly IntervalPrimary.Unit unit;

        private readonly Tup.Cobar.Parser.Ast.Expression.Expression quantity;

        public IntervalPrimary(Tup.Cobar.Parser.Ast.Expression.Expression quantity, IntervalPrimary.Unit unit)
            : base()
        {
            if (quantity == null)
            {
                throw new ArgumentException("quantity expression is null");
            }
            if (unit == Unit.None)
            {
                throw new ArgumentException("unit of time is null");
            }
            this.quantity = quantity;
            this.unit = unit;
        }

        /// <returns>never null</returns>
        public virtual IntervalPrimary.Unit GetUnit()
        {
            return unit;
        }

        /// <returns>never null</returns>
        public virtual Tup.Cobar.Parser.Ast.Expression.Expression GetQuantity()
        {
            return quantity;
        }

        public override void Accept(SQLASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}