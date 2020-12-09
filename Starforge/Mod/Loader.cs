using Starforge.Core;
using Starforge.MapStructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Starforge.Mod {
    public static class Loader {
        private static List<Assembly> LoadedAssemblies = new List<Assembly>();

        public static void LoadAssembly(Assembly asm) {
            try {
                foreach(Type type in asm.GetTypes()) {
                    if(type.IsSubclassOf(typeof(Entity)) && type.GetCustomAttribute<EntityDefinitionAttribute>() != null) {
                        EntityRegistry.Register(type);
                    }
                }

                LoadedAssemblies.Add(asm);
            } catch(Exception e) {
                Logger.Log(LogLevel.Error, $"Failed to load assembly: {asm.GetName()}");
                Logger.LogException(e);
            }
        }

        public static void LoadAssembly(string path) {
            try {
                Assembly asm = Assembly.LoadFile(path);
                LoadAssembly(asm);
            } catch(Exception e) {
                Logger.Log(LogLevel.Error, $"Failed to load assembly: ${path}");
                Logger.LogException(e);
            }
        }

        public static void LoadPluginAssemblies() {
            // TODO: Load plugins from mods folder

            // Hardcoded for development purposes
            LoadAssembly(Path.GetFullPath("..\\..\\..\\Starforge.Vanilla\\_bin\\Debug\\Starforge.Vanilla.dll"));
        }
    }
}
