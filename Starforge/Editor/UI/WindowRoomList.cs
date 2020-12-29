using ImGuiNET;
using Starforge.Core;
using System;

namespace Starforge.Editor.UI {

    public class WindowRoomList : Window {
        private int VisibleRoomCount = 0;
        public string[] RoomNames;

        public void UpdateListHeight(int height = 0) {
            if (height == 0) height = Engine.Instance.GraphicsDevice.Viewport.Height;
            VisibleRoomCount = (int)((height - ImGui.GetTextLineHeightWithSpacing() * 10) / ImGui.GetTextLineHeightWithSpacing());
        }

        public override void Render() {
            ImGui.SetNextWindowPos(new System.Numerics.Vector2(0f, Menubar.MenubarHeight));
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(250f, Engine.Instance.GraphicsDevice.Viewport.Height - Menubar.MenubarHeight));
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0f);
            ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, 0f);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0f);

            ImGui.Begin("Rooms",
                ImGuiWindowFlags.NoDecoration |
                ImGuiWindowFlags.NoScrollbar |
                ImGuiWindowFlags.NoResize |
                ImGuiWindowFlags.NoTitleBar |
                ImGuiWindowFlags.NoMove
            );

            ImGui.PopStyleVar(3);

            ImGui.SetNextItemWidth(235f);
            ImGui.ListBoxHeader("", RoomNames.Length, VisibleRoomCount);
            for (int i = 0; i < RoomNames.Length; i++) {
                if (ImGui.Selectable(RoomNames[i], RoomNames[i] == MapEditor.Instance.State.SelectedRoom.Name)) {
                    MapEditor.Instance.SelectRoom(i, true);
                }
            }
            ImGui.ListBoxFooter();

            ImGui.Text($"Rooms: {RoomNames.Length}");

            if (Settings.DebugMode) {
                ImGui.SetCursorPosY(Engine.Instance.GraphicsDevice.Viewport.Height - ImGui.GetTextLineHeightWithSpacing() * 5 - Menubar.MenubarHeight - 10);
                ImGui.Text($"{Math.Round(1000f / ImGui.GetIO().Framerate, 2)} ms/frame ({Math.Round(ImGui.GetIO().Framerate)} FPS)");
                ImGui.NewLine();
                ImGui.Text($"Pointer: {MapEditor.Instance.State.TilePointer}");
                ImGui.Text($"Cursor: {MapEditor.Instance.Camera.ScreenToReal(Input.Mouse.GetVectorPos())}");
                ImGui.Text($"Camera: {MapEditor.Instance.Camera.Position}");
            }
        }
    }
}
