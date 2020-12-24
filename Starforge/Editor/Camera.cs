using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Starforge.Core;

namespace Starforge.Editor {
    /// <summary>
    /// Represents a modifiable viewport of the map.
    /// </summary>
    public class Camera {
        private Rectangle Bounds;
        private Viewport Viewport;
        private float ZoomLevel;

        /// <summary>
        /// This event is called when the camera state changes.
        /// </summary>
        public event PositionChange OnPositionChange;
        public delegate void PositionChange();

        /// <summary>
        /// The current position of the camera.
        /// </summary>
        public Vector2 Position { get; private set; }

        /// <summary>
        /// The camera transformation.
        /// </summary>
        public Matrix Transform { get; private set; }

        /// <summary>
        /// The inverse of the camera transformation.
        /// </summary>
        public Matrix Inverse { get; private set; }

        /// <summary>
        /// The area of the map which can be seen.
        /// </summary>
        public Rectangle VisibleArea { get; private set; }

        /// <summary>
        /// The current zoom level of the camera.
        /// </summary>
        public float Zoom {
            get => ZoomLevel;
            private set {
                ZoomLevel = MathHelper.Clamp(value, 0.001953125f, 4f);
                Update();
            }
        }

        public Camera() {
            Zoom = 1f;
            Position = Vector2.Zero;
            Viewport = Engine.Instance.GraphicsDevice.Viewport;
            Bounds = Viewport.Bounds;

            Engine.OnViewportUpdate += UpdateViewport;
        }

        /// <summary>
        /// Moves the camera the specified amount.
        /// </summary>
        /// <param name="amount">The amount to move the camera.</param>
        public void Move(Vector2 amount) {
            Position -= amount;
            Update();
        }

        /// <summary>
        /// Moves the camera to the specified position.
        /// </summary>
        /// <param name="position">The position to go to.</param>
        public void Goto(Vector2 position) {
            Position = position;
            Update();
        }

        /// <summary>
        /// Centers the camera on the specified position.
        /// </summary>
        /// <param name="position">The position to center on.</param>
        public void GotoCentered(Vector2 position) {
            Zoom = 1f;
            Position = new Vector2(position.X - (Viewport.Width / 2), position.Y - (Viewport.Height / 2));
            Update();
        }

        /// <summary>
        /// Updates the camera translations and bounds.
        /// </summary>
        public void Update() {
            Transform = Matrix.CreateTranslation(new Vector3(Position.X, Position.Y, 0))
                    * Matrix.CreateScale(Zoom)
                    * Matrix.CreateTranslation(new Vector3(Viewport.Width, Viewport.Height, 0));

            Inverse = Matrix.Invert(Transform);

            // Visible area calculations
            Vector2 tl = Vector2.Transform(Vector2.Zero, Inverse);
            Vector2 tr = Vector2.Transform(new Vector2(Viewport.Width, 0), Inverse);
            Vector2 bl = Vector2.Transform(new Vector2(0, Viewport.Height), Inverse);
            Vector2 br = Vector2.Transform(new Vector2(Viewport.Width, Viewport.Height), Inverse);

            Vector2 min = new Vector2(
                MathHelper.Min(tl.X, MathHelper.Min(tr.X, MathHelper.Min(bl.X, br.X))),
                MathHelper.Min(tl.Y, MathHelper.Min(tr.Y, MathHelper.Min(bl.Y, br.Y)))
            );

            Vector2 max = new Vector2(
                MathHelper.Max(tl.X, MathHelper.Max(tr.X, MathHelper.Max(bl.X, br.X))),
                MathHelper.Max(tl.Y, MathHelper.Max(tr.Y, MathHelper.Max(bl.Y, br.Y)))
            );

            VisibleArea = new Rectangle((int)min.X, (int)min.Y, (int)(max.X - min.X), (int)(max.Y - min.Y));

            // Invoke position change event
            OnPositionChange?.Invoke();
        }

        /// <summary>
        /// Zooms the camera in.
        /// </summary>
        /// <param name="pos">The position to center the camera on when zooming.</param>
        public void ZoomIn(Vector2 pos) {
            GotoCentered(ScreenToReal(pos));

            Zoom *= 2f;
            Update();

            GotoCentered(ScreenToReal(pos));
            Update();
        }

        /// <summary>
        /// Zooms the camera out.
        /// </summary>
        /// <param name="pos">The position to center the camera on when zooming.</param>
        public void ZoomOut(Vector2 pos) {
            GotoCentered(ScreenToReal(pos));

            Zoom /= 2f;
            Update();

            GotoCentered(ScreenToReal(pos));
            Update();
        }

        /// <summary>
        /// Converts a real (level/map) position to a position on the screen.
        /// </summary>
        /// <param name="pos">The position to convert.</param>
        /// <returns>The converted position.</returns>
        public Vector2 RealToScreen(Vector2 pos) => Vector2.Transform(pos, Transform);

        /// <summary>
        /// Converts a screen position to a real (level/map) position.
        /// </summary>
        /// <param name="pos">The position to convert.</param>
        /// <returns>The converted position.</returns>
        public Vector2 ScreenToReal(Vector2 pos) => Vector2.Transform(pos, Inverse);

        private void UpdateViewport() {
            Viewport = Engine.Instance.GraphicsDevice.Viewport;
            Update();
        }
    }
}
