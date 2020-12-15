using Microsoft.Xna.Framework;
using Starforge.Core;
using Starforge.Core.Input;
using Starforge.Editor.Actions;
using Starforge.Editor.UI;
using Starforge.MapStructure;
using Starforge.Mod;

namespace Starforge.Editor.Tools {
    class EntityPlaceTool : Tool {

        private Entity heldEntity = null;

        public override void ManageInput(MouseEvent m) {
            Level l = Engine.Scene.SelectedLevel;

            if (heldEntity == null || heldEntity.Name != ToolWindow.Entities[ToolWindow.CurrentEntity]) {
                heldEntity = EntityRegistry.Create(
                    Engine.Scene.SelectedLevel,
                    ToolWindow.Entities[ToolWindow.CurrentEntity],
                    new Vector2(l.TilePointer.X * 8f, l.TilePointer.Y * 8f));
            }

            heldEntity.Level = l;
            heldEntity.Position = new Vector2(l.TilePointer.X * 8f, l.TilePointer.Y * 8f);

            if (m.LeftButtonClick) {
                OnLeftClick();
            }
        }

        private void OnLeftClick() {
            Level l = Engine.Scene.SelectedLevel;
            l.ApplyNewAction(new EntityPlacement(l, ToolWindow.Entities[ToolWindow.CurrentEntity], l.TilePointer));
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
