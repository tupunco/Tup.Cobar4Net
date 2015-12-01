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
using System.Diagnostics.CodeAnalysis;
using Deveel.Math;
using Tup.Cobar4Net.Parser.Ast.Expression.Primary.Literal;
using Tup.Cobar4Net.Parser.Recognizer.Mysql;
using Tup.Cobar4Net.Parser.Recognizer.Mysql.Lexer;

namespace Tup.Cobar4Net.Parser.Util
{
    /// <summary>adapt Java's expression rule into MySql's</summary>
    /// <author>
    ///     <a href="mailto:shuo.qius@alibaba-inc.com">QIU Shuo</a>
    /// </author>
    public class ExprEvalUtils
    {
        private const int ClassMapDouble = 1;

        private const int ClassMapFloat = 2;

        private const int ClassMapBigIng = 3;

        private const int ClassMapBigDecimal = 4;

        private const int ClassMapLong = 5;

        private const int NumInt = 1;

        private const int NumLong = 2;

        private const int NumBigInteger = 3;

        private const int NumBigDecimal = 4;

        private static readonly IDictionary<Type, int> classMap = new Dictionary<Type, int>(5);

        static ExprEvalUtils()
        {
            classMap[typeof (double)] = ClassMapDouble;
            classMap[typeof (float)] = ClassMapFloat;
            classMap[typeof (BigInteger)] = ClassMapBigIng;
            classMap[typeof (BigDecimal)] = ClassMapBigDecimal;
            classMap[typeof (long)] = ClassMapLong;
        }

        public static bool Obj2bool(object obj)
        {
            if (obj == LiteralBoolean.True)
            {
                return true;
            }
            if (obj == LiteralBoolean.False)
            {
                return false;
            }
            if (obj is bool)
            {
                return (bool)obj;
            }

            Number num = null;
            if (obj is string)
            {
                num = String2Number((string)obj);
            }
            else
            {
                num = (Number)obj;
            }
            var classType = classMap[num.GetType()];
            if (classType == 0)
            {
                return (int)num != 0;
            }
            switch (classType)
            {
                case ClassMapBigDecimal:
                {
                    return BigDecimal.Zero.CompareTo((BigDecimal)num) != 0;
                }

                case ClassMapBigIng:
                {
                    return BigInteger.Zero.CompareTo((BigInteger)num) != 0;
                }

                case ClassMapDouble:
                {
#pragma warning disable RECS0018 // Comparison of floating point numbers with equality operator
                    return (double)num != 0d;
#pragma warning restore RECS0018 // Comparison of floating point numbers with equality operator
                }

                case ClassMapFloat:
                {
#pragma warning disable RECS0018 // Comparison of floating point numbers with equality operator
                    return (float)num != 0f;
#pragma warning restore RECS0018 // Comparison of floating point numbers with equality operator
                }

                case ClassMapLong:
                {
                    return (long)num != 0L;
                }

                default:
                {
                    throw new ArgumentException("unsupported number type: " + num.GetType());
                }
            }
        }

        public static Number Calculate(IUnaryOperandCalculator cal, Number num)
        {
            if (num == null)
            {
                return 0;
            }
            switch (num.TypeCode)
            {
                case NumberTypeCode.Int32:
                    return cal.Calculate((int)num);

                case NumberTypeCode.Int64:
                    return cal.Calculate((long)num);

                case NumberTypeCode.BigInteger:
                    return cal.Calculate((BigInteger)num);

                case NumberTypeCode.Double:
                case NumberTypeCode.Decimal:
                case NumberTypeCode.BigDecimal:
                    return cal.Calculate((BigDecimal)num);
            }

            throw new ArgumentException("unsupported add calculate: " + num.GetType());
        }

        public static Number Calculate(IBinaryOperandCalculator cal, Number n1, Number n2)
        {
            if (n1 == null || n2 == null)
            {
                return 0;
            }
            switch (n1.TypeCode)
            {
                case NumberTypeCode.Int32:
                    return cal.Calculate((int)n1, (int)n2);

                case NumberTypeCode.Int64:
                    return cal.Calculate((long)n1, (long)n2);

                case NumberTypeCode.BigInteger:
                    return cal.Calculate((BigInteger)n1, (BigInteger)n2);

                case NumberTypeCode.Double:
                case NumberTypeCode.Decimal:
                case NumberTypeCode.BigDecimal:
                    return cal.Calculate((BigDecimal)n1, (BigDecimal)n2);
            }
            throw new ArgumentException("unsupported add calculate: " + n1.GetType());
        }

        /// <param name="obj1">class of String or NumberValue</param>
        public static Pair<Number, Number> ConvertNum2SameLevel(object obj1, object obj2)
        {
            Number n1;
            Number n2;
            if (obj1 is string)
            {
                n1 = String2Number((string)obj1);
            }
            else
            {
                n1 = (Number)obj1;
            }
            if (obj2 is string)
            {
                n2 = String2Number((string)obj2);
            }
            else
            {
                n2 = (Number)obj2;
            }
            if (n1 == null || n2 == null)
            {
                return new Pair<Number, Number>(n1, n2);
            }
            var l1 = GetNumberLevel(n1.GetType());
            var l2 = GetNumberLevel(n2.GetType());
            if (l1 > l2)
            {
                n2 = UpTolevel(n2, l1);
            }
            else
            {
                if (l1 < l2)
                {
                    n1 = UpTolevel(n1, l2);
                }
            }
            return new Pair<Number, Number>(n1, n2);
        }

        private static Number UpTolevel(Number num, int level)
        {
            if (num == null)
                return 0;

            switch (level)
            {
                case NumInt:
                {
                    if (num.TypeCode == NumberTypeCode.Int32)
                    {
                        return num;
                    }
                    return num;
                }

                case NumLong:
                {
                    if (num.TypeCode == NumberTypeCode.Int64)
                    {
                        return num;
                    }
                    return num;
                }

                case NumBigInteger:
                {
                    if (num.TypeCode == NumberTypeCode.BigInteger)
                    {
                        return num;
                    }
                    return BigInteger.Parse(num.ToString());
                }

                case NumBigDecimal:
                {
                    if (num.TypeCode == NumberTypeCode.BigDecimal)
                    {
                        return num;
                    }
                    return BigDecimal.Parse(num.ToString());
                }

                default:
                {
                    throw new ArgumentException("unsupported number level: " + level);
                }
            }
        }

        private static int GetNumberLevel(Type clz)
        {
            if (typeof (int).IsAssignableFrom(clz))
            {
                return NumInt;
            }
            if (typeof (long).IsAssignableFrom(clz))
            {
                return NumLong;
            }
            if (typeof (BigInteger).IsAssignableFrom(clz))
            {
                return NumBigInteger;
            }
            if (typeof (BigDecimal).IsAssignableFrom(clz))
            {
                return NumBigDecimal;
            }
            if (typeof (Number).IsAssignableFrom(clz))
            {
                return NumLong;
            }
            throw new ArgumentException("unsupported number class: " + clz);
        }

        [SuppressMessage("Potential Code Quality Issues",
            "RECS0022:A catch clause that catches System.Exception and has an empty body", Justification = "<¹ÒÆð>")]
        public static Number String2Number(string str)
        {
            if (str == null)
            {
                return 0;
            }

            //TODO --ExprEvalUtils-String2Number
            try
            {
                return Convert.ToInt32(str);
            }
            catch (Exception ex)
            {
                Console.WriteLine("String2Number-ToInt32-{0}-{1}", str, ex);
            }
            try
            {
                return Convert.ToInt64(str);
            }
            catch (Exception ex)
            {
                Console.WriteLine("String2Number-ToInt64-{0}-{1}", str, ex);
            }
            try
            {
                var lexer = new MySqlLexer(str);
                switch (lexer.Token())
                {
                    case MySqlToken.LiteralNumPureDigit:
                    {
                        return lexer.GetIntegerValue();
                    }

                    case MySqlToken.LiteralNumMixDigit:
                    {
                        return lexer.GetDecimalValue();
                    }

                    default:
                    {
                        throw new ArgumentException("unrecognized number: " + str);
                    }
                }
            }
            catch (SqlSyntaxErrorException e)
            {
                throw new ArgumentException("str", e);
            }
        }
    }
}