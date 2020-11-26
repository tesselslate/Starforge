namespace Starforge.Util {
    public struct Rectangle {
        public int X;
        public int Y;
        public int Width;
        public int Height;

        public Rectangle(int x, int y, int w, int h) {
            X = x;
            Y = y;
            Width = w;
            Height = h;
        }
        
        public bool CollideCheck(int cx, int cy) {
            return X <= cx && cx < X + Width && Y <= cy && cy < Y + Height;
        }
    }
}
