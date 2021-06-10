using Microsoft.Xna.Framework;
using Starforge.Map;
using Starforge.Mod.API;
using Starforge.Mod.API.Properties;
using Starforge.Mod.Content;
using Starforge.Util;
using System;
using System.Collections.Generic;

namespace Starforge.Entities {
    [EntityDefinition("zipMover")]
    public class ZipMover : Entity {
        private static Color RopeColor = MiscHelper.HexToColor("663931");

        public ZipMover(EntityData data, Room room) : base(data, room) { }

        public override bool StretchableX => true;
        public override bool StretchableY => true;

        public override void Render() {
            ZipMoverTheme theme = Themes.Value[GetString("theme", "Normal")];

            // TODO: Use something like a OnPlaced() callback instead
            if (Nodes == null || Nodes.Count == 0 || !Room.Entities.Contains(this)) {
                Nodes = new List<Vector2> { Position + new Vector2(Width + 8f, 0) };
            }

            #region path
            Vector2 center = new Vector2(Width / 2f, Height / 2f);
            Vector2 pathBegin = Position + center;
            Vector2 pathEnd = Nodes[0] + new Vector2(8f, 8f);

            Vector2 rotation = pathEnd - pathBegin;
            if (rotation != Vector2.Zero) {
                rotation.Normalize();
            }
            
            Vector2 offset = rotation.Perpendicular() * 3f;
            Vector2 offset2 = -rotation.Perpendicular() * 4f;
            GFX.Draw.Line(pathBegin + offset, pathEnd + offset, RopeColor);
            GFX.Draw.Line(pathBegin + offset2, pathEnd + offset2, RopeColor);
            // draw the cog at the end of the path
            theme.Cog.DrawCentered(pathEnd);
            #endregion

            // background
            GFX.Draw.Rectangle((int)Position.X + 1, (int)Position.Y + 1, Width - 2, Height - 2, Color.Black);
            // block
            // this approach is more optimal than the vanilla method for larger zippers, as the middle segments aren't enumerated through
            // corners
            theme.BlockEdges[0, 0].Draw(Position);
            theme.BlockEdges[2, 0].Draw(new Vector2(Position.X + Width - 8, Position.Y));
            theme.BlockEdges[0, 2].Draw(new Vector2(Position.X, Position.Y + Height - 8));
            theme.BlockEdges[2, 2].Draw(new Vector2(Position.X + Width - 8, Position.Y + Height - 8));

            // bottom and top
            if (!theme.OptimiseRendering) {
                // This theme's sprites are a bit more complicated, scaling them looks ugly
                Vector2 topPosition = new Vector2(Position.X + 8, Position.Y);
                Vector2 bottomPosition = new Vector2(Position.X, Position.Y + Height - 8);

                for (int i = 1; i < (Width / 8) - 1; i++) {
                    topPosition.X = bottomPosition.X += 8;
                    theme.BlockEdges[1, 0].Draw(topPosition);
                    theme.BlockEdges[1, 2].Draw(bottomPosition);
                }
            }
            else {
                theme.BlockEdges[1, 0].Draw(new Rectangle((int)Position.X + 8, (int)Position.Y, Width - 16, 8));
                theme.BlockEdges[1, 2].Draw(new Rectangle((int)Position.X + 8, (int)Position.Y + Height - 8, Width - 16, 8));
            }
            // left and right
            // we can just draw the sprite once with the correct scale in this case with no loss in quality
            theme.BlockEdges[0, 1].Draw(new Rectangle((int)Position.X, (int)Position.Y + 8, 8, Height - 16));
            theme.BlockEdges[2, 1].Draw(new Rectangle((int)Position.X + Width - 8, (int)Position.Y + 8, 8, Height - 16));

            // streetlight
            theme.Light.Draw(new Vector2(Width / 2f - theme.Light.Width / 2f + Position.X, Position.Y));
        }

        /// <summary> Splits a 24x24 texture into a 3x3 grid of 8x8 textures </summary>
        private static DrawableTexture[,] GetEdges(DrawableTexture baseTexture) {
            DrawableTexture[,] edges = new DrawableTexture[3, 3];
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    edges[i, j] = new DrawableTexture(baseTexture, i * 8, j * 8, 8, 8);

            return edges;
        }

        private static Lazy<Dictionary<string, ZipMoverTheme>> Themes = new Lazy<Dictionary<string, ZipMoverTheme>>(() => new Dictionary<string, ZipMoverTheme>() {
            ["Normal"] = new ZipMoverTheme() {
                Light = GFX.Gameplay["objects/zipmover/light01"],
                BlockEdges = GetEdges(GFX.Gameplay["objects/zipmover/block"]),
                Cog = GFX.Gameplay["objects/zipmover/cog"],
                OptimiseRendering = true
            },
            ["Moon"] = new ZipMoverTheme() {
                Light = GFX.Gameplay["objects/zipmover/moon/light01"],
                BlockEdges = GetEdges(GFX.Gameplay["objects/zipmover/moon/block"]),
                Cog = GFX.Gameplay["objects/zipmover/moon/cog"],
            }
        });

        public struct ZipMoverTheme {
            public DrawableTexture Light;
            public DrawableTexture[,] BlockEdges;
            public DrawableTexture Cog;
            public bool OptimiseRendering; // Whether an optimised rendering method involving sprite scaling should be used
        }

        public override PropertyList Properties => new PropertyList() {
            new ListProperty("theme", new string[] { "Normal", "Moon" }, false, "Normal", "The theme this Zip Mover should use")
        };

        public static PlacementList Placements = new PlacementList() {
            new Placement("Zip Mover"),
            new Placement("Zip Mover (Moon)") {
                ["theme"] = "Moon"
            }
        };
    }
}
