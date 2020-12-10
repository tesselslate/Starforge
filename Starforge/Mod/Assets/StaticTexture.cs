using Microsoft.Xna.Framework;

namespace Starforge.Mod.Assets {
    /// <summary>
    /// StaticTexture is used to create an individual sprite which has a
    /// predetermined position/scale for faster drawing.
    /// </summary>
    public struct StaticTexture {
        public DrawableTexture Texture;

        public Rectangle Destination;

        public Vector2 Position;

        public Vector2 Scale;

        public StaticTexture(DrawableTexture tex) {
            Texture = tex;
            Destination = default;
            Position = Vector2.Zero;
            Scale = Vector2.One;
        }

        public StaticTexture(DrawableTexture tex, Rectangle destination) {
            Texture = tex;
            Destination = destination;
            Position = Vector2.Zero;
            Scale = Vector2.One;
        }

        public StaticTexture(DrawableTexture tex, Rectangle destination, Vector2 scale) {
            Texture = tex;
            Destination = destination;
            Position = Vector2.Zero;
            Scale = scale;
        }

        public StaticTexture(DrawableTexture tex, Vector2 position) {
            Texture = tex;
            Destination = default;
            Position = position;
            Scale = Vector2.One;
        }

        public StaticTexture(DrawableTexture tex, Vector2 position, Vector2 scale) {
            Texture = tex;
            Destination = default;
            Position = position;
            Scale = scale;
        }

        public void Draw() {
            Texture.Draw(Position);
        }

        public void DrawCentered() {
            Texture.DrawCenteredScaling(Position, Scale);
        }
    }
}
