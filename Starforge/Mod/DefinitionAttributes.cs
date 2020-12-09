using System;

namespace Starforge.Mod {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class EffectDefinitionAttribute : Attribute {
        public EffectDefinitionAttribute(string name) {
            ID = name;
        }

        public readonly string ID;
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class EntityDefinitionAttribute : Attribute {
        public EntityDefinitionAttribute(string name) {
            ID = name;
        }

        public readonly string ID;
    }
}
