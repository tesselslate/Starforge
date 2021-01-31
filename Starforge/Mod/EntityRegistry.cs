using Starforge.Core;
using Starforge.Map;
using Starforge.Mod.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Starforge.Mod {
    public static class EntityRegistry {
        public static List<Placement> EntityPlacements;
        private static Dictionary<string, EntityCreator> EntityCreators;

        static EntityRegistry() {
            EntityPlacements = new List<Placement>();
            EntityCreators = new Dictionary<string, EntityCreator>();
        }

        public static Entity CreateEntity(MapElement el, Room room) {
            EntityData data = new EntityData(el);
            Entity entity = EntityCreators.ContainsKey(el.Name) ? EntityCreators[el.Name](data, room) : new UnknownEntity(data, room);
            entity.ID = el.GetInt("id", 0);
            return entity;
        }

        public static void Register(Type type) {
            try {
                if (!type.IsSubclassOf(typeof(Entity))) return;

                EntityDefinitionAttribute attr = type.GetCustomAttribute<EntityDefinitionAttribute>();
                if (attr == null) {
                    Logger.Log(LogLevel.Error, $"Entity {type} does not have a definition attribute");
                    return;
                }

                string id = attr.ID;
                ConstructorInfo ctor = type.GetConstructor(new Type[]
                {
                        typeof(EntityData),
                        typeof(Room)
                });

                if (ctor == null) {
                    Logger.Log(LogLevel.Error, $"Entity of type {type} with ID {id} does not have a valid ctor");
                    return;
                }

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
            } catch (Exception e) {
                Logger.Log(LogLevel.Error, $"Encountered an error while attempting to register entity {type}");
                Logger.LogException(e);
            }
        }

        /// <returns>A list of all registered entity IDs.</returns>
        public static List<string> GetRegisteredEntities() {
            return new List<string>(EntityCreators.Keys);
        }

        /// <returns>A list of all registered entity placements.</returns>
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
