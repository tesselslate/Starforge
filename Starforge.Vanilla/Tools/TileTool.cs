using ImGuiNET;
using Microsoft.Xna.Framework;
using Starforge.Core;
using Starforge.Editor;
using Starforge.Editor.UI;
using Starforge.Mod.Content;
using System.Linq;

namespace Starforge.Vanilla.Tools {
    /// <summary>
    /// A base class for any tile-based tools. Implements Render, RenderGUI and CanSelectLayer.
    /// </summary>
    public abstract class TileTool : Tool {
        /// <remarks>The hint is set out of bounds (beyond upleft corner) so the hint does not appear when first selecting the tool.</remarks>
        protected Rectangle Hint = new Rectangle(-8, -8, 8, 8);
        private ToolLayer[] selectableLayers = new ToolLayer[] { ToolLayer.Foreground, ToolLayer.Background };
        public override ToolLayer[] GetSelectableLayers() => selectableLayers;

        public override string GetSearchGroup() => "Tile";

        public override void Render() {
            GFX.Draw.BorderedRectangle(Hint, Settings.ToolColor * 0.5f, Settings.ToolColor);
            // While this might look like a mistake, setting Hint after Rendering fixes a huge visual bug with the Rectangle Tool
            Hint.X = MapEditor.Instance.State.TilePointer.X * 8;
            Hint.Y = MapEditor.Instance.State.TilePointer.Y * 8;
        }

        public override void RenderGUI() {
            ImGui.Text("Tilesets");
            ImGui.SetNextItemWidth(235f);

            string search = MapEditor.Instance.ToolListWindow.Searches[GetSearchGroup()];
            var toolListWindow = MapEditor.Instance.ToolListWindow;

            if (ToolManager.SelectedLayer == ToolLayer.Background) {
                if (ImGui.ListBoxHeader("TilesetsList", toolListWindow.BGTilesets.Count, toolListWindow.VisibleItemsCount)) {
                    WindowToolList.CreateSelectables(search, toolListWindow.BGTilesets.OrderBy((s) => s), (item) => {
                        if (ImGui.Selectable(item, ToolManager.BGTileset == toolListWindow.BGTilesets.IndexOf(item))) ToolManager.BGTileset = toolListWindow.BGTilesets.IndexOf(item);
                    });
                    ImGui.ListBoxFooter();
                }
            }
            else {
                if (ImGui.ListBoxHeader("TilesetsList", toolListWindow.FGTilesets.Count, toolListWindow.VisibleItemsCount)) {
                    WindowToolList.CreateSelectables(search, toolListWindow.FGTilesets.OrderBy((s) => s), (item) => {
                        if (ImGui.Selectable(item, ToolManager.FGTileset == toolListWindow.FGTilesets.IndexOf(item))) ToolManager.FGTileset = toolListWindow.FGTilesets.IndexOf(item); 
                    });
                    ImGui.ListBoxFooter();
                }
            }
        }
    }
}
