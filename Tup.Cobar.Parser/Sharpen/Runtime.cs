using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace Sharpen
{
    public class Runtime
    {
        internal static byte[] GetBytesForString(string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }

        internal static byte[] GetBytesForString(string str, string encoding)
        {
            return Encoding.GetEncoding(encoding).GetBytes(str);
        }

        internal static void PrintStackTrace(Exception ex)
        {
            Console.WriteLine(ex);
        }

        internal static void PrintStackTrace(Exception ex, TextWriter tw)
        {
            tw.WriteLine(ex);
        }

        internal static string Substring(string str, int index)
        {
            return str.Substring(index);
        }

        internal static string Substring(string str, int index, int endIndex)
        {
            return str.Substring(index, endIndex - index);
        }

        internal static Type GetType(string name)
        {
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type t = a.GetType(name);
                if (t != null)
                    return t;
            }
            throw new InvalidOperationException("Type not found: " + name);
        }

        internal static void SetCharAt(StringBuilder sb, int index, char c)
        {
            sb[index] = c;
        }

        internal static bool EqualsIgnoreCase(string s1, string s2)
        {
            return s1.Equals(s2, StringComparison.CurrentCultureIgnoreCase);
        }

        internal static string GetStringForBytes(byte[] chars)
        {
            return Encoding.UTF8.GetString(chars);
        }

        internal static string GetStringForBytes(byte[] chars, string encoding)
        {
            return GetEncoding(encoding).GetString(chars);
        }

        internal static string GetStringForBytes(byte[] chars, int start, int len)
        {
            return Encoding.UTF8.GetString(chars, start, len);
        }

        //internal static string GetStringForBytes (byte[] chars, int start, int len, string encoding)
        //{
        //	return GetEncoding (encoding)..Decode (chars, start, len);
        //}

        internal static Encoding GetEncoding(string name)
        {
            //			Encoding e = Encoding.GetEncoding (name, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback);
            Encoding e = Encoding.GetEncoding(name.Replace('_', '-'));
            if (e is UTF8Encoding)
                return new UTF8Encoding(false, true);
            return e;
        }
    }
}