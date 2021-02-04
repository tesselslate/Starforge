using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Starforge.Core;
using Starforge.Editor.Actions;
using Starforge.Editor.Render;
using Starforge.Editor.UI;
using Starforge.Map;
using Starforge.Util;
using System;
using System.Collections.Generic;
using System.IO;

namespace Starforge.Editor {
    public class MapEditor : Scene {
        public Autotiler BGAutotiler { get; private set; }
        public Autotiler FGAutotiler { get; private set; }
        public Camera Camera { get; private set; }
        public EditorState State { get; private set; }
        public static MapEditor Instance { get; private set; }

        // Level rendering
        public LevelRender Renderer { get; private set; }

        // Windows and UI elements
        internal WindowRoomList RoomListWindow;
        public WindowToolList ToolListWindow;
        private ShortcutManager Shortcuts;

        public bool AcceptToolInput { get; private set; } = false;

        #region Scene

        public override void Begin() {
            Engine.MapLoaded = true;
            Logger.Log("Beginning map editor.");

            // Initialize room list window
            RoomListWindow = new WindowRoomList();
            ToolListWindow = new WindowToolList(BGAutotiler, FGAutotiler);

            List<string> roomNames = new List<string>();
            foreach (Room room in State.LoadedLevel.Rooms) roomNames.Add(room.Name);
            RoomListWindow.RoomNames = roomNames.ToArray();

            Engine.CreateWindow(RoomListWindow);
            Engine.CreateWindow(ToolListWindow);

            Engine.Instance.GraphicsDevice.SetRenderTarget(null);
            Engine.OnViewportUpdate += UpdateViewport;
            RoomListWindow.UpdateListHeight();
            ToolListWindow.UpdateListHeight();

            // Initialize shortcuts
            Shortcuts = new ShortcutManager();

            Shortcuts.RegisterShortcut(new Shortcut(Menubar.Open, Keys.LeftControl, Keys.O));
            Shortcuts.RegisterShortcut(new Shortcut(new Action(() => { Menubar.Save(); }), Keys.LeftControl, Keys.S));
            Shortcuts.RegisterShortcut(new Shortcut(Menubar.SaveAs, Keys.LeftControl, Keys.LeftShift, Keys.S));
            Shortcuts.RegisterShortcut(new Shortcut(State.Redo, Keys.LeftControl, Keys.LeftShift, Keys.Z));
            Shortcuts.RegisterShortcut(new Shortcut(State.Undo, Keys.LeftControl, Keys.Z));
        }

        public override bool End() {
            if (State.Unsaved) {
                return false;
            }

            Engine.MapLoaded = false;
            Engine.OnViewportUpdate -= Camera.UpdateViewport;
            Engine.OnViewportUpdate -= UpdateViewport;
            Renderer.Dispose();

            // Remove windows
            RoomListWindow.Visible = false;
            ToolListWindow.Visible = false;

            return true;
        }

        public override void Render(GameTime gt) {
            ImGuiIOPtr io = ImGui.GetIO();

            if (State.SelectedRoom != null) {
                Engine.Instance.GraphicsDevice.SetRenderTarget(Renderer.Overlay);
                Engine.Instance.GraphicsDevice.Clear(Color.Transparent);
                if(!io.WantCaptureMouse) {
                    Engine.Batch.Begin(
                        SpriteSortMode.Deferred,
                        BlendState.AlphaBlend,
                        SamplerState.PointClamp,
                        DepthStencilState.None,
                        RasterizerState.CullNone,
                        null
                    );
                    ToolManager.Render();
                    Engine.Batch.End();
                }
            }

            Renderer.Render();
        }

        public override void Update(GameTime gt) {
            UpdateState();

            // Ignore user input in certain cases
            if (!Engine.Instance.IsActive) return;
            if (Shortcuts.Update()) return;

            ImGuiIOPtr io = ImGui.GetIO();
            if (io.WantCaptureMouse || io.WantCaptureKeyboard) {
                UIHelper.SetCursor(Engine.GUIRenderer.Cursors[ImGui.GetMouseCursor()]);
                return;
            } else {
                UIHelper.SetCursor();
            }

            //////////////////////
            // Handle user inputs
            //////////////////////

            // Right click drag - move camera
            if (Input.Mouse.RightHold && Input.Mouse.Moved) {
                Camera.Move(Input.Mouse.Movement / Camera.Zoom);
            }

            // Scrolled - zoom camera
            if (Input.Mouse.Scrolled) {
                if (Input.Mouse.ScrollAmount > 0) {
                    // User scrolled up
                    Camera.ZoomIn(Input.Mouse.GetVectorPos());
                } else {
                    // User scrolled down
                    Camera.ZoomOut(Input.Mouse.GetVectorPos());
                }
            }

            // Left click - select room or edit room
            if (Input.Mouse.LeftClick) {
                Vector2 rm = Camera.ScreenToReal(Input.Mouse.GetVectorPos());
                Point realPoint = new Point((int)rm.X, (int)rm.Y);

                // Check to see if selected room should change
                if(!(State.SelectedRoom != null && State.SelectedRoom.Meta.Bounds.Contains(realPoint))) {
                    foreach (DrawableRoom room in Renderer.VisibleRooms) {
                        if (room.Room.Meta.Bounds.Contains(realPoint)) {
                            SelectRoom(room);
                            return;
                        }
                    }
                }
            }

            if (State.SelectedRoom != null && AcceptToolInput) ToolManager.Update();
            if (!AcceptToolInput && (Input.Mouse.LeftUnclick || Input.Mouse.Moved)) AcceptToolInput = true;
        }

        public void UpdateState() {
            if (State == null) return;
            if (State.SelectedRoom != null) {
                Vector2 rm = Camera.ScreenToReal(Input.Mouse.GetVectorPos());
                State.TilePointer = new Point((int)Math.Floor((rm.X - State.SelectedRoom.X) / 8), (int)Math.Floor((rm.Y - State.SelectedRoom.Y) / 8));
                State.PixelPointer = new Point((int)Math.Floor(rm.X - State.SelectedRoom.X), (int)Math.Floor(rm.Y - State.SelectedRoom.Y));
            }
        }

        public void UpdateViewport() {
            RoomListWindow.UpdateListHeight();
            ToolListWindow.UpdateListHeight();
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
                    LoadLevel(Level.Decode(MapPacker.ReadMapBinary(reader)), mapPath);
                }
            }
        }

        public void LoadLevel(Level level, string path) {
            Instance = this;

            if (State != null && State.LoadedLevel != null) {
                Logger.Log(LogLevel.Warning, $"MapEditor: Attempted to load {level.Package} while {State.LoadedLevel.Package} was already loaded.");
                return;
            }

            State = new EditorState();
            State.PastActions = new Stack<EditorAction>();
            State.FutureActions = new Stack<EditorAction>();

            Engine.MapLoaded = true;
            State.LoadedLevel = level;
            State.LoadedPath = path;

            Logger.Log($"MapEditor: Loading level {level.Package}");

            // Initialize autotiler
            BGAutotiler = new Autotiler($"{Settings.CelesteDirectory}/Content/Graphics/BackgroundTiles.xml");
            FGAutotiler = new Autotiler($"{Settings.CelesteDirectory}/Content/Graphics/ForegroundTiles.xml");

            // Initialize camera and level renderer
            Camera = new Camera();
            Renderer = new LevelRender(this, State.LoadedLevel);
            foreach (DrawableRoom room in Renderer.Rooms) Renderer.RenderRoom(room);

            Camera.Update();

            if (State.LoadedLevel.Rooms.Count > 0) {
                SelectRoom(0, true);
            }
        }

        public void RerenderAll(RenderFlags toRender) {
            foreach (DrawableRoom room in Renderer.Rooms) Renderer.RenderRoom(room, toRender);
        }

        /// <summary>
        /// Selects the given room.
        /// </summary>
        /// <param name="index">The index of the room to select.</param>
        /// <param name="moveCamera"></param>
        public void SelectRoom(int index, bool moveCamera = false) {
            AcceptToolInput = false;

            // Rerender previously selected room
            if (State.SelectedRoom != null) Renderer.SelectedRoom.Dirty = true;
            State.SelectedRoom = State.LoadedLevel.Rooms[index];

            Renderer.SetSelected(State.LoadedLevel.Rooms[index]);

            if (!moveCamera) return;
            Camera.Zoom = 1f;
            Camera.GotoCentered(new Vector2(-State.SelectedRoom.Meta.Bounds.Center.X, -State.SelectedRoom.Meta.Bounds.Center.Y));
        }

        public void SelectRoom(DrawableRoom room, bool moveCamera = false) {
            SelectRoom(Renderer.Rooms.IndexOf(room), moveCamera);
        }
    }
}
