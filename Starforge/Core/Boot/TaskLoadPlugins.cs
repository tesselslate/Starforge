using System.IO;

namespace Starforge.Core.Boot {
    public class TaskLoadPlugins : BootTask {
        public override void Run() {
            // TODO: Add plugins folder in %localappdata%/Starforge/ to load other plugins from.
            // TODO: Load the Starforge.Vanilla plugin.
            base.Run();
        }
    }
}
