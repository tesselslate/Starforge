using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Starforge.Core;
using Starforge.Util;
using System.IO;
using System.Reflection;

namespace Starforge.Mod.Content {
    /// <summary>
    /// Contains textures and other graphics content for the editor.
    /// </summary>
    public static class GFX {
        #region Atlases

        /// <summary>
        /// The gameplay texture atlas.
        /// </summary>
        public static Atlas Gameplay;

        #endregion

        #region Textures

        /// <summary>
        /// An empty texture.
        /// </summary>
        public static DrawableTexture Empty;

        /// <summary>
        /// A single pixel.
        /// </summary>
        public static DrawableTexture Pixel;

        /// <summary>
        /// The Starforge logo texture.
        /// </summary>
        public static Texture2D Logo { get; private set; }

        private static Rectangle Rect;

        #endregion

        /// <summary>
        /// Used during startup to load certain graphics. Most content is loaded in Core.Boot.TaskUnpackVanillaAtlas.
        /// </summary>
        public static void LoadContent() {
            using(Stream logoStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Starforge.logo_256.png")) {
                Logo = Texture2D.FromStream(Engine.Instance.GraphicsDevice, logoStream);
            }
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

            public static void Rectangle(Rectangle r, Color c) {
                Pixel.Draw(r, c);
            }

            public static void Rectangle(int x, int y, int w, int h, Color c) {
                Pixel.Draw(new Rectangle(x, y, w, h), c);
            }

            // resolution specifies how many sides the drawn polygon should have. The higher, the better the circle
            // high resolutions can make this function really slow!
            // "borrowed" from Celeste
            public static void Circle(Vector2 position, float radius, Color color, int resolution) {
                Vector2 angleVector = Vector2.UnitX * radius;
                Vector2 perpendicular = angleVector.Perpendicular();
                for (int i = 1; i <= resolution; i++) {
                    Vector2 nextAngleVector = MiscHelper.AngleToVector(i * MathHelper.PiOver2 / resolution, radius);
                    Vector2 nextPerpendicular = nextAngleVector.Perpendicular();
                    Line(position + angleVector, position + nextAngleVector, color);
                    Line(position - angleVector, position - nextAngleVector, color);
                    Line(position + perpendicular, position + nextPerpendicular, color);
                    Line(position - perpendicular, position - nextPerpendicular, color);
                    angleVector = nextAngleVector;
                    perpendicular = nextPerpendicular;
                }
            }

            public static void Line(Vector2 start, Vector2 end, Color color) {
                LineAngle(start, MiscHelper.Angle(start, end), Vector2.Distance(start, end), color);
            }

            public static void LineAngle(Vector2 start, float angle, float length, Color color) {
                Engine.Batch.Draw(Pixel.Texture.Texture, start, Pixel.ClipRect, color, angle, Vector2.Zero, new Vector2(length, 1f), SpriteEffects.None, 0f);
            }
        }

    }
}
