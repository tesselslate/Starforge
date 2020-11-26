using Starforge.Core;
using Starforge.MapStructure;
using Starforge.MapStructure.Encoding;
using Starforge.Mod.API;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Starforge.Mod {
    public static class EntityRegistry {
        private static List<Type> RegisteredTypes
            = new List<Type>();

        public static Dictionary<string, EntityCreator> Creators
            = new Dictionary<string, EntityCreator>();

        public static void RegisterEntity(Type type) {
            if(RegisteredTypes.Contains(type))
                return;

            EntityDefinitionAttribute attr = type.GetCustomAttribute<EntityDefinitionAttribute>();
            if(attr != null) {
                string id = attr.ID;
                EntityCreator creator = null;

                ConstructorInfo ctor = type.GetConstructor(new Type[] {
                    typeof(Level),
                    typeof(BinaryMapElement)
                });
                if(ctor != null) {
                    creator = ((Level level, BinaryMapElement data) => (Entity)ctor.Invoke(new object[] {
                        level, data
                    }));
                }

                if(creator != null) {
                    Creators.Add(id, creator);
                    RegisteredTypes.Add(type);
                } else {
                    Logger.Log(LogLevel.Warning, "No suitable constructor for entity " + id);
                }
            } else {
                Logger.Log(LogLevel.Warning, "Type " + type.ToString() + " does not contain an EntityDefinition attribute.");
            }
        }

        public static Entity CreateEntity(string id, Level level, BinaryMapElement el = null) {
            if(Creators.ContainsKey(id)) {
                if(el != null) {
                    return Creators[id](level, el);
                } else {
                    return Creators[id](level, new BinaryMapElement()
                    {
                        Name = id
                    });
                }
            } else {
                if(el != null) {
                    return new Entity(level, el);
                } else {
                    return new Entity(level, new BinaryMapElement()
                    {
                        Name = id
                    });
                }
            }
        }
    }

    public static class TriggerRegistry {
        private static List<Type> RegisteredTypes
            = new List<Type>();

        public static Dictionary<string, TriggerCreator> Creators
            = new Dictionary<string, TriggerCreator>();

        public static void RegisterTrigger(Type type) {
            if(RegisteredTypes.Contains(type))
                return;

            TriggerDefinitionAttribute attr = type.GetCustomAttribute<TriggerDefinitionAttribute>();
            if(attr != null) {
                string id = attr.ID;
                TriggerCreator creator = null;

                ConstructorInfo ctor = type.GetConstructor(new Type[] {
                    typeof(Level),
                    typeof(BinaryMapElement)
                });
                if(ctor != null) {
                    creator = ((Level level, BinaryMapElement data) => (Trigger)ctor.Invoke(new object[] {
                        level, data
                    }));
                }

                if(creator != null) {
                    Creators.Add(id, creator);
                    RegisteredTypes.Add(type);
                } else {
                    Logger.Log(LogLevel.Warning, "No suitable constructor for trigger " + id);
                }
            } else {
                Logger.Log(LogLevel.Warning, "Type " + type.ToString() + " does not contain an TriggerDefinition attribute.");
            }
        }

        public static Trigger CreateTrigger(string id, Level level, BinaryMapElement el = null) {
            if(Creators.ContainsKey(id)) {
                if(el != null) {
                    return Creators[id](level, el);
                } else {
                    return Creators[id](level, new BinaryMapElement()
                    {
                        Name = id
                    });
                }
            } else {
                if(el != null) {
                    return new Trigger(level, el);
                } else {
                    return new Trigger(level, new BinaryMapElement()
                    {
                        Name = id
                    });
                }
            }
        }
    }

    public static class EffectRegistry {
        private static List<Type> RegisteredTypes
            = new List<Type>();

        public static Dictionary<string, EffectCreator> Creators
            = new Dictionary<string, EffectCreator>();

        public static void RegisterEffect(Type type) {
            if(RegisteredTypes.Contains(type))
                return;

            EffectDefinitionAttribute attr = type.GetCustomAttribute<EffectDefinitionAttribute>();
            if(attr != null) {
                string id = attr.ID;
                EffectCreator creator = null;

                ConstructorInfo ctor = type.GetConstructor(new Type[]
                {
                    typeof(BinaryMapElement)
                });
                if(ctor != null) {
                    creator = ((BinaryMapElement data) => (Effect)ctor.Invoke(new object[]
                    {
                        data
                    }));
                } else {
                    creator = ((BinaryMapElement data) => (Effect)ctor.Invoke(new object[] { }));
                }

                Creators.Add(id, creator);
                RegisteredTypes.Add(type);
            } else {
                Logger.Log(LogLevel.Warning, "Type " + type.ToString() + " does not contain an EffectDefinition attribute.");
            }
        }

        public static Effect CreateEffect(string id, BinaryMapElement el = null) {
            if(Creators.ContainsKey(id)) {
                if(el != null) {
                    return Creators[id](el);
                } else {
                    return Creators[id](new BinaryMapElement()
                    {
                        Name = id
                    });
                }
            } else {
                if(el != null) {
                    return new Effect(el);
                } else {
                    return new Effect(new BinaryMapElement()
                    {
                        Name = id
                    });
                }
            }
        }
    }

    public delegate Entity EntityCreator(Level level, BinaryMapElement data);

    public delegate Trigger TriggerCreator(Level level, BinaryMapElement data);

    public delegate Effect EffectCreator(BinaryMapElement data);
}
