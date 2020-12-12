using Microsoft.Xna.Framework;
using Starforge.Core;
using Starforge.Mod.Assets;
using Starforge.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Starforge.MapStructure.Tiling {
    public class Autotiler {
        private Dictionary<char, Tileset> Tilesets;

        public Autotiler(string xmlPath) {
            Tilesets = new Dictionary<char, Tileset>();
            Dictionary<char, XmlElement> tileXmls = new Dictionary<char, XmlElement>();

            XmlDocument doc = new XmlDocument();
            using(FileStream stream = File.OpenRead(xmlPath)) {
                doc.Load(stream);
            }

            foreach(object obj in doc.GetElementsByTagName("Tileset")) {
                XmlElement el = (XmlElement)obj;
                char c = el.AttrChar("id");
                Tileset t = new Tileset(GFX.Gameplay["tilesets/" + el.Attr("path")], 8, 8);
                t.ID = c;
                t.Path = el.Attr("path");

                t.Ignores = new HashSet<char>();
                if(el.HasAttribute("ignores")) {
                    foreach(string ignore in el.Attr("ignores").Split(',')) {
                        t.Ignores.Add(ignore[0]);
                    }
                }

                t.Masks = new List<TileMask>();

                if(el.HasAttribute("copy")) {
                    char copy = el.AttrChar("copy");
                    if(tileXmls.ContainsKey(copy)) {
                        ReadData(t, tileXmls[copy]);
                    } else {
                        Logger.Log(LogLevel.Error, $"Tileset {c} attempted to copy unloaded tileset {copy}");
                    }
                }

                ReadData(t, el);

                t.Masks.Sort(delegate (TileMask a, TileMask b) {
                    int i = 0;
                    int j = 0;
                    for(int k = 0; k < 9; k++) {
                        if(a.Mask[k] == 2) i--;
                        if(b.Mask[k] == 2) j--;
                    }

                    return i - j;
                });

                tileXmls.Add(c, el);
                Tilesets.Add(c, t);
            }
        }

        public void ReadData(Tileset t, XmlElement root) {
            foreach(object obj in root.GetElementsByTagName("set")) {
                XmlElement el = (XmlElement)obj;

                if(el.Attr("mask") == "center") {
                    t.Center = t.ParseTextureString(el.Attr("tiles"));
                } else if(el.Attr("mask") == "padding") {
                    t.Padding = t.ParseTextureString(el.Attr("tiles"));
                } else {
                    string orig = el.Attr("mask");
                    byte[] mask = new byte[9];

                    int num = 0;
                    for(int i = 0; i < orig.Length; i++) {
                        char c = orig[i];
                        if(c == '0') {
                            mask[num++] = 0;
                        } else if(c == '1') {
                            mask[num++] = 1;
                        } else if(c == 'x' || c == 'X') {
                            mask[num++] = 2;
                        }
                    }

                    t.Masks.Add(new TileMask() {
                        Mask = mask,
                        Textures = t.ParseTextureString(el.Attr("tiles"))
                    });
                }
            }
        }

        public bool CheckTile(TileGrid grid, Tileset t, int x, int y) {
            // If position is out of bounds in a given TileGrid, assume there is a tile there
            if(x < 0 || x > grid.Width - 1 || y < 0 || y > grid.Height - 1) return true;

            return grid[x, y] != 48 && !(t.ID != grid[x, y] && (t.Ignores.Contains((char)grid[x, y]) || t.Ignores.Contains('*')));
        }


        public StaticTexture[] GenerateTextureMap(TileGrid grid, int offsetX, int offsetY) {
            StaticTexture[] textures = new StaticTexture[grid.Width * grid.Height];

            for(int i = 0; i < grid.Tiles.GetLength(0); i++) {
                for(int j = 0; j < grid.Tiles.GetLength(1); j++) {
                    StaticTexture tex = new StaticTexture(GFX.Empty);
                    if(grid[i, j] != 48) {
                        textures[j * grid.Width + i] = GenerateTileTexture(grid, i, j);
                    }
                }
            }

            return textures;
        }

        public StaticTexture GenerateTileTexture(TileGrid grid, int i, int j) {
            StaticTexture tex = new StaticTexture(GFX.Empty);
            if(grid[i, j] != 48) {
                int num = 0;
                byte[] adjacent = new byte[9];
                bool center = true;

                Tileset t = Tilesets[(char)grid[i, j]];

                for(int y = -1; y < 2; y++) {
                    for(int x = -1; x < 2; x++) {
                        bool res = CheckTile(grid, t, i + x, j + y);
                        if(res) {
                            adjacent[num++] = 1;
                        } else {
                            adjacent[num++] = 0;
                            center = false;
                        }
                    }
                }

                if(center) {
                    if(!CheckTile(grid, t, i - 2, j)
                        || !CheckTile(grid, t, i + 2, j)
                        || !CheckTile(grid, t, i, j - 2)
                        || !CheckTile(grid, t, i, j + 2)) {
                        tex.Texture = MiscHelper.Choose(i, j, t.Padding);
                    } else {
                        tex.Texture = MiscHelper.Choose(i, j, t.Center);
                    }
                } else {
                    tex.Texture = GFX.Empty; // Set to arbitrary texture incase there isn't a valid mask.
                    tex.Visible = false;
                    foreach(TileMask m in t.Masks) {
                        bool found = true;
                        int index = 0;
                        while(index < 9 && found) {
                            if(m.Mask[index] != 2 && m.Mask[index] != adjacent[index]) found = false;

                            index++;
                        }

                        if(found) {
                            tex.Texture = MiscHelper.Choose(i, j, m.Textures);
                            tex.Visible = true;
                            break;
                        }
                    }
                }

                tex.Position = new Vector2(i * 8, j * 8);
            }

            return tex;
        }

        public void Update(TileGrid grid, StaticTexture[] texArray, Point point) {
            Update(grid, texArray, new Rectangle(point.X - 1, point.Y - 1, 3, 3));
        }

        public void Update(TileGrid grid, StaticTexture[] texArray, Rectangle r) {
            for(int x = 0; x < r.Width; x++) {
                for(int y = 0; y < r.Height; y++) {
                    if(x + r.X < 0 || x + r.X > grid.Width - 1 || y + r.Y < 0 || y + r.Y > grid.Height - 1) continue;
                    texArray[(y + r.Y) * grid.Width + x + r.X] = GenerateTileTexture(grid, x + r.X, y + r.Y);
                }
            }
        }

        public List<Tileset> GetTilesetList() {
            List<Tileset> l = new List<Tileset>();
            foreach(Tileset t in Tilesets.Values) {
                l.Add(t);
            }

            return l;
        }
    }
}
