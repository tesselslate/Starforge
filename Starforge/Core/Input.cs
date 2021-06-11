using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SDL2;

namespace Starforge.Core {
    /// <summary>
    /// Contains information about the current keyboard/mouse state.
    /// </summary>
    public static class Input {
        private static MouseState PreviousMouseState = default;
        private static KeyboardState PreviousKeyboardState = default;

        private static MouseState CurrentMouseState = default;
        private static KeyboardState CurrentKeyboardState = default;

        public static MouseInput     Mouse;
        public static KeyboardInput  Keyboard;

        public static void Reset() {
            SDL.SDL_HideWindow(Engine.Instance.Window.Handle);
            SDL.SDL_ShowWindow(Engine.Instance.Window.Handle);
        }

        /// <summary>
        /// Updates the current mouse/keyboard states.
        /// </summary>
        public static void Update() {
            PreviousMouseState = CurrentMouseState;
            PreviousKeyboardState = CurrentKeyboardState;

            CurrentMouseState = Microsoft.Xna.Framework.Input.Mouse.GetState();
            CurrentKeyboardState = Microsoft.Xna.Framework.Input.Keyboard.GetState();

            Mouse = new MouseInput(CurrentMouseState, PreviousMouseState);
            Keyboard = new KeyboardInput(CurrentKeyboardState, PreviousKeyboardState);
        }

    }

    public struct MouseInput {
        public bool LeftClick;
        public bool LeftHold;
        public bool LeftUnclick;

        public bool MiddleClick;
        public bool MiddleHold;
        public bool MiddleUnclick;

        public bool RightClick;
        public bool RightHold;
        public bool RightUnclick;

        public bool Moved;
        public bool Scrolled;
        public int ScrollAmount;

        public Point Position;
        public Vector2 Movement;

        public MouseInput(MouseState state, MouseState prev) {
            Position = new Point(state.X, state.Y);

            LeftClick     = state.LeftButton   == ButtonState.Pressed && prev.LeftButton   != ButtonState.Pressed;
            LeftHold      = state.LeftButton   == ButtonState.Pressed && prev.LeftButton   == ButtonState.Pressed;
            LeftUnclick   = state.LeftButton   != ButtonState.Pressed && prev.LeftButton   == ButtonState.Pressed;

            MiddleClick   = state.MiddleButton == ButtonState.Pressed && prev.MiddleButton != ButtonState.Pressed;
            MiddleHold    = state.MiddleButton == ButtonState.Pressed && prev.MiddleButton == ButtonState.Pressed;
            MiddleUnclick = state.MiddleButton != ButtonState.Pressed && prev.MiddleButton == ButtonState.Pressed;

            RightClick    = state.RightButton  == ButtonState.Pressed && prev.RightButton  != ButtonState.Pressed;
            RightHold     = state.RightButton  == ButtonState.Pressed && prev.RightButton  == ButtonState.Pressed;
            RightUnclick  = state.RightButton  != ButtonState.Pressed && prev.RightButton  == ButtonState.Pressed;

            Moved = state.X != prev.X || state.Y != prev.Y;
            Scrolled = state.ScrollWheelValue != prev.ScrollWheelValue;
            ScrollAmount = state.ScrollWheelValue - prev.ScrollWheelValue;

            Movement = new Vector2(prev.X - state.X, prev.Y - state.Y);
        }

        /// <summary>
        /// Determines if any mouse button states have changed.
        /// </summary>
        public bool HasAny() => 
            LeftClick   || LeftHold   || LeftUnclick   || 
            MiddleClick || MiddleHold || MiddleUnclick || 
            RightClick  || RightHold  || RightUnclick;
        
        /// <summary>
        /// Gets the position of the mouse as a Vector2.
        /// </summary>
        /// <returns></returns>
        public Vector2 GetVectorPos() => new Vector2(Position.X, Position.Y);
    }

    public struct KeyboardInput {
        private KeyboardState current;
        private KeyboardState previous;

        public KeyboardInput(KeyboardState c, KeyboardState p) {
            current = c;
            previous = p;
        }

        /// <summary>
        /// Returns an array of all currently pressed keys.
        /// </summary>
        public Keys[] GetPressedKeys() => current.GetPressedKeys();
        public bool IsKeyDown(Keys key) => current.IsKeyDown(key);
        public bool IsKeyUp(Keys key) => current.IsKeyUp(key);

        public bool Pressed(Keys key) => current.IsKeyDown(key) && !previous.IsKeyDown(key);
        public bool Released(Keys key) => !current.IsKeyDown(key) && previous.IsKeyDown(key);
        public bool Held(Keys key) => current.IsKeyDown(key) && previous.IsKeyDown(key);
    }
}
