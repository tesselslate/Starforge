using ImGuiNET;

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
                ImGui.EndMenu();
            }

            if (ImGui.BeginMenu("View")) {
                ImGui.EndMenu();
            }

            if (ImGui.BeginMenu("Edit")) {
                ImGui.EndMenu();
            }

            if (ImGui.BeginMenu("Tools")) {
                ImGui.EndMenu();
            }

            if (ImGui.BeginMenu("Help")) {
                ImGui.EndMenu();
            }

            ImGui.EndMainMenuBar();
        }
    }
}
