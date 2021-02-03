using Microsoft.Xna.Framework;
using Starforge.Map;
using Starforge.Mod.API;
using Starforge.Mod.Content;
using Starforge.Util;

namespace Starforge.Vanilla.Entities {
    [EntityDefinition("jumpThru")]
    public class Jumpthru : Entity {
        public Jumpthru(EntityData data, Room room) : base(data, room) { }

        public override bool StretchableX => true;

        public override void Render() {
            string type = GetString("texture", "wood");
            DrawableTexture baseTexture = GFX.Gameplay["objects/jumpthru/" + (type == "default" ? "wood" : type)];
            int textureTiles = baseTexture.Width / 8;

            int width = GetInt("width", 8);
            int columns = width / 8;

            for (int i = 0; i < columns; i++) {
                int xOffset, yOffset;

                if (i == 0) {
                    // left side
                    xOffset = 0;
                    yOffset = Room.ForegroundTiles[((int)Position.X / 8) - 1, (int)Position.Y / 8] == TileGrid.TILE_AIR ? 1 : 0;
                } else if (i == columns - 1) {
                    // right side
                    xOffset = textureTiles - 1;
                    yOffset = Room.ForegroundTiles[((int)Position.X + width) / 8, (int)Position.Y / 8] == TileGrid.TILE_AIR ? 1 : 0;
                } else {
                    // middle
                    xOffset = 1 + MiscHelper.RandInt(i, textureTiles - 2);
                    yOffset = MiscHelper.RandInt(i, 2);
                }
                // Draw the sprite
                new DrawableTexture(baseTexture, xOffset * 8, yOffset * 8, 8, 8).Draw(new Vector2(Position.X + (i * 8), Position.Y));
            }
        }

        public static PlacementList Placements = new PlacementList()
        {
            new Placement("Jump Through (Cliffside)")
            {
                ["texture"] = "cliffside"
            },
            new Placement("Jump Through (Core)")
            {
                ["texture"] = "core"
            },
            new Placement("Jump Through (Dream)")
            {
                ["texture"] = "dream"
            },
            new Placement("Jump Through (Moon)")
            {
                ["texture"] = "moon"
            },
            new Placement("Jump Through (Reflection)")
            {
                ["texture"] = "reflection"
            },
            new Placement("Jump Through (Temple)")
            {
                ["texture"] = "temple"
            },
            new Placement("Jump Through (Temple B)")
            {
                ["texture"] = "templeB"
            },
            new Placement("Jump Through (Wood)")
            {
                ["texture"] = "wood"
            }
        };

        public override PropertyList Properties => new PropertyList() {
            new Property("texture", new string[]{"cliffside", "core", "dream", "moon", "reflection", "temple", "templeB", "wood"}, "The style the Jumpthru has"),
            new Property("surfaceIndex", PropertyType.Integer, "")
        };
    }
}
