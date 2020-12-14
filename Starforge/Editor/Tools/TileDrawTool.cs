using Microsoft.Xna.Framework;
using Starforge.Core;
using Starforge.Core.Input;
using Starforge.Editor.Actions;
using Starforge.Editor.UI;
using Starforge.MapStructure;

namespace Starforge.Editor.Tools {
    public class TileDrawTool : Tool {

        private Rectangle ToolHint = new Rectangle(0, 0, 8, 8);

        private static DrawTilePlacement CurrentDrawAction = null;

        public override void ManageInput(MouseEvent m) {
            Level l = Engine.Scene.SelectedLevel;

            ToolHint.X = l.TilePointer.X * 8;
            ToolHint.Y = l.TilePointer.Y * 8;

            if (m.LeftButtonClick) {
                OnLeftClick();
            }
            if (m.LeftButtonDrag) {
                OnLeftDrag();
            }
            if (m.LeftButtonUnclick) {
                OnLeftUnclick();
            }
        }

        private void OnLeftClick() {
            Level l = Engine.Scene.SelectedLevel;

            switch (ToolWindow.CurrentTileType) {
            case TileType.Foreground:
                CurrentDrawAction = new DrawTilePlacement(l, ToolWindow.CurrentTileType, ToolWindow.CurrentFGTileset, l.TilePointer);
                break;
            case TileType.Background:
                CurrentDrawAction = new DrawTilePlacement(l, ToolWindow.CurrentTileType, ToolWindow.CurrentBGTileset, l.TilePointer);
                break;
            }
            l.ApplyNewAction(CurrentDrawAction);
        }

        private void OnLeftDrag() {
            Level l = Engine.Scene.SelectedLevel;

            // when holding and continuously drawing, add to existing action
            if (CurrentDrawAction != null) {
                CurrentDrawAction.AddPoint(l.TilePointer);
            }
        }

        private void OnLeftUnclick() {
            // finished drawing
            CurrentDrawAction = null;
        }

        public override void Render() {
            GFX.Pixel.Draw(ToolHint, Engine.Config.ToolAccentColor, 0.25f);
            GFX.Draw.HollowRectangle(ToolHint, Color.Goldenrod);
        }

        public override string getName() {
            return "Tile Drawing";
        }
    }
}
