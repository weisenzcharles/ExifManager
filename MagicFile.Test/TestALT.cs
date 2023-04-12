using ATL;
using ATL.CatalogDataReaders;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicFile.Test
{
    internal class TestALT
    {
        [Test]
        public void TestWavFile()
        {
            ICatalogDataReader catalogData = CatalogDataReaderFactory.GetInstance().GetCatalogDataReader(@"D:\wav\梅艳芳 - 不快不吐.wav");
            Track theTrack = new(@"D:\wav\梅艳芳 - 不快不吐.wav");


            theTrack.Title = "不快不吐";
            theTrack.Album = "不快不吐";
            theTrack.Artist = "梅艳芳";
            theTrack.Save();
        }
    }
}
