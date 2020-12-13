using ImGuiNET;
using Starforge.Core;
using System;
using System.Collections.Generic;

namespace Starforge.Editor.UI {
    public static class ToolWindow {

        public static int CurrentBGTileset;
        public static int CurrentFGTileset;

        public static List<string> BGTilesets;
        public static List<string> FGTilesets;

        public static List<string> Tools;
        public static List<string> Layers;

        public static TileType CurrentTileType = TileType.Foreground;
        public static ToolType CurrentTool = ToolType.Point;

        static ToolWindow() {
            BGTilesets = new List<string>();
            FGTilesets = new List<string>();

            // Tools
            Tools = new List<string>();
            foreach (ToolType type in Enum.GetValues(typeof(ToolType))) {
                Tools.Add(type.ToString());
            }

            // Layers
            Layers = new List<string>();
            foreach (TileType type in Enum.GetValues(typeof(TileType))) {
                Layers.Add(type.ToString());
            }
        }

        public static void Render() {
            ImGui.SetNextWindowPos(new System.Numerics.Vector2(Engine.Instance.GraphicsDevice.Viewport.Width - 150f, MenuBar.MenuBarSize));
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(150f, Engine.Instance.GraphicsDevice.Viewport.Height - MenuBar.MenuBarSize));
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0f);
            ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, 0f);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0f);

            ImGui.Begin("Tools",
                ImGuiWindowFlags.NoDecoration |
                ImGuiWindowFlags.NoScrollbar |
                ImGuiWindowFlags.NoResize |
                ImGuiWindowFlags.NoTitleBar |
                ImGuiWindowFlags.NoMove);

            ImGui.PopStyleVar(3);

            ImGui.SetNextItemWidth(135f);
            if (CurrentTileType == TileType.Foreground) {
                ImGui.ListBox("", ref CurrentFGTileset, FGTilesets.ToArray(), FGTilesets.Count, 30);
            }
            else {
                ImGui.ListBox("", ref CurrentBGTileset, BGTilesets.ToArray(), BGTilesets.Count, 30);
            }

            int selectedTileType = (int)CurrentTileType;
            int selectedTool = (int)CurrentTool;
            ImGui.ListBox("Layer", ref selectedTileType, Layers.ToArray(), Layers.Count);
            ImGui.ListBox("Tool", ref selectedTool, Tools.ToArray(), Tools.Count);
            CurrentTileType = (TileType)selectedTileType;
            CurrentTool = (ToolType)selectedTool;

            ImGui.End();
        }
    }
}
