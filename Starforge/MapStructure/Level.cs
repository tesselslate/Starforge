using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Starforge.Core;
using Starforge.Core.Input;
using Starforge.Editor;
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

        public StaticTexture[] FgGrid;
        public StaticTexture[] BgGrid;
        private bool TilesDirty = true;

        public RenderTarget2D Target { get; private set; }
        public RenderTarget2D Overlay { get; private set; }
        public Point OverlayPosition { get; private set; }
        public bool Dirty { get; set; } = true;

        // whether this level is currently selected
        private bool Selected { get; set; } = false;

        // whether this level was just newly selected
        public bool WasSelected = false;
        private double InputProcessWait;

        // Current position of the mouse in the room, in tiles
        public Point TilePointer {
            get;
            private set;
        }

        public Stack<Editor.Actions.Action> PastActions;
        public Stack<Editor.Actions.Action> FutureActions;

        public Level() {
            // Create empty lists for usual level elements (entities, etc)
            Entities = new List<Entity>();
            Triggers = new List<Trigger>();
            BackgroundDecals = new List<Decal>();
            ForegroundDecals = new List<Decal>();
            PastActions = new Stack<Editor.Actions.Action>();
            FutureActions = new Stack<Editor.Actions.Action>();
        }

        // create a new level from reading the binary
        public Level(BinaryMapElement element, Map parent) : this() {
            Attributes = element.Attributes;
            CreateTileGrids();

            Meta = new LevelMeta(this);
            Parent = parent;

            // Normalize room size
            if (Width % 8 != 0) {
                Width += 8 - (Width % 8);
            }
            if (Height % 8 != 0) {
                Height += 8 - (Height % 8);
            }

            foreach (BinaryMapElement child in element.Children) {
                switch (child.Name) {
                case "entities":
                    foreach (BinaryMapElement entity in child.Children) {
                        Entities.Add(EntityRegistry.Create(this, entity));
                    }
                    break;
                case "triggers":
                    foreach (BinaryMapElement trigger in child.Children) {
                        Triggers.Add(EntityRegistry.CreateTrigger(this, trigger));
                    }
                    break;
                case "bgdecals":
                    foreach (BinaryMapElement decal in child.Children) {
                        BackgroundDecals.Add(new Decal(
                            decal.GetFloat("x"),
                            decal.GetFloat("y"),
                            decal.GetFloat("scaleX", 1),
                            decal.GetFloat("scaleY", 1),
                            decal.GetString("texture")
                        ));
                    }
                    break;
                case "fgdecals":
                    foreach (BinaryMapElement decal in child.Children) {
                        ForegroundDecals.Add(new Decal(
                            decal.GetFloat("x"),
                            decal.GetFloat("y"),
                            decal.GetFloat("scaleX", 1),
                            decal.GetFloat("scaleY", 1),
                            decal.GetString("texture")
                        ));
                    }
                    break;
                case "objtiles":
                    ObjectTiles = new TileGrid(
                        MiscHelper.ReadCSV(
                            child.GetString("innerText"),
                            Width / 8,
                            Height / 8
                        ),
                        Width / 8,
                        Height / 8
                    );
                    break;
                case "solids":
                    ForegroundTiles = new TileGrid(
                        child.GetString("innerText"),
                        Width / 8,
                        Height / 8
                    );
                    break;
                case "bg":
                    BackgroundTiles = new TileGrid(
                        child.GetString("innerText"),
                        Width / 8,
                        Height / 8
                     );
                    break;
                }
            }

            UpdateBounds();
        }

        private void CreateTileGrids() {
            // This is just used to ensure the TileGrids aren't null when resaving (some rooms don't have these.)
            BackgroundTiles = new TileGrid(Width / 8, Height / 8);
            ForegroundTiles = new TileGrid(Width / 8, Height / 8);
            ObjectTiles = new TileGrid(Width / 8, Height / 8);
        }

        public override BinaryMapElement ToBinary() {
            BinaryMapElement bin = new BinaryMapElement() {
                Name = "level",
                Attributes = Attributes
            };

            addListToBinary(ref bin, Entities, "entities");
            addListToBinary(ref bin, Triggers, "triggers");
            addListToBinary(ref bin, BackgroundDecals, "bgdecals");
            addListToBinary(ref bin, ForegroundDecals, "fgdecals");

            // Add background tiles
            BinaryMapElement bgTiles = new BinaryMapElement() {
                Name = "bg"
            };
            bgTiles.SetAttribute("innerText", BackgroundTiles.ToString());
            bin.Children.Add(bgTiles);

            // Add foreground tiles
            BinaryMapElement fgTiles = new BinaryMapElement() {
                Name = "solids"
            };
            fgTiles.SetAttribute("innerText", ForegroundTiles.ToString());
            bin.Children.Add(fgTiles);

            // Add object tiles
            BinaryMapElement objTiles = new BinaryMapElement() {
                Name = "objtiles"
            };
            objTiles.SetAttribute("innerText", ObjectTiles.ToCSV());
            bin.Children.Add(objTiles);

            return bin;
        }

        private static void addListToBinary<T>(ref BinaryMapElement bin, List<T> objects, string name) where T : MapElement {
            if (objects.Count <= 0) return;

            BinaryMapElement binaryMapElement = new BinaryMapElement() {
                Name = name
            };
            foreach (T obj in objects) {
                binaryMapElement.Children.Add(obj.ToBinary());
            }
            bin.Children.Add(binaryMapElement);
        }

        public void Update(KeyboardState kbd, MouseEvent m, GameTime gt) {
            // Start input cooldown if this level just got selected
            if (!WasSelected) {
                WasSelected = true;
                InputProcessWait = 0.2;
                return;
            }
            // if cooldown is still running
            if (InputProcessWait > 0) {
                InputProcessWait -= gt.ElapsedGameTime.TotalSeconds;
                return;
            }

            Vector2 rm = Engine.Scene.Camera.ScreenToReal(new Vector2(m.Position.X, m.Position.Y));
            Point roomPos = new Point(
                (int)rm.X - X,
                (int)rm.Y - Y
            );

            TilePointer = new Point((int)Math.Floor(roomPos.X / 8f), (int)Math.Floor(roomPos.Y / 8f));
            ToolManager.Manage(m);
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
            if (Selected) {
                // if selected -> update overlay
                Engine.Instance.GraphicsDevice.SetRenderTarget(Overlay);
                Engine.Instance.GraphicsDevice.Clear(Color.Transparent);

                Engine.Batch.Begin(SpriteSortMode.Deferred,
                                   BlendState.AlphaBlend,
                                   SamplerState.PointClamp, null, RasterizerState.CullNone, null);

                ToolManager.Render();

                Engine.Batch.End();
            }

            if (!Dirty) {
                return;
            }

            Engine.Instance.GraphicsDevice.SetRenderTarget(Target);
            if (Selected) {
                Engine.Instance.GraphicsDevice.Clear(Engine.Config.SelectedRoomColor);
            }
            else {
                Engine.Instance.GraphicsDevice.Clear(Engine.Config.RoomColor);
            }

            Engine.Batch.Begin(SpriteSortMode.Deferred,
                               BlendState.AlphaBlend,
                               SamplerState.PointClamp, null, RasterizerState.CullNone, null);

            if (TilesDirty) {
                RegenerateTileGrids();
            }

            // Background tiles
            for (int pos = 0; pos < BgGrid.Length; pos++) {
                if (BgGrid[pos].Visible) BgGrid[pos].Draw();
            }

            // Background decals
            for (int pos = 0; pos < BackgroundDecals.Count; pos++) {
                BackgroundDecals[pos].Texture.DrawCentered();
            }

            // Entities
            for (int pos = 0; pos < Entities.Count; pos++) {
                Entities[pos].Render();
            }

            // Foreground tiles
            for (int pos = 0; pos < FgGrid.Length; pos++) {
                if (FgGrid[pos].Visible) FgGrid[pos].Draw();
            }

            // Foreground decals
            for (int pos = 0; pos < ForegroundDecals.Count; pos++) {
                ForegroundDecals[pos].Texture.DrawCentered();
            }

            Engine.Batch.End();
            Dirty = false;
        }

        // Sets the selected state of this Level
        public void SetSelected(bool selected) {
            if (Selected == selected) {
                return;
            }
            Selected = selected;
            Dirty = true;

            if (Selected) {
                // generate overlay for this when this becomes Selected
                Overlay = new RenderTarget2D(
                    Engine.Instance.GraphicsDevice,
                    Width, Height, false,
                    SurfaceFormat.Color, DepthFormat.None,
                    0, RenderTargetUsage.PreserveContents);
            }
            else {
                // delete overlay when this becomes deselected
                Overlay.Dispose();
            }
        }

        public void ApplyNewAction(Editor.Actions.Action action) {
            if (action.Apply()) {
                Dirty = true;
            }
            PastActions.Push(action);
            FutureActions.Clear();
        }

        // Undoes the most recent User action for this level
        public void Undo() {
            if (PastActions.Count == 0) {
                return;
            }

            // Take last of the Past actions, Undo it and add it to the Future Actions
            Editor.Actions.Action action = PastActions.Pop();
            if (action.Undo()) {
                Dirty = true;
            }

            FutureActions.Push(action);
        }

        // Redoes the last undone user action
        public void Redo() {
            if (FutureActions.Count == 0) {
                return;
            }

            Editor.Actions.Action action = FutureActions.Pop();
            if (action.Apply()) {
                Dirty = true;
            }

            PastActions.Push(action);
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