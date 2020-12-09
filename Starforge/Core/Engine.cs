using Starforge.Mod;
using System;
using System.IO;

namespace Starforge.Core {
    public partial class Engine {
        public static readonly string StarforgeDirectory;

        public static bool Loaded {
            get;
            private set;
        }

        public static Engine Instance;

        static Engine() {
            StarforgeDirectory = Environment.CurrentDirectory;
            Loaded = false;
        }

        public static void Main(string[] args) {
            if(File.Exists("./log_old.txt")) File.Delete("./log_old.txt");
            File.Move("./log.txt", "./log_old.txt");

            // Set log stream
            FileStream logStream = File.OpenWrite(Path.Combine(
                StarforgeDirectory,
                "log.txt"
            ));

            Logger.SetOutputStream(new StreamWriter(logStream));

            // Load plugins (first stage mod loading)
            Loader.LoadPluginAssemblies();

            using(Engine sf = new Engine()) {
                Logger.Log("Beginning game loop");
                Instance = sf;
                sf.Run();
            }
        }

        public static void Exit(int code = 0, bool exit = true) {
            Logger.Log("Closing Starforge.");
            Logger.Close();

            if(exit) {
                Environment.Exit(code);
            }
        }
    }
}
