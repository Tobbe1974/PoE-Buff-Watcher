using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using PoE.BuffWatcher.Capture;
using PoE.BuffWatcher.Scanner;

namespace PoE.BuffWatcher.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            string windowName = "Path Of Exile";

            var windowHandler = new WindowHandler(windowName);
            var imageCapture = new ImageCapture(windowHandler);

            

            while (true)
            {
                Thread.Sleep(400);
                var image = imageCapture.GetBitmapFromGameWindow();

                ReadImage(image);
            }
        }

        private static void ReadImage(Image image)
        {
            var time = Stopwatch.StartNew();
            var imageScanner = new ImageScanner(image);
            var buffs = imageScanner.FindBuffs();
            var debuffs = imageScanner.FindDebuffs();

            time.Stop();

            System.Console.WriteLine($"Found {buffs.Count} buffs and {debuffs.Count} in {time.ElapsedMilliseconds}");
        }
    }
}
