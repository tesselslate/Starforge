using Microsoft.Xna.Framework;
using Starforge.Mod;
using Starforge.Mod.API;
using System.Collections.Generic;

namespace Starforge.Map {
    /// <summary>
    /// Represents an individual room within a level.
    /// </summary>
    public class Room : IPackable {
        public RoomMeta Meta;
        public string Name { get => Meta.Name; set => Meta.Name = value; }
        public int X { get => Meta.Bounds.X; set => Meta.Bounds.X = value; }
        public int Y { get => Meta.Bounds.Y; set => Meta.Bounds.Y = value; }
        public int Width { get => Meta.Bounds.Width; set => Meta.Bounds.Width = value; }
        public int Height { get => Meta.Bounds.Height; set => Meta.Bounds.Height = value; }

        public Level Parent;

        public List<Decal> BackgroundDecals;
        public List<Decal> ForegroundDecals;
        public List<Entity> Entities;
        public List<Entity> Triggers;

        public TileGrid BackgroundTiles;
        public TileGrid ForegroundTiles;
        public TileGrid ObjectTiles;

        public Room() {
            BackgroundDecals = new List<Decal>();
            ForegroundDecals = new List<Decal>();
            Entities = new List<Entity>();
            Triggers = new List<Entity>();
        }

        /// <summary>
        /// Creates a Room instance from the given MapElement and parent Level.
        /// </summary>
        /// <param name="el">The MapElement to decode.</param>
        /// <param name="level">The level this room belongs to.</param>
        /// <returns>The parsed Room.</returns>
        public static Room Decode(MapElement el, Level level) {
            Room r = new Room();
            r.Meta = new RoomMeta(el);
            r.Parent = level;

            // Normalize room size to be an increment of a whole tile.
            if (r.Width % 8 != 0) r.Width += 8 - r.Width % 8;
            if (r.Height % 8 != 0) r.Height += 8 - r.Height % 8;

            foreach (MapElement child in el.Children) {
                switch (child.Name) {
                case "bgdecals":
                    foreach (MapElement decal in child.Children) {
                        r.BackgroundDecals.Add(Decal.Decode(decal));
                    }

                    break;
                case "fgdecals":
                    foreach (MapElement decal in child.Children) {
                        r.ForegroundDecals.Add(Decal.Decode(decal));
                    }

                    break;
                case "entities":
                    foreach (MapElement entity in child.Children) {
                        r.Entities.Add(Registry.CreateEntity(entity, r));
                    }

                    break;
                case "triggers":
                    foreach (MapElement trigger in child.Children) {
                        r.Triggers.Add(new UnknownEntity(new EntityData(trigger), r));
                    }

                    break;
                case "objtiles":
                    r.ObjectTiles = new TileGrid(child.GetString("innerText"), ',', r.Width / 8, r.Height / 8, TileGrid.OBJ_AIR);
                    break;
                case "solids":
                    r.ForegroundTiles = new TileGrid(child.GetString("innerText"), r.Width / 8, r.Height / 8, TileGrid.TILE_AIR);
                    break;
                case "bg":
                    r.BackgroundTiles = new TileGrid(child.GetString("innerText"), r.Width / 8, r.Height / 8, TileGrid.TILE_AIR);
                    break;
                }
            }

            // It should be noted that there are two additional child elements - bgtiles and fgtiles.
            // These appear to follow the same format as the objtiles element and likely have a similar function.
            // However, they aren't parsed here simply because they are so rarely needed and object tiles work fine.

            return r;
        }

        public MapElement Encode() {
            // Add room metadata
            MapElement el = new MapElement() { Name = "level" };
            Meta.Encode(el);
            
            // Add decals
            el.AddList(BackgroundDecals, "bgdecals");
            el.AddList(ForegroundDecals, "fgdecals");

            // Add entities and triggers
            el.AddList(Entities, "entities");
            el.AddList(Triggers, "triggers");

            // Add tile grids
            MapElement objtiles = new MapElement() { Name = "objtiles" };
            objtiles.SetAttribute("innerText", ObjectTiles.ToCSV());
            objtiles.SetAttribute("tileset", "scenery");

            MapElement fgtiles = new MapElement() { Name = "solids" };
            fgtiles.SetAttribute("innerText", ForegroundTiles.ToString());

            MapElement bgtiles = new MapElement() { Name = "bg" };
            bgtiles.SetAttribute("innerText", BackgroundTiles.ToString());

            el.Children.Add(objtiles);
            el.Children.Add(fgtiles);
            el.Children.Add(bgtiles);

            return el;
        }
    }

    /// <summary>
    /// Contains metadata about a room.
    /// </summary>
    public struct RoomMeta {
        public string Name;
        public Rectangle Bounds;

        public string Ambience;
        public string AltMusic;
        public string Music;

        public bool DelayAltMusicFade;
        public int AmbienceProgress;
        public int MusicProgress;
        public bool MusicLayer1;
        public bool MusicLayer2;
        public bool MusicLayer3;
        public bool MusicLayer4;
        public bool Whisper;

        public Vector2 CameraOffset;
        public int Color;
        public bool Dark;
        public bool DisableDownTransition;
        public bool Space;
        public bool Underwater;
        public string WindPattern;

        public RoomMeta(MapElement el) {
            Name = el.GetString("name");
            Bounds = new Rectangle(
                el.GetInt("x"),
                el.GetInt("y"),
                el.GetInt("width"),
                el.GetInt("height")
            );

            Ambience = el.GetString("ambience", "");
            AltMusic = el.GetString("alt_music", "");
            Music = el.GetString("music", "");

            DelayAltMusicFade = el.GetBool("delayAltMusicFade", false);
            AmbienceProgress = string.IsNullOrEmpty(el.GetString("ambienceProgress")) ? -1 : el.GetInt("ambienceProgress");
            MusicProgress = string.IsNullOrEmpty(el.GetString("musicProgress")) ? -1 : el.GetInt("musicProgress");
            MusicLayer1 = el.GetBool("musicLayer1", true);
            MusicLayer2 = el.GetBool("musicLayer2", true);
            MusicLayer3 = el.GetBool("musicLayer3", true);
            MusicLayer4 = el.GetBool("musicLayer4", true);
            Whisper = el.GetBool("whisper", false);

            Color = el.GetInt("c", 0);
            CameraOffset = new Vector2(el.GetFloat("cameraOffsetX", 0f), el.GetFloat("cameraOffsetY", 0f));
            Dark = el.GetBool("dark", false);
            DisableDownTransition = el.GetBool("disableDownTransition", false);
            Space = el.GetBool("space", false);
            Underwater = el.GetBool("underwater", false);
            WindPattern = el.GetString("windPattern", "None");
        }

        public void Encode(MapElement el) {
            el.SetAttribute("name", Name);
            el.SetAttribute("x", Bounds.X);
            el.SetAttribute("y", Bounds.Y);
            el.SetAttribute("width", Bounds.Width);
            el.SetAttribute("height", Bounds.Height);
            el.SetAttribute("ambience", Ambience);
            el.SetAttribute("alt_music", AltMusic);
            el.SetAttribute("music", Music);
            el.SetAttribute("delayAltMusicFade", DelayAltMusicFade);
            el.SetAttribute("ambienceProgress", AmbienceProgress);
            el.SetAttribute("musicProgress", MusicProgress);
            el.SetAttribute("musicLayer1", MusicLayer1);
            el.SetAttribute("musicLayer2", MusicLayer2);
            el.SetAttribute("musicLayer3", MusicLayer3);
            el.SetAttribute("musicLayer4", MusicLayer4);
            el.SetAttribute("whisper", Whisper);
            el.SetAttribute("c", Color);
            el.SetAttribute("cameraOffsetX", CameraOffset.X);
            el.SetAttribute("cameraOffsetY", CameraOffset.Y);
            el.SetAttribute("dark", Dark);
            el.SetAttribute("disableDownTransition", DisableDownTransition);
            el.SetAttribute("space", Space);
            el.SetAttribute("underwater", Underwater);
            el.SetAttribute("windPattern", WindPattern);
        }
    }
}
