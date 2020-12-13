using Starforge.Mod;
using Starforge.Platform;
using System;
using System.IO;

namespace Starforge.Core {
    public partial class Engine {
        public static bool Loaded {
            get;
            private set;
        }

        public static string CelesteDirectory {
            get;
            private set;
        }
        
        public static readonly string StarforgeDirectory;

        public static Engine Instance;

        public static PlatformBase Platform;

        static Engine() {
            StarforgeDirectory = Environment.CurrentDirectory;
            Loaded = false;
        }

        public static void Main(string[] args) {
            if (File.Exists("./log_old.txt")) File.Delete("./log_old.txt");
            if (File.Exists("./log.txt")) File.Move("./log.txt", "./log_old.txt");

            // Set log stream
            FileStream logStream = File.OpenWrite(Path.Combine(
                StarforgeDirectory,
                "log.txt"
            ));

            Logger.SetOutputStream(new StreamWriter(logStream));

            // Set up platform-specific helper
            switch(SDL2.SDL.SDL_GetPlatform()) {
            case "Windows":
                Platform = new PlatformWindows();
                break;
            case "Mac OS X":
                throw new PlatformNotSupportedException("Mac OS is currently unsupported.");
            case "Linux":
                throw new PlatformNotSupportedException("Linux is currently unsupported.");
            default: 
                throw new PlatformNotSupportedException($"Invalid platform: {SDL2.SDL.SDL_GetPlatform()}");
            }

            CelesteDirectory = Platform.GetCelesteDirectory();
            Config.ContentDirectory = Path.Combine(CelesteDirectory, "Content");

            // Load plugins (first stage mod loading)
            Loader.LoadPluginAssemblies();

            using (Engine sf = new Engine()) {
                Logger.Log("Beginning game loop");
                Instance = sf;
                sf.Run();
            }
        }

        public static void Exit(int code = 0, bool exit = true) {
            Logger.Log("Closing Starforge.");
            Logger.Close();

            if (exit) {
                Environment.Exit(code);
            }
        }
    }
}
