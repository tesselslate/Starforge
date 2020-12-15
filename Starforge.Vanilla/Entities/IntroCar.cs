using Microsoft.Xna.Framework;
using Starforge.Core;
using Starforge.MapStructure;
using Starforge.Mod;
using Starforge.Mod.Assets;

namespace Starforge.Vanilla.Entities {
    [EntityDefinition("introCar")]
    public class IntroCar : Entity {
        private static readonly DrawableTexture bodySprite = GFX.Gameplay["scenery/car/body"];
        private static readonly DrawableTexture wheelsSprite = GFX.Gameplay["scenery/car/wheels"];

        public IntroCar(Level level, EntityData data) : base(level, data) { }

        public override void Render() {
            wheelsSprite.Draw(new Vector2(Position.X + 2f - (wheelsSprite.Width / 2f) + 8f, Position.Y - wheelsSprite.Height / 3));
            bodySprite.Draw(new Vector2(Position.X - (bodySprite.Width / 2) + 8f, Position.Y - bodySprite.Height));
        }
    }
}
