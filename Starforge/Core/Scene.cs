using Microsoft.Xna.Framework;

namespace Starforge.Core {
    /// <summary>
    /// A Scene is used to represent a collection of UI and graphical elements, such as the map editor.
    /// </summary>
    public abstract class Scene {
        /// <summary>
        /// Called when the Scene is first created.
        /// </summary>
        public abstract void Begin();

        /// <summary>
        /// Called before the Scene is destroyed. This is where cleanup tasks should go.
        /// </summary>
        /// <returns>Whether or not the Scene should be ended.</returns>
        public abstract bool End();

        /// <summary>
        /// Called every frame when the Scene is focused to do drawing tasks.
        /// </summary>
        public abstract void Render(GameTime gt);

        /// <summary>
        /// Called every frame before Render.
        /// </summary>
        public abstract void Update(GameTime gt);
    }
}
