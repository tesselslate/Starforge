using Microsoft.Xna.Framework.Input;
using Starforge.Editor.UI;
using Starforge.Editor.Tools;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace Starforge.Editor {
    public static class ToolManager {

        public static Dictionary<ToolType, Tool> Tools = new Dictionary<ToolType, Tool>() {
            [ToolType.TileDraw] = new TileDrawTool(),
            [ToolType.TileRectangle] = new TileRectangleTool(),
            [ToolType.EntityPlace] = new EntityPlaceTool()
        };

        public static void Manage(MouseState m) {
            Tools[ToolWindow.CurrentTool].ManageInput(m);
        }

        // Renders the tools overlays/hints on the given target
        public static void Render() {
            Tools[ToolWindow.CurrentTool].Render();
        }

    }

    public enum ToolType {
        TileDraw,
        TileRectangle,
        EntityPlace
    }

    public enum TileType {
        Foreground,
        Background
    }
}
