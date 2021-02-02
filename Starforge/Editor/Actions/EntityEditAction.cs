using Starforge.Map;
using System;

namespace Starforge.Editor.Actions {
    public class EntityEditAction : EditorAction {

        Entity Entity;

        public EntityEditAction(Room r, Entity e) : base(r) {
            Entity = e;
        }

        public override bool Apply() {
            throw new NotImplementedException();
        }

        public override bool Undo() {
            throw new NotImplementedException();
        }
    }
}
