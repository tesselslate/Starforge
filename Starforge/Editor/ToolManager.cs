using Starforge.Editor.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starforge.Editor {
    public static class ToolManager {
        /// <summary>
        /// A dictionary containing all available tools.
        /// </summary>
        public static Dictionary<ToolType, Tool> Tools;

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
                [ToolType.TileBrush] = new TileBrushTool()
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
