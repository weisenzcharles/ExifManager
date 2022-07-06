using log4net.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExifTools
{
    // 扩展 log4net 的日志输出
    public class LogSocketAppender : log4net.Appender.AppenderSkeleton
    {
        public LogSocketAppender()
        {
            Name = "LogSocketAppender";
        }

        //protected override void Append(LoggingEvent loggingEvent)
        //{
        //    string strLoggingMessage = RenderLoggingEvent(loggingEvent);
        //    byte[] tmpBuffer = Encoding.Default.GetBytes(strLoggingMessage);
        //    lock (Program.AsyncSocketSvr.LogOutputSocketProtocolMgr)
        //    {
        //        for (int i = 0; i < Program.AsyncSocketSvr.LogOutputSocketProtocolMgr.Count(); i++)
        //        {
        //            Program.AsyncSocketSvr.LogOutputSocketProtocolMgr.ElementAt(i).LogFixedBuffer.WriteBuffer(tmpBuffer);
        //            Program.AsyncSocketSvr.LogOutputSocketProtocolMgr.ElementAt(i).InitiativeSend();
        //        }
        //    }
        //}

        protected override void Append(LoggingEvent loggingEvent)
        {
            throw new NotImplementedException();
        }
    }
}
