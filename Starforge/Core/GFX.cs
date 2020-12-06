using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Starforge.Mod.Assets;
using System.IO;

namespace Starforge.Core {
    public static class GFX {
        public static Atlas Gameplay;

        public static DrawableTexture Pixel;

        public static void Load() {
            Gameplay = Atlas.FromAtlas(Path.Combine(Engine.Config.ContentDirectory, "Graphics", "Atlases/") + "Gameplay", AtlasFormat.Packer);

            Texture2D px = new Texture2D(Engine.Instance.GraphicsDevice, 1, 1);
            px.SetData(new Color[] { Color.White });
            Pixel = new DrawableTexture(new VirtualTexture(px));
        }
    }
}
