using ImGuiNET;
using Starforge.Core;
using Starforge.MapStructure;
using Starforge.Util;
using System.Numerics;

namespace Starforge.Editor.UI {
    public static class SettingsWindow {
        public static bool Classic = false;
        public static bool Dark = false;
        public static bool Light = false;
        private static GUITheme PreviousTheme;

        private static Vector3 BackgroundColor;
        private static Vector3 RoomColor;
        private static Vector3 SelectedRoomColor;
        private static Vector3 ToolAccentColor;

        private static ImGuiColorEditFlags ColorEditFlags = ImGuiColorEditFlags.InputRGB | ImGuiColorEditFlags.DisplayRGB | ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.NoAlpha;

        public static void Prepare() {
            BackgroundColor = MiscHelper.ColorToVect3(Engine.Config.BackgroundColor);
            RoomColor = MiscHelper.ColorToVect3(Engine.Config.RoomColor);
            SelectedRoomColor = MiscHelper.ColorToVect3(Engine.Config.SelectedRoomColor);
            ToolAccentColor = MiscHelper.ColorToVect3(Engine.Config.ToolAccentColor);
        }

        public static void Render() {
            PreviousTheme = Engine.Config.ImGUITheme;

            ImGui.SetNextWindowSize(new Vector2(600f, 400f));
            ImGui.SetNextWindowPos(new Vector2(Engine.Instance.GraphicsDevice.Viewport.Width / 2 - 300, Engine.Instance.GraphicsDevice.Viewport.Height / 2 - 200));
            ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, 2f);

            ImGui.OpenPopup("Settings");
            ImGui.BeginPopupModal("Settings", ref MenuBar.Settings, ImGuiWindowFlags.NoResize);

            ImGui.BeginTabBar("SettingsTab", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton);

            // General Settings
            if (ImGui.BeginTabItem("General")) {
                ImGui.Checkbox("Debug Mode", ref Engine.Config.Debug);

                ImGui.NewLine();
                ImGui.Separator();
                ImGui.Text("Paths");
                ImGui.NewLine();

                if (!Engine.Config.CelesteAutodetect) {
                    ImGui.InputText("Celeste Path", ref Engine.CelesteDirectory, 1000);
                }
                ImGui.Checkbox("Celeste (Autodetected)", ref Engine.Config.CelesteAutodetect);

                if (!Engine.Config.ContentAutodetect) {
                    ImGui.InputText("Content Path", ref Engine.ContentDirectory, 1000);
                }
                ImGui.Checkbox("Content (Autodetected)", ref Engine.Config.ContentAutodetect);
                ImGui.EndTabItem();
            }

            // Visual Settings
            if (ImGui.BeginTabItem("Graphics")) {
                ImGui.Checkbox("Vertical Sync", ref Engine.Config.VerticalSync);

                ImGui.NewLine();
                ImGui.Separator();
                ImGui.Text("Theming");
                ImGui.NewLine();
                ImGui.Columns(2, "GraphicsColumns", false);

                ImGui.SetNextItemWidth(100f);
                if (ImGui.BeginCombo("Color Theme", Engine.Config.ImGUITheme.ToString())) {
                    ImGui.Selectable("Classic", ref Classic);
                    ImGui.Selectable("Dark", ref Dark);
                    ImGui.Selectable("Light", ref Light);
                    ImGui.EndCombo();

                    if (Classic && PreviousTheme != GUITheme.Classic) {
                        ImGui.StyleColorsClassic();
                        Engine.Config.ImGUITheme = GUITheme.Classic;

                        Dark = Light = false;
                    }

                    if (Dark && PreviousTheme != GUITheme.Dark) {
                        ImGui.StyleColorsDark();
                        Engine.Config.ImGUITheme = GUITheme.Dark;

                        Classic = Light = false;
                    }

                    if (Light && PreviousTheme != GUITheme.Light) {
                        ImGui.StyleColorsLight();
                        Engine.Config.ImGUITheme = GUITheme.Light;

                        Classic = Dark = false;
                    }
                }

                ImGui.NextColumn();

                bool RerenderAll = false;
                if (ImGui.ColorEdit3("Background Color", ref BackgroundColor, ColorEditFlags)) {
                    Engine.Config.BackgroundColor = MiscHelper.Vect3ToColor(BackgroundColor);
                }

                if (ImGui.ColorEdit3("Room Color", ref RoomColor, ColorEditFlags)) {
                    Engine.Config.RoomColor = MiscHelper.Vect3ToColor(RoomColor);
                    RerenderAll = true;
                }

                if (ImGui.ColorEdit3("Selected Room Color", ref SelectedRoomColor, ColorEditFlags)) {
                    Engine.Config.SelectedRoomColor = MiscHelper.Vect3ToColor(SelectedRoomColor);
                    if (Engine.Scene.LoadedMap != null) {
                        Engine.Scene.SelectedLevel.Dirty = true;
                    }
                }
                if (ImGui.ColorEdit3("Tool Accent Color", ref ToolAccentColor, ColorEditFlags)) {
                    Engine.Config.ToolAccentColor = MiscHelper.Vect3ToColor(ToolAccentColor);
                    if (Engine.Scene.LoadedMap != null) {
                        Engine.Scene.SelectedLevel.Dirty = true;
                    }
                }

                ImGui.SetCursorPos(new Vector2(ImGui.GetWindowWidth() - 135f, ImGui.GetWindowHeight() - ImGui.GetTextLineHeightWithSpacing() - 10));
                if (ImGui.Button("Reset to Default")) {
                    BackgroundColor = MiscHelper.ColorToVect3(Engine.Config.BackgroundColor = new Microsoft.Xna.Framework.Color(14, 14, 14));
                    RoomColor = MiscHelper.ColorToVect3(Engine.Config.RoomColor = new Microsoft.Xna.Framework.Color(40, 40, 40));
                    SelectedRoomColor = MiscHelper.ColorToVect3(Engine.Config.SelectedRoomColor = new Microsoft.Xna.Framework.Color(60, 60, 60));
                    ToolAccentColor = MiscHelper.ColorToVect3(Engine.Config.ToolAccentColor = new Microsoft.Xna.Framework.Color(237, 210, 31));
                    RerenderAll = true;
                }

                // If colors were changed, rooms need to be rerendered.
                if (RerenderAll) {
                    if (Engine.Scene.LoadedMap != null) {
                        foreach (Level level in Engine.Scene.LoadedMap.Levels) {
                            level.Dirty = true;
                        }
                    }
                }

                ImGui.NextColumn();

                ImGui.SetCursorPosY(ImGui.GetWindowHeight() - ImGui.GetTextLineHeightWithSpacing() - 10);
                ImGui.Text("Some changes may require a restart.");
                ImGui.EndTabItem();
            }

            ImGui.PopStyleVar();
            ImGui.EndTabBar();
            ImGui.EndPopup();
        }
    }
}
