using Microsoft.Xna.Framework;
using Starforge.Core;
using Starforge.MapStructure;
using Starforge.Mod;
using Starforge.Mod.Assets;

namespace Starforge.Vanilla.Entities {
    [EntityDefinition("spinner")]
    public class CrystalSpinner : Entity {
        private StaticTexture Sprite;
        public CrystalSpinner(Level level, EntityData data) : base(level, data) {
            DrawableTexture tex = GFX.Gameplay["danger/crystal/fg_blue03"];

            if(data.GetBool("dust")) {
                // Dust bunny
            } else {
                switch(data.GetString("color").ToLower()) {
                    case "red":
                        tex = GFX.Gameplay["danger/crystal/fg_red03"];
                        break;
                    case "purple":
                        tex = GFX.Gameplay["danger/crystal/fg_purple03"];
                        break;
                    case "rainbow":
                        tex = GFX.Gameplay["danger/crystal/fg_white03"];
                        break;
                    default:
                        tex = GFX.Gameplay["danger/crystal/fg_blue03"];
                        break;
                }
            }

            Sprite = new StaticTexture(
                tex,
                Position
            );
        }

        public override void Render() {
            Sprite.DrawCentered();
        }
    }
}
