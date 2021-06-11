using Microsoft.Xna.Framework;
using Starforge.Core;
using Starforge.Map;
using Starforge.Editor.Actions;
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
        /// The position of the cursor, in pixels, in the selected room.
        /// </summary>
        public Point PixelPointer;

        /// <summary>
        /// A list of previously applied actions which can be undone.
        /// </summary>
        public Stack<EditorAction> PastActions;

        /// <summary>
        /// A list of previously undone actions which can be reapplied.
        /// </summary>
        public Stack<EditorAction> FutureActions;

        /// <returns>Whether or not the Editor is in "Pixel Perfect" mode, meaning placements are not limited to the tile grid</returns>
        public static bool PixelPerfect() => Input.Keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl) || Input.Keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightControl);

        /// <returns>Whether or not there are changes that can be redone.</returns>
        public bool CanRedo() => FutureActions.Count > 0;

        /// <returns>Whether or not there are changes that can be undone.</returns>
        public bool CanUndo() => PastActions.Count > 0;

        /// <summary>
        /// Adds a room to the level.
        /// </summary>
        /// <param name="room">The room to add.</param>
        public void AddRoom(Room room) {
            if (LoadedLevel.Rooms.Contains(room)) return;

            LoadedLevel.Rooms.Add(room);
            MapEditor.Instance.Renderer.AddRoom(room);
            MapEditor.Instance.RoomListWindow.RoomNames = GetRoomNameList();

            MapEditor.Instance.SelectRoom(MapEditor.Instance.Renderer.GetRoom(room), true);
        }

        /// <summary>
        /// Updates a room in the level.
        /// </summary>
        /// <param name="room">The room to update.</param>
        public void UpdateRoom(Room room) {
            MapEditor.Instance.Renderer.UpdateRoom(room);
            MapEditor.Instance.RoomListWindow.RoomNames = GetRoomNameList();

            if (room == SelectedRoom) MapEditor.Instance.SelectRoom(MapEditor.Instance.Renderer.GetRoom(room));
        }

        /// <summary>
        /// Removes a room from the level.
        /// </summary>
        /// <param name="room">The room to remove.</param>
        public void RemoveRoom(Room room) {
            if (!LoadedLevel.Rooms.Contains(room)) return;

            MapEditor.Instance.Renderer.RemoveRoom(room);
            LoadedLevel.Rooms.Remove(room);
            MapEditor.Instance.RoomListWindow.RoomNames = GetRoomNameList();

            if (SelectedRoom == room) {
                SelectedRoom = null;
                MapEditor.Instance.Renderer.Overlay.Dispose();
            }
        }

        /// <returns>An array of all the room names.</returns>
        public string[] GetRoomNameList() {
            List<string> roomNames = new List<string>();
            foreach (Room room in LoadedLevel.Rooms) roomNames.Add(room.Name);
            return roomNames.ToArray();
        }

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
            if (CanUndo()) {
                EditorAction action = PastActions.Pop();
                Unsaved = true;

                action.Undo();
                FutureActions.Push(action);
            }
        }

        /// <summary>
        /// Applies the previously undone action, if one is available.
        /// </summary>
        public void Redo() {
            if (CanRedo()) {
                EditorAction action = FutureActions.Pop();
                action.Apply();
                Unsaved = true;

                PastActions.Push(action);
            }
        }

        /// <summary>
        /// Saves the currently loaded map.
        /// </summary>
        public void Save() {
            if (!File.Exists(LoadedPath)) {
                using (FileStream stream = File.Create(LoadedPath)) {
                    stream.Close();
                }
            }

            // first write the binary into memory. This way, in case of a crash, the map binary on disc doesn't get corrupted.
            using MemoryStream memStream = new MemoryStream();
            using BinaryWriter writer = new BinaryWriter(memStream);
            MapPacker.WriteMapBinary(writer, LoadedLevel.Encode());

            // map written successfully, now save it
            File.WriteAllBytes(LoadedPath, memStream.ToArray());

            Unsaved = false;
        }
    }
}
