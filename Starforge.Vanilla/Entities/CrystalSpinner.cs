using Starforge.Map;
using Starforge.Mod.API;
using Starforge.Mod.Content;

namespace Starforge.Vanilla.Entities {
    [EntityDefinition("spinner")]
    public class CrystalSpinner : Entity {
        private DrawableTexture Texture;

        public CrystalSpinner(EntityData data, Room room) : base(data, room) {
            if (GetBool("dust")) {
                Texture = GFX.Gameplay["danger/dustcreature/base00"];
            } else {
                switch (GetString("color").ToLower()) {
                case "red":
                    Texture = GFX.Gameplay["danger/crystal/fg_red03"];
                    break;
                case "purple":
                    Texture = GFX.Gameplay["danger/crystal/fg_purple03"];
                    break;
                case "rainbow":
                    Texture = GFX.Gameplay["danger/crystal/fg_white03"];
                    break;
                default:
                    Texture = GFX.Gameplay["danger/crystal/fg_blue03"];
                    break;
                }
            }
        }

        public override void Render() {
            Texture.DrawCentered(Position);
        }

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
    }
}