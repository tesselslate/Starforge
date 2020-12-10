using Microsoft.Xna.Framework;
using Starforge.Core;
using Starforge.MapStructure;
using Starforge.Mod;
using Starforge.Mod.Assets;

namespace Starforge.Vanilla.Entities {
    [EntityDefinition("player")]
    public class Player : Entity {
        private static DrawableTexture Sprite = GFX.Gameplay["characters/player/sitDown00"];

        public Player(Level level, EntityData data) : base(level, data) { }

        public override void Render() {
            Sprite.DrawCentered(new Vector2(X, Y - 16));
        }
    }
}