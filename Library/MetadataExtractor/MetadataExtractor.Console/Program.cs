﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Iptc;
using MetadataExtractor.Formats.Jpeg;

namespace MetadataExtractor.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            //const string filePath = @"E:\metadata-extractor-dotnet-2.2.0\MetadataExtractor.Console\Data\withIptcExifGps.jpg";
            const string filePath = @"K:\metadata-extractor-2.2.0\MetadataExtractor.Console\Resources\1.MOV";

            //const string filePath = @"K:\metadata-extractor-2.2.0\MetadataExtractor.Console\Resources\1.JPG";
            System.Console.WriteLine($"Processing file: {filePath}");

            // There are multiple ways to get a Metadata object for a file

            //
            // SCENARIO 1: UNKNOWN FILE TYPE
            //
            // This is the most generic approach.  It will transparently determine the file type and invoke the appropriate
            // readers.  In most cases, this is the most appropriate usage.  This will handle JPEG, TIFF, GIF, BMP and RAW
            // (CRW/CR2/NEF/RW2/ORF) files and extract whatever metadata is available and understood.
            //
            try
            {
                var directories = ImageMetadataReader.ReadMetadata(filePath);

                Print(directories, "Using ImageMetadataReader");
            }
            catch (ImageProcessingException e)
            {
                PrintError(e);
            }
            catch (IOException e)
            {
                PrintError(e);
            }

            //
            // SCENARIO 2: SPECIFIC FILE TYPE
            //
            // If you know the file to be a JPEG, you may invoke the JpegMetadataReader, rather than the generic reader
            // used in approach 1.  Similarly, if you knew the file to be a TIFF/RAW image you might use TiffMetadataReader,
            // PngMetadataReader for PNG files, BmpMetadataReader for BMP files, or GifMetadataReader for GIF files.
            //
            // Using the specific reader offers a very, very slight performance improvement.
            //
            //try
            //{
            //    var directories = JpegMetadataReader.ReadMetadata(filePath);

            //    Print(directories, "Using JpegMetadataReader");
            //}
            //catch (JpegProcessingException e)
            //{
            //    PrintError(e);
            //}
            //catch (IOException e)
            //{
            //    PrintError(e);
            //}

            ////
            //// APPROACH 3: SPECIFIC METADATA TYPE
            ////
            //// If you only wish to read a subset of the supported metadata types, you can do this by
            //// passing the set of readers to use.
            ////
            //// This currently only applies to JPEG file processing.
            ////
            //try
            //{
            //    // Handle only Exif and IPTC from JPEG
            //    var readers = new IJpegSegmentMetadataReader[] { new ExifReader(), new IptcReader() };

            //    var directories = JpegMetadataReader.ReadMetadata(filePath, readers);

            //    Print(directories, "Using JpegMetadataReader for Exif and IPTC only");
            //}
            //catch (JpegProcessingException e)
            //{
            //    PrintError(e);
            //}
            //catch (IOException e)
            //{
            //    PrintError(e);
            //}

            // Write all extracted values to stdout
            static void Print(IEnumerable<Directory> directories, string method)
            {
                System.Console.WriteLine();
                System.Console.WriteLine("-------------------------------------------------");
                System.Console.Write(' '); System.Console.WriteLine(method);
                System.Console.WriteLine("-------------------------------------------------");
                System.Console.WriteLine();

                // Extraction gives us potentially many directories
                foreach (var directory in directories)
                {
                    // Each directory stores values in tags
                    foreach (var tag in directory.Tags)
                        System.Console.WriteLine(tag);

                    // Each directory may also contain error messages
                    foreach (var error in directory.Errors)
                        System.Console.Error.WriteLine("ERROR: " + error);
                }
            }

            static DateTime? GetTakenDateTime(IEnumerable<Directory> directories)
            {
                // obtain the Exif SubIFD directory
                var directory = directories.OfType<ExifSubIfdDirectory>().FirstOrDefault();

                if (directory == null)
                    return null;

                // query the tag's value
                if (directory.TryGetDateTime(ExifDirectoryBase.TagDateTimeOriginal, out var dateTime))
                    return dateTime;

                return null;
            }

            static string? GetExposureProgramDescription(IEnumerable<Directory> directories)
            {
                // obtain a specific directory
                var directory = directories.OfType<ExifSubIfdDirectory>().FirstOrDefault();

                if (directory == null)
                    return null;

                // create a descriptor
                var descriptor = new ExifSubIfdDescriptor(directory);

                // get tag description
                return descriptor.GetExposureProgramDescription();
            }

            static void PrintError(Exception exception) => System.Console.Error.WriteLine($"EXCEPTION: {exception}");
        }
    }
}
