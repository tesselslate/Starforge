using Microsoft.Xna.Framework;
using Starforge.MapStructure.Encoding;
using System.Collections.Generic;

namespace Starforge.Mod {
    public class EntityData {
        public readonly List<Vector2> Nodes = new List<Vector2>();
        public readonly string Name;
        public readonly BinaryMapElement Element;
        public readonly Dictionary<string, object> Attributes = new Dictionary<string, object>();

        public EntityData(BinaryMapElement bin) {
            Element = bin;

            Name = Element.Name;
            Nodes = new List<Vector2>();

            foreach (BinaryMapElement child in bin.Children) {
                if (child.Name == "nodes") {
                    foreach (BinaryMapElement el in child.Children) Nodes.Add(new Vector2(
                         el.GetFloat("x"),
                         el.GetFloat("y")
                     ));
                }
            }

            Attributes = new Dictionary<string, object>();
            foreach (KeyValuePair<string, object> pair in bin.Attributes) {
                Attributes.Add(pair.Key, pair.Value);
            }
        }

        public EntityData(string name) {
            Name = name;
            Nodes = new List<Vector2>();
            Element = default;
            Attributes = new Dictionary<string, object>();
        }

        public bool HasAttribute(string name) {
            return Attributes.ContainsKey(name);
        }

        protected object GetAttribute(string name, object defaultValue = null) {
            object obj;

            if (!Attributes.TryGetValue(name, out obj)) {
                return defaultValue;
            }

            return obj;
        }

        public bool GetBool(string name, bool defaultValue = false) {
            return bool.Parse(GetAttribute(name, defaultValue).ToString());
        }

        public float GetFloat(string name, float defaultValue = 0f) {
            return float.Parse(GetAttribute(name, defaultValue).ToString());
        }

        public int GetInt(string name, int defaultValue = 0) {
            return int.Parse(GetAttribute(name, defaultValue).ToString());
        }

        public string GetString(string name, string defaultValue = "") {
            return GetAttribute(name, defaultValue).ToString();
        }
    }
}
