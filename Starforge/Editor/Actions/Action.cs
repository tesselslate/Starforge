using Microsoft.Xna.Framework;
using Starforge.MapStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starforge.Editor.Actions {

    // representing a user action, like placing/removing an entity or a tile
    abstract class Action {

        // the type of action
        private ToolType toolType;

        // the place of the action
        private Rectangle position;

        public Action() {

        }

        public abstract void Apply(Level l);
    }

}
