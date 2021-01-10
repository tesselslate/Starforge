using SDL2;
using Starforge.Util;
using System;

namespace Starforge.Core.Boot {
    public class TaskLoadCursors : BootTask {
        public override void Run() {
            foreach(SDL.SDL_SystemCursor cursor in Enum.GetValues(typeof(SDL.SDL_SystemCursor))) {
                UIHelper.Cursors[cursor] = SDL.SDL_CreateSystemCursor(cursor);
            }

            base.Run();
        }
    }
}
