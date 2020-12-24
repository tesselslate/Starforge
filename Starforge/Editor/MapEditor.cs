using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Starforge.Core;
using Starforge.Editor.Render;
using Starforge.Map;

namespace Starforge.Editor {
    public class MapEditor : Scene {
        public Autotiler BGAutotiler { get; private set; }
        public Autotiler FGAutotiler { get; private set; }
        public Camera Camera { get; private set; }
        public Level Level { get; private set; }

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
            Camera = new Camera();
            Camera.Update();
        }

        public override bool End() {
            throw new System.NotImplementedException();
        }

        public override void Render(GameTime gt) {
            Renderer.Render();
        }

        public override void Update(GameTime gt) {
            if (Input.Keyboard.IsKeyDown(Keys.W)) Camera.Move(new Vector2(0, -10));
            if (Input.Keyboard.IsKeyDown(Keys.A)) Camera.Move(new Vector2(-10, 0));
            if (Input.Keyboard.IsKeyDown(Keys.S)) Camera.Move(new Vector2(0, 10));
            if (Input.Keyboard.IsKeyDown(Keys.D)) Camera.Move(new Vector2(10, 0));
        }

        #endregion

        public void LoadLevel(Level level) {
            // Load level
            Level = level;
            BGAutotiler = new Autotiler($"{Settings.CelesteDirectory}/Content/Graphics/BackgroundTiles.xml");
            FGAutotiler = new Autotiler($"{Settings.CelesteDirectory}/Content/Graphics/ForegroundTiles.xml");

            Renderer = new LevelRender(this, level, true);
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
