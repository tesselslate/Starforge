using Microsoft.Xna.Framework;
using Starforge.Editor.Tools;
using Starforge.Map;
using System.Collections.Generic;

namespace Starforge.Editor.Actions {
    public class TileBrushAction : TileAction {
        private HashSet<Point> Points;

        public TileBrushAction(Room r, ToolLayer l, int t, Point p) : base(r, l, t) {
            Points = new HashSet<Point>();
            AddPoint(p);
        }

        public void AddPoint(Point p) {
            if (Points.Contains(p)) return;
            if (p.X < 0 || p.Y < 0 || p.X < Grid.Width - 1 || p.Y < Grid.Height - 1) return;

            PreviousTiles.Add((p, Grid[p.X, p.Y]));
            Points.Add(p);
            SetPoint(p);
        }

        public override bool Apply() {
            bool changed = false;
            foreach (Point p in Points) {
                if (SetPoint(p)) changed = true;
            }

            if (changed) Redraw();
            return changed;
        }

        public override ToolType GetToolType() => ToolType.TileBrush;
    }
}
