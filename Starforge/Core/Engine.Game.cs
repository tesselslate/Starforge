using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Starforge.Editor;
using Starforge.MapStructure;
using Starforge.MapStructure.Encoding;
using Starforge.Mod.Assets;
using System;
using System.Collections.Generic;
using System.IO;

namespace Starforge.Core {
    public partial class Engine : Game {
        public static SpriteBatch Batch { 
            get; 
            private set; 
        }

        public static SpriteBatch GUIBatch {
            get;
            private set;
        }

        public static Scene Scene {
            get;
            private set;
        }

        public List<VirtualTexture> VirtualContent {
            get;
            private set;
        }

        private Engine() {
            GraphicsDeviceManager gdm = new GraphicsDeviceManager(this);

            // Default graphics settings
            gdm.IsFullScreen = false;
            gdm.PreferredBackBufferWidth = 1280;
            gdm.PreferredBackBufferHeight = 720;
            gdm.PreferMultiSampling = true;
            gdm.SynchronizeWithVerticalRetrace = true;

            IsMouseVisible = true;
        }

        protected override void Initialize() {
            base.Initialize();

            Scene = new Scene();
            using(FileStream stream = File.OpenRead("./Content/Maps/7-Summit.bin")) {
                using(BinaryReader reader = new BinaryReader(stream)) {
                    Scene.LoadMap(Map.FromBinary(MapPacker.ReadMapBinary(reader)));
                }
            }
        }

        protected override void LoadContent() {
            base.LoadContent();

            Batch = new SpriteBatch(GraphicsDevice);
            GUIBatch = new SpriteBatch(GraphicsDevice);
            VirtualContent = new List<VirtualTexture>();

            GFX.Load();
        }

        protected override void UnloadContent() {
            base.UnloadContent();
        }

        protected override void Draw(GameTime gameTime) {
            base.Draw(gameTime);

            GraphicsDevice.Clear(Config.BackgroundColor);
            Scene.Render();
        }

        protected override void Update(GameTime gameTime) {
            base.Update(gameTime);
            Scene.Update();
        }

        protected override void OnExiting(object sender, EventArgs args) {
            base.OnExiting(sender, args);

            // Perform cleanup actions.
            Exit(0, false);
        }
    }
}
