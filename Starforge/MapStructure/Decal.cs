using Starforge.MapStructure.Encoding;

namespace Starforge.MapStructure {
    public class Decal {
        public int X;
        public int Y;
        public int ScaleX;
        public int ScaleY;
        public string Texture;

        public Decal(int x, int y, int scaleX, int scaleY, string texture) {
            X = x;
            Y = y;
            ScaleX = scaleX;
            ScaleY = scaleY;
            Texture = texture;
        }

        public BinaryMapElement ToBinary() {
            BinaryMapElement bin = new BinaryMapElement()
            {
                Name = "decal"
            };

            bin.SetAttribute("x", X);
            bin.SetAttribute("y", Y);
            bin.SetAttribute("scaleX", ScaleX);
            bin.SetAttribute("scaleY", ScaleY);
            bin.SetAttribute("texture", Texture);

            return bin;
        }
    }
}
