using ImGuiNET;
using Starforge.Util;
using System;

namespace Starforge.Editor.UI {
    public class WindowUnsavedChanges : Window {
        private Action Callback;

        /// <summary>
        /// Opens a window notifying the user they have unsaved changes, and activates the callback once the user is OK with doing so.
        /// </summary>
        /// <param name="callback">The callback to activate after changes are saved.</param>
        public WindowUnsavedChanges(Action callback = null) {
            Callback = callback;
        }

        public override void Render() {
            ImGui.OpenPopup("Unsaved Changes");
            UIHelper.CenterWindow(400f, 125f);

            if (ImGui.BeginPopupModal("Unsaved Changes", ref Visible, ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize)) {
                ImGui.TextWrapped("You have unsaved changes. Would you like to save them?");
                ImGui.SetCursorPos(new System.Numerics.Vector2(195f, 90f));

                if (ImGui.Button("Cancel", new System.Numerics.Vector2(60f, 25f))) Visible = false;

                ImGui.SameLine();
                if (ImGui.Button("Don't Save", new System.Numerics.Vector2(80f, 25f))) {
                    if (Callback != null) Callback.Invoke();
                    Visible = false;
                }

                ImGui.SameLine();
                if (ImGui.Button("Save", new System.Numerics.Vector2(40f, 25f))) {
                    if (Menubar.Save()) {
                        if (Callback != null) Callback.Invoke();
                        Visible = false;
                    }
                }
            }

            ImGui.EndPopup();
        }
    }
}