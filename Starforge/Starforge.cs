using Starforge.MapStructure;
using Starforge.MapStructure.Encoding;
using Starforge.Mod;
using System.IO;

namespace Starforge {
    public static class Starforge {
        public static void Main(string[] args) {
            using(FileStream stream = File.OpenRead(".\\test.bin")) {
                using(BinaryReader reader = new BinaryReader(stream)) {
                    BinaryMapElement bin = MapPacker.ReadMapBinary(reader);
                    Map map = Map.FromBinary(bin);

                    int x = 0;
                    x = x + 1;
                }
            }
        }
    }
}
