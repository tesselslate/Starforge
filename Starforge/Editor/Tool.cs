namespace Starforge.Editor {
    public abstract class Tool {
        public abstract string GetName();
        public abstract void Update();
        public abstract void Render();
        public abstract void RenderGUI();
        public abstract bool CanSelectLayer();
    }

    public enum ToolLayer {
        Background,
        Foreground
    }
}
