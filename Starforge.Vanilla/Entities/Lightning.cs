using Microsoft.Xna.Framework;
using Starforge.Core;
using Starforge.MapStructure;
using Starforge.Mod;
using Starforge.Util;

namespace Starforge.Vanilla.Entities
{
    [EntityDefinition("lightning")]
    public class Lightning : Entity {
        private static readonly Color BgColor = MiscHelper.HexToColor("fcf579") * 0.2f;
        private static readonly Color OutlineColor = MiscHelper.HexToColor("fcf579");
        public Lightning(Level level, EntityData data) : base(level, data) { }

        public override void Render() {
            Rectangle renderPos = new Rectangle((int)Position.X, (int)Position.Y, GetInt("width", 8), GetInt("height", 8));

            GFX.Pixel.Draw(renderPos, BgColor);
            GFX.Draw.HollowRectangle(renderPos, OutlineColor);
        }
    }
}