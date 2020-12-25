using System;
using System.Collections.Generic;

namespace Starforge.Map {
    /// <summary>
    /// A class which holds attributes.
    /// </summary>
    public class AttributeHolder {
        public Dictionary<string, object> Attributes;

        protected object GetAttribute(string name, object defaultValue = null) {
            if (!Attributes.TryGetValue(name, out object obj)) {
                return defaultValue;
            }

            return obj;
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
    }
}
