using Microsoft.Xna.Framework;
using Starforge.Editor;
using Starforge.Editor.Actions;
using Starforge.Editor.Render;
using Starforge.Map;
using System.Collections.Generic;

namespace Starforge.Vanilla.Actions {
    public abstract class TileAction : EditorAction {
        protected ToolLayer Layer { get; }
        protected short Tileset;
        protected Autotiler Tiler;
        protected TileGrid Grid;
        protected TextureMap Textures;

        protected List<(Point, short)> PreviousTiles;

        public TileAction(Room r, ToolLayer l, int t) : base(r) {
            PreviousTiles = new List<(Point, short)>();

            Layer = l;
            Tiler = l == ToolLayer.Background ? MapEditor.Instance.BGAutotiler : MapEditor.Instance.FGAutotiler;
            Grid = l == ToolLayer.Background ? r.BackgroundTiles : r.ForegroundTiles;
            Textures = l == ToolLayer.Background ? DrawableRoom.BGTiles : DrawableRoom.FGTiles;

            if (t == 0) Tileset = TileGrid.TILE_AIR;
            else Tileset = (short)Tiler.GetTilesetList()[t - 1].ID;
        }

        public override bool Undo() {
            bool changed = false;

            foreach ((Point p, short t) in PreviousTiles) {
                if (SetPoint(p, t)) changed = true;
            }

            if (changed) DrawableRoom.Dirty = true;

            return changed;
        }

        protected bool SetPoint(Point p) {
            return SetPoint(p, Tileset);
        }

        protected bool SetPoint(Point p, short t) {
            bool res = Grid[p.X, p.Y] != t;
            Grid[p.X, p.Y] = t;

            if (res) {
                Tiler.Update(DrawableRoom, Layer == ToolLayer.Foreground, p);
                DrawableRoom.Dirty = true;
            }

            return res;
        }
    }
}
