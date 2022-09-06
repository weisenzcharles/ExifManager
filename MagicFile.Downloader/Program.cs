namespace MagicFile.Downloader
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var url = "https://apps.weixinqqq.com/musics/I_Wanna_Go-%E5%B8%83%E5%85%B0%E5%A6%AE%E6%96%AF%E7%9A%AE%E5%B0%94%E6%96%AF-1180363.flac";
            var file = HttpDownloader.Download(url, "D:\\downloads", "2211221213231.flac");
            Console.WriteLine("Hello, World!");
        }
    }
}