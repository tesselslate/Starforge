using Microsoft.Xna.Framework;
using Starforge.Core;
using Starforge.MapStructure;
using Starforge.Mod;
using Starforge.Mod.Assets;
using Starforge.Util;

namespace Starforge.Vanilla.Entities {
    [EntityDefinition("jumpThru")]
    public class Jumpthru : Entity {
        public Jumpthru(Level level, EntityData data) : base(level, data) {
            StretchableX = true;
        }

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
                    yOffset = Level.ForegroundTiles.GetTile(((int)Position.X / 8) - 1, (int)Position.Y / 8) == '0' ? 1 : 0;
                }
                else if (i == columns - 1) {
                    // right side
                    xOffset = textureTiles - 1;
                    yOffset = Level.ForegroundTiles.GetTile(((int)Position.X + width) / 8, (int)Position.Y / 8) == '0' ? 1 : 0;
                }
                else {
                    // middle
                    xOffset = 1 + MiscHelper.Rand.Next(textureTiles - 2);
                    yOffset = MiscHelper.Rand.Next(0, 2);
                }
                // Draw the sprite
                new DrawableTexture(baseTexture, xOffset * 8, yOffset * 8, 8, 8).Draw(new Vector2(Position.X + (i * 8), Position.Y));
            }
        }
    }
}