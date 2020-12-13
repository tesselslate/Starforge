using System;
using System.IO;

namespace Starforge.Platform {
    public abstract class PlatformBase {
        public abstract string GetCelesteDirectory();

        public abstract string GetSteamDirectory();
    }
}
