using SharpKml.Base;
using SharpKml.Dom;
using SharpKml.Dom.GX;
using SharpKml.Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ExifLibrary;
using MetadataExtractor;
using MetadataExtractor.Formats.QuickTime;
using MetadataExtractor.Formats.Exif;
using System.Text.RegularExpressions;

namespace MagicFile.Test
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

            string path = @"\\192.168.0.199\Photo\2023\DJI Album";

            DirectoryInfo directoryInfo = new DirectoryInfo(path);

            var files = directoryInfo.GetFiles();


            foreach (var file in files)
            {
                Regex regex = new Regex(@"(\d{8})_(\d{6})_(\d{4})");
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
                }
                else
                {
                    var mediaFile = TagLib.File.Create(file.FullName);

                    var fileTag = mediaFile.Tag;

                    if (fileTag is TagLib.Image.ImageTag)
                    {
                        destFileName = RenamePhoto(file, mediaFile);
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
                //mediaFile.Dispose();
            }


            //string filepath = @"C:\Users\Charles Zhang\Desktop\001\20180720_082809.mp4";

            //var directories = ImageMetadataReader.ReadMetadata(filepath);

            //foreach (var directory in directories)
            //{
            //    foreach (var tag in directory.Tags)
            //        Console.WriteLine($"[{directory.Name}] {tag.Name} = {tag.Description}");

            //    if (directory.HasError)
            //    {
            //        foreach (var error in directory.Errors)
            //            Console.WriteLine($"ERROR: {error}");
            //    }
            //}

            //System.IO.Directory.CreateDirectory("Data/files"); // Need the sub-directory for the Kmz test
            //CopyResource("MagicFile.Test.Data.20191114.kml", "Data/20191114.kml");
            //CopyResource("MagicFile.Test.Data.20191118_092807_4133.jpg", "Data/20191118_092807_4133.jpg");
            //CopyResource("MagicFile.Test.Data.20191117_085933_6304.jpg", "Data/20191117_085933_6304.jpg");
            //CopyResource("MagicFile.Test.Data.DSC05338.ARW", "Data/DSC05338.ARW");
            //CopyResource("MagicFile.Test.Data.Unbelievable.mp4", "Data/Unbelievable.mp4");
            //CopyResource("MagicFile.Test.Data.TheMan.FLAC", "Data/TheMan.FLAC");
            ////KmlFile file = OpenFile("Enter a file to show the placemarks of:");
            ////if (file == null)
            ////{
            ////    return;
            ////}
            ////if (file.Root is Kml kml)
            ////{
            ////    var placemarks = new List<Placemark>();
            ////    ExtractPlacemarks(kml.Feature, placemarks);

            ////    // Sort using their names
            ////    placemarks.Sort((a, b) => string.Compare(a.Name, b.Name));

            ////    // Display the results
            ////    foreach (Placemark placemark in placemarks)
            ////    {
            ////        Geometry geometry = placemark.Geometry;
            ////        var track = geometry as Track;
            ////        coordinates.AddRange(track.Coordinates.ToList());
            ////        when.AddRange(track.When.ToList());
            ////    }
            ////}

            /////*MetadataTest*/();
            ////var imageFile = ImageFile.FromFile(GetInputFile("", "Data/DSC05338.ARW"));
            ////var latTag = imageFile.Properties.Get<GPSLatitudeLongitude>(ExifTag.GPSLatitude);
            ////var longTag = imageFile.Properties.Get<GPSLatitudeLongitude>(ExifTag.GPSLongitude);
            ////var altTag = imageFile.Properties.Get<GPSLatitudeLongitude>(ExifTag.GPSAltitude);
            ////// note the explicit cast to ushort
            ////imageFile.Properties.Set(ExifTag.ISOSpeedRatings, (ushort)200);
            //////imageFile.Properties.Set(ExifTag.ISOSpeedRatings, (ushort)200);
            //////imageFile.Properties.Set(ExifTag.GPSLongitude, (ushort)200);
            //var tfile = TagLib.File.Create(@"Data/Unbelievable.mp4");
            //string videoTitle = tfile.Tag.Title;
            //System.TimeSpan videoDuration = tfile.Properties.Duration;
            //Console.WriteLine("Video Title: {0}, duration: {1}", videoTitle, videoDuration);


            //var audioFile = TagLib.File.Create(@"E:\The Man.FLAC");
            //string audioTitle = audioFile.Tag.Title;
            //System.TimeSpan audioDuration = audioFile.Properties.Duration;
            //Console.WriteLine("Audio Title: {0}, duration: {1}", audioTitle, audioDuration);
            //audioFile.Tag.Title = "aaaaa";
            //audioFile.Save();
            //audioTitle = audioFile.Tag.Title;

            //Console.WriteLine("Audio Title: {0}, duration: {1}", audioTitle, audioDuration);


            //var rawFile = TagLib.File.Create(@"Data/DSC05338.ARW");
            //string rawTitle = rawFile.Tag.Title;
            //var imgtag = rawFile.Tag as TagLib.Image.CombinedImageTag;
            //DateTime? snapshot = imgtag.DateTime;
            //Console.WriteLine("Raw Title: {0}, snapshot taken on {1}", rawTitle, snapshot);

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
                using (FileStream stream = File.Open(filename, FileMode.Open))
                {
                    file = KmlFile.Load(stream);
                }
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
