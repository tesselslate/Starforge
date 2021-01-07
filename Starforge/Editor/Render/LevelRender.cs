using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Starforge.Core;
using Starforge.Editor.UI;
using Starforge.Map;
using Starforge.Mod.Content;
using Starforge.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Starforge.Editor.Render {
    public class LevelRender : IDisposable {
        /// <summary>
        /// The RenderTargetUsage to use when drawing rooms.
        /// </summary>
        private RenderTargetUsage TargetUsage;

        /// <summary>
        /// The list of RenderTargets for each room in the map.
        /// </summary>
        public List<DrawableRoom> Rooms;

        /// <summary>
        /// The list of rooms which are visible onscreen.
        /// </summary>
        public List<DrawableRoom> VisibleRooms;

        /// <summary>
        /// The currently loaded level.
        /// </summary>
        public Level Level;

        /// <summary>
        /// The parent map editor.
        /// </summary>
        public MapEditor Editor;

        // Selected room
        public DrawableRoom SelectedRoom { get; private set; }
        public RenderTarget2D Overlay { get; private set; }

        /// <summary>
        /// Creates a new LevelRender instance for rendering the level.
        /// </summary>
        /// <param name="level">The level to render.</param>
        public LevelRender(MapEditor editor, Level level) {
            // Create room render targets
            TargetUsage = Settings.AlwaysRerender ? RenderTargetUsage.DiscardContents : RenderTargetUsage.PreserveContents;
            Rooms = new List<DrawableRoom>();
            Editor = editor;
            Level = level;

            foreach (Room room in Level.Rooms) {
                Rooms.Add(new DrawableRoom(room, TargetUsage));
            }

            Editor.Camera.OnPositionChange += () => {
                VisibleRooms = new List<DrawableRoom>();
                foreach (DrawableRoom room in Rooms) {
                    if (Editor.Camera.VisibleArea.Intersects(room.Room.Meta.Bounds)) {
                        VisibleRooms.Add(room);
                    }
                }
            };
        }

        public void AddRoom(Room room) {
            Rooms.Add(new DrawableRoom(room, TargetUsage));
            Editor.Camera.Update();
        }

        public void UpdateRoom(Room room) {
            DrawableRoom dr = GetRoom(room);
            int index = Rooms.IndexOf(dr);

            dr.Target.Dispose();
            Rooms[index] = new DrawableRoom(room, TargetUsage);
            Editor.Camera.Update();
        }

        public void RemoveRoom(Room room) {
            DrawableRoom dr;
            if ((dr = GetRoom(room)) != null) {
                Rooms.Remove(dr);
                dr.Target.Dispose();
            }
            Editor.Camera.Update();
        }

        /// <summary>
        /// Disposes of the LevelRender.
        /// </summary>
        public void Dispose() {
            foreach (DrawableRoom room in Rooms) {
                room.Target.Dispose();
            }
        }

        /// <summary>
        /// Gets a DrawableRoom based on a Room.
        /// </summary>
        /// <param name="room">The room to search for.</param>
        /// <returns>The DrawableRoom of the given room.</returns>
        public DrawableRoom GetRoom(Room room) {
            return Rooms.Where(d => d.Room == room).FirstOrDefault();
        }

        /// <summary>
        /// Renders the entire level.
        /// </summary>
        public void Render() {
            // If AlwaysRerender is on, always rerender rooms.
            if (TargetUsage == RenderTargetUsage.DiscardContents) {
                foreach (DrawableRoom room in VisibleRooms) {
                    RenderRoom(room);
                }
            }

            foreach (DrawableRoom room in VisibleRooms) {
                if (room.Dirty) RenderRoom(room, Menubar.RerenderFlags);
            }

            Engine.Instance.GraphicsDevice.SetRenderTarget(null);
            Engine.Instance.GraphicsDevice.Clear(Settings.BackgroundColor);

            Engine.Batch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                DepthStencilState.None,
                null,
                null,
                Editor.Camera.Transform
            );

            // Render rooms
            foreach (DrawableRoom room in VisibleRooms) {
                if (!room.Target.IsDisposed) Engine.Batch.Draw(room.Target, new Vector2(room.Room.X, room.Room.Y), Color.White);
            }

            // Render selected room
            if (SelectedRoom != null && !Overlay.IsDisposed) {
                Engine.Batch.Draw(Overlay, new Vector2(SelectedRoom.Room.X, SelectedRoom.Room.Y), Color.White * 0.75f);
            }

            // Render fillers
            foreach (Rectangle r in Level.Fillers) {
                GFX.Draw.Rectangle(r, Color.DimGray);
            }

            Engine.Batch.End();
        }

        /// <summary>
        /// Renders a room.
        /// </summary>
        /// <param name="room">The room to render.</param>
        /// <param name="flags">The render flags.</param>
        public void RenderRoom(DrawableRoom room, RenderFlags flags = RenderFlags.All) {
            // Generate decals
            room.BGDecals = CreateDecalTextureList(room.Room.BackgroundDecals);
            room.FGDecals = CreateDecalTextureList(room.Room.ForegroundDecals);

            // Generate tiles
            if (room.TilesDirty) {
                room.BGTiles = Editor.BGAutotiler.GenerateTextureMap(room.Room.BackgroundTiles, true);
                room.FGTiles = Editor.FGAutotiler.GenerateTextureMap(room.Room.ForegroundTiles, true);

                for (int i = 0; i < room.Room.ObjectTiles.Map.Length; i++) {
                    if (room.Room.ObjectTiles.Map[i] != TileGrid.OBJ_AIR) {
                        room.OBTiles.Add(
                            new StaticTexture(GFX.Scenery[room.Room.ObjectTiles.Map[i]])
                            {
                                Position = new Vector2(i % room.Room.ObjectTiles.Width * 8, i / room.Room.ObjectTiles.Width * 8)
                            }
                        );
                    }
                }

                room.TilesDirty = false;
            }

            Engine.Instance.GraphicsDevice.SetRenderTarget(room.Target);
            if (SelectedRoom == room) Engine.Instance.GraphicsDevice.Clear(Settings.SelectedRoomColor);
            else Engine.Instance.GraphicsDevice.Clear(Settings.RoomColor);

            Engine.Batch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                DepthStencilState.None,
                RasterizerState.CullNone,
                null
            );

            // Background tiles
            if (flags.HasFlag(RenderFlags.BGTiles)) room.BGTiles.Draw();

            // Background decals
            if (flags.HasFlag(RenderFlags.BGDecals)) foreach (StaticTexture t in room.BGDecals) t.DrawCentered();

            // Object tiles
            if (flags.HasFlag(RenderFlags.OBTiles)) foreach (StaticTexture t in room.OBTiles) t.Draw();

            // Entities
            if (flags.HasFlag(RenderFlags.Entities)) foreach (Entity e in room.Room.Entities) e.Render();

            // Foreground tiles
            if (flags.HasFlag(RenderFlags.FGTiles)) room.FGTiles.Draw();

            // Foreground decals
            if (flags.HasFlag(RenderFlags.FGDecals)) foreach (StaticTexture t in room.FGDecals) t.DrawCentered();

            // Triggers
            if (flags.HasFlag(RenderFlags.Triggers)) {
                foreach (Entity t in room.Room.Triggers) {
                    Rectangle tr = new Rectangle((int)t.Position.X, (int)t.Position.Y, (int)t.Attributes["width"], (int)t.Attributes["height"]);
                    GFX.Draw.BorderedRectangle(tr, Settings.TriggerColor * 0.4f, Settings.TriggerColor);
                    GFX.Draw.TextCentered(MiscHelper.CleanCamelCase(t.Name), tr, Color.White);
                }
            }

            Engine.Batch.End();

            room.Dirty = false;
        }
        

        /// <summary>
        /// Sets the currently selected room.
        /// </summary>
        public void SetSelected(Room room) {
            DrawableRoom old = null;
            if (SelectedRoom != null) {
                Overlay.Dispose();
                old = SelectedRoom;
            }

            SelectedRoom = GetRoom(room);
            SelectedRoom.Dirty = true;
            if (old != null) old.Dirty = true;

            Overlay = new RenderTarget2D(Engine.Instance.GraphicsDevice, room.Width, room.Height, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
        }

        /// <summary>
        /// Creates a list of StaticTextures based on a list of Decals.
        /// </summary>
        /// <param name="decals">The Decal list to us.</param>
        private List<StaticTexture> CreateDecalTextureList(List<Decal> decals) {
            List<StaticTexture> list = new List<StaticTexture>();
            foreach (Decal decal in decals) {
                DrawableTexture tex = GFX.Gameplay["decals/" + decal.Name.Replace('\\', '/').Substring(0, decal.Name.Length - 4)];
                list.Add(new StaticTexture(tex, new Vector2(decal.X, decal.Y), decal.Scale));
            }

            return list;
        }
    }

    /// <summary>
    /// Contains the data for drawing a given room.
    /// </summary>
    public class DrawableRoom {
        public RenderTarget2D Target;
        public Room Room;

        public List<StaticTexture> BGDecals;
        public List<StaticTexture> FGDecals;

        public TextureMap BGTiles;
        public TextureMap FGTiles;
        public List<StaticTexture> OBTiles;

        public bool Dirty = true;
        public bool TilesDirty = true;

        public DrawableRoom(Room room, RenderTargetUsage targetUsage) {
            Room = room;

            Target = new RenderTarget2D(Engine.Instance.GraphicsDevice, room.Width, room.Height, false, SurfaceFormat.Color, DepthFormat.None, 0, targetUsage);
            BGTiles = new TextureMap(room.Width / 8, room.Height / 8);
            FGTiles = new TextureMap(room.Width / 8, room.Height / 8);
            OBTiles = new List<StaticTexture>();
        }
    }

    public enum RenderFlags {
        None = 0,
        BGDecals = 1 << 0,
        BGTiles = 1 << 1,
        FGDecals = 1 << 2,
        FGTiles = 1 << 3,
        Entities = 1 << 4,
        Triggers = 1 << 5,
        OBTiles = 1 << 6,
        All = BGDecals | BGTiles | FGDecals | FGTiles | Entities | Triggers | OBTiles
    }
}
