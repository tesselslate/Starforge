using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Starforge.Core;

namespace Starforge.Editor {
    public class Camera {
        private Rectangle Bounds;

        public Vector2 Position {
            get;
            private set;
        }

        public Matrix Inverse {
            get;
            private set;
        }

        public Matrix Transform {
            get;
            private set;
        }

        public Rectangle VisibleArea {
            get;
            private set;
        }

        private Viewport Viewport;

        private float zoom;

        public event OnPositionChange PositionChange;

        public delegate void OnPositionChange();

        public float Zoom {
            get => zoom;
            set {
                zoom = MathHelper.Clamp(value, 0.1f, 2.5f);
                Update();
            }
        }

        public Camera(Viewport viewport) {
            Zoom = 1f;
            Position = Vector2.Zero;
            Viewport = viewport;
            Bounds = Viewport.Bounds;
        }

        public void Move(Vector2 amount) {
            Position -= amount;
            Update();
        }

        public void Goto(Vector2 position) {
            Position = new Vector2(position.X, position.Y);
            Update();
        }

        public void GotoCentered(Vector2 position) {
            Position = new Vector2(VisibleArea.Center.X, VisibleArea.Center.Y);
        }

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
            PositionChange?.Invoke();
        }

        // Convert a position on the map to a position on the screen.
        public Vector2 RealToScreen(Vector2 position) {
            return Vector2.Transform(position, Transform);
        }

        // Convert a position on the screen to a position on the map.
        public Vector2 ScreenToReal(Vector2 position) {
            return Vector2.Transform(position, Matrix.Invert(Transform));
        }
    }
}
