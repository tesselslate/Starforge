using Microsoft.Xna.Framework;
using Starforge.MapStructure.Encoding;
using Starforge.Mod;
using System.Collections.Generic;

namespace Starforge.MapStructure {
    public class Entity : MapElement {
        public float X {
            get => GetFloat("x");
            set => SetAttribute("x", value);
        }

        public float Y {
            get => GetFloat("y");
            set => SetAttribute("y", value);
        }

        public float Width {
            get => GetFloat("width");
            set => SetAttribute("width", value);
        }

        public float Height {
            get => GetFloat("height");
            set => SetAttribute("height", value);
        }

        public Vector2 Position {
            get => new Vector2(X, Y);
        }

        public readonly string Name;

        public List<Vector2> Nodes;
        public Level Level;

        public Entity(Level level, EntityData data) {
            Name = data.Name;
            Nodes = new List<Vector2>();

            if (data.Nodes.Count > 0) {
                foreach (Vector2 node in data.Nodes) Nodes.Add(node);
            }

            Level = level;
            foreach (KeyValuePair<string, object> pair in data.Attributes) Attributes.Add(pair.Key, pair.Value);
        }

        public Entity(string name) {
            Name = name;
        }

        public override BinaryMapElement ToBinary() {
            BinaryMapElement bin = new BinaryMapElement() {
                Name = Name
            };

            foreach (KeyValuePair<string, object> pair in Attributes) bin.Attributes.Add(pair.Key, pair.Value);

            if (Nodes.Count > 0) {
                foreach (Vector2 node in Nodes) {
                    BinaryMapElement binNode = new BinaryMapElement() {
                        Name = "node"
                    };

                    binNode.SetAttribute("x", node.X);
                    binNode.SetAttribute("y", node.Y);

                    bin.Children.Add(binNode);
                }
            }

            return bin;
        }

        public virtual void Render() { }
    }

    public class Trigger : Entity {
        public Trigger(Level level, EntityData data) : base(level, data) { }
    }
}
