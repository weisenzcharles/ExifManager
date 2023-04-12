using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicFile.Common.Extensions
{
    /// <summary>
    /// 字符串扩展类。
    /// </summary>
    public static class StringExtension
    {

        /// <summary>
        /// 移除非法字符。
        /// </summary>
        /// <param name="input">需要处理的字符串。</param>
        /// <returns></returns>
        public static string TrimInvaildSymbol(this string input)
        {
            return input.Replace(":", "").Replace("/", "").Replace("\\", "").Replace("?", "").Replace("<", "").Replace(">", "").Replace("*", "").Replace("|", "").Replace("\"", "").Trim().Trim('.');
        }

    }
}
