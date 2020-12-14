using Microsoft.Xna.Framework;
using Starforge.Core;
using Starforge.MapStructure;
using Starforge.Mod;
using Starforge.Mod.Assets;

namespace Starforge.Vanilla.Entities {
    [EntityDefinition("refill")]
    public class Refill : Entity {
        private static readonly DrawableTexture OneDashSprite = GFX.Gameplay["objects/refill/idle00"];
        private static readonly DrawableTexture TwoDashSprite = GFX.Gameplay["objects/refillTwo/idle00"];

        public Refill(Level level, EntityData data) : base(level, data) { }

        public override void Render() {
            DrawableTexture texture = GetBool("twoDash", false) ? TwoDashSprite : OneDashSprite;
            texture.DrawOutlineCentered(Position, Color.Black);
            texture.DrawCentered(Position);
        }
    }
}
