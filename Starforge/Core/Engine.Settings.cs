using Microsoft.Xna.Framework;
using Starforge.Util;

namespace Starforge.Core {
    public partial class Engine {
        public static class Config {
            public static Color BackgroundColor = new Color(14, 14, 14);

            public static Color RoomColor = new Color(40, 40, 40);

            public static Color SelectedRoomColor = new Color(70, 70, 70);

            // Temporary for testing. Proper content directory detection later.
            public static string ContentDirectory = "./Content/";
        }
    }
}
