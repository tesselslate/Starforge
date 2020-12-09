using Microsoft.Xna.Framework;
using Starforge.MapStructure;
using Starforge.MapStructure.Encoding;
using System.Collections.Generic;

namespace Starforge.Mod {
    public class EntityData {
        public readonly List<Vector2> Nodes = new List<Vector2>();
        public readonly string Name;
        public readonly BinaryMapElement Element;
        public readonly Dictionary<string, object> Attributes;

        public EntityData(BinaryMapElement bin) {
            Element = bin;

            Name = Element.Name;
            Nodes = new List<Vector2>();

            foreach(BinaryMapElement child in bin.Children) {
                if(child.Name == "nodes") {
                    foreach(BinaryMapElement el in child.Children) Nodes.Add(new Vector2(
                        el.GetFloat("x"),
                        el.GetFloat("y")
                    ));
                }
            }

            Attributes = new Dictionary<string, object>();
            foreach(KeyValuePair<string, object> pair in bin.Attributes) {
                Attributes.Add(pair.Key, pair.Value);
            }
        }

        public EntityData(string name) {
            Name = name;
            Nodes = new List<Vector2>();
            Element = default;
            Attributes = new Dictionary<string, object>();
        }
    }
}
