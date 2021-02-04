using Starforge.Map;
using Starforge.Editor.Actions;

namespace Starforge.Vanilla.Actions {
    public class EntityRemovalAction : EditorAction {
        private Entity Entity;

        public EntityRemovalAction(Room r, Entity e) : base(r) {
            Entity = e;
        }

        public override bool Apply() {
            Room.Entities.Remove(Entity);
            DrawableRoom.Dirty = true;
            return true;
        }

        public override bool Undo() {
            Room.Entities.Add(Entity);
            DrawableRoom.Dirty = true;
            return true;
        }
    }
}
