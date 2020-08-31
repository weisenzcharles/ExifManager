using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using ATL;
using ATL.AudioData;
namespace ATLTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string rootPath = @"E:\Charles\Codes\MagicFile\ATLTest\bin\Debug\Data";

            System.IO.Directory.CreateDirectory("Data/files"); // Need the sub-directory for the Kmz test
                                                               //CopyResource("MagicFile.Test.Data.Akon - Right Now「Na Na Na」.flac", "Data/Akon - Right Now「Na Na Na」.flac");
                                                               //CopyResource("MagicFile.Test.Data.Alex Goot - Catch My Breath.mp3", "Data/Alex Goot - Catch My Breath.mp3");
            var files = Directory.GetFiles(rootPath);


            foreach (string file in files)
            {
                Track theTrack = new Track(file);
                //theTrack.Title = theTrack.Title + "111";
                //theTrack.Save();
                Console.WriteLine("Title : " + theTrack.Title);
                Console.WriteLine("Album : " + theTrack.Album);
                Console.WriteLine("Artist : " + theTrack.Artist);
                Console.WriteLine("Description : " + theTrack.Description);
                Console.WriteLine("Duration (ms) : " + theTrack.DurationMs);
                //Console.WriteLine();
            }
            //string filename = GetInputFile( "Data/20191114.kml");
            //Track theTrack = new Track("//192.168.0.199/home/Adele - Someone Like You.flac");
          
            // Works the same way on any supported format (MP3, FLAC, WMA, SPC...)
           
            Console.WriteLine("Hello World!");
            //theTrack.Composer = "Oscar Wilde (アイドル)"; // Support for "exotic" charsets
            //theTrack.AdditionalFields["customField"] = "fancyValue"; // Support for custom fields
            //
            Console.ReadLine();
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

            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name)) { using (FileStream file = File.OpenWrite(destination)) { } }

        }
    }
}
