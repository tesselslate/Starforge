using Microsoft.Xna.Framework;
using Starforge.Map;
using Starforge.Mod.API;
using Starforge.Mod.Content;

namespace Starforge.Vanilla.Entities {
    [EntityDefinition("infiniteStar")]
    public class Feather : Entity {
        private static DrawableTexture Sprite;

        public Feather(EntityData data, Room room) : base(data, room) {
            Sprite = GFX.Gameplay["objects/flyFeather/idle00"];
        }

        public override void Render() {
            Sprite.DrawCentered(Position);
            if (GetBool("shielded", false)) {
                GFX.Draw.Circle(Position, 10f, Color.White, 3);
            }
        }

        public static PlacementList Placements = new PlacementList()
        {
            new Placement("Feather"),
            new Placement("Feather (Shielded)")
            {
                ["shielded"] = true
            }
        };
    }
}