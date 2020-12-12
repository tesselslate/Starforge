using Starforge.Core;
using Starforge.MapStructure;
using Starforge.MapStructure.Encoding;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Starforge.Mod {

    public static class EntityRegistry {
        private static Dictionary<string, EntityCreator> Creators = new Dictionary<string, EntityCreator>();

        public static Entity Create(Level level, string name) {
            EntityData data = new EntityData(name);

            if (Creators.ContainsKey(name)) {
                return Creators[name](level, data);
            }
            else {
                return new Entity(level, data);
            }
        }

        public static Entity Create(Level level, BinaryMapElement el) {
            EntityData data = new EntityData(el);

            if (Creators.ContainsKey(data.Name)) {
                return Creators[data.Name](level, data);
            }
            else {
                return new Entity(level, data);
            }
        }

        public static Trigger CreateTrigger(Level level, BinaryMapElement el) {
            EntityData data = new EntityData(el);

            if (Creators.ContainsKey(data.Name)) {
                return (Trigger)Creators[data.Name](level, data);
            }
            else {
                return new Trigger(level, data);
            }
        }

        public static void Register(Type type) {
            try {
                if (type.IsSubclassOf(typeof(Entity))) {
                    EntityDefinitionAttribute attr = type.GetCustomAttribute<EntityDefinitionAttribute>();
                    if (attr != null) {
                        string id = attr.ID;
                        ConstructorInfo ctor = type.GetConstructor(new Type[]
                        {
                            typeof(Level),
                            typeof(EntityData)
                        });

                        if (ctor != null) {
                            Creators.Add(id, (Level l, EntityData e) => (Entity)ctor.Invoke(new object[] { l, e }));
                            Logger.Log($"Registered entity of ID {id}");
                        }
                        else {
                            Logger.Log(LogLevel.Error, $"Attempted to register entity of type ${type}, but could not find a suitable constructor");
                        }
                    }
                    else {
                        Logger.Log(LogLevel.Error, $"Attempted to register entity of type ${type}, but could not find a definition attribute");
                    }
                }
            }
            catch (Exception e) {
                Logger.Log(LogLevel.Error, $"Failed to register entity of type {type}");
                Logger.LogException(e);
            }
        }
    }

    public delegate Entity EntityCreator(Level level, EntityData data);
}
