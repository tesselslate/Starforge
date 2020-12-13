using Microsoft.Xna.Framework;
using Starforge.MapStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starforge.Editor.Actions {

    class TilePlacement : Action {

        private TileType tileType;

        private Rectangle area;

        public TilePlacement(TileType t, Rectangle a) {
            tileType = t;
            area = a;
        }

        public override void Apply(Level l) {
            throw new NotImplementedException();
        }
    }
}
