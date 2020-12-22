using Microsoft.Xna.Framework;
using Starforge.Core;

namespace Starforge.Editor {
    public class MapEditor : Scene {
        /// <summary>
        /// The instance of the currently running map editor.
        /// </summary>
        public static MapEditor Instance { get; private set; }

        public MapEditor() {
            Instance = this;
        }

        public override void Begin() {
            Logger.Log("Beginning map editor.");
        }

        public override bool End() {
            throw new System.NotImplementedException();
        }

        public override void Render(GameTime gt) {
            throw new System.NotImplementedException();
        }

        public override void Update(GameTime gt) {
            throw new System.NotImplementedException();
        }
    }
}
