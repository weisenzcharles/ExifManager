using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagLib;
using TagLib.Id3v2;

namespace MagicFile.Test
{
    public class TestTagLib
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestWavFile()
        {

            //string filePath = "D:\\wav\\陈慧琳.-.[环球精彩金曲系列30－陈慧琳劲歌集](2001)[WAV].wav";
            string filePath = "D:\\wav\\陈慧琳 - 不得了.wav";

            var file = TagLib.File.Create(filePath);
            //var custom = file.GetTag(TagLib.TagTypes.Id3v2);
            file.Tag.Album = "1234";
            file.Tag.Performers = new string[] { "hahaha", "hehehe" };
            //var custom2 = (TagLib.Ogg.XiphComment)tfile.GetTag(TagLib.TagTypes.Xiph);

            //custom.Album = "1234";
            //custom2.SetField("MY_TAG", new string[] { "value1", "value2" });

            TagLib.File f = TagLib.File.Create(filePath);
            TagLib.Id3v2.Tag t = (TagLib.Id3v2.Tag)f.GetTag(TagTypes.Id3v2);
            PrivateFrame p = PrivateFrame.Get(t, "albumtype", true);
            p.PrivateData = System.Text.Encoding.Unicode.GetBytes("TAG CHANGED");
            f.Tag.Album = "test";
            f.Save();

            file.Save();

            Assert.Pass();
        }
    }
}
