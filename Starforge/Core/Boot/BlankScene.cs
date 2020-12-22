using Microsoft.Xna.Framework;
using Starforge.Editor.UI;

namespace Starforge.Core.Boot {
    public class BlankScene : Scene {
        public override void Begin() { }
        public override bool End() => true;

        public override void Render(GameTime gt) {
            Engine.Instance.GraphicsDevice.Clear(Settings.BackgroundColor);

            Menubar.Render(false);
        }

        public override void Update(GameTime gt) { }
    }
}
