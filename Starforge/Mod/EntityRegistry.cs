using Starforge.Core;
using Starforge.Map;
using Starforge.Mod.API;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Starforge.Mod {
    public static class EntityRegistry {
        private static List<Placement> EntityPlacements;
        private static Dictionary<string, EntityCreator> EntityCreators;
        private static Dictionary<Type, PropertyInfo[]> EntityCache;

        static EntityRegistry() {
            EntityPlacements = new List<Placement>();
            EntityCreators = new Dictionary<string, EntityCreator>();
            EntityCache = new Dictionary<Type, PropertyInfo[]>();
        }

        public static Entity CreateEntity(MapElement el, Room room) {
            EntityData data = new EntityData(el);

            if (EntityCreators.ContainsKey(el.Name)) {
                return EntityCreators[el.Name](data, room);
            } else {
                return new UnknownEntity(data, room);
            }
        }

        public static void Register(Type type) {
            try {
                if (!type.IsSubclassOf(typeof(Entity))) return;

                EntityDefinitionAttribute attr = type.GetCustomAttribute<EntityDefinitionAttribute>();
                if (attr != null) {
                    string id = attr.ID;
                    ConstructorInfo ctor = type.GetConstructor(new Type[]
                    {
                        typeof(EntityData),
                        typeof(Room)
                    });

                    if (ctor != null) {
                        EntityCache.Add(type, type.GetProperties());

                        FieldInfo placementsField = null;
                        if ((placementsField = type.GetField("Placements", BindingFlags.Public | BindingFlags.Static)) != null) {
                            if (placementsField.FieldType == typeof(PlacementList)) {
                                PlacementList placements = (PlacementList)placementsField.GetValue(null);

                                foreach (Placement p in placements) {
                                    p.Parent = type;
                                    EntityPlacements.Add(p);
                                }

                                Logger.Log($"Registered {placements.Count} placements for {attr.ID}");
                            }
                        }

                        EntityCreators.Add(id, (EntityData d, Room r) => (Entity)ctor.Invoke(new object[] { d, r }));
                        Logger.Log($"Registered entity {id} of type {type}");
                    } else {
                        Logger.Log(LogLevel.Error, $"Entity of type {type} with ID {id} does not have a valid ctor");
                    }
                } else {
                    Logger.Log(LogLevel.Error, $"Entity {type} does not have a definition attribute");
                }
            } catch (Exception e) {
                Logger.Log(LogLevel.Error, $"Encountered an error while attempting to register entity {type}");
                Logger.LogException(e);
            }
        }

        /// <returns>A list of all registered entity IDs.</returns>
        public static List<string> GetRegisteredEntities() {
            return new List<string>(EntityCreators.Keys);
        }

        /// <returns>A list of all registered entity placements.</retrurns>
        public static Dictionary<string, Placement> GetRegisteredPlacements() {
            Dictionary<string, Placement> res = new Dictionary<string, Placement>();

            foreach (Placement p in EntityPlacements) {
                res.Add(p.Name, p);
            }

            return res;
        }
    }

    public delegate Entity EntityCreator(EntityData data, Room room);
}
