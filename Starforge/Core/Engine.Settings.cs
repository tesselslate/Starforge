using Microsoft.Xna.Framework;

namespace Starforge.Core {
    public partial class Engine {
        public static class Config {
            public static Color BackgroundColor = new Color(14, 14, 14);

            // Temporary for testing. Proper content directory detection later.
            public static string ContentDirectory = "./Content/";
        }
    }
}
