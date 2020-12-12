using ImGuiNET;
using Starforge.Core;
using Starforge.MapStructure.Tiling;

namespace Starforge.Editor.UI {
    public static class ToolWindow {
        public static int CurrentBGTileset;

        public static int CurrentFGTileset;

        public static string[] BGTilesets;

        public static string[] FGTilesets;

        public static int CurrentTilesetList = 0;

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
            if(CurrentTilesetList == 0) {
                ImGui.ListBox("", ref CurrentFGTileset, FGTilesets, FGTilesets.Length, 30);
            } else {
                ImGui.ListBox("", ref CurrentBGTileset, BGTilesets, BGTilesets.Length, 30);
            }

            ImGui.ListBox("Tools", ref CurrentTilesetList, new string[] { "Foreground", "Background" }, 2);
            
            ImGui.End();
        }
    }
}
