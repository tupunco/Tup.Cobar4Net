using System;
using System.Text;

namespace Sharpen
{
    internal static class Runtime
    {
        internal static long CurrentTimeMillis()
        {
            return DateTime.UtcNow.ToMillisecondsSinceEpoch();
        }

        #region Encoding

        private readonly static Encoding s_Utf8_Encoding = Encoding.UTF8;

        public static byte[] GetBytesForString(string str)
        {
            return s_Utf8_Encoding.GetBytes(str);
        }

        public static byte[] GetBytesForString(string str, string encoding)
        {
            return Encoding.GetEncoding(encoding).GetBytes(str);
        }

        //public static void PrintStackTrace(Exception ex)
        //{
        //    Console.WriteLine(ex);
        //}

        //public static void PrintStackTrace(Exception ex, TextWriter tw)
        //{
        //    tw.WriteLine(ex);
        //}

        public static string Substring(string str, int index)
        {
            return str.Substring(index);
        }

        public static string Substring(string str, int index, int endIndex)
        {
            return str.Substring(index, endIndex - index);
        }

        //public static Type GetType(string name)
        //{
        //    foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
        //    {
        //        Type t = a.GetType(name);
        //        if (t != null)
        //            return t;
        //    }
        //    throw new InvalidOperationException("Type not found: " + name);
        //}

        //public static void SetCharAt(StringBuilder sb, int index, char c)
        //{
        //    sb[index] = c;
        //}

        public static bool EqualsIgnoreCase(string s1, string s2)
        {
            return s1.Equals(s2, StringComparison.CurrentCultureIgnoreCase);
        }

        public static string GetStringForBytes(byte[] chars)
        {
            return s_Utf8_Encoding.GetString(chars);
        }

        public static string GetStringForBytes(byte[] chars, string encoding)
        {
            return GetEncoding(encoding).GetString(chars);
        }

        public static string GetStringForBytes(byte[] chars, int start, int len)
        {
            return s_Utf8_Encoding.GetString(chars, start, len);
        }

        public static string GetStringForBytes(byte[] chars, int start, int len, string encoding)
        {
            return GetEncoding(encoding).GetString(chars, start, len);
        }

        public static Encoding GetEncoding(string name)
        {
            //			Encoding e = Encoding.GetEncoding (name, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback);
            Encoding e = Encoding.GetEncoding(name.Replace('_', '-'));
            if (e is UTF8Encoding)
                return new UTF8Encoding(false, true);
            return e;
        }

        #endregion Encoding
    }
}