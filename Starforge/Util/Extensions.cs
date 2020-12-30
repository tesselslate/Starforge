using Microsoft.Xna.Framework;
using Starforge.Map;
using System;
using System.Xml;

namespace Starforge.Util {
    public static class Extensions {
        public static string Attr(this XmlElement el, string attributeName) {
            return el.Attributes[attributeName].InnerText;
        }

        public static char AttrChar(this XmlElement el, string attributeName) {
            return Convert.ToChar(el.Attributes[attributeName].Value);
        }

        public static Vector2 Perpendicular(this Vector2 vector) {
            return new Vector2(-vector.Y, vector.X);
        }

        public static void SetArea(this Entity entity, Rectangle area) {
            entity.Position.X = area.X;
            entity.Position.Y = area.Y;
            entity.Attributes["width"] = area.Width;
            entity.Attributes["height"] = area.Height;
        }
    }
}