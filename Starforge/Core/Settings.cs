using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Starforge.Util;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Starforge.Core {
    /// <summary>
    /// Loads, saves, and contains settings for the editor.
    /// </summary>
    public static class Settings {
        /// <summary>
        /// The directory in which Starforge's configuration files are stored.
        /// </summary>
        public static string ConfigDirectory { get; set; }

        /// <summary>
        /// Whether or not rooms should always be rerendered.
        /// </summary>
        public static bool AlwaysRerender = false;

        /// <summary>
        /// The directory in which Celeste and its files are located.
        /// </summary>
        public static string CelesteDirectory;

        /// <summary>
        /// The background color of the editor.
        /// </summary>
        public static Color BackgroundColor = new Color(14, 14, 14);

        /// <summary>
        /// Whether or not the editor should be in dark mode.
        /// </summary>
        public static bool DarkTheme = true;

        /// <summary>
        /// The maximum amount of simultaneous threads to run when launching.
        /// </summary>
        public static int MaxStartupThreads = 2;

        /// <summary>
        /// Attempts to load the configuration file from the given path.
        /// </summary>
        /// <param name="path">The path to load the configuration file from.</param>
        /// <returns>Whether the config was successfully loaded.</returns>
        public static bool LoadConfig(string path) {
            if (!File.Exists(path)) {
                Logger.Log(LogLevel.Error, $"Configuration file {path} does not exist.");
                return false;
            }

            string[] cfgRaw = File.ReadAllLines(path);
            Dictionary<string, string> cfg = new Dictionary<string, string>();

            foreach (string line in cfgRaw) cfg.Add(line.Substring(0, line.IndexOf(" ")), line.Substring(line.IndexOf(" ") + 1));

            FieldInfo[] settingsInfo = typeof(Settings).GetFields();
            foreach (FieldInfo field in settingsInfo) {
                if (!cfg.ContainsKey(field.Name)) {
                    Logger.Log(LogLevel.Error, $"Configuration value {field.Name} was not found in the configuration file.");
                    continue;
                }

                switch (field.FieldType.FullName) {
                case "System.Boolean": // Boolean
                    if (bool.TryParse(cfg[field.Name], out bool resBool)) {
                        field.SetValue(null, resBool);
                    } else {
                        Logger.Log(LogLevel.Error, $"Configuration value {field.Name} was not a boolean. Value: {cfg[field.Name]}");
                    }

                    break;
                case "System.Int32": // Integer
                    if (int.TryParse(cfg[field.Name], out int resInt)) {
                        field.SetValue(null, resInt);
                    } else {
                        Logger.Log(LogLevel.Error, $"Configuration value {field.Name} was not an integer. Value: {cfg[field.Name]}");
                    }

                    break;
                case "System.Single": // Float
                    if (float.TryParse(cfg[field.Name], out float resFloat)) {
                        field.SetValue(null, resFloat);
                    } else {
                        Logger.Log(LogLevel.Error, $"Configuration value {field.Name} was not a float. Value: {cfg[field.Name]}");
                    }

                    break;
                case "System.String": // String
                    field.SetValue(null, cfg[field.Name]);
                    break;
                case "Microsoft.Xna.Framework.Color": // Color
                    field.SetValue(null, MiscHelper.HexToColor(cfg[field.Name]));
                    break;
                default:
                    Logger.Log(LogLevel.Error, $"Unsupported configuration field {field.Name} of type {field.FieldType.FullName}");
                    break;
                }
            }

            return true;
        }

        /// <summary>
        /// Writes the configuration to the specified path.
        /// </summary>
        /// <param name="path">The path to write the configuration to.</param>
        public static void WriteConfig(string path) {
            if (File.Exists(path)) File.Delete(path);

            using (FileStream stream = File.OpenWrite(path)) {
                using (StreamWriter writer = new StreamWriter(stream)) {
                    FieldInfo[] fields = typeof(Settings).GetFields();
                    foreach (FieldInfo field in fields) {
                        if (field.FieldType.FullName == "Microsoft.Xna.Framework.Color") {
                            writer.WriteLine($"{field.Name} {MiscHelper.ColorToHex((Color)field.GetValue(null))}");
                        } else {
                            writer.WriteLine($"{field.Name} {field.GetValue(null)}");
                        }
                    }

                    writer.Close();
                }
            }
        }
    }
}
