using Starforge.Core;
using Starforge.MapStructure.Encoding;
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

        public readonly string Name;

        public List<Node> Nodes;
        public Level Level;

        public Entity(Level level, BinaryMapElement data) {
            Name = data.Name;

            if(data.Children.Count > 0) {
                Nodes = new List<Node>();
                foreach(BinaryMapElement node in data.Children) {
                    Nodes.Add(new Node(node));
                }
            }

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
        public Trigger(Level level, BinaryMapElement data) : base(level, data) { }
    }

    public struct Node {
        public float X;
        public float Y;

        public Node(BinaryMapElement element) {
            if(element.Name != "node")
                Logger.Log(LogLevel.Warning, "Attempted to create entity node from " + element.Name);

            X = element.GetFloat("x", 0);
            Y = element.GetFloat("y", 0);
        }
    }
}
