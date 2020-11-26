using System;
using System.IO;

namespace Starforge.Core {
    public static class Logger {
        public static void LogException(Exception e) {
            Console.WriteLine(e.ToString());
        }

        public static void Log(LogLevel level, string msg) {
            Console.WriteLine(level.ToString() + " | " + msg);
        }
    }

    public enum LogLevel {
        Verbose,
        Debug,
        Info,
        Warning,
        Error,
        Critical
    }
}
