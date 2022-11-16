using MagicFile.Test.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicFile.Test
{
    internal class TestFile
    {
        [Test]
        public void ConvertCode() {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            string path = @"d:\\wav\\唐诗三百首CD1.wav.cue";
            Encoding fromEncoding = Encoding.GetEncoding("GB18030");
            Encoding encoding = FileEncoding.GetType(path);
   
            string content = File.ReadAllText(path, fromEncoding);
            File.WriteAllText(path, content, Encoding.UTF8);
        }

        public string GB2312ToUtf8(string gb2312String)
        {
            Encoding fromEncoding = Encoding.GetEncoding("gb2312");
            Encoding toEncoding = Encoding.UTF8;
            return EncodingConvert(gb2312String, fromEncoding, toEncoding);
        }

        public string Utf8ToGB2312(string utf8String)
        {
            Encoding fromEncoding = Encoding.UTF8;
            Encoding toEncoding = Encoding.GetEncoding("gb2312");
            return EncodingConvert(utf8String, fromEncoding, toEncoding);
        }

        public string EncodingConvert(string fromString, Encoding fromEncoding, Encoding toEncoding)
        {
            byte[] fromBytes = fromEncoding.GetBytes(fromString);
            byte[] toBytes = Encoding.Convert(fromEncoding, toEncoding, fromBytes);

            string toString = toEncoding.GetString(toBytes);
            return toString;
        }
    }
}
