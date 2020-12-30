using Microsoft.Xna.Framework;
using Starforge.Map;
using Starforge.Mod.API;
using Starforge.Mod.Content;

namespace Starforge.Vanilla.Entities {
    [EntityDefinition("introCar")]
    public class IntroCar : Entity {
        private static DrawableTexture bodySprite;
        private static DrawableTexture wheelsSprite;

        public IntroCar(EntityData data, Room room) : base(data, room) {
            bodySprite = GFX.Gameplay["scenery/car/body"];
            wheelsSprite = GFX.Gameplay["scenery/car/wheels"];
        }

        public override void Render() {
            wheelsSprite.Draw(new Vector2(Position.X + 2f - (wheelsSprite.Width / 2f) + 8f, Position.Y - wheelsSprite.Height / 3));
            bodySprite.Draw(new Vector2(Position.X - (bodySprite.Width / 2) + 8f, Position.Y - bodySprite.Height));
        }

        public static PlacementList Placements = new PlacementList()
        {
            new Placement("Intro Car")
        };
    }
}