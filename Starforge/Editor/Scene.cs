using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Starforge.Core;
using Starforge.Core.Input;
using Starforge.Editor.UI;
using Starforge.MapStructure;
using Starforge.MapStructure.Tiling;
using Starforge.Util;
using System.Collections.Generic;

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

        public Level SelectedLevel {
            get;
            private set;
        }

        public Scene() {
            VisibleLevels = new List<Level>();

            Camera = new Camera();

            // Update visible level list when the camera is moved
            Camera.PositionChange += UpdateVisibleLevels;
            Camera.Update();

            PreviousKeyboardState = new KeyboardState();
            PreviousMouseState = new MouseState();

            InputHandler.RegisterShortcut(new Shortcut(Undo, Keys.LeftControl, Keys.Z));
            InputHandler.RegisterShortcut(new Shortcut(Redo, Keys.LeftControl, Keys.LeftShift, Keys.Z));
            InputHandler.RegisterShortcut(new Shortcut(Redo, Keys.LeftControl, Keys.Y));
        }

        public void LoadMap(Map map) {
            LoadedMap = map;
            if (LoadedMap.Levels.Count > 0) {
                SelectedLevel = LoadedMap.Levels[0];
                SelectedLevel.SetSelected(true);
            }

            BGAutotiler = new Autotiler($"{Engine.ContentDirectory}/Graphics/BackgroundTiles.xml");
            FGAutotiler = new Autotiler($"{Engine.ContentDirectory}/Graphics/ForegroundTiles.xml");

            Camera.Zoom = 1f;
            Camera.GotoCentered(new Vector2(-SelectedLevel.Bounds.Center.X, -SelectedLevel.Bounds.Center.Y));
            Camera.Update();

            List<string> roomNames = new List<string>();
            foreach (Level level in map.Levels) {
                roomNames.Add(level.Name);
            }
            RoomListWindow.RoomNames = roomNames.ToArray();

            // Tilesets
            ToolWindow.BGTilesets.Clear();
            ToolWindow.FGTilesets.Clear();
            ToolWindow.BGTilesets.Add("Air");
            ToolWindow.FGTilesets.Add("Air");

            foreach (Tileset t in BGAutotiler.GetTilesetList()) {
                ToolWindow.BGTilesets.Add(MiscHelper.CleanCamelCase(t.Path.Substring(2)));
            }
            foreach (Tileset t in FGAutotiler.GetTilesetList()) {
                ToolWindow.FGTilesets.Add(MiscHelper.CleanCamelCase(t.Path));
            }
        }

        public void Update(GameTime gt) {
            MouseState mouseState = Mouse.GetState();

            KeyboardState kbd = Keyboard.GetState();
            MouseEvent mouseEvent = new MouseEvent(mouseState);
            ImGuiIOPtr io = ImGui.GetIO();

            // Only process input for editor if ImGUI doesn't currently want user input
            // (e.g. user is not hovering over/focused on GUI elements)
            if (!io.WantCaptureKeyboard && !io.WantCaptureMouse && Engine.Instance.IsActive) {
                HandleUserInput(mouseEvent, kbd, gt);
            }

            // Detect if user changed selected room
            if (LoadedMap.Levels[RoomListWindow.CurrentRoom] != SelectedLevel && RoomListWindow.LastRoom != RoomListWindow.CurrentRoom) {
                ChangeSelectedRoom(RoomListWindow.CurrentRoom, true);
            }

            InputHandler.Handle(kbd);

            // Set previous keyboard/mouse state
            PreviousKeyboardState = kbd;
            PreviousMouseState = mouseState;
        }

        private void ChangeSelectedRoom(int newRoom, bool moveCamera) {
            SelectedLevel.SetSelected(false);
            SelectedLevel.Render();

            SelectedLevel = LoadedMap.Levels[newRoom];
            SelectedLevel.SetSelected(true);
            SelectedLevel.WasSelected = false;
            SelectedLevel.Render();

            if (moveCamera) {
                // Don't ask me how the camera works. It makes no sense to me honestly.
                // It breaks on anything other than default zoom, so it's just set back to that when
                // going to a room from the list.
                Camera.Zoom = 1f;
                Camera.GotoCentered(new Vector2(-SelectedLevel.Bounds.Center.X, -SelectedLevel.Bounds.Center.Y));
                Camera.Update();
            }

            RoomListWindow.SetCurrentRoom(newRoom);
        }

        private void HandleUserInput(MouseEvent m, KeyboardState kbd, GameTime gt) {
            if (m.hasAny()) {
                UpdateZoom(m);
                UpdateDrag(m);
                UpdateClick(m);
            }

            // Send inputs to level for further processing
            SelectedLevel.Update(kbd, m, gt);
        }

        private void Undo() {
            SelectedLevel.Undo();
        }

        private void Redo() {
            SelectedLevel.Redo();
        }

        private void UpdateZoom(MouseEvent m) {
            if (!m.Scrolled) {
                return;
            }

            if (m.ScrollDistance > 0) {
                // Scrolled up
                Camera.ZoomIn(m.getVectorPosition());
            }

            if (m.ScrollDistance < 0) {
                // Scrolled down
                Camera.ZoomOut(m.getVectorPosition());
            }
        }

        private void UpdateDrag(MouseEvent m) {
            if (!m.RightButtonDrag) {
                return;
            }

            Camera.Move(m.MouseMovement / Camera.Zoom);
        }

        private void UpdateClick(MouseEvent m) {
            if (!m.LeftButtonClick) {
                return;
            }

            // User clicked mouse
            Vector2 realPos = Camera.ScreenToReal(m.getVectorPosition());
            Point point = new Point((int)realPos.X, (int)realPos.Y);

            // Search for room that was clicked on
            // Most common option, check for currently selected level first
            if (SelectedLevel.Bounds.Contains(point)) {
                return;
            }

            for (int i = 0; i < LoadedMap.Levels.Count; i++) {
                if (LoadedMap.Levels[i].Bounds.Contains(point)) {
                    ChangeSelectedRoom(i, false);
                    break;
                }
            }
        }

        public void UpdateVisibleLevels() {
            if (LoadedMap == null) {
                return;
            }
            List<Level> visible = new List<Level>();
            foreach (Level level in LoadedMap.Levels) {
                if (Camera.VisibleArea.Intersects(level.Bounds)) {
                    visible.Add(level);
                }
            }

            VisibleLevels = visible;
        }

        public void Render(GameTime gt) {
            // Rerender "dirty" levels (those which need to be rerendered)
            foreach (Level level in VisibleLevels) {
                level.Render();
            }

            Engine.Instance.GraphicsDevice.SetRenderTarget(null);
            Engine.Instance.GraphicsDevice.Clear(Engine.Config.BackgroundColor);

            Engine.Batch.Begin(SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointClamp, null,
                RasterizerState.CullNone, null,
                Camera.Transform);

            LoadedMap.Render();

            // draw each level
            foreach (Level level in VisibleLevels) {
                Engine.Batch.Draw(level.Target, level.Position, Color.White);
            }
            // draw the selected level's overlay
            Engine.Batch.Draw(SelectedLevel.Overlay, SelectedLevel.Position, Color.White);
            
            Engine.Batch.End();

            // Render ImGUI content
            Engine.GUI.BeforeLayout(gt);

            MenuBar.Render();
            RoomListWindow.Render();
            ToolWindow.Render();

            if (MenuBar.Settings) {
                SettingsWindow.Render();
            }

            Engine.GUI.AfterLayout();
        }
    }
}
