using System.Collections.Generic;

namespace Starforge.Platform {
    /// <summary>
    /// PlatformBase is a base for platform-specific helper classes. They are mostly responsible
    /// for locating certain directories, such as the user's Celeste install.
    /// </summary>
    public abstract class PlatformBase {
        public abstract List<string> GetCelesteDirectories();

        public abstract string GetSteamInstallPath();
    }
}
