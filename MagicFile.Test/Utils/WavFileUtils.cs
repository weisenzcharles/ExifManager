using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace MagicFile.Test.Utils
{
    public static class WavFileUtils
    {
        /// <summary>
        /// 基于 NAudio 工具对 Wav 音频文件剪切（限 PCM 格式）
        /// </summary>
        /// <param name="inPath">目标文件</param>
        /// <param name="outPath">输出文件</param>
        /// <param name="cutFromStart">开始时间</param>
        /// <param name="cutFromEnd">结束时间</param>
        public static void TrimWavFile(string inPath, string outPath, TimeSpan cutFromStart, TimeSpan cutFromEnd)
        {
            using (WaveFileReader reader = new WaveFileReader(inPath))
            {
                int fileLength = (int)reader.Length; using (WaveFileWriter writer = new WaveFileWriter(outPath, reader.WaveFormat))
                {
                    float bytesPerMillisecond = reader.WaveFormat.AverageBytesPerSecond / 1000f;
                    int startPos = (int)Math.Round(cutFromStart.TotalMilliseconds * bytesPerMillisecond);
                    startPos = startPos - startPos % reader.WaveFormat.BlockAlign;
                    int endPos = (int)Math.Round(cutFromEnd.TotalMilliseconds * bytesPerMillisecond);
                    endPos = endPos - endPos % reader.WaveFormat.BlockAlign;
                    //判断结束位置是否越界
                    endPos = endPos > fileLength ? fileLength : endPos;
                    TrimWavFile(reader, writer, startPos, endPos);
                }
            }
        }

        /// <summary>
        /// 重新合并 wav 文件
        /// </summary>
        /// <param name="reader">读取流</param>
        /// <param name="writer">写入流</param>
        /// <param name="startPos">开始流</param>
        /// <param name="endPos">结束流</param>
        private static void TrimWavFile(WaveFileReader reader, WaveFileWriter writer, int startPos, int endPos)
        {
            reader.Position = startPos;
            byte[] buffer = new byte[1024];
            while (reader.Position < endPos)
            {
                int bytesRequired = (int)(endPos - reader.Position);
                if (bytesRequired > 0)
                {
                    int bytesToRead = Math.Min(bytesRequired, buffer.Length);
                    int bytesRead = reader.Read(buffer, 0, bytesToRead);
                    if (bytesRead > 0)
                    {
                        writer.Write(buffer, 0, bytesRead);
                    }
                }
            }
        }

        /// <summary>
        /// 基于 NAudio 工具对 MP3 音频文件剪切指定片段（补充）
        /// </summary>
        /// <param name="inputPath">目标文件路径</param>
        /// <param name="outputPath">输出文件路径</param>
        /// <param name="begin">开始时间</param>
        /// <param name="end">结束时间</param>
        public static void TrimMp3File(string inputPath, string outputPath, TimeSpan? begin, TimeSpan? end)
        {
            if (begin.HasValue && end.HasValue && begin > end)
                throw new ArgumentOutOfRangeException("end", "end should be greater than begin");

            using (var reader = new Mp3FileReader(inputPath))
            using (var writer = File.Create(outputPath))
            {
                Mp3Frame frame;
                while ((frame = reader.ReadNextFrame()) != null)
                    if (reader.CurrentTime >= begin || !begin.HasValue)
                    {
                        if (reader.CurrentTime <= end || !end.HasValue)
                            writer.Write(frame.RawData, 0, frame.RawData.Length);
                        else
                            break;
                    }
            }
        }
    }
}
