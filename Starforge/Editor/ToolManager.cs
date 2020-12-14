using Microsoft.Xna.Framework.Input;
using Starforge.Editor.UI;
using Starforge.Editor.Tools;
using Starforge.MapStructure;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace Starforge.Editor {
    public static class ToolManager {

        public static Dictionary<ToolType, Tool> Tools = new Dictionary<ToolType, Tool>() {
            [ToolType.TileDraw] = new TileDrawTool(),
            [ToolType.TileRectangle] = new TileRectangleTool()
        };

        public static void Manage(MouseState m, Level l) {
            Tools[ToolWindow.CurrentTool].ManageInput(m, l);
        }

        // Renders the tools overlays/hints on the given target
        public static void Render(RenderTarget2D target) {
            Tools[ToolWindow.CurrentTool].Render(target);
        }

    }

    public enum ToolType {
        TileDraw,
        TileRectangle
    }

    public enum TileType {
        Foreground,
        Background
    }
}
