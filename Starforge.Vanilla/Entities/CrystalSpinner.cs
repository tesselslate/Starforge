using Microsoft.Xna.Framework;
using Starforge.Map;
using Starforge.Mod.API;
using Starforge.Mod.API.Properties;
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
            new Placement("Crystal Spinner (Core)")
            {
                ["color"] = "core"
            },
            new Placement("Dust Sprite")
            {
                ["dust"] = true
            }
        };

        private string[] spinnerColors = new string[] { "blue", "purple", "rainbow", "red", "core" };

        public override PropertyList Properties => new PropertyList() {
            new ListProperty("color", spinnerColors, false, "blue", "The color of the spinner"),
            new BoolProperty("dust", false, "Whether this is a dust bunny or spinner"),
            new BoolProperty("attachToSolid", false, "Whether to attach this to a solid in range")
        };
    }
}
