using ATL;
using ATL.CatalogDataReaders;
using MagicFile.Test.Utils;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicFile.Test
{
    internal class NAudioTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestWavFile()
        {

            string filePath = "D:\\wav\\陈慧琳.-.[环球精彩金曲系列30－陈慧琳劲歌集](2001)[WAV].wav";
            string filePath1 = "D:\\wav\\梅艳芳 - 同声一笑.wav";

            //var tfile = TagLib.File.Create(filePath1);
            //var custom = tfile.GetTag(TagLib.TagTypes.Id3v2);
            //var custom2 = (TagLib.Ogg.XiphComment)tfile.GetTag(TagLib.TagTypes.Xiph);

            //custom.Album = "1234";
            ////custom2.SetField("MY_TAG", new string[] { "value1", "value2" });

            //tfile.Save();
            AudioFileReader audioFileReader = new AudioFileReader(filePath1);
            Id3v2Tag id3V2Tag = Id3v2Tag.ReadTag(audioFileReader);
            Track track = new Track(filePath);

            ICatalogDataReader catalogData = CatalogDataReaderFactory.GetInstance().GetCatalogDataReader(@"D:\wav\陈慧琳.-.[环球精彩金曲系列30－陈慧琳劲歌集](2001)[WAV].cue");

            IList<Track> tracks = catalogData.Tracks;

            int currentTimeSpan = 0;
            FileInfo fileInfo = new FileInfo(filePath);
            //获取录音文件时长（秒）
            int fileTime = track.Duration;
            //计算文件需要切割多少等份
            int trackCount = tracks.Count;
            int i = 0;
            while (i < trackCount)
            {
                Track currentTrack = tracks[i];
                string nowTime = Util.GetTimeStamp();//当前时间戳
                //切割后保存的文件绝对地址
                var outputPath = System.IO.Path.Combine(fileInfo?.Directory?.FullName, string.Format("{0} - {1}{2}", currentTrack.Artist, currentTrack.Title, fileInfo.Extension));
                //切割的开始时间
                TimeSpan cutFromStart = TimeSpan.FromSeconds(currentTimeSpan);
                //切割的结束时间
                TimeSpan cutFromEnd = cutFromStart + TimeSpan.FromSeconds(currentTrack.Duration);
                //音频切割
                WavFileUtils.TrimWavFile(filePath, outputPath, cutFromStart, cutFromEnd);
                currentTimeSpan = currentTimeSpan + currentTrack.Duration;

                Track theTrack = new(outputPath);
                theTrack.Title = currentTrack.Title;
                theTrack.Album = currentTrack.Album;
                theTrack.Artist = currentTrack.Artist;
                theTrack.Save();
                i++;
            }
            Assert.Pass();
        }
    }
}
