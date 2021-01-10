using System;

namespace Starforge.Mod.API {
    /// <summary>
    /// Used to identify a custom effect style.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class EffectDefinitionAttribute : Attribute {
        public EffectDefinitionAttribute(string name) {
            ID = name;
        }

        public readonly string ID;
    }

    /// <summary>
    /// Used to identify a custom entity.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class EntityDefinitionAttribute : Attribute {
        public EntityDefinitionAttribute(string name) {
            ID = name;
        }

        public readonly string ID;
    }

    /// <summary>
    /// Used to identify a custom trigger.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class TriggerDefinitionAttribute : EntityDefinitionAttribute {
        public TriggerDefinitionAttribute(string name) : base(name) { }
    }
}