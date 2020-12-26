using ImGuiNET;
using Microsoft.Xna.Framework;
using Starforge.Core;
using Starforge.Util;
using System;

namespace Starforge.Editor.UI {

    public class WindowRoomList : Window {
        private int VisibleRoomCount = 0;
        public string[] RoomNames;

        public void UpdateListHeight() {
            VisibleRoomCount = (int)((Engine.Instance.GraphicsDevice.Viewport.Height - ImGui.GetTextLineHeightWithSpacing() * 10) / ImGui.GetTextLineHeightWithSpacing());
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
                if (ImGui.Selectable(RoomNames[i], RoomNames[i] == MapEditor.Instance.SelectedRoom.Name)) {
                    MapEditor.Instance.SelectRoom(i);
                }
            }
            ImGui.ListBoxFooter();

            ImGui.Text($"Rooms: {RoomNames.Length}");

            if (Settings.DebugMode) {
                ImGui.SetCursorPosY(Engine.Instance.GraphicsDevice.Viewport.Height - ImGui.GetTextLineHeightWithSpacing() * 3 - Menubar.MenubarHeight - 10);
                ImGui.Text($"{Math.Round(1000f / ImGui.GetIO().Framerate, 2)} ms/frame ({Math.Round(ImGui.GetIO().Framerate)} FPS)");
                ImGui.Text($"Cursor: {MapEditor.Instance.Camera.ScreenToReal(Input.Mouse.GetVectorPos())}");
                ImGui.Text($"Camera: {MapEditor.Instance.Camera.Position}");
            }
        }
    }
}
