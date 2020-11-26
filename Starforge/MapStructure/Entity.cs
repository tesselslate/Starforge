using Starforge.Core;
using Starforge.Mod.API;
using Starforge.MapStructure.Encoding;
using Starforge.Util;
using System.Reflection;
using System.Collections.Generic;

namespace Starforge.MapStructure {
    public class Entity : MapElement {
        public readonly string Name;

        public List<Node> Nodes;
        public Level Level;

        public Rectangle SelectionHitbox;

        public Entity(Level level, BinaryMapElement data) {
            Name = data.Name;

            if(data.Children.Count > 0) {
                Nodes = new List<Node>();
                foreach(BinaryMapElement node in data.Children) {
                    Nodes.Add(new Node(node));
                }
            }

            // Create default selection hitbox for placeholder entities
            SelectionHitbox = new Rectangle(
                data.GetInt("x"),
                data.GetInt("y"),
                8,
                8
            );

            Level = level;
            Attributes = data.Attributes;
        }

        public override BinaryMapElement ToBinary() {
            BinaryMapElement bin = new BinaryMapElement()
            {
                Name = Name
            };

            bin.Attributes = Attributes;

            if(Nodes != null) {
                foreach(Node node in Nodes) {
                    BinaryMapElement binNode = new BinaryMapElement()
                    {
                        Name = "node"
                    };

                    binNode.SetAttribute("x", node.X);
                    binNode.SetAttribute("y", node.Y);

                    bin.Children.Add(binNode);
                }
            }

            return bin;
        }
    }

    public class Trigger : Entity {
        public int Width {
            get => GetInt("width");
            set => SetAttribute("width", value);
        }

        public int Height {
            get => GetInt("height");
            set => SetAttribute("height", value);
        }

        public Trigger(Level level, BinaryMapElement data) : base(level, data) { }
    }

    public struct Node {
        public int X;
        public int Y;

        public Node(BinaryMapElement element) {
            if(element.Name != "node")
                Logger.Log(LogLevel.Warning, "Attempted to create entity node from " + element.Name);

            X = element.GetInt("x", 0);
            Y = element.GetInt("y", 0);
        }
    }
}
