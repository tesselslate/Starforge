using Microsoft.Xna.Framework;
using Starforge.Editor.Tools;
using Starforge.Map;

namespace Starforge.Vanilla.Actions {
    public class TileRectangleAction : TileAction {
        private Rectangle Area;

        public TileRectangleAction(Room r, ToolLayer l, int t, Rectangle a) : base(r, l, t) {
            Area = a;

            if (Area.X < 0) {
                Area.Width += Area.X;
                Area.X = 0;
            }

            if (Area.Y < 0) {
                Area.Height += Area.Y;
                Area.Y = 0;
            }

            if (Area.X + Area.Width >= Grid.Width) Area.Width = Grid.Width - Area.X;
            if (Area.Y + Area.Height >= Grid.Height) Area.Height = Grid.Height - Area.Y;

            for (int x = Area.X; x < Area.X + Area.Width; x++) {
                for (int y = Area.Y; y < Area.Y + Area.Height; y++) {
                    if (Grid[x, y] != Tileset) PreviousTiles.Add((new Point(x, y), Grid[x, y]));
                }
            }
        }

        public override bool Apply() {
            bool changed = false;

            for (int x = Area.X; x < Area.X + Area.Width; x++) {
                for (int y = Area.Y; y < Area.Y + Area.Height; y++) {
                    if (Grid[x, y] != Tileset) changed = true;
                    Grid[x, y] = Tileset;
                }
            }

            Tiler.Update(DrawableRoom, Layer == ToolLayer.Foreground, Area);
            if (changed) DrawableRoom.Dirty = true;
            return changed;
        }
    }
}
