using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Starforge.Editor.Tools {
    public abstract class Tool {

        public Tool() { }

        public abstract void ManageInput(MouseState m);

        public abstract void Render();

        public abstract string getName();
        
    }
}
