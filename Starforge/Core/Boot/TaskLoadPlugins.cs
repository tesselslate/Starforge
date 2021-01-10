using Starforge.Mod;

namespace Starforge.Core.Boot {
    public class TaskLoadPlugins : BootTask {
        public override void Run() {
            Loader.LoadPluginAssemblies();
            base.Run();
        }
    }
}
