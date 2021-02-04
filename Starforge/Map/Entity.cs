using Microsoft.Xna.Framework;
using Starforge.Mod.API;
using System;
using System.Collections.Generic;

namespace Starforge.Map {
    public abstract class Entity : AttributeHolder, IPackable {
        public int ID;
        public readonly string Name;
        public List<Vector2> Nodes;

        public virtual PropertyList Properties => new PropertyList();

        public virtual Rectangle Hitbox => new Rectangle(
            (int)Position.X, 
            (int)Position.Y, 
            Width == 0 ? 4 : Width, 
            Height == 0 ? 4 : Height
        );

        public Vector2 Position {
            get => new Vector2(GetFloat("x"), GetFloat("y"));
            set {
                Attributes["x"] = value.X;
                Attributes["y"] = value.Y;
            }
        }

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

        // Returns the Region of the entity the point is in
        public EntityRegion GetEntityRegion(Point pos) {
            if (!ContainsPosition(pos)) {
                return EntityRegion.Outside;
            }
            EntityRegion Horizontal = EntityRegion.Middle;
            EntityRegion Vertical = EntityRegion.Middle;

            int SelectionWidth = (int)Math.Min(Hitbox.Width / 3f, 8f);
            int SelectionHeight = (int)Math.Min(Hitbox.Height / 3f, 8f);

            if (StretchableX) {
                // figure out if left, right or middle horizontally
                if (pos.X < Hitbox.X + SelectionWidth) {
                    Horizontal = EntityRegion.Left;
                }
                else if (pos.X < Hitbox.X + Hitbox.Width - SelectionWidth) {
                    Horizontal = EntityRegion.Middle;
                }
                else {
                    Horizontal = EntityRegion.Right;
                }
            }
            if (StretchableY) {
                // figure out if top, bottom or middle vertically
                if (pos.Y < Hitbox.Y + SelectionHeight) {
                    Vertical = EntityRegion.Top;
                }
                else if (pos.Y < Hitbox.Y + Hitbox.Height - SelectionHeight) {
                    Vertical = EntityRegion.Middle;
                }
                else {
                    Vertical = EntityRegion.Bottom;
                }
            }

            return Vertical | Horizontal;
        }

        public abstract void Render();
    }

    // Names for the different corners and sides of the entity that can be dragged
    // The values are such that the corners are just a combination of the two sides, e.g. TopLeft = Top | Left
    // Outside is the 0 Element, i.e. for every x: x | Outside = Outside
    // Middle is the 1 Element, i.e. for every x: x | Middle = x
    public enum EntityRegion {
        Outside = -1,
        Middle = 0,
        Top = 1 << 0,
        Bottom = 1 << 1,
        Left = 1 << 2,
        Right = 1 << 3,
        TopLeft = Top | Left,
        TopRight = Top | Right,
        BottomLeft = Bottom | Left,
        BottomRight = Bottom | Right
    }

}
