using Starforge.Editor;
using Starforge.Editor.Render;
using Starforge.Map;
using Starforge.Mod.API;

namespace Starforge.Vanilla.Entities {
    [EntityDefinition("dashBlock")]
    class DashBlock : Entity {
        public DashBlock(EntityData data, Room room) : base(data, room) { }

        public override bool StretchableX => true;
        public override bool StretchableY => true;

        public override void Render() {
            TextureMap map = MapEditor.Instance.FGAutotiler.GenerateFakeTileMap(Room, Position, Width / 8, Height / 8, (short)GetChar("tiletype", '3')); ;
            for (int i = 0; i < map.Textures.Length; i++) map.Textures[i].Position += Position;
            map.Draw();
        }

        public static PlacementList Placements = new PlacementList()
        {
            new Placement("Dash Block")
            {
                ["tiletype"] = "3"
            }
        };
    }
}
