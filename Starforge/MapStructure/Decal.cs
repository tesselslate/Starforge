using Microsoft.Xna.Framework;
using Starforge.Core;
using Starforge.MapStructure.Encoding;
using Starforge.Mod.Assets;

namespace Starforge.MapStructure {
    public class Decal : MapElement {
        public float X;
        public float Y;
        public float ScaleX;
        public float ScaleY;
        public string Name;

        public StaticTexture Texture {
            get;
            private set;
        }

        public Decal(float x, float y, float scaleX, float scaleY, string texture) {
            X = x;
            Y = y;
            ScaleX = scaleX;
            ScaleY = scaleY;
            Name = texture;

            Update();
        }

        public override BinaryMapElement ToBinary() {
            BinaryMapElement bin = new BinaryMapElement() {
                Name = "decal"
            };

            bin.SetAttribute("x", X);
            bin.SetAttribute("y", Y);
            bin.SetAttribute("scaleX", ScaleX);
            bin.SetAttribute("scaleY", ScaleY);
            bin.SetAttribute("texture", Name);

            return bin;
        }

        public void Update() {
            StaticTexture tex = new StaticTexture(
                GFX.Gameplay["decals/" + Name.Substring(0, Name.Length - 4).Replace('\\', '/')],
                new Vector2(X, Y),
                new Vector2(ScaleX, ScaleY)
            );

            Texture = tex;
        }
    }
}
