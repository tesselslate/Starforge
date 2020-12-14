using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Starforge.Core;
using Starforge.MapStructure.Encoding;
using Starforge.Mod;
using System.Collections.Generic;

namespace Starforge.MapStructure {
    public abstract class Entity : MapElement {
      
        public float Width {
            get => GetFloat("width");
            set => SetAttribute("width", value);
        }

        public float Height {
            get => GetFloat("height");
            set => SetAttribute("height", value);
        }

        public Vector2 Position;

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

            Position.X = GetFloat("x");
            Position.Y = GetFloat("y");
        }

        public Entity(string name) {
            Name = name;
        }

        public override BinaryMapElement ToBinary() {

            SetAttribute("x", Position.X);
            SetAttribute("y", Position.Y);

            BinaryMapElement bin = new BinaryMapElement() {
                Name = Name
            };

            foreach (KeyValuePair<string, object> pair in Attributes) bin.Attributes.Add(pair.Key, pair.Value);

            if (Nodes.Count == 0) {
                return bin;
            }

            foreach (Vector2 node in Nodes) {
                BinaryMapElement binNode = new BinaryMapElement() {
                    Name = "node"
                };

                binNode.SetAttribute("x", node.X);
                binNode.SetAttribute("y", node.Y);

                bin.Children.Add(binNode);
            }

            return bin;
        }

        public abstract void Render();
    }

    public class Trigger : Entity {
        public Trigger(Level level, EntityData data) : base(level, data) { }

        public override void Render() { }
    }

    public class UnknownEntity : Entity {
        private static readonly Color BgColor = Color.Cyan * 0.2f;
        private static readonly Color OutlineColor = Color.Cyan;

        public UnknownEntity(Level level, EntityData data) : base(level, data) { }

        public override void Render() {
            // default width/height of 4 for non-rectangular entities works pretty well
            Rectangle renderPos = new Rectangle((int)Position.X, (int)Position.Y, GetInt("width", 4), GetInt("height", 4));
            GFX.Pixel.Draw(renderPos, BgColor);
            GFX.Draw.HollowRectangle(renderPos, OutlineColor);
        }
    }
}
