using Starforge.Editor.Tools;
using Starforge.Mod.API;
using System.Collections.Generic;

namespace Starforge.Editor {
    public static class ToolManager {
        /// <summary>
        /// A dictionary containing all available tools.
        /// </summary>
        public static Dictionary<ToolType, Tool> Tools;

        /// <summary>
        /// The currently selected entity placement.
        /// </summary>
        public static Placement SelectedEntity;

        /// <summary>
        /// The currently selected tool.
        /// </summary>
        public static Tool SelectedTool;

        /// <summary>
        /// The currently selected tool layer (background/foreground).
        /// </summary>
        public static ToolLayer SelectedLayer;

        /// <summary>
        /// The index of the currently selected background tileset.
        /// </summary>
        public static int BGTileset;

        /// <summary>
        /// The index of the currently selected foreground tileset.
        /// </summary>
        public static int FGTileset;

        static ToolManager() {
            Tools = new Dictionary<ToolType, Tool>()
            {
                [ToolType.TileBrush] = new TileBrushTool(),
                [ToolType.TileRectangle] = new TileRectangleTool(),
                [ToolType.Entity] = new EntityTool()
            };

            SelectedTool = Tools[ToolType.TileBrush];
            SelectedLayer = ToolLayer.Foreground;

            BGTileset = 0;
            FGTileset = 0;
        }

        public static void Render() {
            SelectedTool.Render();
        }

        public static void Update() {
            SelectedTool.Update();
        }
    }
}
