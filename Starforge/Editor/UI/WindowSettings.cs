using ImGuiNET;
using Starforge.Core;
using Starforge.Util;
using System.Numerics;

namespace Starforge.Editor.UI {
    public class WindowSettings : Window {
        private static ImGuiColorEditFlags ColorEditFlags = ImGuiColorEditFlags.InputRGB | ImGuiColorEditFlags.DisplayRGB | ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.NoAlpha;

        private static Vector3 BackgroundColor;
        private static Vector3 SelectedRoomColor;

        public WindowSettings() {
            BackgroundColor = MiscHelper.ColorToVect3(Settings.BackgroundColor);
            SelectedRoomColor = MiscHelper.ColorToVect3(Settings.SelectedRoomColor);
        }

        public override void Render() {
            UIHelper.CenterWindow(600f, 400f);
            ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, 2f);
            ImGui.OpenPopup("Settings");
            ImGui.BeginPopupModal("Settings", ref Visible, ImGuiWindowFlags.NoResize);

            ImGui.BeginTabBar("SettingsTab", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton);
            if (ImGui.BeginTabItem("General")) {
                ImGui.Columns(2, "GeneralColumns", false);
                // 1st Column
                ImGui.SetNextItemWidth(100f);
                if (ImGui.BeginCombo("Theme", Settings.DarkTheme ? "Dark" : "Light")) {
                    if (ImGui.Selectable("Dark", Settings.DarkTheme)) {
                        Settings.DarkTheme = true;
                        ImGui.StyleColorsDark();
                    }
                    if (ImGui.Selectable("Light", !Settings.DarkTheme)) {
                        Settings.DarkTheme = false;
                        ImGui.StyleColorsLight();
                    }

                    ImGui.EndCombo();
                }

                // 2nd Column
                ImGui.NextColumn();

                ImGui.InputText("Celeste Path", ref Settings.CelesteDirectory, 4096);

                ImGui.SetCursorPos(new Vector2(500f, 370f));
                ImGui.Checkbox("Debug Mode", ref Settings.DebugMode);

                ImGui.NextColumn();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("Graphics")) {
                ImGui.Columns(2, "GraphicsColumns", false);
                // 1st Column
                ImGui.Checkbox("Always Rerender", ref Settings.AlwaysRerender);
                ImGui.Checkbox("Vertical Sync", ref Settings.VerticalSync);

                // 2nd Column
                ImGui.NextColumn();

                if (ImGui.ColorEdit3("Background Color", ref BackgroundColor, ColorEditFlags)) {
                    Settings.BackgroundColor = MiscHelper.Vect3ToColor(BackgroundColor);
                }

                if (ImGui.ColorEdit3("Selected Room Color", ref SelectedRoomColor, ColorEditFlags)) {
                    Settings.SelectedRoomColor = MiscHelper.Vect3ToColor(SelectedRoomColor);
                }

                ImGui.SetCursorPos(new Vector2(ImGui.GetWindowWidth() - 135f, ImGui.GetWindowHeight() - ImGui.GetTextLineHeightWithSpacing() - 10));
                if (ImGui.Button("Reset to Default")) {
                    BackgroundColor = MiscHelper.ColorToVect3(Settings.BackgroundColor = new Microsoft.Xna.Framework.Color(14, 14, 14));
                    SelectedRoomColor = MiscHelper.ColorToVect3(Settings.SelectedRoomColor = new Microsoft.Xna.Framework.Color(60, 60, 60));
                }

                ImGui.NextColumn();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("Advanced")) {
                ImGui.SetNextItemWidth(50f);
                ImGui.InputInt("Max Startup Threads", ref Settings.MaxStartupThreads, 0, 0);

                ImGui.EndTabItem();
            }

            ImGui.PopStyleVar();
        }
    }
}
