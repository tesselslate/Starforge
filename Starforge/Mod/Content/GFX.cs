using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Starforge.Core;
using Starforge.Editor;
using Starforge.Util;
using System;
using System.Collections.Generic;
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
        /// The PICO-8 font.
        /// </summary>
        public static DrawableTexture[] Font;

        /// <summary>
        /// A single pixel.
        /// </summary>
        public static DrawableTexture Pixel;

        /// <summary>
        /// The Scenery (object tiles) tileset.
        /// </summary>
        public static Tileset Scenery;

        /// <summary>
        /// The Starforge logo texture.
        /// </summary>
        public static Texture2D Logo { get; private set; }

        #endregion

        /// <summary>
        /// Used during startup to load certain graphics. Most content is loaded in Core.Boot.TaskUnpackVanillaAtlas.
        /// </summary>
        public static void LoadContent() {
            using (Stream logoStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Starforge.logo_256.png")) {
                Logo = Texture2D.FromStream(Engine.Instance.GraphicsDevice, logoStream);
            }
        }

        /// <summary>
        /// Used to draw shapes and text.
        /// </summary>
        public static class Draw {
            private static Rectangle Rect;
            private static string FontMap = "abcdefghijklmnopqrstuvwxyz0123456789~!@#4%^&*()_+-=?:.";

            /// <summary>
            /// Draws a rectangle with the specified fill color and border color.
            /// </summary>
            /// <param name="r">The rectangle to draw.</param>
            /// <param name="color">The color to fill the rectangle with.</param>
            /// <param name="border">The color to draw the border of the rectangle with.</param>
            public static void BorderedRectangle(Rectangle r, Color color, Color border) {
                Rectangle(r, color);
                HollowRectangle(r, border);
            }

            /// <summary>
            /// Draws a circle.
            /// </summary>
            /// <param name="position">The position of the circle.</param>
            /// <param name="radius">The radius of the circle.</param>
            /// <param name="color">The color of the circle.</param>
            /// <param name="resolution">The amount of sides the circle should have. More sides results in better quality, but slower drawing.</param>
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

            /// <summary>
            /// Draws a hollow rectangle with the specified color and dimensions.
            /// </summary>
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

            /// <summary>
            /// Draws a hollow rectangle with the specified color and dimensions.
            /// </summary>
            /// <param name="r">The rectangle to draw.</param>
            /// <param name="c">The color to draw with.</param>
            public static void HollowRectangle(Rectangle r, Color c) {
                HollowRectangle(r.X, r.Y, r.Width, r.Height, c);
            }

            /// <summary>
            /// Draws a hollow rectangle with the specified color, thickness, and dimensions.
            /// </summary>
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

            /// <summary>
            /// Draws a hollow rectangle with the specified color, thickness, and dimensions.
            /// </summary>
            /// <param name="r">The rectangle to draw.</param>
            /// <param name="thickness">The thickness to draw the rectangluar outline with.</param>
            /// <param name="c">The color to draw with.</param>
            public static void HollowRectangle(Rectangle r, int thickness, Color c) {
                HollowRectangle(r.X, r.Y, r.Width, r.Height, thickness, c);
            }

            /// <summary>
            /// Draws a line.
            /// </summary>
            /// <param name="start">The start position of the line.</param>
            /// <param name="end">The end position of the line.</param>
            /// <param name="color">The color to draw the line with.</param>
            public static void Line(Vector2 start, Vector2 end, Color color) {
                LineAngle(start, MiscHelper.Angle(start, end), Vector2.Distance(start, end), color);
            }

            /// <summary>
            /// Draws a line with a given start point, angle, length, and color.
            /// </summary>
            /// <param name="start">The starting point of the line.</param>
            /// <param name="angle">The angle of the line.</param>
            /// <param name="length">The length of the line.</param>
            /// <param name="color">The color of the line.</param>
            public static void LineAngle(Vector2 start, float angle, float length, Color color) {
                Engine.Batch.Draw(Pixel.Texture.Texture, start, Pixel.ClipRect, color, angle, Vector2.Zero, new Vector2(length, 1f), SpriteEffects.None, 0f);
            }

            /// <summary>
            /// Draws a rectangle with the specified color.
            /// </summary>
            public static void Rectangle(Rectangle r, Color c) {
                Pixel.Draw(r, c);
            }

            /// <summary>
            /// Draws a rectangle with the specified dimensions and color.
            /// </summary>
            public static void Rectangle(int x, int y, int w, int h, Color c) {
                Pixel.Draw(new Rectangle(x, y, w, h), c);
            }

            /// <summary>
            /// Draws text on the screen.
            /// </summary>
            /// <param name="msg">The text to draw.</param>
            /// <param name="pos">The position to draw the text.</param>
            /// <param name="c">The color to draw the text in.</param>
            public static void Text(string msg, Vector2 pos, Color c) {
                float xp = pos.X;
                foreach (char ch in msg.ToLower()) {
                    int loc = FontMap.IndexOf(ch);
                    if (loc >= 0) {
                        Font[loc].Draw(new Vector2(xp, pos.Y), c);
                    }
                    xp += 4f;
                }
            }

            /// <summary>
            /// Draws text centered in the middle of the given rectangle.
            /// </summary>
            /// <param name="msg">The text to draw.</param>
            /// <param name="rect">The rectangle to center the text in.</param>
            /// <param name="c">The color to draw the text in.</param>
            public static void TextCentered(string msg, Rectangle rect, Color c) {
                int charLimit = (int)Math.Floor(rect.Width / 4f);
                string[] words = msg.Split(' ');
                List<string> line = new List<string>();

                List<string> res = new List<string>();

                foreach (string word in words) {
                    if ((string.Join(" ", line) + " " + word).Length > charLimit) {
                        if (line.Count == 0) {
                            // If text can't be wrapped - don't display it.
                            return;
                        }

                        res.Add(string.Join(" ", line));
                        line = new List<string>();
                    }

                    line.Add(word);
                }

                if (string.Join(" ", line).Length > 0) res.Add(string.Join(" ", line));

                // If there are too many rows - don't display it.
                if (res.Count > rect.Height / 6) return;

                int h = rect.Center.Y - res.Count * 3;
                foreach (string row in res) {
                    Text(row, new Vector2(rect.Center.X - (row.Length * 2), h), c);
                    h += 6;
                }
            }
        }
    }
}
