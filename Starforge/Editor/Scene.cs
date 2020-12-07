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
        }

        public void Update() {
            KeyboardState kbd = Keyboard.GetState();
            MouseState m = Mouse.GetState();

            if(m.ScrollWheelValue > PreviousMouseState.ScrollWheelValue) {
                // Scrolled up
                Camera.Zoom += 0.1f;
            } else if(m.ScrollWheelValue < PreviousMouseState.ScrollWheelValue) {
                // Scrolled down
                Camera.Zoom -= 0.1f;
            }

            if(m.LeftButton == ButtonState.Pressed) {
                if(m.X != PreviousMouseState.X || m.Y != PreviousMouseState.Y) {
                    // User is dragging mouse
                    Camera.Move(new Vector2(PreviousMouseState.X - m.X, PreviousMouseState.Y - m.Y) / Camera.Zoom);
                } else {
                    // User clicked mouse
                    
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
            Stopwatch watch = new Stopwatch();
            watch.Start();

            Engine.Batch.Begin(SpriteSortMode.Deferred,
                               BlendState.AlphaBlend,
                               null, null, null, null,
                               Camera.Transform);

            LoadedMap.Render();

            foreach(Level level in VisibleLevels) {
                level.Render();
            }

            Engine.Batch.End();

            watch.Stop();
            Logger.Log($"Rendered in {watch.ElapsedTicks} ticks");
        }
    }
}
