using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Starforge.Map {
    /// <summary>
    /// Represents a map and everything it contains.
    /// </summary>
    public class Level : IPackable {
        public string Package;
        public LevelMeta Meta;

        public List<Rectangle> Fillers;
        public List<Room> Rooms;
        public List<Style> BackgroundStyles;
        public List<Style> ForegroundStyles;

        public Level(string package) {
            Package = package;
            Meta = new LevelMeta(new MapElement());

            Fillers = new List<Rectangle>();
            Rooms = new List<Room>();

            BackgroundStyles = new List<Style>();
            ForegroundStyles = new List<Style>();
        }

        /// <summary>
        /// Creates a Level instance from the given MapElement.
        /// </summary>
        /// <param name="el">The MapElement to create a Level from.</param>
        /// <returns>The parsed level.</returns>
        public static Level Decode(MapElement el) {
            Level l = new Level(el.Package);
            foreach (MapElement child in el.Children) {
                switch (child.Name) {
                case "meta":
                    l.Meta = new LevelMeta(child);

                    break;
                case "levels":
                    foreach (MapElement room in child.Children) {
                        l.Rooms.Add(Room.Decode(room, l));
                    }

                    break;
                case "Filler":
                    foreach (MapElement filler in child.Children) {
                        l.Fillers.Add(new Rectangle(
                            filler.GetInt("x") * 8,
                            filler.GetInt("y") * 8,
                            filler.GetInt("w") * 8,
                            filler.GetInt("h") * 8
                        ));
                    }

                    break;
                case "Style":
                    foreach (MapElement root in child.Children) {
                        if (root.Name == "Backgrounds") l.BackgroundStyles = Style.ParseListElement(root);
                        if (root.Name == "Foregrounds") l.ForegroundStyles = Style.ParseListElement(root);
                    }

                    break;
                }
            }

            return l;
        }

        public MapElement Encode() {
            MapElement el = new MapElement()
            {
                Name = "Map",
                Package = Package
            };

            // Add metadata
            el.Children.Add(Meta);

            // Add rooms
            el.AddList(Rooms, "levels");

            // Add fillers
            MapElement fillers = new MapElement() { Name = "Filler" };
            foreach (Rectangle filler in Fillers) {
                MapElement fillerEl = new MapElement() { Name = "rect" };
                fillerEl.SetAttribute("x", filler.X / 8);
                fillerEl.SetAttribute("y", filler.Y / 8);
                fillerEl.SetAttribute("w", filler.Width / 8);
                fillerEl.SetAttribute("h", filler.Height / 8);
                fillers.Children.Add(fillerEl);
            }
            el.Children.Add(fillers);

            // Add styles
            MapElement stylesEl = new MapElement() { Name = "Style" };
            MapElement bg = new MapElement() { Name = "Backgrounds" };
            MapElement fg = new MapElement() { Name = "Foregrounds" };
            foreach (Style s in BackgroundStyles) bg.Children.Add(s);
            foreach (Style s in ForegroundStyles) fg.Children.Add(s);
            stylesEl.Children.Add(bg);
            stylesEl.Children.Add(fg);

            el.Children.Add(stylesEl);

            return el;
        }
    }

    public class LevelMeta : MapElement {
        private MapElement cmod;
        private MapElement mode;
        private MapElement audio;

        public string AnimatedTiles    { get => GetString("AnimatedTiles");    set => SetAttribute("AnimatedTiles", value);     }
        public string BackgroundTiles  { get => GetString("BackgroundTiles");  set => SetAttribute("BackgroundTiles", value);   }
        public float BloomBase         { get => GetFloat("BloomBase");         set => SetAttribute("BloomBase", value);         }
        public float BloomStrength     { get => GetFloat("BloomStrength");     set => SetAttribute("BloomStrength", value);     }
        public string CassetteSong     { get => GetString("CassetteSong");     set => SetAttribute("CassetteSong", value);      }
        public string ColorGrade       { get => GetString("ColorGrade");       set => SetAttribute("ColorGrade", value);        }
        public string CoreMode         { get => GetString("CoreMode");         set => SetAttribute("CoreMode", value);          }
        public float DarknessAlpha     { get => GetFloat("DarknessAlpha");     set => SetAttribute("DarknessAlpha", value);     }
        public bool Dreaming           { get => GetBool("Dreaming");           set => SetAttribute("Dreaming", value);          }
        public string ForegroundTiles  { get => GetString("ForegroundTiles");  set => SetAttribute("ForegroundTiles", value);   }
        public string Icon             { get => GetString("Icon");             set => SetAttribute("Icon", value);              }
        public bool Interlude          { get => GetBool("Interlude");          set => SetAttribute("Interlude", value);         }
        public string IntroType        { get => GetString("IntroType");        set => SetAttribute("IntroType", value);         }
        public bool OverrideASideMeta  { get => GetBool("OverrideASideMeta");  set => SetAttribute("OverrideASideMeta", value); }
        public string Parent           { get => GetString("Parent");           set => SetAttribute("Parent", value);            }
        public string Portraits        { get => GetString("Portraits");        set => SetAttribute("Portraits", value);         }
        public string PostcardSoundID  { get => GetString("PostcardSoundID");  set => SetAttribute("PostcardSoundID", value);   }
        public string Sprites          { get => GetString("Sprites");          set => SetAttribute("Sprites", value);           }
        public string TitleAccentColor { get => GetString("TitleAccentColor"); set => SetAttribute("TitleAccentColor", value);  }
        public string TitleBaseColor   { get => GetString("TitleBaseColor");   set => SetAttribute("TitleBaseColor", value);    }
        public string TitleTextColor   { get => GetString("TitleTextColor");   set => SetAttribute("TitleTextColor", value);    }

        public int BeatsMax            { get => cmod.GetInt("BeatsMax");       set => cmod.SetAttribute("BeatsMax", value);     }
        public int BeatsPerTick        { get => cmod.GetInt("BeatsPerTick");   set => cmod.SetAttribute("BeatsPerTick", value); }
        public int Blocks              { get => cmod.GetInt("Blocks");         set => cmod.SetAttribute("Blocks", value);       }
        public int LeadBeats           { get => cmod.GetInt("LeadBeats");      set => cmod.SetAttribute("LeadBeats", value);    }
        public bool OldBehavior        { get => cmod.GetBool("OldBehavior");   set => cmod.SetAttribute("OldBehavior", value);  }
        public float TempoMult         { get => cmod.GetFloat("TempoMult");    set => cmod.SetAttribute("TempoMult", value);    }
        public int TicksPerSwap        { get => cmod.GetInt("TicksPerSwap");   set => cmod.SetAttribute("TicksPerSwap", value); }

        public bool HeartIsEnd         { get => mode.GetBool("HeartIsEnd");                set => mode.SetAttribute("HeartIsEnd", value);                }
        public bool IgnoreLevelAudio   { get => mode.GetBool("IgnoreLevelAudioLayerData"); set => mode.SetAttribute("IgnoreLevelAudioLayerData", value); }
        public string Inventory        { get => mode.GetString("Inventory");               set => mode.SetAttribute("Inventory", value);                 }
        public bool SeekerSlowdown     { get => mode.GetBool("SeekerSlowdown");            set => mode.SetAttribute("SeekerSlowdown", value);            }
        public string StartLevel       { get => mode.GetString("StartLevel");              set => mode.SetAttribute("StartLevel", value);                }
        public bool TheoInBubble       { get => mode.GetBool("TheoInBubble");              set => mode.SetAttribute("TheoInBubble", value);              }

        public string Ambience         { get => audio.GetString("Ambience");               set => audio.SetAttribute("Ambience", value);                 }
        public string Music            { get => audio.GetString("Music");                  set => audio.SetAttribute("Music", value);                    }

        public LevelMeta(MapElement metaEl) {
            Name = string.Empty;

            MergeAttributes(metaEl);

            foreach (MapElement el in metaEl.Children) {
                if (el.Name == "cassettemodifier") {
                    cmod = new MapElement() { Name = "cassettemodifier" };
                    cmod.MergeAttributes(el);
                    Children.Add(cmod);
                }

                if (el.Name == "mode") {
                    mode = new MapElement() { Name = "mode" };
                    mode.MergeAttributes(el);
                    Children.Add(mode);

                    if (el.Children.Count == 1 && el.Children.Any(child => child.Name == "audiostate")) {
                        audio = new MapElement() { Name = "audiostate" };
                        audio.MergeAttributes(el.Children.First());
                        mode.Children.Add(audio);
                    }
                }
            }

            // Add any elements which weren't found
            if (cmod == null) {
                cmod = new MapElement() { Name = "cassettemodifier" };
                Children.Add(cmod);
            }

            if (mode == null) {
                mode = new MapElement() { Name = "mode" };
                Children.Add(mode);

                audio = new MapElement() { Name = "audiostate" };
                mode.Children.Add(audio);
            } else if (audio == null) {
                audio = new MapElement() { Name = "audiostate" };
                mode.Children.Add(audio);
            }
        }
    }
}
