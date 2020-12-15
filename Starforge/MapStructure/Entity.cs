using Microsoft.Xna.Framework;
using Starforge.Core;
using Starforge.MapStructure.Encoding;
using Starforge.Mod;
using System.Collections.Generic;

namespace Starforge.MapStructure {
    public abstract class Entity : MapElement {
      
        public Vector2 Position { get; private set; }
        public float Width { get; private set; }
        public float Height { get; private set; }

        public readonly string Name;

        public List<Vector2> Nodes;
        public Level Level;

        public bool StretchableX = false;
        public bool StretchableY = false;

        public Entity(Level level, EntityData data) {
            Name = data.Name;
            Nodes = new List<Vector2>();

            if (data.Nodes.Count > 0) {
                foreach (Vector2 node in data.Nodes) {
                    Nodes.Add(node);
                }
            }

            Level = level;
            foreach (KeyValuePair<string, object> pair in data.Attributes) Attributes.Add(pair.Key, pair.Value);

            Position = new Vector2(GetFloat("x"), GetFloat("y"));
            Width = GetFloat("width");
            Height = GetFloat("height");
        }

        public Entity(string name) {
            Name = name;
        }

        public void SetPosition(Vector2 pos) {
            Position = pos;
            SetAttribute("x", Position.X);
            SetAttribute("y", Position.Y);
        }

        public void SetPosition(float x, float y) {
            SetPosition(new Vector2(x, y));
        }

        public void SetWidth(float width) {
            if (width != 0 && StretchableX) {
                SetAttribute("width", width);
                Width = width;
            }
        }

        public void SetHeight(float height) {
            if (height != 0 && StretchableY) {
                SetAttribute("height", height);
                Height = height;
            }
        }

        public void SetArea(Rectangle rect) {
            SetPosition(new Vector2(rect.X, rect.Y));
            SetWidth(rect.Width);
            SetHeight(rect.Height);
        }

        public void SetArea(float x, float y, float width, float height) {
            SetPosition(x, y);
            SetWidth(width);
            SetHeight(height);
        }

        public override BinaryMapElement ToBinary() {

            SetAttribute("x", Position.X);
            SetAttribute("y", Position.Y);
            SetAttribute("width", Width);
            SetAttribute("height", Height);

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
