using System;
using System.Drawing;
using System.Drawing.Imaging;
using PoE.BuffWatcher.Imports;

namespace PoE.BuffWatcher.Capture
{
    public class ImageCapture
    {
        private readonly WindowHandler _windowHandler;

        public ImageCapture(WindowHandler windowHandler)
        {
            _windowHandler = windowHandler;
        }

        public Image GetBitmapFromGameWindow()
        {
            var gameWindow = _windowHandler.GetGameWindow();
            if (gameWindow == IntPtr.Zero)
            {
                return null;
            }

            var gameWindowDc = _windowHandler.GetGameWindowDC();
            var windowRect = _windowHandler.GetWindowRect();
            int width = windowRect.Right - windowRect.Left;
            int height = windowRect.Bottom - windowRect.Top;

            IntPtr hdcDest = GDI32.CreateCompatibleDC(gameWindowDc);
            IntPtr hBitmap = GDI32.CreateCompatibleBitmap(gameWindowDc, width, height);
            IntPtr hOld = GDI32.SelectObject(hdcDest, hBitmap);
            GDI32.BitBlt(hdcDest, 0, 0, width, height, gameWindowDc, 0, 0, GDI32.SRCCOPY);
            GDI32.SelectObject(hdcDest, hOld);
            GDI32.DeleteDC(hdcDest);
            User32.ReleaseDC(gameWindow, gameWindowDc);

            Image image = Image.FromHbitmap(hBitmap);
            GDI32.DeleteObject(hBitmap);

            return image;
        }

        public void SaveScreenShot(string fileName, ImageFormat format = null)
        {
            format ??= ImageFormat.Bmp;

            var image = GetBitmapFromGameWindow();
            if (image == null)
            {
                Console.WriteLine("Unable to find game window");
                return;
            }

            image.Save(fileName, format);
        }

        public Image LoadScreenShot(string fileName)
        {
            var image = Image.FromFile(fileName);

            return image;
        }

    }
}

