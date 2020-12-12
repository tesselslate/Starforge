using Starforge.Mod.Assets;
using System.Collections.Generic;

namespace Starforge.MapStructure.Tiling {
    public class Tileset {
        // Tileset textures
        public DrawableTexture Texture {
            get;
            private set;
        }

        public DrawableTexture this[int x, int y] {
            get => Tiles[x, y];
        }

        private DrawableTexture[,] Tiles;

        public int TileWidth {
            get;
            private set;
        }

        public int TileHeight {
            get;
            private set;
        }

        public string Path;

        // Tileset masking/info
        public char ID;

        public HashSet<char> Ignores;

        public List<DrawableTexture> Center;

        public List<TileMask> Masks;

        public List<DrawableTexture> Padding;

        public Tileset(DrawableTexture texture, int width, int height) {
            Texture = texture;
            TileWidth = width;
            TileHeight = height;

            Tiles = new DrawableTexture[texture.Width / width, texture.Height / height];
            for (int i = 0; i < texture.Width / width; i++) {
                for (int j = 0; j < texture.Height / height; j++) {
                    Tiles[i, j] = new DrawableTexture(Texture, i * TileWidth, j * TileHeight, TileWidth, TileHeight);
                }
            }
        }

        public List<DrawableTexture> ParseTextureString(string str) {
            List<DrawableTexture> list = new List<DrawableTexture>();
            string[] tiles = str.Split(';');
            foreach (string loc in tiles) {
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
