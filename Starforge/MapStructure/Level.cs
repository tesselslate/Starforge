using Starforge.Core;
using Starforge.MapStructure.Encoding;
using Starforge.Mod;
using System;
using System.Collections.Generic;

namespace Starforge.MapStructure {
    public class Level : MapElement {
        public List<Entity> Entities;
        public List<Trigger> Triggers;

        public Level() {
            // Create empty lists for usual level elements (entities, etc)
            Entities = new List<Entity>();
            Triggers = new List<Trigger>();
        }

        public static Level FromBinary(BinaryMapElement element) {
            Level level = new Level();
            level.Attributes = element.Attributes;

            foreach(BinaryMapElement child in element.Children) {
                if(child.Name == "entities") {
                    foreach(BinaryMapElement entity in child.Children) {
                        level.Entities.Add(EntityRegistry.CreateEntity(entity.Name, level, entity));
                    }
                } else if(child.Name == "triggers") {
                    foreach(BinaryMapElement trigger in child.Children) {
                        level.Triggers.Add(EntityRegistry.CreateEntity(trigger.Name, level, trigger) as Trigger);
                    }
                }
            }

            return level;
        }

        public override BinaryMapElement ToBinary() {
            throw new System.NotImplementedException();
        }
    }

    public class LevelMeta {
        private Level level;

        public string AlternateMusic {
            get => level.GetString("altMusic");
            set => level.SetAttribute("altMusic", value);
        }

        public string Ambience {
            get => level.GetString("ambience");
            set => level.SetAttribute("ambience", value);
        }

        public int AmbienceProgress {
            get => level.GetInt("ambienceProgress");
            set => level.SetAttribute("ambienceProgress", value);
        }

        public int Color {
            get => level.GetInt("c");
            set => level.SetAttribute("c", value);
        }

        public bool Dark {
            get => level.GetBool("dark");
            set => level.SetAttribute("dark", value);
        }

        public bool DelayAlternateMusic {
            get => level.GetBool("delayAltMusicFade");
            set => level.SetAttribute("delayAltMusicFade", value);
        }

        public string Music {
            get => level.GetString("music");
            set => level.SetAttribute("music", value);
        }

        public bool MusicLayer1 {
            get => level.GetBool("musicLayer1");
            set => level.SetAttribute("musicLayer1", value);
        }

        public bool MusicLayer2 {
            get => level.GetBool("musicLayer2");
            set => level.SetAttribute("musicLayer2", value);
        }

        public bool MusicLayer3 {
            get => level.GetBool("musicLayer3");
            set => level.SetAttribute("musicLayer3", value);
        }

        public bool MusicLayer4 {
            get => level.GetBool("musicLayer4");
            set => level.SetAttribute("musicLayer4", value);
        }

        public int MusicProgress {
            get => level.GetInt("musicProgress");
            set => level.SetAttribute("musicProgress", value);
        }

        public string Name {
            get => level.GetString("name");
            set => level.SetAttribute("name", value);
        }

        public bool Space {
            get => level.GetBool("space");
            set => level.SetAttribute("space", value);
        }

        public bool Underwater {
            get => level.GetBool("underwater");
            set => level.SetAttribute("underwater", value);
        }

        public bool Whisper {
            get => level.GetBool("whisper");
            set => level.SetAttribute("whisper", value);
        }

        public string WindPattern {
            get => level.GetString("windPattern");
            set => level.SetAttribute("windPattern", value);
        }

        public LevelMeta(Level parent) {
            level = parent;
        }
    }
}