using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using ATL;
using ATL.AudioData;
namespace ATLTest
{
    class Program
    {
        static void Main(string[] args)
        {
            OrganizeFile();
            //string input = "";
            //string pattern = "([s?])(「+)";
            //string output = Regex.Replace(input, pattern, "$1 $2");
            //output = Regex.Replace(output, "(['\"{}\\(\\)\\[\\]\\*&.?!,…:;]+)([0-9])", "$1 $2");

            //var korea = Encoding.GetEncoding("ks_c_5601-1987");
            //// 匹配数字临近中文
            //Regex regex = new Regex("([0-9])([\u4e00-\u9fa5]+)");
            //Regex regex1 = new Regex("([\u4e00-\u9fa5]+)([0-9])");
            //output = regex.Replace(input, "$1 $2");
            //Console.WriteLine(output);

            //output = regex1.Replace(input, "$1 $2");
            //Console.WriteLine(output);

            //string filename = GetInputFile( "Data/20191114.kml");
            //Track theTrack = new Track("//192.168.0.199/home/Adele - Someone Like You.flac");

            // Works the same way on any supported format (MP3, FLAC, WMA, SPC...)

            //Console.WriteLine(output);
            //theTrack.Composer = "Oscar Wilde (アイドル)"; // Support for "exotic" charsets
            //theTrack.AdditionalFields["customField"] = "fancyValue"; // Support for custom fields
            //
            //Console.ReadLine();
        }

        /// <summary>
        /// 整理文件。
        /// </summary>
        public static void OrganizeFile()
        {

            #region 文件管理...

            Dictionary<string, string> albumsLocal = new Dictionary<string, string>();
            string rootPath = @"\\192.168.0.199\home\Test";
            string artistDirectory = @"\\192.168.0.199\home\Test\music";
            Directory.CreateDirectory(artistDirectory);
            // Need the sub-directory for the Kmz test
            //CopyResource("MagicFile.Test.Data.Akon - Right Now「Na Na Na」.flac", "Data/Akon - Right Now「Na Na Na」.flac");
            //CopyResource("MagicFile.Test.Data.Alex Goot - Catch My Breath.mp3", "Data/Alex Goot - Catch My Breath.mp3");
            var files = Directory.GetFiles(rootPath);


            foreach (string file in files)
            {
                Console.WriteLine("File : " + file);
                Track theTrack = new Track(file);
                if (theTrack != null && !string.IsNullOrEmpty(theTrack.Title))
                {
                    string artistPath = string.Format(@"{0}\{1}", artistDirectory, theTrack.Artist);
                    string albumPath = string.Format(@"{0}\{1}\{2}", artistDirectory, theTrack.Artist, theTrack.Album.Replace(":", " ").Replace("/", " ").Replace("\\", " ").Replace("?", " ").Replace("<", " ").Replace(">", " ").Replace("*", " ").Replace("|", " "));

                    if (!Directory.Exists(artistPath))
                    {
                        Directory.CreateDirectory(artistPath);
                    }


                    string fileName = Path.GetFileNameWithoutExtension(file);
                    if (fileName.Split('-').Length > 0)
                    {
                        string songArtist = fileName.Split('-')[0].Trim();
                        string songTitle = ReplaceFormat(fileName.Split('-')[1].Trim());
                        var artists = songArtist.Split('、');
                        if (artists.Length > 0)
                        {
                            albumPath = string.Format(@"{0}\{1}\{2}", artistDirectory, artists[0], theTrack.Album.Replace(":", " ").Replace("/", " ").Replace("\\", " ").Replace("?", " ").Replace("<", " ").Replace(">", " ").Replace("*", " ").Replace("|", " "));

                            foreach (var artist in artists)
                            {
                                theTrack.Title = ReplaceFormat(theTrack.Title);
                                theTrack.Album = ReplaceFormat(theTrack.Album);
                                theTrack.Artist = artist;
                                theTrack.Save();
                                if (!Directory.Exists(albumPath))
                                {
                                    Directory.CreateDirectory(albumPath);
                                }
                                var destFileName = string.Format("{0}\\{1} - {2}{3}", albumPath, artist, songTitle, Path.GetExtension(file).ToLower());
                                CopyFile(file, destFileName);
                            }
                        }
                        else
                        {
                            if (!Directory.Exists(albumPath))
                            {
                                Directory.CreateDirectory(albumPath);
                            }
                            theTrack.Title = ReplaceFormat(theTrack.Title);
                            theTrack.Album = ReplaceFormat(theTrack.Album);
                            //theTrack.Artist = ReplaceFormat(theTrack.Artist);
                            theTrack.Save();

                            var destFileName = string.Format("{0}\\{1} - {2}{3}", albumPath, songArtist, songTitle, Path.GetExtension(file).ToLower());
                            CopyFile(file, destFileName);
                        }
                    }


                    //string key = string.Format("{0}", theTrack.Album);
                    //if (!albumsLocal.ContainsKey(key))
                    //{


                    //albumsLocal.Add(theTrack.Album, albumPath);
                    //var destFileName = albumPath + "\\" + Path.GetFileNameWithoutExtension(file) + Path.GetExtension(file).ToLower();
                    //CopyFile(file, destFileName);
                    //}
                    //else
                    //{
                    //    string albumPath = key;
                    //    var destFileName = albumPath + "\\" + Path.GetFileNameWithoutExtension(file) + Path.GetExtension(file).ToLower();
                    //    CopyFile(file, destFileName);
                    //}
                    Console.WriteLine("Title : " + theTrack.Title);
                    Console.WriteLine("Album : " + theTrack.Album);
                    Console.WriteLine("Artist : " + theTrack.Artist);
                    Console.WriteLine("Description : " + theTrack.Description);
                    Console.WriteLine("Duration : " + theTrack.DurationMs);

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

        /// <summary>
        /// 移除非法字符。
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string RemoveInvaildSymbol(string input)
        {
            return input.Replace(":", "").Replace("/", "").Replace("\\", "").Replace("?", "").Replace("<", "").Replace(">", "").Replace("*", "").Replace("|", "");
            //string pattern = "([0-9])(['\"{}\\(\\)\\[\\]\\*&.?!,…:;]+)";
            //string output = Regex.Replace(input, pattern, "$1 $2");
            //output = Regex.Replace(output, "(['\"{}\\(\\)\\[\\]\\*&.?!,…:;]+)([0-9])", "$1 $2");
            //return output;
        }

        /// <summary>
        /// 格式化并替换字符。
        /// </summary>
        /// <param name="input">要搜索匹配项的字符串。</param>
        /// <returns></returns>
        public static string ReplaceFormat(string input)
        {

            input = input.Replace(" (", "「").Replace("(", "「").Replace(")", "」");

            string pattern = "([0-9])([\u0800-\ud7ff_a-zA-Z]+)";
            string output = Regex.Replace(input, pattern, "$1 $2");
            output = Regex.Replace(output, "([\u0800-\ud7ff_a-zA-Z]+)([0-9])", "$1 $2");
            output = output.Replace(" (", "「").Replace("(", "「").Replace(")", "」");
            return output;
        }

        /// <summary>
        ///  将现有文件复制到新文件。 允许覆盖同名的文件。
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
                { }
            }

        }
    }
}
