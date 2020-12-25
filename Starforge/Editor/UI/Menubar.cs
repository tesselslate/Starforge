using ImGuiNET;
using Starforge.Core;
using Starforge.Core.Interop;
using Starforge.Map;
using System;
using System.IO;

namespace Starforge.Editor.UI {
    /// <summary>
    /// The menubar of the window.
    /// </summary>
    public static class Menubar {
        /// <summary>
        /// Renders the window menubar.
        /// </summary>
        /// <param name="hasEditor">Whether or not the map editor is currently loaded.</param>
        public static void Render(bool hasEditor = false) {
            if (!ImGui.BeginMainMenuBar()) return;

            if (ImGui.BeginMenu("File")) {
                if (ImGui.MenuItem("New")) New();
                if (ImGui.MenuItem("Open", "CTRL+O")) Open();
                if (ImGui.MenuItem("Save", "CTRL+S", false, Engine.MapLoaded)) Save();
                if (ImGui.MenuItem("Save As", "", false, Engine.MapLoaded)) SaveAs();

                ImGui.EndMenu();
            }

            if (ImGui.BeginMenu("Edit")) {
                if (ImGui.MenuItem("Undo", "CTRL+Z", false, Engine.MapLoaded && MapEditor.Instance.CanUndo)) MapEditor.Instance.Undo();
                if (ImGui.MenuItem("Redo", "CTRL+SHIFT+Z", false, Engine.MapLoaded && MapEditor.Instance.CanRedo)) MapEditor.Instance.Redo();

                ImGui.EndMenu();
            }

            if (ImGui.BeginMenu("View")) {
                ImGui.EndMenu();
            }

            if (ImGui.BeginMenu("Tools")) {
                if (Settings.DebugMode) {
                    ImGui.Separator();
                    if (ImGui.MenuItem("Force GC")) GC.Collect(2, GCCollectionMode.Forced, true, true);
                }

                ImGui.EndMenu();
            }

            if (ImGui.BeginMenu("Help")) {
                ImGui.EndMenu();
            }

            ImGui.EndMainMenuBar();
        }

        #region File

        public static void New() {
            if (Engine.MapLoaded && MapEditor.Instance.Unsaved) {
                // Unsaved changes - notify user
                Engine.CreateWindow(new WindowUnsavedChanges(New));
            } else {
                Engine.CreateWindow(new WindowNewMap());
            }
        }

        public static void Open() {
            if (NfdResult.OKAY == NFD.OpenDialog("bin", Settings.CelesteDirectory, out string mapPath)) {
                MapEditor editor = new MapEditor();
                editor.LoadLevel(mapPath);
                Engine.SetScene(editor);
            }
        }

        public static bool Save() {
            if (Engine.MapLoaded && MapEditor.Instance.Unsaved) {
                if (string.IsNullOrEmpty(MapEditor.Instance.Path)) {
                    if (NfdResult.OKAY == NFD.SaveDialog("bin", Settings.CelesteDirectory, out string mapPath)) {
                        MapEditor.Instance.Path = mapPath;
                    }
                }

                if (string.IsNullOrEmpty(MapEditor.Instance.Path)) return false;

                using (FileStream stream = File.OpenWrite(MapEditor.Instance.Path)) {
                    using (BinaryWriter writer = new BinaryWriter(stream)) {
                        MapPacker.WriteMapBinary(writer, MapEditor.Instance.Level.Encode());
                        return true;
                    }
                }
            }

            return true;
        }

        public static void SaveAs() {

        }

        #endregion

        #region Edit



        #endregion

        #region View



        #endregion

        #region Tools



        #endregion

        #region Help



        #endregion
    }
}
