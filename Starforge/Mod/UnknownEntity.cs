using Microsoft.Xna.Framework;
using Starforge.Map;
using Starforge.Mod.API;
using Starforge.Mod.Content;

namespace Starforge.Mod {
    public class UnknownEntity : Entity {
        private static readonly Color BGColor = Color.Cyan * 0.2f;
        private static readonly Color OutlineColor = Color.Cyan;

        public UnknownEntity(EntityData data, Room room) : base(data, room) { }

        public override void Render() {
            Rectangle renderPos = new Rectangle(
                (int)Position.X, 
                (int)Position.Y, 
                Attributes.ContainsKey("width") ? (int)Attributes["width"] : 4, 
                Attributes.ContainsKey("height") ? (int)Attributes["height"] : 4
            );
            GFX.Draw.BorderedRectangle(renderPos, BGColor, OutlineColor);
        }

        public override PropertyList Properties { get { return new PropertyList(); } }
    }
}
