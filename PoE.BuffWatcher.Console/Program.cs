using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using PoE.BuffWatcher.Capture;

namespace PoE.BuffWatcher.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            string windowName = "Path Of Exile";

            var windowHandler = new WindowHandler(windowName);
            var imageCapture = new ImageCapture(windowHandler);

            var image = imageCapture.GetBitmapFromGameWindow();

            var bars = ReadImage(image);

            bars.Item1?.Save(@"D:\poe\bars\buff.bmp", ImageFormat.Bmp);
            bars.Item2?.Save(@"D:\poe\bars\debuff.bmp", ImageFormat.Bmp);

            bars.Item1?.Dispose();
            bars.Item2?.Dispose();
        }

        private static (Image, Image) ReadImage(Image image)
        {
            var time = Stopwatch.StartNew();
            using var imageScanner = new ImageScanner(image);
            var buffs = imageScanner.FindBuffs();
            var debuffs = imageScanner.FindDebuffs();

            time.Stop();

            return (buffs, debuffs);
        }
    }
}
