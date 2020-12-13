using System;
using System.Diagnostics;
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
            Writer.Flush();
        }

        public static void Log(string msg) {
            if (Level >= LogLevel.Info) {
                Writer.WriteLine($"[{DateTime.Now.ToString()}] | [Info] {msg}");
                Writer.Flush();
            }
        }

        public static void Log(LogLevel level, string msg) {
            if (level >= Level) {
                Writer.WriteLine($"[{DateTime.Now.ToString()}] | [{level.ToString()}] {msg}");
                Writer.Flush();
            }
        }

        public static void OpenLog(string path) {
            if (File.Exists(path)) {
                try {
                    // Attempt to open the file at the specified path.
                    // If it's a txt file (which it should be, if it's a log file),
                    // it will open with the text editor on the user's system.
                    // This way the user can see there was an error and give the necessary
                    // info to get assistance.
                    Process.Start(path);
                }
                catch (Exception e) {
                    LogException(e);
                }
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
