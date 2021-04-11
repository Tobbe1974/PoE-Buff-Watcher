using System.Runtime.InteropServices;

namespace PoE.BuffWatcher.Capture.Structs
{

    [StructLayout(LayoutKind.Sequential)]
    public struct Rect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }
}
