using Starforge.MapStructure.Encoding;
using System.Collections.Generic;

namespace Starforge.MapStructure {
    public class BaseMapElement {
        public Dictionary<string, object> Attributes = new Dictionary<string, object>();

        public BaseMapElement MergeAttributes(BaseMapElement element) {
            foreach(KeyValuePair<string, object> pair in element.Attributes) {
                if(!Attributes.ContainsKey(pair.Key))
                    Attributes.Add(pair.Key, pair.Value);
            }

            return this;
        }

        public bool HasAttribute(string name) {
            return Attributes.ContainsKey(name);
        }

        protected object GetAttribute(string name, object defaultValue = null) {
            object obj;

            if(!Attributes.TryGetValue(name, out obj)) {
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

        public void SetAttribute(string name, object value) {
            if(value == null) {
                Attributes.Remove(name);
                return;
            }
            Attributes[name] = value;
        }
    }

    public abstract class MapElement : BaseMapElement {
        public MapElement() : base() { }

        public abstract BinaryMapElement ToBinary();
    }
}
