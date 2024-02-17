using SharpKml.Base;
using SharpKml.Dom;
using SharpKml.Dom.GX;
using SharpKml.Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using MetadataExtractor;
using MetadataExtractor.Formats.QuickTime;
using MetadataExtractor.Formats.Exif;
using System.Text.RegularExpressions;
using ATL.CatalogDataReaders.BinaryLogic;
using ATL.CatalogDataReaders;
using Microsoft.International.Converters.TraditionalChineseToSimplifiedConverter;
using System.Diagnostics;
using Newtonsoft.Json;
using MetadataExtractor.Formats.FileSystem;
using System.Drawing;

namespace MagicFile
{
    class Program
    {

        private static readonly List<Vector> coordinates = new List<Vector>();
        private static readonly List<DateTime> when = new List<DateTime>();

        private static Dictionary<string, string> metadata = new Dictionary<string, string>();

        private static string GetTimeName(FileInfo fileInfo)
        {
            string suffix = String.Empty;
            if (fileInfo.Name.Replace(fileInfo.Extension, "").Length > 4)
            {
                suffix = fileInfo.Name.Replace(fileInfo.Extension, "").Substring(fileInfo.Name.Replace(fileInfo.Extension, "").Length - 4);
            }
  
            Regex regex = new Regex(@"(\d{4})");
            if (!regex.IsMatch(suffix))
            {
                suffix = new Random().Next(1000, 9999).ToString();
            }
            string filename;
            if (fileInfo.CreationTime >= fileInfo.LastWriteTime)
            {
                filename = string.Format("{0}_{1}", fileInfo.LastWriteTime.ToString("yyyyMMdd_HHmmss"), suffix);
            }
            else
            {
                filename = string.Format("{0}_{1}", fileInfo.CreationTime.ToString("yyyyMMdd_HHmmss"), suffix);
            }
            return filename;
        }

        /// <summary>
        /// 更名视频。
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string RenameVideo(FileInfo fileInfo, TagLib.File file)
        {
            string filename = GetTimeName(fileInfo);
            var destFileName = string.Format(@"{0}\{1}{2}", fileInfo.DirectoryName, filename, fileInfo.Extension);

            if (File.Exists(destFileName))
            {
                filename = filename.Substring(0, 16) + new Random().Next(1000, 9999);
                destFileName = string.Format(@"{0}\{1}{2}", fileInfo.DirectoryName, filename, fileInfo.Extension);
            }
            return destFileName;
        }

        public static string RenameAudio(FileInfo fileInfo, TagLib.File file)
        {
            string filename = string.Empty;
            var fileTag = file.Tag;
            if (!fileTag.IsEmpty)
            {
                filename = string.Format("{0} - {1}", fileTag.Performers[0], fileTag.Title);
            }

            var destFileName = string.Format(@"{0}\{1}{2}", fileInfo.DirectoryName, filename, fileInfo.Extension);

            return destFileName;
        }

        public static string RenamePhoto(FileInfo fileInfo, TagLib.File mediaFile)
        {
            var imgesTag = mediaFile.Tag as TagLib.Image.ImageTag;
            string suffix = string.Empty;
            if (fileInfo.Name.Replace(fileInfo.Extension, "").Length > 4)
            {
                suffix = fileInfo.Name.Replace(fileInfo.Extension, "").Substring(fileInfo.Name.Replace(fileInfo.Extension, "").Length - 4);
            }

            Regex regex = new Regex(@"(\d{4})");
            if (!regex.IsMatch(suffix))
            {
                suffix = new Random().Next(1000, 9999).ToString();
            }
            string filename;
            if (imgesTag.DateTime.HasValue)
            {
                filename = string.Format("{0}_{1}", imgesTag.DateTime.Value.ToString("yyyyMMdd_HHmmss"), suffix);
            }
            else
            {
                filename = GetTimeName(fileInfo);
            }
            var destFileName = string.Format(@"{0}\{1}{2}", fileInfo.DirectoryName, filename, fileInfo.Extension);
            if (File.Exists(destFileName))
            {
                filename = filename.Substring(0, 16) + new Random().Next(1000, 9999);
                destFileName = string.Format(@"{0}\{1}{2}", fileInfo.DirectoryName, filename, fileInfo.Extension);
            }
            return destFileName;
        }


        /// <summary> 
        /// 更名文本。
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string RenameText(FileInfo fileInfo, TagLib.File file)
        {
            //string filename = GetTimeName(fileInfo);

            return GetTimeName(fileInfo);
        }

        static void Main(string[] args)
        {
            // var sourceDir = @"F:\Music";
            // var destDir = @"F:\Album";
            // AudioTrim.Trim(sourceDir, destDir);
            //Cue cue = new Cue(@"E:\Music\李翊君2006-天荒地老的情歌[喜玛拉雅]{WAV]\李翊君.-.[天荒地老的情歌](2006)[WAV].cue");




            PrintMenu();
            while (true)
            {
                Console.Write("选择你要执行的功能模块：");
                var command = Console.ReadLine();
                Console.WriteLine();

                if (command == "1")
                {

                    Console.WriteLine("请输入一个路径：");
                    string path = Console.ReadLine();
                    if (!string.IsNullOrEmpty(path))
                    {
                        Console.WriteLine("正在执行中......");
                        MusicOrganize.Organize(path, OrganizeMode.ByArtist);
                        Console.WriteLine("执行结束");
                    }

                }
                else if (command == "2")
                {
                    Console.WriteLine("正在执行中......");
                    string path = Console.ReadLine();
                    if (!string.IsNullOrEmpty(path))
                    {
                        Console.WriteLine("请输入一个路径：");
                        MusicOrganize.Organize(path, OrganizeMode.ByAlbum);
                        Console.WriteLine("执行结束");
                    }
                }
                else if (command == "3")
                {

                    Console.WriteLine("请输入一个路径：");
                    string path = Console.ReadLine();
                    if (!string.IsNullOrEmpty(path))
                    {
                        Console.WriteLine("正在执行中......");
                        MusicOrganize.Organize(path, OrganizeMode.ByArtist);
                        Console.WriteLine("执行结束");
                    }

                }
                else if (command == "4")
                {
                    Console.WriteLine("正在执行中......");
                    string path = Console.ReadLine();
                    if (!string.IsNullOrEmpty(path))
                    {
                        Console.WriteLine("请输入一个路径：");
                        MusicOrganize.Organize(path, OrganizeMode.ByAlbum);
                        Console.WriteLine("执行结束");
                    }
                }
                else if (command == "5")
                {
                    OrganizeDirectory();
                }
                else if (command == "6")
                {
                    OrganizeMedia();
                }
                else if (command == "7")
                {
                    OrganizeMedia();
                }
                else if (command == "8")
                {
                    OrganizeMediaInfo();
                }
                else if (command == "9")
                {
                    OrganizeMediaInfo();
                }
                else if (command == "q")
                {
                    Environment.Exit(0);
                }
                PrintMenu();
            }

        }
        static bool Confirm()
        {
            Console.WriteLine("是否确认执行此操作? ");
            Console.WriteLine("确认请输入[Y]，按任意键返回菜单");
            var key = Console.ReadKey();
            Console.WriteLine("");
            if (key.KeyChar.ToString().ToUpper() == "Y")
            {
                return true;
            }
            else
            {
                return false;
            }
        }



        /// <summary>
        /// 打印菜单。
        /// </summary>
        static void PrintMenu()
        {
            Console.WriteLine("----------------------------------------------------------------------------");
            Console.WriteLine("------------------------- MagicFile Console v1.0.0 -------------------------");
            Console.WriteLine("1、根据歌手整理音乐文件信息");
            Console.WriteLine("2、根据专辑整理音乐文件信息");
            Console.WriteLine("3、根据歌手整理音乐文件信息（繁体字转换简体字）");
            Console.WriteLine("4、根据专辑整理音乐文件信息（繁体字转换简体字）");
            //Console.WriteLine("5、整理视频文件信息");
            Console.WriteLine("5、整理 Mac Photo 文件夹");
            Console.WriteLine("6、自动根据元数据整理文件");
            Console.WriteLine("7、延时照片整理");
            Console.WriteLine("8、生成高清影片命名");
            Console.WriteLine("9、图片生成高清影片");
            Console.WriteLine("q、退出");
            Console.WriteLine("----------------------------------------------------------------------------");
        }

        private static void OrganizeHyperlapse()
        {
            Console.WriteLine("请输入一个路径：");
            string path = Console.ReadLine();
            if (!string.IsNullOrEmpty(path))
            {
                OrganizeHyperlapse(path);
            }
        }

        private static void OrganizeHyperlapse(string path)
        {

            DirectoryInfo directoryInfo = new(path);

            var files = directoryInfo.GetFiles();


            foreach (var file in files)
            {
                Regex regex = new(@"(\d{8})_(\d{6})_(\d{4})");
                if (regex.IsMatch(file.Name))
                {
                    continue;
                }

                if (file.Extension.ToLower().Equals(".db"))
                {
                    continue;
                }
                var destFileName = string.Empty;
                if (file.Extension.ToLower().Equals(".dng"))
                {
                    var directories = ImageMetadataReader.ReadMetadata(file.FullName);
                    var headerDirectory = directories?.FirstOrDefault(d => d.Name == "Exif IFD0");
                    var tags = headerDirectory.Tags;
                    DateTime? datetime = null;
                    if (headerDirectory.ContainsTag(ExifDirectoryBase.TagDateTime))
                    {
                        datetime = headerDirectory.GetDateTime(ExifDirectoryBase.TagDateTime);
                    }
                    if (headerDirectory.ContainsTag(ExifDirectoryBase.TagDateTimeDigitized))
                    {
                        datetime = headerDirectory.GetDateTime(ExifDirectoryBase.TagDateTimeDigitized);
                    }
                    if (headerDirectory.ContainsTag(ExifDirectoryBase.TagDateTimeOriginal))
                    {
                        datetime = headerDirectory.GetDateTime(ExifDirectoryBase.TagDateTimeOriginal);
                    }
                    var filename = string.Empty;

                    filename = datetime?.ToString("yyyyMMdd_HHmmss_ffff");

                    destFileName = string.Format(@"{0}\{1}{2}", file.DirectoryName, filename, file.Extension);
                    if (File.Exists(destFileName))
                    {
                        filename = filename.Substring(0, 16) + new Random().Next(1000, 9999);
                        destFileName = string.Format(@"{0}\{1}{2}", file.DirectoryName, filename, file.Extension);
                    }
                }
                else if (file.Extension.ToLower().Equals(".arw"))
                {
                    var directories = ImageMetadataReader.ReadMetadata(file.FullName);
                    var headerDirectory = directories?.FirstOrDefault(d => d.Name == "Exif IFD0");
                    var tags = headerDirectory.Tags;
                    DateTime? datetime = null;
                    if (headerDirectory.ContainsTag(ExifDirectoryBase.TagDateTime))
                    {
                        datetime = headerDirectory.GetDateTime(ExifDirectoryBase.TagDateTime);
                    }
                    if (headerDirectory.ContainsTag(ExifDirectoryBase.TagDateTimeDigitized))
                    {
                        datetime = headerDirectory.GetDateTime(ExifDirectoryBase.TagDateTimeDigitized);
                    }
                    if (headerDirectory.ContainsTag(ExifDirectoryBase.TagDateTimeOriginal))
                    {
                        datetime = headerDirectory.GetDateTime(ExifDirectoryBase.TagDateTimeOriginal);
                    }
                    var filename = string.Empty;

                    filename = datetime?.ToString("yyyyMMdd_HHmmss_ffff");

                    destFileName = string.Format(@"{0}\{1}{2}", file.DirectoryName, filename, file.Extension);
                    if (File.Exists(destFileName))
                    {
                        filename = filename.Substring(0, 16) + new Random().Next(1000, 9999);
                        destFileName = string.Format(@"{0}\{1}{2}", file.DirectoryName, filename, file.Extension);
                    }
                }
                else
                {
                    var mediaFile = TagLib.File.Create(file.FullName);

                    var fileTag = mediaFile.Tag;

                    if (fileTag is TagLib.Image.ImageTag)
                    {
                        destFileName = RenamePhoto(file, mediaFile);
                    }
                    else if (fileTag.TagTypes is TagLib.TagTypes.Apple)
                    {
                        destFileName = RenameVideo(file, mediaFile);
                    }
                    else
                    {
                        var properties = mediaFile.Properties;

                        var mediaTypes = properties.MediaTypes;

                        switch (mediaTypes)
                        {
                            case TagLib.MediaTypes.None:
                                break;
                            case TagLib.MediaTypes.Audio:
                                destFileName = RenameAudio(file, mediaFile);
                                break;
                            case TagLib.MediaTypes.Video:
                                destFileName = RenameVideo(file, mediaFile);
                                break;
                            case TagLib.MediaTypes.Video | TagLib.MediaTypes.Audio:
                                destFileName = RenameVideo(file, mediaFile);
                                break;
                            case TagLib.MediaTypes.Photo:
                                destFileName = RenamePhoto(file, mediaFile);
                                break;
                            case TagLib.MediaTypes.Text:
                                destFileName = RenameText(file, mediaFile);
                                break;
                            default:
                                break;
                        }
                    }
                }
                file.MoveTo(destFileName);
            }
            Console.WriteLine("整理完毕！");
        }

        private static void OrganizeMedia()
        {
            Console.WriteLine("请输入一个路径：");
            string path = Console.ReadLine();
            if (!string.IsNullOrEmpty(path))
            {
                OrganizeMedia(path);
            }
        }
        private static void OrganizeMediaInfo()
        {
            Console.WriteLine("请输入一个路径：");
            string path = Console.ReadLine();
            if (!string.IsNullOrEmpty(path))
            {
                OrganizeMediaInfo(path);
            }
        }

        /// <summary>
        /// 整理相册目录。
        /// </summary>
        private static void OrganizeDirectory()
        {
            Console.WriteLine("请输入一个路径：");
            string path = Console.ReadLine();
            if (!string.IsNullOrEmpty(path))
            {
                OrganizeDirectory(path);
            }
        }

        private static void OrganizeMediaInfo(string filePath)
        {
            // 要读取信息的视频文件路径
            //string filePath = "C:/Movies/Example.mp4";
            // 创建一个新的进程来运行 MediaInfo 命令行工具
            Process process = new Process();
            process.StartInfo.FileName = "E:\\Source\\MagicFile\\MagicFile.Console\\bin\\Debug\\net6.0\\MediaInfo\\MediaInfo.exe"; // MediaInfo 命令行工具的路径
            process.StartInfo.Arguments = " --Output=JSON \"" + filePath + "\""; // 要获取的信息类型和文件路径
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;

            // 启动进程并等待它完成
            process.Start();
            process.WaitForExit();

            // 从输出中解析有关视频文件的信息
            string output = process.StandardOutput.ReadToEnd();
            dynamic json = JsonConvert.DeserializeObject(output);
            string format = json.media.track[0].Format;
            int width = json.media.track[1].Width;
            int height = json.media.track[1].Height;
            string videoCodec = json.media.track[1].Format;
            string audioCodec = json.media.track[2].Format;
            int channels = json.media.track[2].Channels;
            int? year = json.media.track[0]?.Encoded_Date?.Year;

            // 获取文件名和扩展名
            string fileName = System.IO.Path.GetFileNameWithoutExtension(filePath);
            string extension = System.IO.Path.GetExtension(filePath);

            // 根据文件信息生成电影文件名
            string movieName = string.Format("{0}.{1}.{2}.{3}.{4}p.{5}.{6}.{7}-{8}",
                fileName, year, format, extension.Substring(1), height, videoCodec, audioCodec, channels, "制作组名称");

            // 输出生成的电影文件名
            Console.WriteLine(movieName);
        }

        /// <summary>
        /// 整理相册目录。
        /// </summary>
        /// <param name="path"></param>
        private static void OrganizeDirectory(string path)
        {
            DirectoryInfo directoryInfo = new(path);

            var directories = directoryInfo.GetDirectories();

            // 遍历目录
            foreach (var directory in directories)
            {
                string pattern = @"^\d{4}-\d{2}-\d{2}\s.+$";
                Regex regex = new(pattern);
                if (regex.IsMatch(directory.Name))
                {
                    continue;
                }

                // 定义正则表达式
                string patternDate = @"^(.+?),\s(\d{4}年\d{1,2}月\d{1,2}日)$|^(\d{4}年\d{1,2}月\d{1,2}日)$";
                //string patternDate = @"^.+,\s\d{4}年\d{1,2}月\d{1,2}日$";
                //string patternDate = @"^(.+?),\s(\d{4}年\d{1,2}月\d{1,2}日)$|^(\d{4}年\d{1,2}月\d{1,2}日)$";

                // 创建正则表达式对象
                Regex regexDate = new Regex(patternDate);

                // 进行匹配
                Match match = regexDate.Match(directory.Name);

                if (match.Success)
                {
                    string location = match.Groups[1].Value.Trim();
                    string date = String.Empty;
                    string newFolderName = String.Empty;
                    if (String.IsNullOrEmpty(location))
                    {
                        date = match.Groups[0].Value.Trim();
                    }
                    else
                    {
                        date = match.Groups[2].Value.Trim();
                    }

                    // 将中文日期字符串转换成 DateTime 对象
                    DateTime dateTime = DateTime.ParseExact(date, "yyyy年M月d日", null);

                    // 将 DateTime 对象转换成 "yyyy-MM-dd" 格式的字符串
                    string formattedDate = dateTime.ToString("yyyy-MM-dd");

                    //Console.WriteLine($"原始日期：{date}");
                    //Console.WriteLine($"转换后日期：{formattedDate}");
                    //Console.WriteLine($"匹配成功！\n地点：{location}\n日期：{date}");

                    // 构建新的文件夹名字
                    if (String.IsNullOrEmpty(location))
                    {
                        newFolderName = $"{formattedDate}";
                    }
                    else
                    {
                        newFolderName = $"{formattedDate} {location}";
                    }

                    string newFolderPath = Path.Combine(Path.GetDirectoryName(directory.FullName), newFolderName);

                    // 检查新的文件夹路径是否存在，如果不存在则重命名
                    if (!System.IO.Directory.Exists(newFolderPath))
                    {
                        System.IO.Directory.Move(directory.FullName, newFolderPath);
                        Console.WriteLine($"文件夹更名成功！新文件夹名称：{newFolderName}");
                    }
                    else
                    {
                        Console.WriteLine("目标文件夹已存在，无法更名。");
                    }
                }
                else
                {
                    Console.WriteLine("匹配失败！");
                }

                //Console.WriteLine(directory.Name);
            }
        }

        private static void OrganizeMedia(string path)
        {

            DirectoryInfo directoryInfo = new(path);
            var directories = directoryInfo.GetDirectories();
            // 遍历目录
            foreach (var directory in directories)
            {
                var files = directory.GetFiles();

                foreach (var file in files)
                {
                    Regex regex = new(@"(\d{8})_(\d{6})_(\d{4})$");
                    if (regex.IsMatch(file.Name))
                    {
                        continue;
                    }

                    if (file.Extension.ToLower().Equals(".db") || file.Extension.ToLower().Equals(".DS_Store"))
                    {
                        continue;
                    }
                    var destFileName = string.Empty;

                    if (file.Extension.ToLower().Equals(".mov"))
                    {
                        var metaDirectories = ImageMetadataReader.ReadMetadata(file.FullName);

                        var filename = string.Empty;

                        var headerDirectory = metaDirectories.Where(d => d.Name == "QuickTime Movie Header").FirstOrDefault();

                        if (headerDirectory != null)
                        {

                            var tags = headerDirectory.Tags;

                            var dateTime = headerDirectory?.GetDescription(QuickTimeMovieHeaderDirectory.TagCreated);

                            var created = headerDirectory.GetDateTime(QuickTimeMovieHeaderDirectory.TagCreated);
                            var modified = headerDirectory.GetDateTime(QuickTimeMovieHeaderDirectory.TagModified);

                            if (created >= modified)
                            {
                                filename = modified.ToString("yyyyMMdd_HHmmss_ffff");
                            }
                            else
                            {
                                filename = created.ToString("yyyyMMdd_HHmmss_ffff");
                            }
                        }
                        else
                        {
                            headerDirectory = metaDirectories.Where(d => d.Name == "File").FirstOrDefault();
                            var modified = headerDirectory.GetDateTime(FileMetadataDirectory.TagFileModifiedDate);
                            filename = modified.ToString("yyyyMMdd_HHmmss_ffff");
                        }
                        destFileName = string.Format(@"{0}\{1}{2}", file.DirectoryName, filename, file.Extension);
                        if (File.Exists(destFileName))
                        {
                            filename = filename.Substring(0, 16) + new Random().Next(1000, 9999);
                            destFileName = string.Format(@"{0}\{1}{2}", file.DirectoryName, filename, file.Extension);
                        }

                    }
                    else if (file.Extension.ToLower().Equals(".heic"))
                    {
                        var metaDirectories = ImageMetadataReader.ReadMetadata(file.FullName);

                        var fileDirectory = metaDirectories.Where(d => d.Name == "File").FirstOrDefault();
                        var tags = fileDirectory.Tags;

                        var dateTime = fileDirectory?.GetDateTime(QuickTimeMovieHeaderDirectory.TagCreated);

                        var created = fileDirectory?.GetDateTime(QuickTimeMovieHeaderDirectory.TagCreated);
                        var modified = fileDirectory.ContainsTag(QuickTimeMovieHeaderDirectory.TagModified) ? fileDirectory?.GetDateTime(QuickTimeMovieHeaderDirectory.TagModified) : dateTime;

                        var filename = string.Empty;
                        if (created >= modified)
                        {
                            filename = modified?.ToString("yyyyMMdd_HHmmss_ffff");
                        }
                        else
                        {
                            filename = created?.ToString("yyyyMMdd_HHmmss_ffff");
                        }
                        destFileName = string.Format(@"{0}\{1}{2}", file.DirectoryName, filename, file.Extension);
                        if (File.Exists(destFileName))
                        {
                            filename = filename.Substring(0, 16) + new Random().Next(1000, 9999);
                            destFileName = string.Format(@"{0}\{1}{2}", file.DirectoryName, filename, file.Extension);
                        }
                    }
                    else if (file.Extension.ToLower().Equals(".dng"))
                    {
                        var metaDirectories = ImageMetadataReader.ReadMetadata(file.FullName);
                        var headerDirectory = metaDirectories?.FirstOrDefault(d => d.Name == "Exif IFD0");
                        var tags = headerDirectory.Tags;
                        DateTime? datetime = null;
                        if (headerDirectory.ContainsTag(ExifDirectoryBase.TagDateTime))
                        {
                            datetime = headerDirectory.GetDateTime(ExifDirectoryBase.TagDateTime);
                        }
                        if (headerDirectory.ContainsTag(ExifDirectoryBase.TagDateTimeDigitized))
                        {
                            datetime = headerDirectory.GetDateTime(ExifDirectoryBase.TagDateTimeDigitized);
                        }
                        if (headerDirectory.ContainsTag(ExifDirectoryBase.TagDateTimeOriginal))
                        {
                            datetime = headerDirectory.GetDateTime(ExifDirectoryBase.TagDateTimeOriginal);
                        }
                        var filename = string.Empty;

                        filename = datetime?.ToString("yyyyMMdd_HHmmss_ffff");

                        destFileName = string.Format(@"{0}\{1}{2}", file.DirectoryName, filename, file.Extension);
                        if (File.Exists(destFileName))
                        {
                            filename = filename.Substring(0, 16) + new Random().Next(1000, 9999);
                            destFileName = string.Format(@"{0}\{1}{2}", file.DirectoryName, filename, file.Extension);
                        }
                    }
                    else
                    {
                        try
                        {
                            var mediaFile = TagLib.File.Create(file.FullName);

                            var fileTag = mediaFile.Tag;

                            if (fileTag is TagLib.Image.ImageTag)
                            {
                                destFileName = RenamePhoto(file, mediaFile);
                            }
                            else if (fileTag.TagTypes is TagLib.TagTypes.Apple)
                            {
                                destFileName = RenameVideo(file, mediaFile);
                            }
                            else
                            {
                                var properties = mediaFile.Properties;

                                var mediaTypes = properties.MediaTypes;

                                switch (mediaTypes)
                                {
                                    case TagLib.MediaTypes.None:
                                        destFileName = RenameText(file, mediaFile);
                                        break;
                                    case TagLib.MediaTypes.Audio:
                                        destFileName = RenameAudio(file, mediaFile);
                                        break;
                                    case TagLib.MediaTypes.Video:
                                        destFileName = RenameVideo(file, mediaFile);
                                        break;
                                    case TagLib.MediaTypes.Video | TagLib.MediaTypes.Audio:
                                        destFileName = RenameVideo(file, mediaFile);
                                        break;
                                    case TagLib.MediaTypes.Photo:
                                        destFileName = RenamePhoto(file, mediaFile);
                                        break;
                                    case TagLib.MediaTypes.Text:
                                        destFileName = RenameText(file, mediaFile);
                                        break;
                                    default:
                                        destFileName = RenameText(file, mediaFile);
                                        break;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            destFileName = GetTimeName(file);
                            //throw;
                        }
                    }
                    file.MoveTo(destFileName);
                }
            }
            Console.WriteLine("整理完毕！");
        }

        private static void MetadataTest()
        {
            IEnumerable<MetadataExtractor.Directory> directories = ImageMetadataReader.ReadMetadata("Data/DSC05338.ARW");
            foreach (var directory in directories)
                foreach (var tag in directory.Tags)
                    Console.WriteLine($"{directory.Name} - {tag.Name} = {tag.Description}");
        }


        public static KmlFile OpenFile(string prompt)
        {
            string filename = GetInputFile(prompt, "Data/20191114.kml");

            KmlFile file;
            try
            {
                using FileStream stream = File.Open(filename, FileMode.Open);
                file = KmlFile.Load(stream);
            }
            catch (Exception ex)
            {
                DisplayError(ex.GetType() + "\n" + ex.Message);
                return null;
            }

            if (file.Root == null)
            {
                DisplayError("Unable to find any recognized Kml in the specified file.");
                return null;
            }
            return file;
        }

        private static void CopyResource(string name, string destination)
        {
            var resources = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name);
            using FileStream file = File.OpenWrite(destination);
            stream.CopyTo(file);
        }

        public static string GetInputFile(string prompt, string defaultFile)
        {
            Console.WriteLine(prompt);
            Console.Write("(Leave blank to use " + defaultFile + ")");

            Console.SetCursorPosition(prompt.Length + 1, 0);
            string filename = Console.ReadLine();
            Console.Clear();

            if (string.IsNullOrEmpty(filename))
            {
                return defaultFile;
            }
            return filename;
        }

        public static void DisplayError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
        }

        private static void ExtractPlacemarks(Feature feature, List<Placemark> placemarks)
        {
            // Is the passed in value a Placemark?
            if (feature is Placemark placemark)
            {
                placemarks.Add(placemark);
            }
            else
            {
                // Is it a Container, as the Container might have a child Placemark?
                if (feature is Container container)
                {
                    // Check each Feature to see if it's a Placemark or another Container
                    foreach (Feature f in container.Features)
                    {
                        ExtractPlacemarks(f, placemarks);
                    }
                }
            }
        }


    }
}
