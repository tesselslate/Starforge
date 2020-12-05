using System;
using System.IO;

namespace Starforge.Core {
    public static class Logger {
        public static LogLevel Level;

        public static StreamWriter Writer {
            get;
            private set;
        }

        public static void LogException(Exception e) {
            Writer.WriteLine(e.ToString());
            Writer.Flush(); // In case the application doesn't manage to cleanly shut down,
                            // flush the writer's contents to be sure the error is logged.
        }

        public static void Log(string msg) {
            if(Level >= LogLevel.Info) {
                Writer.WriteLine($"[{DateTime.Now.ToString()}] | [Info] {msg}");
            }
        }

        public static void Log(LogLevel level, string msg) {
            if(level >= Level) {
                Writer.WriteLine($"[{DateTime.Now.ToString()}] | [{level.ToString()}] {msg}");
            }
        }

        public static void Close() {
            Writer.Flush();
            Writer.Close();
        }

        public static void SetOutputStream(StreamWriter stream) {
            Writer = stream;
        }

        static Logger() {
            Level = LogLevel.Info;
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
