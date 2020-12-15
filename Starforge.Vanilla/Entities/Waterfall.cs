using Microsoft.Xna.Framework;
using Starforge.Core;
using Starforge.MapStructure;
using Starforge.Mod;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Starforge.Vanilla.Entities {
    [EntityDefinition("waterfall")]
    public class Waterfall : Entity {
        public Waterfall(Level level, EntityData data) : base(level, data) { }

        public override void Render() {
            int x = (int)Position.X, y = (int)Position.Y;
            int height = calculateHeight();

            // middle
            GFX.Draw.Rectangle(x + 1, y, 6, height, Water.FillColor);
            // sides
            GFX.Draw.Rectangle(x - 1, y, 2, height, Water.SurfaceColor);
            GFX.Draw.Rectangle(x + 7, y, 2, height, Water.SurfaceColor);
        }

        private int calculateHeight() {
            int x = (int)Position.X, y = (int)Position.Y;
            int height = 0;

            // <Height, Y>
            List<Vector2> waterTuples = new List<Vector2>();
            foreach (var water in Level.Entities.OfType<Water>()) {
                // find all water that we have a chance of colliding with
                int waterWidth = (int)water.Width;
                if (x >= water.Position.X && water.Position.X + waterWidth > x)
                    waterTuples.Add(new Vector2(water.Height, water.Position.Y));
            }

            int nextY = y;
            while (nextY < Level.Height && Level.ForegroundTiles.GetTile(x / 8, (nextY) / 8) == '0') {
                foreach (var collider in waterTuples) {
                    // collider.X is actually the water's height in this case
                    if (nextY >= collider.Y && collider.Y + collider.X > nextY)
                        return height;
                }
                height += 8;
                nextY = y + height;
            }
            return height;
        }
    }
}
