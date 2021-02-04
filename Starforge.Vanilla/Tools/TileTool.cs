using ImGuiNET;
using Microsoft.Xna.Framework;
using Starforge.Core;
using Starforge.Editor;
using Starforge.Editor.Tools;
using Starforge.Mod.Content;

namespace Starforge.Vanilla.Tools {
    /// <summary>
    /// A base class for any tile-based tools. Implements Render, RenderGUI and CanSelectLayer.
    /// </summary>
    public abstract class TileTool : Tool {
        /// <remarks>The hint is set out of bounds (beyond upleft corner) so the hint does not appear when first selecting the tool.</remarks>
        protected Rectangle Hint = new Rectangle(-8, -8, 8, 8);

        public override bool CanSelectLayer() => true;

        public override void Render() {
            GFX.Draw.BorderedRectangle(Hint, Settings.ToolColor * 0.5f, Settings.ToolColor);
            // While this might look like a mistake, setting Hint after Rendering fixes a huge visual bug with the Rectangle Tool
            Hint.X = MapEditor.Instance.State.TilePointer.X * 8;
            Hint.Y = MapEditor.Instance.State.TilePointer.Y * 8;
        }

        public override void RenderGUI() {
            ImGui.Text("Tilesets");
            ImGui.SetNextItemWidth(235f);
            var toolListWindow = MapEditor.Instance.ToolListWindow;
            if (ToolManager.SelectedLayer == ToolLayer.Background) {
                if (ImGui.ListBoxHeader("TilesetsList", toolListWindow.BGTilesets.Count, toolListWindow.VisibleItemsCount)) {
                    for (int i = 0; i < toolListWindow.BGTilesets.Count; i++) {
                        if (ImGui.Selectable(toolListWindow.BGTilesets[i], ToolManager.BGTileset == i)) ToolManager.BGTileset = i;
                    }
                    ImGui.ListBoxFooter();
                }
            } else {
                if (ImGui.ListBoxHeader("TilesetsList", toolListWindow.FGTilesets.Count, toolListWindow.VisibleItemsCount)) {
                    for (int i = 0; i < toolListWindow.FGTilesets.Count; i++) {
                        if (ImGui.Selectable(toolListWindow.FGTilesets[i], ToolManager.FGTileset == i)) ToolManager.FGTileset = i;
                    }
                    ImGui.ListBoxFooter();
                }
            }
        }
    }
}
