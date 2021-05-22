using System;
using System.Collections.Generic;
using System.Text;

namespace CloudPic.BAL.Logger
{
    public class ConsoleLogger : Logger
    {
        public ConsoleLogger(LogSeverity severity) : base(severity)
        { }

        public ConsoleLogger(LogSeverity severity, Logger nextLogger) : base(severity, nextLogger)
        { }

        protected override void WriteLog(LogSeverity severity, string message)
        {
            if (severity == this.Severity)
                Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] Logger [{severity}]: {message}");
        }
    }
}
