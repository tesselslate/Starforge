namespace Starforge.Editor.Tools {
    public abstract class Tool {
        public abstract string GetName();
        public abstract void Update();
        public abstract void Render();
    }

    public enum ToolLayer {
        Background,
        Foreground
    }

    public enum ToolType {
        TileBrush,
        TileRectangle,
        Entity,
        EntitySelection
    }
}
