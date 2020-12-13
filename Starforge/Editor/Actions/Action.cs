using Starforge.MapStructure;

namespace Starforge.Editor.Actions {

    // representing a user action, like placing/removing an entity or a tile
    abstract public class Action {

        protected Level Level;

        public Action(Level l) {
            Level = l;
        }

        public abstract bool Apply();

        public abstract bool Undo();
    }

}
