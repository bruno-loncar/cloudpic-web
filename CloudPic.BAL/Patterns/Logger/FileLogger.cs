using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace CloudPic.BAL.Logger
{
    public class FileLogger : Logger
    {
        public FileLogger(LogSeverity severity) : base(severity)
        { }

        public FileLogger(LogSeverity severity, Logger nextLogger) : base(severity, nextLogger)
        { }

        protected override void WriteLog(LogSeverity severity, string message)
        {
            if (severity == this.Severity)
            {
                var path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                File.AppendAllLines($"{path}/logs/cp_log.txt", new List<string>() { $"[{DateTime.Now.ToLongTimeString()}] Logger [{severity}]: {message}" });
            }
        }
    }
}
