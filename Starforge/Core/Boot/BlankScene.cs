using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Starforge.Editor.UI;
using System;

namespace Starforge.Core.Boot {
    public class BlankScene : Scene {
        private ShortcutManager Shortcuts;

        public override void Begin() {
            Shortcuts = new ShortcutManager();
            Shortcuts.RegisterShortcut(new Shortcut(new Action(Menubar.Open), Keys.LeftControl, Keys.O));
        }

        public override bool End() => true;

        public override void Render(GameTime gt) {
            Engine.Instance.GraphicsDevice.Clear(Settings.BackgroundColor);

            Menubar.Render(false);
        }

        public override void Update(GameTime gt) {
            Shortcuts.Update();
        }
    }
}
