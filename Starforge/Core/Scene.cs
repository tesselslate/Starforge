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
        public abstract void End();

        /// <summary>
        /// Called when the user switches to the Scene from another Scene.
        /// </summary>
        public abstract void Focus();

        /// <summary>
        /// Called when the user switches away from the Scene to another Scene.
        /// </summary>
        public abstract void Unfocus();

        /// <summary>
        /// Called every frame when the Scene is focused to do drawing tasks.
        /// </summary>
        public abstract void Render();

        /// <summary>
        /// Called every frame before Render. This should be used to respond to inputs and whatnot.
        /// </summary>
        public abstract void Update();
    }
}
