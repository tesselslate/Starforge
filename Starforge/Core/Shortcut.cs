using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()

namespace Starforge.Core {
    public class Shortcut {
        /// <summary>
        /// Internal flag keeping track of whether or not the shortcut was activated the previous time it was updated.
        /// </summary>
        private bool WasPressed;

        /// <summary>
        /// The keys required to active the shortcut.
        /// </summary>
        public readonly Tuple<Keys, Keys, Keys> ShortcutButtons;

        /// <summary>
        /// The action which is called when the shortcut is activated.
        /// </summary>
        private Action Callback;

        /// <summary>
        /// The name of the shortcut.
        /// </summary>
        public readonly string Name = "Unnamed Shortcut";

        public Shortcut(Action callback, Keys first, Keys second = Keys.None, Keys third = Keys.None) {
            ShortcutButtons = new Tuple<Keys, Keys, Keys>(first, second, third);
            Callback = callback;
        }

        public Shortcut(string name, Action callback, Keys first, Keys second = Keys.None, Keys third = Keys.None) : this(callback, first, second, third) {
            Name = name;
        }

        /// <summary>
        /// Determines whether or not this Shortcut and the provided Shortcut have equal keys.
        /// </summary>
        /// <param name="obj">The Shortcut to test equality for.</param>
        /// <returns>Whether or not the two shortcuts have the same keys.</returns>
        public override bool Equals(object obj) {
            if (obj.GetType() != GetType()) return false;
            return ShortcutButtons.Equals(((Shortcut)obj).ShortcutButtons);
        }

        /// <summary>
        /// Converts the shortcut to a string.
        /// </summary>
        /// <returns>The shortcut's key combo, formatted as a string.</returns>
        public override string ToString() {
            string res = ShortcutButtons.Item1.ToString();
            if (ShortcutButtons.Item2 != Keys.None) res += "+" + ShortcutButtons.Item2.ToString();
            if (ShortcutButtons.Item3 != Keys.None) res += "+" + ShortcutButtons.Item3.ToString();

            return res;
        }

        /// <summary>
        /// Activates the shortcut (if appropriate) based on the given keyboard state.
        /// </summary>
        /// <param name="kbd">The keyboard state to test.</param>
        /// <returns>Whether or not the shortcut was activated.</returns>
        public bool Handle(KeyboardInput kbd) {
            if (!IsPressed(kbd)) {
                return WasPressed = false;
            }

            if (WasPressed) {
                return false;
            }

            Callback.Invoke();
            return WasPressed = true;
        }

        /// <summary>
        /// Determine whether the shortcut should be activated based on the provided keyboard state.
        /// </summary>
        /// <param name="kbd">The keyboard state to test.</param>
        /// <returns>Whether or not the shortcut is currently being pressed.</returns>
        private bool IsPressed(KeyboardInput kbd) {
            List<Keys> keys = new List<Keys>(kbd.GetPressedKeys());
            if (!keys.Remove(ShortcutButtons.Item1)) return false;
            if (ShortcutButtons.Item2 != Keys.None && !keys.Remove(ShortcutButtons.Item2)) return false;
            if (ShortcutButtons.Item3 != Keys.None && !keys.Remove(ShortcutButtons.Item3)) return false;
            if (keys.Count > 0) return false;

            return true;
        }
    }
}
