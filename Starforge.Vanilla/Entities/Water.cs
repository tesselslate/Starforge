using Microsoft.Xna.Framework;
using Starforge.Core;
using Starforge.MapStructure;
using Starforge.Mod;

namespace Starforge.Vanilla.Entities {
    [EntityDefinition("water")]
    public class Water : Entity {
        public static readonly Color FillColor = Color.LightSkyBlue * 0.3f;
        public static readonly Color SurfaceColor = Color.LightSkyBlue * 0.8f;

        public Water(Level level, EntityData data) : base(level, data) {
            StretchableX = true;
            StretchableY = true;
        }

        public override void Render() {
            Rectangle renderPos = new Rectangle((int)Position.X, (int)Position.Y, GetInt("width", 8), GetInt("height", 8));

            GFX.Pixel.Draw(renderPos, FillColor);
            GFX.Draw.HollowRectangle(renderPos, SurfaceColor);
        }
    }
}
