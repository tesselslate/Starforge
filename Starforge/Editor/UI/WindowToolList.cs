using ImGuiNET;
using Starforge.Core;
using Starforge.Mod;
using Starforge.Mod.API;
using Starforge.Util;
using System.Collections.Generic;

namespace Starforge.Editor.UI {
    public class WindowToolList : Window {
        public List<string> BGTilesets;
        public List<string> FGTilesets;

        public Dictionary<string, Placement> Placements;

        public List<string> Tools;

        public string SelectedTool;
        public int VisibleItemsCount = 0;

        public WindowToolList(Autotiler bg, Autotiler fg) {
            BGTilesets = new List<string> { "Air" };
            foreach (Tileset t in bg.GetTilesetList()) BGTilesets.Add(MiscHelper.CleanCamelCase(t.Path.StartsWith("bg") ? t.Path.Substring(2) : t.Path));

            FGTilesets = new List<string> { "Air" };
            foreach (Tileset t in fg.GetTilesetList()) FGTilesets.Add(MiscHelper.CleanCamelCase(t.Path));

            Placements = new Dictionary<string, Placement>(EntityRegistry.GetRegisteredPlacements());

            Tools = new List<string>() { "TileBrush", "TileRectangle", "Entity"}; // Hardcoded tool names to force a specific tool order that's familliar to Ahorn users.
            foreach (string tool in ToolManager.Tools.Keys) if (!Tools.Contains(tool)) Tools.Add(tool);

            SelectedTool = "TileBrush";
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
            if (ImGui.ListBoxHeader("ToolsList", Tools.Count, Tools.Count)) {
                for (int i = 0; i < Tools.Count; i++) {
                    string tool = Tools[i];
                    if (ImGui.Selectable(ToolManager.Tools[tool].GetName(), SelectedTool == tool)) {
                        SelectedTool = tool;
                        ToolManager.SelectedTool = ToolManager.Tools[tool];
                    }
                }
                ImGui.ListBoxFooter();
            }

            // Layer list
            if (ToolManager.SelectedTool.CanSelectLayer()) {
                ImGui.NewLine();
                ImGui.Text("Layer");
                ImGui.SetNextItemWidth(130f);
                if (ImGui.ListBoxHeader("LayersList", 2, 2)) {
                    if (ImGui.Selectable("Background", ToolManager.SelectedLayer == ToolLayer.Background)) ToolManager.SelectedLayer = ToolLayer.Background;
                    if (ImGui.Selectable("Foreground", ToolManager.SelectedLayer == ToolLayer.Foreground)) ToolManager.SelectedLayer = ToolLayer.Foreground;
                    ImGui.ListBoxFooter();
                }
            }

            ImGui.NextColumn();

            // Tile/entity/etc list
            ToolManager.SelectedTool.RenderGUI();

            ImGui.NextColumn();
        }
    }
}
