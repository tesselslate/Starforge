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

                t.Ignores = new HashSet<char>();
                if(el.HasAttribute("ignores")) {
                    foreach(string ignore in el.Attr("ignores").Split(',')) {
                        t.Ignores.Add(ignore[0]);
                    }
                }

                if(el.HasAttribute("copy")) {
                    char copy = el.AttrChar("copy");
                    if(tileXmls.ContainsKey(copy)) {
                        ReadData(t, tileXmls[copy]);
                    } else {
                        Logger.Log(LogLevel.Error, $"Tileset {c} attempted to copy unloaded tileset {copy}");
                    }
                }

                ReadData(t, el);

                tileXmls.Add(c, el);
                Tilesets.Add(c, t);
            }
        }

        public void ReadData(Tileset t, XmlElement root) {
            t.Masks = new Dictionary<string, List<DrawableTexture>>();

            foreach(object obj in root.GetElementsByTagName("set")) {
                XmlElement el = (XmlElement)obj;

                if(el.Attr("mask") == "center") {
                    t.Center = t.ParseTextureString(el.Attr("tiles"));
                } else if(el.Attr("mask") == "padding") {
                    t.Padding = t.ParseTextureString(el.Attr("tiles"));
                } else {
                    string orig = el.Attr("mask");
                    string mask = "";

                    foreach(char c in orig) {
                        if(c == '0') {
                            mask += '0';
                        } else if(c == '1') {
                            mask += '1';
                        } else if(c == 'x' || c == 'X') {
                            mask += 'x';
                        }
                    }

                    t.Masks.Add(mask, t.ParseTextureString(el.Attr("tiles")));
                }
            }
        }

        public DrawableTexture[] GenerateTextureMap(int[,] grid, int offsetX, int offsetY) {
            List<DrawableTexture> textures = new List<DrawableTexture>();

            for(int i = 0; i < grid.GetLength(0); i++) {
                for(int j = 0; j < grid.GetLength(1); j++) {
                    if(grid[i, j] != 48) {
                        DrawableTexture tex = Tilesets[(char)grid[i, j]][0,0];
                        tex.PregeneratedPosition = new Vector2(i * 8 + offsetX, j * 8 + offsetY);
                        textures.Add(tex);
                    }
                }
            }

            return textures.ToArray();
        }
    }
}
