using Starforge.Core;
using Starforge.Editor;
using Starforge.Editor.Tools;
using Starforge.Map;
using Starforge.Mod.API;
using Starforge.Vanilla.Actions;

namespace Starforge.Vanilla.Tools {
    [ToolDefinition("TileBrush")]
    public class TileBrushTool : TileTool {
        private TileBrushAction Action = null;

        public override string GetName() => "Tiles (Brush)";

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
    }
}
