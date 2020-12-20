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

        /// <summary>
        /// Writes the stacktrace of an exception to the log.
        /// </summary>
        /// <param name="e">The exception to write.</param>
        public static void LogException(Exception e) {
            Writer.WriteLine(e.ToString());
            Writer.Flush();
        }

        /// <summary>
        /// Writes a message to the log, at the Info level.
        /// </summary>
        /// <param name="msg">The message to write.</param>
        public static void Log(string msg) {
            if (Level >= LogLevel.Info) {
                Writer.WriteLine($"[{DateTime.Now.ToString()}] | [Info] {msg}");
                Writer.Flush();
            }
        }

        /// <summary>
        /// Writes a message to the log, with the given log level.
        /// </summary>
        /// <param name="level">The LogLevel to write with.</param>
        /// <param name="msg">The message to write.</param>
        public static void Log(LogLevel level, string msg) {
            if (level >= Level) {
                Writer.WriteLine($"[{DateTime.Now.ToString()}] | [{level.ToString()}] {msg}");
                Writer.Flush();
            }
        }

        /// <summary>
        /// Opens a text file with the specified path, usually to display an error to the user.
        /// </summary>
        /// <param name="path">The path of the text file to open.</param>
        public static void OpenLog(string path) {
            if (File.Exists(path) && Path.GetExtension(path) == ".txt") {
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

        /// <summary>
        /// Shuts down the logger.
        /// </summary>
        public static void Close() {
            Writer.Flush();
            Writer.Close();
        }

        /// <summary>
        /// Sets the stream to which the logger writes. This should only be used when
        /// initially launching Starforge.
        /// </summary>
        /// <param name="stream">The stream to use.</param>
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
