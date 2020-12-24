using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Starforge.Core;
using Starforge.Map;
using System.Collections.Generic;

namespace Starforge.Editor.Render {
    public class LevelRender {
        /// <summary>
        /// The RenderTargetUsage to use when drawing rooms.
        /// </summary>
        private RenderTargetUsage TargetUsage;

        /// <summary>
        /// The list of RenderTargets for each room in the map.
        /// </summary>
        private Dictionary<string, DrawableRoom> Rooms;

        /// <summary>
        /// The list of rooms which are visible onscreen.
        /// </summary>
        private List<DrawableRoom> VisibleRooms;

        /// <summary>
        /// The currently loaded level.
        /// </summary>
        private Level Level;

        /// <summary>
        /// The parent map editor.
        /// </summary>
        private MapEditor Editor;

        /// <summary>
        /// Creates a new LevelRender instance for rendering the level.
        /// </summary>
        /// <param name="level">The level to render.</param>
        /// <param name="prerenderAll">Whether or not all rooms should be rendered before displaying the map.</param>
        public LevelRender(MapEditor editor, Level level, bool prerenderAll = false) {
            // Create room render targets
            TargetUsage = Settings.AlwaysRerender ? RenderTargetUsage.DiscardContents : RenderTargetUsage.PreserveContents;
            Rooms = new Dictionary<string, DrawableRoom>();
            Editor = editor;
            Level = level;

            foreach (Room room in Level.Rooms) {
                Rooms.Add(room.Name, new DrawableRoom(room, TargetUsage));
            }

            if (prerenderAll) {
                foreach (DrawableRoom room in Rooms.Values) RenderRoom(room);
            }
        }

        /// <summary>
        /// Renders the entire level.
        /// </summary>
        public void Render() {
            Engine.Instance.GraphicsDevice.SetRenderTarget(null);
            Engine.Instance.GraphicsDevice.Clear(Settings.BackgroundColor);
            Engine.Batch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                DepthStencilState.None,
                RasterizerState.CullNone,
                null,
                Editor.Camera.Transform
            );

            foreach(DrawableRoom room in Rooms.Values) {
                Engine.Batch.Draw(room.Target, new Vector2(room.Room.X, room.Room.Y), Color.White);
            }

            Engine.Batch.End();
        }

        /// <summary>
        /// Renders a room.
        /// </summary>
        /// <param name="room">The room to render.</param>
        /// <param name="target">The target to render the room to.</param>
        public void RenderRoom(DrawableRoom room) {
            if (!room.TilemapsGenerated) {
                // Generate tile texturemaps
                room.BGTiles = Editor.BGAutotiler.GenerateTextureMap(room.Room.BackgroundTiles, true);
                room.FGTiles = Editor.FGAutotiler.GenerateTextureMap(room.Room.ForegroundTiles, true);
            }

            Engine.Instance.GraphicsDevice.SetRenderTarget(room.Target);
            Engine.Batch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                DepthStencilState.None,
                RasterizerState.CullNone,
                null
            );

            room.BGTiles.Draw();
            room.FGTiles.Draw();

            Engine.Batch.End();
        }
    }

    /// <summary>
    /// Contains the data for drawing a given room.
    /// </summary>
    public class DrawableRoom {
        public RenderTarget2D Target;
        public TextureMap BGTiles;
        public TextureMap FGTiles;
        public bool TilemapsGenerated;
        public Room Room;

        public DrawableRoom(Room room, RenderTargetUsage targetUsage) {
            Room = room;

            Target = new RenderTarget2D(Engine.Instance.GraphicsDevice, room.Width, room.Height, false, SurfaceFormat.Color, DepthFormat.None, 0, targetUsage);
            BGTiles = new TextureMap(room.Width / 8, room.Height / 8);
            FGTiles = new TextureMap(room.Width / 8, room.Height / 8);

            TilemapsGenerated = false;
        }
    }
}
