using ImGuiNET;
using Starforge.Core;
using Starforge.Mod;
using System;
using System.Collections.Generic;

namespace Starforge.Editor.UI {
    public static class ToolWindow {

        public static int CurrentBGTileset;
        public static int CurrentFGTileset;
        public static int CurrentEntity;

        public static List<string> BGTilesets;
        public static List<string> FGTilesets;
        public static List<string> Entities;

        public static List<string> Tools;
        public static List<string> Layers;

        public static TileType CurrentTileType = TileType.Foreground;
        public static ToolType CurrentTool = ToolType.TileDraw;

        static ToolWindow() {
            BGTilesets = new List<string>();
            FGTilesets = new List<string>();
            Entities = EntityRegistry.GetEntities();

            // Tools
            Tools = new List<string>();
            foreach (ToolType type in Enum.GetValues(typeof(ToolType))) {
                Tools.Add(ToolManager.Tools[type].getName());
            }

            // Layers
            Layers = new List<string>();
            foreach (TileType type in Enum.GetValues(typeof(TileType))) {
                Layers.Add(type.ToString());
            }
        }

        public static void Render() {
            ImGui.SetNextWindowPos(new System.Numerics.Vector2(Engine.Instance.GraphicsDevice.Viewport.Width - 150f, 0f));
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(150f, Engine.Instance.GraphicsDevice.Viewport.Height));
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0f);
            ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, 0f);

            ImGui.Begin("Tools",
                ImGuiWindowFlags.NoDecoration |
                ImGuiWindowFlags.NoScrollbar |
                ImGuiWindowFlags.NoResize |
                ImGuiWindowFlags.NoTitleBar |
                ImGuiWindowFlags.NoMove);

            ImGui.PopStyleVar();
            ImGui.PopStyleVar();

            ImGui.SetNextItemWidth(135f);

            switch (CurrentTool) {
            case ToolType.EntityPlace:
                ImGui.ListBox("", ref CurrentEntity, Entities.ToArray(), Entities.Count, 30);
                break;
            case ToolType.TileDraw:
            case ToolType.TileRectangle:
                switch (CurrentTileType) {
                case TileType.Foreground:
                    ImGui.ListBox("", ref CurrentFGTileset, FGTilesets.ToArray(), FGTilesets.Count, 30);
                    break;
                case TileType.Background:
                    ImGui.ListBox("", ref CurrentBGTileset, BGTilesets.ToArray(), BGTilesets.Count, 30);
                    break;
                }
                break;
            }

            if (CurrentTool != ToolType.EntityPlace) {
                int selectedTileType = (int)CurrentTileType;
                ImGui.ListBox("Layer", ref selectedTileType, Layers.ToArray(), Layers.Count);
                CurrentTileType = (TileType)selectedTileType;
            }

            int selectedTool = (int)CurrentTool;
            ImGui.ListBox("Tool", ref selectedTool, Tools.ToArray(), Tools.Count);
            CurrentTool = (ToolType)selectedTool;

            ImGui.End();
        }
    }
}
