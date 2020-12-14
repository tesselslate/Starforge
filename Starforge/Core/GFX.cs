using Microsoft.Xna.Framework;
using Starforge.Mod.Assets;
using System.IO;

namespace Starforge.Core {
    public static class GFX {
        public static Atlas Gameplay;

        public static DrawableTexture Pixel;

        public static DrawableTexture Empty;

        private static Rectangle Rect;

        public static void Load() {
            Gameplay = Atlas.FromAtlas(Path.Combine(Engine.ContentDirectory, "Graphics", "Atlases/") + "Gameplay", AtlasFormat.Packer);

            Pixel = Gameplay["util/pixel"];
            Empty = new DrawableTexture(Gameplay.Sources[0], new Rectangle(4094, 4094, 1, 1), Vector2.Zero, 1, 1);
        }

        public static class Draw {
            public static void HollowRectangle(int x, int y, int w, int h, Color c) {
                Rect.X = x;
                Rect.Y = y;
                Rect.Width = w;
                Rect.Height = 1;
                Pixel.Draw(Rect, c);

                Rect.Y += h - 1;
                Pixel.Draw(Rect, c);

                Rect.Y = y;
                Rect.Width = 1;
                Rect.Height = h;
                Pixel.Draw(Rect, c);

                Rect.X += w - 1;
                Pixel.Draw(Rect, c);
            }

            public static void HollowRectangle(Rectangle r, Color c) {
                HollowRectangle(r.X, r.Y, r.Width, r.Height, c);
            }

            public static void HollowRectangle(int x, int y, int w, int h, int t, Color c) {
                Rect.X = x;
                Rect.Y = y;
                Rect.Width = w;
                Rect.Height = t;
                Pixel.Draw(Rect, c);

                Rect.Y += h - t;
                Pixel.Draw(Rect, c);

                Rect.Y = y;
                Rect.Width = t;
                Rect.Height = h;
                Pixel.Draw(Rect, c);

                Rect.X += w - t;
                Pixel.Draw(Rect, c);
            }

            public static void HollowRectangle(Rectangle r, int thickness, Color c) {
                HollowRectangle(r.X, r.Y, r.Width, r.Height, thickness, c);
            }
        }
    }
}
