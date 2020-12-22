using System.Collections.Generic;

namespace Starforge.Map {
    /// <summary>
    /// Represents a style element of the level (e.g. a parallax styleground)
    /// </summary>
    public class Style : MapElement {
        public static List<Style> ParseListElement(MapElement root) {
            List<Style> res = new List<Style>();

            foreach (MapElement style in root.Children) {
                if (style.Name == "apply") {
                    foreach (MapElement apply in style.Children) {
                        res.Add(ParseStyle(apply, style));
                    }
                } else {
                    res.Add(ParseStyle(style, null));
                }
            }

            return res;
        }

        public static Style ParseStyle(MapElement style, MapElement root) {
            if (root != null && root.Name == "apply") {
                if (style.Name == "parallax") {
                    // Apply styleground
                    return new Parallax(style, root);
                } else {
                    // Apply effect
                    return new Effect(style, root);
                }
            } else if (style.Name == "parallax") {
                // Styleground
                return new Parallax(style, null);
            } else {
                // Effect
                return new Effect(style, null);
            }
        }
    }

    /// <summary>
    /// Represents a styleground.
    /// </summary>
    public class Parallax : Style {
        public Parallax() {
            Name = "parallax";
        }

        public Parallax(MapElement style, MapElement root) : this() {
            Attributes = root != null
                ? style.MergeAttributes(root).Attributes
                : style.Attributes;
        }
    }

    /// <summary>
    /// Represents an effect style (e.g. black hole)
    /// </summary>
    public class Effect : Style {
        public Effect(MapElement style, MapElement root) {
            Attributes = root != null
                ? style.MergeAttributes(root).Attributes
                : style.Attributes;

            Name = style.Name;
        }
    }
}
