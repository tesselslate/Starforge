using ImGuiNET;
using Microsoft.Xna.Framework;
using SDL2;
using Starforge.Core;
using System;
using System.Collections.Generic;

namespace Starforge.Util {
    /// <summary>
    /// Contains various functions to make UI creation easier.
    /// </summary>
    public static class UIHelper {
        public static SDL.SDL_SystemCursor Cursor = SDL.SDL_SystemCursor.SDL_SYSTEM_CURSOR_ARROW;
        public static Dictionary<SDL.SDL_SystemCursor, IntPtr> Cursors = new Dictionary<SDL.SDL_SystemCursor, IntPtr>();


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

        /// <summary>
        /// Displays a tooltip when the user hovers over the previously created widget.
        /// </summary>
        /// <param name="msg">The tooltip to display.</param>
        public static void Tooltip(string msg) {
            if (ImGui.IsItemHovered()) {
                ImGui.BeginTooltip();
                ImGui.Text(msg);
                ImGui.EndTooltip();
            }
        }

        /// <summary>
        /// Sets the cursor to the given system cursor.
        /// </summary>
        /// <param name="cursor">The SDL cursor to set the system cursor to.</param>
        public static void SetCursor(SDL.SDL_SystemCursor cursor = SDL.SDL_SystemCursor.SDL_SYSTEM_CURSOR_ARROW) {
            if (Cursor == cursor) return;

            Cursor = cursor;
            SDL.SDL_SetCursor(Cursors[cursor]);
        }
    }
}
