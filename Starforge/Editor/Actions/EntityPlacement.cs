using Microsoft.Xna.Framework;
using Starforge.Core;
using Starforge.MapStructure;
using Starforge.Mod;

namespace Starforge.Editor.Actions {

    class EntityPlacement : Action {

        private Entity ChangedEntity;

        private string Name;
        private Rectangle Area;

        public EntityPlacement(Level l, string name, Rectangle rect)
            : base(l) {
            Name = name;
            Area = rect;
        }

        public override bool Apply() {
            if (ChangedEntity != null) {
                return false;
            }
            ChangedEntity = MakeEntityAtPosition(Name, Area);
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

        private Entity MakeEntityAtPosition(string name, Rectangle rect) {
            return EntityRegistry.Create(
                Engine.Scene.SelectedLevel,
                name,
                rect);
        }
    }

}
