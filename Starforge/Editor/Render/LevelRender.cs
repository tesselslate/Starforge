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

        private RenderTarget2D BGTiles;
        private RenderTarget2D BGDecals;
        private RenderTarget2D FGTiles;
        private RenderTarget2D FGDecals;
        private RenderTarget2D Entities;
        private RenderTarget2D Triggers;
        public RenderTarget2D Overlay { get; private set; }

        /// <summary>
        /// Creates a new LevelRender instance for rendering the level.
        /// </summary>
        /// <param name="level">The level to render.</param>
        /// <param name="prerenderAll">Whether or not all rooms should be rendered before displaying the map.</param>
        public LevelRender(MapEditor editor, Level level, bool prerenderAll = false) {
            // Create room render targets
            TargetUsage = Settings.AlwaysRerender ? RenderTargetUsage.DiscardContents : RenderTargetUsage.PreserveContents;
            Rooms = new List<DrawableRoom>();
            Editor = editor;
            Level = level;

            foreach (Room room in Level.Rooms) {
                Rooms.Add(new DrawableRoom(room, TargetUsage));
            }

            if (prerenderAll) {
                foreach (DrawableRoom room in Rooms) RenderRoom(room);
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
                if (room != SelectedRoom) Engine.Batch.Draw(room.Target, new Vector2(room.Room.X, room.Room.Y), Color.White);
            }

            // Render selected room
            if (SelectedRoom != null) {
                GFX.Draw.Rectangle(SelectedRoom.Room.Meta.Bounds, Settings.SelectedRoomColor);
                if (Menubar.View_BGTiles) Engine.Batch.Draw(BGTiles, new Vector2(SelectedRoom.Room.X, SelectedRoom.Room.Y), Color.White);
                if (Menubar.View_BGDecals) Engine.Batch.Draw(BGDecals, new Vector2(SelectedRoom.Room.X, SelectedRoom.Room.Y), Color.White);
                if (Menubar.View_Entities) Engine.Batch.Draw(Entities, new Vector2(SelectedRoom.Room.X, SelectedRoom.Room.Y), Color.White);
                if (Menubar.View_FGTiles) Engine.Batch.Draw(FGTiles, new Vector2(SelectedRoom.Room.X, SelectedRoom.Room.Y), Color.White);
                if (Menubar.View_FGDecals) Engine.Batch.Draw(FGDecals, new Vector2(SelectedRoom.Room.X, SelectedRoom.Room.Y), Color.White);
                if (Menubar.View_Triggers) Engine.Batch.Draw(Triggers, new Vector2(SelectedRoom.Room.X, SelectedRoom.Room.Y), Color.White);
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
            room.BGTiles = Editor.BGAutotiler.GenerateTextureMap(room.Room.BackgroundTiles, true);
            room.FGTiles = Editor.FGAutotiler.GenerateTextureMap(room.Room.ForegroundTiles, true);

            // Generate object tiles
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

            Engine.Instance.GraphicsDevice.SetRenderTarget(room.Target);
            Engine.Instance.GraphicsDevice.Clear(Settings.RoomColor);
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

            // Entities
            if (flags.HasFlag(RenderFlags.Entities)) foreach (Entity e in room.Room.Entities) e.Render();

            // Foreground tiles
            if (flags.HasFlag(RenderFlags.FGTiles)) room.FGTiles.Draw();

            // Object tiles
            if (flags.HasFlag(RenderFlags.FGTiles)) foreach (StaticTexture t in room.OBTiles) t.Draw();

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
        }

        /// <summary>
        /// Renders the currently selected room.
        /// </summary>
        /// <param name="render">The layers to rerender.</param>
        public void RenderSelectedRoom(RenderFlags render) {
            if (SelectedRoom == null) return;

            if (render.HasFlag(RenderFlags.BGDecals)) {
                SetSubtarget(BGDecals);
                SelectedRoom.BGDecals = CreateDecalTextureList(SelectedRoom.Room.BackgroundDecals);
                foreach (StaticTexture t in SelectedRoom.BGDecals) t.DrawCentered();
                Engine.Batch.End();
            }

            if (render.HasFlag(RenderFlags.BGTiles)) {
                SetSubtarget(BGTiles);
                SelectedRoom.BGTiles.Draw();
                Engine.Batch.End();
            }

            if (render.HasFlag(RenderFlags.FGDecals)) {
                SetSubtarget(FGDecals);
                SelectedRoom.FGDecals = CreateDecalTextureList(SelectedRoom.Room.ForegroundDecals);
                foreach (StaticTexture t in SelectedRoom.FGDecals) t.DrawCentered();
                Engine.Batch.End();
            }

            if (render.HasFlag(RenderFlags.FGTiles)) {
                SetSubtarget(FGTiles);
                SelectedRoom.FGTiles.Draw();
                foreach (StaticTexture t in SelectedRoom.OBTiles) t.Draw();
                Engine.Batch.End();
            }

            if (render.HasFlag(RenderFlags.Entities)) {
                SetSubtarget(Entities);
                foreach (Entity e in SelectedRoom.Room.Entities) e.Render();
                Engine.Batch.End();
            }

            if (render.HasFlag(RenderFlags.Triggers)) {
                SetSubtarget(Triggers);
                foreach (Entity t in SelectedRoom.Room.Triggers) {
                    Rectangle tr = new Rectangle((int)t.Position.X, (int)t.Position.Y, (int)t.Attributes["width"], (int)t.Attributes["height"]);
                    GFX.Draw.BorderedRectangle(tr, Settings.TriggerColor * 0.4f, Settings.TriggerColor);
                    GFX.Draw.TextCentered(MiscHelper.CleanCamelCase(t.Name), tr, Color.White);
                }
                Engine.Batch.End();
            }

            Engine.Instance.GraphicsDevice.SetRenderTarget(Overlay);
            Engine.Instance.GraphicsDevice.Clear(Color.Transparent);
        }

        /// <summary>
        /// Sets the currently selected room.
        /// </summary>
        public void SetSelected(Room room) {
            foreach (DrawableRoom drawable in Rooms) {
                if (drawable.Room == room) {
                    // Dispose of old render targets
                    if (BGTiles != null) {
                        BGTiles.Dispose();
                        BGDecals.Dispose();
                        FGTiles.Dispose();
                        FGDecals.Dispose();
                        Entities.Dispose();
                        Triggers.Dispose();
                        Overlay.Dispose();
                    }

                    // Select room
                    SelectedRoom = drawable;

                    BGTiles = new RenderTarget2D(Engine.Instance.GraphicsDevice, room.Width, room.Height, false, SurfaceFormat.Color, DepthFormat.None, 0, TargetUsage);
                    BGDecals = new RenderTarget2D(Engine.Instance.GraphicsDevice, room.Width, room.Height, false, SurfaceFormat.Color, DepthFormat.None, 0, TargetUsage);
                    FGTiles = new RenderTarget2D(Engine.Instance.GraphicsDevice, room.Width, room.Height, false, SurfaceFormat.Color, DepthFormat.None, 0, TargetUsage);
                    FGDecals = new RenderTarget2D(Engine.Instance.GraphicsDevice, room.Width, room.Height, false, SurfaceFormat.Color, DepthFormat.None, 0, TargetUsage);
                    Entities = new RenderTarget2D(Engine.Instance.GraphicsDevice, room.Width, room.Height, false, SurfaceFormat.Color, DepthFormat.None, 0, TargetUsage);
                    Triggers = new RenderTarget2D(Engine.Instance.GraphicsDevice, room.Width, room.Height, false, SurfaceFormat.Color, DepthFormat.None, 0, TargetUsage);
                    Overlay = new RenderTarget2D(Engine.Instance.GraphicsDevice, room.Width, room.Height, false, SurfaceFormat.Color, DepthFormat.None, 0, TargetUsage);
                }
            }
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

        private void SetSubtarget(RenderTarget2D target) {
            Engine.Instance.GraphicsDevice.SetRenderTarget(target);
            Engine.Instance.GraphicsDevice.Clear(Color.Transparent);
            Engine.Batch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                DepthStencilState.None,
                RasterizerState.CullNone,
                null
            );
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
        BGDecals = 1,
        BGTiles = 2,
        FGDecals = 4,
        FGTiles = 8,
        Entities = 16,
        Triggers = 32,
        All = 63
    }
}
