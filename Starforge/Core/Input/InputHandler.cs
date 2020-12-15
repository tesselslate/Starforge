using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Starforge.Core.Input {
    public static class InputHandler {

        private static List<Shortcut> ShortcutCallbacks = new List<Shortcut>();

        public static void Handle(KeyboardState keyboardState) {
            foreach(Shortcut shortcut in ShortcutCallbacks) {
                if (shortcut.Handle(keyboardState)) {
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
}
