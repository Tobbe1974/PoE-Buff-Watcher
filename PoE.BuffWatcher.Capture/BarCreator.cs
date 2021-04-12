using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;

namespace PoE.BuffWatcher.Capture
{
    public class BarCreator
    {
        private static int Spacing = 5;
        public static Image CreateBuffImage(Bitmap sourceImage, List<Rectangle> buffs)
        {
            Color transparentColor = Color.FromArgb(255, Color.LawnGreen);

            var height = buffs.Max(b => b.Height);
            var width = buffs.Sum(b => b.Width + Spacing) - Spacing;

            var destinationImage = new Bitmap(width, height + 1);

            using Graphics gDestination = Graphics.FromImage(destinationImage);

            gDestination.Clear(transparentColor);

            int xPos = 1;
            foreach (var rectangle in buffs)
            {
                using var buffBitmap = GetBuffBitmap(sourceImage, rectangle);
                gDestination.DrawImage(buffBitmap, new Point(xPos, 0));

                xPos += rectangle.Width + Spacing;
            }

            destinationImage.MakeTransparent(transparentColor);

            int newWidth = Convert.ToInt32(destinationImage.Width * 0.4);
            int newHeight = Convert.ToInt32(destinationImage.Height * 0.4);

            var bitmap = ResizeImage(destinationImage, newWidth, newHeight);
            destinationImage.Dispose();

            return bitmap;
        }

        private static Bitmap GetBuffBitmap(Bitmap sourceImage, Rectangle rectangle)
        {
            var sourceData = sourceImage.LockBits(rectangle, ImageLockMode.ReadOnly, sourceImage.PixelFormat);
            Bitmap bitmap = new Bitmap(
                sourceData.Width,
                sourceData.Height,
                sourceData.Stride,
                sourceData.PixelFormat,
                sourceData.Scan0);

            sourceImage.UnlockBits(sourceData);

            return bitmap;
        }

        private static Bitmap ResizeImage(Bitmap bitmap, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(bitmap.HorizontalResolution, bitmap.VerticalResolution);

            using var graphics = Graphics.FromImage(destImage);

            graphics.CompositingMode = CompositingMode.SourceCopy;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            using var wrapMode = new ImageAttributes();
            wrapMode.SetWrapMode(WrapMode.TileFlipXY);
            graphics.DrawImage(bitmap, destRect, 0, 0, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel, wrapMode);

            return destImage;
        }
    }
}
