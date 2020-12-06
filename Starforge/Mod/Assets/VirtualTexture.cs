using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

namespace Starforge.Mod.Assets {
    public class VirtualTexture : IDisposable {
        public string Path { get; private set; }

        public string Name { get; private set; }

        public int Width { get; private set; }
        
        public int Height { get; private set; }

        public bool IsDisposed {
            get => Texture == null || Texture.IsDisposed || Texture.GraphicsDevice.IsDisposed;
        }

        public Texture2D Texture;

        internal VirtualTexture(string path) {
            Path = path;
            Name = path;
            Reload();
        }

        internal VirtualTexture(string name, int width, int height) {
            Name = name;
            Width = width;
            Height = height;

            Reload();
        }

        public static VirtualTexture CreateTexture(string path) {
            VirtualTexture text = new VirtualTexture(path);
            Core.Starforge.Instance.VirtualContent.Add(text);
            return text;
        }

        public static VirtualTexture CreateTexture(string path, int width, int height) {
            VirtualTexture text = new VirtualTexture(path, width, height);
            Core.Starforge.Instance.VirtualContent.Add(text);
            return text;
        }

        public void Dispose() {
            if(Texture != null && !Texture.IsDisposed) {
                Texture.Dispose();
            }
            Texture = null;
        }

        internal unsafe void Reload() {
            Dispose();
            if(string.IsNullOrEmpty(Path)) {
                Texture = new Texture2D(Core.Starforge.Instance.GraphicsDevice, Width, Height);
                Color[] array = new Color[Width * Height];
                
                for(int i = 0; i < array.Length; i++) {
                    array[i] = Color.Transparent;
                }

                Texture.SetData(array);
            } else if(System.IO.Path.GetExtension(Path) == ".data") {
                using(FileStream stream = File.OpenRead(Path)) {
                    using(BinaryReader reader = new BinaryReader(stream)) {
                        int width = reader.ReadInt32();
                        int height = reader.ReadInt32();
                        bool alpha = reader.ReadBoolean();

                        Width = width;
                        Height = height;

                        int loop = 0;
                        int r, g, b, a;
                        r = g = b = a = 255;

                        Texture = new Texture2D(Core.Starforge.Instance.GraphicsDevice, width, height);

                        Color[] c = new Color[width * height];
                        int pos = 0;

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

                                            c[pos] = new Color(r, g, b, a);
                                        } else {
                                            a = r = g = b = 0;
                                        }
                                    } else {
                                        b = reader.ReadByte();
                                        g = reader.ReadByte();
                                        r = reader.ReadByte();

                                        c[pos] = new Color(r, g, b, byte.MaxValue);
                                    }
                                } else {
                                    c[pos] = new Color(r, g, b, a);
                                    loop--;
                                }
                                pos++;
                            }
                        }

                        Texture.SetData(c);
                    }
                }
                using(FileStream stream2 = File.OpenWrite($"./content/{System.IO.Path.GetFileNameWithoutExtension(Name)}.png")) {
                    Texture.SaveAsPng(stream2, Width, Height);
                }

            } else if(System.IO.Path.GetExtension(Path) == ".png") {
                using(FileStream stream = File.OpenRead(Path)) {
                    Texture = Texture2D.FromStream(Core.Starforge.Instance.GraphicsDevice, stream);
                }
            }
        }
    }
}
