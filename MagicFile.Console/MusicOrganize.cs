using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using ATL;
using ATL.AudioData;

namespace MagicFile
{
    public enum OrganizeMode
    {
        /// <summary>
        /// 按艺术家。
        /// </summary>
        ByArtist,
        /// <summary>
        /// 按专辑。
        /// </summary>
        ByAlbum,
    }

    public class MusicOrganize
    {


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
        /// 整理文件。
        /// </summary>
        public static void Organize(string path, OrganizeMode mode)
        {

            #region 文件管理...

            Dictionary<string, string> albumsLocal = new();

            string rootPath = string.Format(@"{0}", path);

            string outputDirectory = string.Format(@"{0}\Output", path);
            Directory.CreateDirectory(outputDirectory);

            string backupPath = string.Format(@"{0}\Backup", path);
            Directory.CreateDirectory(backupPath);
            var excludePaths = new List<string>() { outputDirectory, backupPath }.ToArray();
            var files = GetDirectoryFiles(rootPath, excludePaths);

            foreach (string file in files)
            {
                Console.WriteLine("File : " + file);
                if (Path.GetExtension(file).ToLower().Equals(".mp3") || Path.GetExtension(file).ToLower().Equals(".flac"))
                {
                    var lrcFile = string.Format(@"{0}\{1}.lrc", Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file));
                    var existedLrc = File.Exists(lrcFile);
                    Track theTrack = new(file);
                    if (theTrack != null && !string.IsNullOrEmpty(theTrack.Title))
                    {
                        // 艺术家路径
                        string artistPath = string.Format(@"{0}\{1}", outputDirectory, theTrack.Artist.Trim());
                        // 专辑路径
                        string albumPath = string.Empty;
                        albumPath = mode switch
                        {
                            OrganizeMode.ByArtist => string.Format(@"{0}\{1}\{2}", outputDirectory, theTrack.Artist.Trim(), RemoveInvaildSymbol(ReplaceFormat(theTrack.Album.Trim()))),
                            OrganizeMode.ByAlbum => string.Format(@"{0}\{1}", outputDirectory, RemoveInvaildSymbol(ReplaceFormat(theTrack.Album.Trim()))),
                            _ => string.Format(@"{0}\{1}\{2}", outputDirectory, theTrack.Artist.Trim(), RemoveInvaildSymbol(ReplaceFormat(theTrack.Album.Trim()))),
                        };
                        string fileName = Path.GetFileNameWithoutExtension(file);
                        if (fileName.Split('-').Length > 1)
                        {
                            string songArtist = fileName.Split('-')[0].Trim();
                            string songTitle = ReplaceFormat(fileName.Split('-')[1].Trim());
                            var artists = songArtist.Replace(",", "、").Replace("&", "、").Split('、');
                            if (artists.Length > 1)
                            {
                                //albumPath = string.Format(@"{0}\{1}\{2}", outputDirectory, artists[0].Trim(), RemoveInvaildSymbol(ReplaceFormat(theTrack.Album.Trim())));
                                foreach (var artist in artists)
                                {
                                    theTrack.Title = ReplaceFormat(theTrack.Title.Trim());
                                    theTrack.Album = ReplaceFormat(theTrack.Album.Trim());
                                    theTrack.Artist = artist.Trim();
                                    theTrack.Save();
                                    if (!Directory.Exists(albumPath))
                                    {
                                        Directory.CreateDirectory(albumPath);
                                    }
                                    var destFileName = string.Format("{0}\\{1} - {2}{3}", albumPath, artist.Trim(), songTitle, Path.GetExtension(file).ToLower());
                                    Console.WriteLine("New Loaction : " + destFileName);
                                    CopyFile(file, destFileName);
                                    if (existedLrc)
                                    {
                                        var destLrcName = string.Format("{0}\\{1} - {2}{3}", albumPath, artist.Trim(), songTitle, ".lrc");
                                        Console.WriteLine("New Lrc Loaction : " + destLrcName);
                                        CopyFile(lrcFile, destLrcName);
                                    }
                                }
                            }
                            else
                            {
                                if (!Directory.Exists(albumPath))
                                {
                                    Directory.CreateDirectory(albumPath);
                                }
                                theTrack.Title = ReplaceFormat(theTrack.Title.Trim());
                                theTrack.Album = ReplaceFormat(theTrack.Album.Trim());
                                //theTrack.Artist = ReplaceFormat(theTrack.Artist);
                                theTrack.Save();

                                var destFileName = string.Format("{0}\\{1} - {2}{3}", albumPath, songArtist, songTitle, Path.GetExtension(file).ToLower());
                                Console.WriteLine("New Loaction : " + destFileName);
                                CopyFile(file, destFileName);
                                if (existedLrc)
                                {
                                    var destLrcName = string.Format("{0}\\{1} - {2}{3}{4}", albumPath, songArtist, songTitle, songTitle, ".lrc");
                                    Console.WriteLine("New Lrc Loaction : " + destLrcName);
                                    CopyFile(lrcFile, destLrcName);
                                }
                            }
                        }

                    }

                    string newFileName = string.Format("{0}\\{1}{2}", backupPath, Path.GetFileNameWithoutExtension(file), Path.GetExtension(file).ToLower());
                    if (!File.Exists(newFileName))
                    {
                        File.Move(file, newFileName);
                    }
                    else
                    {
                        File.Delete(newFileName);
                    }

                }
            }

            #endregion

            #region 文字格式化...


            Console.OutputEncoding = Encoding.UTF8;

            //string inputEnglish = "12adsdsadf22222343434";
            //string patternEnglish = "([0-9])([a-zA-Z]+)";
            //string outputEnglish = Regex.Replace(inputEnglish, patternEnglish, "$1 $2");
            //outputEnglish = Regex.Replace(outputEnglish, "([a-zA-Z]+)([0-9])", "$1 $2");
            //Console.WriteLine("英文：{0}", outputEnglish);

            //string inputChinese = "12哈哈哈哈哈22222343434";
            //string patternChinese = "([0-9])([\u4e00-\u9fa5]+)";
            //string outputChinese = Regex.Replace(inputChinese, patternChinese, "$1 $2");
            //outputChinese = Regex.Replace(outputChinese, "([\u4e00-\u9fa5]+)([0-9])", "$1 $2");
            //Console.WriteLine("中文：{0}", outputChinese);

            //string inputKorean = "11122아직하지못한말2332434";
            //string patternKorean = "([0-9])([\uac00-\ud7ff]+)";
            //string outputKorean = Regex.Replace(inputKorean, patternKorean, "$1 $2");
            //outputKorean = Regex.Replace(outputKorean, "([\uac00-\ud7ff]+)([0-9])", "$1 $2");
            //Console.WriteLine("韩文：{0}", outputKorean);

            //string inputJapanese = "12と22222343434";
            //string patternJapanese = "([0-9])([\u0800-\u4e00]+)";
            //string outputJapanese = Regex.Replace(inputJapanese, patternJapanese, "$1 $2");
            //outputJapanese = Regex.Replace(outputJapanese, "([\u0800-\u4e00]+)([0-9])", "$1 $2");
            //Console.WriteLine("日文：{0}", outputJapanese);


            //string inputEastAsianLanguages = "16未成年";
            //string patternEastAsianLanguages = "([0-9])([\u0800-\ud7ff_a-zA-Z]+)";
            //string outputEastAsianLanguages = Regex.Replace(inputEastAsianLanguages, patternEastAsianLanguages, "$1 $2");
            //outputEastAsianLanguages = Regex.Replace(outputEastAsianLanguages, "([\u0800-\ud7ff_a-zA-Z]+)([0-9])", "$1 $2");
            //Console.WriteLine("东亚文：{0}", outputEastAsianLanguages);

            #endregion

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
            input = ReplaceNumber(input).Replace(" (", "「").Replace("(", "「").Replace(")", "」").Replace("（", "「").Replace("）", "」").Replace(" [", "「").Replace("[", "「").Replace("]", "」");

            string pattern = "([0-9])([\u0800-\ud7ff_a-zA-Z]+)";
            string output = Regex.Replace(input, pattern, "$1 $2");
            output = Regex.Replace(output, "([\u0800-\ud7ff_a-zA-Z]+)([0-9])", "$1 $2");
            output = output.Replace(" (", "「").Replace("(", "「").Replace(")", "」");
            return output;
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
                    //File.Delete(sourceFileName);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 复制文件夹中的所有内容
        /// </summary>
        /// <param name="sourceDirPath">源文件夹目录</param>
        /// <param name="saveDirPath">指定文件夹目录</param>
        public void CopyDirectory(string sourceDirPath, string saveDirPath)
        {
            try
            {
                if (!Directory.Exists(saveDirPath))
                {
                    Directory.CreateDirectory(saveDirPath);
                }
                string[] files = Directory.GetFiles(sourceDirPath);
                foreach (string file in files)
                {
                    string pFilePath = saveDirPath + "\\" + Path.GetFileName(file);
                    if (File.Exists(pFilePath))
                        continue;
                    File.Copy(file, pFilePath, true);
                }

                string[] dirs = Directory.GetDirectories(sourceDirPath);
                foreach (string dir in dirs)
                {
                    CopyDirectory(dir, saveDirPath + "\\" + Path.GetFileName(dir));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static string GetInputFile(string prompt, string defaultFile)
        {
            Console.WriteLine(prompt);
            //Console.Write("(Leave blank to use " + defaultFile + ")");

            Console.SetCursorPosition(prompt.Length + 1, 0);
            string filename = Console.ReadLine();
            Console.Clear();

            if (string.IsNullOrEmpty(filename))
            {
                return defaultFile;
            }
            return filename;
        }

        private static void CopyResource(string name, string destination)
        {
            var resources = Assembly.GetExecutingAssembly().GetManifestResourceNames();

            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name))
            {
                using (FileStream file = File.OpenWrite(destination))
                {

                }
            }

        }
    }
}
