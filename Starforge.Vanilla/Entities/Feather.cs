using Microsoft.Xna.Framework;
using Starforge.Map;
using Starforge.Mod.API;
using Starforge.Mod.Content;
using Starforge.Util;
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

        public override Rectangle Hitbox => MiscHelper.RectangleCentered(Position, Sprite.Value.Width, Sprite.Value.Height);

        public static PlacementList Placements = new PlacementList()
        {
            new Placement("Feather"),
            new Placement("Feather (Shielded)")
            {
                ["shielded"] = true
            }
        };

        public override PropertyList Properties => new PropertyList() {
            new Property("shielded", PropertyType.Bool, "Whether this feather has a shield or not")
        };
    }
}
