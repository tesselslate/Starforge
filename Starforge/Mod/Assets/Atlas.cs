using Eto.Drawing;
using System;
using System.Collections.Generic;
using System.IO;

namespace Starforge.Mod.Assets {
    public class Atlas {
        private Dictionary<string, Texture> Textures = new Dictionary<string, Texture>();

        public Bitmap Image {
            get;
            private set;
        }

        public Texture this[string id] {
            get => Textures[id];
            set => Textures[id] = value;
        }

        public static Atlas Unpack(string path, AtlasFormat format) {
            Atlas atlas = new Atlas();

            switch(format) {
                case AtlasFormat.Packer:
                    using(FileStream stream = File.OpenRead(path)) {
                        using(BinaryReader reader = new BinaryReader(stream)) {
                            reader.ReadInt32();
                            reader.ReadString();
                            reader.ReadInt32();
                            short textures = reader.ReadInt16();
                            for(int i = 0; i < textures; i++) {
                                string DataPath = reader.ReadString();
                                short images = reader.ReadInt16();
                                for(int j = 0; j < images; j++) {
                                    string AtlasPath = reader.ReadString();
                                    short x = reader.ReadInt16();
                                    short y = reader.ReadInt16();
                                    short w = reader.ReadInt16();
                                    short h = reader.ReadInt16();
                                    short frameX = reader.ReadInt16();
                                    short frameY = reader.ReadInt16();
                                    short frameW = reader.ReadInt16();
                                    short frameH = reader.ReadInt16();
                                    atlas[AtlasPath] = new PackerTexture(atlas, AtlasPath, x, y, w, h, frameX, frameY, frameW, frameH);
                                }
                            }

                            // The LINKS section seems to be unused in the Gameplay atlas, so it's not decoded.
                        }
                    }
                    break;
                case AtlasFormat.PackerNoAtlas:
                    // NoAtlas is only used for misc.
                    using(FileStream stream = File.OpenRead(path)) {
                        using(BinaryReader reader = new BinaryReader(stream)) {
                            reader.ReadInt32();
                            reader.ReadString();
                            reader.ReadInt32();
                            short subatlases = reader.ReadInt16();
                            for(int i = 0; i < subatlases; i++) {
                                string DataFolder = Path.Combine(
                                    Path.GetDirectoryName(path),
                                    reader.ReadString()
                                );

                                short textures = reader.ReadInt16();
                                for(int j = 0; j < textures; j++) {
                                    string AtlasName = reader.ReadString().Replace('\\', '/');

                                    // Skip useless data
                                    reader.ReadInt16();
                                    reader.ReadInt16();
                                    reader.ReadInt16();
                                    reader.ReadInt16();
                                    short x = reader.ReadInt16();
                                    short y = reader.ReadInt16();
                                    short w = reader.ReadInt16();
                                    short h = reader.ReadInt16();

                                    Bitmap img = new Bitmap(new Size(w, h), PixelFormat.Format32bppRgba);
                                    atlas[AtlasName] = new Texture(AtlasName, w, h)
                                    {
                                        Image = UnpackData(Path.Combine(
                                            DataFolder,
                                            AtlasName + ".data"
                                        ))
                                    };
                                }
                            }
                        }
                    }
                    break;
            }

            return atlas;
        }

        public static Bitmap UnpackData(string path) {
            Bitmap img;

            using(FileStream stream = File.OpenRead(path)) {
                using(BinaryReader reader = new BinaryReader(stream)) {
                    int width = reader.ReadInt32();
                    int height = reader.ReadInt32();
                    bool alpha = reader.ReadBoolean();

                    int loop = 0;
                    int r, g, b, a;
                    r = g = b = a = 255;

                    img = new Bitmap(new Size(width, height), PixelFormat.Format32bppRgba);

                    for(int y = 0; y < height; y++) {
                        for(int x = 0; x < width; x++) {
                            if(loop == 0) {
                                loop = reader.ReadByte() - 1;

                                if(alpha) {
                                    a = reader.ReadByte();

                                    if(a > 0) {
                                        b = reader.ReadByte();
                                        g = reader.ReadByte();
                                        r = reader.ReadByte();

                                        img.SetPixel(x, y, Color.FromArgb(a, r, g, b));
                                    } else {
                                        a = r = g = b = 0;
                                    }
                                } else {
                                    b = reader.ReadByte();
                                    g = reader.ReadByte();
                                    r = reader.ReadByte();

                                    img.SetPixel(x, y, Color.FromArgb(byte.MaxValue, r, g, b));
                                }
                            } else {
                                img.SetPixel(x, y, Color.FromArgb(a, r, g, b));
                                loop--;
                            }
                        }
                    }
                }
            }

            return img;
        }
    }

    public enum AtlasFormat {
        Packer,
        PackerNoAtlas
    }
}
