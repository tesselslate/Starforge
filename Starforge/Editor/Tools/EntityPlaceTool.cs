using Microsoft.Xna.Framework;
using Starforge.Core;
using Starforge.Core.Input;
using Starforge.Editor.Actions;
using Starforge.Editor.UI;
using Starforge.MapStructure;
using Starforge.Mod;
using System;

namespace Starforge.Editor.Tools {
    class EntityPlaceTool : Tool {

        private Vector2 HoldStart;
        private Rectangle Hold;

        private Entity heldEntity = null;

        public override void ManageInput(MouseEvent m) {
            Level l = Engine.Scene.SelectedLevel;

            if (heldEntity == null || heldEntity.Name != ToolWindow.Entities[ToolWindow.CurrentEntity]) {
                heldEntity = EntityRegistry.Create(
                    Engine.Scene.SelectedLevel,
                    ToolWindow.Entities[ToolWindow.CurrentEntity],
                    new Rectangle() { X = l.TilePointer.X * 8, Y = l.TilePointer.Y * 8, Width = 0, Height = 0 });
                if (heldEntity.StretchableX) {
                    heldEntity.SetWidth(8);
                }
                if (heldEntity.StretchableY) {
                    heldEntity.SetHeight(8);
                }
            }

            heldEntity.Level = l;

            if (m.LeftButtonClick) {
                OnLeftClick();
            }
            else if (m.LeftButtonDrag) {
                OnLeftDrag();
            }
            else if (m.LeftButtonUnclick) {
                OnLeftUnclick();
            }
            else if (m.MouseMoved) { 
                OnMouseMove();
            }
        }

        private void OnLeftClick() {
            Level l = Engine.Scene.SelectedLevel;
            HoldStart = new Vector2(l.TilePointer.X, l.TilePointer.Y);
        }

        private void OnLeftDrag() {
            Level l = Engine.Scene.SelectedLevel;
            if (heldEntity.StretchableX) {
                Hold.X = (int)MathHelper.Min(HoldStart.X, l.TilePointer.X) * 8;
                Hold.Width = ((int)Math.Abs(HoldStart.X - l.TilePointer.X) + 1) * 8;
            }
            else {
                Hold.X = (int)HoldStart.X * 8;
                Hold.Width = 0;
            }
            if (heldEntity.StretchableY) {
                Hold.Y = (int)MathHelper.Min(HoldStart.Y, l.TilePointer.Y) * 8;
                Hold.Height = ((int)Math.Abs(HoldStart.Y - l.TilePointer.Y) + 1) * 8;
            }
            else {
                Hold.Y = (int)HoldStart.Y * 8;
                Hold.Height = 0;
            }
            heldEntity.SetArea(Hold);
        }

        private void OnLeftUnclick() {
            Level l = Engine.Scene.SelectedLevel;
            l.ApplyNewAction(new EntityPlacement(l, ToolWindow.Entities[ToolWindow.CurrentEntity], Hold));
            Hold = new Rectangle(l.TilePointer.X * 8, l.TilePointer.Y * 8, 8, 8);
            heldEntity.SetArea(Hold);
        }

        private void OnMouseMove() {
            Level l = Engine.Scene.SelectedLevel;
            Hold = new Rectangle(l.TilePointer.X * 8, l.TilePointer.Y * 8, 8, 8);
            heldEntity.SetArea(Hold);
        }

        public override void Render() {
            if (heldEntity != null) {
                heldEntity.Render();
            }
        }

        public override string getName() {
            return "Entity Placing";
        }

    }
}
