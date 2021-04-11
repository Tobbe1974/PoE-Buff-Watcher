using PoE.BuffWatcher.Capture.Structs;

namespace PoE.BuffWatcher.Capture.Extensions
{
    public static class RectExtensions
    {
        public static bool IsZeroRect(this Rect rect)
        {
            return
                rect.Bottom == 0 &&
                rect.Left == 0 &&
                rect.Right == 0 &&
                rect.Top == 0;
        }
    }
}
