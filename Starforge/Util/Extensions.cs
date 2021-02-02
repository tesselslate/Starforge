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
            Vector2 vec = new Vector2(area.X, area.Y);
            entity.Position = vec;
            if (area.Width > 0) entity.Width = area.Width;
            if (area.Height > 0) entity.Height = area.Height;
        }
    }
}
