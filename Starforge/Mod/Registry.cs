﻿using Microsoft.Xna.Framework;
using Starforge.Core;
using Starforge.Map;
using Starforge.Mod.API;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Starforge.Mod {
    public static class Registry {
        private static Dictionary<string, EntityCreator> EntityCreators;
        private static Dictionary<string, EntityCreator> TriggerCreators;

        static Registry() {
            EntityCreators = new Dictionary<string, EntityCreator>();
            TriggerCreators = new Dictionary<string, EntityCreator>();
        }

        public static Entity CreateEntity(MapElement el, Room room) {
            EntityData data = new EntityData(el);

            if (EntityCreators.ContainsKey(el.Name)) {
                return EntityCreators[el.Name](data, room);
            } else {
                return new UnknownEntity(data, room);
            }
        }

        public static Entity CreateEntity(MapElement el, Room room, Rectangle size) {
            EntityData data = new EntityData(el);
            data.Attributes["x"] = size.X;
            data.Attributes["y"] = size.Y;
            data.Attributes["width"] = size.Width;
            data.Attributes["height"] = size.Height;

            if (EntityCreators.ContainsKey(el.Name)) {
                return EntityCreators[el.Name](data, room);
            } else {
                return new UnknownEntity(data, room);
            }
        }

        public static Entity CreateTrigger(MapElement el, Room room, Rectangle size) {
            EntityData data = new EntityData(el);
            data.Attributes["x"] = size.X;
            data.Attributes["y"] = size.Y;
            data.Attributes["width"] = size.Width;
            data.Attributes["height"] = size.Height;

            if (TriggerCreators.ContainsKey(el.Name)) {
                return TriggerCreators[el.Name](data, room);
            } else {
                return new UnknownEntity(data, room);
            }
        }

        public static void Register(Type type) {
            try {
                if (!type.IsSubclassOf(typeof(Entity))) return;

                EntityDefinitionAttribute attr = type.GetCustomAttribute<EntityDefinitionAttribute>();

                if (attr != null) {
                    TryRegister(EntityCreators, type, attr);
                } else {
                    TriggerDefinitionAttribute tattr = type.GetCustomAttribute<TriggerDefinitionAttribute>();
                    if (tattr != null) {
                        TryRegister(TriggerCreators, type, tattr);
                    }

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

        /// <returns>A list of all registered trigger IDs.</returns>
        public static List<string> GetRegisteredTriggers() {
            return new List<string>(TriggerCreators.Keys);
        }

        private static void TryRegister(Dictionary<string, EntityCreator> dict, Type type, Attribute attr) {
            string id = ((EntityDefinitionAttribute)attr).ID;
            ConstructorInfo ctor = type.GetConstructor(new Type[]
            {
                typeof(EntityData),
                typeof(Room)
            });

            if (ctor != null) {
                dict.Add(id, (EntityData d, Room r) => (Entity)ctor.Invoke(new object[] { d, r }));
                Logger.Log($"Registered entity {id} of type {type}");
            } else {
                Logger.Log(LogLevel.Error, $"Entity of type {type} with ID {id} does not have a valid ctor");
            }
        }
    }

    public delegate Entity EntityCreator(EntityData data, Room room);
}
