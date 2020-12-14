using Microsoft.Xna.Framework;
using Starforge.Core;
using Starforge.MapStructure;
using Starforge.Mod;

namespace Starforge.Editor.Actions {

    class EntityPlacement : Action {

        private Entity ChangedEntity;

        private string Name;
        private Point Position;

        public EntityPlacement(Level l, string name, Point position)
            : base(l) {
            Name = name;
            Position = position;
        }

        public override bool Apply() {
            if (ChangedEntity != null) {
                return false;
            }
            ChangedEntity = MakeEntityAtPosition(Name, Position);
            Level.Entities.Add(ChangedEntity);
            return true;
        }

        public override bool Undo() {
            if (ChangedEntity == null) {
                return false;
            }
            Level.Entities.Remove(ChangedEntity);
            ChangedEntity = null;
            return true;
        }

        private Entity MakeEntityAtPosition(string name, Point p) {
            return EntityRegistry.Create(
                Engine.Scene.SelectedLevel,
                name,
                new Vector2(p.X * 8f, p.Y * 8f));
        }
    }

}
