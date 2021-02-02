using Microsoft.Xna.Framework;
using Starforge.Map;
using Starforge.Mod.API;
using Starforge.Mod.Content;
using Starforge.Util;
using System;

namespace Starforge.Vanilla.Entities {
    [EntityDefinition("spinner")]
    public class CrystalSpinner : Entity {
        private static Lazy<DrawableTexture> DustSprite = new Lazy<DrawableTexture>(() => GFX.Gameplay["danger/dustcreature/base00"]);
        private static Lazy<DrawableTexture> RedSprite = new Lazy<DrawableTexture>(() => GFX.Gameplay["danger/crystal/fg_red03"]);
        private static Lazy<DrawableTexture> PurpleSprite = new Lazy<DrawableTexture>(() => GFX.Gameplay["danger/crystal/fg_purple03"]);
        private static Lazy<DrawableTexture> RainbowSprite = new Lazy<DrawableTexture>(() => GFX.Gameplay["danger/crystal/fg_white03"]);
        private static Lazy<DrawableTexture> BlueSprite = new Lazy<DrawableTexture>(() => GFX.Gameplay["danger/crystal/fg_blue03"]);

        public CrystalSpinner(EntityData data, Room room) : base(data, room) { }

        public override void Render() {
            DrawableTexture Texture;
            if (GetBool("dust")) {
                Texture = DustSprite.Value;
            }
            else {
                switch (GetString("color").ToLower()) {
                case "red":
                    Texture = RedSprite.Value;
                    break;
                case "purple":
                    Texture = PurpleSprite.Value;
                    break;
                case "rainbow":
                    Texture = RainbowSprite.Value;
                    break;
                default:
                    Texture = BlueSprite.Value;
                    break;
                }
            }
            Texture.DrawCentered(Position);
        }

        public override Rectangle Hitbox => MiscHelper.RectangleCentered(Position, RedSprite.Value.Width, RedSprite.Value.Height);

        public static PlacementList Placements = new PlacementList()
        {
            new Placement("Crystal Spinner (Blue)")
            {
                ["color"] = "blue"
            },
            new Placement("Crystal Spinner (Purple)")
            {
                ["color"] = "purple"
            },
            new Placement("Crystal Spinner (Rainbow)")
            {
                ["color"] = "rainbow"
            },
            new Placement("Crystal Spinner (Red)")
            {
                ["color"] = "red"
            },
            new Placement("Dust Sprite")
            {
                ["dust"] = true
            }
        };

        public override PropertyList Properties => new PropertyList() {
            new Property("color", PropertyType.String, "The color of the spinner"),
            new Property("dust", PropertyType.Bool, "Whether this is a dust bunny or speen")
        };
    }
}
