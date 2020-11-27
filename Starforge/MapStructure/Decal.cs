using Starforge.MapStructure.Encoding;

namespace Starforge.MapStructure {
    public class Decal {
        public float X;
        public float Y;
        public float ScaleX;
        public float ScaleY;
        public string Texture;

        public Decal(float x, float y, float scaleX, float scaleY, string texture) {
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
