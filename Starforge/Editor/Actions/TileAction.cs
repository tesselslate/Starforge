using Microsoft.Xna.Framework;
using Starforge.Editor.Render;
using Starforge.Editor.Tools;
using Starforge.Map;
using System.Collections.Generic;

namespace Starforge.Editor.Actions {
    public abstract class TileAction : EditorAction {
        protected ToolLayer Layer { get; }
        protected short Tileset;
        protected Autotiler Tiler;
        protected TileGrid Grid;
        protected TextureMap Textures;

        protected DrawableRoom DrawableRoom;
        protected List<(Point, short)> PreviousTiles;

        public TileAction(Room r, ToolLayer l, int t) : base(r) {
            DrawableRoom = MapEditor.Instance.Renderer.GetRoom(r);
            PreviousTiles = new List<(Point, short)>();

            Layer = l;
            Tiler = l == ToolLayer.Background ? MapEditor.Instance.BGAutotiler : MapEditor.Instance.FGAutotiler;
            Grid = l == ToolLayer.Background ? r.BackgroundTiles : r.ForegroundTiles;
            Textures = l == ToolLayer.Background ? DrawableRoom.BGTiles : DrawableRoom.FGTiles;

            if (t == 0) Tileset = TileGrid.TILE_AIR;
            else Tileset = (short)Tiler.GetTilesetList()[t - 1].ID;
        }

        public abstract ToolType GetToolType();

        protected void Redraw() {
            if (Room == MapEditor.Instance.State.SelectedRoom) {
                MapEditor.Instance.Rerender |= Layer == ToolLayer.Background ? RenderFlags.BGTiles : RenderFlags.FGTiles;
            } else {
                MapEditor.Instance.Renderer.RenderRoom(DrawableRoom);
            }
        }

        public override bool Undo() {
            bool changed = false;

            foreach ((Point p, short t) in PreviousTiles) {
                if (SetPoint(p, t)) changed = true;
            }

            if (changed) Redraw();

            return changed;
        }

        protected bool SetPoint(Point p) {
            return SetPoint(p, Tileset);
        }

        protected bool SetPoint(Point p, short t) {
            bool res = Grid[p.X, p.Y] != t;
            Grid[p.X, p.Y] = t;

            if (res) Tiler.Update(DrawableRoom, Layer == ToolLayer.Foreground, p);
            return res;
        }
    }
}
