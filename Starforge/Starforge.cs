using Starforge.MapStructure;
using Starforge.MapStructure.Encoding;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Starforge {
    public static class Starforge {
        public static void Main(string[] args) {
            // I am keeping this here for testing. It'll be removed at some point (probably when testing another feature.)

            using(FileStream stream = File.OpenRead(".\\0-Intro.bin")) {
                using(BinaryReader reader = new BinaryReader(stream)) {
                    BinaryMapElement bin = MapPacker.ReadMapBinary(reader);

                    Map map = Map.FromBinary(bin);
                    
                    using(FileStream stream2 = File.OpenWrite(".\\prologue_resave.bin")) {
                        using(BinaryWriter writer = new BinaryWriter(stream2)) {
                            MapPacker.WriteMapBinary(map.ToBinary(), writer, map.Package);
                        }
                    }
                }
            }
        }
    }
}
