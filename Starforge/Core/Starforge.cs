using System;
using System.IO;

namespace Starforge.Core {
    public partial class Starforge {
        public static readonly string StarforgeDirectory;

        public static bool Loaded {
            get;
            private set;
        }

        public static Starforge Instance;

        static Starforge() {
            StarforgeDirectory = Environment.CurrentDirectory;
            Loaded = false;
        }

        public static void Main(string[] args) {
            // Set log stream
            FileStream logStream = File.OpenWrite(Path.Combine(
                StarforgeDirectory,
                "log.txt"
            ));

            Logger.SetOutputStream(new StreamWriter(logStream));

            using(Starforge sf = new Starforge()) {
                Instance = sf;
                sf.Run();
            }
        }

        public static void Exit(int code = 0) {
            Logger.Log("Closing Starforge.");
            Logger.Close();

            Environment.Exit(code);
        }
    }
}
