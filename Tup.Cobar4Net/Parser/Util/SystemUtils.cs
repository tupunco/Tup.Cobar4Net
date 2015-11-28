using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Tup.Cobar4Net.Parser
{
    public static class SystemUtils
    {
        private static readonly Regex s_EnumNameMapping_Reg = new Regex(@"([a-z])([A-Z])");

        /// <summary>
        /// Get Enum NameValueMapping
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <returns></returns>
        public static IDictionary<string, TEnum> GetEnumNameMapping<TEnum>(string prefix = null)
            where TEnum : struct
        {
            var keywords = new Dictionary<string, TEnum>();
            var values = (TEnum[])Enum.GetValues(typeof(TEnum));
            var names = Enum.GetNames(typeof(TEnum));
            var name = string.Empty;

            var prefixLen = (prefix ?? string.Empty).Length;
            var hasPrefix = prefixLen > 0;
            for (int i = 0; i < names.Length; i++)
            {
                name = names[i];
                if (hasPrefix)
                {
                    if (name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    {
                        keywords.Add(s_EnumNameMapping_Reg.Replace(name.Substring(prefixLen), "$1_$2").ToUpper(), values[i]);
                    }
                }
                else
                {
                    keywords.Add(s_EnumNameMapping_Reg.Replace(name, "$1_$2").ToUpper(), values[i]);
                }
            }
            keywords.Remove("NONE");
            return keywords;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetEnumName<TEnum>(this TEnum value)
            where TEnum : struct
        {
            return s_EnumNameMapping_Reg.Replace(value.ToString(), "$1_$2").ToUpper();
        }
    }
}