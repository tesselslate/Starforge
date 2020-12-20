using System.Collections.Generic;

namespace Starforge.Platform {
    /// <summary>
    /// PlatformBase is a base for platform-specific helper classes. They are mostly responsible
    /// for locating certain directories, such as the user's Celeste install.
    /// </summary>
    public abstract class PlatformBase {
        /// <summary>
        /// Finds Celeste install locations on the user's system.
        /// </summary>
        /// <returns>A list of paths to Celeste installs on the user's system.</returns>
        public abstract List<string> GetCelesteDirectories();

        /// <summary>
        /// Returns the location of the user's Steam install.
        /// </summary>
        /// <returns>The location of the user's Steam install if it exists, otherwise it returns null.</returns>
        public abstract string GetSteamInstallPath();
    }
}
