using Microsoft.Xna.Framework;
using Starforge.Core;
using Starforge.MapStructure.Encoding;
using Starforge.Mod.Assets;

namespace Starforge.MapStructure {
    public class Decal {
        public float X;
        public float Y;
        public float ScaleX;
        public float ScaleY;
        public string Name;
        
        public DrawableTexture Texture {
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

        public BinaryMapElement ToBinary() {
            BinaryMapElement bin = new BinaryMapElement()
            {
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
            DrawableTexture tex = GFX.Gameplay["decals/" + Name.Substring(0, Name.Length - 4).Replace('\\', '/')];
            tex.PregeneratedPosition = new Vector2(X, Y);
            tex.PregeneratedScale = new Vector2(ScaleX, ScaleY);
            Texture = tex;
        }
    }
}
