using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()

namespace Starforge.Core.Input {
    public class Shortcut {

        // flag that remembers if shortcut was already pressed last update
        private bool WasPressed;

        public Tuple<Keys, Keys, Keys> ShortcutButtons;
        private Action Callback;

        public Shortcut(Action callback, Keys first, Keys second = Keys.None, Keys third = Keys.None) {
            ShortcutButtons = new Tuple<Keys, Keys, Keys>(first, second, third);
            Callback = callback;
        }

        public bool Handle(KeyboardState keyboardState) {
            if (!IsPressed(keyboardState)) {
                WasPressed = false;
                return false;
            }

            if (WasPressed) {
                return false;
            }

            WasPressed = true;
            Callback.Invoke();

            return true;
        }

        private bool IsPressed(KeyboardState keyboardState) {
            List<Keys> pressedKeys = new List<Keys>(keyboardState.GetPressedKeys());
            
            if (!pressedKeys.Remove(ShortcutButtons.Item1)) {
                return false;
            }
            if (ShortcutButtons.Item2 != Keys.None) {
                if (!pressedKeys.Remove(ShortcutButtons.Item2)) {
                    return false;
                }
                if (ShortcutButtons.Item3 != Keys.None) {
                    if (!pressedKeys.Remove(ShortcutButtons.Item3)) {
                        return false;
                    }
                }
            }
            if (pressedKeys.Count != 0) {
                return false;
            }
            return true;
        }

        public override bool Equals(object other) {
            if (other.GetType() != GetType()) {
                return false;
            }
            return ShortcutButtons.Equals(((Shortcut)other).ShortcutButtons);
        }

        public override string ToString() {
            string s = ShortcutButtons.Item1.ToString();
            if (ShortcutButtons.Item2 != Keys.None) {
                s += "+" + ShortcutButtons.Item2.ToString();
            }
            if (ShortcutButtons.Item3 != Keys.None) {
                s += "+" + ShortcutButtons.Item3.ToString();
            }
            return s;
        }

    }
}
