using ImGuiNET;
using Starforge.Core;

namespace Starforge.Editor.UI {
    public static class RoomListWindow {
        public static int CurrentRoom = 0;

        public static string[] RoomNames;
        public static void Render() {
            ImGui.SetNextWindowPos(new System.Numerics.Vector2(0f, 0f));
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(100f, Engine.Instance.GraphicsDevice.Viewport.Height));
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0f);
            ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, 0f);

            ImGui.Begin("Rooms", 
                ImGuiWindowFlags.NoDecoration |
                ImGuiWindowFlags.NoScrollbar | 
                ImGuiWindowFlags.NoResize |
                ImGuiWindowFlags.NoTitleBar |
                ImGuiWindowFlags.NoMove );

            ImGui.PopStyleVar();
            ImGui.PopStyleVar();

            ImGui.PushItemWidth(200f);
            ImGui.ListBox("", ref CurrentRoom, RoomNames, RoomNames.Length, 30);
            ImGui.Text($"Rooms: {RoomNames.Length}");
            ImGui.End();
        }
    }
}
