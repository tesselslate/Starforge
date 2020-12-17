using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Starforge.Core.Input;
using Starforge.Editor;
using Starforge.Editor.UI;
using Starforge.MapStructure;
using Starforge.MapStructure.Encoding;
using Starforge.Mod.Assets;
using System;
using System.Collections.Generic;
using System.IO;

namespace Starforge.Core {
    public partial class Engine : Game {
        public static ImGUIHandler GUI;

        public static SpriteBatch Batch {
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

        public GraphicsDeviceManager GDM {
            get;
            private set;
        }

        private Engine() {
            GDM = new GraphicsDeviceManager(this);

            // Default graphics settings
            GDM.IsFullScreen = false;
            GDM.PreferredBackBufferWidth = 1280;
            GDM.PreferredBackBufferHeight = 720;
            GDM.PreferMultiSampling = true;
            GDM.SynchronizeWithVerticalRetrace = Config.VerticalSync;
            IsFixedTimeStep = Config.VerticalSync;

            IsMouseVisible = true;

            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += ResizeHandler;
        }

        protected override void Initialize() {
            GUI = new ImGUIHandler(this);
            GUI.BuildFontAtlas();

            base.Initialize();

            // Load map
            Scene = new Scene();
            using (FileStream stream = File.OpenRead($"{ContentDirectory}/Maps/LostLevels.bin")) {
                using (BinaryReader reader = new BinaryReader(stream)) {
                    Scene.LoadMap(Map.FromBinary(MapPacker.ReadMapBinary(reader)));
                }
            }

            SettingsWindow.Prepare();
        }

        protected override void LoadContent() {
            Batch = new SpriteBatch(GraphicsDevice);
            VirtualContent = new List<VirtualTexture>();

            GFX.Load();

            base.LoadContent();
        }

        protected override void UnloadContent() {
            foreach (VirtualTexture t in VirtualContent) {
                t.Dispose();
            }

            base.UnloadContent();
        }

        protected override void Draw(GameTime gameTime) {
            base.Draw(gameTime);

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Config.BackgroundColor);

            // Render global GUI elements
            GUI.BeforeLayout(gameTime);
            MenuBar.Render();

            if (MenuBar.Settings) {
                SettingsWindow.Render();
            }

            // If map is loaded, render it
            if (Scene.LoadedMap != null) {
                Scene.Render();
            }

            GUI.AfterLayout();
        }

        protected override void Update(GameTime gameTime) {
            base.Update(gameTime);

            // If window is not focused, don't respond to user input
            if (!IsActive) {
                return;
            }

            InputHandler.Update();

            if (Scene.LoadedMap != null) {
                Scene.Update(gameTime);
            }

            // Set InputHandler previous states
            InputHandler.LastMouseRaw = Mouse.GetState();
            InputHandler.Previous = InputHandler.Current;
        }

        protected override void OnExiting(object sender, EventArgs args) {
            // Perform cleanup actions.
            Exit(0, false);

            base.OnExiting(sender, args);
        }

        private void ResizeHandler(object sender, EventArgs e) {
            Scene.Camera.UpdateViewport();
            RoomListWindow.UpdateListHeight();
        }
    }
}
