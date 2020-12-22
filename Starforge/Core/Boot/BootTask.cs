using System.Threading.Tasks;

namespace Starforge.Core.Boot {
    public class BootTask {
        public Task Task;

        public virtual void Run() {
            StartupHelper.Finish(this);
        }
    }
}
