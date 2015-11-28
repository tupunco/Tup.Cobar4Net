using Deveel.Math;
using System.Linq;

namespace System
{
    /// <summary>
    /// Number TypeCode
    /// </summary>
    public enum NumberTypeCode
    {
        Empty = 0,

        //Int16 = 1,
        Int32 = 2,

        Int64 = 3,
        Single = 4,
        Double = 5,
        Decimal = 6,
        BigInteger = 7,
        BigDecimal = 8,
    }

    /// <summary>
    /// Number
    /// </summary>
    public sealed class Number
        : IComparable, IConvertible, IComparable<Number>, IEquatable<Number>
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
            if (value is Number)
                m_value = (value as Number).m_value;
            else
                m_value = value;
            m_typeCode = typeCode;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        public Number(object value)
            : this(value, GetTypeCode(value))
        {
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
        public Number(float value)
            : this(value, NumberTypeCode.Single)
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

        private static NumberTypeCode GetTypeCode(object value)
        {
            if (value == null)
                return NumberTypeCode.Int32;

            var t = value.GetType();
            if (t == typeof(double))
                return NumberTypeCode.Double;
            else if (t == typeof(float))
                return NumberTypeCode.Single;
            else if (t == typeof(long))
                return NumberTypeCode.Int64;
            else if (t == typeof(int))
                return NumberTypeCode.Int32;
            else if (t == typeof(decimal))
                return NumberTypeCode.Decimal;
            else if (t == typeof(BigInteger))
                return NumberTypeCode.BigInteger;
            else if (t == typeof(BigDecimal))
                return NumberTypeCode.BigDecimal;
            else
                return NumberTypeCode.Double;
        }

        public static explicit operator double (Number value)
        {
            return Convert.ToDouble(value.m_value);
        }

        public static explicit operator float (Number value)
        {
            return Convert.ToSingle(value.m_value);
        }

        public static explicit operator long (Number value)
        {
            return Convert.ToInt64(value.m_value);
        }

        public static explicit operator int (Number value)
        {
            return Convert.ToInt32(value.m_value);
        }

        public static explicit operator decimal (Number value)
        {
            return Convert.ToDecimal(value.m_value);
        }

        public static explicit operator BigInteger(Number value)
        {
            return Convert.ToInt64(value.m_value);
        }

        public static explicit operator BigDecimal(Number value)
        {
            return Convert.ToInt32(value.m_value);
        }

        public static implicit operator Number(double value)
        {
            return new Number(value);
        }

        public static implicit operator Number(float value)
        {
            return new Number(value);
        }

        public static implicit operator Number(long value)
        {
            return new Number(value);
        }

        public static implicit operator Number(int value)
        {
            return new Number(value);
        }

        public static implicit operator Number(decimal value)
        {
            return new Number(value);
        }

        public static implicit operator Number(BigInteger value)
        {
            return new Number(value);
        }

        public static implicit operator Number(BigDecimal value)
        {
            return new Number(value);
        }

        public override string ToString()
        {
            return m_value.ToString();
        }

        /// <summary>
        /// ValueOf
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static Number[] ValueOf<T>(T[] arr)
        {
            if (arr == null)
                return new Number[0];

            return arr.Select(x => new Number(x)).ToArray();
        }

        /// <summary>
        /// ValueTo
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static int[] ToInt32(Number[] arr)
        {
            if (arr == null)
                return new int[0];

            return arr.Select(x => (int)x).ToArray();
        }

        /// <summary>
        /// ValueTo
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static long[] ToInt64(Number[] arr)
        {
            if (arr == null)
                return new long[0];

            return arr.Select(x => (long)x).ToArray();
        }

        #region IComparable
        /// <summary>
        /// IComparable
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            return CompareTo((Number)obj);
        }
        /// <summary>
        /// CompareTo
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(Number other)
        {
            if (other == null)
                return -1;

            var c1 = (IComparable)this.m_value;
            var c2 = (IComparable)other.m_value;

            return c1.CompareTo(c2);
        }
        #endregion

        #region IEquatable
        /// <summary>
        /// IEquatable
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Number other)
        {
            return CompareTo(other) == 0;
        }
        #endregion

        #region IConvertible
        public TypeCode GetTypeCode()
        {
            return System.TypeCode.Object;
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            var b = (int)this;
            return b > 1;
        }

        public char ToChar(IFormatProvider provider)
        {
            return (char)((int)this);
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            return (sbyte)((int)this);
        }

        public byte ToByte(IFormatProvider provider)
        {
            return (byte)((int)this);
        }

        public short ToInt16(IFormatProvider provider)
        {
            return (short)((int)this);
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            return (ushort)((int)this);
        }

        public int ToInt32(IFormatProvider provider)
        {
            return (int)this;
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            return (uint)this;
        }

        public long ToInt64(IFormatProvider provider)
        {
            return (long)this;
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            return (ulong)((long)this);
        }

        public float ToSingle(IFormatProvider provider)
        {
            return (float)this;
        }

        public double ToDouble(IFormatProvider provider)
        {
            return (double)this;
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            return (decimal)this;
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public string ToString(IFormatProvider provider)
        {
            return m_value.ToString();
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            if (conversionType == typeof(Number))
                return this;

            return Convert.ChangeType(this.m_value, conversionType, provider);
        }
        #endregion

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
