using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Starforge.Mod.Assets {
    public class DrawableTexture {
        public Vector2 Center { get; private set; }

        public Rectangle ClipRect { get; private set; }

        public Vector2 DrawOffset { get; private set; }

        public VirtualTexture Texture { get; private set; }

        public int Width { get; private set; }
        
        public int Height { get; private set; }

        public DrawableTexture(VirtualTexture texture) {
            Texture = texture;
            ClipRect = new Rectangle(0, 0, Texture.Width, Texture.Height);
            DrawOffset = Vector2.Zero;
            Width = ClipRect.Width;
            Height = ClipRect.Height;
            Center = new Vector2(Width, Height) * 0.5f;
        }

        public DrawableTexture(VirtualTexture texture, int x, int y, int w, int h) {
            Texture = texture;
            ClipRect = new Rectangle(x, y, w, h);
            Width = w;
            Height = h;
            Center = new Vector2(Width, Height) * 0.5f;
        }
        public DrawableTexture(VirtualTexture texture, Rectangle clipRect, Vector2 drawOffset, int w, int h) {
            Texture = texture;
            ClipRect = clipRect;
            DrawOffset = drawOffset;
            Width = w;
            Height = h;
            Center = new Vector2(Width, Height) * 0.5f;
        }

        public DrawableTexture(VirtualTexture texture, Vector2 drawOffset, int w, int h) {
            Texture = texture;
            ClipRect = new Rectangle(0, 0, Texture.Width, Texture.Height);
            DrawOffset = drawOffset;
            Width = w;
            Height = h;
            Center = new Vector2(Width, Height) * 0.5f;
        }
    }
}
