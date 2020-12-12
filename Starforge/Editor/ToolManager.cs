using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Starforge.Core;
using Starforge.Editor.UI;
using Starforge.MapStructure;
using System;

namespace Starforge.Editor {
    public static class ToolManager {
        public static Rectangle ToolHint = new Rectangle(0, 0, 8, 8);

        private static Rectangle Hold = default;
        private static Point HoldStart;

        public static void Manage(MouseState m, Level l) {
            switch(ToolWindow.CurrentTool) {
                case ToolType.Point:
                    ToolHint = new Rectangle(l.TilePointer.X * 8, l.TilePointer.Y * 8, 8, 8);
                    if(m.LeftButton == ButtonState.Pressed) {
                        if(ToolWindow.CurrentTilesetList == 0) {
                            if(ToolWindow.CurrentFGTileset == 0) l.ForegroundTiles.SetTile(l.TilePointer.X, l.TilePointer.Y, 48);
                            else l.ForegroundTiles.SetTile(l.TilePointer.X, l.TilePointer.Y, Engine.Scene.FGTilesets[ToolWindow.CurrentFGTileset - 1].ID);

                            Engine.Scene.FGAutotiler.Update(l.ForegroundTiles, l.FgGrid, l.TilePointer);
                        } else if(ToolWindow.CurrentTilesetList == 1) {
                            if(ToolWindow.CurrentBGTileset == 0) l.BackgroundTiles.SetTile(l.TilePointer.X, l.TilePointer.Y, 48);
                            else l.BackgroundTiles.SetTile(l.TilePointer.X, l.TilePointer.Y, Engine.Scene.BGTilesets[ToolWindow.CurrentBGTileset - 1].ID);

                            Engine.Scene.BGAutotiler.Update(l.BackgroundTiles, l.BgGrid, l.TilePointer);
                        }
                    }
                    break;
                case ToolType.Rectangle:
                    if(m.LeftButton == ButtonState.Pressed) {
                        if(Engine.Scene.PreviousMouseState.LeftButton == ButtonState.Released) {
                            // Just started holding LMB
                            Hold = new Rectangle(l.TilePointer.X, l.TilePointer.Y, 1, 1);
                            HoldStart = new Point(l.TilePointer.X, l.TilePointer.Y);
                        } else {
                            // Thank you Cruor for telling me how to make this not bad :)
                            Hold = new Rectangle(
                                (int)MathHelper.Min(HoldStart.X, l.TilePointer.X),
                                (int)MathHelper.Min(HoldStart.Y, l.TilePointer.Y),
                                Math.Abs(HoldStart.X - l.TilePointer.X),
                                Math.Abs(HoldStart.Y - l.TilePointer.Y)
                            );
                        }
                    } else {
                        if(Engine.Scene.PreviousMouseState.LeftButton == ButtonState.Pressed) {
                            // Create rectangle
                            switch(ToolWindow.CurrentTilesetList) {
                                case 0:
                                    for(int i = Hold.X; i < Hold.X + Hold.Width; i++) {
                                        for(int j = Hold.Y; j < Hold.Y + Hold.Height; j++) {
                                            if(ToolWindow.CurrentFGTileset == 0) l.ForegroundTiles.SetTile(i, j, 48);
                                            else l.ForegroundTiles.SetTile(i, j, Engine.Scene.FGTilesets[ToolWindow.CurrentFGTileset - 1].ID);
                                        }
                                    }

                                    Engine.Scene.FGAutotiler.Update(l.ForegroundTiles, l.FgGrid, new Rectangle(
                                        Hold.X - 1,
                                        Hold.Y - 1,
                                        Hold.Width + 2,
                                        Hold.Height + 2
                                    ));
                                    break;
                                case 1:
                                    for(int i = Hold.X; i < Hold.X + Hold.Width; i++) {
                                        for(int j = Hold.Y; j < Hold.Y + Hold.Height; j++) {
                                            if(ToolWindow.CurrentBGTileset == 0) l.BackgroundTiles.SetTile(i, j, 48);
                                            else l.BackgroundTiles.SetTile(i, j, Engine.Scene.BGTilesets[ToolWindow.CurrentBGTileset - 1].ID);
                                        }
                                    }

                                    Engine.Scene.BGAutotiler.Update(l.BackgroundTiles, l.BgGrid, new Rectangle(
                                        Hold.X - 1,
                                        Hold.Y - 1,
                                        Hold.Width + 2,
                                        Hold.Height + 2
                                    ));
                                    break;
                            }

                            Hold = default;
                        }
                    }

                    ToolHint = new Rectangle(Hold.X * 8, Hold.Y * 8, Hold.Width * 8, Hold.Height * 8);
                    break;
            }
        }
    }

    public enum ToolType {
        Point,
        Rectangle
    }
}
