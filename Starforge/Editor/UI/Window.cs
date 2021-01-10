namespace Starforge.Editor.UI {
    public abstract class Window {
        public bool Visible = true;

        public abstract void Render();

        public virtual void End() { }
    }
}
