using Microsoft.Xna.Framework;
using Starforge.Editor.UI;
using Starforge.Util;
using System;
using System.IO;

namespace Starforge.Core {
    public partial class Engine {
        /// <summary>
        /// Contains default configuration and configuration file (de)serializer.
        /// </summary>
        public static class Config {
            // General
            public static bool Debug = true;

            public static bool CelesteAutodetect = true;
            public static bool ContentAutodetect = true;

            // Graphics
            public static bool VerticalSync = true;
            public static GUITheme ImGUITheme = GUITheme.Dark;
            public static Color BackgroundColor = new Color(14, 14, 14);
            public static Color RoomColor = new Color(40, 40, 40);
            public static Color SelectedRoomColor = new Color(60, 60, 60);
            public static Color ToolAccentColor = new Color(237, 210, 31);

            public static void ReadConfig(string[] configFile) {
                foreach (string line in configFile) {
                    string[] props = line.Split('|');

                    switch (props[0]) {
                    case "ImGUITheme":
                        ImGUITheme = (GUITheme)Enum.Parse(typeof(GUITheme), props[1]);
                        break;
                    case "BackgroundColor":
                        BackgroundColor = MiscHelper.HexToColor(props[1]);
                        break;
                    case "RoomColor":
                        RoomColor = MiscHelper.HexToColor(props[1]);
                        break;
                    case "SelectedRoomColor":
                        SelectedRoomColor = MiscHelper.HexToColor(props[1]);
                        break;
                    case "ToolAccentColor":
                        ToolAccentColor = MiscHelper.HexToColor(props[1]);
                        break;
                    case "CelesteDirectory":
                        if (props[1] == "$AUTODETECT") {
                            CelesteDirectory = "";
                        } else {
                            CelesteDirectory = props[1];
                            CelesteAutodetect = false;
                        }

                        break;
                    case "ContentDirectory":
                        if (props[1] == "$AUTODETECT") {
                            ContentDirectory = "";
                        } else {
                            ContentDirectory = props[1];
                            ContentAutodetect = false;
                        }

                        break;
                    case "Debug":
                        if (!bool.TryParse(props[1], out Debug)) {
                            Debug = false;
                        }
                        break;
                    case "VerticalSync":
                        if (!bool.TryParse(props[1], out VerticalSync)) {
                            VerticalSync = true;
                        }
                        break;
                    }
                }

                switch(ImGUITheme) {
                case GUITheme.Classic:
                    SettingsWindow.Classic = true;
                    break;
                case GUITheme.Dark:
                    SettingsWindow.Dark = true;
                    break;
                case GUITheme.Light:
                    SettingsWindow.Light = true;
                    break;
                }
            }

            public static void WriteConfig(StreamWriter writer) {
                writer.WriteLine($"ImGUITheme|{ImGUITheme.ToString()}");
                writer.WriteLine($"BackgroundColor|{MiscHelper.ColorToHex(BackgroundColor)}");
                writer.WriteLine($"RoomColor|{MiscHelper.ColorToHex(RoomColor)}");
                writer.WriteLine($"SelectedRoomColor|{MiscHelper.ColorToHex(SelectedRoomColor)}");
                writer.WriteLine($"ToolAccentColor|{MiscHelper.ColorToHex(ToolAccentColor)}");
                writer.WriteLine($"CelesteDirectory|{(CelesteAutodetect ? "$AUTODETECT" : CelesteDirectory)}");
                writer.WriteLine($"ContentDirectory|{(ContentAutodetect ? "$AUTODETECT" : ContentDirectory)}");
                writer.WriteLine($"Debug|{Debug}");
                writer.WriteLine($"VerticalSync|{VerticalSync}");
            }
        }
    }

    public enum GUITheme {
        Light,
        Dark,
        Classic
    }
}
