using Microsoft.Win32;
using System;
using System.IO;

namespace Starforge.Platform {
    public class PlatformWindows : PlatformBase {
        public override string GetCelesteDirectory() {
            string SteamPath = GetSteamDirectory();
            if(string.IsNullOrEmpty(SteamPath) || !Directory.Exists(SteamPath)) return null;

            return Path.Combine(SteamPath, "steamapps", "common", "Celeste");
        }

        public override string GetSteamDirectory() {
            // TODO: This code does not account for custom Steam libraries.
            string install;

            if(Environment.Is64BitOperatingSystem) install = (string)Registry.GetValue(STEAMREG_64, "InstallPath", null);
            else install = (string)Registry.GetValue(STEAMREG_32, "InstallPath", null);

            return install;
        }

        private const string STEAMREG_32 = "HKEY_LOCAL_MACHINE\\SOFTWARE\\VALVE\\Steam\\";
        private const string STEAMREG_64 = "HKEY_LOCAL_MACHINE\\SOFTWARE\\WOW6432Node\\Valve\\Steam\\";
    }
}
