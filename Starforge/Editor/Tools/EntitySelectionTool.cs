using Starforge.Core;
using Starforge.Map;
using Starforge.Editor.UI;

namespace Starforge.Editor.Tools
{
    public class EntitySelectionTool : Tool
    {
        public override string GetName() => "Entity Selection";

        public override void Render() {
            //nothing to render
        }

        public override void Update() {
            Room r = MapEditor.Instance.State.SelectedRoom;

            if (Input.Mouse.LeftUnclick) {
                Entity SelectedEntity = r.Entities.Find(e => e.ContainsPosition(MapEditor.Instance.State.PixelPointer));
                if (SelectedEntity != null) {
                    HandleSelectedEntity(SelectedEntity);
                }
            }
            
        }

        public void HandleSelectedEntity(Entity SelectedEntity) {
            Engine.CreateWindow(new WindowEntityEdit(SelectedEntity));
        }

    }
}
