using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Starforge.Core.Input {
    public struct MouseEvent {

        public bool LeftButtonClick;
        public bool LeftButtonDrag;
        public bool LeftButtonUnclick;

        public bool MiddleButtonClick;
        public bool MiddleButtonDrag;
        public bool MiddleButtonUnclick;

        public bool RightButtonClick;
        public bool RightButtonDrag;
        public bool RightButtonUnclick;

        public bool MouseMoved;

        public bool Scrolled;
        public int ScrollDistance;

        public Point Position;
        public Vector2 MouseMovement;

        public MouseEvent(MouseState mouseState) {
            Position = new Point() { X = mouseState.X, Y = mouseState.Y };

            MouseState previousMouseState = InputHandler.LastMouseRaw;
            LeftButtonClick     = mouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton != ButtonState.Pressed;
            LeftButtonDrag      = mouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Pressed;
            LeftButtonUnclick   = mouseState.LeftButton != ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Pressed;

            MiddleButtonClick   = mouseState.MiddleButton == ButtonState.Pressed && previousMouseState.MiddleButton != ButtonState.Pressed;
            MiddleButtonDrag    = mouseState.MiddleButton == ButtonState.Pressed && previousMouseState.MiddleButton == ButtonState.Pressed;
            MiddleButtonUnclick = mouseState.MiddleButton != ButtonState.Pressed && previousMouseState.MiddleButton == ButtonState.Pressed;

            RightButtonClick    = mouseState.RightButton == ButtonState.Pressed && previousMouseState.RightButton != ButtonState.Pressed;
            RightButtonDrag     = mouseState.RightButton == ButtonState.Pressed && previousMouseState.RightButton == ButtonState.Pressed;
            RightButtonUnclick  = mouseState.RightButton != ButtonState.Pressed && previousMouseState.RightButton == ButtonState.Pressed;

            MouseMoved = mouseState.X != previousMouseState.X || mouseState.Y != previousMouseState.Y;

            Scrolled = mouseState.ScrollWheelValue != previousMouseState.ScrollWheelValue;
            ScrollDistance = mouseState.ScrollWheelValue - previousMouseState.ScrollWheelValue;

            MouseMovement = new Vector2(previousMouseState.X - mouseState.X, previousMouseState.Y - mouseState.Y);
        }

        // returns true if any event occured
        public bool HasAny() {
            return LeftButtonClick || LeftButtonDrag || LeftButtonUnclick 
                || MiddleButtonClick || MiddleButtonDrag || MiddleButtonUnclick
                || RightButtonClick || RightButtonDrag || RightButtonUnclick
                || MouseMoved || Scrolled;
        }

        public Vector2 GetVectorPosition() {
            return new Vector2(Position.X, Position.Y);
        }

    }
}
