using Microsoft.Win32;
using Starforge.Core;
using System;
using System.Collections.Generic;
using System.IO;

namespace Starforge.Platform {
    public class PlatformWindows : PlatformBase {
        public override List<string> GetCelesteDirectories() {
            List<string> celesteInstalls = new List<string>();

            List<string> steamLibraries = GetSteamLibraries();
            foreach (string library in steamLibraries) {
                string libCelestePath = Path.Combine(
                    library,
                    "Celeste"
                );

                if (Directory.Exists(libCelestePath)) celesteInstalls.Add(libCelestePath);
            }

            // TODO: Implement install detection for Itch/EGS

            return celesteInstalls;
        }

        public override string GetSteamInstallPath() {
            string install;

            if (Environment.Is64BitOperatingSystem) install = (string)Registry.GetValue(STEAMREG_64, "InstallPath", null);
            else install = (string)Registry.GetValue(STEAMREG_32, "InstallPath", null);

            return install;
        }

        private string SearchSteamLibrary(string library) {
            foreach (string dir in Directory.EnumerateDirectories(library)) {
                if (new DirectoryInfo(dir).Name == "Celeste") return dir;
            }

            return null;
        }


        private List<string> GetSteamLibraries() {
            List<string> libraries = new List<string>();
            string steamInstall = GetSteamInstallPath();

            if (string.IsNullOrEmpty(steamInstall)) {
                return libraries;
            }

            // Add default Steam library if it exists.
            string defaultLibrary = Path.Combine(steamInstall, "steamapps", "common");
            if (Directory.Exists(defaultLibrary)) libraries.Add(defaultLibrary);

            // Check steamapps/libraryfolders.vdf for external Steam libraries.
            string libraryFolders = Path.Combine(steamInstall, "steamapps", "libraryfolders.vdf");
            if (!File.Exists(libraryFolders)) {
                return libraries;
            }

            try {
                string[] content = File.ReadAllLines(libraryFolders);
                foreach (string rawLine in content) {
                    string[] splitLine = rawLine.Trim().Split('"');

                    if(splitLine.Length < 4) {
                        continue;
                    }

                    // Libraries are stored in libraryfolders.vdf like so:
                    //     "1"        "E:\\Steam"
                    // Splitting them by double quotes yields something like:
                    // ["", "1", "     ", "E:\\Steam"]
                    // (indices 0 and 2 can be disregarded.)

                    // Only include this library folder if it is in the proper format
                    // (libraryfolders.vdf contains a few lines other than library locations)
                    // (key is number, value is path)

                    if (int.TryParse(splitLine[1], out int unused)) {
                        string path = Path.Combine(
                            splitLine[3],
                            "steamapps",
                            "common"
                        );

                        if (Directory.Exists(path)) {
                            libraries.Add(path);
                        }
                    }
                }
            } catch (Exception e) {
                Logger.Log(LogLevel.Error, "Unable to open steamapps/libraryfolders.vdf");
                Logger.LogException(e);
            }

            return libraries;
        }

        private const string STEAMREG_32 = "HKEY_LOCAL_MACHINE\\SOFTWARE\\VALVE\\Steam\\";
        private const string STEAMREG_64 = "HKEY_LOCAL_MACHINE\\SOFTWARE\\WOW6432Node\\Valve\\Steam\\";
    }
}
