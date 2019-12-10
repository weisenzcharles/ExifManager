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
namespace MagicFile.Test
{
    class Program
    {

        private static readonly List<Vector> coordinates = new List<Vector>();
        private static readonly List<DateTime> when = new List<DateTime>();
        static void Main(string[] args)
        {
            Directory.CreateDirectory("Data/files"); // Need the sub-directory for the Kmz test
            CopyResource("MagicFile.Test.Data.20191114.kml", "Data/20191114.kml");
            CopyResource("MagicFile.Test.Data.20191118_092807_4133.jpg", "Data/20191118_092807_4133.jpg");
            KmlFile file = OpenFile("Enter a file to show the placemarks of:");
            if (file == null)
            {
                return;
            }
            if (file.Root is Kml kml)
            {
                var placemarks = new List<Placemark>();
                ExtractPlacemarks(kml.Feature, placemarks);

                // Sort using their names
                placemarks.Sort((a, b) => string.Compare(a.Name, b.Name));

                // Display the results
                foreach (Placemark placemark in placemarks)
                {
                    Geometry geometry = placemark.Geometry;
                    var track = geometry as Track;
                    coordinates.AddRange(track.Coordinates.ToList());
                    when.AddRange(track.When.ToList());
                }
            }
            var imageFile = ImageFile.FromFile(GetInputFile("", "Data/20191118_092807_4133.jpg"));
            // note the explicit cast to ushort
            imageFile.Properties.Set(ExifTag.ISOSpeedRatings, (ushort)200);

            Console.WriteLine("Hello World!");
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
