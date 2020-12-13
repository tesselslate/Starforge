using Microsoft.Xna.Framework;
using Starforge.MapStructure;
using System;
using System.Collections.Generic;

namespace Starforge.Editor.Actions {

    public class DrawTilePlacement : TilePlacement {

        private HashSet<Point> Points;

        public DrawTilePlacement(Level l, TileType t, int tileset, Point p)
            : base(l, t, tileset) {
            Points = new HashSet<Point>();
            AddPoint(p);
        }

        public void AddPoint(Point p) {
            if (Points.Contains(p)) {
                return;
            }
            if (p.X < 0 || p.Y < 0 || p.X > Grid.Width - 1 || p.Y > Grid.Height - 1) {
                return;
            }

            PreviousTiles.Add((p, Grid[p.X, p.Y]));
            Points.Add(p);
            if (SetPoint(p)) {
                Level.Dirty = true;
            }
        }

        public override bool Apply() {
            bool changed = false;
            foreach (Point p in Points) {
                if (SetPoint(p)) {
                    changed = true;
                }
            }
            return changed;
        }

        public override ToolType GetToolType() {
            return ToolType.TileDraw;
        }

    }
}
