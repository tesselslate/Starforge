using Microsoft.Xna.Framework;
using Starforge.Map;
using Starforge.Mod.API;
using Starforge.Mod.Content;
using System.Collections.Generic;
using System.Linq;

namespace Starforge.Vanilla.Entities {
    [EntityDefinition("waterfall")]
    public class Waterfall : Entity {
        public Waterfall(EntityData data, Room room) : base(data, room) { }

        public override void Render() {
            int x = (int)Position.X, y = (int)Position.Y;
            int height = CalculateHeight();

            // middle
            GFX.Draw.Rectangle(x + 1, y, 6, height, Water.FillColor);
            // sides
            GFX.Draw.Rectangle(x - 1, y, 2, height, Water.SurfaceColor);
            GFX.Draw.Rectangle(x + 7, y, 2, height, Water.SurfaceColor);
        }

        private int CalculateHeight() {
            int x = (int)Position.X, y = (int)Position.Y;
            int height = 0;

            // <Height, Y>
            List<Vector2> waterTuples = new List<Vector2>();
            foreach (var water in Room.Entities.OfType<Water>()) {
                // find all water that we have a chance of colliding with
                int waterWidth = water.Width;
                if (x >= water.Position.X && water.Position.X + waterWidth > x)
                    waterTuples.Add(new Vector2(water.Height, water.Position.Y));
            }

            int nextY = y;
            while (nextY < Room.Height && Room.ForegroundTiles[x / 8, nextY / 8] == TileGrid.TILE_AIR) {
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

        public static PlacementList Placements = new PlacementList()
        {
            new Placement("Waterfall")
        };
    }
}
