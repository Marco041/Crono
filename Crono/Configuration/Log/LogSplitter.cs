using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crono.Configuration.Log
{
    public class LogSplitter
    {
        private readonly ILog _infoLogger;
        private readonly ILog _errorLogger;

        public LogSplitter(ILog infoLogger, ILog errorLogger)
        {
            _infoLogger = infoLogger;
            _errorLogger = errorLogger;
        }

        public void Debug(Func<string> logFunc)
        {
            if (_infoLogger != null && _infoLogger.IsDebugEnabled) _infoLogger.Debug(logFunc.Invoke());
        }

        public void Info(Func<string> logFunc)
        {
            if (_infoLogger != null && _infoLogger.IsInfoEnabled) _infoLogger.Info(logFunc.Invoke());
        }

        public void Warn(Func<string> logFunc)
        {
            if (_infoLogger != null && _infoLogger.IsWarnEnabled) _infoLogger.Warn(logFunc.Invoke());
        }

        public void Error(string message, Exception err)
        {
            _infoLogger?.Error(ExceptionFormatter.Format(message, err));
            _errorLogger?.Error(message, err);
        }

        public bool IsDebugEnabled => _infoLogger != null && _infoLogger.IsDebugEnabled;
        public bool IsInfoEnabled => _infoLogger != null && _infoLogger.IsInfoEnabled;

        public ILog InfoLogger => _infoLogger;

        public static LogSplitter Null => new LogSplitter(null, null);

        public static LogSplitter GetLogger4Test(string loggerName, string path, string fileName, Level logLevel)
        {
            return new LogSplitter(GetILog($"info_{loggerName}", Path.Combine(path, $"info-{fileName}.log"), logLevel),
                GetILog($"err_{loggerName}", Path.Combine(path, $"err-{fileName}.log"), logLevel));
        }

        private static ILog GetILog(string loggerName, string filename, Level logLevel)
        {
            ILog log = LogManager.GetLogger(loggerName);
            Logger l = (Logger)log.Logger;
            l.Hierarchy.Root.Level = logLevel;
            l.Hierarchy.Configured = true;
            l.AddAppender(CreateFileAppender(loggerName, filename));
            return log;
        }

        private static IAppender CreateFileAppender(string name, string fileName)
        {
            FileAppender appender = new FileAppender
            {
                Name = name,
                File = fileName,
                AppendToFile = true
            };

            PatternLayout layout = new PatternLayout { ConversionPattern = "%date %level %property{Signature} - [%thread] - %logger - %message%newline" };
            layout.ActivateOptions();

            appender.Layout = layout;
            appender.ActivateOptions();

            return appender;
        }
    }
}
