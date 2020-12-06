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
        }

        public void LoadMap(Map map) {
            LoadedMap = map;
        }

        public void Update() {
            Camera.Update();

            KeyboardState kbd = Keyboard.GetState();
            if(kbd.IsKeyDown(Keys.W)) {
                Camera.Move(new Vector2(0, -10));
            }
            if(kbd.IsKeyDown(Keys.A)) {
                Camera.Move(new Vector2(-10, 0));
            }
            if(kbd.IsKeyDown(Keys.S)) {
                Camera.Move(new Vector2(0, 10));
            }
            if(kbd.IsKeyDown(Keys.D)) {
                Camera.Move(new Vector2(10, 0));
            }
            if(kbd.IsKeyDown(Keys.Down)) {
                Camera.Zoom -= 0.1f;
            }
            if(kbd.IsKeyDown(Keys.Up)) {
                Camera.Zoom += 0.1f;
            }
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
