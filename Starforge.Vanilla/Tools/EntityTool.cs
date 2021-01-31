using ImGuiNET;
using Microsoft.Xna.Framework;
using Starforge.Core;
using Starforge.Editor;
using Starforge.Editor.Actions;
using Starforge.Editor.Tools;
using Starforge.Map;
using Starforge.Mod;
using Starforge.Mod.API;
using Starforge.Util;
using System;

namespace Starforge.Vanilla.Tools {
    [ToolDefinition("Entity")]
    public class EntityTool : Tool {
        /// <summary>
        /// The currently selected entity placement.
        /// </summary>
        public static Placement SelectedEntity;

        public Entity HeldEntity;

        private Point Start;

        /// <remarks>The hold is set out of bounds (beyond upleft corner) so the entity does not appear when first selecting the tool.</remarks>
        private Rectangle Hold = new Rectangle(-64, -64, 0, 0);

        public override string GetName() => "Entities";
        public override bool CanSelectLayer() => false;
        public override void Render() {
            if (HeldEntity != null) HeldEntity.Render();
        }

        public override void Update() {
            Room r = MapEditor.Instance.State.SelectedRoom;

            if (SelectedEntity == null)
                SelectedEntity = EntityRegistry.EntityPlacements[0];

            if (HeldEntity != null) {
                HeldEntity.Room = r;

                if (Input.Mouse.LeftClick) HandleClick();
                else if (Input.Mouse.LeftHold) HandleDrag();
                else if (Input.Mouse.LeftUnclick) HandleUnclick();
                else HandleMove();
            } else {
                HeldEntity = SelectedEntity.Create(r);
                HeldEntity.SetArea(Hold);
            }
        }

        public void UpdateHeldEntity() {
            HeldEntity = SelectedEntity.Create(MapEditor.Instance.State.SelectedRoom);
            HeldEntity.SetArea(Hold);
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

            Entity entity = SelectedEntity.Create(MapEditor.Instance.State.SelectedRoom);
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

        public override void RenderGUI() {
            ImGui.Text("Entities");
            ImGui.SetNextItemWidth(235f);

            if (SelectedEntity == null)
                SelectedEntity = EntityRegistry.EntityPlacements[0];

            if (ImGui.ListBoxHeader("EntitiesList", EntityRegistry.EntityPlacements.Count, MapEditor.Instance.ToolListWindow.VisibleItemsCount)) {
                foreach (Placement placement in EntityRegistry.EntityPlacements) {
                    if (ImGui.Selectable(placement.Name, placement == SelectedEntity)) {
                        SelectedEntity = placement;
                        UpdateHeldEntity();
                    }
                }
            }
        }
    }
}
