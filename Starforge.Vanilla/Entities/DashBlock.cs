using Microsoft.Xna.Framework;
using Starforge.Core;
using Starforge.MapStructure;
using Starforge.MapStructure.Tiling;
using Starforge.Mod;
using Starforge.Mod.Assets;
using Starforge.Util;

namespace Starforge.Vanilla.Entities {
    [EntityDefinition("dashBlock")]
    class DashBlock : Entity {
        public DashBlock(Level level, EntityData data) : base(level, data) {
            StretchableX = true;
            StretchableY = true;
        }

        public override void Render() {
            var grid = new TileGrid((int)Width/8, (int)Height/8);
            grid.Fill(GetChar("tiletype", '3'));
            var textures = Engine.Scene.FGAutotiler.GenerateTextureMap(grid, 0,0, false);
            for (int i = 0; i < textures.Length; i++)
            {
                textures[i].Position += Position;
                textures[i].Draw();
            }
        }
    }
}
