using Microsoft.Xna.Framework;
using Starforge.Mod.API;
using System.Collections.Generic;

namespace Starforge.Map {
    public abstract class Entity : AttributeHolder, IPackable {
        public int ID;
        public readonly string Name;
        public List<Vector2> Nodes;
        public Vector2 Position;

        public virtual PropertyList Properties => new PropertyList();

        public virtual Rectangle Hitbox => new Rectangle(
            (int)Position.X, 
            (int)Position.Y, 
            Width == 0 ? 4 : Width, 
            Height == 0 ? 4 : Height
        );

        public int Width {
            get => GetInt("width");
            set => Attributes["width"] = value;
        }

        public int Height {
            get => GetInt("height");
            set => Attributes["height"] = value;
        }

        public virtual bool StretchableX => false;
        public virtual bool StretchableY => false;

        public Room Room;

        public Entity(EntityData data, Room room) {
            Attributes = new Dictionary<string, object>(data.Attributes);

            Name = data.Name;
            Nodes = new List<Vector2>(data.Nodes);

            Position = new Vector2((int)GetFloat("x"), (int)GetFloat("y"));
            Room = room;
        }

        public MapElement Encode() {
            MapElement el = new MapElement() {
                Name = Name,
                Attributes = new Dictionary<string, object>(Attributes)
            };

            el.SetAttribute("x", Position.X);
            el.SetAttribute("y", Position.Y);
            el.SetAttribute("id", ID);

            foreach (Vector2 node in Nodes) {
                MapElement nodeEl = new MapElement() { Name = "node" };
                nodeEl.SetAttribute("x", node.X);
                nodeEl.SetAttribute("y", node.Y);
                el.Children.Add(nodeEl);
            }

            return el;
        }

        public bool ContainsPosition(Point pos) {
            return Hitbox.Contains(pos);
        }

        public abstract void Render();
    }
}
