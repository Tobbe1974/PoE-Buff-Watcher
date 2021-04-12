using System;
using System.Collections.Generic;
using System.Text;
using PoE.BuffWatcher.Imports;

namespace PoE.BuffWatcher.Capture
{
    public class WindowHandler
    {
        private readonly List<string> _possibleNames;
        private IntPtr _foundWindow = IntPtr.Zero;

        public WindowHandler(string windowName)
        {
            _possibleNames = new List<string>(new[] { windowName });
        }

        public WindowHandler(IEnumerable<string> windowNames)
        {
            _possibleNames = new List<string>(windowNames);
        }

        public IntPtr GetGameWindowDC()
        {
            return User32.GetWindowDC(GetGameWindow());
        }

        public Rect GetWindowRect()
        {
            var hWnd = GetGameWindow();
            if (hWnd == IntPtr.Zero)
            {
                return new Rect
                {
                    Bottom = 0,
                    Left = 0,
                    Right = 0,
                    Top = 0,
                };
            }

            var rect = new Rect();
            User32.GetWindowRect(hWnd, ref rect);

            return rect;
        }

        public IntPtr GetGameWindow()
        {
            if (_foundWindow == IntPtr.Zero)
            {
                var openWindows = GetOpenWindows();

                foreach (var possibleName in _possibleNames)
                {
                    var searchName = possibleName.ToLowerInvariant();
                    if (openWindows.Keys.Contains(searchName))
                    {
                        _foundWindow = openWindows[searchName];
                        break;
                    }
                }
            }

            return _foundWindow;
        }


        private static IDictionary<string, IntPtr> GetOpenWindows()
        {
            IntPtr shellWindow = User32.GetShellWindow();
            Dictionary<string, IntPtr> windows = new Dictionary<string, IntPtr>();

            User32.EnumWindows(delegate (IntPtr hWnd, int lParam)
            {
                if (hWnd == shellWindow) return true;
                if (!User32.IsWindowVisible(hWnd)) return true;

                int length = User32.GetWindowTextLength(hWnd);
                if (length == 0) return true;

                StringBuilder builder = new StringBuilder(length);
                User32.GetWindowText(hWnd, builder, length + 1);

                windows[builder.ToString().ToLowerInvariant()] = hWnd;
                return true;

            }, 0);

            return windows;
        }

        public void ResetWindow()
        {
            _foundWindow = IntPtr.Zero;;
        }
    }
}
