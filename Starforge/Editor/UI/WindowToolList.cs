using ImGuiNET;
using Starforge.Core;
using Starforge.Editor.Tools;
using Starforge.Util;
using System;
using System.Collections.Generic;

namespace Starforge.Editor.UI {
    public class WindowToolList : Window {
        public List<string> BGTilesets;
        public List<string> FGTilesets;
        public List<string> Tools;

        public ToolType SelectedTool;
        private int VisibleItemsCount = 0;

        public WindowToolList(Autotiler bg, Autotiler fg) {
            BGTilesets = new List<string>();
            BGTilesets.Add("Air");
            foreach (Tileset t in bg.GetTilesetList()) BGTilesets.Add(MiscHelper.CleanCamelCase(t.Path.StartsWith("bg") ? t.Path.Substring(2) : t.Path));

            FGTilesets = new List<string>();
            FGTilesets.Add("Air");
            foreach (Tileset t in fg.GetTilesetList()) FGTilesets.Add(MiscHelper.CleanCamelCase(t.Path));

            Tools = new List<string>();
            foreach (ToolType type in Enum.GetValues(typeof(ToolType))) Tools.Add(ToolManager.Tools[type].GetName());
        }

        public void UpdateListHeight() {
            VisibleItemsCount = (int)((Engine.Instance.GraphicsDevice.Viewport.Height - ImGui.GetTextLineHeightWithSpacing() * 4) / ImGui.GetTextLineHeightWithSpacing());
        }

        public override void Render() {
            ImGui.SetNextWindowPos(new System.Numerics.Vector2(Engine.Instance.GraphicsDevice.Viewport.Width - 375f, Menubar.MenubarHeight));
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(375f, Engine.Instance.GraphicsDevice.Viewport.Height - Menubar.MenubarHeight));

            ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, 0f);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0f);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0f);

            ImGui.Begin("Tools",
                ImGuiWindowFlags.NoDecoration |
                ImGuiWindowFlags.NoScrollbar |
                ImGuiWindowFlags.NoResize |
                ImGuiWindowFlags.NoTitleBar |
                ImGuiWindowFlags.NoMove
            );

            ImGui.PopStyleVar(3);
            ImGui.Columns(2, "ToolColumns", false);
            ImGui.SetColumnWidth(0, 135f);
            ImGui.SetColumnWidth(1, 240f);

            // Tool list
            ImGui.Text("Tool");
            ImGui.SetNextItemWidth(130f);
            ImGui.ListBoxHeader(" ", Tools.Count, Tools.Count);
            for (int i = 0; i < Tools.Count; i++) {
                string tool = Tools[i];
                if (ImGui.Selectable(tool, SelectedTool.ToString() == ((ToolType)i).ToString())) {
                    ToolType res = (ToolType)i;

                    SelectedTool = res;
                    ToolManager.SelectedTool = ToolManager.Tools[res];
                }
            }
            ImGui.ListBoxFooter();

            // Layer list (tile/decal only)
            if (SelectedTool == ToolType.TileBrush || SelectedTool == ToolType.TileRectangle /*|| SelectedTool == ToolType.Decal*/) {
                ImGui.NewLine();
                ImGui.Text("Layer");
                ImGui.SetNextItemWidth(130f);
                ImGui.ListBoxHeader("  ", 2, 2);
                if (ImGui.Selectable("Background", ToolManager.SelectedLayer == ToolLayer.Background)) ToolManager.SelectedLayer = ToolLayer.Background;
                if (ImGui.Selectable("Foreground", ToolManager.SelectedLayer == ToolLayer.Foreground)) ToolManager.SelectedLayer = ToolLayer.Foreground;
                ImGui.ListBoxFooter();
            }

            ImGui.NextColumn();
            // Tile/entity/etc list
            if (SelectedTool == ToolType.TileBrush || SelectedTool == ToolType.TileRectangle) {
                ImGui.Text("Tilesets");
                ImGui.SetNextItemWidth(235f);

                if (ToolManager.SelectedLayer == ToolLayer.Background) {
                    ImGui.ListBoxHeader("    ", BGTilesets.Count, VisibleItemsCount);
                    for (int i = 0; i < BGTilesets.Count; i++) {
                        if (ImGui.Selectable(BGTilesets[i], ToolManager.BGTileset == i)) ToolManager.BGTileset = i;
                    }
                    ImGui.ListBoxFooter();
                } else {
                    ImGui.ListBoxHeader("    ", FGTilesets.Count, VisibleItemsCount);
                    for (int i = 0; i < FGTilesets.Count; i++) {
                        if (ImGui.Selectable(FGTilesets[i], ToolManager.FGTileset == i)) ToolManager.FGTileset = i;
                    }
                    ImGui.ListBoxFooter();
                }
            }

            ImGui.NextColumn();
        }
    }
}
