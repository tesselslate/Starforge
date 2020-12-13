using ImGuiNET;

namespace Starforge.Editor.UI {
    public static class MenuBar {
        public static float MenuBarSize {
            get;
            private set;
        }

        // File Menu
        public static bool New;
        public static bool Open;
        public static bool Save;
        public static bool SaveAs;

        // Allow Edit Menu options
        public static bool AllowUndo = false;
        public static bool AllowRedo = false;

        // Edit Menu
        public static bool Undo;
        public static bool Redo;

        // Tools Menu
        public static bool Settings;

        public static void Render() {
            // Reset menu item states
            New = Open = Save = SaveAs = Undo = Redo = false;

            ImGui.BeginMainMenuBar();
            MenuBarSize = ImGui.GetWindowHeight();

            if (ImGui.BeginMenu("File")) {
                ImGui.MenuItem("New", null, ref New, true);
                ImGui.MenuItem("Open", "CTRL+O", ref Open, true);
                ImGui.MenuItem("Save", "CTRL+S", ref Save, true);
                ImGui.MenuItem("Save As", "CTRL+SHIFT+S", ref SaveAs, true);

                ImGui.EndMenu();
            }

            if (ImGui.BeginMenu("Edit")) {
                ImGui.MenuItem("Undo", "CTRL+Z", ref Undo, AllowUndo);
                ImGui.MenuItem("Redo", "CTRL+SHIFT+Z", ref Redo, AllowRedo);

                ImGui.EndMenu();
            }

            if (ImGui.BeginMenu("View")) {
                // TODO: Implement View options
                ImGui.EndMenu();
            }

            if (ImGui.BeginMenu("Help")) {
                // TODO: Implement Help options
                ImGui.EndMenu();
            }

            if (ImGui.BeginMenu("Tools")) {
                ImGui.MenuItem("Settings", null, ref Settings, true);

                ImGui.EndMenu();
            }

            ImGui.EndMainMenuBar();
        }
    }
}
