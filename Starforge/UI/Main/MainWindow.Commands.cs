using Eto.Forms;

namespace Starforge.UI.Main {
    public partial class MainWindow {
        public class NewMapCommand : Command {
            public NewMapCommand() {
                MenuText = "New";
                ToolTip = "Create a new map";

                Shortcut = Keys.Control | Keys.N;
            }
        }

        public class OpenMapCommand : Command {
            public OpenMapCommand() {
                MenuText = "Open";
                ToolTip = "Open a map";

                Shortcut = Keys.Control | Keys.O;
            }
        }

        public class SaveMapCommand : Command {
            public SaveMapCommand() {
                MenuText = "Save";
                ToolTip = "Save the currently open file.";

                Shortcut = Keys.Control | Keys.S;
            }
        }

        public class QuitCommand : Command {
            public QuitCommand() {
                MenuText = "Quit";
                ToolTip = "Quit the application.";

                Shortcut = Keys.Alt | Keys.F4;
            }
        }

        public class SaveAsCommand : Command {
            public SaveAsCommand() {
                MenuText = "Save As";
                ToolTip = "Save the currently open file to a specific location.";

                Shortcut = Keys.Control | Keys.Shift | Keys.S;
            }
        }
    }
}
