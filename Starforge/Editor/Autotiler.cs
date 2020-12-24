using Microsoft.Xna.Framework;
using Starforge.Core;
using Starforge.Editor.Render;
using Starforge.Map;
using Starforge.Mod.Content;
using Starforge.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Starforge.Editor {
    /// <summary>
    /// Generates textures based on tiles.
    /// </summary>
    public class Autotiler {
        /// <summary>
        /// A dictionary of all registered tilesets.
        /// </summary>
        private Dictionary<char, Tileset> Tilesets;

        /// <summary>
        /// A byte array used to generate random tile textures.
        /// </summary>
        private byte[] TileRand;

        /// <summary>
        /// Creates an autotiler.
        /// </summary>
        /// <param name="xmlPath">The path to the file containing the tileset definitions.</param>
        public Autotiler(string xmlPath) {
            // Create tile randomization array
            TileRand = new byte[1048576];
            new Random().NextBytes(TileRand);

            // Register tilesets
            Tilesets = new Dictionary<char, Tileset>();
            Dictionary<char, XmlElement> elements = new Dictionary<char, XmlElement>();

            XmlDocument doc = new XmlDocument();
            using (FileStream stream = File.OpenRead(xmlPath)) {
                doc.Load(stream);
            }

            foreach (object obj in doc.GetElementsByTagName("Tileset")) {
                XmlElement el = (XmlElement)obj;

                char c = el.AttrChar("id");
                Tileset t = new Tileset(GFX.Gameplay["tilesets/" + el.Attr("path")], 8, 8);
                t.ID = c;

                t.Ignores = new HashSet<char>();
                if (el.HasAttribute("ignores")) {
                    foreach (string ignore in el.Attr("ignores").Split(',')) {
                        t.Ignores.Add(ignore[0]);
                    }
                }

                t.Masks = new List<TileMask>();
                if (el.HasAttribute("copy")) {
                    char copy = el.AttrChar("copy");

                    if (elements.ContainsKey(copy)) {
                        ReadData(t, elements[copy]);
                    } else {
                        Logger.Log(LogLevel.Error, $"Tileset {c} attempted to copy unregistered tileset {copy}");
                    }
                }

                ReadData(t, el);

                elements.Add(c, el);
                if (el.Attr("path").ToLower() != "template") Tilesets.Add(c, t);
            }
        }

        private void ReadData(Tileset t, XmlElement root) {
            foreach (object obj in root.GetElementsByTagName("set")) {
                XmlElement el = (XmlElement)obj;
                string tileString = el.Attr("tiles");

                switch (el.Attr("mask")) {
                case "center":
                    t.Center = t.ParseTextureString(tileString);
                    break;
                case "padding":
                    t.Padding = t.ParseTextureString(tileString);
                    break;
                default:
                    string orig = el.Attr("mask");
                    byte[] mask = new byte[9];

                    int pos = 0;
                    for (int i = 0; i < orig.Length; i++) {
                        switch(orig[i]) {
                        case '0':
                            mask[pos++] = 0;
                            break;
                        case '1':
                            mask[pos++] = 1;
                            break;
                        case 'x': case 'X':
                            mask[pos++] = 2;
                            break;
                        }
                    }

                    t.Masks.Add(new TileMask()
                    {
                        Mask = mask,
                        Textures = t.ParseTextureString(tileString)
                    });
                    break;
                }
            }
        }

        public bool CheckTile(TileGrid grid, Tileset t, int x, int y, bool edgesExtend) {
            // If position is out of bounds in a given TileGrid, assume there is a tile there
            if (x < 0 || x > grid.Width - 1 || y < 0 || y > grid.Height - 1) return edgesExtend;

            return grid[x, y] != '0' && !(t.ID != grid[x, y] && (t.Ignores.Contains((char)grid[x, y]) || t.Ignores.Contains('*')));
        }

        public TextureMap GenerateTextureMap(TileGrid grid, bool edgesExtend = true) {
            TextureMap map = new TextureMap(grid.Width, grid.Height);

            for (int y = 0; y < grid.Height; y++) {
                int yInc = y * grid.Width;
                for (int x = 0; x < grid.Width; x++) {
                    if (grid[x, y] != '0') {
                        map.Textures[yInc + x] = GenerateTileTexture(grid, x, y, edgesExtend, yInc);
                    } else {
                        map.Textures[yInc + x] = new StaticTexture() { Visible = false };
                    }
                }
            }

            return map;
        }

        public StaticTexture GenerateTileTexture(TileGrid grid, int i, int j, bool edgesExtend = true, int yInc = 0) {
            if (grid[i, j] == '0') {
                return new StaticTexture() { Visible = false };
            }

            StaticTexture tex = new StaticTexture() { Visible = true };

            int num = 0;
            byte[] adjacent = new byte[9];
            bool center = true;

            Tileset t = Tilesets[(char)grid[i, j]];

            for (int y = -1; y < 2; y++) {
                for (int x = -1; x < 2; x++) {
                    bool res = CheckTile(grid, t, i + x, j + y, edgesExtend);
                    if (res) {
                        adjacent[num++] = 1;
                    } else {
                        adjacent[num++] = 0;
                        center = false;
                    }
                }
            }

            if (center) {
                if (!CheckTile(grid, t, i - 2, j, edgesExtend)
                    || !CheckTile(grid, t, i + 2, j, edgesExtend)
                    || !CheckTile(grid, t, i, j - 2, edgesExtend)
                    || !CheckTile(grid, t, i, j + 2, edgesExtend)) {
                    tex.Texture = t.Padding[TileRand[i + yInc] % t.Padding.Count];
                } else {
                    tex.Texture = t.Center[TileRand[i + yInc] % t.Center.Count];
                }
            } else {
                tex.Texture = GFX.Empty; // Set to arbitrary texture incase there isn't a valid mask.
                tex.Visible = false;
                foreach (TileMask m in t.Masks) {
                    bool found = true;
                    int index = 0;
                    while (index < 9 && found) {
                        if (m.Mask[index] != 2 && m.Mask[index] != adjacent[index]) found = false;

                        index++;
                    }

                    if (found) {
                        tex.Texture = m.Textures[TileRand[i + yInc] % m.Textures.Count];
                        tex.Visible = true;
                        break;
                    }
                }
            }

            tex.Position = new Vector2(i * 8, j * 8);

            return tex;
        }
    }
}
