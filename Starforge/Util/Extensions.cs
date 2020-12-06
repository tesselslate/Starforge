using System;
using System.Xml;

namespace Starforge.Util {
    public static class Extensions {
        public static string Attr(this XmlElement el, string attributeName) {
            return el.Attributes[attributeName].InnerText;
        }

        public static char AttrChar(this XmlElement el, string attributeName) {
            return Convert.ToChar(el.Attributes[attributeName].InnerText);
        }
    }
}
