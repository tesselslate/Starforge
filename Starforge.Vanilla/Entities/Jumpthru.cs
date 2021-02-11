using Microsoft.Xna.Framework;
using Starforge.Map;
using Starforge.Mod.API;
using Starforge.Mod.API.Properties;
using Starforge.Mod.Content;
using Starforge.Util;
using System.Collections.Generic;

namespace Starforge.Vanilla.Entities {
    [EntityDefinition("jumpThru")]
    public class Jumpthru : Entity {
        private readonly static Dictionary<string, DrawableTexture[,]> sliceCache = new Dictionary<string, DrawableTexture[,]>();
        public Jumpthru(EntityData data, Room room) : base(data, room) { }

        public override bool StretchableX => true;

        public override void Render() {
            string type = GetString("texture", "wood");
            DrawableTexture[,] slices = GetSlices(type == "default" ? "wood" : type);
            if (slices == null)
                return;
            int textureHorizontalTiles = slices.GetLength(0);

            int width = GetInt("width", 8);
            int columns = width / 8;

            // left side:
            slices[0, Room.ForegroundTiles[((int)Position.X / 8) - 1, (int)Position.Y / 8] == TileGrid.TILE_AIR ? 1 : 0].Draw(Position);
            // right side:
            if (columns > 1)
                slices[textureHorizontalTiles - 1, Room.ForegroundTiles[((int)Position.X + width) / 8, (int)Position.Y / 8] == TileGrid.TILE_AIR ? 1 : 0].Draw(new Vector2(Position.X + width - 8, Position.Y));
            // middle
            Vector2 drawPos = new Vector2(Position.X + 8, Position.Y);
            for (int i = 1; i < columns - 1; i++) {
                int sliceX = 1 + MiscHelper.RandInt(i, textureHorizontalTiles - 2);
                int sliceY = MiscHelper.RandInt(i, 2);
                slices[sliceX, sliceY].Draw(drawPos);
                //slices[1, 0].Draw(drawPos);
                drawPos.X += 8;
            }
        }

        private DrawableTexture[,] GetSlices(string path) {
            if (sliceCache.TryGetValue(path, out DrawableTexture[,] slices))
                return slices;

            DrawableTexture baseTexture = GFX.Gameplay["objects/jumpthru/" + path];
            if (baseTexture == GFX.Empty)
                return null;
            int width = baseTexture.Width / 8, height = baseTexture.Height / 8;

            slices = new DrawableTexture[width, height];
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    slices[i, j] = new DrawableTexture(baseTexture, i * 8, j * 8, 8, 8);

            sliceCache.Add(path, slices);
            return slices;
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
            new ListProperty("texture", new string[]{"cliffside", "core", "dream", "moon", "reflection", "temple", "templeB", "wood"}, true, "wood", "The style the Jumpthru has"),
            new IntProperty("surfaceIndex", 0, "")
        };
    }
}
