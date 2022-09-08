using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;

namespace MagicFile.Test.Utils
{
    class Util
    {
        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <returns></returns>
        public static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds).ToString();
        }

        ///// <summary>
        ///// 返回音频时长
        ///// </summary>
        ///// <param name="SongPath">音频文件路径</param>
        ///// <returns></returns>
        //public static string GetVoiceTime(string SongPath)
        //{
        //    string dirName = Path.GetDirectoryName(SongPath);
        //    string SongName = Path.GetFileName(SongPath);
        //    ShellClass sh = new ShellClass();
        //    Folder dir = sh.NameSpace(dirName);
        //    FolderItem item = dir.ParseName(SongName);
        //    string SongTime = Regex.Match(dir.GetDetailsOf(item, -1), "\\d:\\d{2}:\\d{2}").Value;//返回音频时长
        //    return SongTime;
        //}

        /// <summary>
        /// 时间格式转毫秒值
        /// </summary>
        /// <param name="time">时间字符串</param>
        /// <returns></returns>
        public static long Cover(string time)
        {
            string[] a = time.Split(':');
            if (long.Parse(a[0]) == 0 && long.Parse(a[1]) == 0)
            {
                return long.Parse(a[2]) * 1000;
            }
            else if (long.Parse(a[0]) == 0 && long.Parse(a[1]) != 0)
            {
                return (long.Parse(a[1]) * 60 + long.Parse(a[2])) * 1000;
            }
            else if (long.Parse(a[0]) != 0 && long.Parse(a[1]) == 0)
            {
                return ((long.Parse(a[0]) * 60 * 60) + long.Parse(a[2])) * 1000;
            }
            else if (long.Parse(a[0]) != 0 && long.Parse(a[1]) != 0)
            {
                return (((long.Parse(a[0]) * 60) + long.Parse(a[1])) * 60) * 1000;
            }
            return 0;
        }
    }
}
