using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Starforge.Core;
using Starforge.Editor.Render;
using Starforge.Editor.UI;
using Starforge.Map;
using System;
using System.Collections.Generic;
using System.IO;

namespace Starforge.Editor {
    public class MapEditor : Scene {
        public Autotiler BGAutotiler { get; private set; }
        public Autotiler FGAutotiler { get; private set; }
        public Camera Camera { get; private set; }
        public Level Level { get; private set; }

        /// <summary>
        /// The currently selected room.
        /// </summary>
        public Room SelectedRoom { get; private set; }

        /// <summary>
        /// The path of the map on the disk.
        /// </summary>
        public string MapPath;

        public static MapEditor Instance { get; private set; }

        /// <summary>
        /// Whether or not there are actions which can be undone.
        /// </summary>
        public bool CanUndo { get; private set; } = false;

        /// <summary>
        /// Whether or not there are actions which can be redone.
        /// </summary>
        public bool CanRedo { get; private set; } = false;

        /// <summary>
        /// Whether or not the level has unsaved changes.
        /// </summary>
        public bool Unsaved { get; private set; } = false;

        /// <summary>
        /// The renderer responsible for drawing the level onscreen.
        /// </summary>
        private LevelRender Renderer;
        private WindowRoomList RoomListWindow;
        private ShortcutManager Shortcuts;

        #region Scene

        public override void Begin() {
            Instance = this;
            Logger.Log("Beginning map editor.");

            // Initialize room list window
            RoomListWindow = new WindowRoomList();

            List<string> roomNames = new List<string>();
            foreach (Room room in Level.Rooms) roomNames.Add(room.Name);
            RoomListWindow.RoomNames = roomNames.ToArray();

            Engine.CreateWindow(RoomListWindow);

            // TODO: Initialize tool window

            // Initialize shortcuts
            Shortcuts = new ShortcutManager();

            Shortcuts.RegisterShortcut(new Shortcut(Menubar.Open, Keys.LeftControl, Keys.O));
            Shortcuts.RegisterShortcut(new Shortcut(new Action(() => { Menubar.Save(); }), Keys.LeftControl, Keys.S));
            Shortcuts.RegisterShortcut(new Shortcut(Menubar.SaveAs, Keys.LeftControl, Keys.LeftShift, Keys.S));
            Shortcuts.RegisterShortcut(new Shortcut(Undo, Keys.LeftControl, Keys.Z));
            Shortcuts.RegisterShortcut(new Shortcut(Redo, Keys.LeftControl, Keys.LeftShift, Keys.Z));
        }

        public override bool End() {
            if (Unsaved) {
                return false;
            }

            Engine.MapLoaded = false;
            Engine.OnViewportUpdate -= Camera.UpdateViewport;
            Renderer.Dispose();

            // Remove windows
            RoomListWindow.Visible = false;

            return true;
        }

        public override void Render(GameTime gt) {
            RoomListWindow.UpdateListHeight();
            Renderer.Render();
        }

        public override void Update(GameTime gt) {
            // If ImGui wants user input or the window isn't focused, dont respond.
            ImGuiIOPtr io = ImGui.GetIO();
            if (io.WantCaptureMouse || io.WantCaptureKeyboard || !Engine.Instance.IsActive) return;

            // Handle user inputs
            if (Input.Mouse.RightHold && Input.Mouse.Moved) {
                // Dragging right click - move camera
                Camera.Move(Input.Mouse.Movement / Camera.Zoom);
            }

            if (Input.Mouse.Scrolled) {
                if (Input.Mouse.ScrollAmount > 0) {
                    // User scrolled up
                    Camera.ZoomIn(Input.Mouse.GetVectorPos());
                } else {
                    // User scrolled down
                    Camera.ZoomOut(Input.Mouse.GetVectorPos());
                }
            }
        }

        #endregion

        /// <summary>
        /// Loads a level in the current map editor. This can only be used once per instance.
        /// </summary>
        /// <param name="mapPath">The path of the level to load.</param>
        public void LoadLevel(string mapPath) {
            Logger.Log($"MapEditor: Loading level {mapPath}");

            using (FileStream stream = File.OpenRead(mapPath)) {
                using (BinaryReader reader = new BinaryReader(stream)) {
                    LoadLevel(Level.Decode(MapPacker.ReadMapBinary(reader)));
                }
            }
        }

        public void LoadLevel(Level level) {
            if (Level != null) {
                Logger.Log(LogLevel.Warning, $"MapEditor: Attempted to load {level.Package} while {Level.Package} was already loaded.");
                return;
            }

            Level = level;

            Logger.Log($"MapEditor: Loading level {Level.Package}");

            // Initialize autotiler
            BGAutotiler = new Autotiler($"{Settings.CelesteDirectory}/Content/Graphics/BackgroundTiles.xml");
            FGAutotiler = new Autotiler($"{Settings.CelesteDirectory}/Content/Graphics/ForegroundTiles.xml");

            // Initialize camera and level renderer
            Camera = new Camera();
            Renderer = new LevelRender(this, Level, true);
            Camera.Update();

            if (Level.Rooms.Count > 0) {
                SelectRoom(0);
            }
        }

        /// <summary>
        /// Undoes the previous action.
        /// </summary>
        public void Undo() {

        }

        /// <summary>
        /// Redoes the previously undone action.
        /// </summary>
        public void Redo() {

        }

        public void SelectRoom(int index) {
            // Rerender previously selected room
            if (SelectedRoom != null) Renderer.RenderRoom(Renderer.Rooms[index]);
            SelectedRoom = Level.Rooms[index];

            Camera.Zoom = 1f;
            Camera.GotoCentered(new Vector2(-SelectedRoom.Meta.Bounds.Center.X, -SelectedRoom.Meta.Bounds.Center.Y));
        }
    }
}
