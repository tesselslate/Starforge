using Microsoft.Xna.Framework;
using Starforge.Core;
using Starforge.MapStructure;
using Starforge.Mod;
using Starforge.Mod.Assets;

namespace Starforge.Vanilla.Entities {
    [EntityDefinition("spinner")]
    public class CrystalSpinner : Entity {
        private DrawableTexture Sprite;
        public CrystalSpinner(Level level, EntityData data) : base(level, data) {
            if(data.GetBool("dust")) {
                // Dust bunny
            } else {
                switch(data.GetString("color").ToLower()) {
                    case "red":
                        Sprite = GFX.Gameplay["danger/crystal/fg_red03"];
                        break;
                    case "purple":
                        Sprite = GFX.Gameplay["danger/crystal/fg_purple03"];
                        break;
                    case "rainbow":
                        Sprite = GFX.Gameplay["danger/crystal/fg_white03"];
                        break;
                    default:
                        Sprite = GFX.Gameplay["danger/crystal/fg_blue03"];
                        break;
                }
            }

            Sprite.PregeneratedPosition = new Vector2(X, Y);
        }

        public override void Render() {
            Sprite.PregeneratedDrawCentered();
        }
    }
}
