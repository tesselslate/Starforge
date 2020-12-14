using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Starforge.Core;
using Starforge.Editor.Actions;
using Starforge.Editor.UI;
using Starforge.MapStructure;
using Starforge.Mod;

namespace Starforge.Editor.Tools {
    class EntityPlaceTool : Tool {

        private Entity heldEntity = null;

        public EntityPlaceTool() {
            Level l = Engine.Scene.SelectedLevel;
        }

        public override void ManageInput(MouseState m) {
            Level l = Engine.Scene.SelectedLevel;

            if (heldEntity == null || heldEntity.Name != ToolWindow.Entities[ToolWindow.CurrentEntity]) {
                heldEntity = EntityRegistry.Create(
                    Engine.Scene.SelectedLevel,
                    ToolWindow.Entities[ToolWindow.CurrentEntity],
                    new Vector2(l.TilePointer.X * 8f, l.TilePointer.Y * 8f));
            }

            heldEntity.Position = new Vector2(l.TilePointer.X * 8f, l.TilePointer.Y * 8f);

            if (m.LeftButton != ButtonState.Pressed) {
                return;
            }
            if (Engine.Scene.PreviousMouseState.LeftButton == ButtonState.Pressed) {
                return;
            }

            // if just clicked
            l.ApplyNewAction(new EntityPlacement(l, ToolWindow.Entities[ToolWindow.CurrentEntity], l.TilePointer));

            l.Dirty = true;
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
