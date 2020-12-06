using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Starforge.Core;
using Starforge.MapStructure.Encoding;
using Starforge.Mod.Assets;
using Starforge.Util;
using System.Collections.Generic;

namespace Starforge.MapStructure {
    public class Level : MapElement {
        public int X {
            get => GetInt("x");
            set => SetAttribute("x", value);
        }

        public int Y {
            get => GetInt("y");
            set => SetAttribute("y", value);
        }

        public int Width {
            get => GetInt("width");
            set => SetAttribute("width", value);
        }

        public int Height {
            get => GetInt("height");
            set => SetAttribute("height", value);
        }

        public string Name {
            get => GetString("name");
            set => SetAttribute("name", value);
        }

        public Rectangle Bounds {
            get;
            private set;
        }

        public List<Entity> Entities;
        public List<Trigger> Triggers;
        public List<Decal> BackgroundDecals;
        public List<Decal> ForegroundDecals;
        public TileGrid BackgroundTiles;
        public TileGrid ForegroundTiles;
        public TileGrid ObjectTiles;

        private DrawableTexture[,] FgGrid;
        private DrawableTexture[,] BgGrid;
        private bool TilesDirty = true;

        public LevelMeta Meta;
        public Map Parent;

        public Level() {
            // Create empty lists for usual level elements (entities, etc)
            Entities = new List<Entity>();
            Triggers = new List<Trigger>();
            BackgroundDecals = new List<Decal>();
            ForegroundDecals = new List<Decal>();
        }

        private void CreateTileGrids() {
            // This is just used to ensure the TileGrids aren't null when resaving (some rooms don't have these.)
            BackgroundTiles = new TileGrid(Width / 8, Height / 8);
            ForegroundTiles = new TileGrid(Width / 8, Height / 8);
            ObjectTiles = new TileGrid(Width / 8, Height / 8);
        }

        public static Level FromBinary(BinaryMapElement element, Map parent) {
            Level level = new Level();
            level.Attributes = element.Attributes;
            level.CreateTileGrids();

            level.Meta = new LevelMeta(level);
            level.Parent = parent;

            // Normalize room size
            if(level.Width % 8 != 0) {
                level.Width += 8 - (level.Width % 8);
            }

            if(level.Height % 8 != 0) {
                level.Height += 8 - (level.Height % 8);
            }

            foreach(BinaryMapElement child in element.Children) {
                if(child.Name == "entities") {
                    foreach(BinaryMapElement entity in child.Children) {
                        level.Entities.Add(new Entity(level, entity));
                    }
                } else if(child.Name == "triggers") {
                    foreach(BinaryMapElement trigger in child.Children) {
                        level.Triggers.Add(new Trigger(level, trigger));
                    }
                } else if(child.Name == "bgdecals") {
                    foreach(BinaryMapElement decal in child.Children) {
                        level.BackgroundDecals.Add(new Decal(
                            decal.GetFloat("x"),
                            decal.GetFloat("y"),
                            decal.GetFloat("scaleX"),
                            decal.GetFloat("scaleY"),
                            decal.GetString("texture")
                        ));
                    }
                } else if(child.Name == "fgdecals") {
                    foreach(BinaryMapElement decal in child.Children) {
                        level.ForegroundDecals.Add(new Decal(
                            decal.GetFloat("x"),
                            decal.GetFloat("y"),
                            decal.GetFloat("scaleX"),
                            decal.GetFloat("scaleY"),
                            decal.GetString("texture")
                        ));
                    }
                } else if(child.Name == "objtiles") {
                    level.ObjectTiles = new TileGrid(
                        MiscHelper.ReadCSV(
                            child.GetString("innerText"),
                            level.Width / 8,
                            level.Height / 8
                        ),
                        level.Width / 8,
                        level.Height / 8
                    );
                } else if(child.Name == "solids") {
                    level.ForegroundTiles = new TileGrid(
                        child.GetString("innerText"),
                        level.Width / 8,
                        level.Height / 8
                    );
                } else if(child.Name == "bg") {
                    level.BackgroundTiles = new TileGrid(
                        child.GetString("innerText"),
                        level.Width / 8,
                        level.Height / 8
                     );
                }
            }

            level.Update();

            return level;
        }

        public override BinaryMapElement ToBinary() {
            BinaryMapElement bin = new BinaryMapElement()
            {
                Name = "level",
                Attributes = Attributes
            };

            // Add entities
            if(Entities.Count > 0) {
                BinaryMapElement entities = new BinaryMapElement()
                {
                    Name = "entities"
                };
                foreach(Entity entity in Entities) {
                    entities.Children.Add(entity.ToBinary());
                }
                bin.Children.Add(entities);
            }

            // Add triggers
            if(Triggers.Count > 0) {
                BinaryMapElement triggers = new BinaryMapElement()
                {
                    Name = "triggers"
                };
                foreach(Trigger trigger in Triggers) {
                    triggers.Children.Add(trigger.ToBinary());
                }
                bin.Children.Add(triggers);
            }

            // Add background decals
            if(BackgroundDecals.Count > 0) {
                BinaryMapElement bgDecals = new BinaryMapElement()
                {
                    Name = "bgdecals"
                };
                foreach(Decal decal in BackgroundDecals) {
                    bgDecals.Children.Add(decal.ToBinary());
                }
                bin.Children.Add(bgDecals);
            }

            // Add foreground decals
            if(ForegroundDecals.Count > 0) {
                BinaryMapElement fgDecals = new BinaryMapElement()
                {
                    Name = "fgdecals"
                };
                foreach(Decal decal in ForegroundDecals) {
                    fgDecals.Children.Add(decal.ToBinary());
                }
                bin.Children.Add(fgDecals);
            }

            // Add background tiles
            BinaryMapElement bgTiles = new BinaryMapElement()
            {
                Name = "bg"
            };
            bgTiles.SetAttribute("innerText", BackgroundTiles.ToString());
            bin.Children.Add(bgTiles);

            // Add foreground tiles
            BinaryMapElement fgTiles = new BinaryMapElement()
            {
                Name = "solids"
            };
            fgTiles.SetAttribute("innerText", ForegroundTiles.ToString());
            bin.Children.Add(fgTiles);

            // Add object tiles
            BinaryMapElement objTiles = new BinaryMapElement()
            {
                Name = "objtiles"
            };
            objTiles.SetAttribute("innerText", ObjectTiles.ToCSV());
            bin.Children.Add(objTiles);

            return bin;
        }

        public void Update() {
            Bounds = new Rectangle(X, Y, Width, Height);
        }

        private void RegenerateTileGrids() {
            BgGrid = Engine.Scene.BGAutotiler.GenerateTextureMap(BackgroundTiles.Tiles);
            FgGrid = Engine.Scene.FGAutotiler.GenerateTextureMap(ForegroundTiles.Tiles);
        }

        public void Render() {
            if(TilesDirty) RegenerateTileGrids();

            GFX.Pixel.Draw(Bounds, Engine.Config.RoomColor);

            for(int i = 0; i < FgGrid.GetLength(0); i++) {
                for(int j = 0; j < FgGrid.GetLength(1); j++) {
                    if(BgGrid[i, j] != null)
                        BgGrid[i, j].Draw(new Vector2(X + i * 8, Y + j * 8));
                    if(FgGrid[i, j] != null)
                        FgGrid[i, j].Draw(new Vector2(X + i * 8, Y + j * 8));
                }
            }
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