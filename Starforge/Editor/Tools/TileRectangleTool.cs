using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Starforge.Core;
using Starforge.Core.Input;
using Starforge.Editor.Actions;
using Starforge.Editor.UI;
using Starforge.MapStructure;

namespace Starforge.Editor.Tools {

    public class TileRectangleTool : Tool {

        private Rectangle ToolHint = new Rectangle(0, 0, 8, 8);
        private static Rectangle Hold;
        private static Point HoldStart;

        public override void ManageInput(MouseEvent m) {
            Level l = Engine.Scene.SelectedLevel;

            ToolHint.X = l.TilePointer.X * 8;
            ToolHint.Y = l.TilePointer.Y * 8;

            if (m.LeftButtonClick) {
                OnLeftClick();
            }
            else if (m.LeftButtonDrag) {
                OnLeftDrag();
            }
            else if (m.LeftButtonUnclick) {
                OnLeftUnclick();
            }
            else if (m.MouseMoved) {
                OnMouseMove();
            }

            ToolHint = new Rectangle(Hold.X * 8, Hold.Y * 8, Hold.Width * 8, Hold.Height * 8);
        }

        private void OnMouseMove() {
            Level l = Engine.Scene.SelectedLevel;
            Hold = new Rectangle(l.TilePointer.X, l.TilePointer.Y, 1, 1);
        }

        private void OnLeftClick() {
            Level l = Engine.Scene.SelectedLevel;
            HoldStart = new Point(l.TilePointer.X, l.TilePointer.Y);
        }

        private void OnLeftDrag() {
            Level l = Engine.Scene.SelectedLevel;
            Point topLeftCorner = new Point(
                (int)MathHelper.Min(HoldStart.X, l.TilePointer.X),
                (int)MathHelper.Min(HoldStart.Y, l.TilePointer.Y)
            );
            Point botRightCorner = new Point(
                (int)MathHelper.Max(HoldStart.X, l.TilePointer.X),
                (int)MathHelper.Max(HoldStart.Y, l.TilePointer.Y)
            );
            Hold = new Rectangle(
                topLeftCorner.X,
                topLeftCorner.Y,
                botRightCorner.X - topLeftCorner.X + 1,
                botRightCorner.Y - topLeftCorner.Y + 1
            );
        }

        private void OnLeftUnclick() {
            Level l = Engine.Scene.SelectedLevel;
            switch (ToolWindow.CurrentTileType) {
            case TileType.Foreground:
                l.ApplyNewAction(new RectangleTilePlacement(l, ToolWindow.CurrentTileType, ToolWindow.CurrentFGTileset, Hold));
                break;
            case TileType.Background:
                l.ApplyNewAction(new RectangleTilePlacement(l, ToolWindow.CurrentTileType, ToolWindow.CurrentBGTileset, Hold));
                break;
            }
            Hold = new Rectangle(l.TilePointer.X, l.TilePointer.Y, 1, 1);
            ToolHint.Width = 8;
            ToolHint.Height = 8;
        }

        public override void Render() {
            GFX.Pixel.Draw(ToolHint, Engine.Config.ToolAccentColor, 0.25f);
            GFX.Draw.HollowRectangle(ToolHint, Color.Goldenrod);
        }

        public override string getName() {
            return "Tile Rectangles";
        }
    }

}
