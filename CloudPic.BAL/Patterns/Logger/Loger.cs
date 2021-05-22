using System;
using System.Collections.Generic;
using System.Text;

namespace CloudPic.BAL.Logger
{
    public abstract class Logger
    {
        public enum LogSeverity
        {
            Info,
            Warning,
            Error
        }

        public LogSeverity Severity { get; private set; }
        public Logger NextLogger { get; set; }

        protected Logger(LogSeverity level)
        {
            Severity = level;
        }

        protected Logger(LogSeverity severity, Logger nextLogger) : this(severity)
        {
            NextLogger = nextLogger;
        }

        public void Log(LogSeverity severity, string message)
        {
            WriteLog(severity, message);
            NextLogger?.Log(severity, message);
        }

        protected abstract void WriteLog(LogSeverity severity, string message);
    }
}
