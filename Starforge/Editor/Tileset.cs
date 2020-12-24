using Starforge.Mod.Content;
using System.Collections.Generic;

namespace Starforge.Editor {
    /// <summary>
    /// Contains information about a given tileset and its textures.
    /// </summary>
    public class Tileset {
        /// <summary>
        /// The tileset texture.
        /// </summary>
        public DrawableTexture Texture { get; private set; }

        /// <summary>
        /// Retrieves a tile texture at the given coordinates in the tileset texture.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public DrawableTexture this[int x, int y] => Tiles[x,y];

        /// <summary>
        /// The textures for each tile in the grid.
        /// </summary>
        private DrawableTexture[,] Tiles;

        private int TileWidth;
        private int Width;

        private int TileHeight;
        private int Height;

        public char ID;
        public HashSet<char> Ignores;
        public List<DrawableTexture> Center;
        public List<DrawableTexture> Padding;
        public List<TileMask> Masks;

        public Tileset(DrawableTexture texture, int tileWidth, int tileHeight) {
            Texture = texture;
            TileWidth = tileWidth;
            TileHeight = tileHeight;

            Tiles = new DrawableTexture[Width = texture.Width / tileWidth, Height = texture.Height / tileHeight];
            for(int x = 0; x < Width; x++) {
                for(int y = 0; y < Height; y++) {
                    Tiles[x, y] = new DrawableTexture(Texture, x * TileWidth, y * TileHeight, TileWidth, TileHeight);
                }
            }
        }

        public List<DrawableTexture> ParseTextureString(string str) {
            List<DrawableTexture> list = new List<DrawableTexture>();
            string[] tiles = str.Split(';');
            foreach(string loc in tiles) {
                string[] split = loc.Split(',');
                list.Add(this[int.Parse(split[0]), int.Parse(split[1])]);
            }

            return list;
        }
    }

    public class TileMask {
        public byte[] Mask = new byte[9];
        public List<DrawableTexture> Textures = new List<DrawableTexture>();
    }
}
