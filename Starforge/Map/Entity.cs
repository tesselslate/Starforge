using Microsoft.Xna.Framework;
using Starforge.Mod.API;
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
            Attributes = new Dictionary<string, object>(data.Attributes);

            Name = data.Name;
            Nodes = new List<Vector2>(data.Nodes);

            Position = new Vector2((int)data.Attributes["x"], (int)data.Attributes["y"]);
            Parent = room;
        }

        public MapElement Encode() {
            MapElement el = new MapElement()
            {
                Name = Name,
                Attributes = Attributes
            };

            el.SetAttribute("x", Position.X);
            el.SetAttribute("y", Position.Y);
            el.SetAttribute("id", ID);

            foreach(Vector2 node in Nodes) {
                MapElement nodeEl = new MapElement() { Name = "node" };
                nodeEl.SetAttribute("x", node.X);
                nodeEl.SetAttribute("y", node.Y);
                el.Children.Add(nodeEl);
            }

            return el;
        }

        public virtual void Render() { }
    }
}
