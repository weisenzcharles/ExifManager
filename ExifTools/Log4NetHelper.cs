﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Layout;
using log4net.Repository.Hierarchy;


namespace ExifTools
{
    public class Log4NetHelper
    {
        #region 变量定义

        //定义信息的二次处理
        public static event Action<string> OutputMessage;
        //ILog对象
        private static readonly ILog log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 初始化日志
        /// </summary>
        public static void InitLog()
        {
            var domain = "ExifManager";
            var repository = log4net.LogManager.CreateRepository(domain);
            log4net.Config.XmlConfigurator.ConfigureAndWatch(repository, new System.IO.FileInfo("Config//log4net.config"));
        }


        //记录异常日志数据库连接字符串
        private const string _ConnectionString = @"data source=your server;initial catalog=db;integrated security=false;persist security info=True;User ID=sa;Password=sa.";

        //信息模板
        private const string _ConversionPattern = "%n【记录时间】%date%n【描述】%message%n";

        #endregion

        #region 定义信息二次处理方式

        private static void HandMessage(object Msg)
        {
            OutputMessage?.Invoke(Msg.ToString());
        }

        private static void HandMessage(object Msg, Exception ex)
        {
            OutputMessage?.Invoke(string.Format("{0}:{1}", Msg.ToString(), ex.ToString()));
        }

        private static void HandMessage(string format, params object[] args)
        {
            OutputMessage?.Invoke(string.Format(format, args));
        }

        #endregion

        #region 封装Log4net

        public static void Debug(object message)
        {
            HandMessage(message);
            if (log.IsDebugEnabled)
            {
                log.Debug(message);
            }
        }

        public static void Debug(object message, Exception ex)
        {
            HandMessage(message, ex);
            if (log.IsDebugEnabled)
            {
                log.Debug(message, ex);
            }
        }

        public static void DebugFormat(string format, params object[] args)
        {
            HandMessage(format, args);
            if (log.IsDebugEnabled)
            {
                log.DebugFormat(format, args);
            }
        }

        public static void Error(object message)
        {
            HandMessage(message);
            if (log.IsErrorEnabled)
            {
                log.Error(message);
            }
        }

        public static void Error(object message, Exception ex)
        {
            HandMessage(message, ex);
            if (log.IsErrorEnabled)
            {
                log.Error(message, ex);
            }
        }

        public static void ErrorFormat(string format, params object[] args)
        {
            HandMessage(format, args);
            if (log.IsErrorEnabled)
            {
                log.ErrorFormat(format, args);
            }
        }

        public static void Fatal(object message)
        {
            HandMessage(message);
            if (log.IsFatalEnabled)
            {
                log.Fatal(message);
            }
        }

        public static void Fatal(object message, Exception ex)
        {
            HandMessage(message, ex);
            if (log.IsFatalEnabled)
            {
                log.Fatal(message, ex);
            }
        }

        public static void FatalFormat(string format, params object[] args)
        {
            HandMessage(format, args);
            if (log.IsFatalEnabled)
            {
                log.FatalFormat(format, args);
            }
        }

        public static void Info(object message)
        {
            HandMessage(message);
            if (log.IsInfoEnabled)
            {
                log.Info(message);
            }
        }

        public static void Info(object message, Exception ex)
        {
            HandMessage(message, ex);
            if (log.IsInfoEnabled)
            {
                log.Info(message, ex);
            }
        }

        public static void InfoFormat(string format, params object[] args)
        {
            HandMessage(format, args);
            if (log.IsInfoEnabled)
            {
                log.InfoFormat(format, args);
            }
        }

        public static void Warn(object message)
        {
            HandMessage(message);
            if (log.IsWarnEnabled)
            {
                log.Warn(message);
            }
        }

        public static void Warn(object message, Exception ex)
        {
            HandMessage(message, ex);
            if (log.IsWarnEnabled)
            {
                log.Warn(message, ex);
            }
        }

        public static void WarnFormat(string format, params object[] args)
        {
            HandMessage(format, args);
            if (log.IsWarnEnabled)
            {
                log.WarnFormat(format, args);
            }
        }

        #endregion

        #region 手动加载配置

        public static void LoadAppender()
        {
            var domain = "ExifManager";
            var repository = log4net.LogManager.CreateRepository(domain);
            XmlConfigurator.Configure(repository);
        }

        //public static void LoadFileAppender()
        //{
        //    FileAppender appender = new FileAppender();
        //    appender.Name = "FileAppender";
        //    appender.File = "error.log";
        //    appender.AppendToFile = true;

        //    PatternLayout patternLayout = new PatternLayout();
        //    patternLayout.ConversionPattern = _ConversionPattern;
        //    patternLayout.ActivateOptions();
        //    appender.Layout = patternLayout;

        //    //选择UTF8编码，确保中文不乱码。
        //    appender.Encoding = Encoding.UTF8;

        //    appender.ActivateOptions();
        //    BasicConfigurator.Configure(appender);
        //}

        //public static void LoadRollingFileAppender()
        //{
        //    RollingFileAppender appender = new RollingFileAppender();
        //    appender.AppendToFile = true;
        //    appender.Name = "RollingFileAppender";
        //    appender.DatePattern = "yyyy-MM-dd HH'时.log'";
        //    appender.File = "Logs/";
        //    appender.AppendToFile = true;
        //    appender.RollingStyle = RollingFileAppender.RollingMode.Composite;
        //    appender.MaximumFileSize = "500kb";
        //    appender.MaxSizeRollBackups = 10;
        //    appender.StaticLogFileName = false;


        //    PatternLayout patternLayout = new PatternLayout();
        //    patternLayout.ConversionPattern = _ConversionPattern;
        //    patternLayout.ActivateOptions();
        //    appender.Layout = patternLayout;

        //    //选择UTF8编码，确保中文不乱码。
        //    appender.Encoding = Encoding.UTF8;

        //    appender.ActivateOptions();
        //    BasicConfigurator.Configure(appender);
        //}

        //public static void LoadConsoleAppender()
        //{
        //    ConsoleAppender appender = new ConsoleAppender();
        //    appender.Name = "ConsoleAppender";

        //    PatternLayout patternLayout = new PatternLayout();
        //    patternLayout.ConversionPattern = _ConversionPattern;
        //    patternLayout.ActivateOptions();
        //    appender.Layout = patternLayout;

        //    appender.ActivateOptions();
        //    BasicConfigurator.Configure(appender);
        //}

        //public static void LoadTraceAppender()
        //{
        //    TraceAppender appender = new TraceAppender();
        //    appender.Name = "TraceAppender";

        //    PatternLayout patternLayout = new PatternLayout();
        //    patternLayout.ConversionPattern = _ConversionPattern;
        //    patternLayout.ActivateOptions();
        //    appender.Layout = patternLayout;

        //    appender.ActivateOptions();
        //    BasicConfigurator.Configure(appender);
        //}

        //public static void LoadEventLogAppender()
        //{
        //    EventLogAppender appender = new EventLogAppender();
        //    appender.Name = "EventLogAppender";

        //    PatternLayout patternLayout = new PatternLayout();
        //    patternLayout.ConversionPattern = _ConversionPattern;
        //    patternLayout.ActivateOptions();
        //    appender.Layout = patternLayout;

        //    appender.ActivateOptions();
        //    BasicConfigurator.Configure(appender);
        //}

        #endregion

        #region 定义常规应用程序中未处理的异常信息记录方式

        public static void LoadUnhandledException()
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler
                ((sender, e) => { log.Fatal("未处理的异常", e.ExceptionObject as Exception); });
        }

        #endregion
    }
}
