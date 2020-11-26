using System;

namespace Starforge.Mod.API {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class EntityDefinitionAttribute : Attribute {
        public EntityDefinitionAttribute(string id) {
            ID = id;
        }

        public string ID;
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class EffectDefinitionAttribute : Attribute {
        public EffectDefinitionAttribute(string id) {
            ID = id;
        }

        public string ID;
    }
}
