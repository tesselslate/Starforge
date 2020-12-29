using Starforge.Map;

namespace Starforge.Editor.Actions {
    public abstract class EditorAction {
        /// <summary>
        /// The room the action was applied to.
        /// </summary>
        protected Room Room;

        public EditorAction(Room room) {
            Room = room;
        }

        /// <summary>
        /// Applies the action.
        /// </summary>
        /// <returns>Whether or not any changes were made.</returns>
        public abstract bool Apply();

        /// <summary>
        /// Undoes the action.
        /// </summary>
        /// <returns>Whether or not any changes were made.</returns>
        public abstract bool Undo();
    }
}
