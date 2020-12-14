using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Starforge.MapStructure;

namespace Starforge.Editor.Tools {
    public abstract class Tool {

        public Tool() { }

        public abstract void ManageInput(MouseState m, Level l);

        public abstract void Render(RenderTarget2D target);
        
    }
}
