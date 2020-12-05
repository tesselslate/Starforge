using System;
using System.IO;

namespace Starforge.Core {
    public static partial class Starforge {
        public static readonly string StarforgeDirectory;

        public static bool Loaded {
            get;
            private set;
        }

        static Starforge() {
            StarforgeDirectory = Environment.CurrentDirectory;
            Loaded = false;
        }

        [STAThread]
        public static void Main(string[] args) {
            // Set log stream
            FileStream logStream = File.OpenWrite(Path.Combine(
                StarforgeDirectory,
                "log.txt"
            ));

            Logger.SetOutputStream(new StreamWriter(logStream));
        }

        public static void Exit(int code = 0) {
            Logger.Log("Closing Starforge.");
            Logger.Close();

            Environment.Exit(code);
        }
    }
}
