using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Starforge.Core.Input {
    public static class InputHandler {

        private static List<WeakReference<Pressable>> Pressables;

        public static void Handle(KeyboardState keyboardState) {

        }

        public static void RegisterPressable(Pressable p) {
            Pressables.Add(new WeakReference<Pressable>(p));
        }

        private static void DeregisterPressable(WeakReference<Pressable> p) {
            Pressables.Remove(p);
        }

    }
}
