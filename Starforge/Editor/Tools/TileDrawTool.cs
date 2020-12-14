using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Starforge.Core;
using Starforge.Editor.Actions;
using Starforge.Editor.UI;
using Starforge.MapStructure;

namespace Starforge.Editor.Tools {
    public class TileDrawTool : Tool {

        private Rectangle ToolHint = new Rectangle(0, 0, 8, 8);

        private static DrawTilePlacement CurrentDrawAction = null;

        public override void ManageInput(MouseState m, Level l) {
            ToolHint.X = l.TilePointer.X * 8;
            ToolHint.Y = l.TilePointer.Y * 8;

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

        public override void Render(RenderTarget2D target) {
            Engine.Instance.GraphicsDevice.SetRenderTarget(target);
            Engine.Instance.GraphicsDevice.Clear(Color.Transparent);

            Engine.Batch.Begin(SpriteSortMode.Deferred,
                               BlendState.AlphaBlend,
                               SamplerState.PointClamp, null, RasterizerState.CullNone, null);

            GFX.Pixel.Draw(ToolHint, Engine.Config.ToolAccentColor, 0.25f);
            GFX.Draw.HollowRectangle(ToolHint, Color.Goldenrod);
            Engine.Batch.End();
        }
    }
}
