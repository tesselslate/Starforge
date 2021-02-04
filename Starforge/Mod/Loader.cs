using Starforge.Core;
using Starforge.Editor;
using Starforge.Map;
using Starforge.Mod.API;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Starforge.Mod {
    public static class Loader {
        /// <summary>
        /// A list of plugin assemblies not contained in mods.
        /// </summary>
        private static List<Assembly> PluginAssemblies = new List<Assembly>();

        /// <summary>
        /// Attempts to load a plugin assembly.
        /// </summary>
        /// <param name="asm">The assembly to load.</param>
        public static void LoadPluginAssembly(Assembly asm) {
            try {
                foreach (Type type in asm.GetTypes().Where((type) => !type.IsAbstract)) {
                    if (type.IsSubclassOf(typeof(Entity))) {
                        if (type.GetCustomAttribute<EntityDefinitionAttribute>() != null || type.GetCustomAttribute<TriggerDefinitionAttribute>() != null) {
                            EntityRegistry.Register(type);
                        } else {
                            Logger.Log(LogLevel.Warning, $"Assembly {asm.GetName()} contains {type} without an appropriate definition attribute");
                        }
                    }
                    if (type.IsSubclassOf(typeof(Tool))) {
                        if (type.GetCustomAttribute<ToolDefinitionAttribute>() != null) {
                            ToolManager.Register(type);
                        }
                        else {
                            Logger.Log(LogLevel.Warning, $"Assembly {asm.GetName()} contains {type} without an appropriate definition attribute");
                        }
                    }
                }

                PluginAssemblies.Add(asm);
            } catch (Exception e) {
                Logger.Log(LogLevel.Error, $"Failed to load plugin assembly {asm.GetName()}");
                if (e is ReflectionTypeLoadException) {
                    foreach (Exception e2 in ((ReflectionTypeLoadException)e).LoaderExceptions) {
                        Logger.Log(LogLevel.Error, $"RTL - {e2}");
                    }
                }

                Logger.LogException(e);
            }
        }

        /// <summary>
        /// Attempts to load a plugin assembly from the given path.
        /// </summary>
        /// <param name="path">The path to load the assembly from.</param>
        public static void LoadPluginAssembly(string path) {
            try {
                Assembly asm = Assembly.LoadFile(path);
                LoadPluginAssembly(asm);
            } catch (Exception e) {
                Logger.Log(LogLevel.Error, $"Failed to load assembly: {path}");
                Logger.LogException(e);
            }
        }

        /// <summary>
        /// Loads all plugin assemblies.
        /// </summary>
        public static void LoadPluginAssemblies() {
            string pluginDir = Path.Combine(Settings.ConfigDirectory, "Plugins");
            if (!Directory.Exists(pluginDir)) Directory.CreateDirectory(pluginDir);

            // Load Starforge.Vanilla manually
            LoadPluginAssembly(Path.GetFullPath("./Starforge.Vanilla.dll"));

            // Load plugins from Plugins folder
            foreach (string path in Directory.GetFiles(pluginDir)) {
                LoadPluginAssembly(path);
            }
        }
    }
}
