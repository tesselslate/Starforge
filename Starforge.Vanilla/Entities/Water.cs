using Microsoft.Xna.Framework;
using Starforge.Map;
using Starforge.Mod.API;
using Starforge.Mod.Content;

namespace Starforge.Vanilla.Entities {
    [EntityDefinition("water")]
    public class Water : Entity {
        public static readonly Color FillColor = Color.LightSkyBlue * 0.3f;
        public static readonly Color SurfaceColor = Color.LightSkyBlue * 0.8f;

        public override bool StretchableX => true;
        public override bool StretchableY => true;

        public Water(EntityData data, Room room) : base(data, room) { }

        public override void Render() {
            Rectangle renderPos = new Rectangle((int)Position.X, (int)Position.Y, GetInt("width", 8), GetInt("height", 8));

            GFX.Pixel.Draw(renderPos, FillColor);
            GFX.Draw.HollowRectangle(renderPos, SurfaceColor);
        }

        public static PlacementList Placements = new PlacementList()
        {
            new Placement("Water")
        };

    }
}
