using Microsoft.Xna.Framework;
using Starforge.Core;
using Starforge.MapStructure;
using Starforge.MapStructure.Tiling;
using Starforge.Mod.Assets;
using System.Collections.Generic;

namespace Starforge.Editor.Actions {

    public abstract class TilePlacement : Action {

        protected TileType Tile { get; }
        protected int TileSet;
        protected Autotiler Tiler;
        protected StaticTexture[] Textures;
        protected TileGrid Grid;

        // a list remembering what was overwritten so it can be undone
        protected List<(Point, int)> PreviousTiles;

        public TilePlacement(Level l, TileType t, int tileset)
            : base(l) {
            Tile = t;
            TileSet = tileset;
            switch (t) {
            case TileType.Foreground:
                Tiler = Engine.Scene.FGAutotiler;
                Textures = l.FgGrid;
                Grid = l.ForegroundTiles;
                break;
            case TileType.Background:
                Tiler = Engine.Scene.BGAutotiler;
                Textures = l.BgGrid;
                Grid = l.BackgroundTiles;
                break;
            }

            PreviousTiles = new List<(Point, int)>();
        }

        public abstract ToolType GetToolType();

        public override bool Undo() {
            bool changed = false;
            foreach ((Point p, int tileID) in PreviousTiles) {
                if (SetPointByID(p, tileID)) {
                    changed = true;
                }
            }
            return changed;
        }

        protected bool SetPoint(Point p) {
            return SetPoint(p, TileSet);
        }

        protected bool SetPoint(Point p, int tile) {
            if (tile == 0) {
                return SetPointByID(p, '0');
            }

            return SetPointByID(p, Tiler.GetTilesetList()[tile - 1].ID);
        }

        protected bool SetPointByID(Point p, int ID) {
            Grid.SetTile(p.X, p.Y, ID);

            return Tiler.Update(Grid, Textures, p);
        }
    }
}
