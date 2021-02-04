using Microsoft.Xna.Framework;
using Starforge.Map;
using Starforge.Mod.API;
using Starforge.Mod.Content;
using Starforge.Util;
using System;

namespace Starforge.Vanilla.Entities {
    [EntityDefinition("refill")]
    public class Refill : Entity {
        private static Lazy<DrawableTexture> OneDashSprite = new Lazy<DrawableTexture>(() => GFX.Gameplay["objects/refill/idle00"]);
        private static Lazy<DrawableTexture> TwoDashSprite = new Lazy<DrawableTexture>(() => GFX.Gameplay["objects/refillTwo/idle00"]);

        public Refill(EntityData data, Room room) : base(data, room) { }

        public override void Render() {
            DrawableTexture texture = GetBool("twoDash", false) ? TwoDashSprite.Value : OneDashSprite.Value;
            texture.DrawOutlineCentered(Position, Color.Black);
            texture.DrawCentered(Position);
        }
        public override Rectangle Hitbox => MiscHelper.RectangleCentered(Position, OneDashSprite.Value.Width, OneDashSprite.Value.Height);

        public static PlacementList Placements = new PlacementList() {
            new Placement("Refill"),
            new Placement("Refill (Two Dashes)")
            {
                ["twoDash"] = true
            }
        };

        public override PropertyList Properties => new PropertyList() {
                new Property("twoDash", PropertyType.Bool, "Whether this is a two dash or normal crystal"),
                new Property("oneUse", PropertyType.Bool, "Whether this dash refill is single use or will respawn")
        };
    }
}
