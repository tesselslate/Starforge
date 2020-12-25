using ImGuiNET;
using Microsoft.Xna.Framework;
using Starforge.Core;

namespace Starforge.Util {
    /// <summary>
    /// Contains various functions to make UI creation easier.
    /// </summary>
    public static class UIHelper {
        /// <summary>
        /// Displays a tooltip when the user hovers over the previously created widget.
        /// </summary>
        /// <param name="msg">The tooltip to display.</param>
        public static void Tooltip(string msg) {
            if(ImGui.IsItemHovered()) {
                ImGui.BeginTooltip();
                ImGui.Text(msg);
                ImGui.EndTooltip();
            }
        }

        /// <summary>
        /// Centers and positions a window of the given size.
        /// </summary>
        /// <param name="size">The size to make the window.</param>
        public static void CenterWindow(Vector2 size) {
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(size.X, size.Y));
            ImGui.SetNextWindowPos(new System.Numerics.Vector2(Engine.Instance.GraphicsDevice.Viewport.Width / 2 - size.X / 2, Engine.Instance.GraphicsDevice.Viewport.Height / 2 - size.Y / 2));
        }

        /// <summary>
        /// Centers and positions a window of the given size.
        /// </summary>
        /// <param name="width">The window width.</param>
        /// <param name="height">The window height.</param>
        public static void CenterWindow(float width, float height) {
            CenterWindow(new Vector2(width, height));
        }
    }
}
