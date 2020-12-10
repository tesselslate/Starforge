using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Starforge.Core;
using Starforge.MapStructure.Encoding;
using Starforge.Mod;
using Starforge.Mod.Assets;
using Starforge.Util;
using System;
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

        public Vector2 Position {
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

        public LevelMeta Meta;
        public Map Parent;

        private StaticTexture[] FgGrid;
        private StaticTexture[] BgGrid;
        private bool TilesDirty = true;

        public RenderTarget2D Target { get; private set; }
        public bool Dirty = true;

        public bool Selected = false;

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
                        level.Entities.Add(EntityRegistry.Create(level, entity));
                    }
                } else if(child.Name == "triggers") {
                    foreach(BinaryMapElement trigger in child.Children) {
                        level.Triggers.Add(EntityRegistry.CreateTrigger(level, trigger));
                    }
                } else if(child.Name == "bgdecals") {
                    foreach(BinaryMapElement decal in child.Children) {
                        level.BackgroundDecals.Add(new Decal(
                            decal.GetFloat("x"),
                            decal.GetFloat("y"),
                            decal.GetFloat("scaleX", 1),
                            decal.GetFloat("scaleY", 1),
                            decal.GetString("texture")
                        ));
                    }
                } else if(child.Name == "fgdecals") {
                    foreach(BinaryMapElement decal in child.Children) {
                        level.ForegroundDecals.Add(new Decal(
                            decal.GetFloat("x"),
                            decal.GetFloat("y"),
                            decal.GetFloat("scaleX", 1),
                            decal.GetFloat("scaleY", 1),
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

            level.UpdateBounds();

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

        public void Update(KeyboardState kbd, MouseState m) {
            Point roomPos = new Point(m.X - X, m.Y - Y);
            Point tile = new Point((int)Math.Floor(roomPos.X / 8f), (int)Math.Floor(roomPos.Y / 8f));
        }

        public void UpdateBounds() {
            Bounds = new Rectangle(X, Y, Width, Height);
            Position = new Vector2(X, Y);

            Target = new RenderTarget2D(
                Engine.Instance.GraphicsDevice,
                Width, Height, false,
                SurfaceFormat.Color, DepthFormat.None,
                0, RenderTargetUsage.PreserveContents);
        }

        private void RegenerateTileGrids() {
            Parent.ResetRNG();

            BgGrid = Engine.Scene.BGAutotiler.GenerateTextureMap(BackgroundTiles, X, Y);
            FgGrid = Engine.Scene.FGAutotiler.GenerateTextureMap(ForegroundTiles, X, Y);
            TilesDirty = false;
        }

        public void Render() {
            Engine.Instance.GraphicsDevice.SetRenderTarget(Target);
            if(Selected) Engine.Instance.GraphicsDevice.Clear(Engine.Config.SelectedRoomColor);
            else Engine.Instance.GraphicsDevice.Clear(Engine.Config.RoomColor);

            Engine.Batch.Begin(SpriteSortMode.Deferred,
                               BlendState.AlphaBlend,
                               SamplerState.PointClamp, null, RasterizerState.CullNone, null);

            if(TilesDirty) RegenerateTileGrids();

            // Background tiles
            for(int pos = 0; pos < BgGrid.Length; pos++) {
                BgGrid[pos].Draw();
            }

            // Background decals
            for(int pos = 0; pos < BackgroundDecals.Count; pos++) {
                BackgroundDecals[pos].Texture.DrawCentered();
            }

            // Entities
            for(int pos = 0; pos < Entities.Count; pos++) {
                Entities[pos].Render();
            }

            // Foreground tiles
            for(int pos = 0; pos < FgGrid.Length; pos++) {
                FgGrid[pos].Draw();
            }

            // Foreground decals
            for(int pos = 0; pos < ForegroundDecals.Count; pos++) {
                ForegroundDecals[pos].Texture.DrawCentered();
            }

            Engine.Batch.End();
            Dirty = false;
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