using Microsoft.Xna.Framework;
using Starforge.Core;
using System;
using System.Collections.Generic;
using System.IO;

namespace Starforge.Mod.Content {
    public class Atlas : IDisposable {
        public DrawableTexture this[string id] {
            get {
                if (Textures.ContainsKey(id)) return Textures[id];
                return GFX.Empty;
            }
            set => Textures[id] = value;
        }

        public List<VirtualTexture> Sources;

        private Dictionary<string, DrawableTexture> Textures;

        public static Atlas FromAtlas(string path, AtlasFormat format) {
            Atlas atlas = new Atlas();
            atlas.Sources = new List<VirtualTexture>();
            atlas.Textures = new Dictionary<string, DrawableTexture>();
            ReadAtlasData(atlas, path, format);

            return atlas;
        }

        public void Dispose() {
            foreach (VirtualTexture tex in Sources) {
                tex.Dispose();
            }

            Sources.Clear();
            Textures.Clear();
        }

        private static void ReadAtlasData(Atlas atlas, string path, AtlasFormat format) {
            switch (format) {
            case AtlasFormat.Packer:
                using (FileStream stream = File.OpenRead(path + ".meta")) {
                    using (BinaryReader reader = new BinaryReader(stream)) {
                        // Read useless data
                        reader.ReadInt32();
                        reader.ReadString();
                        reader.ReadInt32();

                        short atlases = reader.ReadInt16();
                        for (int i = 0; i < atlases; i++) {
                            string dataName = reader.ReadString();
                            string dataPath = Path.Combine(Path.GetDirectoryName(path), dataName + ".data");
                            VirtualTexture text = VirtualTexture.CreateTexture(dataPath);
                            atlas.Sources.Add(text);

                            short textures = reader.ReadInt16();
                            for (int j = 0; j < textures; j++) {
                                string name = reader.ReadString().Replace('\\', '/');
                                short x = reader.ReadInt16();
                                short y = reader.ReadInt16();
                                short w = reader.ReadInt16();
                                short h = reader.ReadInt16();
                                short fx = reader.ReadInt16();
                                short fy = reader.ReadInt16();
                                short fw = reader.ReadInt16();
                                short fh = reader.ReadInt16();
                                atlas.Textures[name] = new DrawableTexture(text, new Rectangle(x, y, w, h), new Vector2(-fx, -fy), fw, fh);
                            }
                        }

                        // LINKS section does not appear to be used in Gameplay atlas.
                    }
                }

                break;
            case AtlasFormat.PackerNoAtlas:
                using (FileStream stream = File.OpenRead(path + ".meta")) {
                    using (BinaryReader reader = new BinaryReader(stream)) {
                        // Read useless data
                        reader.ReadInt32();
                        reader.ReadString();
                        reader.ReadInt32();

                        short atlases = reader.ReadInt16();
                        for (int i = 0; i < atlases; i++) {
                            string atlasName = reader.ReadString();
                            string atlasPath = Path.Combine(Path.GetDirectoryName(path), atlasName);

                            short textures = reader.ReadInt16();
                            for (int j = 0; j < textures; j++) {
                                string name = reader.ReadString().Replace('\\', '/');
                                // Read useless data
                                reader.ReadInt16();
                                reader.ReadInt16();
                                reader.ReadInt16();
                                reader.ReadInt16();
                                short x = reader.ReadInt16();
                                short y = reader.ReadInt16();
                                short w = reader.ReadInt16();
                                short h = reader.ReadInt16();

                                VirtualTexture tex = VirtualTexture.CreateTexture(Path.Combine(atlasPath, name + ".data"));
                                atlas.Sources.Add(tex);
                                atlas.Textures[name] = new DrawableTexture(tex, new Vector2(-x, -y), w, h);
                            }
                        }
                    }
                }

                break;
            }
        }
    }

    public enum AtlasFormat {
        Packer,
        PackerNoAtlas
    }
}