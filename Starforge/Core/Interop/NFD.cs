using System;
using System.Runtime.InteropServices;
using System.Text;

// This wrapper is heavily based off of the nfd-sharp project.
// It can be found here: https://github.com/benklett/nfd-sharp/blob/master/NfdSharp/Nfd.cs
//                       https://github.com/benklett/nfd-sharp/blob/master/NfdSharp/Utils.cs

namespace Starforge.Core.Interop {
    public static class NFD {
        [DllImport("nfd_d", CallingConvention = CallingConvention.Cdecl)]
        private static extern NfdResult NFD_OpenDialog(
            IntPtr filterList,
            IntPtr defaultPath,
            out IntPtr outPath);

        [DllImport("nfd_d", CallingConvention = CallingConvention.Cdecl)]
        private static extern NfdResult NFD_SaveDialog(
            IntPtr filterList,
            IntPtr defaultPath,
            out IntPtr outPath);

        [DllImport("nfd_d", CallingConvention = CallingConvention.Cdecl)]
        private static extern NfdResult NFD_PickFolder(
            IntPtr defaultPath,
            out IntPtr outPath);

        [DllImport("nfd_d", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr NFD_GetError();

        public static NfdResult OpenDialog(string filterList, string defaultPath, out string path) {
            IntPtr filterListPtr = NFDParser.ToNfdString(filterList);
            IntPtr defaultPathPtr = NFDParser.ToNfdString(defaultPath);

            NfdResult res = NFD_OpenDialog(filterListPtr, defaultPathPtr, out IntPtr outPath);
            Marshal.FreeHGlobal(filterListPtr);
            Marshal.FreeHGlobal(defaultPathPtr);

            path = res != NfdResult.OKAY ? null : NFDParser.FromNfdString(outPath);

            // Here, we *should* free the outPath pointer.
            // However, doing so causes a crash!
            // Does not doing so cause a memory leak? Probably. :)

            if (res == NfdResult.ERROR) {
                Logger.Log(LogLevel.Error, "nativefiledialog error:");
                Logger.Log(LogLevel.Error, GetError());
            }

            return res;
        }

        public static NfdResult SaveDialog(string filterList, string defaultPath, out string path) {
            IntPtr filterListPtr = NFDParser.ToNfdString(filterList);
            IntPtr defaultPathPtr = NFDParser.ToNfdString(defaultPath);

            NfdResult res = NFD_SaveDialog(filterListPtr, defaultPathPtr, out IntPtr outPath);
            Marshal.FreeHGlobal(filterListPtr);
            Marshal.FreeHGlobal(defaultPathPtr);

            path = res != NfdResult.OKAY ? null : NFDParser.FromNfdString(outPath);

            // The outPath pointer should also probably be freed here.

            if (res == NfdResult.ERROR) {
                Logger.Log(LogLevel.Error, "nativefiledialog error:");
                Logger.Log(LogLevel.Error, GetError());
            }

            return res;
        }

        public static NfdResult PickFolder(string defaultPath, out string path) {
            IntPtr defaultPathPtr = NFDParser.ToNfdString(defaultPath);

            NfdResult res = NFD_PickFolder(defaultPathPtr, out IntPtr outPath);
            Marshal.FreeHGlobal(defaultPathPtr);

            path = res != NfdResult.OKAY ? null : NFDParser.FromNfdString(outPath);

            if (res == NfdResult.ERROR) {
                Logger.Log(LogLevel.Error, "nativefiledialog error:");
                Logger.Log(LogLevel.Error, GetError());
            }

            return res;
        }

        public static string GetError() {
            return NFDParser.FromNfdString(NFD_GetError());
        }
    }

    public static class NFDParser {
        public static IntPtr ToNfdString(string str) {
            if (str == "") return IntPtr.Zero;

            byte[] bytes = Encoding.UTF8.GetBytes(str);
            byte[] nullTerm = new byte[bytes.Length + 1];
            Array.Copy(bytes, nullTerm, bytes.Length);
            nullTerm[nullTerm.Length - 1] = 0; // Null terminate string

            // Create IntPtr for null-terminated string
            IntPtr ptr = Marshal.AllocHGlobal(nullTerm.Length);
            Marshal.Copy(nullTerm, 0, ptr, nullTerm.Length);

            return ptr;
        }

        public static string FromNfdString(IntPtr ptr) {
            byte[] bytes = new byte[4096];

            // Copy bytes from pointer
            Marshal.Copy(ptr, bytes, 0, 1);
            int i = 0;

            // While string is not null terminated, copy bytes
            while (i < 4095 && bytes[i++] != 0) {
                Marshal.Copy(ptr, bytes, 0, i + 1);
            }

            // Decrement index to get string length
            i--;

            byte[] res = new byte[i];
            Array.Copy(bytes, res, i);

            return Encoding.UTF8.GetString(res);
        }
    }

    public enum NfdResult {
        ERROR,
        OKAY,
        CANCEL
    }
}
