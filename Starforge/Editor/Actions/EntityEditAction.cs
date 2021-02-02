using Starforge.Map;
using System.Collections.Generic;

namespace Starforge.Editor.Actions {
    using Attributes = Dictionary<string, object>;

    public class EntityEditAction : EditorAction {

        private Entity Entity;
        private Attributes PreEdit;
        private Attributes PostEdit;

        public EntityEditAction(Room r, Entity e, Attributes PreEdit, Attributes PostEdit) : base(r) {
            Entity = e;
            this.PreEdit = PreEdit;
            this.PostEdit = PostEdit;
        }

        public override bool Apply() {
            if (Entity == null) {
                return false;
            }

            foreach (KeyValuePair<string, object> pair in PostEdit) {
                Entity.Attributes[pair.Key] = PostEdit[pair.Key];
            }

            this.DrawableRoom.Dirty = true;
            return true;
        }

        public override bool Undo() {
            if (Entity == null) {
                return false;
            }

            foreach (KeyValuePair<string, object> pair in PreEdit) {
                Entity.Attributes[pair.Key] = PreEdit[pair.Key];
            }

            this.DrawableRoom.Dirty = true;
            return true;
        }
    }
}
