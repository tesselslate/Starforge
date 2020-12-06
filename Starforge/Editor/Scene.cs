using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Starforge.Core;
using Starforge.MapStructure;
using System.Collections.Generic;

namespace Starforge.Editor {
    public class Scene {
        public Camera Camera {
            get;
            private set;
        }

        public Map LoadedMap;

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
        }

        public void Update() {
            Camera.Update();

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
                    if(
                        Camera.VisibleArea.Contains(level.TopLeft) ||
                        Camera.VisibleArea.Contains(level.TopRight) ||
                        Camera.VisibleArea.Contains(level.BottomLeft) ||
                        Camera.VisibleArea.Contains(level.BottomRight)
                     ) {
                        visible.Add(level);
                    }
                }

                VisibleLevels = visible;
            }
        }

        public void Render() {
            Engine.Batch.Begin(SpriteSortMode.Deferred,
                               BlendState.AlphaBlend,
                               null, null, null, null,
                               Camera.Transform);

            foreach(Level level in VisibleLevels) {
                GFX.Pixel.Draw(level.Bounds, Color.SlateGray);
            }

            Engine.Batch.End();
        }
    }
}
