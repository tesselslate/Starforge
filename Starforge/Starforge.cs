using Starforge.MapStructure;
using Starforge.MapStructure.Encoding;
using Starforge.Mod.Assets;
using System;
using System.Diagnostics;
using System.IO;

namespace Starforge {
    public static class Starforge {
        public static void Main(string[] args) {
            foreach(string file in Directory.GetFiles(".\\testmaps\\")) {
                using(FileStream stream = File.OpenRead(file)) {
                    using(BinaryReader reader = new BinaryReader(stream)) {
                        Stopwatch watch = new Stopwatch();
                        watch.Start();

                        BinaryMapElement bin = MapPacker.ReadMapBinary(reader);

                        watch.Stop();
                        Console.WriteLine($"Read map binary in {watch.ElapsedMilliseconds}ms");

                        watch.Restart();

                        Map map = Map.FromBinary(bin);

                        watch.Stop();
                        Console.WriteLine($"Parsed map binary in {watch.ElapsedMilliseconds}ms");

                        using(FileStream stream2 = File.OpenWrite(".\\resavemaps\\" + Path.GetFileName(file))) {
                            using(BinaryWriter writer = new BinaryWriter(stream2)) {
                                watch.Restart();
                                BinaryMapElement resave = map.ToBinary();
                                watch.Stop();
                                Console.WriteLine($"Serialized map in {watch.ElapsedMilliseconds}ms");

                                watch.Restart();
                                MapPacker.WriteMapBinary(resave, writer, resave.Package);
                                watch.Stop();
                                Console.WriteLine($"Wrote map binary in {watch.ElapsedMilliseconds}ms");
                            }
                        }
                    }
                }
            }

            Console.ReadLine();
        }
    }
}
