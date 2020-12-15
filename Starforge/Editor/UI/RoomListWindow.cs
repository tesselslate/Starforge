using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Starforge.Core;
using System;

namespace Starforge.Editor.UI {
    public static class RoomListWindow {
        public static int CurrentRoom = 0;

        public static int LastRoom = 0;

        public static string[] RoomNames;

        private static int RoomListHeight = 30;

        public static void UpdateListHeight(int height = 0) {
            int h = (height == 0) ? Engine.Instance.GraphicsDevice.Viewport.Height : height;
            RoomListHeight = (int)((h - ImGui.GetTextLineHeightWithSpacing() * 10) / ImGui.GetTextLineHeightWithSpacing());
        }

        public static void Render() {
            ImGui.SetNextWindowPos(new System.Numerics.Vector2(0f, MenuBar.MenuBarSize));
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(250f, Engine.Instance.GraphicsDevice.Viewport.Height - MenuBar.MenuBarSize));
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0f);
            ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, 0f);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0f);

            ImGui.Begin("Rooms",
                ImGuiWindowFlags.NoDecoration |
                ImGuiWindowFlags.NoScrollbar |
                ImGuiWindowFlags.NoResize |
                ImGuiWindowFlags.NoTitleBar |
                ImGuiWindowFlags.NoMove);

            ImGui.PopStyleVar(3);

            ImGui.SetNextItemWidth(235f);
            ImGui.ListBox("", ref CurrentRoom, RoomNames, RoomNames.Length, RoomListHeight);
            ImGui.Text($"Rooms: {RoomNames.Length}");

            MouseState m = Mouse.GetState();

            if (Engine.Config.Debug) {
                ImGui.SetCursorPosY(Engine.Instance.GraphicsDevice.Viewport.Height - ImGui.GetTextLineHeightWithSpacing() * 6 - MenuBar.MenuBarSize - 10);
                ImGui.Text($"{Math.Round(1000f / ImGui.GetIO().Framerate, 2)} ms/frame ({Math.Round(ImGui.GetIO().Framerate)} FPS)");
                ImGui.Text($"Current room: {Engine.Scene.SelectedLevel.Name}");
                ImGui.Text($"Visible rooms: {Engine.Scene.VisibleLevels.Count}");
                ImGui.Text("");
                ImGui.Text($"Cursor: {Engine.Scene.Camera.ScreenToReal(new Vector2(m.X, m.Y))}");
                ImGui.Text($"Camera: {Engine.Scene.Camera.Position}");
            }

            ImGui.End();
        }

        public static void SetCurrentRoom(int newRoom) {
            LastRoom = CurrentRoom;
            CurrentRoom = newRoom;
        }
    }
}
