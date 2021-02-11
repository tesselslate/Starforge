using Starforge.Map;
using Starforge.Editor.Actions;
using System.Collections.Generic;

namespace Starforge.Vanilla.Actions {
    using Attributes = Dictionary<string, object>;

    public class BulkEntityEditAction : EditorAction {

        private List<Entity> Entities;
        private List<Attributes> PreEdit;
        private List<Attributes> PostEdit;

        public BulkEntityEditAction(Room r, List<Entity> e, List<Attributes> preEdit, List<Attributes> postEdit) : base(r) {
            Entities = new List<Entity>(e);
            PreEdit = preEdit;
            PostEdit = postEdit;
        }

        public override bool Apply() {
            if (Entities == null) {
                return false;
            }
            for (int i = 0; i < Entities.Count; i++) {
                Entity entity = Entities[i];
                foreach (KeyValuePair<string, object> pair in PostEdit[i]) {
                    entity.Attributes[pair.Key] = pair.Value;
                }
            }

            DrawableRoom.Dirty = true;
            return true;
        }

        public override bool Undo() {
            if (Entities == null) {
                return false;
            }

            for (int i = 0; i < Entities.Count; i++) {
                Entity entity = Entities[i];
                foreach (KeyValuePair<string, object> pair in PreEdit[i]) {
                    entity.Attributes[pair.Key] = pair.Value;
                }
            }

            DrawableRoom.Dirty = true;
            return true;
        }
    }
}
