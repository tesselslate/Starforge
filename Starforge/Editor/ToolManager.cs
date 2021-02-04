using Starforge.Core;
using Starforge.Editor.Tools;
using Starforge.Mod.API;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Starforge.Editor {
    public static class ToolManager {
        /// <summary>
        /// A dictionary containing all available tools.
        /// </summary>
        public static Dictionary<string, Tool> Tools;

        /// <summary>
        /// The currently selected tool.
        /// </summary>
        public static Tool SelectedTool {
            get => _selectedTool ??= Tools["TileBrush"];
            set => _selectedTool = value;
        }
        private static Tool _selectedTool;

        /// <summary>
        /// The currently selected tool layer (background/foreground).
        /// </summary>
        public static ToolLayer SelectedLayer;

        /// <summary>
        /// The index of the currently selected background tileset.
        /// </summary>
        public static int BGTileset;

        /// <summary>
        /// The index of the currently selected foreground tileset.
        /// </summary>
        public static int FGTileset;

        static ToolManager() {
            Tools = new Dictionary<string, Tool>();

            SelectedLayer = ToolLayer.Foreground;

            BGTileset = 0;
            FGTileset = 0;
        }

        public static void Render() {
            SelectedTool.Render();
        }

        public static void Update() {
            SelectedTool.Update();
        }

        public static void Register(Type type) {
            try {
                if (!type.IsSubclassOf(typeof(Tool))) return;

                ToolDefinitionAttribute attr = type.GetCustomAttribute<ToolDefinitionAttribute>();
                if (attr == null) {
                    Logger.Log(LogLevel.Error, $"Tool {type} does not have a definition attribute");
                    return;
                }

                string id = attr.ID;
                ConstructorInfo ctor = type.GetConstructor(Array.Empty<Type>());

                if (ctor == null) {
                    Logger.Log(LogLevel.Error, $"Tool of type {type} with ID {id} does not have a valid ctor");
                    return;
                }

                Tool tool = (Tool)ctor.Invoke(Array.Empty<object>());
                Tools.Add(id, tool);
                Logger.Log($"Registered tool {id} of type {type}");
            } catch (Exception e) {
                Logger.Log(LogLevel.Error, $"Encountered an error while attempting to register tool {type}");
                Logger.LogException(e);
            }
        }
    }
}
