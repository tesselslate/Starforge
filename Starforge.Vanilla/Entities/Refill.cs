using Microsoft.Xna.Framework;
using Starforge.Map;
using Starforge.Mod.API;
using Starforge.Mod.Content;

namespace Starforge.Vanilla.Entities {
    [EntityDefinition("refill")]
    public class Refill : Entity {
        private static DrawableTexture OneDashSprite;
        private static DrawableTexture TwoDashSprite;

        public Refill(EntityData data, Room room) : base(data, room) {
            OneDashSprite = GFX.Gameplay["objects/refill/idle00"];
            TwoDashSprite = GFX.Gameplay["objects/refillTwo/idle00"];
        }

        public override void Render() {
            DrawableTexture texture = GetBool("twoDash", false) ? TwoDashSprite : OneDashSprite;
            texture.DrawOutlineCentered(Position, Color.Black);
            texture.DrawCentered(Position);
        }

        public static PlacementList Placements = new PlacementList()
        {
            new Placement("Refill"),
            new Placement("Refill (Two Dashes)")
            {
                ["twoDash"] = true
            }
        };
    }
}