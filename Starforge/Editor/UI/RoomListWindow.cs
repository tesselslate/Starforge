using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Starforge.Core;

namespace Starforge.Editor.UI {
    public static class RoomListWindow {
        public static int CurrentRoom = 0;

        public static int LastRoom = 0;

        public static string[] RoomNames;

        public static void Render() {
            ImGui.SetNextWindowPos(new System.Numerics.Vector2(0f, 0f));
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(250f, Engine.Instance.GraphicsDevice.Viewport.Height));
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0f);
            ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, 0f);

            ImGui.Begin("Rooms",
                ImGuiWindowFlags.NoDecoration |
                ImGuiWindowFlags.NoScrollbar |
                ImGuiWindowFlags.NoResize |
                ImGuiWindowFlags.NoTitleBar |
                ImGuiWindowFlags.NoMove);

            ImGui.PopStyleVar();
            ImGui.PopStyleVar();

            ImGui.SetNextItemWidth(235f);
            ImGui.ListBox("", ref CurrentRoom, RoomNames, RoomNames.Length, 30);
            ImGui.Text($"Rooms: {RoomNames.Length}");

            MouseState m = Mouse.GetState();

            if(Engine.Config.Debug) {
                ImGui.SetCursorPosY(Engine.Instance.GraphicsDevice.Viewport.Height - ImGui.GetTextLineHeightWithSpacing() * 6);
                ImGui.Text(string.Format("{0:F3} ms/frame ({1:F1} FPS)", 1000f / ImGui.GetIO().Framerate, ImGui.GetIO().Framerate));
                ImGui.Text($"Current room: {Engine.Scene.SelectedLevel.Name}");
                ImGui.Text($"Visible rooms: {Engine.Scene.VisibleLevels.Count}");
                ImGui.Text("");
                ImGui.Text($"Cursor: {Engine.Scene.Camera.ScreenToReal(new Vector2(m.X, m.Y))}");
                ImGui.Text($"Camera: {Engine.Scene.Camera.Position}");
            }

            ImGui.End();
        }
    }
}
