using ImGuiNET;
using Microsoft.Xna.Framework.Input;
using Starforge.Core;
using Starforge.Editor.Actions;
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
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(200f, Engine.Instance.GraphicsDevice.Viewport.Height - Menubar.MenubarHeight));
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

            ImGui.SetNextItemWidth(185f);
            ImGui.ListBoxHeader("     ", RoomNames.Length, VisibleRoomCount);
            for (int i = 0; i < RoomNames.Length; i++) {
                if (ImGui.Selectable(RoomNames[i], MapEditor.Instance.State.SelectedRoom != null && RoomNames[i] == MapEditor.Instance.State.SelectedRoom.Name)) MapEditor.Instance.SelectRoom(i, true);
                if (ImGui.BeginPopupContextItem(RoomNames[i], ImGuiPopupFlags.MouseButtonRight)) {
                    ImGui.Text(RoomNames[i]);
                    ImGui.Separator();

                    if (ImGui.MenuItem("Configure Room")) Engine.CreateWindow(new WindowRoomConfig(MapEditor.Instance.State.LoadedLevel.Rooms[i]));
                    if (ImGui.MenuItem("Remove Room")) MapEditor.Instance.State.Apply(new RoomRemovalAction(MapEditor.Instance.State.LoadedLevel.Rooms[i]));

                    ImGui.EndPopup();
                }
            }
            ImGui.ListBoxFooter();

            ImGui.Text($"Rooms: {RoomNames.Length}");

            if (Settings.DebugMode) {
                string keys = "";
                foreach (Keys key in Input.Keyboard.GetPressedKeys()) {
                    keys += key.ToString() + " ";
                }

                ImGui.SetCursorPosY(Engine.Instance.GraphicsDevice.Viewport.Height - ImGui.GetTextLineHeightWithSpacing() * 6 - Menubar.MenubarHeight - 10);
                ImGui.Text($"{Math.Round(1000f / ImGui.GetIO().Framerate, 2)} ms/frame ({Math.Round(ImGui.GetIO().Framerate)} FPS)");
                ImGui.Text($"Pointer: {MapEditor.Instance.State.TilePointer}");
                ImGui.Text($"Cursor: {MapEditor.Instance.Camera.ScreenToReal(Input.Mouse.GetVectorPos())}");
                ImGui.Text($"Camera: {MapEditor.Instance.Camera.Position}");
                ImGui.Text($"Keys: {keys}");
                ImGui.Text($"Tool Input: {MapEditor.Instance.AcceptToolInput}");
            }
        }
    }
}
