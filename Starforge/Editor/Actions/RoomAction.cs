using Starforge.Map;

namespace Starforge.Editor.Actions {
    public class RoomAdditionAction : EditorAction {
        public RoomAdditionAction(Room room) : base(room) { }

        public override bool Apply() {
            int w = Room.Meta.Bounds.Width / 8;
            int h = Room.Meta.Bounds.Height / 8;

            Room.BackgroundTiles = new TileGrid(w, h) { DefaultValue = TileGrid.TILE_AIR };
            Room.BackgroundTiles.Fill(TileGrid.TILE_AIR);

            Room.ForegroundTiles = new TileGrid(w, h) { DefaultValue = TileGrid.TILE_AIR };
            Room.ForegroundTiles.Fill(TileGrid.TILE_AIR);

            Room.ObjectTiles = new TileGrid(w, h) { DefaultValue = TileGrid.OBJ_AIR };
            Room.ObjectTiles.Fill(TileGrid.OBJ_AIR);

            MapEditor.Instance.State.AddRoom(Room);
            return true;
        }

        public override bool Undo() {
            MapEditor.Instance.State.RemoveRoom(Room);
            return true;
        }
    }

    public class RoomRemovalAction : EditorAction {
        public RoomRemovalAction(Room room) : base(room) { }

        public override bool Apply() {
            MapEditor.Instance.State.RemoveRoom(Room);
            return true;
        }

        public override bool Undo() {
            MapEditor.Instance.State.AddRoom(Room);
            return true;
        }
    }

    public class RoomModificationAction : EditorAction {
        private MapElement OldRoom;
        private RoomMeta NewMeta;

        public RoomModificationAction(Room room, RoomMeta newMeta) : base(room) {
            OldRoom = room.Encode();
            NewMeta = newMeta;
        }

        public override bool Apply() {
            int w = NewMeta.Bounds.Width / 8;
            int h = NewMeta.Bounds.Height / 8;

            Room.Meta = NewMeta;
            Room.BackgroundTiles.Resize(w, h);
            Room.ForegroundTiles.Resize(w, h);
            Room.ObjectTiles.Resize(w, h);

            MapEditor.Instance.State.UpdateRoom(Room);
            return true;
        }

        public override bool Undo() {
            Room oldRoom = Room.Decode(OldRoom, MapEditor.Instance.State.LoadedLevel);
            Room.Meta = oldRoom.Meta;

            int w = oldRoom.Meta.Bounds.Width / 8;
            int h = oldRoom.Meta.Bounds.Height / 8;
            Room.BackgroundTiles.Resize(w, h);
            Room.ForegroundTiles.Resize(w, h);
            Room.ObjectTiles.Resize(w, h);

            MapEditor.Instance.State.UpdateRoom(Room);
            return true;
        }
    }
}
