using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Starforge.Core;
using Starforge.Editor.UI;
using Starforge.MapStructure;

namespace Starforge.Editor {
    public static class ToolManager {
        public static Rectangle ToolHint = new Rectangle(0, 0, 8, 8);

        private static Rectangle Hold = default;

        public static RerenderType Manage(MouseState m, Level l) {
            switch(ToolWindow.CurrentTool) {
                case ToolType.Point:
                    ToolHint = new Rectangle(l.TilePointer.X * 8, l.TilePointer.Y * 8, 8, 8);
                    if(m.LeftButton == ButtonState.Pressed) {
                        if(ToolWindow.CurrentTilesetList == 0) {
                            if(ToolWindow.CurrentFGTileset == 0) l.ForegroundTiles.SetTile(l.TilePointer.X, l.TilePointer.Y, 48);
                            else l.ForegroundTiles.SetTile(l.TilePointer.X, l.TilePointer.Y, Engine.Scene.FGTilesets[ToolWindow.CurrentFGTileset - 1].ID);
                            return RerenderType.Foreground;
                        } else if(ToolWindow.CurrentTilesetList == 1) {
                            if(ToolWindow.CurrentBGTileset == 0) l.BackgroundTiles.SetTile(l.TilePointer.X, l.TilePointer.Y, 48);
                            else l.BackgroundTiles.SetTile(l.TilePointer.X, l.TilePointer.Y, Engine.Scene.BGTilesets[ToolWindow.CurrentBGTileset - 1].ID);

                            return RerenderType.Background;
                        }
                    }
                    break;
                case ToolType.Rectangle:
                    /*if(m.LeftButton == ButtonState.Pressed) {
                        if(Engine.Scene.PreviousMouseState.LeftButton == ButtonState.Released) {
                            // Just started holding
                            Hold = new Rectangle(l.TilePointer.X, l.TilePointer.Y, 1, 1);
                        } else {
                            // Continuing to hold down LMB
                            Hold = new Rectangle(Hold.X, Hold.Y, l.TilePointer.X - Hold.X, l.TilePointer.Y - Hold.Y);
                        }

                        ToolHint = new Rectangle(Hold.X * 8, Hold.Y * 8, Hold.Width * 8, Hold.Height * 8);
                    } else if(m.LeftButton == ButtonState.Released && Engine.Scene.PreviousMouseState.LeftButton == ButtonState.Pressed) {
                        ToolHint = new Rectangle(l.TilePointer.X * 8, l.TilePointer.Y * 8, 8, 8);

                        // Released LMB - create rectangle
                        if(ToolWindow.CurrentTilesetList == 0) {
                            for(int i = Hold.X; i < Hold.Right; i++) {
                                for(int j = Hold.Y; j < Hold.Bottom; j++) {
                                    if(ToolWindow.CurrentFGTileset == 0) l.ForegroundTiles.SetTile(i, j, 48);
                                    else l.ForegroundTiles.SetTile(i, j, Engine.Scene.FGTilesets[ToolWindow.CurrentFGTileset - 1].ID);
                                }
                            }

                            return RerenderType.Foreground;
                        } else if(ToolWindow.CurrentTilesetList == 1) {
                            for(int i = Hold.X; i < Hold.Right; i++) {
                                for(int j = Hold.Y; j < Hold.Bottom; j++) {
                                    if(ToolWindow.CurrentBGTileset == 0) l.BackgroundTiles.SetTile(i, j, 48);
                                    else l.BackgroundTiles.SetTile(i, j, Engine.Scene.BGTilesets[ToolWindow.CurrentBGTileset - 1].ID);
                                }
                            }

                            return RerenderType.Background;
                        }
                    } else {
                        ToolHint = new Rectangle(l.TilePointer.X * 8, l.TilePointer.Y * 8, 8, 8);
                    }*/
                    break;
            }

            return RerenderType.None;
        }
    }

    public enum RerenderType {
        Foreground,
        Background,
        None
    }

    public enum ToolType {
        Point,
        Rectangle
    }
}
