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

        public Entity(MapElement el, Room room) {
            Attributes = new Dictionary<string, object>(el.Attributes);

            Name = el.Name;
            Nodes = new List<Vector2>();

            foreach (MapElement node in el.Children) Nodes.Add(new Vector2(node.GetFloat("x"), node.GetFloat("y")));
            Position = new Vector2(el.GetFloat("x"), el.GetFloat("y"));
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
}
