using Starforge.MapStructure.Encoding;
using Starforge.Mod;
using Starforge.Util;
using System.Collections.Generic;

namespace Starforge.MapStructure {
    public class Map : MapElement {
        public string Name;
        public string Package;

        public MapMeta Meta;

        public List<Rectangle> Fillers;
        public List<Level> Levels;
        public List<Style> BackgroundStyles;
        public List<Style> ForegroundStyles;

        public Map(string package) {
            Name = "Map";
            Package = package;

            // Creating a MapMeta instance requires a BinaryMapElement.
            // We don't have one, so we just pass an empty one.
            // MapMeta will deal with it. :)
            Meta = new MapMeta(new BinaryMapElement());

            // Create empty lists for usual map elements (rooms, etc)
            Fillers = new List<Rectangle>();
            Levels = new List<Level>();
            BackgroundStyles = new List<Style>();
            ForegroundStyles = new List<Style>();
        }

        public static Style ParseStyle(BinaryMapElement style, BinaryMapElement parent) {
            if(parent != null && parent.Name == "apply") {
                if(style.Name == "parallax") {
                    // Apply parallax styleground
                    Parallax p = Parallax.FromBinary(style);
                    p.MergeAttributes(parent);

                    return p;
                } else {
                    // Apply effect
                    Effect e = EffectRegistry.CreateEffect(style.Name, style);
                    e.MergeAttributes(parent);

                    return e;
                }
            } else if(style.Name == "parallax") {
                // Parallax styleground
                return Parallax.FromBinary(style);
            } else {
                // Effect
                return EffectRegistry.CreateEffect(style.Name, style);
            }
        }

        public static List<Style> ParseStyles(BinaryMapElement bin) {
            List<Style> styles = new List<Style>();

            foreach(BinaryMapElement style in bin.Children) {
                if(style.Name == "apply") {
                    // Apply stylegrounds are weird, and only present in vanilla maps
                    // (to my knowledge.)
                    //
                    // They have a set of attributes, and then a set of children stylegrounds.
                    // They apply their attributes to each child styleground,
                    // then add the child styleground.
                    //
                    // These are essentially a way to apply attributes to stylegrounds in bulk..
                    // Although, it seems relatively useless. Still exists and needs to be accounted for, though.
                    foreach(BinaryMapElement apply in style.Children) {
                        styles.Add(ParseStyle(apply, style));
                    }
                } else {
                    styles.Add(ParseStyle(style, null));
                }
            }

            return styles;
        }

        public static Map FromBinary(BinaryMapElement bin) {
            Map map = new Map(bin.Package);

            foreach(BinaryMapElement child in bin.Children) {
                if(child.Name == "meta") {
                    map.Meta = new MapMeta(child);
                } else if(child.Name == "levels") {
                    foreach(BinaryMapElement level in child.Children) {
                        map.Levels.Add(Level.FromBinary(level));
                    }
                } else if(child.Name == "Filler") {
                    foreach(BinaryMapElement filler in child.Children) {
                        map.Fillers.Add(new Rectangle(
                            filler.GetInt("x"),
                            filler.GetInt("y"),
                            filler.GetInt("w"),
                            filler.GetInt("h")
                        ));
                    }
                } else if(child.Name == "Style") {
                    foreach(BinaryMapElement styleRoot in child.Children) {
                        if(styleRoot.Name == "Backgrounds") {
                            map.BackgroundStyles = ParseStyles(styleRoot);
                        } else if(styleRoot.Name == "Foregrounds") {
                            map.ForegroundStyles = ParseStyles(styleRoot);
                        }
                    }
                }
            }

            return map;
        }

        public override BinaryMapElement ToBinary() {
            BinaryMapElement bin = new BinaryMapElement()
            {
                Name = "Map",
                Package = Package
            };

            // Add metadata
            bin.Children.Add(Meta.MetaElement);

            // Add levels
            BinaryMapElement levelsElement = new BinaryMapElement()
            {
                Name = "levels"
            };

            foreach(Level level in Levels) {
                levelsElement.Children.Add(level.ToBinary());
            }

            bin.Children.Add(levelsElement);

            // Add fillers
            BinaryMapElement fillersElement = new BinaryMapElement()
            {
                Name = "Filler"
            };

            foreach(Rectangle filler in Fillers) {
                BinaryMapElement rect = new BinaryMapElement();
                rect.SetAttribute("x", filler.X);
                rect.SetAttribute("y", filler.Y);
                rect.SetAttribute("w", filler.Width);
                rect.SetAttribute("h", filler.Height);

                fillersElement.Children.Add(rect);
            }

            bin.Children.Add(fillersElement);

            // Add styles
            BinaryMapElement stylesElement = new BinaryMapElement() { Name = "Style" };
            BinaryMapElement bgElement = new BinaryMapElement() { Name = "Backgrounds" };
            BinaryMapElement fgElement = new BinaryMapElement() { Name = "Foregrounds" };

            foreach(Style style in BackgroundStyles) {
                bgElement.Children.Add(style.ToBinary());
            }

            foreach(Style style in ForegroundStyles) {
                fgElement.Children.Add(style.ToBinary());
            }

            stylesElement.Children.Add(bgElement);
            stylesElement.Children.Add(fgElement);

            bin.Children.Add(stylesElement);

            return bin;
        }
    }

    public class MapMeta {
        private BinaryMapElement meta;
        private BinaryMapElement mode;
        private BinaryMapElement audio;
        private BinaryMapElement cassette;

        public BinaryMapElement MetaElement {
            get => meta;
        }

        // Meta
        public float BloomBase {
            get => meta.GetFloat("BloomBase", 1f);
            set => meta.SetAttribute("BloomBase", value);
        }

        public float BloomStrength {
            get => meta.GetFloat("BloomStrength", 0f);
            set => meta.SetAttribute("BloomStrength", value);
        }

        public string ColorGrade {
            get => meta.GetString("ColorGrade", "");
            set => meta.SetAttribute("ColorGrade", value);
        }

        public string CoreMode {
            get => meta.GetString("CoreMode", "None");
            set => meta.SetAttribute("CoreMode", value);
        }

        public float DarknessAlpha {
            get => meta.GetFloat("DarknessAlpha", 0.05f);
            set => meta.SetAttribute("DarknessAlpha", value);
        }

        public bool Dreaming {
            get => meta.GetBool("Dreaming", false);
            set => meta.SetAttribute("Dreaming", value);
        }

        public string Icon {
            get => meta.GetString("Icon", "");
            set => meta.SetAttribute("Icon", value);
        }

        public bool Interlude {
            get => meta.GetBool("Interlude", false);
            set => meta.SetAttribute("Interlude", value);
        }

        public bool OverrideASideMeta {
            get => meta.GetBool("OverrideASideMeta", false);
            set => meta.SetAttribute("OverrideASideMeta", value);
        }

        public string PostcardSoundID {
            get => meta.GetString("PostcardSoundID");
            set => meta.SetAttribute("PostcardSoundID", value);
        }

        public string IntroType {
            get => meta.GetString("IntroType", "Jump");
            set => meta.SetAttribute("IntroType", value);
        }

        public string TitleAccentColor {
            get => meta.GetString("TitleAccentColor", "");
            set => meta.SetAttribute("TitleAccentColor", value);
        }

        public string TitleBaseColor {
            get => meta.GetString("TitleBaseColor", "");
            set => meta.SetAttribute("TitleAccentColor", value);
        }

        public string TitleTextColor {
            get => meta.GetString("TitleTextColor", "");
            set => meta.SetAttribute("TitleTextColor", value);
        }

        public string Wipe {
            get => meta.GetString("Wipe");
            set => meta.SetAttribute("Wipe", value);
        }

        // Audio
        public string Ambience {
            get => audio.GetString("Ambience");
            set => audio.SetAttribute("Ambience", value);
        }

        public string Music {
            get => audio.GetString("Music");
            set => audio.SetAttribute("Music", value);
        }

        // Cassette Modifier
        public string CassetteSong {
            get => meta.GetString("CassetteSong");
            set => meta.SetAttribute("CassetteSong", value);
        }

        public int C_BeatsMax {
            get => cassette.GetInt("BeatsMax", 256);
            set => cassette.SetAttribute("BeatsMax", value);
        }

        public int C_BeatsPerTick {
            get => cassette.GetInt("BeatsPerTick", 4);
            set => cassette.SetAttribute("BeatsPerTick", value);
        }

        public int C_Blocks {
            get => cassette.GetInt("Blocks");
            set => cassette.SetAttribute("Blocks", value);
        }

        public int C_LeadBeats {
            get => cassette.GetInt("LeadBeats");
            set => cassette.SetAttribute("LeadBeats", value);
        }

        public bool C_OldBehavior {
            get => cassette.GetBool("OldBehavior");
            set => cassette.SetAttribute("OldBehavior", value);
        }

        public float C_TempoMult {
            get => cassette.GetFloat("TempoMult");
            set => cassette.SetAttribute("TempoMult", value);
        }

        public int C_TicksPerSwap {
            get => cassette.GetInt("TicksPerSwap", 2);
            set => cassette.SetAttribute("TicksPerSwap", value);
        }

        // Mode
        public bool AllowTheoBubble {
            get => mode.GetBool("TheoInBubble");
            set => mode.SetAttribute("TheoInBubble", value);
        }

        public bool HeartEnd {
            get => mode.GetBool("HeartIsEnd");
            set => mode.SetAttribute("HeartIsEnd", value);
        }

        public bool IgnoreLevelAudioLayerData {
            get => mode.GetBool("IgnoreLevelAudioLayerData");
            set => mode.SetAttribute("IgnoreLevelAudioLayerData", value);
        }

        public string Inventory {
            get => mode.GetString("Inventory");
            set => mode.SetAttribute("Inventory", value);
        }

        public bool SeekerSlowdown {
            get => mode.GetBool("SeekerSlowdown");
            set => mode.SetAttribute("SeekerSlowdown", value);
        }

        public string StartingLevel {
            get => mode.GetString("StartLevel");
            set => mode.SetAttribute("StartLevel", value);
        }

        // XMLs
        public string XmlAnimatedTiles {
            get => meta.GetString("AnimatedTiles");
            set => meta.SetAttribute("AnimatedTiles", value);
        }

        public string XmlBackgroundTiles {
            get => meta.GetString("BackgroundTiles");
            set => meta.SetAttribute("BackgroundTiles", value);
        }

        public string XmlForegroundTiles {
            get => meta.GetString("ForegroundTiles");
            set => meta.SetAttribute("ForegroundTiles", value);
        }

        public string XmlPortraits {
            get => meta.GetString("Portraits");
            set => meta.SetAttribute("Portraits", value);
        }

        public string XmlSprites {
            get => meta.GetString("Sprites");
            set => meta.SetAttribute("Sprites", value);
        }

        public MapMeta(BinaryMapElement metaElement) {
            meta = metaElement;
            meta.Name = "meta";

            bool hasMode = false;
            bool hasAudio = false;
            bool hasCassette = false;

            foreach(BinaryMapElement child in meta.Children) {
                if(child.Name == "cassettemodifier")
                    hasCassette = true;

                if(child.Name == "mode") {
                    hasMode = true;
                    if(child.Children.Find(el => el.Name == "audiostate") != null) {
                        hasAudio = true;
                    }
                }
            }

            BinaryMapElement modeElement;
            BinaryMapElement audioElement;
            BinaryMapElement cassetteElement;

            if(!hasCassette) {
                meta.Children.Add(cassetteElement = new BinaryMapElement()
                {
                    Name = "cassettemodifier"
                });
            } else {
                cassetteElement = meta.Children.Find(el => el.Name == "cassettemodifier");
            }

            if(!hasMode) {
                meta.Children.Add(modeElement = new BinaryMapElement()
                {
                    Name = "mode"
                });
            } else {
                modeElement = meta.Children.Find(el => el.Name == "mode");
            }

            if(!hasAudio) {
                modeElement.Children.Add(audioElement = new BinaryMapElement()
                {
                    Name = "audiostate"
                });
            } else {
                audioElement = modeElement.Children.Find(el => el.Name == "audiostate");
            }

            mode = modeElement;
            audio = audioElement;
            cassette = cassetteElement;
        }
    }
}
