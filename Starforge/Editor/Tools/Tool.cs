using Microsoft.Xna.Framework.Input;
using Starforge.Core.Input;

namespace Starforge.Editor.Tools {
    public abstract class Tool {

        public abstract void ManageInput(MouseEvent m);

        public abstract void Render();

        public abstract string getName();
        
    }
}
