using Eto.Drawing;
using Eto.Forms;
using Starforge.UI;
using Starforge.UI.Main;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Starforge.Core {
    public static partial class Starforge {
        public static readonly string StarforgeDirectory;

        public static Application App {
            get;
            private set;
        }

        public static bool GUIReady {
            get;
            private set;
        }

        public static bool Loaded {
            get;
            private set;
        }

        public static MainWindow Window {
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

            // Global exception handler
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(AppDomain_HandleException);

            // Initialize Eto
            App = new Application();

            // Initialize Eto event handlers
            App.UnhandledException += Eto_HandleException;
            App.Terminating += Eto_Terminating;

            // Start GUI
            Window = new MainWindow();
            App.Run(Window);

            // The Exit function is called after the GUI thread is terminated.
            // Contains cleanup for the main thread.
            Exit();
        }

        public static void Exit(int code = 0) {
            Logger.Log("Closing Starforge.");
            Logger.Close();

            Environment.Exit(code);
        }

        public static void Load() {
            Logger.Log("Loading Starforge..");

            // Switch the "main window" from displaying the splash screen to the editor.
            Window.DisplayMainWindow();
        }
    }
}
