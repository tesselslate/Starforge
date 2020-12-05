using System;
using System.ComponentModel;

namespace Starforge.Core {
    public static partial class Starforge {
        private static void AppDomain_HandleException(object sender, UnhandledExceptionEventArgs e) {
            Logger.Log(LogLevel.Error, $"The main thread encountered a{(e.IsTerminating ? " fatal" : "n ")} unhandled exception.");
            Logger.LogException((Exception)e.ExceptionObject);
        }

        private static void Eto_HandleException(object sender, Eto.UnhandledExceptionEventArgs e) {
            Logger.Log(LogLevel.Error, $"Eto encountered a{(e.IsTerminating ? " fatal" : "n ")} unhandled exception.");
            Logger.LogException((Exception)e.ExceptionObject);
        }

        private static void Eto_Terminating(object sender, CancelEventArgs e) {
            Logger.Log("Eto (GUI) thread has ended.");
        }
    }
}
