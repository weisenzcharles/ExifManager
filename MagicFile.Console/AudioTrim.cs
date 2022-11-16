using ATL;
using ATL.CatalogDataReaders;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MagicFile
{
    public class AudioTrim
    {
        /// <summary>
        /// 音频剪切。
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="destPath"></param>
        public static void Trim(string sourcePath, string destPath)
        {
            //var dir = @"\\192.168.0.199\Downloads\Music";
            var directories = AudioTrim.GetDirectories(sourcePath);
            if (directories.Any())
            {
                foreach (var directory in directories)
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(directory);
                    FileInfo[] fileInfos = directoryInfo.GetFiles("*.cue");
                    if (fileInfos.Any())
                    {
                        foreach (var fileInfo in fileInfos)
                        {
                            // 备份文件
                            CopyFile(fileInfo.FullName, fileInfo.FullName.Replace(fileInfo.Extension, ".bak"));
                            ConvertCoding(fileInfo.FullName);

                            string wavFile = fileInfo.FullName.Replace(fileInfo.Extension, ".wav");
                            if (File.Exists(wavFile))
                            {
                                TrimAlbum(fileInfo.FullName, wavFile, destPath);
                            }
                        }
                    }

                }
            }
        }

        public static void ConvertCoding(string filePath)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encoding encoding = Encoding.GetEncoding("GB18030");
            string content = File.ReadAllText(filePath, encoding);
            File.WriteAllText(filePath, content, Encoding.UTF8);
        }




        /// <summary>
        ///  将现有文件复制到新文件。允许覆盖同名的文件。
        /// </summary>
        /// <param name="sourceFileName">要复制的文件。</param>
        /// <param name="destFileName">目标文件的名称。 不能是目录。</param>
        /// <returns></returns>
        public static bool CopyFile(string sourceFileName, string destFileName)
        {

            if (File.Exists(sourceFileName))
            {
                
                File.Copy(sourceFileName, destFileName, true);

                if (File.Exists(destFileName))
                {
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// 分割专辑。
        /// </summary>
        /// <param name="cueFile"></param>
        /// <param name="wavPath"></param>
        /// <param name="destPath"></param>
        public static void TrimAlbum(string cueFile, string wavFile, string destPath)
        {
            Console.WriteLine("正在处理「" + cueFile + "」");
            FileInfo fileInfo = new FileInfo(wavFile);

            Track track = new Track(wavFile);

            ICatalogDataReader catalogData = CatalogDataReaderFactory.GetInstance().GetCatalogDataReader(cueFile);
            // 获取专辑信息
            IList<Track> tracks = catalogData.Tracks;

            #region 创建专辑目录

            // 艺术家路径
            string artistPath = string.Format(@"{0}\{1}", destPath, catalogData.Artist.Trim());
            if (!Directory.Exists(artistPath))
            {
                Directory.CreateDirectory(artistPath);
            }
            // 专辑路径
            string albumPath = string.Format(@"{0}\{1}\{2}", destPath, catalogData.Artist.Trim(), RemoveInvaildSymbol(ReplaceString(ReplaceFormat(catalogData.Title.Trim()))));

            Console.WriteLine("专辑路径：" + albumPath);
            if (!Directory.Exists(albumPath))
            {
                Directory.CreateDirectory(albumPath);
            }
      
            #endregion

            int currentTimeSpan = 0;

            // 音频时长（秒）
            int totalDuration = track.Duration;
            // 分割数量
            int trackCount = tracks.Count;

            // 循环切割
            for (int i = 0; i < trackCount; i++)
            {
                Track currentTrack = tracks[i];

                #region 处理标签...

                string title = ReplaceFormat(currentTrack.Title);
                string artist = ReplaceFormat(currentTrack.Artist);
                string album = ReplaceFormat(currentTrack.Album);
                if (title.Contains("-"))
                {
                    artist = ReplaceFormat(title.Substring(0, title.IndexOf("-")));
                    title = ReplaceFormat(title.Substring(title.IndexOf("-") + 1));
                }

                #endregion

                #region 处理路径...

                // 目标路径
                var destFileName = string.Format("{0}\\{1} - {2}{3}", albumPath, RemoveInvaildSymbol(artist.Trim()), RemoveInvaildSymbol(title), Path.GetExtension(wavFile).ToLower());

                Console.WriteLine("目标路径：" + destFileName);

                #endregion

                if (!File.Exists(destFileName))
                {
                    // 分割的开始时间
                    TimeSpan timeBegin = TimeSpan.FromSeconds(currentTimeSpan);
                    // 分割的结束时间
                    TimeSpan timeEnd = timeBegin + TimeSpan.FromSeconds(currentTrack.Duration);
                    // 音频切割
                    TrimWavFile(wavFile, destFileName, timeBegin, timeEnd);
                    currentTimeSpan = currentTimeSpan + currentTrack.Duration;

                    SaveTag(currentTrack.Title, currentTrack.Artist, currentTrack.Album, currentTrack.TrackNumber, destFileName);
                }
            }

            Console.WriteLine("正在处理「迁移文件」");

            MoveOtherFiles(cueFile, wavFile, fileInfo, catalogData, albumPath);

            Console.WriteLine("完成处理「" + cueFile + "」");
        }

        private static void MoveOtherFiles(string cueFile, string wavFile, FileInfo fileInfo, ICatalogDataReader catalogData, string albumPath)
        {
            string[] files = Directory.GetFiles(fileInfo.Directory.FullName, "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".png") || s.EndsWith(".jpg") || s.EndsWith(".txt")).ToArray();
            foreach (var file in files)
            {
                if (!file.Contains("免责声明"))
                {
                    string dest = string.Format("{0}\\{1}", albumPath, Path.GetFileName(file).ToLower());
                    CopyFile(file, dest);
                }
            }
            var destFileName = string.Empty;
            if (catalogData.Title.Contains("CD"))
            {
                destFileName = string.Format("{0}\\{1} - {2}", albumPath, catalogData.Artist.Trim(), RemoveInvaildSymbol(ReplaceString(ReplaceFormat(catalogData.Title.Replace("CD", " - CD").Trim()))));

            }
            else
            {
                destFileName = string.Format("{0}\\{1} - {2}", albumPath, catalogData.Artist.Trim(), RemoveInvaildSymbol(ReplaceString(ReplaceFormat(catalogData.Title.Trim()))));
            }

            CopyFile(cueFile, string.Format("{0}{1}", destFileName, Path.GetExtension(cueFile).ToLower()));

            CopyFile(wavFile, string.Format("{0}{1}", destFileName, Path.GetExtension(wavFile).ToLower()));

            ConvertCueFile(string.Format("{0}{1}", destFileName, Path.GetExtension(cueFile).ToLower()), Path.GetFileName(wavFile), Path.GetFileName(string.Format("{0}{1}", destFileName, Path.GetExtension(wavFile).ToLower())));

        }

        public static void ConvertCueFile(string filePath, string oldTile, string newTitle)
        {
            //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            //Encoding encoding = Encoding.GetEncoding("GB18030");
            string content = File.ReadAllText(filePath);
            content = content.Replace(oldTile, newTitle);
            File.WriteAllText(filePath, content);
        }

        public static string GetFileName(string title, string artist, string album, string extension)
        {
            if (title.Contains("-"))
            {
                artist = title.Substring(0, title.IndexOf("-"));
                title = title.Substring(title.IndexOf("-") + 1);
                return string.Format("{0} - {1}{2}", artist, title, extension);
            }
            else
            {
                if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(artist))
                {
                    return string.Format("{0} - {1}{2}", artist, title, extension);
                }
                else
                {
                    return string.Format("{0}{1}", title, extension);
                }
            }
        }

        public static void SaveTag(string title, string artist, string album, int? trackNumber, string filePath)
        {
            Console.WriteLine("正在处理「设置标签」");
            if (!string.IsNullOrEmpty(filePath))
            {
                Track track = new(filePath);
                track.Title = title;
                track.Album = album;
                track.Artist = artist;
                track.TrackNumber = trackNumber;
                track.Save();
            }
        }

        private readonly static Dictionary<int, string> numMapping_reverse = new()
        {
             { 1, "一" },
             { 2, "二" },
             { 3, "三" },
             { 4, "四" },
             { 5, "五" },
             { 6, "六" },
             { 7, "七" },
             { 8, "八" },
             { 9, "九" },
             { 0, "零" }
        };

        public static string ReplaceNumber(string input)
        {
            string output = input;
            string pattern = ".*[0-9]{1,}.*";
            Regex regex = new(pattern);
            if (regex.IsMatch(input))
            {
                foreach (var pair in numMapping_reverse)
                {
                    output = output.Replace(pair.Key.ToString(), pair.Value);
                }
            }
            return output;
        }

        /// <summary>
        /// 移除非法字符。
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string RemoveInvaildSymbol(string input)
        {
            return input.Replace(":", "").Replace("/", "").Replace("\\", "").Replace("?", "").Replace("<", "").Replace(">", "").Replace("*", "").Replace("|", "").Replace("\"", "").Trim().Trim('.');
        }

        /// <summary>
        /// 格式化并替换字符。
        /// </summary>
        /// <param name="input">要搜索匹配项的字符串。</param>
        /// <returns></returns>
        public static string ReplaceFormat(string input)
        {
            input = input.Replace(" (", "「").Replace("(", "「").Replace(")", "」").Replace("（", "「").Replace("）", "」").Replace(" [", "「").Replace("[", "「").Replace("]", "」");

            string pattern = "([0-9])([\u0800-\ud7ff_a-zA-Z]+)";
            string output = Regex.Replace(input, pattern, "$1 $2");
            output = Regex.Replace(output, "([\u0800-\ud7ff_a-zA-Z]+)([0-9])", "$1 $2");
            output = output.Replace(" (", "「").Replace("(", "「").Replace(")", "」");
            return output;
        }

        static string[] STRINGS = { "CD1", "CD2", "CD3", "CD4", "CD5", "CD6", "CD7", "CD8", "CD9", "CDA", "CDB", "CDC" };
        public static string ReplaceString(string str)
        {
            foreach (var item in STRINGS)
            {
                str = str.Replace(item, "");
            }
            return str;
        }

        /// <summary>
        /// 读取目录所有最底层子目录。
        /// </summary>
        /// <param name="dirPath"></param>
        /// <param name="excludePaths"></param>
        /// <returns></returns>
        public static List<string> GetDirectories(string dirPath, string[] excludePaths = null)
        {
            List<string> directories = new();
            DirectoryInfo directoryInfo = new(dirPath);
            FileInfo[] fileInfos = directoryInfo.GetFiles();    // 文件
            DirectoryInfo[] directoryInfos = directoryInfo.GetDirectories();   // 文件夹
            if (directoryInfos.Count() > 0)
            {
                // 获取子文件夹内的文件列表，递归遍历  
                foreach (DirectoryInfo info in directoryInfos)
                {
                    if (excludePaths == null || !excludePaths.Contains(info.FullName))
                    {
                        directories.AddRange(GetDirectories(info.FullName));
                    }
                }
            }
            else
            {
                directories.Add(dirPath);
            }
            return directories;
        }

        /// <summary>
        /// 读取目录所有文件。
        /// </summary>
        /// <param name="dirPath"></param>
        /// <param name="excludePaths"></param>
        /// <returns></returns>
        public static List<string> GetDirectoryFiles(string dirPath, string[] excludePaths = null)
        {
            List<string> files = new();
            DirectoryInfo directoryInfo = new(dirPath);
            FileInfo[] fileInfos = directoryInfo.GetFiles();    // 文件
            DirectoryInfo[] directoryInfos = directoryInfo.GetDirectories();   // 文件夹
            foreach (FileInfo fileInfo in fileInfos)
            {
                files.Add(fileInfo.FullName);
            }
            // 获取子文件夹内的文件列表，递归遍历  
            foreach (DirectoryInfo info in directoryInfos)
            {
                if (excludePaths == null || !excludePaths.Contains(info.FullName))
                {
                    files.AddRange(GetDirectoryFiles(info.FullName));
                }
            }
            return files;
        }

        /// <summary>
        /// 基于 NAudio 工具对 Wav 音频文件剪切（限 PCM 格式）
        /// </summary>
        /// <param name="inPath">目标文件</param>
        /// <param name="outPath">输出文件</param>
        /// <param name="cutFromStart">开始时间</param>
        /// <param name="cutFromEnd">结束时间</param>
        public static void TrimWavFile(string inPath, string outPath, TimeSpan cutFromStart, TimeSpan cutFromEnd)
        {
            using (WaveFileReader reader = new WaveFileReader(inPath))
            {
                int fileLength = (int)reader.Length; using (WaveFileWriter writer = new WaveFileWriter(outPath, reader.WaveFormat))
                {
                    float bytesPerMillisecond = reader.WaveFormat.AverageBytesPerSecond / 1000f;
                    int startPos = (int)Math.Round(cutFromStart.TotalMilliseconds * bytesPerMillisecond);
                    startPos = startPos - startPos % reader.WaveFormat.BlockAlign;
                    int endPos = (int)Math.Round(cutFromEnd.TotalMilliseconds * bytesPerMillisecond);
                    endPos = endPos - endPos % reader.WaveFormat.BlockAlign;
                    //判断结束位置是否越界
                    endPos = endPos > fileLength ? fileLength : endPos;
                    TrimWavFile(reader, writer, startPos, endPos);
                }
            }
        }

        /// <summary>
        /// 重新合并 wav 文件
        /// </summary>
        /// <param name="reader">读取流</param>
        /// <param name="writer">写入流</param>
        /// <param name="startPos">开始流</param>
        /// <param name="endPos">结束流</param>
        private static void TrimWavFile(WaveFileReader reader, WaveFileWriter writer, int startPos, int endPos)
        {
            reader.Position = startPos;
            byte[] buffer = new byte[1024];
            while (reader.Position < endPos)
            {
                int bytesRequired = (int)(endPos - reader.Position);
                if (bytesRequired > 0)
                {
                    int bytesToRead = Math.Min(bytesRequired, buffer.Length);
                    int bytesRead = reader.Read(buffer, 0, bytesToRead);
                    if (bytesRead > 0)
                    {
                        writer.Write(buffer, 0, bytesRead);
                    }
                }
            }
        }

    }
}
