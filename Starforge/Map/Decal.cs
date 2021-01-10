using Microsoft.Xna.Framework;

namespace Starforge.Map {
    /// <summary>
    /// Represents a Decal in the map.
    /// </summary>
    public class Decal : IPackable {
        public float X;
        public float Y;
        public Vector2 Scale;
        public string Name;

        public static Decal Decode(MapElement el) {
            return new Decal()
            {
                X = el.GetFloat("x"),
                Y = el.GetFloat("y"),
                Scale = new Vector2(el.GetFloat("scaleX"), el.GetFloat("scaleY")),
                Name = el.GetString("texture")
            };
        }

        public MapElement Encode() {
            MapElement el = new MapElement() { Name = "decal" };
            el.SetAttribute("x", X);
            el.SetAttribute("y", Y);
            el.SetAttribute("scaleX", Scale.X);
            el.SetAttribute("scaleY", Scale.Y);
            el.SetAttribute("texture", Name);

            return el;
        }
    }
}
