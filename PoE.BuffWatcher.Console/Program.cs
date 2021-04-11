using System;
using System.Threading;
using PoE.BuffWatcher.Capture;

namespace PoE_Buff_Watcher
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
                Thread.Sleep(500);
                string filename = $"D:\\PoE\\{DateTime.Now.Ticks}.bmp";

                imageCapture.SaveScreenShot(filename);
            }
        }
    }
}
