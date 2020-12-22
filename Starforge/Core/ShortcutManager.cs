using System.Collections.Generic;

namespace Starforge.Core {
    /// <summary>
    /// Maintains and activates a list of shortcuts.
    /// </summary>
    public class ShortcutManager {
        private List<Shortcut> Shortcuts = new List<Shortcut>();
        
        /// <summary>
        /// Iterate through the list of registered shortcuts and activate any which should be activated.
        /// </summary>
        /// <returns>Whether or not a shortcut was activated.</returns>
        public bool Update() {
            foreach (Shortcut shortcut in Shortcuts) {
                if (shortcut.Handle(Input.Keyboard)) return true;
            }

            return false;
        }

        /// <summary>
        /// Attempts to register a shortcut.
        /// </summary>
        /// <param name="shortcut">The shortcut to register.</param>
        /// <returns>Whether or not the shortcut was successfully registered.</returns>
        public bool RegisterShortcut(Shortcut shortcut) {
            if (Shortcuts.Contains(shortcut)) {
                Logger.Log(LogLevel.Warning, $"Attempted to register shortcut {shortcut.Name}, but another shortcut has the same keybindings. ({shortcut}");
                return false;
            }

            Shortcuts.Add(shortcut);
            return true;
        }
    }
}
