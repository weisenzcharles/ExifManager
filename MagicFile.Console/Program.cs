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

namespace MagicFile
{
    class Program
    {

        private static readonly List<Vector> coordinates = new List<Vector>();
        private static readonly List<DateTime> when = new List<DateTime>();

        private static Dictionary<string, string> metadata = new Dictionary<string, string>();

        private static string GetTimeName(FileInfo fileInfo)
        {
            string suffix = fileInfo.Name.Replace(fileInfo.Extension, "").Substring(fileInfo.Name.Replace(fileInfo.Extension, "").Length - 4);
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
            string suffix = fileInfo.Name.Replace(fileInfo.Extension, "").Substring(fileInfo.Name.Replace(fileInfo.Extension, "").Length - 4);
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

                }
                else if (command == "4")
                {
                    OrganizeMedia();
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
            Console.WriteLine("3、整理视频文件信息");
            Console.WriteLine("4、自动根据元数据整理文件");
            Console.WriteLine("q、退出");
            Console.WriteLine("----------------------------------------------------------------------------");
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

        private static void OrganizeMedia(string path)
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
                if (file.Extension.ToLower().Equals(".mov"))
                {
                    var directories = ImageMetadataReader.ReadMetadata(file.FullName);

                    var headerDirectory = directories.Where(d => d.Name == "QuickTime Movie Header").FirstOrDefault();
                    var tags = headerDirectory.Tags;

                    var dateTime = headerDirectory?.GetDescription(QuickTimeMovieHeaderDirectory.TagCreated);

                    var created = headerDirectory.GetDateTime(QuickTimeMovieHeaderDirectory.TagCreated);
                    var modified = headerDirectory.GetDateTime(QuickTimeMovieHeaderDirectory.TagModified);

                    var filename = string.Empty;
                    if (created >= modified)
                    {
                        filename = modified.ToString("yyyyMMdd_HHmmss_ffff");
                    }
                    else
                    {
                        filename = created.ToString("yyyyMMdd_HHmmss_ffff");
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
                    var directories = ImageMetadataReader.ReadMetadata(file.FullName);

                    var fileDirectory = directories.Where(d => d.Name == "File").FirstOrDefault();
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
