using ImGuiNET;
using Starforge.Core;

namespace Starforge.Editor.UI {
    public static class ToolWindow {
        public static int CurrentBGTileset;
        public static int CurrentFGTileset;

        public static string[] BGTilesets;
        public static string[] FGTilesets;
        public static string[] Tools;

        public static int CurrentTilesetList = 0;
        public static ToolType CurrentTool = ToolType.Point;

        private static int CurrentToolInt = 0;
        private static string[] Layers = new string[] { "Foreground", "Background" };

        public static void Render() {
            ImGui.SetNextWindowPos(new System.Numerics.Vector2(Engine.Instance.GraphicsDevice.Viewport.Width - 150f, 0f));
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(150f, Engine.Instance.GraphicsDevice.Viewport.Height));
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0f);
            ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, 0f);

            ImGui.Begin("Tools",
                ImGuiWindowFlags.NoDecoration |
                ImGuiWindowFlags.NoScrollbar |
                ImGuiWindowFlags.NoResize |
                ImGuiWindowFlags.NoTitleBar |
                ImGuiWindowFlags.NoMove);

            ImGui.PopStyleVar();
            ImGui.PopStyleVar();

            ImGui.SetNextItemWidth(135f);
            if (CurrentTilesetList == 0) {
                ImGui.ListBox("", ref CurrentFGTileset, FGTilesets, FGTilesets.Length, 30);
            }
            else {
                ImGui.ListBox("", ref CurrentBGTileset, BGTilesets, BGTilesets.Length, 30);
            }

            ImGui.ListBox("Layer", ref CurrentTilesetList, Layers, 2);
            ImGui.ListBox("Tool", ref CurrentToolInt, Tools, Tools.Length);

            CurrentTool = (ToolType)CurrentToolInt;

            ImGui.End();
        }
    }
}
