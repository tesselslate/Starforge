using Starforge.Core;
using Starforge.MapStructure;
using Starforge.Mod;
using Starforge.Mod.Assets;

namespace Starforge.Vanilla.Entities
{
    [EntityDefinition("infiniteStar")]
    public class Feather : Entity
    {
        private static readonly DrawableTexture Sprite = GFX.Gameplay["objects/flyFeather/idle00"];

        public Feather(Level level, EntityData data) : base(level, data) { }

        public override void Render()
        {
            Sprite.DrawCentered(Position);
            if (GetBool("shielded", false)) {
                // TODO: Circle
                //Draw.Circle(Position, 10f, Color.White, 3);
            }
        }
    }
}

