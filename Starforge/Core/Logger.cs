using System;
using System.Diagnostics;
using System.IO;

namespace Starforge.Core {
    public static class Logger {
        public static LogLevel Level;

        public static bool Active { get; private set; }

        public static StreamWriter Writer { get; private set; }

        /// <summary>
        /// Writes a message to the log, at the Info level.
        /// </summary>
        /// <param name="msg">The message to write.</param>
        public static void Log(string msg) {
            if (Active && Level >= LogLevel.Info) {
                Writer.WriteLine($"[{DateTime.Now}] | [Info] {msg}");
                Writer.Flush();
            }
        }

        /// <summary>
        /// Writes a message to the log, with the given log level.
        /// </summary>
        /// <param name="level">The LogLevel to write with.</param>
        /// <param name="msg">The message to write.</param>
        public static void Log(LogLevel level, string msg) {
            if (Active && level >= Level) {
                Writer.WriteLine($"[{DateTime.Now}] | [{level}] {msg}");
                Writer.Flush();
            }
        }

        /// <summary>
        /// Writes the stacktrace of an exception to the log.
        /// </summary>
        /// <param name="e">The exception to write.</param>
        public static void LogException(Exception e) {
            Writer.WriteLine(e.ToString());
            Writer.Flush();
        }

        public static void LogStackTrace() {
            Log(new StackTrace(true).ToString());
        }

        /// <summary>
        /// Creates an error log and displays it to the user.
        /// </summary>
        /// <param name="msg">The error message.</param>
        public static void CreateErrorLog(string msg) {
            string errLogPath = Path.Combine(
                Engine.RootDirectory,
                "crashlog.txt"
            );

            try {
                string currentErrLog = string.Empty;
                if (File.Exists(errLogPath)) {
                    currentErrLog = File.ReadAllText(errLogPath);
                    File.Delete(errLogPath);
                }

                using (FileStream stream = File.OpenWrite(errLogPath)) {
                    using (StreamWriter writer = new StreamWriter(stream)) {
                        writer.WriteLine($"========== Error Log ========== [{DateTime.Now.ToString()}]\nStarforge has encountered a fatal error and is unable to continue executing.\n{msg}\n");
                        writer.Write(currentErrLog);
                        writer.Close();
                    }
                }

                // Open the crash/error log.
                new Process{ 
                    StartInfo = new ProcessStartInfo(errLogPath) {
                        UseShellExecute = true
                    } 
                }.Start();
            } catch (Exception e) {
                LogException(e);
            }
        }

        /// <summary>
        /// Shuts down the logger.
        /// </summary>
        public static void Close() {
            Writer.Flush();
            Writer.Close();

            Active = false;
        }

        /// <summary>
        /// Sets the stream to which the logger writes. This should only be used when
        /// initially launching Starforge.
        /// </summary>
        /// <param name="stream">The stream to use.</param>
        public static void SetOutputStream(StreamWriter stream) {
            Writer = stream;
            Active = true;
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
