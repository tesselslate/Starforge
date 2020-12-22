using Microsoft.Xna.Framework;
using Starforge.Mod.Content;
using System.IO;

namespace Starforge.Core.Boot {
    public class TaskUnpackVanillaAtlas : BootTask {
        public override void Run() {
            GFX.Gameplay = Atlas.FromAtlas(Path.Combine(Settings.CelesteDirectory, "Content", "Graphics", "Atlases", "Gameplay"), AtlasFormat.Packer);
            GFX.Empty = new DrawableTexture(GFX.Gameplay.Sources[0], new Rectangle(4094, 4094, 1, 1), Vector2.Zero, 1, 1);
            GFX.Pixel = new DrawableTexture(GFX.Gameplay.Sources[0], new Rectangle(13, 13, 1, 1), Vector2.Zero, 1, 1);

            base.Run();
        }
    }
}
