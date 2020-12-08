using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Starforge.Core;
using Starforge.MapStructure;
using Starforge.MapStructure.Tiling;
using System.Collections.Generic;
using System.Diagnostics;

namespace Starforge.Editor {
    public class Scene {
        public Camera Camera {
            get;
            private set;
        }

        public Map LoadedMap;

        public Autotiler BGAutotiler;

        public Autotiler FGAutotiler;

        public KeyboardState PreviousKeyboardState {
            get;
            private set;
        }

        public MouseState PreviousMouseState {
            get;
            private set;
        }

        public List<Level> VisibleLevels {
            get;
            private set;
        }

        private List<Level> LevelTargets;

        public Scene() {
            VisibleLevels = new List<Level>();

            Camera = new Camera(Engine.Instance.GraphicsDevice.Viewport);

            // Update visible level list when the camera is moved
            Camera.PositionChange += UpdateVisibleLevels;
            Camera.Update();

            PreviousKeyboardState = new KeyboardState();
            PreviousMouseState = new MouseState();
        }

        public void LoadMap(Map map) {
            LoadedMap = map;

            BGAutotiler = new Autotiler("./Content/Graphics/BackgroundTiles.xml");
            FGAutotiler = new Autotiler("./Content/Graphics/ForegroundTiles.xml");

            Camera.Zoom = 1f;
            Camera.GotoCentered(Vector2.Zero);
            Camera.Update();
        }

        public void Update() {
            KeyboardState kbd = Keyboard.GetState();
            MouseState m = Mouse.GetState();

            if(Engine.Instance.IsActive) {
                if(m.ScrollWheelValue > PreviousMouseState.ScrollWheelValue) {
                    // Scrolled up
                    Camera.ZoomIn(new Vector2(m.X, m.Y));
                } else if(m.ScrollWheelValue < PreviousMouseState.ScrollWheelValue) {
                    // Scrolled down
                    Camera.ZoomOut(new Vector2(m.X, m.Y));
                }

                if(m.RightButton == ButtonState.Pressed) {
                    if(m.X != PreviousMouseState.X || m.Y != PreviousMouseState.Y) {
                        // User is dragging mouse
                        Camera.Move(new Vector2(PreviousMouseState.X - m.X, PreviousMouseState.Y - m.Y) / Camera.Zoom);
                    } else {
                        // User clicked mouse

                    }
                }
            }

            // Set previous keyboard/mouse state
            PreviousKeyboardState = kbd;
            PreviousMouseState = m;
        }

        public void UpdateVisibleLevels() {
            if(LoadedMap != null) {
                List<Level> visible = new List<Level>();
                foreach(Level level in LoadedMap.Levels) {
                    if(Camera.VisibleArea.Intersects(level.Bounds)) {
                        visible.Add(level);
                    }
                }

                VisibleLevels = visible;
            }
        }

        public void Render() {
            LevelTargets = new List<Level>();

            // Rerender "dirty" levels (those which need to be rerendered)
            foreach(Level level in VisibleLevels) {
                if(level.Dirty) level.Render();
                LevelTargets.Add(level);
            }

            Engine.Instance.GraphicsDevice.SetRenderTarget(null);
            Engine.Instance.GraphicsDevice.Clear(Engine.Config.BackgroundColor);

            Engine.Batch.Begin(SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointWrap, null, 
                RasterizerState.CullNone, null,
                Camera.Transform);

            LoadedMap.Render();

            foreach(Level level in LevelTargets) {
                Engine.Batch.Draw(level.Target, level.Position, Color.White);
            }

            Engine.Batch.End();
        }
    }
}
