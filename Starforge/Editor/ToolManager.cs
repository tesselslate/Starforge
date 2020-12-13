using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Starforge.Core;
using Starforge.Editor.Actions;
using Starforge.Editor.UI;
using Starforge.MapStructure;
using Starforge.MapStructure.Tiling;
using Starforge.Mod.Assets;
using System;

namespace Starforge.Editor {
    public static class ToolManager {
        private const int TILE_SIZE = 8;

        public static Rectangle ToolHint = new Rectangle(0, 0, TILE_SIZE, TILE_SIZE);

        private static Rectangle Hold = default;
        private static Point HoldStart;

        private static DrawTilePlacement CurrentDrawAction = null;

        public static void Manage(MouseState m, Level l) {
            ToolHint = new Rectangle(l.TilePointer.X * TILE_SIZE, l.TilePointer.Y * TILE_SIZE, TILE_SIZE, TILE_SIZE);

            switch (ToolWindow.CurrentTool) {
            case ToolType.TileDraw:
                HandlePointTool(m, l);
                break;
            case ToolType.TileRectangle:
                HandleRectangleTool(m, l);
                break;
            }

        }

        private static void HandlePointTool(MouseState m, Level l) {
            if (m.LeftButton != ButtonState.Pressed) {
                if (Engine.Scene.PreviousMouseState.LeftButton == ButtonState.Pressed) {
                    // if the user just let go of press, add action to recent Actions of the level
                    //l.PastActions.Add(CurrentDrawAction);
                }
                return;
            }

            // when newly pressing button down
            if (Engine.Scene.PreviousMouseState.LeftButton == ButtonState.Released) {
                switch (ToolWindow.CurrentTileType) {
                case TileType.Foreground:
                    CurrentDrawAction = new DrawTilePlacement(l, ToolWindow.CurrentTileType, ToolWindow.CurrentFGTileset, l.TilePointer);
                    break;
                case TileType.Background:
                    CurrentDrawAction = new DrawTilePlacement(l, ToolWindow.CurrentTileType, ToolWindow.CurrentBGTileset, l.TilePointer);
                    break;
                }
                l.ApplyNewAction(CurrentDrawAction);
                return;
            }

            // when holding and continuously drawing, add to existing action
            CurrentDrawAction.AddPoint(l.TilePointer);
        }

        private static void HandleRectangleTool(MouseState m, Level l) {
            if (m.LeftButton == ButtonState.Pressed) {
                if (Engine.Scene.PreviousMouseState.LeftButton == ButtonState.Released) {
                    // Just started holding LMB
                    Hold = new Rectangle(l.TilePointer.X, l.TilePointer.Y, 1, 1);
                    HoldStart = new Point(l.TilePointer.X, l.TilePointer.Y);
                }
                else {
                    // Thank you Cruor for telling me how to make this not bad :)
                    Point topLeftCorner = new Point(
                        (int)MathHelper.Min(HoldStart.X, l.TilePointer.X),
                        (int)MathHelper.Min(HoldStart.Y, l.TilePointer.Y)
                    );
                    Point botRightCorner = new Point(
                        (int)MathHelper.Max(HoldStart.X, l.TilePointer.X),
                        (int)MathHelper.Max(HoldStart.Y, l.TilePointer.Y)
                    );
                    Hold = new Rectangle(
                        topLeftCorner.X, 
                        topLeftCorner.Y, 
                        botRightCorner.X - topLeftCorner.X + 1,
                        botRightCorner.Y - topLeftCorner.Y + 1
                    );
                }
                ToolHint = new Rectangle(Hold.X * TILE_SIZE, Hold.Y * TILE_SIZE, Hold.Width * TILE_SIZE, Hold.Height * TILE_SIZE);
            }
            else if (Engine.Scene.PreviousMouseState.LeftButton == ButtonState.Pressed) {
                // Create rectangle action
                switch (ToolWindow.CurrentTileType) {
                case TileType.Foreground:
                    l.ApplyNewAction(new RectangleTilePlacement(l, ToolWindow.CurrentTileType, ToolWindow.CurrentFGTileset, Hold));
                    break;
                case TileType.Background:
                    l.ApplyNewAction(new RectangleTilePlacement(l, ToolWindow.CurrentTileType, ToolWindow.CurrentBGTileset, Hold));
                    break;
                }
                Hold = default;
            }
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
