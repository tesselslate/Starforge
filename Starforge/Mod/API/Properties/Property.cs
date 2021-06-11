using Starforge.Map;
using System.Collections;
using System.Collections.Generic;

namespace Starforge.Mod.API {

    public abstract class Property {
        public string Name { get; protected set; }
        public string Description { get; protected set; }
        public object DefaultValue { get; protected set; }
        public abstract bool RenderGUI(Entity mainEntity, List<Entity> entities);

        public Property(string name, object defaultValue, string description) {
            Name = name;
            Description = description;
            DefaultValue = defaultValue;
        }
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
