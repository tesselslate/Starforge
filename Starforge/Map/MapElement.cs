using System;
using System.Collections.Generic;

namespace Starforge.Map {
    /// <summary>
    /// Represents a decoded element from the binary map format.
    /// </summary>
    public class MapElement {
        public Dictionary<string, object> Attributes;
        public List<MapElement> Children;
        public string Name;

        /// <summary>
        /// Used only in the root map element, to give it a name.
        /// </summary>
        public string Package;

        public MapElement() {
            Attributes = new Dictionary<string, object>();
            Children = new List<MapElement>();
        }

        protected object GetAttribute(string name, object defaultValue = null) {
            if (!Attributes.TryGetValue(name, out object obj)) {
                return defaultValue;
            }

            return obj;
        }

        /// <summary>
        /// Adds a list of packable elements to this element.
        /// </summary>
        /// <typeparam name="T">A class which implements IPackable.</typeparam>
        /// <param name="items">The list of items to encode.</param>
        /// <param name="name">The name of the child element to add the items to.</param>
        public void AddList<T>(List<T> items, string name) where T : IPackable {
            if (items.Count == 0) return;

            MapElement childEl = new MapElement() { Name = name };
            foreach (IPackable toPack in items) {
                childEl.Children.Add(toPack.Encode());
            }

            Children.Add(childEl);
        }

        /// <summary>
        /// Gets a boolean attribute from the MapElement.
        /// </summary>
        /// <param name="name">The name of the attribute.</param>
        /// <param name="defaultValue">The value to return if the attribute doesn't exist.</param>
        /// <exception cref="FormatException">If the supplied attribute is not a boolean.</exception>
        /// <returns>The boolean value of the attribute if it exists, otherwise the defaultValue provided.</returns>
        public bool GetBool(string name, bool defaultValue = false) {
            return bool.Parse(GetAttribute(name, defaultValue).ToString());
        }

        /// <summary>
        /// Gets a float attribute from the MapElement.
        /// </summary>
        /// <param name="name">The name of the attribute.</param>
        /// <param name="defaultValue">The value to return if the attribute doesn't exist.</param>
        /// <exception cref="FormatException">If the supplied attribute is not a float.</exception>
        /// <returns>The float value of the attribute if it exists, otherwise the defaultValue provided.</returns>
        public float GetFloat(string name, float defaultValue = 0f) {
            return float.Parse(GetAttribute(name, defaultValue).ToString());
        }

        /// <summary>
        /// Gets an integer attribute from the MapElement.
        /// </summary>
        /// <param name="name">The name of the attribute.</param>
        /// <param name="defaultValue">The value to return if the attribute doesn't exist.</param>
        /// <exception cref="FormatException">If the supplied attribute is not an integer.</exception>
        /// <returns>The integer value of the attribute if it exists, otherwise the defaultValue provided.</returns>
        public int GetInt(string name, int defaultValue = 0) {
            return int.Parse(GetAttribute(name, defaultValue).ToString());
        }

        /// <summary>
        /// Gets a string attribute from the MapElement.
        /// </summary>
        /// <param name="name">The name of the attribute.</param>
        /// <param name="defaultValue">The value to return if the attribute doesn't exist.</param>
        /// <returns>The string value of the attribute if it exists, otherwise the defaultValue provided.</returns>
        public string GetString(string name, string defaultValue = "") {
            return GetAttribute(name, defaultValue).ToString();
        }

        /// <summary>
        /// Merges the attributes of another map element into the current one.
        /// Any duplicate attributes are overwritten with the element they are being merged from.
        /// </summary>
        /// <param name="el">The MapElement to take attributes from.</param>
        /// <returns>The merged map element.</returns>
        public MapElement MergeAttributes(MapElement el) {
            foreach (KeyValuePair<string, object> pair in el.Attributes) {
                Attributes[pair.Key] = pair.Value;
            }

            return this;
        }

        public void SetAttribute(string key, object value) {
            if (value == null || string.IsNullOrEmpty(value.ToString())) {
                if (Attributes.ContainsKey(key)) {
                    Attributes.Remove(key);
                }
                return;
            }

            Attributes[key] = value;
        }
    }

    /// <summary>
    /// Represents a class which can be converted to the MapElement format.
    /// </summary>
    public interface IPackable {
        /// <summary>
        /// Encodes the class into a MapElement for writing a binary map.
        /// </summary>
        /// <returns>The MapElement representation of the class.</returns>
        MapElement Encode();
    }
}
