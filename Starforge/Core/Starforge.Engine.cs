using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Starforge.Mod.Assets;
using System.Collections.Generic;

namespace Starforge.Core {
    public partial class Starforge : Game {
        public static SpriteBatch Batch { 
            get; 
            private set; 
        }

        public Atlas atlas;

        public List<VirtualTexture> VirtualContent;

        private Starforge() {
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

            
        }

        protected override void LoadContent() {
            base.LoadContent();

            Batch = new SpriteBatch(GraphicsDevice);
            VirtualContent = new List<VirtualTexture>();
        }

        protected override void UnloadContent() {
            base.UnloadContent();
        }

        protected override void Draw(GameTime gameTime) {
            base.Draw(gameTime);

            GraphicsDevice.Clear(Color.White);
        }

        protected override void Update(GameTime gameTime) {
            base.Update(gameTime);
        }
    }
}
