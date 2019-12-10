using System;
using System.Collections.Generic;
using System.Xml.Linq;
using SharpKml.Base;
using SharpKml.Dom;
using SharpKml.Dom.GX;
using SharpKml.Engine;
using System.Globalization;
using System.Linq;



namespace Examples
{
    /// <summary>
    /// Uses a KmlFile to parse a kml file and iterates over the placemarks in the file,
    /// displaying their names in alphabetical order.
    /// </summary>
    public static class SortPlacemarks
    {
        public static void Run()
        {
            KmlFile file = Program.OpenFile("Enter a file to show the placemarks of:");
            if (file == null)
            {
                return;
            }
            //            const string Kml =
            //@"<gx:Track xmlns=""http://www.opengis.net/kml/2.2"" xmlns:gx=""http://www.google.com/kml/ext/2.2"">
            //    <when>2010-05-28T02:02:09Z</when>
            //    <when>2010-05-28T02:02:35Z</when>
            //    <when>2010-05-28T02:02:44Z</when>
            //    <when>2010-05-28T02:02:53Z</when>
            //    <when>2010-05-28T02:02:54Z</when>
            //    <when>2010-05-28T02:02:55Z</when>
            //    <when>2010-05-28T02:02:56Z</when>
            //    <gx:coord>-122.5 37.5 156.5</gx:coord>
            //    <gx:coord>-122.5 37.5 152.5</gx:coord>
            //    <gx:coord>-122.5 37.5 147.5</gx:coord>
            //    <gx:coord>-122.5 37.5 142.5</gx:coord>
            //    <gx:coord>-122.5 37.5 141.5</gx:coord>
            //    <gx:coord>-122.5 37.5 141.5</gx:coord>
            //    <gx:coord>-122.5 37.5 140.5</gx:coord>
            //</gx:Track>";

            //            Track parsed = Parse<Track>(Kml);
            //            parsed.When.Count
            // It's good practice for the root element of the file to be a Kml element
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
                    var coordinates = track.Coordinates.ToList();
                    var when = track.When.ToList();
                    //IEnumerator<DateTime> e = when.GetEnumerator();

                    //while (e.MoveNext())
                    //{
                    //    DateTime c = e.Current;
                    //}
                    //Console.WriteLine();
                    IEnumerable<Element> s = geometry.Children;
                    //var s = geometry.
                    Console.WriteLine(placemark.Name);
                }
            }
        }
        private static T Parse<T>(string kml) where T : Element
        {
            var parser = new Parser();
            parser.ParseString(kml, namespaces: true);
            AssertElementSerializesCorrectly(kml, parser.Root);
            return (T)parser.Root;
        }
        private static void AssertElementSerializesCorrectly(string kml, Element element)
        {
            var serializer = new Serializer();
            serializer.Serialize(element);

            XDocument original = XmlTestHelper.Normalize(XDocument.Parse(kml));
            XDocument serialized = XmlTestHelper.Normalize(XDocument.Parse(serializer.Xml));

            //Assert.That(
            //    XNode.DeepEquals(original, serialized),
            //    Is.True,
            //    () => "Expected:\r\n" + serialized.ToString() + "\r\nTo equal:\r\n" + original.ToString());
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
