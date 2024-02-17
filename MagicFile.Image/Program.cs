using System.Drawing;

namespace MagicFile.Image
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string imagePath = "D:\\Images\\";

            DirectoryInfo directoryInfo = new(imagePath);

            FileInfo[] files = directoryInfo.GetFiles();
            foreach (FileInfo file in files)
            {
                SplitImage(file.FullName);
            }

            Console.WriteLine("Hello, World!");
        }


        public static void SplitImage(string imagePath)
        {
            string parentDir = Path.GetDirectoryName(imagePath);
            string destDir = Path.Combine(parentDir, "parts");
            if (!Directory.Exists(destDir))
            {
                Directory.CreateDirectory(destDir);
            }
            using (var image = System.Drawing.Image.FromFile(imagePath))
            {
                int width = image.Width / 2;
                int height = image.Height / 2;

                using (var bitmap1 = new Bitmap(width, height))
                using (var bitmap2 = new Bitmap(width, height))
                using (var bitmap3 = new Bitmap(width, height))
                using (var bitmap4 = new Bitmap(width, height))
                {
                    using (var graphics1 = Graphics.FromImage(bitmap1))
                    using (var graphics2 = Graphics.FromImage(bitmap2))
                    using (var graphics3 = Graphics.FromImage(bitmap3))
                    using (var graphics4 = Graphics.FromImage(bitmap4))
                    {
                        graphics1.DrawImage(image, new Rectangle(0, 0, width, height), new Rectangle(0, 0, width, height), GraphicsUnit.Pixel);
                        graphics2.DrawImage(image, new Rectangle(0, 0, width, height), new Rectangle(width, 0, width, height), GraphicsUnit.Pixel);
                        graphics3.DrawImage(image, new Rectangle(0, 0, width, height), new Rectangle(0, height, width, height), GraphicsUnit.Pixel);
                        graphics4.DrawImage(image, new Rectangle(0, 0, width, height), new Rectangle(width, height, width, height), GraphicsUnit.Pixel);
                    }
                    string extension = Path.GetExtension(imagePath);
                    string filename = Path.GetFileNameWithoutExtension(imagePath);
                    bitmap1.Save(Path.Combine(destDir, filename + "-part1" + extension), System.Drawing.Imaging.ImageFormat.Jpeg);
                    bitmap2.Save(Path.Combine(destDir, filename + "-part2" + extension), System.Drawing.Imaging.ImageFormat.Jpeg);
                    bitmap3.Save(Path.Combine(destDir, filename + "-part3" + extension), System.Drawing.Imaging.ImageFormat.Jpeg);
                    bitmap4.Save(Path.Combine(destDir, filename + "-part4" + extension), System.Drawing.Imaging.ImageFormat.Jpeg);
                }
            }
        }
    }
}