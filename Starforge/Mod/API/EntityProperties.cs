using System.Collections;
using System.Collections.Generic;

namespace Starforge.Mod.API {

    public class Property {
        public string Name;
        public PropertyType Type;
        public string Description;

        public Property(string Name, PropertyType Type, string Description) {
            this.Name = Name;
            this.Type = Type;
            this.Description = Description;
        }
    }

    public enum PropertyType {
        String,
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
