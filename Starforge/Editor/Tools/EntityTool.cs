using Microsoft.Xna.Framework;
using Starforge.Core;
using Starforge.Editor.Actions;
using Starforge.Map;
using Starforge.Util;
using System;

namespace Starforge.Editor.Tools {
    public class EntityTool : Tool {
        public Entity HeldEntity;

        private Point Start;
        private Rectangle Hold;

        public override string GetName() => "Entities";

        public override void Render() {
            if (HeldEntity != null) HeldEntity.Render();
        }

        public override void Update() {
            Room r = MapEditor.Instance.State.SelectedRoom;

            if (HeldEntity != null) {
                HeldEntity.Parent = r;

                if (Input.Mouse.LeftClick) HandleClick();
                else if (Input.Mouse.LeftHold) HandleDrag();
                else if (Input.Mouse.LeftUnclick) HandleUnclick();
                else if (Input.Mouse.Moved) HandleMove();
            } else {
                HeldEntity = ToolManager.SelectedEntity.Create(r);
                HeldEntity.SetArea(Hold);
            }
        }

        public void UpdateHeldEntity() {
            HeldEntity = ToolManager.SelectedEntity.Create(MapEditor.Instance.State.SelectedRoom);
        }

        private void HandleClick() {
            Start = MapEditor.Instance.State.TilePointer;
        }

        private void HandleDrag() {
            if (HeldEntity.StretchableX) {
                Hold.X = (int)MathHelper.Min(Start.X, MapEditor.Instance.State.TilePointer.X) * 8;
                Hold.Width = (Math.Abs(Start.X - MapEditor.Instance.State.TilePointer.X) + 1) * 8;
            } else {
                Hold.X = Start.X * 8;
                Hold.Width = 0;
            }

            if (HeldEntity.StretchableY) {
                Hold.Y = (int)MathHelper.Min(Start.Y, MapEditor.Instance.State.TilePointer.Y) * 8;
                Hold.Height = (Math.Abs(Start.Y - MapEditor.Instance.State.TilePointer.Y) + 1) * 8;
            } else {
                Hold.Y = Start.Y * 8;
                Hold.Height = 0;
            }

            HeldEntity.SetArea(Hold);
        }

        private void HandleUnclick() {
            HeldEntity.SetArea(Hold);

            Entity entity = ToolManager.SelectedEntity.Create(MapEditor.Instance.State.SelectedRoom);
            if (HeldEntity.StretchableX || HeldEntity.StretchableY) entity.SetArea(Hold);
            else entity.Position = HeldEntity.Position;

            MapEditor.Instance.State.Apply(new EntityPlacementAction(
                MapEditor.Instance.State.SelectedRoom,
                entity
            ));

            Hold = new Rectangle(MapEditor.Instance.State.TilePointer.X * 8, MapEditor.Instance.State.TilePointer.Y * 8, 8, 8);
            HeldEntity.SetArea(Hold);
        }

        private void HandleMove() {
            Hold = new Rectangle(MapEditor.Instance.State.TilePointer.X * 8, MapEditor.Instance.State.TilePointer.Y * 8, 8, 8);
            HeldEntity.SetArea(Hold);
        }
    }
}
