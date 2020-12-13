using Microsoft.Xna.Framework;
using Starforge.Util;

namespace Starforge.Core {
    public partial class Engine {
        public static class Config {
            public static Color BackgroundColor = new Color(14, 14, 14);

            public static Color RoomColor = new Color(40, 40, 40);

            public static Color SelectedRoomColor = new Color(60, 60, 60);

            public static Color ToolAccentColor = new Color(237, 210, 31);

            // Temporary for testing. Proper content directory detection later.
            public static string ContentDirectory = "./Content/";

            public static bool Debug = true;
        }
    }
}
