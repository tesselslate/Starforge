using Microsoft.Xna.Framework;
using Starforge.Map;
using Starforge.Mod.API;
using Starforge.Mod.Content;
using System;

namespace Starforge.Vanilla.Entities {
    [EntityDefinition("infiniteStar")]
    public class Feather : Entity {
        private static Lazy<DrawableTexture> Sprite = new Lazy<DrawableTexture>(() => GFX.Gameplay["objects/flyFeather/idle00"]);

        public Feather(EntityData data, Room room) : base(data, room) { }

        public override void Render() {
            Sprite.Value.DrawCentered(Position);
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