using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Starforge.Core;
using System;

namespace Starforge.Mod.Assets {
    public class DrawableTexture {
        public Vector2 Center { get; private set; }

        public Rectangle ClipRect { get; private set; }

        public Vector2 DrawOffset { get; private set; }

        public VirtualTexture Texture { get; private set; }

        public int Width { get; private set; }
        
        public int Height { get; private set; }

        // Used so that ClipRect isn't converted to a nullable rectangle on each draw call.
        private Rectangle? NullableClipRect;

        public DrawableTexture(VirtualTexture texture) {
            Texture = texture;
            ClipRect = new Rectangle(0, 0, Texture.Width, Texture.Height);
            DrawOffset = Vector2.Zero;
            Width = ClipRect.Width;
            Height = ClipRect.Height;
            Center = new Vector2(Width, Height) * 0.5f;
            NullableClipRect = new Rectangle?(ClipRect);
        }

        public DrawableTexture(VirtualTexture texture, int x, int y, int w, int h) {
            Texture = texture;
            ClipRect = new Rectangle(x, y, w, h);
            Width = w;
            Height = h;
            Center = new Vector2(Width, Height) * 0.5f;
            DrawOffset = Vector2.Zero;
            NullableClipRect = new Rectangle?(ClipRect);
        }
        public DrawableTexture(VirtualTexture texture, Rectangle clipRect, Vector2 drawOffset, int w, int h) {
            Texture = texture;
            ClipRect = clipRect;
            DrawOffset = drawOffset;
            Width = w;
            Height = h;
            Center = new Vector2(Width, Height) * 0.5f;
            NullableClipRect = new Rectangle?(ClipRect);
        }

        public DrawableTexture(VirtualTexture texture, Vector2 drawOffset, int w, int h) {
            Texture = texture;
            ClipRect = new Rectangle(0, 0, Texture.Width, Texture.Height);
            DrawOffset = drawOffset;
            Width = w;
            Height = h;
            Center = new Vector2(Width, Height) * 0.5f;
            NullableClipRect = new Rectangle?(ClipRect);
        }

        public DrawableTexture(DrawableTexture parent, int x, int y, int w, int h) {
            Texture = parent.Texture;
            ClipRect = parent.GetRelativeRect(x, y, w, h);
            DrawOffset = new Vector2(-Math.Min(x - parent.DrawOffset.X, 0f), -Math.Min(y - parent.DrawOffset.Y, 0f));
            Width = w;
            Height = h;
            Center = new Vector2(Width, Height) * 0.5f;
            NullableClipRect = new Rectangle?(ClipRect);
        }

        public void Draw(Vector2 position) {
            Engine.Batch.Draw(Texture.Texture, position, NullableClipRect, Color.White);
        }

        public void Draw(Rectangle destination) {
            Engine.Batch.Draw(Texture.Texture, destination, NullableClipRect, Color.White);
        }

        public void Draw(Rectangle destination, Color color) {
            Engine.Batch.Draw(Texture.Texture, destination, NullableClipRect, color);
        }

        public void DrawCentered(Vector2 position) {
            Engine.Batch.Draw(Texture.Texture, position, NullableClipRect, Color.White, 0f, Center - DrawOffset, 1f, SpriteEffects.None, 0f);
        }

        public Rectangle GetRelativeRect(int x, int y, int w, int h) {
            int x0 = (int)(ClipRect.X - DrawOffset.X + x);
            int y0 = (int)(ClipRect.Y - DrawOffset.Y + y);
            int x1 = (int)MathHelper.Clamp(x0, ClipRect.Left, ClipRect.Right);
            int y1 = (int)MathHelper.Clamp(y0, ClipRect.Top, ClipRect.Bottom);
            int w0 = Math.Max(0, Math.Min(x0 + w, ClipRect.Right) - x1);
            int h0 = Math.Max(0, Math.Min(y0 + h, ClipRect.Bottom) - y1);
            return new Rectangle(x1, y1, w0, h0);
        }
    }
}
