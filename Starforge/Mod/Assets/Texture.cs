using System.Drawing;

namespace Starforge.Mod.Assets {
    public class Texture {
        public string Name;
        public int Width;
        public int Height;

        public Bitmap Image;

        public Texture(string name, int w, int h) {
            Name = name;
            Width = w;
            Height = h;
        }
    }

    public class PackerTexture : Texture {
        public Atlas Parent;
        public string DataPath;
        public int X;
        public int Y;
        public int FrameX;
        public int FrameY;
        public int FrameWidth;
        public int FrameHeight;

        public PackerTexture(Atlas parent, string name, string path, int x, int y, int w, int h, int fx, int fy, int fw, int fh) : base(name, w, h) { }
    }
}
