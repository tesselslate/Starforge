using Starforge.Util;
using System.Collections;
using System.Collections.Generic;

namespace Starforge.Mod.API {

    public class Property {
        public string Name { get; private set; }
        public PropertyType Type { get; private set; }
        public string Description { get; private set; }

        // Reserved for List Property Types, represents all possible values
        public SortedDictionary<string, object> Values { get; private set; }

        // Additionally display strings saved as a string to pass to imgui
        public string[] DisplayValues;
        public string SelectedEntry;

        // Constructor for normal input field
        public Property(string Name, PropertyType Type, string Description) {
            this.Name = Name;
            this.Type = Type;
            this.Description = Description;
        }

        // Constructor for dropdown input field for strings
        public Property(string Name, string[] Values, string Description) {
            this.Name = Name;
            this.Type = PropertyType.List;
            this.Description = Description;
            this.Values = new SortedDictionary<string, object>();
            DisplayValues = new string[Values.Length];
            int i = 0;
            foreach (string v in Values) {
                this.Values.Add(MiscHelper.CleanCamelCase(v), v);
                this.DisplayValues[i++] = MiscHelper.CleanCamelCase(v);
            }
        }

        // Constructor for dictionary lists with names for a set of values
        public Property(string Name, Dictionary<string, object> Dictionary, string Description) {
            this.Name = Name;
            this.Type = PropertyType.List;
            this.Description = Description;
            this.Values = new SortedDictionary<string, object>(Dictionary);
            DisplayValues = new string[Dictionary.Count];
            int i = 0;
            foreach (KeyValuePair<string, object> pair in Values) {
                this.DisplayValues[i++] = MiscHelper.CleanCamelCase(pair.Key);
            }
        }

    }

    public enum PropertyType {
        String,
        Char,
        Integer,
        Float,
        Bool,
        List
    }

    public class PropertyList : ICollection<Property> {
        private List<Property> List = new List<Property>();

        public int Count => List.Count;
        public bool IsReadOnly => false;

        public IEnumerator<Property> GetEnumerator() => List.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => List.GetEnumerator();

        public void Add(Property property) => List.Add(property);
        public void Clear() => List.Clear();
        public bool Contains(Property property) => List.Contains(property);
        public void CopyTo(Property[] property, int loc) => List.CopyTo(property, loc);
        public bool Remove(Property property) => List.Remove(property);
    }
}
