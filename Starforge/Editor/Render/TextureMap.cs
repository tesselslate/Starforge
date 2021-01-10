namespace Starforge.Editor.Render {
    /// <summary>
    /// Contains a map of textures which can be rendered in quick succession or modified.
    /// </summary>
    public class TextureMap {
        /// <summary>
        /// The underlying texture array.
        /// </summary>
        public StaticTexture[] Textures;

        /// <summary>
        /// Access a texture at the given position in the map.
        /// </summary>
        /// <param name="x">The X position.</param>
        /// <param name="y">The Y position.</param>
        /// <returns>The texture at the given position.</returns>
        public StaticTexture this[int x, int y] {
            get {
                if (x < 0 || x >= Width || y < 0 || y >= Height) return default;
                else return Textures[x + y * Width];
            }
        }

        /// <summary>
        /// The width of the grid.
        /// </summary>
        public int Width;

        /// <summary>
        /// The height of the grid.
        /// </summary>
        public int Height;

        /// <summary>
        /// Creates a blank texture map with the given size.
        /// </summary>
        /// <param name="width">The width of the map.</param>
        /// <param name="height">The height of the map.</param>
        public TextureMap(int width, int height) {
            Textures = new StaticTexture[(Width = width) * (Height = height)];
        }

        /// <summary>
        /// Draws all the textures in the map which are visible.
        /// </summary>
        public void Draw() {
            for(int i = 0; i < Textures.Length; i++) {
                if(Textures[i].Visible) {
                    Textures[i].Draw();
                }
            }
        }

        public void Draw(float alpha) {
            for(int i = 0; i < Textures.Length; i++) {
                if(Textures[i].Visible) {
                    Textures[i].Draw(alpha);
                }
            }
        }
    }
}
