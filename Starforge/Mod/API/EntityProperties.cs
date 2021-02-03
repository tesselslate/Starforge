using Starforge.Util;
using System.Collections;
using System.Collections.Generic;

namespace Starforge.Mod.API {

    public class Property {
        public string Name { get; private set; }
        public PropertyType Type { get; private set; }
        public string Description { get; private set; }

        // Is true if there is a list of possible values for a drop down
        public bool List { get; private set; }

        // Reserved for List Property Types, represents all possible values
        public string[] Values { get; private set; }
        // Values to be displayed in a dropdown menu, with CamelCase cleaned names
        public string[] DisplayValues { get; private set; }

        // Constructor for normal input field
        public Property(string Name, PropertyType Type, string Description) {
            this.Name = Name;
            this.Type = Type;
            this.Description = Description;
            this.List = false;
        }

        // Constructor for dropdown input field for strings
        public Property(string Name, string[] Values, string Description) {
            this.Name = Name;
            this.Type = PropertyType.String;
            this.List = true;
            this.Description = Description;
            this.Values = Values;
            DisplayValues = new string[Values.Length];
            for (int i = 0; i < DisplayValues.Length; ++i) {
                DisplayValues[i] = MiscHelper.CleanCamelCase(Values[i]);
            }
        }

    }

    public enum PropertyType {
        String,
        Char,
        Integer,
        Float,
        Bool
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
