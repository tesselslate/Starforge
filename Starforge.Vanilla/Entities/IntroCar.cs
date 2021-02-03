using Microsoft.Xna.Framework;
using Starforge.Map;
using Starforge.Mod.API;
using Starforge.Mod.Content;
using System;

namespace Starforge.Vanilla.Entities {
    [EntityDefinition("introCar")]
    public class IntroCar : Entity {
        private static Lazy<DrawableTexture> bodySprite = new Lazy<DrawableTexture>(() => GFX.Gameplay["scenery/car/body"]);
        private static Lazy<DrawableTexture> wheelsSprite = new Lazy<DrawableTexture>(() => GFX.Gameplay["scenery/car/wheels"]);

        public IntroCar(EntityData data, Room room) : base(data, room) { }

        public override void Render() {
            wheelsSprite.Value.Draw(new Vector2(Position.X + 2f - (wheelsSprite.Value.Width / 2f) + 8f, Position.Y - wheelsSprite.Value.Height / 3));
            bodySprite.Value.Draw(new Vector2(Position.X - (bodySprite.Value.Width / 2) + 8f, Position.Y - bodySprite.Value.Height));
        }

        public override Rectangle Hitbox => new Rectangle(
            (int)(Position.X - (bodySprite.Value.Width / 2) + 8f),
            (int)(Position.Y - bodySprite.Value.Height),
            bodySprite.Value.Width,
            bodySprite.Value.Height
        );

        public static PlacementList Placements = new PlacementList()
        {
            new Placement("Intro Car")
        };

        public override PropertyList Properties => new PropertyList() {
            new Property("hasRoadAndBarriers", PropertyType.Bool, "")
        };
    }
}
