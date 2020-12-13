using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Starforge.Core;
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

        public static void Manage(MouseState m, Level l) {
            ToolHint = new Rectangle(l.TilePointer.X * TILE_SIZE, l.TilePointer.Y * TILE_SIZE, TILE_SIZE, TILE_SIZE);

            bool changed = false;
            switch (ToolWindow.CurrentTool) {
            case ToolType.Point:
                changed = HandlePointTool(m, l);
                break;
            case ToolType.Rectangle:
                changed = HandleRectangleTool(m, l);
                break;
            }

            if (changed) {
                l.Dirty = true;
            }
        }

        private static bool HandlePointTool(MouseState m, Level l) {
            if (m.LeftButton != ButtonState.Pressed) {
                return false;
            }
            bool changed = false;
            switch (ToolWindow.CurrentTileType) {
            case TileType.Foreground:
                changed = setPoint(Engine.Scene.FGAutotiler, l.ForegroundTiles, l.FgGrid, ToolWindow.CurrentFGTileset, l.TilePointer);
                break;
            case TileType.Background:
                changed = setPoint(Engine.Scene.BGAutotiler, l.BackgroundTiles, l.BgGrid, ToolWindow.CurrentBGTileset, l.TilePointer);
                break;
            }

            return changed;
        }

        private static bool HandleRectangleTool(MouseState m, Level l) {
            bool changed = false;

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
                // Create rectangle
                switch (ToolWindow.CurrentTileType) {
                case TileType.Foreground:
                    changed = setRectangle(Engine.Scene.FGAutotiler, l.ForegroundTiles, l.FgGrid, ToolWindow.CurrentFGTileset, Hold);
                    break;
                case TileType.Background:
                    changed = setRectangle(Engine.Scene.BGAutotiler, l.BackgroundTiles, l.BgGrid, ToolWindow.CurrentBGTileset, Hold);
                    break;
                }
                Hold = default;
            }

            return changed;
        }

        // Sets a single tile to a given tileset, returns true if anything changed
        private static bool setPoint(Autotiler tiler, TileGrid tileGrid, StaticTexture[] textures, int tileset, Point position) {
            if (tileset == 0) {
                tileGrid.SetTile(position.X, position.Y, '0');
            }
            else {
                tileGrid.SetTile(position.X, position.Y, tiler.GetTilesetList()[tileset - 1].ID);
            }

            return tiler.Update(tileGrid, textures, position);
        }

        // Sets a rectangle to a given tileset, returns true if anything changed
        private static bool setRectangle(Autotiler tiler, TileGrid tileGrid, StaticTexture[] textures, int tileset, Rectangle area) {
            for (int x = area.X; x < area.X + area.Width; x++) {
                for (int y = area.Y; y < area.Y + area.Height; y++) {
                    if (tileset == 0) {
                        tileGrid.SetTile(x, y, '0');
                    }
                    else {
                        tileGrid.SetTile(x, y, tiler.GetTilesetList()[tileset - 1].ID);
                    }
                }
            }

            return tiler.Update(tileGrid, textures, area);
        }
    }

    public enum ToolType {
        Point,
        Rectangle
    }

    public enum TileType {
        Foreground,
        Background
    }
}
