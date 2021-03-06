using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace PoE.BuffWatcher.Capture
{
    public class ImageScanner : IDisposable
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

        private static int LineLength = 5;
        private static int ColorTolerance = 4;

        private readonly Bitmap _bitmap;
        
        public ImageScanner(Image image)
        {
            _bitmap = new Bitmap(image);
        }

        public Image FindBuffs()
        {
            var yPos = BuffStartY + ArcPixels + SkipPixels;
            var rowData = GetRowData(yPos);
            var rightBorders = FindRightBorders(rowData);

            if (rightBorders.Count == 0)
            {
                return null;
            }

            var rects = CalculateRectangles(BuffStartY, rightBorders);
            return BarCreator.CreateBuffImage(_bitmap, rects);
        }

        public Image FindDebuffs()
        {
            var yPos = DebuffStartY + ArcPixels + SkipPixels;
            var rowData = GetRowData(yPos);
            var rightBorders = FindRightBorders(rowData);

            if (rightBorders.Count == 0)
            {
                return null;
            }

            var rects = CalculateRectangles(DebuffStartY, rightBorders);
            return BarCreator.CreateBuffImage(_bitmap, rects);
        }


        private unsafe Span<uint> GetRowData(int yPos)
        {
            var data = _bitmap.LockBits(new Rectangle(0, yPos, _bitmap.Width, 1), ImageLockMode.ReadOnly, _bitmap.PixelFormat);
            var rowData = new Span<uint>(data.Scan0.ToPointer(), data.Width);
            _bitmap.UnlockBits(data);

            return rowData;
        }


        private List<int> FindRightBorders(Span<uint> rowData)
        {
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

            return diff < ColorTolerance;
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

        public void Dispose()
        {
            _bitmap?.Dispose();
        }
    }
}
