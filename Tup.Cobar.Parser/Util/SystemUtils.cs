using System;

namespace Tup.Cobar.Parser
{
    public static class SystemUtils
    {
        /// <summary>
        /// 将指定数目的字节从起始于特定偏移量的源数组复制到起始于特定偏移量的目标数组。
        /// </summary>
        /// <param name="src"></param>
        /// <param name="srcOffset"></param>
        /// <param name="dst"></param>
        /// <param name="dstOffset"></param>
        /// <param name="count"></param>
        public static void arraycopy(char[] src, int srcOffset, char[] dst, int dstOffset, int count)
        {
            Array.Copy(src, srcOffset, dst, dstOffset, count);
        }
    }
}