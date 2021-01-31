using ImGuiNET;
using Microsoft.Xna.Framework;
using Starforge.Core;
using Starforge.Editor;
using Starforge.Editor.Tools;
using Starforge.Map;
using Starforge.Mod.API;
using Starforge.Mod.Content;
using Starforge.Vanilla.Actions;

namespace Starforge.Vanilla.Tools {
    [ToolDefinition("TileBrush")]
    public class TileBrushTool : Tool {
        /// <remarks>The hint is set out of bounds (beyond upleft corner) so the hint does not appear when first selecting the tool.</remarks>
        private Rectangle Hint = new Rectangle(-8, -8, 8, 8);

        private TileBrushAction Action = null;

        public override string GetName() => "Tiles (Brush)";
        public override bool CanSelectLayer() => true;

        public override void Render() {
            GFX.Draw.BorderedRectangle(Hint, Settings.ToolColor * 0.5f, Settings.ToolColor);
            Hint.X = MapEditor.Instance.State.TilePointer.X * 8;
            Hint.Y = MapEditor.Instance.State.TilePointer.Y * 8;
        }

        public override void RenderGUI() {
            CreateGUITileTool();
        }

        public override void Update() {
            if (Input.Mouse.LeftClick) HandleClick();
            if (Input.Mouse.LeftHold) HandleDrag();
            if (Input.Mouse.LeftUnclick) HandleUnclick();
        }

        private void HandleClick() {
            Room r = MapEditor.Instance.State.SelectedRoom;

            switch (ToolManager.SelectedLayer) {
            case ToolLayer.Background:
                Action = new TileBrushAction(r, ToolLayer.Background, ToolManager.BGTileset, MapEditor.Instance.State.TilePointer);
                break;
            case ToolLayer.Foreground:
                Action = new TileBrushAction(r, ToolLayer.Foreground, ToolManager.FGTileset, MapEditor.Instance.State.TilePointer);
                break;
            }

            MapEditor.Instance.State.Apply(Action);
        }

        private void HandleDrag() {
            Room r = MapEditor.Instance.State.SelectedRoom;

            if (Action != null) Action.AddPoint(MapEditor.Instance.State.TilePointer);
        }

        private void HandleUnclick() {
            Action = null;
        }

        internal static void CreateGUITileTool() {
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
