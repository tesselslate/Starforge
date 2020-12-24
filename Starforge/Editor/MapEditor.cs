using ImGuiNET;
using Microsoft.Xna.Framework;
using Starforge.Core;
using Starforge.Editor.Render;
using Starforge.Editor.UI;
using Starforge.Map;

namespace Starforge.Editor {
    public class MapEditor : Scene {
        public Autotiler BGAutotiler { get; private set; }
        public Autotiler FGAutotiler { get; private set; }
        public Camera Camera { get; private set; }
        public Level Level { get; private set; }

        /// <summary>
        /// The renderer responsible for drawing the level onscreen.
        /// </summary>
        private LevelRender Renderer;

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

        #region Scene

        public override void Begin() {
            Logger.Log("Beginning map editor.");
        }

        public override bool End() {
            throw new System.NotImplementedException();
        }

        public override void Render(GameTime gt) {
            Renderer.Render();
            Menubar.Render(true);
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
        /// <param name="level">The level to load.</param>
        public void LoadLevel(Level level) {
            if (Level != null) {
                Logger.Log(LogLevel.Warning, $"MapEditor: Attempted to load {level.Package} while {Level.Package} was already loaded.");
                return;
            }

            Logger.Log($"MapEditor: Loading level {level.Package}");

            // Load level
            Level = level;
            BGAutotiler = new Autotiler($"{Settings.CelesteDirectory}/Content/Graphics/BackgroundTiles.xml");
            FGAutotiler = new Autotiler($"{Settings.CelesteDirectory}/Content/Graphics/ForegroundTiles.xml");

            Camera = new Camera();
            Renderer = new LevelRender(this, level, true);

            // Center camera on first room and update it.
            if (Level.Rooms.Count > 0) {
                Camera.GotoCentered(new Vector2(-Level.Rooms[0].X, -Level.Rooms[0].Y));
           }
            Camera.Update();
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
    }
}
