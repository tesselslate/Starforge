using ImGuiNET;
using Starforge.Core;

namespace Starforge.Editor.UI {
    public static class DebugWindow {
        public static void Render() {
            ImGui.Begin("Debug");
            ImGui.Text(string.Format("Application average {0:F3} ms/frame ({1:F1} FPS)", 1000f / ImGui.GetIO().Framerate, ImGui.GetIO().Framerate));
            ImGui.Text($"Visible rooms: {Engine.Scene.VisibleLevels.Count}");
            ImGui.End();
        }
    }
}
