using Starforge.Editor.Render;
using Starforge.Map;

namespace Starforge.Editor.Actions {
    public abstract class EditorAction {
        /// <summary>
        /// The room the action was applied to.
        /// </summary>
        protected Room Room;

        /// <summary>
        /// The drawable room the action was applied to.
        /// </summary>
        protected DrawableRoom DrawableRoom;

        public EditorAction(Room room) {
            Room = room;
            DrawableRoom = MapEditor.Instance.Renderer.GetRoom(room);
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
