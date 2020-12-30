using Microsoft.Xna.Framework;
using Starforge.Editor.Actions;
using Starforge.Map;
using System.Collections.Generic;
using System.IO;

namespace Starforge.Editor {
    /// <summary>
    /// Contains information about the current state of the editor.
    /// </summary>
    public class EditorState {
        /// <summary>
        /// The currently loaded level.
        /// </summary>
        public Level LoadedLevel;

        /// <summary>
        /// The path, on disk, of the currently loaded level.
        /// </summary>
        public string LoadedPath;

        /// <summary>
        /// Whether or not the level has unsaved changes.
        /// </summary>
        public bool Unsaved;

        /// <summary>
        /// The currently selected room.
        /// </summary>
        public Room SelectedRoom;
        
        /// <summary>
        /// The position of the cursor, in tiles, in the selected room.
        /// </summary>
        public Point TilePointer;

        /// <summary>
        /// A list of previously applied actions which can be undone.
        /// </summary>
        public Stack<EditorAction> PastActions;

        /// <summary>
        /// A list of previously undone actions which can be reapplied.
        /// </summary>
        public Stack<EditorAction> FutureActions;

        /// <returns>Whether or not there are changes that can be redone.</returns>
        public bool CanRedo() => FutureActions.Count > 0;

        /// <returns>Whether or not there are changes that can be undone.</returns>
        public bool CanUndo() => PastActions.Count > 0;

        /// <summary>
        /// Applies a new action to the level.
        /// </summary>
        /// <param name="action">The action to apply.</param>
        public void Apply(EditorAction action) {
            Unsaved = true;

            action.Apply();
            PastActions.Push(action);
            FutureActions.Clear();
        }

        /// <summary>
        /// Undoes the last action, if one is available.
        /// </summary>
        public void Undo() {
            if (PastActions.Count > 0) {
                EditorAction action = PastActions.Pop();
                Unsaved = true;

                FutureActions.Push(action);
            }
        }

        /// <summary>
        /// Applies the previously undone action, if one is available.
        /// </summary>
        public void Redo() {
            if (FutureActions.Count > 0) {
                EditorAction action = FutureActions.Pop();
                Unsaved = true;

                PastActions.Push(action);
            }
        }

        /// <summary>
        /// Saves the currently loaded map.
        /// </summary>
        public void Save() {
            using (FileStream stream = new FileStream(LoadedPath, FileMode.Truncate)) {
                using (BinaryWriter writer = new BinaryWriter(stream)) {
                    MapPacker.WriteMapBinary(writer, LoadedLevel.Encode());
                    Unsaved = false;
                }
            }
        }
    }
}
