using Starforge.Map;
using Starforge.Editor.Actions;
using System.Collections.Generic;

namespace Starforge.Vanilla.Actions {
    public class EntityRemovalAction : EditorAction {
        private List<Entity> Entities;

        public EntityRemovalAction(Room r, IEnumerable<Entity> entities) : base(r) {
            Entities = new List<Entity>(entities);
        }

        public override bool Apply() {
            foreach (var e in Entities) {
                Room.Entities.Remove(e);
            }
            DrawableRoom.Dirty = true;
            return true;
        }

        public override bool Undo() {
            Room.Entities.AddRange(Entities);
            DrawableRoom.Dirty = true;
            return true;
        }
    }
}
