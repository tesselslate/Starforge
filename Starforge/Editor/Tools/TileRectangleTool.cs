using Microsoft.Xna.Framework;
using Starforge.Core;
using Starforge.Editor.Actions;
using Starforge.Map;
using Starforge.Mod.Content;

namespace Starforge.Editor.Tools {
    public class TileRectangleTool : Tool {
        private Rectangle Hint = new Rectangle(0, 0, 8, 8);
        private Rectangle Hold;
        private Point Start;

        public override string GetName() => "Tiles (Rectangle)";

        public override void Render() {
            GFX.Draw.BorderedRectangle(Hint, Settings.ToolColor * 0.5f, Settings.ToolColor);
        }

        public override void Update() {
            Hint.X = MapEditor.Instance.State.TilePointer.X * 8;
            Hint.Y = MapEditor.Instance.State.TilePointer.Y * 8;

            if (Input.Mouse.LeftClick) HandleClick();
            else if (Input.Mouse.LeftHold) HandleDrag();
            else if (Input.Mouse.LeftUnclick) HandleUnclick();
            else if (Input.Mouse.Moved) HandleMove();

            Hint = new Rectangle(Hold.X * 8, Hold.Y * 8, Hold.Width * 8, Hold.Height * 8);
        }

        private void HandleMove() {
            Hold = new Rectangle(MapEditor.Instance.State.TilePointer.X, MapEditor.Instance.State.TilePointer.Y, 1, 1);
        }

        private void HandleClick() {
            Start = MapEditor.Instance.State.TilePointer;
        }

        private void HandleDrag() {
            Point tp = MapEditor.Instance.State.TilePointer;

            Point tl = new Point(
                (int)MathHelper.Min(Start.X, tp.X),
                (int)MathHelper.Min(Start.Y, tp.Y)
            );

            Point br = new Point(
                (int)MathHelper.Max(Start.X, tp.X),
                (int)MathHelper.Max(Start.Y, tp.Y)
            );

            Hold = new Rectangle(
                tl.X, tl.Y,
                br.X - tl.X + 1,
                br.Y - tl.Y + 1
            );
        }

        private void HandleUnclick() {
            Room r = MapEditor.Instance.State.SelectedRoom;

            switch (ToolManager.SelectedLayer) {
            case ToolLayer.Background:
                MapEditor.Instance.State.Apply(new TileRectangleAction(r, ToolManager.SelectedLayer, ToolManager.BGTileset, Hold));
                break;
            case ToolLayer.Foreground:
                MapEditor.Instance.State.Apply(new TileRectangleAction(r, ToolManager.SelectedLayer, ToolManager.FGTileset, Hold));
                break;
            }

            Hold = new Rectangle(MapEditor.Instance.State.TilePointer.X, MapEditor.Instance.State.TilePointer.Y, 1, 1);
            Hint.Width = 8;
            Hint.Height = 8;
        }
    }
}
