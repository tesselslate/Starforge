using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Starforge.Map {
    public class Entity : IPackable {
        public int ID;
        public readonly string Name;
        public List<Vector2> Nodes;
        public Vector2 Position;
        public Dictionary<string, object> Attributes;

        public Room Parent;

        public Entity(EntityData data, Room room) {
            Attributes = data.Attributes;

            Name = data.Name;
            Nodes = new List<Vector2>();

            foreach (Vector2 node in data.Nodes) Nodes.Add(node);
            Position = data.Position;
            Parent = room;
        }

        public MapElement Encode() {
            MapElement el = new MapElement()
            {
                Name = Name,
                Attributes = Attributes
            };

            foreach(Vector2 node in Nodes) {
                MapElement nodeEl = new MapElement() { Name = "node" };
                nodeEl.SetAttribute("x", node.X);
                nodeEl.SetAttribute("y", node.Y);
                el.Children.Add(nodeEl);
            }

            return el;
        }
    }

    public class EntityData {
        public readonly int ID;
        public readonly string Name;
        public readonly List<Vector2> Nodes;
        public readonly Vector2 Position;
        public readonly Vector2 Size;

        public readonly Dictionary<string, object> Attributes;

        public EntityData(MapElement el) {
            Attributes = new Dictionary<string, object>();
            foreach (KeyValuePair<string, object> attr in el.Attributes) Attributes.Add(attr.Key, attr.Value);

            ID = el.GetInt("id", -1);
            Name = el.Name;

            Nodes = new List<Vector2>();
            foreach (MapElement node in el.Children) Nodes.Add(new Vector2(node.GetFloat("x"), node.GetFloat("y")));

            Position = new Vector2(el.GetFloat("x"), el.GetFloat("y"));
            if (el.GetInt("width") > 0 || el.GetInt("height") > 0) Size = new Vector2(el.GetInt("width"), el.GetInt("height"));
        }
    }
}
