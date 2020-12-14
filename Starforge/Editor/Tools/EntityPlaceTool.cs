using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Starforge.Core;
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
                heldEntity = makeEntityAtPosition(ToolWindow.Entities[ToolWindow.CurrentEntity], l.TilePointer);
            }

            heldEntity.Position = new Vector2(l.TilePointer.X * 8f, l.TilePointer.Y * 8f);

            if (m.LeftButton != ButtonState.Pressed) {
                return;
            }
            if (Engine.Scene.PreviousMouseState.LeftButton == ButtonState.Pressed) {
                return;
            }

            // if just clicked
            Entity newEntity = makeEntityAtPosition(ToolWindow.Entities[ToolWindow.CurrentEntity], l.TilePointer);

            l.Entities.Add(newEntity);
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

        private Entity makeEntityAtPosition(string name, Point p) {
            return EntityRegistry.Create(
                Engine.Scene.SelectedLevel,
                name,
                new Vector2(p.X * 8f, p.Y * 8f));
        }

    }
}
