using Deveel.Math;
using System;

namespace Sharpen
{
    public enum NumberTypeCode
    {
        Empty = 0,
        //Int16 = 1,
        Int32 = 2,
        Int64 = 3,
        //Single = 4,
        Double = 5,
        Decimal = 6,
        BigInteger = 7,
        BigDecimal = 8,
    }
    /// <summary>
    /// Number
    /// </summary>
    public sealed class Number
    {
        private readonly object m_value;
        private readonly NumberTypeCode m_typeCode;

        /// <summary>
        /// TypeCode
        /// </summary>
        public NumberTypeCode TypeCode
        {
            get { return m_typeCode; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="typeCode"></param>
        private Number(object value, NumberTypeCode typeCode)
        {
            m_value = value;
            m_typeCode = typeCode;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public Number(decimal value)
            : this(value, NumberTypeCode.Decimal)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public Number(double value)
            : this(value, NumberTypeCode.Double)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public Number(int value)
            : this(value, NumberTypeCode.Int32)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public Number(long value)
            : this(value, NumberTypeCode.Int64)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public Number(BigInteger value)
            : this(value, NumberTypeCode.BigInteger)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public Number(BigDecimal value)
            : this(value, NumberTypeCode.BigDecimal)
        {
        }

        public static explicit operator double (Number value) { return Convert.ToDouble(value); }
        public static explicit operator long (Number value) { return Convert.ToInt64(value); }
        public static explicit operator int (Number value) { return Convert.ToInt32(value); }
        public static explicit operator decimal (Number value) { return Convert.ToDecimal(value); }
        public static explicit operator BigInteger(Number value) { return Convert.ToInt64(value); }
        public static explicit operator BigDecimal(Number value) { return Convert.ToInt32(value); }

        public static implicit operator Number(double value) { return new Number(value); }
        public static implicit operator Number(long value) { return new Number(value); }
        public static implicit operator Number(int value) { return new Number(value); }
        public static implicit operator Number(decimal value) { return new Number(value); }
        public static implicit operator Number(BigInteger value) { return new Number(value); }
        public static implicit operator Number(BigDecimal value) { return new Number(value); }


        public double DoubleValue()
        {
            return (double)m_value;
        }

        public float FloatValue()
        {
            return (float)m_value;
        }

        public sbyte ByteValue()
        {
            return (sbyte)m_value;
        }

        public int IntValue()
        {
            return (int)m_value;
        }

        public long LongValue()
        {
            return (long)m_value;
        }

        public short ShortValue()
        {
            return (short)m_value;
        }

        //public static Number GetInstance(object obj)
        //{
        //    if (obj is Number)
        //    {
        //        return (Number)obj;
        //    }

        //    if (!IsNumberType(obj))
        //    {
        //        throw new ArgumentException();
        //    }

        //    return new Number(Convert.ToDecimal(obj));
        //}

        //private static bool IsNumberType(object obj)
        //{
        //    return obj is sbyte
        //        || obj is byte
        //        || obj is short
        //        || obj is ushort
        //        || obj is int
        //        || obj is uint
        //        || obj is long
        //        || obj is ulong
        //        || obj is float
        //        || obj is double
        //        || obj is decimal;
        //}
    }
}