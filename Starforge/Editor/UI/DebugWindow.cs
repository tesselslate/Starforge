using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Starforge.Core;

namespace Starforge.Editor.UI {
    public static class DebugWindow {
        public static void Render() {
            MouseState m = Mouse.GetState();

            ImGui.Begin("Debug");
            ImGui.Text(string.Format("Performance: {0:F3} ms/frame ({1:F1} FPS)", 1000f / ImGui.GetIO().Framerate, ImGui.GetIO().Framerate));
            ImGui.Text($"Cursor: {Engine.Scene.Camera.ScreenToReal(new Vector2(m.X, m.Y))}");
            ImGui.Text($"Current room: {Engine.Scene.SelectedLevel.Name}");
            ImGui.Text($"Visible rooms: {Engine.Scene.VisibleLevels.Count}");
            ImGui.End();
        }
    }
}
