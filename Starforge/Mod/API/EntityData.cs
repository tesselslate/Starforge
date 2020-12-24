using Microsoft.Xna.Framework;
using Starforge.Map;
using System.Collections.Generic;

namespace Starforge.Mod.API {
    /// <summary>
    /// Contains data about an entity.
    /// </summary>
    public class EntityData {
        public readonly string Name;
        public readonly MapElement Element;
        public readonly List<Vector2> Nodes = new List<Vector2>();
        public readonly Dictionary<string, object> Attributes;

        public EntityData(MapElement el) {
            Element = el;
            Name = el.Name;
            Nodes = new List<Vector2>();

            foreach(MapElement child in el.Children) Nodes.Add(new Vector2(child.GetFloat("x"), child.GetFloat("y")));
            Attributes = new Dictionary<string, object>(el.Attributes);
        }

        public EntityData(string name) {
            Name = name;
            Nodes = new List<Vector2>();
            Attributes = new Dictionary<string, object>();
        }
    }
}
