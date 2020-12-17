using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Starforge.Core.Input {
    public static class InputHandler {
        private static List<Shortcut> ShortcutCallbacks = new List<Shortcut>();
        public static InputState Current;
        public static InputState Previous;
        public static MouseState LastMouseRaw;

        public static void Update() {
            Current = new InputState()
            {
                Mouse = new MouseEvent(Mouse.GetState()),
                Keyboard = Keyboard.GetState()
            };

            foreach(Shortcut shortcut in ShortcutCallbacks) {
                if (shortcut.Handle(Current.Keyboard)) {
                    return;
                }
            }
        }

        public static void RegisterShortcut(Shortcut shortcut) {
            if (ShortcutCallbacks.Contains(shortcut)) {
                // if this shortcut is already registered
                Logger.Log("Tried to add shortcut \"" + shortcut + "\" but it was already Registered");
                return;
            }
            ShortcutCallbacks.Add(shortcut);
        }
    }

    public struct InputState {
        public MouseEvent Mouse;
        public KeyboardState Keyboard;
    }
}
