using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace PoE.BuffWatcher.Scanner
{
    public class ImageScanner
    {
        private const int BuffHeight = 84;
        private const int BuffWidth = 87;

        private const int BuffStartY = 9;
        private const int DebuffStartY = 122;

        // Number of pixels in the corners that arc
        private const int ArcPixels = 6;

        private const int SkipPixels = 10;

        private byte ScanColorA = 0xFF;
        private byte ScanColorR = 0x57;
        private byte ScanColorG = 0x41;
        private byte ScanColorB = 0x33;

        private readonly Bitmap _bitmap;

        public ImageScanner(Image image)
        {
            _bitmap = new Bitmap(image);
        }

        public List<Rectangle> FindBuffs()
        {
            var yPos = BuffStartY + ArcPixels + SkipPixels;
            var rowData = GetRowData(yPos);
            var rightBorders = FindRightBorders(rowData);

            return CalculateRectangles(BuffStartY, rightBorders);
        }

        public List<Rectangle> FindDebuffs()
        {
            var yPos = DebuffStartY + ArcPixels + SkipPixels;
            var rowData = GetRowData(yPos);
            var rightBorders = FindRightBorders(rowData);

            return CalculateRectangles(DebuffStartY, rightBorders);
        }


        private unsafe Span<uint> GetRowData(int yPos)
        {
            var time = Stopwatch.StartNew();
            var data = _bitmap.LockBits(new Rectangle(0, yPos, _bitmap.Width, 1), ImageLockMode.ReadOnly, _bitmap.PixelFormat);
            var rowData = new Span<uint>(data.Scan0.ToPointer(), data.Width);
            _bitmap.UnlockBits(data);

            time.Stop();
            System.Console.WriteLine($"GetRowData took {time.ElapsedMilliseconds}");
            return rowData;
        }


        private List<int> FindRightBorders(in Span<uint> rowData)
        {
            var time = Stopwatch.StartNew();
            int lastPosFound = BuffWidth;
            int abortThreshHold = BuffWidth * 2;
            List<int> rightBorders = new List<int>();

            for (int i = BuffWidth; i - lastPosFound < abortThreshHold && i < _bitmap.Width; i++)
            {
                if (ColorCheck(rowData[i]))
                {
                    rightBorders.Add(i);
                    i += BuffWidth;
                    lastPosFound = i;
                }
            }

            time.Stop();
            System.Console.WriteLine($"FindRightBorders took {time.ElapsedMilliseconds}");

            return rightBorders;
        }

        private bool ColorCheck(uint color)
        {
            var bytes = BitConverter.GetBytes(color);

            var diff =
                Math.Abs(bytes[3] - ScanColorA) +
                Math.Abs(bytes[2] - ScanColorR) +
                Math.Abs(bytes[1] - ScanColorG) +
                Math.Abs(bytes[0] - ScanColorB);

            return diff < 5;
        }

        private List<Rectangle> CalculateRectangles(int buffStartY, List<int> rightBorders)
        {
            return rightBorders.Select(rb =>
                new Rectangle(
                    rb - BuffWidth + 1,
                    buffStartY,
                    BuffWidth,
                    BuffHeight)
            ).ToList();
        }
    }
}
