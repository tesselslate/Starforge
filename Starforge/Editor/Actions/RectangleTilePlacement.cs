using Microsoft.Xna.Framework;
using Starforge.MapStructure;
using System;

namespace Starforge.Editor.Actions {

    public class RectangleTilePlacement : TilePlacement {

        private Rectangle Area;

        public RectangleTilePlacement(Level l, TileType t, int tileset, Rectangle a)
            : base(l, t, tileset) {
            Area = a;

            // crop area to not be larger than the room
            if (Area.X < 0) {
                Area.Width += Area.X;
                Area.X = 0;
            }
            if (Area.Y < 0) {
                Area.Height += Area.Y;
                Area.Y = 0;
            }
            if (Area.X + Area.Width >= Grid.Width) {
                Area.Width = Grid.Width - Area.X;
            }
            if (Area.Y + Area.Height >= Grid.Height) {
                Area.Height = Grid.Height - Area.Y;
            }

            for (int x = Area.X; x < Area.X + Area.Width; x++) {
                for (int y = Area.Y; y < Area.Y + Area.Height; y++) {
                    if (Grid[x, y] != TileSet) {
                        PreviousTiles.Add((new Point(x, y), Grid[x, y]));
                    }
                }
            }
        }

        public override bool Apply() {
            for (int x = Area.X; x < Area.X + Area.Width; x++) {
                for (int y = Area.Y; y < Area.Y + Area.Height; y++) {
                    if (TileSet == 0) {
                        Grid.SetTile(x, y, '0');
                    }
                    else {
                        Grid.SetTile(x, y, Tiler.GetTilesetList()[TileSet - 1].ID);
                    }
                }
            }

            return Tiler.Update(Grid, Textures, Area);
        }

        public override ToolType GetToolType() {
            return ToolType.Rectangle;
        }

    }
}
