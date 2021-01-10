using Starforge.Map;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Starforge.Mod.API {
    public class Placement {
        public Dictionary<string, object> Attributes = new Dictionary<string, object>();
        public string Name;
        public Type Parent;

        public object this[string key] {
            get => Attributes[key];
            set => Attributes[key] = value;
        }

        public Placement(string name) {
            Name = name;
        }

        public Entity Create(Room room) {
            Entity e = (Entity)Activator.CreateInstance(Parent, new object[] {
                new EntityData(Parent.GetCustomAttribute<EntityDefinitionAttribute>().ID) {
                    Attributes = new Dictionary<string, object>(Attributes)
                },
                room
            });
            e.Attributes = new Dictionary<string, object>(Attributes);

            return e;
        }
    }

    /// <summary>
    /// Contains a list of available predefined entity placements.
    /// </summary>
    public class PlacementList : ICollection<Placement> {
        private List<Placement> List = new List<Placement>();

        public int Count => List.Count;
        public bool IsReadOnly => false;

        public IEnumerator<Placement> GetEnumerator() => List.GetEnumerator();
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => List.GetEnumerator();

        public void Add(Placement placement) => List.Add(placement);
        public void Clear() => List.Clear();
        public bool Contains(Placement placement) => List.Contains(placement);
        public void CopyTo(Placement[] placements, int loc) => List.CopyTo(placements, loc);
        public bool Remove(Placement placement) => List.Remove(placement);
    }
}
